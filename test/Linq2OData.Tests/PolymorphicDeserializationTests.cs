using Linq2OData.Core;
using Linq2OData.Core.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linq2OData.Tests;

public class PolymorphicDeserializationTests
{
	private static JsonSerializerOptions CreateOptions() => new()
	{
		Converters = { new ODataEntityConverterFactory() }
	};

	[Fact]
	public void Polymorphic_BaseTypeWithDiscriminator_DeserializesToCorrectDerivedType()
	{
		var json = """
		{
			"@odata.type": "#TestNamespace.DerivedAnimal",
			"Id": 1,
			"Name": "Fluffy",
			"Breed": "Golden Retriever"
		}
		""";

		var result = JsonSerializer.Deserialize<BaseAnimal>(json, CreateOptions());

		Assert.NotNull(result);
		Assert.IsType<DerivedAnimal>(result);
		var derived = (DerivedAnimal)result;
		Assert.Equal(1, derived.Id);
		Assert.Equal("Fluffy", derived.Name);
		Assert.Equal("Golden Retriever", derived.Breed);
	}

	[Fact]
	public void Polymorphic_DerivedTypeNavigationWithJsonIgnore_PopulatesWhenExpanded()
	{
		var json = """
		{
			"@odata.type": "#TestNamespace.DerivedAnimal",
			"Id": 1,
			"Name": "Fluffy",
			"Breed": "Golden Retriever",
			"Owner": {
				"OwnerId": 100,
				"OwnerName": "John"
			}
		}
		""";

		var result = JsonSerializer.Deserialize<BaseAnimal>(json, CreateOptions());

		Assert.NotNull(result);
		Assert.IsType<DerivedAnimal>(result);
		var derived = (DerivedAnimal)result;
		Assert.NotNull(derived.Owner);
		Assert.Equal(100, derived.Owner.OwnerId);
		Assert.Equal("John", derived.Owner.OwnerName);
	}

	[Fact]
	public void Polymorphic_DerivedTypeNavigationWithJsonIgnore_RemainsNullWhenNotExpanded()
	{
		var json = """
		{
			"@odata.type": "#TestNamespace.DerivedAnimal",
			"Id": 1,
			"Name": "Fluffy",
			"Breed": "Golden Retriever"
		}
		""";

		var result = JsonSerializer.Deserialize<BaseAnimal>(json, CreateOptions());

		Assert.NotNull(result);
		Assert.IsType<DerivedAnimal>(result);
		var derived = (DerivedAnimal)result;
		Assert.Null(derived.Owner);
	}

	[Fact]
	public void Polymorphic_CollectionOfPolymorphicTypes_DeserializesCorrectly()
	{
		var json = """
		[
			{
				"@odata.type": "#TestNamespace.BaseAnimal",
				"Id": 1,
				"Name": "Generic Animal"
			},
			{
				"@odata.type": "#TestNamespace.DerivedAnimal",
				"Id": 2,
				"Name": "Fluffy",
				"Breed": "Golden Retriever",
				"Owner": {
					"OwnerId": 100,
					"OwnerName": "John"
				}
			},
			{
				"@odata.type": "#TestNamespace.AnotherDerivedAnimal",
				"Id": 3,
				"Name": "Whiskers",
				"Species": "Cat"
			}
		]
		""";

		var result = JsonSerializer.Deserialize<List<BaseAnimal>>(json, CreateOptions());

		Assert.NotNull(result);
		Assert.Equal(3, result.Count);
		
		Assert.IsType<BaseAnimal>(result[0]);
		Assert.Equal("Generic Animal", result[0].Name);
		
		Assert.IsType<DerivedAnimal>(result[1]);
		var dog = (DerivedAnimal)result[1];
		Assert.Equal("Fluffy", dog.Name);
		Assert.Equal("Golden Retriever", dog.Breed);
		Assert.NotNull(dog.Owner);
		Assert.Equal("John", dog.Owner.OwnerName);
		
		Assert.IsType<AnotherDerivedAnimal>(result[2]);
		var cat = (AnotherDerivedAnimal)result[2];
		Assert.Equal("Whiskers", cat.Name);
		Assert.Equal("Cat", cat.Species);
	}

	[Fact]
	public void Polymorphic_NestedPolymorphicInNonPolymorphicParent_Works()
	{
		var json = """
		{
			"HouseholdId": 1,
			"Address": "123 Main St",
			"Pets": [
				{
					"@odata.type": "#TestNamespace.DerivedAnimal",
					"Id": 1,
					"Name": "Fluffy",
					"Breed": "Golden Retriever",
					"Owner": {
						"OwnerId": 100,
						"OwnerName": "John"
					}
				},
				{
					"@odata.type": "#TestNamespace.AnotherDerivedAnimal",
					"Id": 2,
					"Name": "Whiskers",
					"Species": "Cat"
				}
			]
		}
		""";

		var result = JsonSerializer.Deserialize<Household>(json, CreateOptions());

		Assert.NotNull(result);
		Assert.Equal(1, result.HouseholdId);
		Assert.Equal("123 Main St", result.Address);
		Assert.NotNull(result.Pets);
		Assert.Equal(2, result.Pets.Count);
		
		Assert.IsType<DerivedAnimal>(result.Pets[0]);
		var dog = (DerivedAnimal)result.Pets[0];
		Assert.Equal("Fluffy", dog.Name);
		Assert.NotNull(dog.Owner);
		
		Assert.IsType<AnotherDerivedAnimal>(result.Pets[1]);
	}

	[Fact]
	public void Polymorphic_UnknownDiscriminator_FallsBackToBaseType()
	{
		var json = """
		{
			"@odata.type": "#TestNamespace.UnknownAnimal",
			"Id": 1,
			"Name": "Mystery Pet"
		}
		""";

		var result = JsonSerializer.Deserialize<BaseAnimal>(json, CreateOptions());

		Assert.NotNull(result);
		// Should fallback to base type when discriminator is not recognized
		Assert.IsType<BaseAnimal>(result);
		Assert.Equal(1, result.Id);
		Assert.Equal("Mystery Pet", result.Name);
	}

	[Fact]
	public void Polymorphic_NoDiscriminator_DeserializesAsBaseType()
	{
		var json = """
		{
			"Id": 1,
			"Name": "No Type Info"
		}
		""";

		var result = JsonSerializer.Deserialize<BaseAnimal>(json, CreateOptions());

		Assert.NotNull(result);
		Assert.IsType<BaseAnimal>(result);
		Assert.Equal(1, result.Id);
		Assert.Equal("No Type Info", result.Name);
	}

	[Fact]
	public void Polymorphic_EmptyCollection_DeserializesToEmptyList()
	{
		var json = """
		{
			"HouseholdId": 1,
			"Address": "123 Main St",
			"Pets": []
		}
		""";

		var result = JsonSerializer.Deserialize<Household>(json, CreateOptions());

		Assert.NotNull(result);
		Assert.NotNull(result.Pets);
		Assert.Empty(result.Pets);
	}

	[Fact]
	public void Polymorphic_MultiLevelInheritance_DeserializesCorrectly()
	{
		var json = """
		{
			"@odata.type": "#TestNamespace.GrandchildAnimal",
			"Id": 1,
			"Name": "Max",
			"Breed": "Labrador",
			"TrainingLevel": "Advanced"
		}
		""";

		var result = JsonSerializer.Deserialize<BaseAnimal>(json, CreateOptions());

		Assert.NotNull(result);
		Assert.IsType<GrandchildAnimal>(result);
		var grandchild = (GrandchildAnimal)result;
		Assert.Equal("Max", grandchild.Name);
		Assert.Equal("Labrador", grandchild.Breed);
		Assert.Equal("Advanced", grandchild.TrainingLevel);
	}

	// Test entity types

	[ODataPolymorphic]
	[ODataDerivedType(typeof(BaseAnimal), "#TestNamespace.BaseAnimal")]
	[ODataDerivedType(typeof(DerivedAnimal), "#TestNamespace.DerivedAnimal")]
	[ODataDerivedType(typeof(AnotherDerivedAnimal), "#TestNamespace.AnotherDerivedAnimal")]
	[ODataDerivedType(typeof(GrandchildAnimal), "#TestNamespace.GrandchildAnimal")]
	[ODataEntity]
	public class BaseAnimal
	{
		public BaseAnimal() { }

		[ODataMember("Id")]
		public int Id { get; set; }

		[ODataMember("Name")]
		public string? Name { get; set; }
	}

	public class DerivedAnimal : BaseAnimal
	{
		public DerivedAnimal() { }

		[ODataMember("Breed")]
		public string? Breed { get; set; }

		[ODataMember("Owner", true)]
		[JsonIgnore]
		public Owner? Owner { get; set; }
	}

	public class AnotherDerivedAnimal : BaseAnimal
	{
		public AnotherDerivedAnimal() { }

		[ODataMember("Species")]
		public string? Species { get; set; }
	}

	public class GrandchildAnimal : DerivedAnimal
	{
		public GrandchildAnimal() { }

		[ODataMember("TrainingLevel")]
		public string? TrainingLevel { get; set; }
	}

	[ODataEntity]
	public class Owner
	{
		public Owner() { }

		[ODataMember("OwnerId")]
		public int OwnerId { get; set; }

		[ODataMember("OwnerName")]
		public string? OwnerName { get; set; }
	}

	[ODataEntity]
	public class Household
	{
		public Household() { }

		[ODataMember("HouseholdId")]
		public int HouseholdId { get; set; }

		[ODataMember("Address")]
		public string? Address { get; set; }

		[ODataMember("Pets", true)]
		[JsonIgnore]
		public List<BaseAnimal>? Pets { get; set; }
	}
}
