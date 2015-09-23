IF OBJECT_ID(N'[dbo].[ProductOutOfStockRequests]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProductOutOfStockRequests]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[IdProduct] INT NOT NULL, 
		[Name] NVARCHAR(250) NOT NULL, 
		[Email] NVARCHAR(250) NOT NULL, 
		[DateCreated] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		CONSTRAINT [FK_ProductOutOfStockRequests_ToProducts] FOREIGN KEY ([IdProduct]) REFERENCES [Products]([Id]), 
	);
END

GO