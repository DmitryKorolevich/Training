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
:r .\Scripts\VHelpTickets.sql
:r .\Scripts\PromotionData.sql
:r .\Scripts\VCustomers.sql
:r .\Scripts\CustomerAddressStructureModeData.sql