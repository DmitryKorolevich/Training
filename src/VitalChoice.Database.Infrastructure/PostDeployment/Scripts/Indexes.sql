IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'Recipes') AND name = N'IX_Articles_Url')
BEGIN
	DROP INDEX [IX_Articles_Url] ON [dbo].Recipes

	CREATE NONCLUSTERED INDEX [IX_Articles_Url] ON [dbo].[Articles]
	(
		[Url] ASC
	)
END

GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'Recipes') AND name = N'IX_Articles_MasterContentItemId')
BEGIN
	DROP INDEX [IX_Articles_MasterContentItemId] ON [dbo].Recipes

	CREATE NONCLUSTERED INDEX [IX_Articles_MasterContentItemId] ON [dbo].[Articles]
	(
		[MasterContentItemId] ASC
	)
END

GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'Recipes') AND name = N'IX_ContentPages_MasterContentItemId')
BEGIN
	DROP INDEX [IX_ContentPages_MasterContentItemId] ON [dbo].Recipes

	CREATE NONCLUSTERED INDEX [IX_ContentPages_MasterContentItemId] ON [dbo].[ContentPages]
	(
		[MasterContentItemId] ASC
	)
END


IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'AspNetUsers') AND name = N'IX_TokenDateType')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_TokenDateType] ON [dbo].[AspNetUsers] ([ConfirmationToken], [DeletedDate], [IdUserType])
END