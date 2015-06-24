IF COL_LENGTH('[dbo].[Products]','IdAddedBy') IS NOT NULL
BEGIN
	ALTER TABLE [dbo].[Products]
	DROP CONSTRAINT [FK_Products_ToAddedUser]

	ALTER TABLE [dbo].[Products]
	DROP COLUMN [IdAddedBy]
END


IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'IdEditedBy' AND [Object_ID] = OBJECT_ID(N'[dbo].Discounts', N'U'))
	EXEC SP_RENAME 'Discounts.IdExternal' , 'IdEditedBy', 'COLUMN'

GO

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_Discounts_ToUser')
	ALTER TABLE [dbo].[Discounts] ADD CONSTRAINT [FK_Discounts_ToUser] FOREIGN KEY ([IdEditedBy]) REFERENCES [Users] ([Id])

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'IdEditedBy' AND [Object_ID] = OBJECT_ID(N'[dbo].Products', N'U'))
	EXEC SP_RENAME 'Products.IdExternal' , 'IdEditedBy', 'COLUMN'

GO

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_Products_ToUser')
	ALTER TABLE [dbo].[Products] ADD CONSTRAINT [FK_Products_ToUser] FOREIGN KEY ([IdEditedBy]) REFERENCES [Users] ([Id])

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'IdAddedBy' AND [Object_ID] = OBJECT_ID(N'[dbo].Discounts', N'U'))
BEGIN
	ALTER TABLE dbo.Discounts
	ADD IdAddedBy INT NULL

	ALTER TABLE [dbo].[Discounts] ADD CONSTRAINT [FK_Discounts_ToAddedUser] FOREIGN KEY ([IdAddedBy]) REFERENCES [Users] ([Id])
END
GO