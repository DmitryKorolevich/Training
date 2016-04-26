IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ContentCategories') AND name = N'IdOld')
BEGIN
	ALTER TABLE dbo.ContentCategories
	ADD IdOld INT NULL

	CREATE NONCLUSTERED INDEX IX_ContentCategories_IdOld ON ContentCategories ([IdOld])

END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Articles') AND name = N'IdOld')
BEGIN
	ALTER TABLE dbo.Articles
	ADD IdOld INT NULL

	CREATE NONCLUSTERED INDEX IX_Articles_IdOld ON Articles ([IdOld])

END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ContentPages') AND name = N'IdOld')
BEGIN
	ALTER TABLE dbo.ContentPages
	ADD IdOld INT NULL

	CREATE NONCLUSTERED INDEX IX_ContentPages_IdOld ON ContentPages ([IdOld])

END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Recipes') AND name = N'IdOld')
BEGIN
	ALTER TABLE dbo.Recipes
	ADD IdOld INT NULL

	CREATE NONCLUSTERED INDEX IX_Recipes_IdOld ON Recipes ([IdOld])

END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.FAQs') AND name = N'IdOld')
BEGIN
	ALTER TABLE dbo.FAQs
	ADD IdOld INT NULL

	CREATE NONCLUSTERED INDEX IX_FAQs_IdOld ON FAQs ([IdOld])

END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ProductCategories') AND name = N'IdOld')
BEGIN
	ALTER TABLE dbo.ProductCategories
	ADD IdOld INT NULL

	CREATE NONCLUSTERED INDEX IX_ProductCategories_IdOld ON ProductCategories ([IdOld])

END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Products') AND name = N'IdOld')
BEGIN
	ALTER TABLE dbo.Products
	ADD IdOld INT NULL

	CREATE NONCLUSTERED INDEX IX_Products_IdOld ON Products ([IdOld])

END

GO