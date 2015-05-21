IF OBJECT_ID(N'[dbo].[ProductOptionTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProductOptionTypes]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[Name] NVARCHAR(50) NOT NULL, 
		[IdFieldType] INT NOT NULL, 
		[IdProductType] INT NULL, 
		[DefaultValue] NVARCHAR(250) NULL, 
		CONSTRAINT [FK_ProductOptionTypes_ToFieldType] FOREIGN KEY ([IdFieldType]) REFERENCES [FieldTypes]([Id]), 
		CONSTRAINT [FK_ProductOptionTypes_ToProductType] FOREIGN KEY ([IdProductType]) REFERENCES [ProductTypes]([Id])
	);

	CREATE INDEX [IX_ProductOptionTypes_Name] ON [dbo].[ProductOptionTypes] ([Name]) INCLUDE (Id, IdFieldType, IdProductType)
END