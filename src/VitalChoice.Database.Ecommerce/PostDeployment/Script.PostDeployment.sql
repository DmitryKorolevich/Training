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

:r .\Scripts\WorkflowExecutors.sql
:r .\Scripts\WorkflowTrees.sql
:r .\Scripts\WorkflowTreeActions.sql
:r .\Scripts\WorkflowResolverPaths.sql

:r .\Scripts\RecordStatusCodes.sql
:r .\Scripts\ProductCategories.sql
:r .\Scripts\Countries.sql
:r .\Scripts\GiftCertificates.sql