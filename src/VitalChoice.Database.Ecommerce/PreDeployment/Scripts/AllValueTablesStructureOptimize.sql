DECLARE @drop_sql VARCHAR(4000)

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Id' AND object_id = OBJECT_ID(N'AddressOptionValues'))
BEGIN
	ALTER TABLE AddressOptionValues
	DROP CONSTRAINT PK_AddressOptionValues

	ALTER TABLE AddressOptionValues
	ADD CONSTRAINT PK_AddressOptionValues PRIMARY KEY (IdAddress, IdOptionType)

	DROP INDEX IX_AddressOptionValues_Value ON AddressOptionValues

	ALTER TABLE AddressOptionValues
	DROP COLUMN Id

	CREATE NONCLUSTERED INDEX IX_AddressOptionValues_Value 
	ON AddressOptionValues (Value)

	CREATE NONCLUSTERED INDEX IX_AddressOptionValues_ValuesSearch 
	ON AddressOptionValues (Value, IdOptionType) 
	INCLUDE (IdAddress)
END

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Id' AND object_id = OBJECT_ID(N'AffiliateOptionValues'))
BEGIN
	ALTER TABLE AffiliateOptionValues
	DROP CONSTRAINT PK_AffiliateOptionValues

	ALTER TABLE AffiliateOptionValues
	ADD CONSTRAINT PK_AffiliateOptionValues PRIMARY KEY (IdAffiliate, IdOptionType)

	DROP INDEX IX_AffiliateOptionValues_Value ON AffiliateOptionValues

	ALTER TABLE AffiliateOptionValues
	DROP COLUMN Id

	CREATE NONCLUSTERED INDEX IX_AffiliateOptionValues_Value 
	ON AffiliateOptionValues (Value)

	CREATE NONCLUSTERED INDEX IX_AffiliateOptionValues_ValuesSearch 
	ON AffiliateOptionValues (Value, IdOptionType) 
	INCLUDE (IdAffiliate)
END

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Id' AND object_id = OBJECT_ID(N'CustomerNoteOptionValues'))
BEGIN
	ALTER TABLE CustomerNoteOptionValues
	DROP CONSTRAINT PK_CustomerNoteOptionValues

	ALTER TABLE CustomerNoteOptionValues
	ADD CONSTRAINT PK_CustomerNoteOptionValues PRIMARY KEY (IdCustomerNote, IdOptionType)

	DROP INDEX IX_CustomerNoteOptionValues_Value ON CustomerNoteOptionValues

	ALTER TABLE CustomerNoteOptionValues
	DROP COLUMN Id

	CREATE NONCLUSTERED INDEX IX_CustomerNoteOptionValues_Value 
	ON CustomerNoteOptionValues (Value)

	CREATE NONCLUSTERED INDEX IX_CustomerNoteOptionValues_ValuesSearch 
	ON CustomerNoteOptionValues (Value, IdOptionType) 
	INCLUDE (IdCustomerNote)
END

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Id' AND object_id = OBJECT_ID(N'CustomerOptionValues'))
BEGIN
	ALTER TABLE CustomerOptionValues
	DROP CONSTRAINT PK_CustomerOptionValues

	ALTER TABLE CustomerOptionValues
	ADD CONSTRAINT PK_CustomerOptionValues PRIMARY KEY (IdCustomer, IdOptionType)

	DROP INDEX IX_CustomerOptionValues_Value ON CustomerOptionValues

	ALTER TABLE CustomerOptionValues
	DROP COLUMN Id

	CREATE NONCLUSTERED INDEX IX_CustomerOptionValues_Value 
	ON CustomerOptionValues (Value)

	CREATE NONCLUSTERED INDEX IX_CustomerOptionValues_ValuesSearch 
	ON CustomerOptionValues (Value, IdOptionType) 
	INCLUDE (IdCustomer)
END

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Id' AND object_id = OBJECT_ID(N'CustomerPaymentMethodValues'))
BEGIN
	ALTER TABLE CustomerPaymentMethodValues
	DROP CONSTRAINT PK_CustomerPaymentMethodValues

	ALTER TABLE CustomerPaymentMethodValues
	ADD CONSTRAINT PK_CustomerPaymentMethodValues PRIMARY KEY (IdCustomerPaymentMethod, IdOptionType)

	DROP INDEX IX_CustomerPaymentMethodValues_Value ON CustomerPaymentMethodValues

	ALTER TABLE CustomerPaymentMethodValues
	DROP COLUMN Id

	CREATE NONCLUSTERED INDEX IX_CustomerPaymentMethodValues_Value 
	ON CustomerPaymentMethodValues (Value)

	CREATE NONCLUSTERED INDEX IX_CustomerPaymentMethodValues_ValuesSearch 
	ON CustomerPaymentMethodValues (Value, IdOptionType) 
	INCLUDE (IdCustomerPaymentMethod)
END

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Id' AND object_id = OBJECT_ID(N'DiscountOptionValues'))
BEGIN
	SET @drop_sql = 'ALTER TABLE DiscountOptionValues DROP CONSTRAINT ' + 
		(SELECT name FROM sysobjects WHERE xtype = 'PK' AND parent_obj = OBJECT_ID('DiscountOptionValues'))
	EXEC (@drop_sql)

	ALTER TABLE DiscountOptionValues
	ADD CONSTRAINT PK_DiscountOptionValues PRIMARY KEY (IdDiscount, IdOptionType)

	DROP INDEX IX_DiscountOptionValues_Value ON DiscountOptionValues

	ALTER TABLE DiscountOptionValues
	DROP COLUMN Id

	CREATE NONCLUSTERED INDEX IX_DiscountOptionValues_Value 
	ON DiscountOptionValues (Value)

	CREATE NONCLUSTERED INDEX IX_DiscountOptionValues_ValuesSearch 
	ON DiscountOptionValues (Value, IdOptionType) 
	INCLUDE (IdDiscount)
END

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Id' AND object_id = OBJECT_ID(N'OrderAddressOptionValues'))
BEGIN
	ALTER TABLE OrderAddressOptionValues
	DROP CONSTRAINT PK_OrderAddressOptionValues

	ALTER TABLE OrderAddressOptionValues
	ADD CONSTRAINT PK_OrderAddressOptionValues PRIMARY KEY (IdOrderAddress, IdOptionType)

	DROP INDEX IX_OrderAddressOptionValues_Value ON OrderAddressOptionValues

	ALTER TABLE OrderAddressOptionValues
	DROP COLUMN Id

	CREATE NONCLUSTERED INDEX IX_OrderAddressOptionValues_Value 
	ON OrderAddressOptionValues (Value)

	CREATE NONCLUSTERED INDEX IX_OrderAddressOptionValues_ValuesSearch 
	ON OrderAddressOptionValues (Value, IdOptionType) 
	INCLUDE (IdOrderAddress)
END

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Id' AND object_id = OBJECT_ID(N'OrderOptionValues'))
BEGIN
	ALTER TABLE OrderOptionValues
	DROP CONSTRAINT PK_OrderOptionValues

	ALTER TABLE OrderOptionValues
	ADD CONSTRAINT PK_OrderOptionValues PRIMARY KEY (IdOrder, IdOptionType)

	DROP INDEX IX_OrderOptionValues_Value ON OrderOptionValues

	ALTER TABLE OrderOptionValues
	DROP COLUMN Id

	CREATE NONCLUSTERED INDEX IX_OrderOptionValues_Value 
	ON OrderOptionValues (Value)

	CREATE NONCLUSTERED INDEX IX_OrderOptionValues_ValuesSearch 
	ON OrderOptionValues (Value, IdOptionType) 
	INCLUDE (IdOrder)
END

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Id' AND object_id = OBJECT_ID(N'OrderPaymentMethodOptionValues'))
BEGIN
	ALTER TABLE OrderPaymentMethodOptionValues
	DROP CONSTRAINT PK_OrderPaymentMethodOptionValues

	ALTER TABLE OrderPaymentMethodOptionValues
	ADD CONSTRAINT PK_OrderPaymentMethodOptionValues PRIMARY KEY (IdOrderPaymentMethod, IdOptionType)

	DROP INDEX IX_OrderPaymentMethodOptionValues_Value ON OrderPaymentMethodOptionValues

	ALTER TABLE OrderPaymentMethodOptionValues
	DROP COLUMN Id

	CREATE NONCLUSTERED INDEX IX_OrderPaymentMethodOptionValues_Value 
	ON OrderPaymentMethodOptionValues (Value)

	CREATE NONCLUSTERED INDEX IX_OrderPaymentMethodOptionValues_ValuesSearch 
	ON OrderPaymentMethodOptionValues (Value, IdOptionType) 
	INCLUDE (IdOrderPaymentMethod)
END

IF OBJECT_ID(N'SkuOptionValues') IS NULL
BEGIN
	CREATE TABLE [dbo].[SkuOptionValues]
	(
		[IdSku] [int] NOT NULL,
		[IdOptionType] [int] NOT NULL,
		[Value] [nvarchar](250) NULL
		CONSTRAINT FK_SkuOptionValues_ToSku FOREIGN KEY (IdSku) REFERENCES Skus (Id),
		CONSTRAINT FK_SkuOptionValue_ToProductOptionType FOREIGN KEY (IdOptionType) REFERENCES ProductOptionTypes(Id)
	)

	ALTER TABLE SkuOptionValues
	ADD CONSTRAINT PK_SkuOptionValues PRIMARY KEY (IdSku, IdOptionType)

	CREATE NONCLUSTERED INDEX IX_SkuOptionValues_Value 
	ON SkuOptionValues (Value)

	CREATE NONCLUSTERED INDEX IX_SkuOptionValues_ValuesSearch 
	ON SkuOptionValues (Value, IdOptionType) 
	INCLUDE (IdSku)

	EXECUTE sp_executesql
	N'INSERT INTO SkuOptionValues
	(IdSku, IdOptionType, Value)
	SELECT DISTINCT IdSku, IdOptionType, Value 
	FROM ProductOptionValues 
	WHERE IdSku IS NOT NULL'
END

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Id' AND object_id = OBJECT_ID(N'ProductOptionValues'))
BEGIN
	SET @drop_sql = 'ALTER TABLE ProductOptionValues DROP CONSTRAINT ' + 
		(SELECT name FROM sysobjects WHERE xtype = 'PK' AND parent_obj = OBJECT_ID('ProductOptionValues'))
	EXEC (@drop_sql)

	ALTER TABLE ProductOptionValues
	DROP CONSTRAINT FK_ProductOptionValues_ToProduct, FK_ProductOptionValues_ToSku

	DROP INDEX IX_ProductOptionValues_Value ON ProductOptionValues

	ALTER TABLE ProductOptionValues
	DROP COLUMN IdSku

	DELETE FROM ProductOptionValues
	WHERE IdProduct IS NULL

	ALTER TABLE ProductOptionValues
	ALTER COLUMN IdProduct INT NOT NULL

	EXECUTE sp_executesql 
	N'ALTER TABLE ProductOptionValues WITH CHECK
	ADD CONSTRAINT PK_ProductOptionValues PRIMARY KEY (IdProduct, IdOptionType)'

	ALTER TABLE ProductOptionValues
	ADD CONSTRAINT FK_ProductOptionValues_ToProduct FOREIGN KEY (IdProduct) REFERENCES Products (Id)

	ALTER TABLE ProductOptionValues
	DROP COLUMN Id

	CREATE NONCLUSTERED INDEX IX_ProductOptionValues_Value 
	ON ProductOptionValues (Value)

	CREATE NONCLUSTERED INDEX IX_ProductOptionValues_ValuesSearch 
	ON ProductOptionValues (Value, IdOptionType) 
	INCLUDE (IdProduct)
END

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Id' AND object_id = OBJECT_ID(N'PromotionOptionValues'))
BEGIN
	SET @drop_sql = 'ALTER TABLE PromotionOptionValues DROP CONSTRAINT ' + 
		(SELECT name FROM sysobjects WHERE xtype = 'PK' AND parent_obj = OBJECT_ID('PromotionOptionValues'))
	EXEC (@drop_sql)

	ALTER TABLE PromotionOptionValues
	ADD CONSTRAINT PK_PromotionOptionValues PRIMARY KEY (IdPromotion, IdOptionType)

	DROP INDEX IX_PromotionOptionValues_Value ON PromotionOptionValues

	ALTER TABLE PromotionOptionValues
	DROP COLUMN Id

	CREATE NONCLUSTERED INDEX IX_PromotionOptionValues_Value 
	ON PromotionOptionValues (Value)

	CREATE NONCLUSTERED INDEX IX_PromotionOptionValues_ValuesSearch 
	ON PromotionOptionValues (Value, IdOptionType) 
	INCLUDE (IdPromotion)
END