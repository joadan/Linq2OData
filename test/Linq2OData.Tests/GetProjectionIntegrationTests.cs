using Linq2OData.Core;
using Linq2OData.Core.Builders;

namespace Linq2OData.Tests;

/// <summary>
/// Integration tests demonstrating real-world usage patterns for Select projections
/// with the GetBuilder API (used for single entity retrieval by key).
/// </summary>
public class GetProjectionIntegrationTests
{
    // Test entity classes matching the real scenario from Program.cs
    [ODataEntitySet("Persons")]
    public class TestPerson : IODataEntitySet
    {
        [ODataMember("ID")]
        public int ID { get; set; }
        
        [ODataMember("Name")]
        public string? Name { get; set; }
        
        [ODataMember("Email")]
        public string? Email { get; set; }
        
        [ODataMember("PersonDetail", isComplex: true)]
        public TestPersonDetail? PersonDetail { get; set; }
        
        public string __Key => $"ID={ID}";
    }

    [ODataEntitySet("PersonDetails")]
    public class TestPersonDetail : IODataEntitySet
    {
        [ODataMember("PersonID")]
        public int PersonID { get; set; }
        
        [ODataMember("Age")]
        public int Age { get; set; }
        
        [ODataMember("Phone")]
        public string? Phone { get; set; }
        
        [ODataMember("Person", isComplex: true)]
        public TestPerson? Person { get; set; }
        
        public string __Key => $"PersonID={PersonID}";
    }

    [Fact]
    public void GetBuilder_WithNestedProjection_GeneratesCorrectODataUrl_V4()
    {
        // Arrange - This simulates the real scenario from TestV4ClientAsync in Program.cs
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://example.com/odata/")
        };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);

        // Create the GetBuilder (simulating clientV4.Get<Person>(e => e.ID = 4))
        var getBuilder = new GetBuilder<TestPerson>(odataClient, p => p.ID = 4);

        // Act - Apply the Select projection (the bug scenario)
        var projectionBuilder = getBuilder.Select(e => new 
        { 
            e.Name, 
            e.ID, 
            PersonDetail = e.PersonDetail!.Person 
        });

        // Trigger SetProjection() by calling it through reflection
        var setProjectionMethod = projectionBuilder.GetType()
            .GetMethod("SetProjection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        setProjectionMethod?.Invoke(projectionBuilder, null);

        // Assert - Verify the internal state has correct OData query components
        var selectProperty = typeof(GetBuilder<TestPerson>)
            .GetField("select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var expandProperty = typeof(GetBuilder<TestPerson>)
            .GetField("expand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var select = selectProperty?.GetValue(getBuilder) as string;
        var expand = expandProperty?.GetValue(getBuilder) as string;

        // These are the values that would be used to construct the OData URL
        Assert.Equal("Name,ID", select);
        Assert.Equal("PersonDetail($expand=Person)", expand);

        // The final URL would be:
        // https://example.com/odata/Persons(ID=4)?$select=Name,ID&$expand=PersonDetail($expand=Person)
    }

    [Fact]
    public void GetBuilder_WithComplexNestedProjection_GeneratesCorrectQuery_V4()
    {
        // Arrange
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://example.com/odata/")
        };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var getBuilder = new GetBuilder<TestPersonDetail>(odataClient, pd => pd.PersonID = 1);

        // Act - Project with both properties from nested object and the nested object itself
        var projectionBuilder = getBuilder.Select(pd => new 
        { 
            pd.Age,
            pd.Phone,
            PersonName = pd.Person!.Name,
            PersonEmail = pd.Person!.Email
        });

        // Trigger SetProjection()
        var setProjectionMethod = projectionBuilder.GetType()
            .GetMethod("SetProjection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        setProjectionMethod?.Invoke(projectionBuilder, null);

        // Assert
        var selectProperty = typeof(GetBuilder<TestPersonDetail>)
            .GetField("select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var expandProperty = typeof(GetBuilder<TestPersonDetail>)
            .GetField("expand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var select = selectProperty?.GetValue(getBuilder) as string;
        var expand = expandProperty?.GetValue(getBuilder) as string;

        Assert.Equal("Age,Phone", select);
        Assert.Equal("Person($select=Name,Email)", expand);
    }

    [Fact]
    public void GetBuilder_WithSimpleProjection_GeneratesCorrectQuery_V4()
    {
        // Arrange
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://example.com/odata/")
        };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var getBuilder = new GetBuilder<TestPerson>(odataClient, p => p.ID = 5);

        // Act - Simple property projection
        var projectionBuilder = getBuilder.Select(e => new { e.Name, e.Email });

        // Trigger SetProjection()
        var setProjectionMethod = projectionBuilder.GetType()
            .GetMethod("SetProjection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        setProjectionMethod?.Invoke(projectionBuilder, null);

        // Assert
        var selectProperty = typeof(GetBuilder<TestPerson>)
            .GetField("select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var expandProperty = typeof(GetBuilder<TestPerson>)
            .GetField("expand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var select = selectProperty?.GetValue(getBuilder) as string;
        var expand = expandProperty?.GetValue(getBuilder) as string;

        Assert.Equal("Name,Email", select);
        Assert.Empty(expand ?? ""); // Empty expand string for simple properties
    }

    [Fact]
    public void GetBuilder_WithOnlyNestedObject_GeneratesCorrectQuery_V4()
    {
        // Arrange
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://example.com/odata/")
        };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var getBuilder = new GetBuilder<TestPerson>(odataClient, p => p.ID = 6);

        // Act - Only project the nested object
        var projectionBuilder = getBuilder.Select(e => new { e.PersonDetail });

        // Trigger SetProjection()
        var setProjectionMethod = projectionBuilder.GetType()
            .GetMethod("SetProjection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        setProjectionMethod?.Invoke(projectionBuilder, null);

        // Assert
        var selectProperty = typeof(GetBuilder<TestPerson>)
            .GetField("select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var expandProperty = typeof(GetBuilder<TestPerson>)
            .GetField("expand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var select = selectProperty?.GetValue(getBuilder) as string;
        var expand = expandProperty?.GetValue(getBuilder) as string;

        Assert.Empty(select ?? ""); // Empty select string (OData V4 behavior)
        Assert.Equal("PersonDetail", expand);
    }

    [Fact]
    public void GetBuilder_V2_WithNestedProjection_GeneratesCorrectQuery()
    {
        // Arrange - Test OData V2 behavior (different from V4)
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://example.com/odata/")
        };
        var odataClient = new ODataClient(httpClient, ODataVersion.V2);
        var getBuilder = new GetBuilder<TestPerson>(odataClient, p => p.ID = 4);

        // Act
        var projectionBuilder = getBuilder.Select(e => new 
        { 
            e.Name, 
            PersonDetail = e.PersonDetail!.Person 
        });

        // Trigger SetProjection()
        var setProjectionMethod = projectionBuilder.GetType()
            .GetMethod("SetProjection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        setProjectionMethod?.Invoke(projectionBuilder, null);

        // Assert
        var selectProperty = typeof(GetBuilder<TestPerson>)
            .GetField("select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var expandProperty = typeof(GetBuilder<TestPerson>)
            .GetField("expand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var select = selectProperty?.GetValue(getBuilder) as string;
        var expand = expandProperty?.GetValue(getBuilder) as string;

        // V2 includes complex properties in select and uses slash notation for expand
        Assert.Equal("Name,PersonDetail", select);
        Assert.Equal("PersonDetail/Person", expand);
    }
}
