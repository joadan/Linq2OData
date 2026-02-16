using Linq2OData.Core;
using Linq2OData.Core.Expressions;
using System.Linq.Expressions;

namespace Linq2OData.Tests;

/// <summary>
/// Comprehensive tests for the OData $expand functionality.
/// Tests cover various expand scenarios including simple, nested, multiple, and collection expands.
/// </summary>
public class ExpandTests
{
    // Test entity classes
    [ODataEntitySet("Orders")]
    public class TestOrder : IODataEntitySet
    {
        [ODataMember("ID")]
        public int ID { get; set; }
        
        [ODataMember("OrderDate")]
        public DateTime OrderDate { get; set; }
        
        [ODataMember("TotalAmount")]
        public decimal TotalAmount { get; set; }
        
        [ODataMember("Customer", isComplex: true)]
        public TestCustomer? Customer { get; set; }
        
        [ODataMember("OrderItems", isComplex: true)]
        public List<TestOrderItem>? OrderItems { get; set; }
        
        public string __Key => $"ID={ID}";
    }

    [ODataEntitySet("Customers")]
    public class TestCustomer : IODataEntitySet
    {
        [ODataMember("ID")]
        public int ID { get; set; }
        
        [ODataMember("Name")]
        public string? Name { get; set; }
        
        [ODataMember("Email")]
        public string? Email { get; set; }
        
        [ODataMember("Address", isComplex: true)]
        public TestAddress? Address { get; set; }
        
        [ODataMember("Orders", isComplex: true)]
        public List<TestOrder>? Orders { get; set; }
        
        [ODataMember("PrimaryContact", isComplex: true)]
        public TestContact? PrimaryContact { get; set; }
        
        public string __Key => $"ID={ID}";
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
        
        [ODataMember("PostalCode")]
        public string? PostalCode { get; set; }
        
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
        
        [ODataMember("Code")]
        public string? Code { get; set; }
        
        [ODataMember("Region", isComplex: true)]
        public TestRegion? Region { get; set; }
        
        public string __Key => $"ID={ID}";
    }

    [ODataEntitySet("Regions")]
    public class TestRegion : IODataEntitySet
    {
        [ODataMember("ID")]
        public int ID { get; set; }
        
        [ODataMember("Name")]
        public string? Name { get; set; }
        
        public string __Key => $"ID={ID}";
    }

    [ODataEntitySet("OrderItems")]
    public class TestOrderItem : IODataEntitySet
    {
        [ODataMember("ID")]
        public int ID { get; set; }
        
        [ODataMember("Quantity")]
        public int Quantity { get; set; }
        
        [ODataMember("UnitPrice")]
        public decimal UnitPrice { get; set; }
        
        [ODataMember("Product", isComplex: true)]
        public TestProduct? Product { get; set; }
        
        public string __Key => $"ID={ID}";
    }

    [ODataEntitySet("Products")]
    public class TestProduct : IODataEntitySet
    {
        [ODataMember("ID")]
        public int ID { get; set; }
        
        [ODataMember("Name")]
        public string? Name { get; set; }
        
        [ODataMember("Description")]
        public string? Description { get; set; }
        
        [ODataMember("Category", isComplex: true)]
        public TestCategory? Category { get; set; }
        
        [ODataMember("Supplier", isComplex: true)]
        public TestSupplier? Supplier { get; set; }
        
        public string __Key => $"ID={ID}";
    }

    [ODataEntitySet("Categories")]
    public class TestCategory : IODataEntitySet
    {
        [ODataMember("ID")]
        public int ID { get; set; }
        
        [ODataMember("Name")]
        public string? Name { get; set; }
        
        [ODataMember("ParentCategory", isComplex: true)]
        public TestCategory? ParentCategory { get; set; }
        
        public string __Key => $"ID={ID}";
    }

    [ODataEntitySet("Suppliers")]
    public class TestSupplier : IODataEntitySet
    {
        [ODataMember("ID")]
        public int ID { get; set; }
        
        [ODataMember("Name")]
        public string? Name { get; set; }
        
        [ODataMember("Address", isComplex: true)]
        public TestAddress? Address { get; set; }
        
        public string __Key => $"ID={ID}";
    }

    [ODataEntitySet("Contacts")]
    public class TestContact : IODataEntitySet
    {
        [ODataMember("ID")]
        public int ID { get; set; }
        
        [ODataMember("FirstName")]
        public string? FirstName { get; set; }
        
        [ODataMember("LastName")]
        public string? LastName { get; set; }
        
        [ODataMember("Phone")]
        public string? Phone { get; set; }
        
        public string __Key => $"ID={ID}";
    }

    #region Simple Expand Tests

    [Fact]
    public void Expand_SingleNavigationProperty_V4_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new { o.ID, o.Customer };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("ID", result.select);
        Assert.Equal("Customer", result.expand);
    }

    [Fact]
    public void Expand_SingleNavigationProperty_V2_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new { o.ID, o.Customer };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V2);

        // Assert
        Assert.Equal("ID,Customer", result.select);
        Assert.Equal("Customer", result.expand);
    }

    [Fact]
    public void Expand_CollectionNavigationProperty_V4_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestCustomer, object>> expression = c => new { c.Name, c.Orders };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Name", result.select);
        Assert.Equal("Orders", result.expand);
    }

    [Fact]
    public void Expand_OnlyNavigationProperty_V4_NoSelectGenerated()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new { o.Customer };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Empty(result.select);
        Assert.Equal("Customer", result.expand);
    }

    #endregion

    #region Multiple Root Level Expands

    [Fact]
    public void Expand_MultipleNavigationProperties_V4_GeneratesCommaSeparatedExpands()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            o.Customer,
            o.OrderItems
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("ID", result.select);
        Assert.Equal("Customer,OrderItems", result.expand);
    }

    [Fact]
    public void Expand_MultipleNavigationProperties_V2_GeneratesCommaSeparatedExpands()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            o.Customer,
            o.OrderItems
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V2);

        // Assert
        Assert.Equal("ID,Customer,OrderItems", result.select);
        Assert.Equal("Customer,OrderItems", result.expand);
    }

    [Fact]
    public void Expand_ThreeNavigationProperties_V4_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestCustomer, object>> expression = c => new 
        { 
            c.Name,
            c.Address,
            c.Orders,
            c.PrimaryContact
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Name", result.select);
        Assert.Equal("Address,Orders,PrimaryContact", result.expand);
    }

    #endregion

    #region Nested Expand Tests

    [Fact]
    public void Expand_TwoLevelNested_V4_GeneratesCorrectNestedExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            CustomerAddress = o.Customer!.Address
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("ID", result.select);
        Assert.Equal("Customer($expand=Address)", result.expand);
    }

    [Fact]
    public void Expand_NestedLevels_V2()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new
        {
            TT = o.OrderItems!.Select(e => e.Product)
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V2);

        // Assert
        Assert.Equal("OrderItems/Product", result.expand);
    }


    [Fact]
    public void Expand_TwoLevelNested_V2_GeneratesSlashNotation()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            CustomerAddress = o.Customer!.Address
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V2);

        // Assert
        Assert.Equal("ID,Customer", result.select);
        Assert.Equal("Customer/Address", result.expand);
    }

    [Fact]
    public void Expand_ThreeLevelNested_V4_GeneratesDeepNestedExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            Country = o.Customer!.Address!.Country
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("ID", result.select);
        Assert.Equal("Customer($expand=Address($expand=Country))", result.expand);
    }

    [Fact]
    public void Expand_FourLevelNested_V4_GeneratesVeryDeepNestedExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            Region = o.Customer!.Address!.Country!.Region
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("ID", result.select);
        Assert.Equal("Customer($expand=Address($expand=Country($expand=Region)))", result.expand);
    }

    [Fact]
    public void Expand_FourLevelNested_V2_GeneratesSlashNotation()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            Region = o.Customer!.Address!.Country!.Region
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V2);

        // Assert
        Assert.Equal("ID,Customer", result.select);
        Assert.Equal("Customer/Address/Country/Region", result.expand);
    }

    #endregion

    #region Nested Expand with Properties

    [Fact]
    public void Expand_NestedWithPropertySelection_V4_GeneratesExpandWithSelect()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            CustomerName = o.Customer!.Name,
            CustomerAddress = o.Customer!.Address
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("ID", result.select);
        Assert.Equal("Customer($select=Name;$expand=Address)", result.expand);
    }

    [Fact]
    public void Expand_DeepNestedWithPropertyAtEachLevel_V4_GeneratesCorrectQuery()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            CustomerName = o.Customer!.Name,
            AddressCity = o.Customer!.Address!.City,
            CountryCode = o.Customer!.Address!.Country!.Code
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("ID", result.select);
        Assert.Equal("Customer($select=Name;$expand=Address($select=City;$expand=Country($select=Code)))", result.expand);
    }

    [Fact]
    public void Expand_NestedWithMultiplePropertiesAtSameLevel_V4_GeneratesCorrectSelect()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            CustomerName = o.Customer!.Name,
            CustomerEmail = o.Customer!.Email,
            CustomerAddress = o.Customer!.Address
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("ID", result.select);
        Assert.Equal("Customer($select=Name,Email;$expand=Address)", result.expand);
    }

    #endregion

    #region Multiple Branches in Expand

    [Fact]
    public void Expand_MultipleBranchesFromSameEntity_V4_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            CustomerAddress = o.Customer!.Address,
            CustomerOrders = o.Customer!.Orders
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("ID", result.select);
        Assert.Equal("Customer($expand=Address,Orders)", result.expand);
    }

    [Fact]
    public void Expand_MultipleDifferentDepthBranches_V4_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            CustomerName = o.Customer!.Name,
            CustomerAddress = o.Customer!.Address,
            AddressCountry = o.Customer!.Address!.Country
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("ID", result.select);
        Assert.Equal("Customer($select=Name;$expand=Address($expand=Country))", result.expand);
    }

    [Fact]
    public void Expand_TwoDifferentRootLevelWithNestedExpands_V4_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            CustomerCountry = o.Customer!.Address!.Country,
            o.OrderItems
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("ID", result.select);
        // Both root-level expands should be present
        Assert.Contains("Customer($expand=Address($expand=Country))", result.expand);
        Assert.Contains("OrderItems", result.expand);
        // The two should be comma-separated
        Assert.Contains(",", result.expand);
    }

    #endregion

    #region Self-Referencing Expand

    [Fact]
    public void Expand_SelfReferencingEntity_V4_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestCategory, object>> expression = c => new 
        { 
            c.Name,
            c.ParentCategory
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Name", result.select);
        Assert.Equal("ParentCategory", result.expand);
    }

    [Fact]
    public void Expand_SelfReferencingEntityNested_V4_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestCategory, object>> expression = c => new 
        { 
            c.Name,
            ParentName = c.ParentCategory!.Name,
            GrandParent = c.ParentCategory!.ParentCategory
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Name", result.select);
        Assert.Equal("ParentCategory($select=Name;$expand=ParentCategory)", result.expand);
    }

    #endregion

    #region Complex Real-World Scenarios

    [Fact]
    public void Expand_ComplexOrderScenario_V4_GeneratesCorrectExpand()
    {
        // Arrange - Get order with customer details, address, and product info from order items
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            o.OrderDate,
            o.TotalAmount,
            CustomerName = o.Customer!.Name,
            CustomerCity = o.Customer!.Address!.City,
            o.OrderItems
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("ID,OrderDate,TotalAmount", result.select);
        Assert.Equal("Customer($select=Name;$expand=Address($select=City)),OrderItems", result.expand);
    }

    [Fact]
    public void Expand_ComplexProductScenario_V4_GeneratesCorrectExpand()
    {
        // Arrange - Get product with category hierarchy and supplier location
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestProduct, object>> expression = p => new 
        { 
            p.Name,
            p.Description,
            CategoryName = p.Category!.Name,
            ParentCategoryName = p.Category!.ParentCategory!.Name,
            SupplierName = p.Supplier!.Name,
            SupplierCountry = p.Supplier!.Address!.Country!.Name
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Name,Description", result.select);
        Assert.Contains("Category($select=Name;$expand=ParentCategory($select=Name))", result.expand);
        Assert.Contains("Supplier($select=Name;$expand=Address($expand=Country($select=Name)))", result.expand);
    }

    [Fact]
    public void Expand_MixedSimpleAndComplexExpands_V4_GeneratesCorrectExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestCustomer, object>> expression = c => new 
        { 
            c.Name,
            c.Email,
            c.Address,
            AddressCountry = c.Address!.Country,
            c.PrimaryContact,
            c.Orders
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Equal("Name,Email", result.select);
        Assert.Contains("Address($expand=Country)", result.expand);
        Assert.Contains("PrimaryContact", result.expand);
        Assert.Contains("Orders", result.expand);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Expand_OnlyComplexProperties_V4_OnlyExpandGenerated()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.Customer,
            o.OrderItems
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Empty(result.select);
        Assert.Equal("Customer,OrderItems", result.expand);
    }

    [Fact]
    public void Expand_OnlyDeepNestedProperty_V4_GeneratesOnlyExpand()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            CountryName = o.Customer!.Address!.Country!.Name
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Empty(result.select);
        Assert.Equal("Customer($expand=Address($expand=Country($select=Name)))", result.expand);
    }

    [Fact]
    public void Expand_MultiplePropertiesFromSameNestedEntity_V4_GroupsCorrectly()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            CountryName = o.Customer!.Address!.Country!.Name,
            CountryCode = o.Customer!.Address!.Country!.Code
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V4);

        // Assert
        Assert.Empty(result.select);
        Assert.Equal("Customer($expand=Address($expand=Country($select=Name,Code)))", result.expand);
    }

    #endregion

    #region OData Version Comparison Tests

    [Fact]
    public void Expand_SameQuery_V2AndV4_GeneratesDifferentFormats()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            CustomerCity = o.Customer!.Address!.City
        };

        // Act
        var nodeV4 = visitor.Parse(expression);
        var resultV4 = nodeV4.GetSelectExpand(ODataVersion.V4);
        
        var nodeV2 = visitor.Parse(expression);
        var resultV2 = nodeV2.GetSelectExpand(ODataVersion.V2);

        // Assert - V4 uses nested format with $expand=
        Assert.Equal("Customer($expand=Address($select=City))", resultV4.expand);
        
        // Assert - V2 uses slash notation
        Assert.Equal("Customer/Address", resultV2.expand);
        Assert.Contains("Customer", resultV2.select);
    }

    [Fact]
    public void Expand_ComplexNestedQuery_V3_GeneratesSlashNotation()
    {
        // Arrange
        var visitor = new QueryNodeVisitor();
        Expression<Func<TestOrder, object>> expression = o => new 
        { 
            o.ID,
            Region = o.Customer!.Address!.Country!.Region
        };

        // Act
        var node = visitor.Parse(expression);
        var result = node.GetSelectExpand(ODataVersion.V3);

        // Assert
        Assert.Equal("Customer/Address/Country/Region", result.expand);
    }

    #endregion
}
