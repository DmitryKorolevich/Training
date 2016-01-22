IF OBJECT_ID(N'dbo.Carts', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Carts]
	(
		[Id] INT NOT NULL IDENTITY
		CONSTRAINT [PK_Carts] PRIMARY KEY (Id),
		[CartUid] UNIQUEIDENTIFIER NOT NULL,
		[IdCustomer] INT NULL
		CONSTRAINT [FK_CartToCustomer] FOREIGN KEY (IdCustomer) REFERENCES dbo.Customers (Id),
		[IdOrder] INT NULL
		CONSTRAINT [FK_CartToOrder] FOREIGN KEY (IdOrder) REFERENCES dbo.Orders (Id),
		[IdDiscount] INT NULL
		CONSTRAINT [FK_CartToDiscount] FOREIGN KEY (IdDiscount) REFERENCES dbo.Discounts (Id)
	)
END


IF OBJECT_ID(N'dbo.CartToSkus', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CartToSkus]
	(
		[IdCart] INT NOT NULL
		CONSTRAINT [FK_CartSkusToCart] FOREIGN KEY (IdCart) REFERENCES dbo.Carts (Id),
		[IdSku] INT NOT NULL
		CONSTRAINT [FK_CartSkusToSku] FOREIGN KEY (IdSku) REFERENCES dbo.Skus (Id)
		CONSTRAINT [PK_CartToSkus] PRIMARY KEY (IdCart, IdSku),
		[Amount] MONEY NOT NULL,
		[Quantity] INT NOT NULL
	)
END

IF OBJECT_ID(N'dbo.CartToGiftCertificates', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CartToGiftCertificates]
	(
		[IdCart] INT NOT NULL
		CONSTRAINT [FK_CartGiftCertificatesToCart] FOREIGN KEY (IdCart) REFERENCES dbo.Carts (Id),
		[IdGiftCertificate] INT NOT NULL
		CONSTRAINT [FK_CartGiftCertificatesToGiftCertificate] FOREIGN KEY (IdGiftCertificate) REFERENCES dbo.GiftCertificates (Id)
		CONSTRAINT [PK_CartToGiftCertificates] PRIMARY KEY (IdCart, IdGiftCertificate),
		[Amount] MONEY NOT NULL
	)
END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Carts') AND name = N'IX_CARTUID_UQ')
BEGIN
	CREATE UNIQUE INDEX [IX_CARTUID_UQ] ON [dbo].[Carts] (CartUid)
	CREATE UNIQUE INDEX [IX_CUSTOMERID_UQ] ON [dbo].[Carts] (IdCustomer)
END