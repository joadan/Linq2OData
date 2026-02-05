using Linq2OData.Core;
using Linq2OData.Core.Builders;
using Linq2OData.Core.Expressions;
using System.Linq.Expressions;

namespace Linq2OData.Tests;

/// <summary>
/// Tests for the LINQ expression-based orderby functionality.
/// </summary>
public class OrderByExpressionTests
{
    // Test entity classes
    [ODataEntitySet("Suppliers")]
    public class TestSupplier : IODataEntitySet
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public decimal Rating { get; set; }
        public DateTime JoinDate { get; set; }
        public TestAddress? Address { get; set; }
        public string __Keys => $"ID={ID}";
    }

    [ODataEntitySet("Addresses")]
    public class TestAddress : IODataEntitySet
    {
        public int ID { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string __Keys => $"ID={ID}";
    }

    [ODataEntitySet("Products")]
    public class TestProduct : IODataEntitySet
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string __Keys => $"ID={ID}";
    }

    #region ODataOrderByVisitor Tests

    [Fact]
    public void ODataOrderByVisitor_SimpleProperty_GeneratesCorrectOrderBy()
    {
        // Arrange
        var visitor = new ODataOrderByVisitor();
        Expression<Func<TestSupplier, string>> expression = s => s.Name!;

        // Act
        var result = visitor.ToOrderBy(expression);

        // Assert
        Assert.Equal("Name", result);
    }

    [Fact]
    public void ODataOrderByVisitor_NestedProperty_GeneratesPathWithSlash()
    {
        // Arrange
        var visitor = new ODataOrderByVisitor();
        Expression<Func<TestSupplier, string>> expression = s => s.Address!.City!;

        // Act
        var result = visitor.ToOrderBy(expression);

        // Assert
        Assert.Equal("Address/City", result);
    }

    [Fact]
    public void ODataOrderByVisitor_AppendOrderBy_SimpleAscending_GeneratesCorrectString()
    {
        // Arrange
        var visitor = new ODataOrderByVisitor();

        // Act
        var result = visitor.AppendOrderBy("", "Name", descending: false);

        // Assert
        Assert.Equal("Name", result);
    }

    [Fact]
    public void ODataOrderByVisitor_AppendOrderBy_SimpleDescending_GeneratesCorrectString()
    {
        // Arrange
        var visitor = new ODataOrderByVisitor();

        // Act
        var result = visitor.AppendOrderBy("", "Price", descending: true);

        // Assert
        Assert.Equal("Price desc", result);
    }

    [Fact]
    public void ODataOrderByVisitor_AppendOrderBy_MultipleProperties_GeneratesCommaSeratedString()
    {
        // Arrange
        var visitor = new ODataOrderByVisitor();

        // Act
        var step1 = visitor.AppendOrderBy("", "Country", descending: false);
        var step2 = visitor.AppendOrderBy(step1, "Name", descending: false);
        var step3 = visitor.AppendOrderBy(step2, "Rating", descending: true);

        // Assert
        Assert.Equal("Country,Name,Rating desc", step3);
    }

    #endregion

    #region QueryBuilder Order Tests

    [Fact]
    public void QueryBuilder_Order_SimpleProperty_SetsOrderByString()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        var orderByBuilder = queryBuilder.Order(s => s.Name!);

        // Assert
        Assert.Equal("Name", queryBuilder.orderby);
    }

    [Fact]
    public void QueryBuilder_OrderDescending_SimpleProperty_SetsOrderByStringWithDesc()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        var orderByBuilder = queryBuilder.OrderDescending(s => s.Rating);

        // Assert
        Assert.Equal("Rating desc", queryBuilder.orderby);
    }

    [Fact]
    public void QueryBuilder_Order_StringOverload_SetsOrderByString()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        queryBuilder.Order("Name,Price desc");

        // Assert
        Assert.Equal("Name,Price desc", queryBuilder.orderby);
    }

    #endregion

    #region OrderByBuilder ThenBy Tests

    [Fact]
    public void OrderByBuilder_ThenBy_AppendsSecondProperty()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        queryBuilder.Order(s => s.Country!)
                   .ThenBy(s => s.Name!);

        // Assert
        Assert.Equal("Country,Name", queryBuilder.orderby);
    }

    [Fact]
    public void OrderByBuilder_ThenByDescending_AppendsSecondPropertyWithDesc()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        queryBuilder.Order(s => s.Country!)
                   .ThenByDescending(s => s.Rating);

        // Assert
        Assert.Equal("Country,Rating desc", queryBuilder.orderby);
    }

    [Fact]
    public void OrderByBuilder_MultipleThenBy_GeneratesCorrectString()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        queryBuilder.Order(s => s.Country!)
                   .ThenBy(s => s.Rating)
                   .ThenByDescending(s => s.JoinDate)
                   .ThenBy(s => s.Name!);

        // Assert
        Assert.Equal("Country,Rating,JoinDate desc,Name", queryBuilder.orderby);
    }

    [Fact]
    public void OrderByBuilder_OrderDescending_WithThenBy_GeneratesCorrectString()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        queryBuilder.OrderDescending(s => s.Rating)
                   .ThenBy(s => s.Name!);

        // Assert
        Assert.Equal("Rating desc,Name", queryBuilder.orderby);
    }

    #endregion

    #region OrderByBuilder Method Chaining Tests

    [Fact]
    public void OrderByBuilder_Filter_ReturnsQueryBuilder()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        var result = queryBuilder.Order(s => s.Name!)
                                 .Filter(s => s.Country == "USA");

        // Assert
        Assert.IsType<QueryBuilder<TestSupplier>>(result);
    }

    [Fact]
    public void OrderByBuilder_Top_ReturnsQueryBuilder()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        var result = queryBuilder.Order(s => s.Name!)
                                 .Top(10);

        // Assert
        Assert.IsType<QueryBuilder<TestSupplier>>(result);
    }

    [Fact]
    public void OrderByBuilder_Skip_ReturnsQueryBuilder()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        var result = queryBuilder.Order(s => s.Name!)
                                 .Skip(5);

        // Assert
        Assert.IsType<QueryBuilder<TestSupplier>>(result);
    }

    [Fact]
    public void OrderByBuilder_Count_ReturnsQueryBuilder()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        var result = queryBuilder.Order(s => s.Name!)
                                 .Count(true);

        // Assert
        Assert.IsType<QueryBuilder<TestSupplier>>(result);
    }

    [Fact]
    public void OrderByBuilder_ImplicitConversion_WorksCorrectly()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act - implicit conversion when assigned to QueryBuilder
        QueryBuilder<TestSupplier> result = queryBuilder.Order(s => s.Name!);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<QueryBuilder<TestSupplier>>(result);
    }

    #endregion

    #region Nested Property Order Tests

    [Fact]
    public void QueryBuilder_Order_NestedProperty_GeneratesPathSyntax()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        queryBuilder.Order(s => s.Address!.City!);

        // Assert
        Assert.Equal("Address/City", queryBuilder.orderby);
    }

    [Fact]
    public void QueryBuilder_Order_MultipleNestedProperties_GeneratesCorrectString()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        queryBuilder.Order(s => s.Address!.Country!)
                   .ThenBy(s => s.Address!.City!)
                   .ThenByDescending(s => s.Rating);

        // Assert
        Assert.Equal("Address/Country,Address/City,Rating desc", queryBuilder.orderby);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void QueryBuilder_OrderWithFilterAndTop_GeneratesCorrectQuery()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        queryBuilder.Filter(s => s.Country == "USA")
                   .Order(s => s.Rating)
                       .ThenByDescending(s => s.JoinDate)
                   .Top(20);

        // Assert
        Assert.Equal("Rating,JoinDate desc", queryBuilder.orderby);
        Assert.NotNull(queryBuilder.filter);
        Assert.Equal(20, queryBuilder.top);
    }

    [Fact]
    public void QueryBuilder_ComplexQueryWithOrder_SetsAllProperties()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://test.com") };
        var odataClient = new ODataClient(httpClient, ODataVersion.V4);
        var queryBuilder = new QueryBuilder<TestSupplier>(odataClient);

        // Act
        queryBuilder.Filter(s => s.Rating > 4)
                   .OrderDescending(s => s.JoinDate)
                       .ThenBy(s => s.Name!)
                   .Skip(10)
                   .Top(20)
                   .Count(true);

        // Assert
        Assert.Equal("JoinDate desc,Name", queryBuilder.orderby);
        Assert.NotNull(queryBuilder.filter);
        Assert.Equal(10, queryBuilder.skip);
        Assert.Equal(20, queryBuilder.top);
        Assert.True(queryBuilder.count);
    }

    #endregion
}
