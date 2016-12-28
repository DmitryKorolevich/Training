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

:r .\Scripts\KPICacheItems.sql
:r .\Scripts\OrderShippingPackages.sql
:r .\Scripts\GCs.sql
:r .\Scripts\SkuOOSHistoryItems.sql
:r .\Scripts\OrderReviewRules.sql