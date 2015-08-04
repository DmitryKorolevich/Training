IF OBJECT_ID(N'[dbo].[ProductReviews]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProductReviews]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[StatusCode] INT NOT NULL DEFAULT 1,
		[DateCreated] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[DateEdited] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[IdProduct] INT NOT NULL, 
		[CustomerName] NVARCHAR(250) NOT NULL, 
		[Email] NVARCHAR(250) NOT NULL, 
		[Title] NVARCHAR(250) NOT NULL, 
		[Description] NVARCHAR(2000) NOT NULL, 
		[Rating] INT NOT NULL, 
		CONSTRAINT [FK_ProductReviews_ToProducts] FOREIGN KEY ([IdProduct]) REFERENCES [Products]([Id]), 
		CONSTRAINT [FK_ProductReviews_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes] ([StatusCode])
	);

	CREATE INDEX [IX_ProductReviews_IdProductStatusCode] ON [dbo].[ProductReviews] (IdProduct, StatusCode)
	CREATE INDEX [IX_ProductReviews_IdProduct] ON [dbo].[ProductReviews] (IdProduct)
END