/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

:r .\Scripts\ProductOptionTypes.sql
:r .\Scripts\DiscountOptionTypes.sql
:r .\Scripts\CustomerAreaBasicsData.sql
:r .\Scripts\OrderLookups.sql
:r .\Scripts\VProductSkus.sql
:r .\Scripts\OrdersData.sql
:r .\Scripts\AffiliatesData.sql
:r .\Scripts\PromotionData.sql
:r .\Scripts\CustomerAddressStructureModeData.sql
:r .\Scripts\VCustomers.sql
:r .\Scripts\VOrders.sql
:r .\Scripts\VHelpTickets.sql
:r .\Scripts\SPGetEngangedAffiliatesCount.sql
:r .\Scripts\SPGetAffiliatesSummaryReport.sql
:r .\Scripts\SPGetProductCategoryStatistic.sql
:r .\Scripts\SPGetSkusInProductCategoryStatistic.sql
:r .\Scripts\VSkus.sql
:r .\Scripts\VAffiliateNotPaidCommissions.sql
:r .\Scripts\VHealthwisePeriods.sql
:r .\Scripts\VOrderWithRegionInfoItems.sql
:r .\Scripts\SPGetOrdersRegionStatistic.sql
:r .\Scripts\SPGetOrdersZipStatistic.sql
:r .\Scripts\VCustomerFavorites.sql
:r .\Scripts\InventorySkuData.sql
:r .\Scripts\VTopProducts.sql
:r .\Scripts\AutoShip.sql
:r .\Scripts\VAutoShips.sql
:r .\Scripts\TFGetTableIdsByString.sql
:r .\Scripts\SPGetInventorySkusUsageReport.sql
:r .\Scripts\SPGetInventoriesSummaryUsageReport.sql
:r .\Scripts\SPsForWholesaleDropShipReport.sql
:r .\Scripts\NewslettersData.sql
:r .\Scripts\SPGetTransactionAndRefundReport.sql
:r .\Scripts\VOrderCountOnCustomers.sql
:r .\Scripts\VFirstOrderOnCustomers.sql
:r .\Scripts\SPsForOrdersSummarySalesReport.sql
:r .\Scripts\VWholesaleSummaryInfo.sql
:r .\Scripts\SPGetCustomersStandardOrderTotals.sql
:r .\Scripts\SPGetCustomersStandardOrdersLast.sql
:r .\Scripts\SPGetSkuBreakDownReport.sql