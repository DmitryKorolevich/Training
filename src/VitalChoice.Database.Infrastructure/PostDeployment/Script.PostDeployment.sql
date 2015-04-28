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


:r .\Scripts\StartInit.sql
:r .\Scripts\LocalizationItems.sql
:r .\Scripts\ContentItems.sql
:r .\Scripts\ContentProcessors.sql
:r .\Scripts\RecordStatusCodes.sql
:r .\Scripts\ContentTestDataInsert.sql
:r .\Scripts\FAQs.sql
:r .\Scripts\Articles.sql
:r .\Scripts\ContentPages.sql
:r .\Scripts\SetupIdentity.sql
:r .\Scripts\ProductCategories.sql
:r .\Scripts\Countries.sql
:r .\Scripts\AppSettings.sql
