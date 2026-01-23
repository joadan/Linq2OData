using Linq2OData.Generator.Metadata;
using Linq2OData.Generator.Models;

namespace Linq2OData.Tests
{
    public class MetadataTests
    {
        private string odataDemoMetadataV2;
        private string sapSalesQuotationMetadataV2;

        public MetadataTests()
        {

            odataDemoMetadataV2 =  File.ReadAllText(Path.Combine("SampleData", "Metadata", "V2", "ODataDemo.xml"));
            sapSalesQuotationMetadataV2 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V2", "SapSalesQuotation.xml"));
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
            Assert.Contains("SalesQuotation", salesQuotationEntity.Keys);
            
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
            Assert.Equal(3, metadata.EntityTypes.Count);
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
            Assert.Single(productEntity.Keys);
            Assert.Contains("ID", productEntity.Keys);
            
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
            Assert.Single(categoryEntity.Keys);
            Assert.Contains("ID", categoryEntity.Keys);
            
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
            Assert.Single(supplierEntity.Keys);
            Assert.Contains("ID", supplierEntity.Keys);
            
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
    }
}
