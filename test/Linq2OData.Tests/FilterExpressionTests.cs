using Linq2OData.Core;
using Linq2OData.Core.Expressions;
using System.Linq.Expressions;

namespace Linq2OData.Tests;

/// <summary>
/// Tests for the LINQ expression-based filter functionality.
/// </summary>
public class FilterExpressionTests
{
    // Test entity classes
    [ODataEntitySet("Products")]
    public class TestProduct : IODataEntitySet
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DiscontinuedDate { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public DateTimeOffset? LastChecked { get; set; }
        public TestCategory? Category { get; set; }
        public string __Key => $"ID={ID}";
    }

    [ODataEntitySet("Categories")]
    public class TestCategory : IODataEntitySet
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string __Key => $"ID={ID}";
    }

    #region Basic Comparison Tests

    [Fact]
    public void ODataFilterVisitor_Equal_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.ID == 5;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(ID eq 5)", result);
    }

    [Fact]
    public void ODataFilterVisitor_NotEqual_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.ID != 5;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(ID ne 5)", result);
    }

    [Fact]
    public void ODataFilterVisitor_LessThan_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Price < 100;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(Price lt 100)", result);
    }

    [Fact]
    public void ODataFilterVisitor_LessThanOrEqual_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Price <= 100;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(Price le 100)", result);
    }

    [Fact]
    public void ODataFilterVisitor_GreaterThan_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Stock > 10;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(Stock gt 10)", result);
    }

    [Fact]
    public void ODataFilterVisitor_GreaterThanOrEqual_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Stock >= 10;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(Stock ge 10)", result);
    }

    #endregion

    #region Boolean Tests

    [Fact]
    public void ODataFilterVisitor_BooleanTrue_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.IsAvailable == true;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(IsAvailable eq true)", result);
    }

    [Fact]
    public void ODataFilterVisitor_BooleanFalse_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.IsAvailable == false;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(IsAvailable eq false)", result);
    }

    #endregion

    #region String Tests

    [Fact]
    public void ODataFilterVisitor_StringEqual_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Name == "Widget";

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(Name eq 'Widget')", result);
    }

    [Fact]
    public void ODataFilterVisitor_StringVariable_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var searchName = "Gadget";
        Expression<Func<TestProduct, bool>> expression = p => p.Name == searchName;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(Name eq 'Gadget')", result);
    }

    #endregion

    #region Null Tests

    [Fact]
    public void ODataFilterVisitor_NullEqual_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.DiscontinuedDate == null;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(DiscontinuedDate eq null)", result);
    }

    [Fact]
    public void ODataFilterVisitor_NullNotEqual_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.DiscontinuedDate != null;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(DiscontinuedDate ne null)", result);
    }

    #endregion

    #region DateTime Tests - OData V2

    [Fact]
    public void ODataFilterVisitor_DateTimeConstant_V2_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var testDate = new DateTime(2024, 1, 15, 10, 30, 0);
        Expression<Func<TestProduct, bool>> expression = p => p.CreatedDate < testDate;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V2);

        // Assert
        Assert.Equal("(CreatedDate lt datetime'2024-01-15T10:30:00')", result);
    }

    [Fact]
    public void ODataFilterVisitor_DateTimeVariable_V2_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var cutoffDate = new DateTime(2024, 6, 1, 0, 0, 0);
        Expression<Func<TestProduct, bool>> expression = p => p.CreatedDate >= cutoffDate;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V2);

        // Assert
        Assert.Equal("(CreatedDate ge datetime'2024-06-01T00:00:00')", result);
    }

    [Fact]
    public void ODataFilterVisitor_DateTimeNow_V2_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var beforeNow = DateTime.Now;
        Expression<Func<TestProduct, bool>> expression = p => p.CreatedDate < DateTime.Now;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V2);
        var afterNow = DateTime.Now;

        // Assert
        Assert.StartsWith("(CreatedDate lt datetime'", result);
        Assert.EndsWith("')", result);
        // Verify the date format is correct (yyyy-MM-ddTHH:mm:ss)
        var dateStr = result.Substring("(CreatedDate lt datetime'".Length, 19);
        var parsedDate = DateTime.ParseExact(dateStr, "yyyy-MM-ddTHH:mm:ss", null);
        Assert.True(parsedDate >= beforeNow.AddSeconds(-1) && parsedDate <= afterNow.AddSeconds(1));
    }

    [Fact]
    public void ODataFilterVisitor_DateTimeUtcNow_V2_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var beforeNow = DateTime.UtcNow;
        Expression<Func<TestProduct, bool>> expression = p => p.CreatedDate < DateTime.UtcNow;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V2);
        var afterNow = DateTime.UtcNow;

        // Assert
        Assert.StartsWith("(CreatedDate lt datetime'", result);
        Assert.EndsWith("')", result);
        // Verify the date format is correct
        var dateStr = result.Substring("(CreatedDate lt datetime'".Length, 19);
        var parsedDate = DateTime.ParseExact(dateStr, "yyyy-MM-ddTHH:mm:ss", null);
        Assert.True(parsedDate >= beforeNow.AddSeconds(-1) && parsedDate <= afterNow.AddSeconds(1));
    }

    [Fact]
    public void ODataFilterVisitor_NullableDateTime_V2_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var testDate = new DateTime(2024, 12, 31, 23, 59, 59);
        Expression<Func<TestProduct, bool>> expression = p => p.DiscontinuedDate < testDate;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V2);

        // Assert
        Assert.Equal("(DiscontinuedDate lt datetime'2024-12-31T23:59:59')", result);
    }

    #endregion

    #region DateTime Tests - OData V4

    [Fact]
    public void ODataFilterVisitor_DateTimeConstant_V4_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var testDate = new DateTime(2024, 1, 15, 10, 30, 0);
        Expression<Func<TestProduct, bool>> expression = p => p.CreatedDate < testDate;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(CreatedDate lt datetime'2024-01-15T10:30:00')", result);
    }

    [Fact]
    public void ODataFilterVisitor_DateTimeVariable_V4_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var cutoffDate = new DateTime(2024, 6, 1, 0, 0, 0);
        Expression<Func<TestProduct, bool>> expression = p => p.CreatedDate >= cutoffDate;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(CreatedDate ge datetime'2024-06-01T00:00:00')", result);
    }

    [Fact]
    public void ODataFilterVisitor_DateTimeUtcNow_V4_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var beforeNow = DateTime.UtcNow;
        Expression<Func<TestProduct, bool>> expression = p => p.CreatedDate < DateTime.UtcNow;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);
        var afterNow = DateTime.UtcNow;

        // Assert
        Assert.StartsWith("(CreatedDate lt datetime'", result);
        Assert.EndsWith("')", result);
    }

    #endregion

    #region DateTimeOffset Tests - OData V2

    [Fact]
    public void ODataFilterVisitor_DateTimeOffsetConstant_V2_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var testDate = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        Expression<Func<TestProduct, bool>> expression = p => p.LastModified < testDate;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V2);

        // Assert
        // V2 should convert DateTimeOffset to datetime (without offset)
        Assert.StartsWith("(LastModified lt datetime'2024-01-15T", result);
        Assert.EndsWith("')", result);
    }

    [Fact]
    public void ODataFilterVisitor_NullableDateTimeOffset_V2_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var testDate = new DateTimeOffset(2024, 12, 31, 23, 59, 59, TimeSpan.Zero);
        Expression<Func<TestProduct, bool>> expression = p => p.LastChecked < testDate;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V2);

        // Assert
        Assert.StartsWith("(LastChecked lt datetime'2024-12-31T", result);
        Assert.EndsWith("')", result);
    }

    #endregion

    #region DateTimeOffset Tests - OData V4

    [Fact]
    public void ODataFilterVisitor_DateTimeOffsetConstant_V4_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var testDate = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        Expression<Func<TestProduct, bool>> expression = p => p.LastModified < testDate;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        // V4 should include offset information
        Assert.StartsWith("(LastModified lt datetimeoffset'2024-01-15T10:30:00", result);
        Assert.EndsWith("')", result);
        Assert.Contains("+02:00", result);
    }

    [Fact]
    public void ODataFilterVisitor_DateTimeOffsetUtcNow_V4_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.LastModified > DateTimeOffset.UtcNow;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.StartsWith("(LastModified gt datetimeoffset'", result);
        Assert.Contains("+00:00", result); // UTC should have +00:00 offset
        Assert.EndsWith("')", result);
    }

    #endregion

    #region Logical Operators Tests

    [Fact]
    public void ODataFilterVisitor_AndOperator_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Price > 10 && p.Stock < 100;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("((Price gt 10) and (Stock lt 100))", result);
    }

    [Fact]
    public void ODataFilterVisitor_OrOperator_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Price < 10 || p.Stock > 100;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("((Price lt 10) or (Stock gt 100))", result);
    }

    [Fact]
    public void ODataFilterVisitor_ComplexLogical_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => (p.Price > 10 && p.Stock < 100) || p.IsAvailable == false;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(((Price gt 10) and (Stock lt 100)) or (IsAvailable eq false))", result);
    }

    #endregion

    #region Nested Property Tests

    [Fact]
    public void ODataFilterVisitor_NestedProperty_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Category!.Name == "Electronics";

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(Category/Name eq 'Electronics')", result);
    }

    [Fact]
    public void ODataFilterVisitor_NestedPropertyWithNull_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Category!.Name == null;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(Category/Name eq null)", result);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void ODataFilterVisitor_DecimalValue_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Price == 99.99m;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        // The decimal separator may be culture-specific, so just verify the structure
        Assert.StartsWith("(Price eq 99", result);
        Assert.EndsWith("99)", result);
    }

    [Fact]
    public void ODataFilterVisitor_NullExpression_ThrowsException()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => visitor.ToFilter<TestProduct>(null!, ODataVersion.V4));
    }

    #endregion

    #region String Method Tests - OData V2

    [Fact]
    public void ODataFilterVisitor_StringContains_V2_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Name!.Contains("Widget");

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V2);

        // Assert
        // OData V2 uses substringof with reversed parameters
        Assert.Equal("substringof('Widget', Name)", result);
    }

    [Fact]
    public void ODataFilterVisitor_StringContainsWithVariable_V2_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var searchTerm = "Phone";
        Expression<Func<TestProduct, bool>> expression = p => p.Name!.Contains(searchTerm);

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V2);

        // Assert
        Assert.Equal("substringof('Phone', Name)", result);
    }

    [Fact]
    public void ODataFilterVisitor_StringStartsWith_V2_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Name!.StartsWith("Apple");

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V2);

        // Assert
        Assert.Equal("startswith(Name, 'Apple')", result);
    }

    [Fact]
    public void ODataFilterVisitor_StringEndsWith_V2_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Name!.EndsWith("Pro");

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V2);

        // Assert
        Assert.Equal("endswith(Name, 'Pro')", result);
    }

    #endregion

    #region String Method Tests - OData V3

    [Fact]
    public void ODataFilterVisitor_StringContains_V3_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Name!.Contains("Widget");

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V3);

        // Assert
        // OData V3 uses substringof with reversed parameters
        Assert.Equal("substringof('Widget', Name)", result);
    }

    [Fact]
    public void ODataFilterVisitor_StringStartsWith_V3_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Name!.StartsWith("Apple");

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V3);

        // Assert
        Assert.Equal("startswith(Name, 'Apple')", result);
    }

    [Fact]
    public void ODataFilterVisitor_StringEndsWith_V3_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Name!.EndsWith("Pro");

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V3);

        // Assert
        Assert.Equal("endswith(Name, 'Pro')", result);
    }

    #endregion

    #region String Method Tests - OData V4

    [Fact]
    public void ODataFilterVisitor_StringContains_V4_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Name!.Contains("Widget");

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        // OData V4 uses contains with normal parameter order
        Assert.Equal("contains(Name, 'Widget')", result);
    }

    [Fact]
    public void ODataFilterVisitor_StringContainsWithVariable_V4_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        var searchTerm = "Phone";
        Expression<Func<TestProduct, bool>> expression = p => p.Name!.Contains(searchTerm);

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("contains(Name, 'Phone')", result);
    }

    [Fact]
    public void ODataFilterVisitor_StringStartsWith_V4_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Name!.StartsWith("Apple");

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("startswith(Name, 'Apple')", result);
    }

    [Fact]
    public void ODataFilterVisitor_StringEndsWith_V4_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Name!.EndsWith("Pro");

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("endswith(Name, 'Pro')", result);
    }

    [Fact]
    public void ODataFilterVisitor_StringMethodsInComplexExpression_V4_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Name!.Contains("Phone") && p.Price > 500;

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(contains(Name, 'Phone') and (Price gt 500))", result);
    }

    [Fact]
    public void ODataFilterVisitor_MultipleStringMethods_V4_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Name!.StartsWith("Apple") || p.Name.EndsWith("Pro");

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("(startswith(Name, 'Apple') or endswith(Name, 'Pro'))", result);
    }

    [Fact]
    public void ODataFilterVisitor_StringContainsOnNestedProperty_V4_GeneratesCorrectFilter()
    {
        // Arrange
        var visitor = new ODataFilterVisitor();
        Expression<Func<TestProduct, bool>> expression = p => p.Category!.Name!.Contains("Electro");

        // Act
        var result = visitor.ToFilter(expression, ODataVersion.V4);

        // Assert
        Assert.Equal("contains(Category/Name, 'Electro')", result);
    }

    #endregion
}
