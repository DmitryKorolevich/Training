/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

:r .\Scripts\BigStringValues.sql

:r .\Scripts\RecordStatusCodes.sql

:r .\Scripts\Workflow.sql

:r .\Scripts\ProductCategories.sql
:r .\Scripts\InventoryCategories.sql
:r .\Scripts\Countries.sql
:r .\Scripts\GiftCertificates.sql
:r .\Scripts\GiftCetfificatesUpdate.sql

:r .\Scripts\FieldTypes.sql
:r .\Scripts\ProductTypes.sql
:r .\Scripts\Lookups.sql
:r .\Scripts\LookupVariants.sql
:r .\Scripts\Products.sql
:r .\Scripts\ProductsToCategories.sql
:r .\Scripts\VSkus.sql
:r .\Scripts\Discounts.sql
:r .\Scripts\ProductReviews.sql
:r .\Scripts\VProductsWithReviews.sql

:r .\Scripts\CustomerAreaBasics.sql

:r .\Scripts\ReferencesToUsers.sql

:r .\Scripts\Orders.sql

:r .\Scripts\Affiliates.sql
:r .\Scripts\VAffiliates.sql
:r .\Scripts\HelpTickets.sql
:r .\Scripts\Promotions.sql
:r .\Scripts\ProductsOutOfStock.sql

:r .\Scripts\ObjectHistoryLogItems.sql

:r .\Scripts\CustomerFavoritesView.sql

:r .\Scripts\DiscountUpdates.sql

:r .\Scripts\CustomerAddressStructureUpdate.sql

:r .\Scripts\AllValueTablesStructureOptimize.sql