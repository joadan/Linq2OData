using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2OData.TestClients.AdHocClient
{
    internal static class SampleData
    {

		internal const string SalesOrderCollection = @"{""d"":{""results"":[{""__metadata"":{""id"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')"",""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderType"",""etag"":""W/\""datetimeoffset'2005-10-24T15%3A36%3A52.0000000Z'\""""},""SalesOrder"":""4000043"",""SalesOrderType"":""ZOS"",""SalesOrganization"":""4010"",""DistributionChannel"":""40"",""OrganizationDivision"":""40"",""SalesGroup"":"""",""SalesOffice"":""UVE"",""SalesDistrict"":"""",""SoldToParty"":""4502"",""CreationDate"":""\/Date(1130112000000)\/"",""CreatedByUser"":""GPICCIN"",""LastChangeDate"":""\/Date(1148601600000)\/"",""SenderBusinessSystemName"":"""",""ExternalDocumentID"":"""",""LastChangeDateTime"":""\/Date(1130168212000+0000)\/"",""ExternalDocLastChangeDateTime"":null,""PurchaseOrderByCustomer"":""16306"",""PurchaseOrderByShipToParty"":"""",""CustomerPurchaseOrderType"":"""",""CustomerPurchaseOrderDate"":""\/Date(1129248000000)\/"",""SalesOrderDate"":""\/Date(1130112000000)\/"",""TotalNetAmount"":""2656.24"",""OverallDeliveryStatus"":""C"",""TotalBlockStatus"":"""",""OverallOrdReltdBillgStatus"":"""",""OverallSDDocReferenceStatus"":"""",""TransactionCurrency"":""EUR"",""SDDocumentReason"":"""",""PricingDate"":""\/Date(1130112000000)\/"",""PriceDetnExchangeRate"":""1.00000"",""BillingPlan"":"""",""RequestedDeliveryDate"":""\/Date(1130457600000)\/"",""ShippingCondition"":""01"",""CompleteDeliveryIsDefined"":false,""ShippingType"":"""",""HeaderBillingBlockReason"":"""",""DeliveryBlockReason"":"""",""DeliveryDateTypeRule"":"""",""IncotermsClassification"":""EXW"",""IncotermsTransferLocation"":"""",""IncotermsLocation1"":"""",""IncotermsLocation2"":"""",""IncotermsVersion"":"""",""CustomerPriceGroup"":"""",""PriceListType"":""04"",""CustomerPaymentTerms"":""B114"",""PaymentMethod"":"""",""FixedValueDate"":null,""AssignmentReference"":"""",""ReferenceSDDocument"":"""",""ReferenceSDDocumentCategory"":"""",""AccountingDocExternalReference"":"""",""CustomerAccountAssignmentGroup"":""02"",""AccountingExchangeRate"":""0.00000"",""CorrespncExternalReference"":"""",""SlsDocSo2PLastContactPersnName"":"""",""SlsDocSo2PLstCntctPersnTelNmbr"":""0061297504666"",""POCorrespncExternalReference"":"""",""CustomerConditionGroup1"":"""",""CustomerConditionGroup2"":"""",""CustomerConditionGroup3"":"""",""CustomerConditionGroup4"":"""",""CustomerConditionGroup5"":"""",""CustomerGroup"":""04"",""AdditionalCustomerGroup1"":"""",""AdditionalCustomerGroup2"":"""",""AdditionalCustomerGroup3"":"""",""AdditionalCustomerGroup4"":"""",""AdditionalCustomerGroup5"":"""",""SlsDocIsRlvtForProofOfDeliv"":false,""CustomerTaxClassification1"":"""",""CustomerTaxClassification2"":"""",""CustomerTaxClassification3"":"""",""CustomerTaxClassification4"":"""",""CustomerTaxClassification5"":"""",""CustomerTaxClassification6"":"""",""CustomerTaxClassification7"":"""",""CustomerTaxClassification8"":"""",""CustomerTaxClassification9"":"""",""TaxDepartureCountry"":"""",""VATRegistrationCountry"":"""",""SalesOrderApprovalReason"":"""",""SalesDocApprovalStatus"":"""",""OverallSDProcessStatus"":""C"",""TotalCreditCheckStatus"":"""",""OverallTotalDeliveryStatus"":""C"",""OverallSDDocumentRejectionSts"":""A"",""BillingDocumentDate"":""\/Date(1130457600000)\/"",""ContractAccount"":"""",""AdditionalValueDays"":""0"",""CustomerPurchaseOrderSuplmnt"":"""",""ServicesRenderedDate"":null,""to_BillingPlan"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_BillingPlan""}},""to_Item"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_Item""}},""to_Partner"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_Partner""}},""to_PaymentPlanItemDetails"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_PaymentPlanItemDetails""}},""to_PrecedingProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_PrecedingProcFlowDoc""}},""to_PricingElement"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_PricingElement""}},""to_RelatedObject"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_RelatedObject""}},""to_SubsequentProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_SubsequentProcFlowDoc""}},""to_Text"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_Text""}}},{""__metadata"":{""id"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000045')"",""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000045')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderType"",""etag"":""W/\""datetimeoffset'2005-10-24T17%3A16%3A34.0000000Z'\""""},""SalesOrder"":""4000045"",""SalesOrderType"":""ZOS"",""SalesOrganization"":""4010"",""DistributionChannel"":""40"",""OrganizationDivision"":""40"",""SalesGroup"":"""",""SalesOffice"":""UVE"",""SalesDistrict"":"""",""SoldToParty"":""8585"",""CreationDate"":""\/Date(1130112000000)\/"",""CreatedByUser"":""GPICCIN"",""LastChangeDate"":""\/Date(1148601600000)\/"",""SenderBusinessSystemName"":"""",""ExternalDocumentID"":"""",""LastChangeDateTime"":""\/Date(1130174194000+0000)\/"",""ExternalDocLastChangeDateTime"":null,""PurchaseOrderByCustomer"":""ALTY05 PRCS 005"",""PurchaseOrderByShipToParty"":"""",""CustomerPurchaseOrderType"":"""",""CustomerPurchaseOrderDate"":""\/Date(1130112000000)\/"",""SalesOrderDate"":""\/Date(1130112000000)\/"",""TotalNetAmount"":""3330.60"",""OverallDeliveryStatus"":""C"",""TotalBlockStatus"":"""",""OverallOrdReltdBillgStatus"":""C"",""OverallSDDocReferenceStatus"":"""",""TransactionCurrency"":""EUR"",""SDDocumentReason"":"""",""PricingDate"":""\/Date(1130112000000)\/"",""PriceDetnExchangeRate"":""1.00000"",""BillingPlan"":""0000000671"",""RequestedDeliveryDate"":""\/Date(1133136000000)\/"",""ShippingCondition"":""01"",""CompleteDeliveryIsDefined"":false,""ShippingType"":"""",""HeaderBillingBlockReason"":"""",""DeliveryBlockReason"":"""",""DeliveryDateTypeRule"":"""",""IncotermsClassification"":""EXW"",""IncotermsTransferLocation"":""CANTARANA - CONA (VE)"",""IncotermsLocation1"":"""",""IncotermsLocation2"":"""",""IncotermsVersion"":"""",""CustomerPriceGroup"":"""",""PriceListType"":""04"",""CustomerPaymentTerms"":""L016"",""PaymentMethod"":"""",""FixedValueDate"":null,""AssignmentReference"":"""",""ReferenceSDDocument"":"""",""ReferenceSDDocumentCategory"":"""",""AccountingDocExternalReference"":"""",""CustomerAccountAssignmentGroup"":""02"",""AccountingExchangeRate"":""0.00000"",""CorrespncExternalReference"":"""",""SlsDocSo2PLastContactPersnName"":"""",""SlsDocSo2PLstCntctPersnTelNmbr"":""00902122206727"",""POCorrespncExternalReference"":"""",""CustomerConditionGroup1"":"""",""CustomerConditionGroup2"":"""",""CustomerConditionGroup3"":"""",""CustomerConditionGroup4"":"""",""CustomerConditionGroup5"":"""",""CustomerGroup"":""02"",""AdditionalCustomerGroup1"":"""",""AdditionalCustomerGroup2"":"""",""AdditionalCustomerGroup3"":"""",""AdditionalCustomerGroup4"":"""",""AdditionalCustomerGroup5"":"""",""SlsDocIsRlvtForProofOfDeliv"":false,""CustomerTaxClassification1"":"""",""CustomerTaxClassification2"":"""",""CustomerTaxClassification3"":"""",""CustomerTaxClassification4"":"""",""CustomerTaxClassification5"":"""",""CustomerTaxClassification6"":"""",""CustomerTaxClassification7"":"""",""CustomerTaxClassification8"":"""",""CustomerTaxClassification9"":"""",""TaxDepartureCountry"":"""",""VATRegistrationCountry"":"""",""SalesOrderApprovalReason"":"""",""SalesDocApprovalStatus"":"""",""OverallSDProcessStatus"":""C"",""TotalCreditCheckStatus"":""A"",""OverallTotalDeliveryStatus"":""C"",""OverallSDDocumentRejectionSts"":""A"",""BillingDocumentDate"":""\/Date(1133136000000)\/"",""ContractAccount"":"""",""AdditionalValueDays"":""0"",""CustomerPurchaseOrderSuplmnt"":"""",""ServicesRenderedDate"":null,""to_BillingPlan"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000045')/to_BillingPlan""}},""to_Item"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000045')/to_Item""}},""to_Partner"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000045')/to_Partner""}},""to_PaymentPlanItemDetails"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000045')/to_PaymentPlanItemDetails""}},""to_PrecedingProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000045')/to_PrecedingProcFlowDoc""}},""to_PricingElement"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000045')/to_PricingElement""}},""to_RelatedObject"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000045')/to_RelatedObject""}},""to_SubsequentProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000045')/to_SubsequentProcFlowDoc""}},""to_Text"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000045')/to_Text""}}},{""__metadata"":{""id"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000046')"",""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000046')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderType"",""etag"":""W/\""datetimeoffset'2005-10-24T17%3A25%3A23.0000000Z'\""""},""SalesOrder"":""4000046"",""SalesOrderType"":""ZOS"",""SalesOrganization"":""4010"",""DistributionChannel"":""40"",""OrganizationDivision"":""40"",""SalesGroup"":"""",""SalesOffice"":""UVE"",""SalesDistrict"":""007"",""SoldToParty"":""8300"",""CreationDate"":""\/Date(1130112000000)\/"",""CreatedByUser"":""SROMANATO"",""LastChangeDate"":""\/Date(1148601600000)\/"",""SenderBusinessSystemName"":"""",""ExternalDocumentID"":"""",""LastChangeDateTime"":""\/Date(1130174723000+0000)\/"",""ExternalDocLastChangeDateTime"":null,""PurchaseOrderByCustomer"":"""",""PurchaseOrderByShipToParty"":"""",""CustomerPurchaseOrderType"":"""",""CustomerPurchaseOrderDate"":""\/Date(1130112000000)\/"",""SalesOrderDate"":""\/Date(1130112000000)\/"",""TotalNetAmount"":""26752.16"",""OverallDeliveryStatus"":""C"",""TotalBlockStatus"":"""",""OverallOrdReltdBillgStatus"":"""",""OverallSDDocReferenceStatus"":"""",""TransactionCurrency"":""EUR"",""SDDocumentReason"":"""",""PricingDate"":""\/Date(1130112000000)\/"",""PriceDetnExchangeRate"":""1.00000"",""BillingPlan"":""0000000471"",""RequestedDeliveryDate"":""\/Date(1130889600000)\/"",""ShippingCondition"":""01"",""CompleteDeliveryIsDefined"":false,""ShippingType"":"""",""HeaderBillingBlockReason"":"""",""DeliveryBlockReason"":"""",""DeliveryDateTypeRule"":"""",""IncotermsClassification"":""EXW"",""IncotermsTransferLocation"":"""",""IncotermsLocation1"":"""",""IncotermsLocation2"":"""",""IncotermsVersion"":"""",""CustomerPriceGroup"":"""",""PriceListType"":""03"",""CustomerPaymentTerms"":""B114"",""PaymentMethod"":"""",""FixedValueDate"":null,""AssignmentReference"":"""",""ReferenceSDDocument"":"""",""ReferenceSDDocumentCategory"":"""",""AccountingDocExternalReference"":"""",""CustomerAccountAssignmentGroup"":""02"",""AccountingExchangeRate"":""0.00000"",""CorrespncExternalReference"":"""",""SlsDocSo2PLastContactPersnName"":"""",""SlsDocSo2PLstCntctPersnTelNmbr"":""004989326700"",""POCorrespncExternalReference"":"""",""CustomerConditionGroup1"":"""",""CustomerConditionGroup2"":"""",""CustomerConditionGroup3"":"""",""CustomerConditionGroup4"":"""",""CustomerConditionGroup5"":"""",""CustomerGroup"":""01"",""AdditionalCustomerGroup1"":"""",""AdditionalCustomerGroup2"":"""",""AdditionalCustomerGroup3"":"""",""AdditionalCustomerGroup4"":"""",""AdditionalCustomerGroup5"":"""",""SlsDocIsRlvtForProofOfDeliv"":false,""CustomerTaxClassification1"":"""",""CustomerTaxClassification2"":"""",""CustomerTaxClassification3"":"""",""CustomerTaxClassification4"":"""",""CustomerTaxClassification5"":"""",""CustomerTaxClassification6"":"""",""CustomerTaxClassification7"":"""",""CustomerTaxClassification8"":"""",""CustomerTaxClassification9"":"""",""TaxDepartureCountry"":"""",""VATRegistrationCountry"":"""",""SalesOrderApprovalReason"":"""",""SalesDocApprovalStatus"":"""",""OverallSDProcessStatus"":""C"",""TotalCreditCheckStatus"":"""",""OverallTotalDeliveryStatus"":""C"",""OverallSDDocumentRejectionSts"":""A"",""BillingDocumentDate"":""\/Date(1130889600000)\/"",""ContractAccount"":"""",""AdditionalValueDays"":""0"",""CustomerPurchaseOrderSuplmnt"":"""",""ServicesRenderedDate"":null,""to_BillingPlan"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000046')/to_BillingPlan""}},""to_Item"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000046')/to_Item""}},""to_Partner"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000046')/to_Partner""}},""to_PaymentPlanItemDetails"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000046')/to_PaymentPlanItemDetails""}},""to_PrecedingProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000046')/to_PrecedingProcFlowDoc""}},""to_PricingElement"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000046')/to_PricingElement""}},""to_RelatedObject"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000046')/to_RelatedObject""}},""to_SubsequentProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000046')/to_SubsequentProcFlowDoc""}},""to_Text"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000046')/to_Text""}}},{""__metadata"":{""id"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000047')"",""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000047')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderType"",""etag"":""W/\""datetimeoffset'2005-10-24T17%3A28%3A15.0000000Z'\""""},""SalesOrder"":""4000047"",""SalesOrderType"":""ZOS"",""SalesOrganization"":""4010"",""DistributionChannel"":""40"",""OrganizationDivision"":""40"",""SalesGroup"":"""",""SalesOffice"":""UVE"",""SalesDistrict"":"""",""SoldToParty"":""120004"",""CreationDate"":""\/Date(1130112000000)\/"",""CreatedByUser"":""RBELLEMO"",""LastChangeDate"":""\/Date(1148601600000)\/"",""SenderBusinessSystemName"":"""",""ExternalDocumentID"":"""",""LastChangeDateTime"":""\/Date(1130174895000+0000)\/"",""ExternalDocLastChangeDateTime"":null,""PurchaseOrderByCustomer"":""211005-1"",""PurchaseOrderByShipToParty"":"""",""CustomerPurchaseOrderType"":"""",""CustomerPurchaseOrderDate"":""\/Date(1129852800000)\/"",""SalesOrderDate"":""\/Date(1130112000000)\/"",""TotalNetAmount"":""8483.28"",""OverallDeliveryStatus"":""C"",""TotalBlockStatus"":"""",""OverallOrdReltdBillgStatus"":""C"",""OverallSDDocReferenceStatus"":"""",""TransactionCurrency"":""EUR"",""SDDocumentReason"":"""",""PricingDate"":""\/Date(1130112000000)\/"",""PriceDetnExchangeRate"":""1.00000"",""BillingPlan"":""0000000461"",""RequestedDeliveryDate"":""\/Date(1131667200000)\/"",""ShippingCondition"":""01"",""CompleteDeliveryIsDefined"":false,""ShippingType"":"""",""HeaderBillingBlockReason"":"""",""DeliveryBlockReason"":"""",""DeliveryDateTypeRule"":"""",""IncotermsClassification"":""EXW"",""IncotermsTransferLocation"":"""",""IncotermsLocation1"":"""",""IncotermsLocation2"":"""",""IncotermsVersion"":"""",""CustomerPriceGroup"":"""",""PriceListType"":""04"",""CustomerPaymentTerms"":""B003"",""PaymentMethod"":"""",""FixedValueDate"":null,""AssignmentReference"":"""",""ReferenceSDDocument"":"""",""ReferenceSDDocumentCategory"":"""",""AccountingDocExternalReference"":"""",""CustomerAccountAssignmentGroup"":""02"",""AccountingExchangeRate"":""0.00000"",""CorrespncExternalReference"":"""",""SlsDocSo2PLastContactPersnName"":"""",""SlsDocSo2PLstCntctPersnTelNmbr"":""00380444625272"",""POCorrespncExternalReference"":"""",""CustomerConditionGroup1"":"""",""CustomerConditionGroup2"":"""",""CustomerConditionGroup3"":"""",""CustomerConditionGroup4"":"""",""CustomerConditionGroup5"":"""",""CustomerGroup"":""04"",""AdditionalCustomerGroup1"":"""",""AdditionalCustomerGroup2"":"""",""AdditionalCustomerGroup3"":"""",""AdditionalCustomerGroup4"":"""",""AdditionalCustomerGroup5"":"""",""SlsDocIsRlvtForProofOfDeliv"":false,""CustomerTaxClassification1"":"""",""CustomerTaxClassification2"":"""",""CustomerTaxClassification3"":"""",""CustomerTaxClassification4"":"""",""CustomerTaxClassification5"":"""",""CustomerTaxClassification6"":"""",""CustomerTaxClassification7"":"""",""CustomerTaxClassification8"":"""",""CustomerTaxClassification9"":"""",""TaxDepartureCountry"":"""",""VATRegistrationCountry"":"""",""SalesOrderApprovalReason"":"""",""SalesDocApprovalStatus"":"""",""OverallSDProcessStatus"":""C"",""TotalCreditCheckStatus"":""A"",""OverallTotalDeliveryStatus"":""C"",""OverallSDDocumentRejectionSts"":""A"",""BillingDocumentDate"":""\/Date(1131667200000)\/"",""ContractAccount"":"""",""AdditionalValueDays"":""0"",""CustomerPurchaseOrderSuplmnt"":"""",""ServicesRenderedDate"":null,""to_BillingPlan"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000047')/to_BillingPlan""}},""to_Item"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000047')/to_Item""}},""to_Partner"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000047')/to_Partner""}},""to_PaymentPlanItemDetails"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000047')/to_PaymentPlanItemDetails""}},""to_PrecedingProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000047')/to_PrecedingProcFlowDoc""}},""to_PricingElement"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000047')/to_PricingElement""}},""to_RelatedObject"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000047')/to_RelatedObject""}},""to_SubsequentProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000047')/to_SubsequentProcFlowDoc""}},""to_Text"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000047')/to_Text""}}},{""__metadata"":{""id"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000048')"",""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000048')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderType"",""etag"":""W/\""datetimeoffset'2005-10-25T10%3A10%3A21.0000000Z'\""""},""SalesOrder"":""4000048"",""SalesOrderType"":""ZOS"",""SalesOrganization"":""4010"",""DistributionChannel"":""40"",""OrganizationDivision"":""40"",""SalesGroup"":"""",""SalesOffice"":""UVE"",""SalesDistrict"":""022"",""SoldToParty"":""6950"",""CreationDate"":""\/Date(1130198400000)\/"",""CreatedByUser"":""RBELLEMO"",""LastChangeDate"":""\/Date(1148601600000)\/"",""SenderBusinessSystemName"":"""",""ExternalDocumentID"":"""",""LastChangeDateTime"":""\/Date(1130235021000+0000)\/"",""ExternalDocLastChangeDateTime"":null,""PurchaseOrderByCustomer"":""PO-2947"",""PurchaseOrderByShipToParty"":"""",""CustomerPurchaseOrderType"":"""",""CustomerPurchaseOrderDate"":""\/Date(1130112000000)\/"",""SalesOrderDate"":""\/Date(1130198400000)\/"",""TotalNetAmount"":""4235.39"",""OverallDeliveryStatus"":""C"",""TotalBlockStatus"":"""",""OverallOrdReltdBillgStatus"":"""",""OverallSDDocReferenceStatus"":"""",""TransactionCurrency"":""EUR"",""SDDocumentReason"":""Z01"",""PricingDate"":""\/Date(1130198400000)\/"",""PriceDetnExchangeRate"":""1.00000"",""BillingPlan"":"""",""RequestedDeliveryDate"":""\/Date(1131667200000)\/"",""ShippingCondition"":""01"",""CompleteDeliveryIsDefined"":false,""ShippingType"":"""",""HeaderBillingBlockReason"":"""",""DeliveryBlockReason"":"""",""DeliveryDateTypeRule"":"""",""IncotermsClassification"":""EXW"",""IncotermsTransferLocation"":"""",""IncotermsLocation1"":"""",""IncotermsLocation2"":"""",""IncotermsVersion"":"""",""CustomerPriceGroup"":"""",""PriceListType"":""04"",""CustomerPaymentTerms"":""B107"",""PaymentMethod"":"""",""FixedValueDate"":null,""AssignmentReference"":"""",""ReferenceSDDocument"":"""",""ReferenceSDDocumentCategory"":"""",""AccountingDocExternalReference"":"""",""CustomerAccountAssignmentGroup"":""02"",""AccountingExchangeRate"":""0.00000"",""CorrespncExternalReference"":"""",""SlsDocSo2PLastContactPersnName"":"""",""SlsDocSo2PLstCntctPersnTelNmbr"":""00441214864886"",""POCorrespncExternalReference"":"""",""CustomerConditionGroup1"":"""",""CustomerConditionGroup2"":"""",""CustomerConditionGroup3"":"""",""CustomerConditionGroup4"":"""",""CustomerConditionGroup5"":"""",""CustomerGroup"":""01"",""AdditionalCustomerGroup1"":"""",""AdditionalCustomerGroup2"":"""",""AdditionalCustomerGroup3"":"""",""AdditionalCustomerGroup4"":"""",""AdditionalCustomerGroup5"":"""",""SlsDocIsRlvtForProofOfDeliv"":false,""CustomerTaxClassification1"":"""",""CustomerTaxClassification2"":"""",""CustomerTaxClassification3"":"""",""CustomerTaxClassification4"":"""",""CustomerTaxClassification5"":"""",""CustomerTaxClassification6"":"""",""CustomerTaxClassification7"":"""",""CustomerTaxClassification8"":"""",""CustomerTaxClassification9"":"""",""TaxDepartureCountry"":"""",""VATRegistrationCountry"":"""",""SalesOrderApprovalReason"":"""",""SalesDocApprovalStatus"":"""",""OverallSDProcessStatus"":""C"",""TotalCreditCheckStatus"":"""",""OverallTotalDeliveryStatus"":""C"",""OverallSDDocumentRejectionSts"":""A"",""BillingDocumentDate"":""\/Date(1131667200000)\/"",""ContractAccount"":"""",""AdditionalValueDays"":""0"",""CustomerPurchaseOrderSuplmnt"":"""",""ServicesRenderedDate"":null,""to_BillingPlan"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000048')/to_BillingPlan""}},""to_Item"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000048')/to_Item""}},""to_Partner"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000048')/to_Partner""}},""to_PaymentPlanItemDetails"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000048')/to_PaymentPlanItemDetails""}},""to_PrecedingProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000048')/to_PrecedingProcFlowDoc""}},""to_PricingElement"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000048')/to_PricingElement""}},""to_RelatedObject"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000048')/to_RelatedObject""}},""to_SubsequentProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000048')/to_SubsequentProcFlowDoc""}},""to_Text"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000048')/to_Text""}}},{""__metadata"":{""id"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000049')"",""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000049')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderType"",""etag"":""W/\""datetimeoffset'2005-10-25T10%3A33%3A59.0000000Z'\""""},""SalesOrder"":""4000049"",""SalesOrderType"":""ZOS"",""SalesOrganization"":""4010"",""DistributionChannel"":""40"",""OrganizationDivision"":""40"",""SalesGroup"":"""",""SalesOffice"":""UVE"",""SalesDistrict"":"""",""SoldToParty"":""4502"",""CreationDate"":""\/Date(1130198400000)\/"",""CreatedByUser"":""GPICCIN"",""LastChangeDate"":""\/Date(1148601600000)\/"",""SenderBusinessSystemName"":"""",""ExternalDocumentID"":"""",""LastChangeDateTime"":""\/Date(1130236439000+0000)\/"",""ExternalDocLastChangeDateTime"":null,""PurchaseOrderByCustomer"":""16289"",""PurchaseOrderByShipToParty"":"""",""CustomerPurchaseOrderType"":"""",""CustomerPurchaseOrderDate"":""\/Date(1129248000000)\/"",""SalesOrderDate"":""\/Date(1130198400000)\/"",""TotalNetAmount"":""8605.70"",""OverallDeliveryStatus"":""C"",""TotalBlockStatus"":"""",""OverallOrdReltdBillgStatus"":"""",""OverallSDDocReferenceStatus"":"""",""TransactionCurrency"":""EUR"",""SDDocumentReason"":"""",""PricingDate"":""\/Date(1130198400000)\/"",""PriceDetnExchangeRate"":""1.00000"",""BillingPlan"":"""",""RequestedDeliveryDate"":""\/Date(1130457600000)\/"",""ShippingCondition"":""01"",""CompleteDeliveryIsDefined"":false,""ShippingType"":"""",""HeaderBillingBlockReason"":"""",""DeliveryBlockReason"":"""",""DeliveryDateTypeRule"":"""",""IncotermsClassification"":""EXW"",""IncotermsTransferLocation"":"""",""IncotermsLocation1"":"""",""IncotermsLocation2"":"""",""IncotermsVersion"":"""",""CustomerPriceGroup"":"""",""PriceListType"":""04"",""CustomerPaymentTerms"":""B114"",""PaymentMethod"":"""",""FixedValueDate"":null,""AssignmentReference"":"""",""ReferenceSDDocument"":"""",""ReferenceSDDocumentCategory"":"""",""AccountingDocExternalReference"":"""",""CustomerAccountAssignmentGroup"":""02"",""AccountingExchangeRate"":""0.00000"",""CorrespncExternalReference"":"""",""SlsDocSo2PLastContactPersnName"":"""",""SlsDocSo2PLstCntctPersnTelNmbr"":""0061297504666"",""POCorrespncExternalReference"":"""",""CustomerConditionGroup1"":"""",""CustomerConditionGroup2"":"""",""CustomerConditionGroup3"":"""",""CustomerConditionGroup4"":"""",""CustomerConditionGroup5"":"""",""CustomerGroup"":""04"",""AdditionalCustomerGroup1"":"""",""AdditionalCustomerGroup2"":"""",""AdditionalCustomerGroup3"":"""",""AdditionalCustomerGroup4"":"""",""AdditionalCustomerGroup5"":"""",""SlsDocIsRlvtForProofOfDeliv"":false,""CustomerTaxClassification1"":"""",""CustomerTaxClassification2"":"""",""CustomerTaxClassification3"":"""",""CustomerTaxClassification4"":"""",""CustomerTaxClassification5"":"""",""CustomerTaxClassification6"":"""",""CustomerTaxClassification7"":"""",""CustomerTaxClassification8"":"""",""CustomerTaxClassification9"":"""",""TaxDepartureCountry"":"""",""VATRegistrationCountry"":"""",""SalesOrderApprovalReason"":"""",""SalesDocApprovalStatus"":"""",""OverallSDProcessStatus"":""C"",""TotalCreditCheckStatus"":"""",""OverallTotalDeliveryStatus"":""C"",""OverallSDDocumentRejectionSts"":""A"",""BillingDocumentDate"":""\/Date(1130457600000)\/"",""ContractAccount"":"""",""AdditionalValueDays"":""0"",""CustomerPurchaseOrderSuplmnt"":"""",""ServicesRenderedDate"":null,""to_BillingPlan"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000049')/to_BillingPlan""}},""to_Item"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000049')/to_Item""}},""to_Partner"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000049')/to_Partner""}},""to_PaymentPlanItemDetails"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000049')/to_PaymentPlanItemDetails""}},""to_PrecedingProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000049')/to_PrecedingProcFlowDoc""}},""to_PricingElement"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000049')/to_PricingElement""}},""to_RelatedObject"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000049')/to_RelatedObject""}},""to_SubsequentProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000049')/to_SubsequentProcFlowDoc""}},""to_Text"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000049')/to_Text""}}},{""__metadata"":{""id"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000050')"",""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000050')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderType"",""etag"":""W/\""datetimeoffset'2005-10-25T10%3A43%3A51.0000000Z'\""""},""SalesOrder"":""4000050"",""SalesOrderType"":""ZOS"",""SalesOrganization"":""4010"",""DistributionChannel"":""40"",""OrganizationDivision"":""40"",""SalesGroup"":"""",""SalesOffice"":""UVE"",""SalesDistrict"":"""",""SoldToParty"":""4502"",""CreationDate"":""\/Date(1130198400000)\/"",""CreatedByUser"":""GPICCIN"",""LastChangeDate"":""\/Date(1148601600000)\/"",""SenderBusinessSystemName"":"""",""ExternalDocumentID"":"""",""LastChangeDateTime"":""\/Date(1130237031000+0000)\/"",""ExternalDocLastChangeDateTime"":null,""PurchaseOrderByCustomer"":""16413 - RLG191005"",""PurchaseOrderByShipToParty"":"""",""CustomerPurchaseOrderType"":"""",""CustomerPurchaseOrderDate"":""\/Date(1129680000000)\/"",""SalesOrderDate"":""\/Date(1130198400000)\/"",""TotalNetAmount"":""10874.57"",""OverallDeliveryStatus"":""C"",""TotalBlockStatus"":"""",""OverallOrdReltdBillgStatus"":"""",""OverallSDDocReferenceStatus"":"""",""TransactionCurrency"":""EUR"",""SDDocumentReason"":"""",""PricingDate"":""\/Date(1130198400000)\/"",""PriceDetnExchangeRate"":""1.00000"",""BillingPlan"":"""",""RequestedDeliveryDate"":""\/Date(1131062400000)\/"",""ShippingCondition"":""01"",""CompleteDeliveryIsDefined"":false,""ShippingType"":"""",""HeaderBillingBlockReason"":"""",""DeliveryBlockReason"":"""",""DeliveryDateTypeRule"":"""",""IncotermsClassification"":""EXW"",""IncotermsTransferLocation"":"""",""IncotermsLocation1"":"""",""IncotermsLocation2"":"""",""IncotermsVersion"":"""",""CustomerPriceGroup"":"""",""PriceListType"":""04"",""CustomerPaymentTerms"":""B114"",""PaymentMethod"":"""",""FixedValueDate"":null,""AssignmentReference"":"""",""ReferenceSDDocument"":"""",""ReferenceSDDocumentCategory"":"""",""AccountingDocExternalReference"":"""",""CustomerAccountAssignmentGroup"":""02"",""AccountingExchangeRate"":""0.00000"",""CorrespncExternalReference"":"""",""SlsDocSo2PLastContactPersnName"":"""",""SlsDocSo2PLstCntctPersnTelNmbr"":""0061297504666"",""POCorrespncExternalReference"":"""",""CustomerConditionGroup1"":"""",""CustomerConditionGroup2"":"""",""CustomerConditionGroup3"":"""",""CustomerConditionGroup4"":"""",""CustomerConditionGroup5"":"""",""CustomerGroup"":""04"",""AdditionalCustomerGroup1"":"""",""AdditionalCustomerGroup2"":"""",""AdditionalCustomerGroup3"":"""",""AdditionalCustomerGroup4"":"""",""AdditionalCustomerGroup5"":"""",""SlsDocIsRlvtForProofOfDeliv"":false,""CustomerTaxClassification1"":"""",""CustomerTaxClassification2"":"""",""CustomerTaxClassification3"":"""",""CustomerTaxClassification4"":"""",""CustomerTaxClassification5"":"""",""CustomerTaxClassification6"":"""",""CustomerTaxClassification7"":"""",""CustomerTaxClassification8"":"""",""CustomerTaxClassification9"":"""",""TaxDepartureCountry"":"""",""VATRegistrationCountry"":"""",""SalesOrderApprovalReason"":"""",""SalesDocApprovalStatus"":"""",""OverallSDProcessStatus"":""C"",""TotalCreditCheckStatus"":"""",""OverallTotalDeliveryStatus"":""C"",""OverallSDDocumentRejectionSts"":""A"",""BillingDocumentDate"":""\/Date(1131062400000)\/"",""ContractAccount"":"""",""AdditionalValueDays"":""0"",""CustomerPurchaseOrderSuplmnt"":"""",""ServicesRenderedDate"":null,""to_BillingPlan"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000050')/to_BillingPlan""}},""to_Item"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000050')/to_Item""}},""to_Partner"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000050')/to_Partner""}},""to_PaymentPlanItemDetails"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000050')/to_PaymentPlanItemDetails""}},""to_PrecedingProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000050')/to_PrecedingProcFlowDoc""}},""to_PricingElement"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000050')/to_PricingElement""}},""to_RelatedObject"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000050')/to_RelatedObject""}},""to_SubsequentProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000050')/to_SubsequentProcFlowDoc""}},""to_Text"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000050')/to_Text""}}},{""__metadata"":{""id"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000051')"",""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000051')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderType"",""etag"":""W/\""datetimeoffset'2005-10-25T12%3A10%3A17.0000000Z'\""""},""SalesOrder"":""4000051"",""SalesOrderType"":""ZOS"",""SalesOrganization"":""4010"",""DistributionChannel"":""40"",""OrganizationDivision"":""40"",""SalesGroup"":"""",""SalesOffice"":""UVE"",""SalesDistrict"":"""",""SoldToParty"":""7905"",""CreationDate"":""\/Date(1130198400000)\/"",""CreatedByUser"":""GPICCIN"",""LastChangeDate"":""\/Date(1148601600000)\/"",""SenderBusinessSystemName"":"""",""ExternalDocumentID"":"""",""LastChangeDateTime"":""\/Date(1130242217000+0000)\/"",""ExternalDocLastChangeDateTime"":null,""PurchaseOrderByCustomer"":""CO05021497"",""PurchaseOrderByShipToParty"":"""",""CustomerPurchaseOrderType"":"""",""CustomerPurchaseOrderDate"":""\/Date(1129161600000)\/"",""SalesOrderDate"":""\/Date(1130198400000)\/"",""TotalNetAmount"":""2932.38"",""OverallDeliveryStatus"":""C"",""TotalBlockStatus"":"""",""OverallOrdReltdBillgStatus"":"""",""OverallSDDocReferenceStatus"":"""",""TransactionCurrency"":""EUR"",""SDDocumentReason"":"""",""PricingDate"":""\/Date(1130198400000)\/"",""PriceDetnExchangeRate"":""1.00000"",""BillingPlan"":"""",""RequestedDeliveryDate"":""\/Date(1130198400000)\/"",""ShippingCondition"":""01"",""CompleteDeliveryIsDefined"":false,""ShippingType"":"""",""HeaderBillingBlockReason"":"""",""DeliveryBlockReason"":"""",""DeliveryDateTypeRule"":"""",""IncotermsClassification"":""EXW"",""IncotermsTransferLocation"":"""",""IncotermsLocation1"":"""",""IncotermsLocation2"":"""",""IncotermsVersion"":"""",""CustomerPriceGroup"":"""",""PriceListType"":""04"",""CustomerPaymentTerms"":""B107"",""PaymentMethod"":"""",""FixedValueDate"":null,""AssignmentReference"":"""",""ReferenceSDDocument"":"""",""ReferenceSDDocumentCategory"":"""",""AccountingDocExternalReference"":"""",""CustomerAccountAssignmentGroup"":""02"",""AccountingExchangeRate"":""0.00000"",""CorrespncExternalReference"":"""",""SlsDocSo2PLastContactPersnName"":"""",""SlsDocSo2PLstCntctPersnTelNmbr"":"""",""POCorrespncExternalReference"":"""",""CustomerConditionGroup1"":"""",""CustomerConditionGroup2"":"""",""CustomerConditionGroup3"":"""",""CustomerConditionGroup4"":"""",""CustomerConditionGroup5"":"""",""CustomerGroup"":""04"",""AdditionalCustomerGroup1"":"""",""AdditionalCustomerGroup2"":"""",""AdditionalCustomerGroup3"":"""",""AdditionalCustomerGroup4"":"""",""AdditionalCustomerGroup5"":"""",""SlsDocIsRlvtForProofOfDeliv"":false,""CustomerTaxClassification1"":"""",""CustomerTaxClassification2"":"""",""CustomerTaxClassification3"":"""",""CustomerTaxClassification4"":"""",""CustomerTaxClassification5"":"""",""CustomerTaxClassification6"":"""",""CustomerTaxClassification7"":"""",""CustomerTaxClassification8"":"""",""CustomerTaxClassification9"":"""",""TaxDepartureCountry"":"""",""VATRegistrationCountry"":"""",""SalesOrderApprovalReason"":"""",""SalesDocApprovalStatus"":"""",""OverallSDProcessStatus"":""C"",""TotalCreditCheckStatus"":"""",""OverallTotalDeliveryStatus"":""C"",""OverallSDDocumentRejectionSts"":""A"",""BillingDocumentDate"":""\/Date(1130198400000)\/"",""ContractAccount"":"""",""AdditionalValueDays"":""0"",""CustomerPurchaseOrderSuplmnt"":"""",""ServicesRenderedDate"":null,""to_BillingPlan"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000051')/to_BillingPlan""}},""to_Item"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000051')/to_Item""}},""to_Partner"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000051')/to_Partner""}},""to_PaymentPlanItemDetails"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000051')/to_PaymentPlanItemDetails""}},""to_PrecedingProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000051')/to_PrecedingProcFlowDoc""}},""to_PricingElement"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000051')/to_PricingElement""}},""to_RelatedObject"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000051')/to_RelatedObject""}},""to_SubsequentProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000051')/to_SubsequentProcFlowDoc""}},""to_Text"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000051')/to_Text""}}},{""__metadata"":{""id"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000052')"",""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000052')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderType"",""etag"":""W/\""datetimeoffset'2005-10-25T13%3A56%3A25.0000000Z'\""""},""SalesOrder"":""4000052"",""SalesOrderType"":""ZOS"",""SalesOrganization"":""4010"",""DistributionChannel"":""40"",""OrganizationDivision"":""40"",""SalesGroup"":"""",""SalesOffice"":""UVE"",""SalesDistrict"":"""",""SoldToParty"":""1751"",""CreationDate"":""\/Date(1130198400000)\/"",""CreatedByUser"":""SROMANATO"",""LastChangeDate"":""\/Date(1148601600000)\/"",""SenderBusinessSystemName"":"""",""ExternalDocumentID"":"""",""LastChangeDateTime"":""\/Date(1130248585000+0000)\/"",""ExternalDocLastChangeDateTime"":null,""PurchaseOrderByCustomer"":""17450/AB1309"",""PurchaseOrderByShipToParty"":"""",""CustomerPurchaseOrderType"":"""",""CustomerPurchaseOrderDate"":""\/Date(1129161600000)\/"",""SalesOrderDate"":""\/Date(1130198400000)\/"",""TotalNetAmount"":""10930.40"",""OverallDeliveryStatus"":""C"",""TotalBlockStatus"":"""",""OverallOrdReltdBillgStatus"":"""",""OverallSDDocReferenceStatus"":"""",""TransactionCurrency"":""EUR"",""SDDocumentReason"":"""",""PricingDate"":""\/Date(1130198400000)\/"",""PriceDetnExchangeRate"":""1.00000"",""BillingPlan"":"""",""RequestedDeliveryDate"":""\/Date(1131062400000)\/"",""ShippingCondition"":""01"",""CompleteDeliveryIsDefined"":false,""ShippingType"":"""",""HeaderBillingBlockReason"":"""",""DeliveryBlockReason"":"""",""DeliveryDateTypeRule"":"""",""IncotermsClassification"":""EXW"",""IncotermsTransferLocation"":"""",""IncotermsLocation1"":"""",""IncotermsLocation2"":"""",""IncotermsVersion"":"""",""CustomerPriceGroup"":"""",""PriceListType"":""04"",""CustomerPaymentTerms"":""B114"",""PaymentMethod"":"""",""FixedValueDate"":null,""AssignmentReference"":"""",""ReferenceSDDocument"":"""",""ReferenceSDDocumentCategory"":"""",""AccountingDocExternalReference"":"""",""CustomerAccountAssignmentGroup"":""02"",""AccountingExchangeRate"":""0.00000"",""CorrespncExternalReference"":"""",""SlsDocSo2PLastContactPersnName"":"""",""SlsDocSo2PLstCntctPersnTelNmbr"":""004318691638"",""POCorrespncExternalReference"":"""",""CustomerConditionGroup1"":"""",""CustomerConditionGroup2"":"""",""CustomerConditionGroup3"":"""",""CustomerConditionGroup4"":"""",""CustomerConditionGroup5"":"""",""CustomerGroup"":""04"",""AdditionalCustomerGroup1"":"""",""AdditionalCustomerGroup2"":"""",""AdditionalCustomerGroup3"":"""",""AdditionalCustomerGroup4"":"""",""AdditionalCustomerGroup5"":"""",""SlsDocIsRlvtForProofOfDeliv"":false,""CustomerTaxClassification1"":"""",""CustomerTaxClassification2"":"""",""CustomerTaxClassification3"":"""",""CustomerTaxClassification4"":"""",""CustomerTaxClassification5"":"""",""CustomerTaxClassification6"":"""",""CustomerTaxClassification7"":"""",""CustomerTaxClassification8"":"""",""CustomerTaxClassification9"":"""",""TaxDepartureCountry"":"""",""VATRegistrationCountry"":"""",""SalesOrderApprovalReason"":"""",""SalesDocApprovalStatus"":"""",""OverallSDProcessStatus"":""C"",""TotalCreditCheckStatus"":"""",""OverallTotalDeliveryStatus"":""C"",""OverallSDDocumentRejectionSts"":""A"",""BillingDocumentDate"":""\/Date(1131062400000)\/"",""ContractAccount"":"""",""AdditionalValueDays"":""0"",""CustomerPurchaseOrderSuplmnt"":"""",""ServicesRenderedDate"":null,""to_BillingPlan"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000052')/to_BillingPlan""}},""to_Item"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000052')/to_Item""}},""to_Partner"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000052')/to_Partner""}},""to_PaymentPlanItemDetails"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000052')/to_PaymentPlanItemDetails""}},""to_PrecedingProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000052')/to_PrecedingProcFlowDoc""}},""to_PricingElement"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000052')/to_PricingElement""}},""to_RelatedObject"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000052')/to_RelatedObject""}},""to_SubsequentProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000052')/to_SubsequentProcFlowDoc""}},""to_Text"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000052')/to_Text""}}},{""__metadata"":{""id"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000053')"",""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000053')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderType"",""etag"":""W/\""datetimeoffset'2005-10-25T13%3A53%3A26.0000000Z'\""""},""SalesOrder"":""4000053"",""SalesOrderType"":""ZOS"",""SalesOrganization"":""4010"",""DistributionChannel"":""40"",""OrganizationDivision"":""40"",""SalesGroup"":"""",""SalesOffice"":""UVE"",""SalesDistrict"":"""",""SoldToParty"":""100555"",""CreationDate"":""\/Date(1130198400000)\/"",""CreatedByUser"":""GPICCIN"",""LastChangeDate"":""\/Date(1148601600000)\/"",""SenderBusinessSystemName"":"""",""ExternalDocumentID"":"""",""LastChangeDateTime"":""\/Date(1130248406000+0000)\/"",""ExternalDocLastChangeDateTime"":null,""PurchaseOrderByCustomer"":""E-MAIL 04/09/05 JULIUS SHI"",""PurchaseOrderByShipToParty"":"""",""CustomerPurchaseOrderType"":"""",""CustomerPurchaseOrderDate"":""\/Date(1125792000000)\/"",""SalesOrderDate"":""\/Date(1130198400000)\/"",""TotalNetAmount"":""38442.25"",""OverallDeliveryStatus"":""C"",""TotalBlockStatus"":"""",""OverallOrdReltdBillgStatus"":"""",""OverallSDDocReferenceStatus"":"""",""TransactionCurrency"":""EUR"",""SDDocumentReason"":"""",""PricingDate"":""\/Date(1130198400000)\/"",""PriceDetnExchangeRate"":""1.00000"",""BillingPlan"":"""",""RequestedDeliveryDate"":""\/Date(1130457600000)\/"",""ShippingCondition"":""01"",""CompleteDeliveryIsDefined"":false,""ShippingType"":"""",""HeaderBillingBlockReason"":"""",""DeliveryBlockReason"":"""",""DeliveryDateTypeRule"":"""",""IncotermsClassification"":""EXW"",""IncotermsTransferLocation"":"""",""IncotermsLocation1"":"""",""IncotermsLocation2"":"""",""IncotermsVersion"":"""",""CustomerPriceGroup"":"""",""PriceListType"":""03"",""CustomerPaymentTerms"":""B004"",""PaymentMethod"":"""",""FixedValueDate"":null,""AssignmentReference"":"""",""ReferenceSDDocument"":"""",""ReferenceSDDocumentCategory"":"""",""AccountingDocExternalReference"":"""",""CustomerAccountAssignmentGroup"":""02"",""AccountingExchangeRate"":""0.00000"",""CorrespncExternalReference"":"""",""SlsDocSo2PLastContactPersnName"":"""",""SlsDocSo2PLstCntctPersnTelNmbr"":""00862168407207"",""POCorrespncExternalReference"":"""",""CustomerConditionGroup1"":"""",""CustomerConditionGroup2"":"""",""CustomerConditionGroup3"":"""",""CustomerConditionGroup4"":"""",""CustomerConditionGroup5"":"""",""CustomerGroup"":""04"",""AdditionalCustomerGroup1"":"""",""AdditionalCustomerGroup2"":"""",""AdditionalCustomerGroup3"":"""",""AdditionalCustomerGroup4"":"""",""AdditionalCustomerGroup5"":"""",""SlsDocIsRlvtForProofOfDeliv"":false,""CustomerTaxClassification1"":"""",""CustomerTaxClassification2"":"""",""CustomerTaxClassification3"":"""",""CustomerTaxClassification4"":"""",""CustomerTaxClassification5"":"""",""CustomerTaxClassification6"":"""",""CustomerTaxClassification7"":"""",""CustomerTaxClassification8"":"""",""CustomerTaxClassification9"":"""",""TaxDepartureCountry"":"""",""VATRegistrationCountry"":"""",""SalesOrderApprovalReason"":"""",""SalesDocApprovalStatus"":"""",""OverallSDProcessStatus"":""C"",""TotalCreditCheckStatus"":"""",""OverallTotalDeliveryStatus"":""C"",""OverallSDDocumentRejectionSts"":""A"",""BillingDocumentDate"":""\/Date(1130457600000)\/"",""ContractAccount"":"""",""AdditionalValueDays"":""0"",""CustomerPurchaseOrderSuplmnt"":"""",""ServicesRenderedDate"":null,""to_BillingPlan"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000053')/to_BillingPlan""}},""to_Item"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000053')/to_Item""}},""to_Partner"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000053')/to_Partner""}},""to_PaymentPlanItemDetails"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000053')/to_PaymentPlanItemDetails""}},""to_PrecedingProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000053')/to_PrecedingProcFlowDoc""}},""to_PricingElement"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000053')/to_PricingElement""}},""to_RelatedObject"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000053')/to_RelatedObject""}},""to_SubsequentProcFlowDoc"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000053')/to_SubsequentProcFlowDoc""}},""to_Text"":{""__deferred"":{""uri"":""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000053')/to_Text""}}}]}}";


        internal const string SalesOrderExpandItems = @"{
	""d"": {
		""results"": [
			{
				""__metadata"": {
					""id"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')"",
					""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')"",
					""type"": ""API_SALES_ORDER_SRV.A_SalesOrderType"",
					""etag"": ""W/\""datetimeoffset'2005-10-24T15%3A36%3A52.0000000Z'\""""
				},
				""SalesOrder"": ""4000043"",
				""SalesOrderType"": ""ZOS"",
				""SalesOrganization"": ""4010"",
				""DistributionChannel"": ""40"",
				""OrganizationDivision"": ""40"",
				""SalesGroup"": """",
				""SalesOffice"": ""UVE"",
				""SalesDistrict"": """",
				""SoldToParty"": ""4502"",
				""CreationDate"": ""/Date(1130112000000)/"",
				""CreatedByUser"": ""GPICCIN"",
				""LastChangeDate"": ""/Date(1148601600000)/"",
				""SenderBusinessSystemName"": """",
				""ExternalDocumentID"": """",
				""LastChangeDateTime"": ""/Date(1130168212000+0000)/"",
				""ExternalDocLastChangeDateTime"": null,
				""PurchaseOrderByCustomer"": ""16306"",
				""PurchaseOrderByShipToParty"": """",
				""CustomerPurchaseOrderType"": """",
				""CustomerPurchaseOrderDate"": ""/Date(1129248000000)/"",
				""SalesOrderDate"": ""/Date(1130112000000)/"",
				""TotalNetAmount"": ""2656.24"",
				""OverallDeliveryStatus"": ""C"",
				""TotalBlockStatus"": """",
				""OverallOrdReltdBillgStatus"": """",
				""OverallSDDocReferenceStatus"": """",
				""TransactionCurrency"": ""EUR"",
				""SDDocumentReason"": """",
				""PricingDate"": ""/Date(1130112000000)/"",
				""PriceDetnExchangeRate"": ""1.00000"",
				""BillingPlan"": """",
				""RequestedDeliveryDate"": ""/Date(1130457600000)/"",
				""ShippingCondition"": ""01"",
				""CompleteDeliveryIsDefined"": false,
				""ShippingType"": """",
				""HeaderBillingBlockReason"": """",
				""DeliveryBlockReason"": """",
				""DeliveryDateTypeRule"": """",
				""IncotermsClassification"": ""EXW"",
				""IncotermsTransferLocation"": """",
				""IncotermsLocation1"": """",
				""IncotermsLocation2"": """",
				""IncotermsVersion"": """",
				""CustomerPriceGroup"": """",
				""PriceListType"": ""04"",
				""CustomerPaymentTerms"": ""B114"",
				""PaymentMethod"": """",
				""FixedValueDate"": null,
				""AssignmentReference"": """",
				""ReferenceSDDocument"": """",
				""ReferenceSDDocumentCategory"": """",
				""AccountingDocExternalReference"": """",
				""CustomerAccountAssignmentGroup"": ""02"",
				""AccountingExchangeRate"": ""0.00000"",
				""CorrespncExternalReference"": """",
				""SlsDocSo2PLastContactPersnName"": """",
				""SlsDocSo2PLstCntctPersnTelNmbr"": ""0061297504666"",
				""POCorrespncExternalReference"": """",
				""CustomerConditionGroup1"": """",
				""CustomerConditionGroup2"": """",
				""CustomerConditionGroup3"": """",
				""CustomerConditionGroup4"": """",
				""CustomerConditionGroup5"": """",
				""CustomerGroup"": ""04"",
				""AdditionalCustomerGroup1"": """",
				""AdditionalCustomerGroup2"": """",
				""AdditionalCustomerGroup3"": """",
				""AdditionalCustomerGroup4"": """",
				""AdditionalCustomerGroup5"": """",
				""SlsDocIsRlvtForProofOfDeliv"": false,
				""CustomerTaxClassification1"": """",
				""CustomerTaxClassification2"": """",
				""CustomerTaxClassification3"": """",
				""CustomerTaxClassification4"": """",
				""CustomerTaxClassification5"": """",
				""CustomerTaxClassification6"": """",
				""CustomerTaxClassification7"": """",
				""CustomerTaxClassification8"": """",
				""CustomerTaxClassification9"": """",
				""TaxDepartureCountry"": """",
				""VATRegistrationCountry"": """",
				""SalesOrderApprovalReason"": """",
				""SalesDocApprovalStatus"": """",
				""OverallSDProcessStatus"": ""C"",
				""TotalCreditCheckStatus"": """",
				""OverallTotalDeliveryStatus"": ""C"",
				""OverallSDDocumentRejectionSts"": ""A"",
				""BillingDocumentDate"": ""/Date(1130457600000)/"",
				""ContractAccount"": """",
				""AdditionalValueDays"": ""0"",
				""CustomerPurchaseOrderSuplmnt"": """",
				""ServicesRenderedDate"": null,
				""to_BillingPlan"": {
					""__deferred"": {
						""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_BillingPlan""
					}
				},
				""to_Item"": {
					""results"": [
						{
							""__metadata"": {
								""id"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='10')"",
								""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='10')"",
								""type"": ""API_SALES_ORDER_SRV.A_SalesOrderItemType""
							},
							""SalesOrder"": ""4000043"",
							""SalesOrderItem"": ""10"",
							""HigherLevelItem"": ""0"",
							""HigherLevelItemUsage"": """",
							""SalesOrderItemCategory"": ""ZTAM"",
							""SalesOrderItemText"": ""MINIDAT OEDA 80 HH R22"",
							""PurchaseOrderByCustomer"": ""16306"",
							""PurchaseOrderByShipToParty"": """",
							""UnderlyingPurchaseOrderItem"": """",
							""ExternalItemID"": """",
							""Material"": ""SETVMIN0"",
							""OriginallyRequestedMaterial"": ""SETVMIN0"",
							""MaterialByCustomer"": """",
							""PricingDate"": ""/Date(1130112000000)/"",
							""PricingReferenceMaterial"": """",
							""BillingPlan"": """",
							""RequestedQuantity"": ""1.000"",
							""RequestedQuantityUnit"": ""NR"",
							""RequestedQuantitySAPUnit"": ""NR"",
							""RequestedQuantityISOUnit"": ""PCE"",
							""OrderQuantityUnit"": ""NR"",
							""OrderQuantitySAPUnit"": ""NR"",
							""OrderQuantityISOUnit"": ""PCE"",
							""ConfdDelivQtyInOrderQtyUnit"": ""1.000"",
							""ItemGrossWeight"": ""0.000"",
							""ItemNetWeight"": ""0.000"",
							""ItemWeightUnit"": ""KG"",
							""ItemWeightSAPUnit"": ""KG"",
							""ItemWeightISOUnit"": ""KGM"",
							""ItemVolume"": ""0.000"",
							""ItemVolumeUnit"": """",
							""ItemVolumeSAPUnit"": """",
							""ItemVolumeISOUnit"": """",
							""TransactionCurrency"": ""EUR"",
							""NetAmount"": ""2656.24"",
							""TotalSDDocReferenceStatus"": """",
							""SDDocReferenceStatus"": """",
							""MaterialSubstitutionReason"": """",
							""MaterialGroup"": ""PRODFIN"",
							""MaterialPricingGroup"": """",
							""AdditionalMaterialGroup1"": """",
							""AdditionalMaterialGroup2"": """",
							""AdditionalMaterialGroup3"": """",
							""AdditionalMaterialGroup4"": """",
							""AdditionalMaterialGroup5"": """",
							""BillingDocumentDate"": ""/Date(1130457600000)/"",
							""ContractAccount"": """",
							""AdditionalValueDays"": ""0"",
							""ServicesRenderedDate"": null,
							""Batch"": """",
							""ProductionPlant"": ""4012"",
							""OriginalPlant"": """",
							""AltvBsdConfSubstitutionStatus"": """",
							""StorageLocation"": """",
							""DeliveryGroup"": ""1"",
							""ShippingPoint"": ""4010"",
							""ShippingType"": """",
							""DeliveryPriority"": ""0"",
							""DeliveryDateQuantityIsFixed"": false,
							""DeliveryDateTypeRule"": """",
							""IncotermsClassification"": ""EXW"",
							""IncotermsTransferLocation"": """",
							""IncotermsLocation1"": """",
							""IncotermsLocation2"": """",
							""TaxAmount"": ""0.00"",
							""ProductTaxClassification1"": ""1"",
							""ProductTaxClassification2"": ""0"",
							""ProductTaxClassification3"": """",
							""ProductTaxClassification4"": """",
							""ProductTaxClassification5"": """",
							""ProductTaxClassification6"": """",
							""ProductTaxClassification7"": """",
							""ProductTaxClassification8"": """",
							""ProductTaxClassification9"": """",
							""MatlAccountAssignmentGroup"": ""01"",
							""CostAmount"": ""0.00"",
							""CustomerPaymentTerms"": ""B114"",
							""FixedValueDate"": null,
							""CustomerGroup"": ""04"",
							""SalesDocumentRjcnReason"": """",
							""ItemBillingBlockReason"": """",
							""SlsDocIsRlvtForProofOfDeliv"": false,
							""WBSElement"": """",
							""ProfitCenter"": """",
							""AccountingExchangeRate"": ""0.00000"",
							""ReferenceSDDocument"": """",
							""ReferenceSDDocumentItem"": ""0"",
							""CorrespncExternalReference"": """",
							""POCorrespncExternalReference"": """",
							""CustomerConditionGroup1"": """",
							""CustomerConditionGroup2"": """",
							""CustomerConditionGroup3"": """",
							""CustomerConditionGroup4"": """",
							""CustomerConditionGroup5"": """",
							""SDProcessStatus"": ""C"",
							""DeliveryStatus"": ""C"",
							""OrderRelatedBillingStatus"": """",
							""Subtotal1Amount"": ""2710.45"",
							""Subtotal2Amount"": ""6949.86"",
							""Subtotal3Amount"": ""2656.24"",
							""Subtotal4Amount"": ""0.00"",
							""Subtotal5Amount"": ""0.00"",
							""Subtotal6Amount"": ""0.00"",
							""RequirementSegment"": """",
							""to_BillingPlan"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='10')/to_BillingPlan""
								}
							},
							""to_Partner"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='10')/to_Partner""
								}
							},
							""to_PrecedingProcFlowDocItem"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='10')/to_PrecedingProcFlowDocItem""
								}
							},
							""to_PricingElement"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='10')/to_PricingElement""
								}
							},
							""to_RelatedObject"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='10')/to_RelatedObject""
								}
							},
							""to_SalesOrder"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='10')/to_SalesOrder""
								}
							},
							""to_ScheduleLine"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='10')/to_ScheduleLine""
								}
							},
							""to_SubsequentProcFlowDocItem"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='10')/to_SubsequentProcFlowDocItem""
								}
							},
							""to_Text"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='10')/to_Text""
								}
							}
						},
						{
							""__metadata"": {
								""id"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='20')"",
								""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='20')"",
								""type"": ""API_SALES_ORDER_SRV.A_SalesOrderItemType""
							},
							""SalesOrder"": ""4000043"",
							""SalesOrderItem"": ""20"",
							""HigherLevelItem"": ""10"",
							""HigherLevelItemUsage"": """",
							""SalesOrderItemCategory"": ""ZTAC"",
							""SalesOrderItemText"": ""Configurabile Minidat ED"",
							""PurchaseOrderByCustomer"": ""16306"",
							""PurchaseOrderByShipToParty"": """",
							""UnderlyingPurchaseOrderItem"": """",
							""ExternalItemID"": """",
							""Material"": ""CONFMIN0"",
							""OriginallyRequestedMaterial"": ""CONFMIN0"",
							""MaterialByCustomer"": """",
							""PricingDate"": ""/Date(1130112000000)/"",
							""PricingReferenceMaterial"": """",
							""BillingPlan"": """",
							""RequestedQuantity"": ""1.000"",
							""RequestedQuantityUnit"": ""NR"",
							""RequestedQuantitySAPUnit"": ""NR"",
							""RequestedQuantityISOUnit"": ""PCE"",
							""OrderQuantityUnit"": ""NR"",
							""OrderQuantitySAPUnit"": ""NR"",
							""OrderQuantityISOUnit"": ""PCE"",
							""ConfdDelivQtyInOrderQtyUnit"": ""1.000"",
							""ItemGrossWeight"": ""0.000"",
							""ItemNetWeight"": ""0.000"",
							""ItemWeightUnit"": ""KG"",
							""ItemWeightSAPUnit"": ""KG"",
							""ItemWeightISOUnit"": ""KGM"",
							""ItemVolume"": ""0.000"",
							""ItemVolumeUnit"": """",
							""ItemVolumeSAPUnit"": """",
							""ItemVolumeISOUnit"": """",
							""TransactionCurrency"": ""EUR"",
							""NetAmount"": ""0.00"",
							""TotalSDDocReferenceStatus"": """",
							""SDDocReferenceStatus"": """",
							""MaterialSubstitutionReason"": """",
							""MaterialGroup"": ""PRODFIN"",
							""MaterialPricingGroup"": """",
							""AdditionalMaterialGroup1"": """",
							""AdditionalMaterialGroup2"": """",
							""AdditionalMaterialGroup3"": """",
							""AdditionalMaterialGroup4"": ""002"",
							""AdditionalMaterialGroup5"": """",
							""BillingDocumentDate"": ""/Date(1130457600000)/"",
							""ContractAccount"": """",
							""AdditionalValueDays"": ""0"",
							""ServicesRenderedDate"": null,
							""Batch"": """",
							""ProductionPlant"": ""4012"",
							""OriginalPlant"": """",
							""AltvBsdConfSubstitutionStatus"": """",
							""StorageLocation"": """",
							""DeliveryGroup"": ""1"",
							""ShippingPoint"": ""4010"",
							""ShippingType"": """",
							""DeliveryPriority"": ""0"",
							""DeliveryDateQuantityIsFixed"": false,
							""DeliveryDateTypeRule"": """",
							""IncotermsClassification"": ""EXW"",
							""IncotermsTransferLocation"": """",
							""IncotermsLocation1"": """",
							""IncotermsLocation2"": """",
							""TaxAmount"": ""0.00"",
							""ProductTaxClassification1"": ""1"",
							""ProductTaxClassification2"": ""0"",
							""ProductTaxClassification3"": """",
							""ProductTaxClassification4"": """",
							""ProductTaxClassification5"": """",
							""ProductTaxClassification6"": """",
							""ProductTaxClassification7"": """",
							""ProductTaxClassification8"": """",
							""ProductTaxClassification9"": """",
							""MatlAccountAssignmentGroup"": ""01"",
							""CostAmount"": ""1549.26"",
							""CustomerPaymentTerms"": ""B114"",
							""FixedValueDate"": null,
							""CustomerGroup"": ""04"",
							""SalesDocumentRjcnReason"": """",
							""ItemBillingBlockReason"": """",
							""SlsDocIsRlvtForProofOfDeliv"": false,
							""WBSElement"": """",
							""ProfitCenter"": """",
							""AccountingExchangeRate"": ""0.00000"",
							""ReferenceSDDocument"": """",
							""ReferenceSDDocumentItem"": ""0"",
							""CorrespncExternalReference"": """",
							""POCorrespncExternalReference"": """",
							""CustomerConditionGroup1"": """",
							""CustomerConditionGroup2"": """",
							""CustomerConditionGroup3"": """",
							""CustomerConditionGroup4"": """",
							""CustomerConditionGroup5"": """",
							""SDProcessStatus"": ""C"",
							""DeliveryStatus"": ""C"",
							""OrderRelatedBillingStatus"": """",
							""Subtotal1Amount"": ""0.00"",
							""Subtotal2Amount"": ""0.00"",
							""Subtotal3Amount"": ""0.00"",
							""Subtotal4Amount"": ""0.00"",
							""Subtotal5Amount"": ""0.00"",
							""Subtotal6Amount"": ""0.00"",
							""RequirementSegment"": """",
							""to_BillingPlan"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='20')/to_BillingPlan""
								}
							},
							""to_Partner"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='20')/to_Partner""
								}
							},
							""to_PrecedingProcFlowDocItem"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='20')/to_PrecedingProcFlowDocItem""
								}
							},
							""to_PricingElement"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='20')/to_PricingElement""
								}
							},
							""to_RelatedObject"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='20')/to_RelatedObject""
								}
							},
							""to_SalesOrder"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='20')/to_SalesOrder""
								}
							},
							""to_ScheduleLine"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='20')/to_ScheduleLine""
								}
							},
							""to_SubsequentProcFlowDocItem"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='20')/to_SubsequentProcFlowDocItem""
								}
							},
							""to_Text"": {
								""__deferred"": {
									""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='20')/to_Text""
								}
							}
						}
					]
				},
				""to_Partner"": {
					""__deferred"": {
						""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_Partner""
					}
				},
				""to_PaymentPlanItemDetails"": {
					""__deferred"": {
						""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_PaymentPlanItemDetails""
					}
				},
				""to_PrecedingProcFlowDoc"": {
					""__deferred"": {
						""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_PrecedingProcFlowDoc""
					}
				},
				""to_PricingElement"": {
					""__deferred"": {
						""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_PricingElement""
					}
				},
				""to_RelatedObject"": {
					""__deferred"": {
						""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_RelatedObject""
					}
				},
				""to_SubsequentProcFlowDoc"": {
					""__deferred"": {
						""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_SubsequentProcFlowDoc""
					}
				},
				""to_Text"": {
					""__deferred"": {
						""uri"": ""https://itwebdsp01test.swegon.com/sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('4000043')/to_Text""
					}
				}
			}
		]
	}
}";

    }
}
