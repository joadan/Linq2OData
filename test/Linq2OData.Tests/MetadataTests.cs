using Linq2OData.Generator.Metadata;
using Linq2OData.Generator.Models;

namespace Linq2OData.Tests
{
    public class MetadataTests
    {
        private string odataDemoMetadataV2;
        private string sapSalesQuotationMetadataV2;
        private string odataDemoMetadataV4;

        public MetadataTests()
        {

            odataDemoMetadataV2 =  File.ReadAllText(Path.Combine("SampleData", "Metadata", "V2", "ODataDemo.xml"));
            sapSalesQuotationMetadataV2 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V2", "SapSalesQuotation.xml"));
            odataDemoMetadataV4 =  File.ReadAllText(Path.Combine("SampleData", "Metadata", "V4", "ODataDemo.xml"));
        }


        [Fact]
        public void ParseSalesQuotationMetadata_ShouldParseSuccessfully()
        {
            // Act
            var metadata = MetadataParser.Parse(sapSalesQuotationMetadataV2);

            // Assert
            Assert.NotNull(metadata);
            Assert.NotEmpty(metadata.EntityTypes);
            
            // Verify the A_SalesQuotationType entity exists
            var salesQuotationEntity = metadata.EntityTypes.FirstOrDefault(e => e.Name == "A_SalesQuotationType");
            Assert.NotNull(salesQuotationEntity);
            Assert.Equal("Sales Quotation Header", salesQuotationEntity.Label);
            
            // Verify keys
            Assert.Contains("SalesQuotation", salesQuotationEntity.KeyProperties.Select(e=> e.Name));
            
            // Verify some properties exist
            Assert.NotEmpty(salesQuotationEntity.Properties);
            var salesQuotationProp = salesQuotationEntity.Properties.FirstOrDefault(p => p.Name == "SalesQuotation");
            Assert.NotNull(salesQuotationProp);
            Assert.Equal("Edm.String", salesQuotationProp.DataType);
            Assert.False(salesQuotationProp.Nullable);
            Assert.Equal(10, salesQuotationProp.MaxLength);
        }

        [Fact]
        public void ParseSalesQuotationMetadata_ShouldParseFunctions()
        {
           
            // Act
            var metadata = MetadataParser.Parse(sapSalesQuotationMetadataV2);

            // Assert - Verify services are parsed
            Assert.NotNull(metadata.Functions);
            Assert.NotEmpty(metadata.Functions);
            
            // Verify releaseApprovalRequest service
            var releaseService = metadata.Functions.FirstOrDefault(s => s.Name == "releaseApprovalRequest");
            Assert.NotNull(releaseService);
            Assert.Equal("API_SALES_QUOTATION_SRV.FunctionResult", releaseService.ReturnType);
            Assert.Equal("POST", releaseService.HttpMethod);
            
            // Verify service parameters
            Assert.NotEmpty(releaseService.Parameters);
            var salesQuotationParam = releaseService.Parameters.FirstOrDefault(p => p.Name == "SalesQuotation");
            Assert.NotNull(salesQuotationParam);
            Assert.Equal("Edm.String", salesQuotationParam.DataType);
            Assert.Equal(11000, salesQuotationParam.MaxLength);
            Assert.Equal("In", salesQuotationParam.Mode);
            
            // Verify rejectApprovalRequest service
            var rejectService = metadata.Functions.FirstOrDefault(s => s.Name == "rejectApprovalRequest");
            Assert.NotNull(rejectService);
            Assert.Equal("API_SALES_QUOTATION_SRV.FunctionResult", rejectService.ReturnType);
            Assert.Equal("POST", rejectService.HttpMethod);
        }

        [Fact]
        public void ParseSalesQuotationMetadata_ShouldParseEntitySets()
        {
           
            // Act
            var metadata = MetadataParser.Parse(sapSalesQuotationMetadataV2);

            // Assert - Verify entity sets are parsed
            Assert.NotNull(metadata.EntitySets);
            Assert.NotEmpty(metadata.EntitySets);
            
            // Verify A_SalesQuotation entity set
            var salesQuotationSet = metadata.EntitySets.FirstOrDefault(es => es.Name == "A_SalesQuotation");
            Assert.NotNull(salesQuotationSet);
            Assert.Equal("A_SalesQuotationType", salesQuotationSet.EntityTypeName);
            
            // Verify A_SalesQuotationItem entity set
            var salesQuotationItemSet = metadata.EntitySets.FirstOrDefault(es => es.Name == "A_SalesQuotationItem");
            Assert.NotNull(salesQuotationItemSet);
            Assert.Equal("A_SalesQuotationItemType", salesQuotationItemSet.EntityTypeName);
            
            // Verify A_SalesQuotationPartner entity set
            var partnerSet = metadata.EntitySets.FirstOrDefault(es => es.Name == "A_SalesQuotationPartner");
            Assert.NotNull(partnerSet);
            Assert.Equal("A_SalesQuotationPartnerType", partnerSet.EntityTypeName);
            
            // Verify count (should have 14 entity sets based on the XML)
            Assert.Equal(14, metadata.EntitySets.Count);
        }

        [Fact]
        public void ParseSalesQuotationMetadata_ShouldParseVersionAndNamespace()
        {
          
            // Act
            var metadata = MetadataParser.Parse(sapSalesQuotationMetadataV2);

            // Assert - Verify version and namespace are parsed
            Assert.Equal(ODataVersion.V2, metadata.ODataVersion);
            Assert.NotNull(metadata.Namespace);
            Assert.NotEmpty(metadata.Namespace);
            Assert.Equal("API_SALES_QUOTATION_SRV", metadata.Namespace);
        }

        [Fact]
        public void ParseSampleServiceMetadata_ShouldParseSuccessfully()
        {
           
            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV2);

            // Assert
            Assert.NotNull(metadata);
            Assert.NotEmpty(metadata.EntityTypes);
            
            // Verify namespace and version
            Assert.Equal("ODataDemo", metadata.Namespace);
            Assert.Equal(ODataVersion.V2, metadata.ODataVersion);
            
            // Verify entity types count
            Assert.Equal(4, metadata.EntityTypes.Count);
        }

        [Fact]
        public void ParseSampleServiceMetadata_ShouldParseProductEntity()
        {
          
            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV2);

            // Assert - Verify Product entity
            var productEntity = metadata.EntityTypes.FirstOrDefault(e => e.Name == "Product");
            Assert.NotNull(productEntity);
            
            // Verify key
            Assert.Single(productEntity.KeyProperties);
            Assert.Contains("ID", productEntity.KeyProperties.Select(e=> e.Name));
            
            // Verify properties
            Assert.Equal(7, productEntity.Properties.Count);
            
            var idProperty = productEntity.Properties.FirstOrDefault(p => p.Name == "ID");
            Assert.NotNull(idProperty);
            Assert.Equal("Edm.Int32", idProperty.DataType);
            Assert.False(idProperty.Nullable);
            
            var nameProperty = productEntity.Properties.FirstOrDefault(p => p.Name == "Name");
            Assert.NotNull(nameProperty);
            Assert.Equal("Edm.String", nameProperty.DataType);
            
            var priceProperty = productEntity.Properties.FirstOrDefault(p => p.Name == "Price");
            Assert.NotNull(priceProperty);
            Assert.Equal("Edm.Decimal", priceProperty.DataType);
            Assert.False(priceProperty.Nullable);
            
            // Verify navigations
            Assert.Equal(2, productEntity.Navigations.Count);
            
            var categoryNav = productEntity.Navigations.FirstOrDefault(n => n.Name == "Category");
            Assert.NotNull(categoryNav);
            Assert.Equal("Category", categoryNav.ToEntity);
            
            var supplierNav = productEntity.Navigations.FirstOrDefault(n => n.Name == "Supplier");
            Assert.NotNull(supplierNav);
            Assert.Equal("Supplier", supplierNav.ToEntity);
        }

        [Fact]
        public void ParseSampleServiceMetadata_ShouldParseCategoryEntity()
        {
         
            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV2);

            // Assert - Verify Category entity
            var categoryEntity = metadata.EntityTypes.FirstOrDefault(e => e.Name == "Category");
            Assert.NotNull(categoryEntity);
            
            // Verify key
            Assert.Single(categoryEntity.KeyProperties);
            Assert.Contains("ID", categoryEntity.KeyProperties.Select(e=>e.Name));
            
            // Verify properties
            Assert.Equal(2, categoryEntity.Properties.Count);
            
            var idProperty = categoryEntity.Properties.FirstOrDefault(p => p.Name == "ID");
            Assert.NotNull(idProperty);
            Assert.Equal("Edm.Int32", idProperty.DataType);
            Assert.False(idProperty.Nullable);
            
            var nameProperty = categoryEntity.Properties.FirstOrDefault(p => p.Name == "Name");
            Assert.NotNull(nameProperty);
            Assert.Equal("Edm.String", nameProperty.DataType);
            
            // Verify navigation
            Assert.Single(categoryEntity.Navigations);
            var productsNav = categoryEntity.Navigations.FirstOrDefault(n => n.Name == "Products");
            Assert.NotNull(productsNav);
            Assert.Equal("Product", productsNav.ToEntity);
        }

        [Fact]
        public void ParseSampleServiceMetadata_ShouldParseSupplierEntity()
        {
          
            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV2);

            // Assert - Verify Supplier entity
            var supplierEntity = metadata.EntityTypes.FirstOrDefault(e => e.Name == "Supplier");
            Assert.NotNull(supplierEntity);
            
            // Verify key
            Assert.Single(supplierEntity.KeyProperties);
            Assert.Contains("ID", supplierEntity.KeyProperties.Select(e=>e.Name));
            
            // Verify properties
            Assert.Equal(4, supplierEntity.Properties.Count);
            
            var idProperty = supplierEntity.Properties.FirstOrDefault(p => p.Name == "ID");
            Assert.NotNull(idProperty);
            Assert.Equal("Edm.Int32", idProperty.DataType);
            Assert.False(idProperty.Nullable);
            
            var addressProperty = supplierEntity.Properties.FirstOrDefault(p => p.Name == "Address");
            Assert.NotNull(addressProperty);
            Assert.Equal("ODataDemo.Address", addressProperty.DataType);
            Assert.False(addressProperty.Nullable);
            
            // Verify navigation
            Assert.Single(supplierEntity.Navigations);
            var productsNav = supplierEntity.Navigations.FirstOrDefault(n => n.Name == "Products");
            Assert.NotNull(productsNav);
            Assert.Equal("Product", productsNav.ToEntity);
        }

        [Fact]
        public void ParseSampleServiceMetadata_ShouldParseEntitySets()
        {
 
            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV2);

            // Assert - Verify entity sets
            Assert.NotNull(metadata.EntitySets);
            Assert.Equal(3, metadata.EntitySets.Count);
            
            var productsSet = metadata.EntitySets.FirstOrDefault(es => es.Name == "Products");
            Assert.NotNull(productsSet);
            Assert.Equal("Product", productsSet.EntityTypeName);
            
            var categoriesSet = metadata.EntitySets.FirstOrDefault(es => es.Name == "Categories");
            Assert.NotNull(categoriesSet);
            Assert.Equal("Category", categoriesSet.EntityTypeName);
            
            var suppliersSet = metadata.EntitySets.FirstOrDefault(es => es.Name == "Suppliers");
            Assert.NotNull(suppliersSet);
            Assert.Equal("Supplier", suppliersSet.EntityTypeName);
        }

        [Fact]
        public void ParseSampleServiceMetadata_ShouldParseFunctionImport()
        {

            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV2);

            // Assert - Verify function import
            Assert.NotNull(metadata.Functions);
            Assert.Single(metadata.Functions);
            
            var getProductsFunc = metadata.Functions.FirstOrDefault(f => f.Name == "GetProductsByRating");
            Assert.NotNull(getProductsFunc);
            Assert.Equal("Collection(ODataDemo.Product)", getProductsFunc.ReturnType);
            Assert.Equal("GET", getProductsFunc.HttpMethod);
            
            // Verify function parameter
            Assert.Single(getProductsFunc.Parameters);
            var ratingParam = getProductsFunc.Parameters.FirstOrDefault(p => p.Name == "rating");
            Assert.NotNull(ratingParam);
            Assert.Equal("Edm.Int32", ratingParam.DataType);
        }

        [Fact]
        public void ParseSampleServiceMetadata_ShouldParseNavigationTypes()
        {
            // Arrange
        
            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV2);

            // Assert - Verify navigation multiplicity
            var productEntity = metadata.EntityTypes.FirstOrDefault(e => e.Name == "Product");
            Assert.NotNull(productEntity);
            
            // Product to Category is 0..1 (ZeroOrOne)
            var categoryNav = productEntity.Navigations.FirstOrDefault(n => n.Name == "Category");
            Assert.NotNull(categoryNav);
            Assert.Equal(ODataNavigationType.ZeroOrOne, categoryNav.NavigationType);
            
            // Category to Products is * (Many)
            var categoryEntity = metadata.EntityTypes.FirstOrDefault(e => e.Name == "Category");
            Assert.NotNull(categoryEntity);
            var productsNav = categoryEntity.Navigations.FirstOrDefault(n => n.Name == "Products");
            Assert.NotNull(productsNav);
            Assert.Equal(ODataNavigationType.Many, productsNav.NavigationType);
        }

        #region OData V4 Tests

        [Fact]
        public void ParseODataDemoV4Metadata_ShouldParseVersionCorrectly()
        {
            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV4);

            // Assert
            Assert.NotNull(metadata);
            Assert.Equal(ODataVersion.V4, metadata.ODataVersion);
            Assert.Equal("ODataDemo", metadata.Namespace);
        }

        [Fact]
        public void ParseODataDemoV4Metadata_ShouldParseEntityTypes()
        {
            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV4);

            // Assert
            Assert.NotNull(metadata.EntityTypes);
            Assert.NotEmpty(metadata.EntityTypes);
            
            // Verify Product entity exists
            var productEntity = metadata.EntityTypes.FirstOrDefault(e => e.Name == "Product");
            Assert.NotNull(productEntity);
            
            // Verify key
            Assert.Single(productEntity.KeyProperties);
            Assert.Contains("ID", productEntity.KeyProperties.Select(e => e.Name));
            
            // Verify properties
            var idProperty = productEntity.Properties.FirstOrDefault(p => p.Name == "ID");
            Assert.NotNull(idProperty);
            Assert.Equal("Edm.Int32", idProperty.DataType);
            Assert.False(idProperty.Nullable);
            
            var nameProperty = productEntity.Properties.FirstOrDefault(p => p.Name == "Name");
            Assert.NotNull(nameProperty);
            Assert.Equal("Edm.String", nameProperty.DataType);
            
            var priceProperty = productEntity.Properties.FirstOrDefault(p => p.Name == "Price");
            Assert.NotNull(priceProperty);
            Assert.Equal("Edm.Double", priceProperty.DataType);
            Assert.False(priceProperty.Nullable);
        }

        [Fact]
        public void ParseODataDemoV4Metadata_ShouldParseNavigationProperties()
        {
            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV4);

            // Assert
            var productEntity = metadata.EntityTypes.FirstOrDefault(e => e.Name == "Product");
            Assert.NotNull(productEntity);
            
            // Verify navigation properties
            Assert.Equal(3, productEntity.Navigations.Count);
            
            // Verify Categories navigation (Collection)
            var categoriesNav = productEntity.Navigations.FirstOrDefault(n => n.Name == "Categories");
            Assert.NotNull(categoriesNav);
            Assert.Equal("Category", categoriesNav.ToEntity);
            Assert.Equal(ODataNavigationType.Many, categoriesNav.NavigationType);
            
            // Verify Supplier navigation (single)
            var supplierNav = productEntity.Navigations.FirstOrDefault(n => n.Name == "Supplier");
            Assert.NotNull(supplierNav);
            Assert.Equal("Supplier", supplierNav.ToEntity);
            Assert.Equal(ODataNavigationType.ZeroOrOne, supplierNav.NavigationType);
            
            // Verify ProductDetail navigation (single)
            var detailNav = productEntity.Navigations.FirstOrDefault(n => n.Name == "ProductDetail");
            Assert.NotNull(detailNav);
            Assert.Equal("ProductDetail", detailNav.ToEntity);
            Assert.Equal(ODataNavigationType.ZeroOrOne, detailNav.NavigationType);
        }

        [Fact]
        public void ParseODataDemoV4Metadata_ShouldParseComplexTypes()
        {
            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV4);

            // Assert
            var addressType = metadata.EntityTypes.FirstOrDefault(e => e.Name == "Address");
            Assert.NotNull(addressType);
            
            // Verify Address complex type properties
            Assert.Equal(5, addressType.Properties.Count);
            
            var streetProperty = addressType.Properties.FirstOrDefault(p => p.Name == "Street");
            Assert.NotNull(streetProperty);
            Assert.Equal("Edm.String", streetProperty.DataType);
            
            var cityProperty = addressType.Properties.FirstOrDefault(p => p.Name == "City");
            Assert.NotNull(cityProperty);
            Assert.Equal("Edm.String", cityProperty.DataType);
        }

        [Fact]
        public void ParseODataDemoV4Metadata_ShouldParseEntitySets()
        {
            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV4);

            // Assert
            Assert.NotNull(metadata.EntitySets);
            Assert.NotEmpty(metadata.EntitySets);
            Assert.Equal(7, metadata.EntitySets.Count);
            
            // Verify Products entity set
            var productsSet = metadata.EntitySets.FirstOrDefault(es => es.Name == "Products");
            Assert.NotNull(productsSet);
            Assert.Equal("Product", productsSet.EntityTypeName);
            Assert.NotNull(productsSet.EntityType);
            
            // Verify Categories entity set
            var categoriesSet = metadata.EntitySets.FirstOrDefault(es => es.Name == "Categories");
            Assert.NotNull(categoriesSet);
            Assert.Equal("Category", categoriesSet.EntityTypeName);
            
            // Verify Suppliers entity set
            var suppliersSet = metadata.EntitySets.FirstOrDefault(es => es.Name == "Suppliers");
            Assert.NotNull(suppliersSet);
            Assert.Equal("Supplier", suppliersSet.EntityTypeName);
        }

        [Fact]
        public void ParseODataDemoV4Metadata_ShouldParseActionImports()
        {
            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV4);

            // Assert
            Assert.NotNull(metadata.Functions);
            Assert.Single(metadata.Functions);
            
            // Verify IncreaseSalaries action
            var increaseSalariesAction = metadata.Functions.FirstOrDefault(f => f.Name == "IncreaseSalaries");
            Assert.NotNull(increaseSalariesAction);
            Assert.Equal("POST", increaseSalariesAction.HttpMethod);
            
            // Verify action parameter
            Assert.Single(increaseSalariesAction.Parameters);
            var percentageParam = increaseSalariesAction.Parameters.FirstOrDefault(p => p.Name == "percentage");
            Assert.NotNull(percentageParam);
            Assert.Equal("Edm.Int32", percentageParam.DataType);
        }

        [Fact]
        public void ParseODataDemoV4Metadata_ShouldParseEntityInheritance()
        {
            // Act
            var metadata = MetadataParser.Parse(odataDemoMetadataV4);

            // Assert - Verify derived types exist
            var featuredProductEntity = metadata.EntityTypes.FirstOrDefault(e => e.Name == "FeaturedProduct");
            Assert.NotNull(featuredProductEntity);
            
            // FeaturedProduct should have its own navigation property
            var advertisementNav = featuredProductEntity.Navigations.FirstOrDefault(n => n.Name == "Advertisement");
            Assert.NotNull(advertisementNav);
            Assert.Equal("Advertisement", advertisementNav.ToEntity);
            
            // Verify Customer (derived from Person)
            var customerEntity = metadata.EntityTypes.FirstOrDefault(e => e.Name == "Customer");
            Assert.NotNull(customerEntity);
            
            var totalExpenseProperty = customerEntity.Properties.FirstOrDefault(p => p.Name == "TotalExpense");
            Assert.NotNull(totalExpenseProperty);
            Assert.Equal("Edm.Decimal", totalExpenseProperty.DataType);
            
            // Verify Employee (derived from Person)
            var employeeEntity = metadata.EntityTypes.FirstOrDefault(e => e.Name == "Employee");
            Assert.NotNull(employeeEntity);
            
            var employeeIdProperty = employeeEntity.Properties.FirstOrDefault(p => p.Name == "EmployeeID");
            Assert.NotNull(employeeIdProperty);
            Assert.Equal("Edm.Int64", employeeIdProperty.DataType);
        }

        #endregion
    }
}
