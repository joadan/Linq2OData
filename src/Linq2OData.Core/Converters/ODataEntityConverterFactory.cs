using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Linq2OData.Core.Converters
{
	/// <summary>
	/// Intercepts deserialization of all <see cref="IODataEntitySet"/> entity types and
	/// deserializes them property-by-property from a <see cref="JsonNode"/>.
	/// This prevents System.Text.Json from eagerly building type metadata for the entire
	/// connected entity graph, eliminating slow first-request reflection on large models.
	/// Navigation properties are only deserialized when the JSON response actually contains them.
	/// Supersedes <see cref="ODataNavigationPropertyConverterFactory"/> for all OData versions.
	/// </summary>
	public class ODataEntityConverterFactory : JsonConverterFactory
	{
		public override bool CanConvert(Type typeToConvert) =>
			typeToConvert.IsClass &&
			!typeToConvert.IsAbstract &&
			typeof(IODataEntitySet).IsAssignableFrom(typeToConvert);

		public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
			(JsonConverter)Activator.CreateInstance(
				typeof(ODataEntityConverter<>).MakeGenericType(typeToConvert))!;
	}

	internal sealed class ODataEntityConverter<T> : JsonConverter<T> where T : class, IODataEntitySet, new()
	{
		private sealed record PropertyEntry(PropertyInfo Prop, string JsonName, bool IsNavProp);

		// Per-type caches built once via static initializer — one instance per closed generic type
		private static readonly PropertyEntry[] _properties = BuildPropertyCache();
		private static readonly Dictionary<string, Type> _derivedTypeMap = BuildDerivedTypeMap();

		private static PropertyEntry[] BuildPropertyCache()
		{
			var entries = new List<PropertyEntry>();
			foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (!prop.CanRead || !prop.CanWrite) continue;

				var isNavProp = IsEntityType(prop.PropertyType) || IsEntityList(prop.PropertyType);

				// Respect [JsonIgnore(Condition = Always)] on scalar properties; nav props are always included
				if (!isNavProp && prop.GetCustomAttribute<JsonIgnoreAttribute>()?.Condition == JsonIgnoreCondition.Always)
					continue;

				var jsonName = prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? prop.Name;
				entries.Add(new PropertyEntry(prop, jsonName, isNavProp));
			}
			return [.. entries];
		}

		/// <summary>
		/// Builds a discriminator-to-concrete-type map from <see cref="JsonDerivedTypeAttribute"/>s
		/// on the base entity type to support OData polymorphism via the @odata.type property.
		/// </summary>
		private static Dictionary<string, Type> BuildDerivedTypeMap()
		{
			var map = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
			foreach (var attr in typeof(T).GetCustomAttributes<JsonDerivedTypeAttribute>())
			{
				if (attr.TypeDiscriminator is string discriminator)
					map[discriminator] = attr.DerivedType;
			}
			return map;
		}

		public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.Null)
				return null;

			var node = JsonNode.Parse(ref reader);
			if (node is not JsonObject obj)
				return null;

			// Polymorphism: if this base type declares derived types, check the @odata.type discriminator
			if (_derivedTypeMap.Count > 0 &&
				obj.TryGetPropertyValue("@odata.type", out var discriminatorNode) &&
				discriminatorNode?.GetValue<string>() is string discriminator &&
				_derivedTypeMap.TryGetValue(discriminator, out var actualType) &&
				actualType != typeof(T))
			{
				// Deserialize as the concrete derived type — triggers its own ODataEntityConverter<TDerived>
				return (T?)obj.Deserialize(actualType, options);
			}

			var entity = new T();

			foreach (var (prop, jsonName, isNavProp) in _properties)
			{
				if (!obj.TryGetPropertyValue(jsonName, out var propNode) || propNode is null)
					continue;

				if (isNavProp)
				{
					// OData V2/V3: non-expanded nav props arrive as { "__deferred": { "uri": "..." } } — skip them
					if (propNode is JsonObject navObj && navObj.ContainsKey("__deferred"))
						continue;

					if (propNode.GetValueKind() == JsonValueKind.Null) continue;
					// Skip empty expanded collections — nothing to populate
					if (propNode is JsonArray { Count: 0 }) continue;
				}

				// For nav props: triggers this factory recursively for nested entity types (on-demand).
				// For scalars: uses STJ native handling + any registered value converters.
				prop.SetValue(entity, propNode.Deserialize(prop.PropertyType, options));
			}

			return entity;
		}

		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
		{
			// Nav props are excluded from the write path — entity types are not used in write payloads
			// (ODataInputBase derived types handle create/update operations)
			writer.WriteStartObject();
			foreach (var (prop, jsonName, isNavProp) in _properties)
			{
				if (isNavProp) continue;
				writer.WritePropertyName(jsonName);
				JsonSerializer.Serialize(writer, prop.GetValue(value), prop.PropertyType, options);
			}
			writer.WriteEndObject();
		}

		private static bool IsEntityType(Type type) =>
			type.IsClass && typeof(IODataEntitySet).IsAssignableFrom(type);

		private static bool IsEntityList(Type type) =>
			type.IsGenericType &&
			type.GetGenericTypeDefinition() == typeof(List<>) &&
			typeof(IODataEntitySet).IsAssignableFrom(type.GetGenericArguments()[0]);
	}
}
