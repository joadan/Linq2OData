// LINQ Expression-Based Order Usage Examples

// Example 1: Simple ascending order
var result = await queryBuilder
    .Order(s => s.Name)
    .ExecuteAsync();
// Generates: $orderby=Name

// Example 2: Simple descending order
var result = await queryBuilder
    .OrderDescending(s => s.Price)
    .ExecuteAsync();
// Generates: $orderby=Price desc

// Example 3: Multiple orderby with ThenBy
var result = await queryBuilder
    .Order(s => s.Country)
        .ThenBy(s => s.Name)
        .ThenBy(s => s.ID)
    .ExecuteAsync();
// Generates: $orderby=Country,Name,ID

// Example 4: Mixed ascending and descending with ThenByDescending
var result = await queryBuilder
    .Order(s => s.Country)
        .ThenByDescending(s => s.Rating)
        .ThenBy(s => s.Name)
    .ExecuteAsync();
// Generates: $orderby=Country,Rating desc,Name

// Example 5: Order with nested property paths
var result = await queryBuilder
    .Order(s => s.Address.City)
    .ExecuteAsync();
// Generates: $orderby=Address/City

// Example 6: Complex ordering with nested properties
var result = await queryBuilder
    .Order(s => s.Address.Country)
        .ThenBy(s => s.Address.City)
        .ThenByDescending(s => s.CreatedDate)
    .ExecuteAsync();
// Generates: $orderby=Address/Country,Address/City,CreatedDate desc

// Example 7: Order combined with Filter and Top
var result = await queryBuilder
    .Filter(s => s.IsActive)
    .Order(s => s.Name)
    .Top(20)
    .ExecuteAsync();
// Generates: $filter=(IsActive eq true)&$orderby=Name&$top=20

// Example 8: Full query with Order, Expand, Filter
var result = await queryBuilder
    .Expand(s => s.Products)
    .Filter(s => s.Country == "USA")
    .Order(s => s.Rating)
        .ThenByDescending(s => s.Name)
    .Top(50)
    .ExecuteAsync();
// Generates: $expand=Products&$filter=(Country eq 'USA')&$orderby=Rating,Name desc&$top=50

// Example 9: Order after Expand with nested expands
var result = await queryBuilder
    .Expand(s => s.Products)
        .ThenExpand(products => products.Select(p => p.Category))
    .OrderDescending(s => s.CreatedDate)
        .ThenBy(s => s.Name)
    .ExecuteAsync();
// Generates: $expand=Products($expand=Category)&$orderby=CreatedDate desc,Name

// Example 10: Real-world usage - paginated sorted list
var suppliers = await clientV2
    .Query<Supplier>()
    .Filter(s => s.IsActive && s.Country == "USA")
    .Order(s => s.Rating)
        .ThenByDescending(s => s.JoinDate)
        .ThenBy(s => s.Name)
    .Skip(20)
    .Top(10)
    .ExecuteAsync();

// Example 11: Order with expand and filter
var activeProducts = await client
    .Query<Product>()
    .Expand(p => p.Category)
    .Expand(p => p.Manufacturer)
    .Filter(p => p.InStock && p.Price > 10)
    .Order(p => p.Category.Name)
        .ThenBy(p => p.Name)
    .Top(100)
    .ExecuteAsync();

// Example 12: Multiple orderings for report generation
var salesReport = await client
    .Query<Order>()
    .Filter(o => o.OrderDate > DateTime.Now.AddMonths(-3))
    .OrderDescending(o => o.OrderDate)
        .ThenBy(o => o.Customer.Name)
        .ThenBy(o => o.OrderNumber)
    .Count(true)
    .ExecuteAsync();

// Example 13: String-based orderby (legacy support)
var result = await queryBuilder
    .Order("Name,Price desc")
    .ExecuteAsync();
// Generates: $orderby=Name,Price desc

// Example 14: Order with Select projection
var projected = await queryBuilder
    .Order(s => s.Rating)
        .ThenBy(s => s.Name)
    .Select()
    .ExecuteAsync();
