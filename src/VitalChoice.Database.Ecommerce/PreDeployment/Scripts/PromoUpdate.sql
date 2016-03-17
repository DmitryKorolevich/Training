
IF OBJECT_ID(N'dbo.OrderToPromos') IS NULL
BEGIN
	CREATE TABLE OrderToPromos (
		[IdOrder] INT NOT NULL
			CONSTRAINT FK_OrderToPromoToOrder FOREIGN KEY (IdOrder) REFERENCES dbo.Orders (Id),
		[IdSku] INT NOT NULL
			CONSTRAINT PK_PromoOrdered PRIMARY KEY (IdOrder DESC, IdSku)
			CONSTRAINT FK_OrderToPromoToSku FOREIGN KEY (IdSku) REFERENCES dbo.Skus (Id),
		[IdPromo] INT NULL
			CONSTRAINT FK_OrderToPromoToPromo FOREIGN KEY (IdPromo) REFERENCES dbo.Promotions(Id),
		[Amount] MONEY NOT NULL,
		[Quantity] INT NOT NULL
	)
END

IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'OrderToPromos') AND Name = N'Disabled')
BEGIN
	ALTER TABLE OrderToPromos
	ADD [Disabled] BIT NULL
END
