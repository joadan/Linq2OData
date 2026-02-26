using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Linq2OData.Core.Converters
{
	/// <summary>
	/// Intercepts deserialization of all OData entity types decorated with <see cref="ODataEntityAttribute"/>
	/// (or <see cref="ODataEntitySetAttribute"/>) and deserializes them property-by-property from a
	/// <see cref="JsonNode"/>.
	/// This prevents System.Text.Json from eagerly building type metadata for the entire
	/// connected entity graph, eliminating slow first-request reflection on large models.
	/// Navigation properties are only deserialized when the JSON response actually contains them.
	/// Supersedes <see cref="ODataNavigationPropertyConverterFactory"/> for all OData versions.
	/// 
	/// Handles OData polymorphism by reading @odata.type discriminators and instantiating the correct
	/// derived type, completely replacing STJ's [JsonPolymorphic] system to ensure compatibility with
	/// custom deserialization logic.
	/// </summary>
	public class ODataEntityConverterFactory : JsonConverterFactory
	{
		public override bool CanConvert(Type typeToConvert) =>
			typeToConvert.IsClass &&
			!typeToConvert.IsAbstract &&
			(typeToConvert.GetCustomAttribute<ODataEntityAttribute>(inherit: false) != null ||
			 typeToConvert.GetCustomAttribute<ODataEntitySetAttribute>(inherit: false) != null);

		public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
			(JsonConverter)Activator.CreateInstance(
				typeof(ODataEntityConverter<>).MakeGenericType(typeToConvert))!;
	}

	internal sealed class ODataEntityConverter<T> : JsonConverter<T> where T : class, new()
	{
		private sealed record PropertyEntry(PropertyInfo Prop, string JsonName, bool IsNavProp);
		private sealed record DerivedTypeEntry(string Discriminator, Type Type);

		// Per-type caches built once via static initializer — one instance per closed generic type
		private static readonly PropertyEntry[] _properties = BuildPropertyCache();
		private static readonly DerivedTypeEntry[]? _derivedTypes = BuildDerivedTypeCache();

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

				// Determine JSON property name: prefer JsonPropertyName, fall back to ODataMember, then prop.Name.
				// This is critical for generated types where navigation properties only have ODataMemberAttribute,
				// not JsonPropertyNameAttribute, ensuring they deserialize correctly from OData responses.
				var jsonName = prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
					?? prop.GetCustomAttribute<ODataMemberAttribute>()?.Name
					?? prop.Name;
				entries.Add(new PropertyEntry(prop, jsonName, isNavProp));
			}
			return [.. entries];
		}

		private static DerivedTypeEntry[]? BuildDerivedTypeCache()
		{
			// Check if this type has [ODataPolymorphic] — meaning it's a polymorphic base
			var polyAttr = typeof(T).GetCustomAttribute<ODataPolymorphicAttribute>(inherit: false);
			if (polyAttr == null)
				return null;

			var entries = new List<DerivedTypeEntry>();
			foreach (var attr in typeof(T).GetCustomAttributes<ODataDerivedTypeAttribute>(inherit: false))
			{
				entries.Add(new DerivedTypeEntry(attr.TypeDiscriminator, attr.DerivedType));
			}
			return entries.Count > 0 ? [.. entries] : null;
		}

		public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.Null)
				return null;

			var node = JsonNode.Parse(ref reader);
			if (node is not JsonObject obj)
				return null;

			// Handle polymorphic types: check for OData type discriminator and deserialize as derived type
			if (_derivedTypes != null && obj.TryGetPropertyValue("@odata.type", out var typeNode))
			{
				var discriminator = typeNode?.GetValue<string>();
				if (discriminator != null)
				{
					var derivedType = _derivedTypes.FirstOrDefault(e => 
						e.Discriminator.Equals(discriminator, StringComparison.OrdinalIgnoreCase))?.Type;

					if (derivedType != null && derivedType != typeof(T))
					{
						// Recursively deserialize as the derived type using its ODataEntityConverter
						var derivedEntity = obj.Deserialize(derivedType, options);
						return (T?)derivedEntity;
					}
				}
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

					// Null values in JSON should remain null
					if (propNode.GetValueKind() == JsonValueKind.Null) continue;

					// Empty arrays ([]) should deserialize to empty List<T>, not be skipped
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
			type.IsClass &&
			(type.GetCustomAttribute<ODataEntityAttribute>(inherit: false) != null ||
			 type.GetCustomAttribute<ODataEntitySetAttribute>(inherit: false) != null);

		private static bool IsEntityList(Type type) =>
			type.IsGenericType &&
			type.GetGenericTypeDefinition() == typeof(List<>) &&
			IsEntityType(type.GetGenericArguments()[0]);
	}
}
