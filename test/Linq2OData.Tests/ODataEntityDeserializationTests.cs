using Linq2OData.Core;
using System.Text.Json.Serialization;

namespace Linq2OData.Tests;

/// <summary>
/// Tests for the ODataEntityConverterFactory deserialization introduced with the [ODataEntity] attribute.
/// Covers non-entity-set types, [JsonIgnore] bypass, attribute inheritance, polymorphism,
/// and regression for [ODataEntitySet] types across V2 and V4.
/// </summary>
public class ODataEntityDeserializationTests
{
	// ------------------------------------------------------------------
	// V4 — non-entity-set type decorated with [ODataEntity]
	// ------------------------------------------------------------------

	/// <summary>
	/// A type marked [ODataEntity] but not [ODataEntitySet] (e.g. Trip, PlanItem in TripPin)
	/// must have its expanded collection navigation property populated.
	/// </summary>
	[Fact]
	public void ODataV4_NonEntitySetType_ExpandedCollectionNavProp_IsPopulated()
	{
		const string json = """{"value":[{"OrderId":1,"Description":"Test Order","Lines":[{"LineId":10,"Product":"Widget","Quantity":5}]}]}""";

		var odataClient = new ODataClient(new HttpClient(), ODataVersion.V4);
		var result = odataClient.ProcessQueryResponse<List<TestOrder>>(json);

		Assert.NotNull(result.Data);
		Assert.Single(result.Data);
		Assert.NotNull(result.Data[0].Lines);
		Assert.Single(result.Data[0].Lines!);
		Assert.Equal(10, result.Data[0].Lines![0].LineId);
		Assert.Equal("Widget", result.Data[0].Lines![0].Product);
		Assert.Equal(5, result.Data[0].Lines![0].Quantity);
	}

	/// <summary>
	/// A type marked [ODataEntity] must have its expanded single object navigation property populated.
	/// </summary>
	[Fact]
	public void ODataV4_NonEntitySetType_ExpandedSingleNavProp_IsPopulated()
	{
		const string json = """{"value":[{"OrderId":1,"Description":"Test Order","Customer":{"CustomerId":42,"Name":"Acme Corp"}}]}""";

		var odataClient = new ODataClient(new HttpClient(), ODataVersion.V4);
		var result = odataClient.ProcessQueryResponse<List<TestOrder>>(json);

		Assert.NotNull(result.Data);
		Assert.Single(result.Data);
		Assert.NotNull(result.Data[0].Customer);
		Assert.Equal(42, result.Data[0].Customer!.CustomerId);
		Assert.Equal("Acme Corp", result.Data[0].Customer!.Name);
	}

	/// <summary>
	/// Navigation properties decorated with [JsonIgnore] must still be populated by the factory
	/// when present in the response — this is the core generated-client pattern (Trip.Photos, etc.).
	/// </summary>
	[Fact]
	public void ODataV4_NonEntitySetType_JsonIgnoreOnNavProps_IsBypassedWhenExpanded()
	{
		const string json = """{"value":[{"OrderId":1,"Lines":[{"LineId":10,"Product":"Widget","Quantity":5}],"Customer":{"CustomerId":42,"Name":"Acme"}}]}""";

		var odataClient = new ODataClient(new HttpClient(), ODataVersion.V4);
		var result = odataClient.ProcessQueryResponse<List<TestOrder>>(json);

		Assert.NotNull(result.Data);
		Assert.Single(result.Data);
		// Both nav props have [JsonIgnore] — factory must bypass that and populate them
		Assert.NotNull(result.Data[0].Lines);
		Assert.NotNull(result.Data[0].Customer);
	}

	/// <summary>
	/// When navigation properties are absent from the response they must remain null.
	/// </summary>
	[Fact]
	public void ODataV4_NonEntitySetType_AbsentNavProps_RemainNull()
	{
		const string json = """{"value":[{"OrderId":1,"Description":"No nav props"}]}""";

		var odataClient = new ODataClient(new HttpClient(), ODataVersion.V4);
		var result = odataClient.ProcessQueryResponse<List<TestOrder>>(json);

		Assert.NotNull(result.Data);
		Assert.Single(result.Data);
		Assert.Equal(1, result.Data[0].OrderId);
		Assert.Equal("No nav props", result.Data[0].Description);
		Assert.Null(result.Data[0].Lines);
		Assert.Null(result.Data[0].Customer);
	}

	// ------------------------------------------------------------------
	// V4 — [ODataEntitySet] type regression (extends ODataEntityAttribute)
	// ------------------------------------------------------------------

	/// <summary>
	/// [ODataEntitySet] extends [ODataEntity], so entity-set types must still be intercepted
	/// and have their navigation properties populated correctly.
	/// </summary>
	[Fact]
	public void ODataV4_EntitySetType_SatisfiesODataEntityCheck_NavPropsPopulated()
	{
		const string json = """{"value":[{"CustomerId":1,"Name":"Acme","Orders":[{"OrderId":100,"Description":"Big Order"}]}]}""";

		var odataClient = new ODataClient(new HttpClient(), ODataVersion.V4);
		var result = odataClient.ProcessQueryResponse<List<TestCustomerEntitySet>>(json);

		Assert.NotNull(result.Data);
		Assert.Single(result.Data);
		Assert.Equal(1, result.Data[0].CustomerId);
		Assert.NotNull(result.Data[0].Orders);
		Assert.Single(result.Data[0].Orders!);
		Assert.Equal(100, result.Data[0].Orders![0].OrderId);
	}

	// ------------------------------------------------------------------
	// V4 — type without [ODataEntity] is not intercepted
	// ------------------------------------------------------------------

	/// <summary>
	/// A plain class with no [ODataEntity] attribute must not be intercepted by the factory
	/// and must be deserialized correctly by System.Text.Json's default handling.
	/// </summary>
	[Fact]
	public void ODataV4_TypeWithoutODataEntityAttribute_DeserializedByDefaultConverter()
	{
		const string json = """{"value":[{"Id":1,"Value":"hello"}]}""";

		var odataClient = new ODataClient(new HttpClient(), ODataVersion.V4);
		var result = odataClient.ProcessQueryResponse<List<TestPlainClass>>(json);

		Assert.NotNull(result.Data);
		Assert.Single(result.Data);
		Assert.Equal(1, result.Data[0].Id);
		Assert.Equal("hello", result.Data[0].Value);
	}

	// ------------------------------------------------------------------
	// V4 — polymorphic @odata.type discriminator
	// ------------------------------------------------------------------

	/// <summary>
	/// When a [ODataEntity] base type declares [JsonDerivedType] attributes, an @odata.type
	/// discriminator in the response must cause the factory to deserialize the correct concrete type.
	/// </summary>
	// ------------------------------------------------------------------
	// V4 — empty expanded collection deserializes to empty list
	// ------------------------------------------------------------------

	/// <summary>
	/// An expanded collection that arrives as an empty JSON array should deserialize
	/// to an empty List<T>, not null. This distinguishes between:
	/// - Property absent from JSON → null (not expanded)
	/// - Property is [] → empty List<T> (expanded but has no items)
	/// </summary>
	[Fact]
	public void ODataV4_NonEntitySetType_EmptyExpandedCollection_DeserializesToEmptyList()
	{
		const string json = """{"value":[{"OrderId":1,"Description":"No items","Lines":[]}]}""";

		var odataClient = new ODataClient(new HttpClient(), ODataVersion.V4);
		var result = odataClient.ProcessQueryResponse<List<TestOrder>>(json);

		Assert.NotNull(result.Data);
		Assert.Single(result.Data);
		// Empty array in JSON should deserialize to empty List<T>
		Assert.NotNull(result.Data[0].Lines);
		Assert.Empty(result.Data[0].Lines!);
	}

	// ------------------------------------------------------------------
	// V2 — __deferred nav prop on a [ODataEntity] non-entity-set type stays null
	// ------------------------------------------------------------------

	/// <summary>
	/// OData V2/V3 __deferred objects on a non-entity-set [ODataEntity] type must be
	/// skipped (nav prop stays null), just as they are on entity-set types.
	/// </summary>
	[Fact]
	public void ODataV2_NonEntitySetType_DeferredNavProp_RemainsNull()
	{
		// TestOrder is a non-entity-set [ODataEntity] type nested inside a V2 entity-set.
		// Its Lines nav prop arrives as __deferred — must remain null after deserialization.
		const string json = """{"d":{"results":[{"CustomerId":1,"Name":"Acme","Orders":{"results":[{"OrderId":100,"Description":"Pending","Lines":{"__deferred":{"uri":"https://example.com/Orders(100)/Lines"}}}]}}]}}""";

		var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);
		var result = odataClient.ProcessQueryResponse<List<TestCustomerEntitySet>>(json);

		Assert.NotNull(result.Data);
		Assert.Single(result.Data);
		Assert.NotNull(result.Data[0].Orders);
		var order = result.Data[0].Orders![0];
		Assert.Equal(100, order.OrderId);
		Assert.Null(order.Lines);
	}

	// ------------------------------------------------------------------
	// V2 — non-entity-set type nested inside an entity-set, results wrapper
	// ------------------------------------------------------------------

	/// <summary>
	/// In OData V2 responses the expanded collection wrapper is {"results":[...]}.
	/// A non-entity-set type nav prop must be deserialized correctly through this wrapper.
	/// </summary>
	[Fact]
	public void ODataV2_EntitySetType_ExpandedNonEntitySetNavProp_ResultsWrapper_IsPopulated()
	{
		const string json = """{"d":{"results":[{"CustomerId":1,"Name":"Acme","Orders":{"results":[{"OrderId":100,"Description":"Big Order"}]}}]}}""";

		var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);
		var result = odataClient.ProcessQueryResponse<List<TestCustomerEntitySet>>(json);

		Assert.NotNull(result.Data);
		Assert.Single(result.Data);
		Assert.NotNull(result.Data[0].Orders);
		Assert.Single(result.Data[0].Orders!);
		Assert.Equal(100, result.Data[0].Orders![0].OrderId);
	}

	// ------------------------------------------------------------------
	// Test entity types
	// ------------------------------------------------------------------

	[ODataEntity]
	private class TestOrder
	{
		public int OrderId { get; set; }
		public string? Description { get; set; }

		[JsonIgnore]
		public List<TestOrderLine>? Lines { get; set; }

		[JsonIgnore]
		public TestOrderCustomer? Customer { get; set; }
	}

	[ODataEntity]
	private class TestOrderLine
	{
		public int LineId { get; set; }
		public string? Product { get; set; }
		public int Quantity { get; set; }
	}

	[ODataEntity]
	private class TestOrderCustomer
	{
		public int CustomerId { get; set; }
		public string? Name { get; set; }
	}

	[ODataEntitySet("Customers")]
	private class TestCustomerEntitySet : IODataEntitySet
	{
		public int CustomerId { get; set; }
		public string? Name { get; set; }

		[JsonIgnore]
		public List<TestOrder>? Orders { get; set; }

		public string __Key => $"CustomerId={CustomerId}";
	}

	private class TestPlainClass
	{
		public int Id { get; set; }
		public string? Value { get; set; }
	}

	}

