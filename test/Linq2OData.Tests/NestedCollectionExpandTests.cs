using Linq2OData.Core;
using Linq2OData.Core.Expressions;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Linq2OData.Tests;

/// <summary>
/// Tests for nested collection expands using .Select() syntax (e.g., e.Trips.Select(t => t.PlanItems))
/// </summary>
public class NestedCollectionExpandTests
{
    [ODataEntitySet("People")]
    public class TestPerson : IODataEntitySet
    {
        [ODataMember("UserName")]
        public string? UserName { get; set; }

        [ODataMember("Trips", true)]
        [JsonIgnore]
        public List<TestTrip>? Trips { get; set; }

        public string __Key => $"UserName='{UserName}'";
    }

    [ODataEntity]
    public class TestTrip
    {
        [ODataMember("TripId")]
        public int TripId { get; set; }

        [ODataMember("Name")]
        public string? Name { get; set; }

        [ODataMember("PlanItems", true)]
        [JsonIgnore]
        public List<TestPlanItem>? PlanItems { get; set; }
    }

    [ODataEntity]
    public class TestPlanItem
    {
        [ODataMember("PlanItemId")]
        public int PlanItemId { get; set; }

        [ODataMember("Description")]
        public string? Description { get; set; }
    }

    [Fact]
    public void Expand_CollectionWithNestedSelect_V4_GeneratesCorrectExpand()
    {
        // Arrange - mimics: e => e.Trips.Select(t => t.PlanItems)
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestPerson, object>> expression = p => p.Trips!.Select(t => t.PlanItems);

        // Act
        var node = visitor.Parse(expression);
        var expand = node.GetOnlyExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Trips($expand=PlanItems)", expand);
    }

    [Fact]
    public void Expand_CollectionWithNestedSelect_V4_Deserialization_Works()
    {
        // Arrange - JSON response with nested expanded collections
        const string json = """
		{
			"value": [
				{
					"UserName": "alice",
					"Trips": [
						{
							"TripId": 1,
							"Name": "Trip to Paris",
							"PlanItems": [
								{ "PlanItemId": 101, "Description": "Flight" },
								{ "PlanItemId": 102, "Description": "Hotel" }
							]
						},
						{
							"TripId": 2,
							"Name": "Trip to London",
							"PlanItems": [
								{ "PlanItemId": 201, "Description": "Train" }
							]
						},
						{
							"TripId": 3,
							"Name": "Trip to Stockholm",
							"PlanItems": []
						}
					]
				}
			]
		}
		""";

        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V4);

        // Act
        var result = odataClient.ProcessQueryResponse<List<TestPerson>>(json);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);

        var person = result.Data[0];
        Assert.Equal("alice", person.UserName);
        Assert.NotNull(person.Trips);
        Assert.Equal(3, person.Trips!.Count);

        // Verify first trip
        Assert.Equal(1, person.Trips[0].TripId);
        Assert.Equal("Trip to Paris", person.Trips[0].Name);
        Assert.NotNull(person.Trips[0].PlanItems);
        Assert.Equal(2, person.Trips[0].PlanItems!.Count);
        Assert.Equal(101, person.Trips[0].PlanItems[0].PlanItemId);
        Assert.Equal("Flight", person.Trips[0].PlanItems[0].Description);

        // Verify second trip
        Assert.Equal(2, person.Trips[1].TripId);
        Assert.NotNull(person.Trips[1].PlanItems);
        Assert.Single(person.Trips[1].PlanItems!);
        Assert.Equal(201, person.Trips[1].PlanItems[0].PlanItemId);

        // Verify third trip - empty PlanItems array in JSON should deserialize to empty list
        Assert.NotNull(person.Trips[2].PlanItems);
        Assert.Empty(person.Trips[2].PlanItems!);
    }

    [Fact]
    public void Expand_CollectionWithNestedSelect_V4_TripsWithoutPlanItems_DeserializesTripsOnly()
    {
        // Arrange - JSON response where Trips doesn't have PlanItems expanded
        const string json = """
		{
			"value": [
				{
					"UserName": "bob",
					"Trips": [
						{
							"TripId": 10,
							"Name": "Business Trip"
						}
					]
				}
			]
		}
		""";

        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V4);

        // Act
        var result = odataClient.ProcessQueryResponse<List<TestPerson>>(json);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);

        var person = result.Data[0];
        Assert.Equal("bob", person.UserName);
        Assert.NotNull(person.Trips);
        Assert.Single(person.Trips!);
        Assert.Equal(10, person.Trips[0].TripId);
        Assert.Equal("Business Trip", person.Trips[0].Name);
        Assert.Null(person.Trips[0].PlanItems); // PlanItems not expanded
    }

    [Fact]
    public void ODataMemberAttribute_UsedForJsonPropertyName_WhenJsonPropertyNameAbsent()
    {
        // Verify the fix: ODataMemberAttribute.Name should be used for JSON deserialization
        // when JsonPropertyName is absent
        const string json = """
		{
			"value": [
				{
					"UserName": "alice",
					"Trips": [
						{
							"TripId": 1,
							"Name": "Paris"
						}
					]
				}
			]
		}
		""";

        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V4);
        var result = odataClient.ProcessQueryResponse<List<TestPerson>>(json);

        // The generated types have [ODataMember("Trips", true)] but no [JsonPropertyName]
        // The converter should use ODataMemberAttribute.Name (which is "Trips") for deserialization
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.NotNull(result.Data[0].Trips);
        Assert.Equal("alice", result.Data[0].UserName);
        Assert.Single(result.Data[0].Trips!);
        Assert.Equal(1, result.Data[0].Trips[0].TripId);
    }
}

