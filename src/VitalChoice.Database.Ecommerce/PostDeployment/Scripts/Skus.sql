﻿IF OBJECT_ID(N'[dbo].[Skus]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Skus]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdProduct] INT NOT NULL, 
		[DateCreated] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[DateEdited] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[StatusCode] INT NOT NULL DEFAULT 1, 
		[Price] MONEY NOT NULL DEFAULT 0, 
		[WholesalePrice] MONEY NOT NULL DEFAULT 0, 
		[Code] NVARCHAR(20) NOT NULL, 
		CONSTRAINT [FK_Skus_ToProduct] FOREIGN KEY ([IdProduct]) REFERENCES [Products]([Id]),
		CONSTRAINT [FK_Skus_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes] ([StatusCode])
	);

	CREATE INDEX [IX_Skus_Code] ON [dbo].[Skus] ([Code], [StatusCode]) INCLUDE (Id, IdProduct, DateCreated, DateEdited, Price, WholesalePrice)
END

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'Hidden' AND [Object_ID] = OBJECT_ID(N'[dbo].Skus', N'U'))
	ALTER TABLE dbo.Skus
	ADD Hidden BIT NOT NULL DEFAULT 0

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'Order' AND [Object_ID] = OBJECT_ID(N'[dbo].Skus', N'U'))
	ALTER TABLE Skus ADD [Order] INT NOT NULL DEFAULT(0)