// LINQ Expression-Based Expand Usage Examples
// Supports both OData v2/v3 and v4 syntax automatically!

// Example 1: Simple expand - ExecuteAsync() directly after Expand()
var result = await queryBuilder
    .Expand(e => e.Orders)
    .ExecuteAsync();
// OData v2/v3: $expand=Orders
// OData v4:    $expand=Orders

// Example 2: Expand with additional filters
var result = await queryBuilder
    .Expand(e => e.Orders)
    .Filter(e => e.IsActive)
    .Top(10)
    .ExecuteAsync();
// OData v2/v3: $expand=Orders&$filter=(IsActive eq true)&$top=10
// OData v4:    $expand=Orders&$filter=(IsActive eq true)&$top=10

// Example 3: Nested expand with ThenExpand (single property)
var result = await queryBuilder
    .Expand(e => e.Order)
        .ThenExpand(o => o.Customer)
        .ThenExpand(c => c.Address)
    .ExecuteAsync();
// OData v2/v3: $expand=Order/Customer/Address
// OData v4:    $expand=Order($expand=Customer($expand=Address))

// Example 4: Nested expand on COLLECTION using Select()
var result = await queryBuilder
    .Expand(e => e.Orders)  // Orders is a collection
        .ThenExpand(orders => orders.Select(o => o.OrderDetails))  // Expand nested collection
        .ThenExpand(details => details.Select(d => d.Product))     // Further nesting
    .ExecuteAsync();
// OData v2/v3: $expand=Orders/OrderDetails/Product
// OData v4:    $expand=Orders($expand=OrderDetails($expand=Product))

// Example 5: Alternative syntax using ThenExpandCollection
var result = await queryBuilder
    .Expand(e => e.Orders)
        .ThenExpandCollection<Order, List<OrderDetail>>(o => o.OrderDetails)
        .ThenExpandCollection<OrderDetail, Product>(d => d.Product)
    .ExecuteAsync();
// OData v2/v3: $expand=Orders/OrderDetails/Product
// OData v4:    $expand=Orders($expand=OrderDetails($expand=Product))

// Example 6: Multiple expands at the same level (comma-separated in both versions)
var result = await queryBuilder
    .Expand(e => e.Orders)
    .Expand(e => e.Customer)
    .Expand(e => e.Address)
    .ExecuteAsync();
// OData v2/v3: $expand=Orders,Customer,Address
// OData v4:    $expand=Orders,Customer,Address

// Example 7: Multiple expands with nested expansions on collections
var result = await queryBuilder
    .Expand(e => e.Orders)
        .ThenExpand(orders => orders.Select(o => o.OrderDetails))
        .ThenExpand(details => details.Select(d => d.Product))
    .Expand(e => e.Customer)
        .ThenExpand(c => c.Address)
    .Expand(e => e.ShippingAddress)
    .ExecuteAsync();
// OData v2/v3: $expand=Orders/OrderDetails/Product,Customer/Address,ShippingAddress
// OData v4:    $expand=Orders($expand=OrderDetails($expand=Product)),Customer($expand=Address),ShippingAddress

// Example 8: Nested object properties (path-based expand)
var result = await queryBuilder
    .Expand(e => e.Customer.Address)
    .ExecuteAsync();
// OData v2/v3: $expand=Customer/Address
// OData v4:    $expand=Customer/Address

// Example 9: Expand collection with filter
var result = await queryBuilder
    .Expand(e => e.Orders)
        .ThenExpand(orders => orders.Select(o => o.OrderDetails))
    .Filter(e => e.Status == "Active")
    .Top(10)
    .Skip(20)
    .ExecuteAsync();
// OData v2/v3: $expand=Orders/OrderDetails&$filter=(Status eq 'Active')&$top=10&$skip=20
// OData v4:    $expand=Orders($expand=OrderDetails)&$filter=(Status eq 'Active')&$top=10&$skip=20

// Example 10: Real-world usage - OData v2 endpoint
var suppliersV2 = await clientV2
    .Query<Supplier>()
    .Expand(s => s.Products)
        .ThenExpand(products => products.Select(p => p.Category))
    .Filter(s => s.Country == "USA")
    .Top(50)
    .ExecuteAsync();
// Generates: $expand=Products/Category&$filter=(Country eq 'USA')&$top=50

// Example 11: Real-world usage - OData v4 endpoint
var suppliersV4 = await clientV4
    .Query<Supplier>()
    .Expand(s => s.Products)
        .ThenExpand(products => products.Select(p => p.Category))
    .Filter(s => s.Country == "USA")
    .Top(50)
    .ExecuteAsync();
// Generates: $expand=Products($expand=Category)&$filter=(Country eq 'USA')&$top=50

// Example 12: Multiple collection expands with filters
var activeSuppliers = await clientV2
    .Query<Supplier>()
    .Expand(s => s.Products)
        .ThenExpand(products => products.Select(p => p.Manufacturer))
    .Expand(s => s.Locations)
        .ThenExpand(locations => locations.Select(l => l.Address))
    .Filter(s => s.IsActive && s.Rating > 4)
    .Skip(10)
    .Top(20)
    .ExecuteAsync();
// OData v2/v3: $expand=Products/Manufacturer,Locations/Address&$filter=...
// OData v4:    $expand=Products($expand=Manufacturer),Locations($expand=Address)&$filter=...

// Example 13: Deep nested collection expands
var detailedOrders = await client
    .Query<Order>()
    .Expand(o => o.Customer)
        .ThenExpand(c => c.Address)
            .ThenExpand(a => a.Country)
    .Filter(o => o.OrderDate > DateTime.Now.AddDays(-30))
    .ExecuteAsync();
// OData v2/v3: $expand=Customer/Address/Country&$filter=...
// OData v4:    $expand=Customer($expand=Address($expand=Country))&$filter=...

