namespace Linq2OData.Core
{
	/// <summary>
	/// Declares a derived type in a polymorphic hierarchy along with its OData type discriminator.
	/// Used by <see cref="Converters.ODataEntityConverterFactory"/> to map @odata.type values to concrete types.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class ODataDerivedTypeAttribute : Attribute
	{
		public ODataDerivedTypeAttribute(Type derivedType, string typeDiscriminator)
		{
			DerivedType = derivedType;
			TypeDiscriminator = typeDiscriminator;
		}

		public Type DerivedType { get; }
		public string TypeDiscriminator { get; }
	}
}
