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
:r .\Scripts\AppSettings.sql
:r .\Scripts\UpdateIdentity.sql
:r .\Scripts\CURs.sql
:r .\Scripts\FilesPermissions.sql
:r .\Scripts\MarketingPermissions.sql
:r .\Scripts\EcommerceMigration.sql

:r .\Scripts\RecipesToProducts.sql
:r .\Scripts\ArticlesToProducts.sql

:r .\Scripts\RecipeUpdate.sql
:r .\Scripts\ContentAreas.sql
:r .\Scripts\BugTickets.sql
:r .\Scripts\CustomPublicStyles.sql

:r .\Scripts\ProductsOutOfStockData.sql
:r .\Scripts\MastersData.sql

:r .\Scripts\FedEx.sql

:r .\Scripts\UpdateIdentityWithStorefront.sql

:r .\Scripts\Products.sql
:r .\Scripts\ContentPagesData.sql
:r .\Scripts\ContentIndexes.sql
:r .\Scripts\Redirects.sql

:r .\Scripts\EmailTemplates.sql