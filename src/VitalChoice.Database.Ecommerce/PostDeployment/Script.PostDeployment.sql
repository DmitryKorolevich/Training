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

:r .\Scripts\VProductSkus.sql
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
:r .\Scripts\VTopProducts.sql
:r .\Scripts\VAutoShips.sql
:r .\Scripts\TFGetTableIdsByString.sql
:r .\Scripts\SPGetInventorySkusUsageReport.sql
:r .\Scripts\SPGetInventoriesSummaryUsageReport.sql
:r .\Scripts\SPsForWholesaleDropShipReport.sql
:r .\Scripts\SPGetTransactionAndRefundReport.sql
:r .\Scripts\VOrderCountOnCustomers.sql
:r .\Scripts\VFirstOrderOnCustomers.sql
:r .\Scripts\SPsForOrdersSummarySalesReport.sql
:r .\Scripts\VWholesaleSummaryInfo.sql
:r .\Scripts\SPGetCustomersStandardOrderTotals.sql
:r .\Scripts\SPGetCustomersStandardOrdersLast.sql
:r .\Scripts\SPGetSkuBreakDownReport.sql
:r .\Scripts\SPGetSkuPOrderTypeBreakDownReport.sql
:r .\Scripts\SPGetSkuPOrderTypeFutureBreakDownReport.sql
:r .\Scripts\SPGetMailingListReport.sql
:r .\Scripts\SPsForShippedViaReport.sql
:r .\Scripts\VAffiliates.sql
:r .\Scripts\VProductsWithReviews.sql
:r .\Scripts\SPsForProductQualityReport.sql
:r .\Scripts\SPGetKPISales.sql
:r .\Scripts\SPGetShortOrders.sql
:r .\Scripts\SPGetAAFESReport.sql
:r .\Scripts\VCustomersWithDublicateEmails.sql
:r .\Scripts\SPGetCustomerSkuUsageReport.sql
:r .\Scripts\SPGetOrderDiscountReport.sql
:r .\Scripts\OrderIndexes.sql
:r .\Scripts\SettingOptionTypesData.sql
:r .\Scripts\Indexes.sql
:r .\Scripts\OrderOptionTypes.sql
:r .\Scripts\ProductOptionTypes.sql