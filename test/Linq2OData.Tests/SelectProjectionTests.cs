using Linq2OData.Core;
using Linq2OData.Core.Expressions;
using System.Linq.Expressions;

namespace Linq2OData.Tests;

/// <summary>
/// Tests for the LINQ Select projection functionality and $select/$expand generation.
/// These tests verify that the SelectExpressionVisitor and QueryNode correctly
/// generate OData query strings for various projection scenarios.
/// </summary>
public class SelectProjectionTests
{
    // Test entity classes
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
        
        [ODataMember("Address", isComplex: true)]
        public TestAddress? Address { get; set; }
        
        [ODataMember("Person", isComplex: true)]
        public TestPerson? Person { get; set; }
        
        public string __Key => $"PersonID={PersonID}";
    }

    [ODataEntitySet("Addresses")]
    public class TestAddress : IODataEntitySet
    {
        [ODataMember("ID")]
        public int ID { get; set; }
        
        [ODataMember("Street")]
        public string? Street { get; set; }
        
        [ODataMember("City")]
        public string? City { get; set; }
        
        [ODataMember("Country", isComplex: true)]
        public TestCountry? Country { get; set; }
        
        public string __Key => $"ID={ID}";
    }

    [ODataEntitySet("Countries")]
    public class TestCountry : IODataEntitySet
    {
        [ODataMember("ID")]
        public int ID { get; set; }
        
        [ODataMember("Name")]
        public string? Name { get; set; }
        
        public string __Key => $"ID={ID}";
    }

    [ODataEntitySet("Products")]
    public class TestProduct : IODataEntitySet
    {
        [ODataMember("ID")]
        public int ID { get; set; }
        
        [ODataMember("Name")]
        public string? Name { get; set; }
        
        [ODataMember("Price")]
        public decimal Price { get; set; }
        
        [ODataMember("Category", isComplex: true)]
        public TestCategory? Category { get; set; }
        
        public string __Key => $"ID={ID}";
    }

    [ODataEntitySet("Categories")]
    public class TestCategory : IODataEntitySet
    {
        [ODataMember("ID")]
        public int ID { get; set; }
        
        [ODataMember("Name")]
        public string? Name { get; set; }
        
        [ODataMember("Products", isComplex: true)]
        public List<TestProduct>? Products { get; set; }
        
        public string __Key => $"ID={ID}";
    }

    #region Simple Property Selection Tests

    [Fact]
    public void SelectProjection_SimpleProperties_V4_GeneratesCorrectSelect()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPerson, object>> expression = p => new { p.Name, p.ID };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Name,ID", result.select);
        Assert.Empty(result.expand);
    }

    [Fact]
    public void SelectProjection_SimpleProperties_V2_GeneratesCorrectSelect()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPerson, object>> expression = p => new { p.Name, p.ID };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V2);

        // Assert
        Assert.Equal("Name,ID", result.select);
        Assert.Empty(result.expand);
    }

    [Fact]
    public void SelectProjection_SingleProperty_V4_GeneratesCorrectSelect()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPerson, object>> expression = p => new { p.Name };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Name", result.select);
        Assert.Empty(result.expand);
    }

    #endregion

    #region Simple Expand Tests

    [Fact]
    public void SelectProjection_SimpleExpand_V4_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPerson, object>> expression = p => new { p.Name, p.PersonDetail };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Name", result.select);
        Assert.Equal("PersonDetail", result.expand);
    }

    [Fact]
    public void SelectProjection_SimpleExpand_V2_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPerson, object>> expression = p => new { p.Name, p.PersonDetail };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V2);

        // Assert
        Assert.Equal("Name,PersonDetail", result.select);
        Assert.Equal("PersonDetail", result.expand);
    }

    #endregion

    #region Nested Expand Tests (Bug Fix Scenario)

    [Fact]
    public void SelectProjection_NestedExpand_V4_GeneratesCorrectExpandWithPrefix()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPerson, object>> expression = p => new { p.Name, p.ID, PersonDetail = p.PersonDetail!.Person };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Name,ID", result.select);
        // This is the key fix: nested expands should use $expand= prefix
        Assert.Equal("PersonDetail($expand=Person)", result.expand);
    }

    [Fact]
    public void SelectProjection_NestedExpandWithProperties_V4_GeneratesCorrectSelectAndExpand()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPersonDetail, object>> expression = pd => new { pd.Age, pd.Phone, Address = pd.Address!.City };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Age,Phone", result.select);
        Assert.Equal("Address($select=City)", result.expand);
    }

    [Fact]
    public void SelectProjection_NestedExpandMultipleLevels_V4_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPersonDetail, object>> expression = pd => new { pd.Age, Country = pd.Address!.Country!.Name };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Age", result.select);
        Assert.Equal("Address($expand=Country($select=Name))", result.expand);
    }

    [Fact]
    public void SelectProjection_MultipleNestedExpands_V4_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPersonDetail, object>> expression = pd => new 
        { 
            pd.Age, 
            AddressCity = pd.Address!.City,
            CountryName = pd.Address!.Country!.Name 
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Age", result.select);
        Assert.Equal("Address($select=City;$expand=Country($select=Name))", result.expand);
    }

    [Fact]
    public void SelectProjection_NestedExpand_V2_GeneratesCorrectSlashNotation()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPerson, object>> expression = p => new { p.Name, PersonDetail = p.PersonDetail!.Person };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V2);

        // Assert
        Assert.Equal("Name,PersonDetail", result.select);
        // V2 uses slash notation for nested expands
        Assert.Equal("PersonDetail/Person", result.expand);
    }

    #endregion

    #region Complex Scenarios

    [Fact]
    public void SelectProjection_MixedPropertiesAndExpands_V4_GeneratesCorrectQuery()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestProduct, object>> expression = p => new 
        { 
            p.Name, 
            p.Price, 
            CategoryName = p.Category!.Name,
            CategoryProducts = p.Category!.Products 
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Name,Price", result.select);
        Assert.Equal("Category($select=Name;$expand=Products)", result.expand);
    }

    [Fact]
    public void SelectProjection_OnlyNestedExpand_V4_NoSelectGenerated()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPerson, object>> expression = p => new { PersonDetail = p.PersonDetail!.Person };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Empty(result.select);
        Assert.Equal("PersonDetail($expand=Person)", result.expand);
    }

    [Fact]
    public void SelectProjection_DeepNesting_V4_GeneratesCorrectNestedExpand()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPerson, object>> expression = p => new 
        { 
            p.Name,
            AddressCountry = p.PersonDetail!.Address!.Country!.Name 
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Name", result.select);
        Assert.Equal("PersonDetail($expand=Address($expand=Country($select=Name)))", result.expand);
    }

    [Fact]
    public void SelectProjection_NullForgivingOperator_V4_HandledCorrectly()
    {
        // Arrange - Test the null-forgiving operator (!) doesn't break query generation
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPerson, object>> expression = p => new 
        { 
            p.Name, 
            p.ID, 
            PersonDetail = p.PersonDetail!.Person 
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Name,ID", result.select);
        Assert.Equal("PersonDetail($expand=Person)", result.expand);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void SelectProjection_EmptyProjection_ReturnsEmptyStrings()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPerson, object>> expression = p => new { };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Empty(result.select);
        Assert.Empty(result.expand);
    }

    [Fact]
    public void SelectProjection_OnlyComplexProperty_V4_OnlyExpandGenerated()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPerson, object>> expression = p => new { p.PersonDetail };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Empty(result.select);
        Assert.Equal("PersonDetail", result.expand);
    }

    [Fact]
    public void SelectProjection_OnlyComplexProperty_V2_BothSelectAndExpandGenerated()
    {
        // Arrange
        var visitor = new SelectExpressionVisitor();
        Expression<Func<TestPerson, object>> expression = p => new { p.PersonDetail };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V2);

        // Assert
        // V2 requires complex properties in both $select and $expand
        Assert.Equal("PersonDetail", result.select);
        Assert.Equal("PersonDetail", result.expand);
    }

    #endregion
}
