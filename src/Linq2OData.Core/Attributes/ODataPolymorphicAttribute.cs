namespace Linq2OData.Core
{
	/// <summary>
	/// Marks an OData entity type as the base of a polymorphic hierarchy.
	/// Used by <see cref="Converters.ODataEntityConverterFactory"/> to handle OData @odata.type discriminators.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class ODataPolymorphicAttribute : Attribute
	{
	}
}
