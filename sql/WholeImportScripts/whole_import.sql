GO
USE [VitalChoice.Ecommerce]
GO

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Skus OFF;
GO

SET IDENTITY_INSERT [VitalChoice.Infrastructure].dbo.AspNetUsers OFF;
GO

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.CustomerNotes OFF;
GO

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Products OFF;
GO

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Discounts OFF;
GO

SET IDENTITY_INSERT Orders OFF;
GO

SET IDENTITY_INSERT HealthwisePeriods OFF;
GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Orders') AND name = N'IX_IdCustomer')
	DROP INDEX [IX_IdCustomer] ON [dbo].[Orders]

GO

CREATE NONCLUSTERED INDEX [IX_IdCustomer] ON [dbo].[Orders]
(
	[IdCustomer] DESC
)
INCLUDE ( 	[Id],
	[IdObjectType],
	[DateCreated],
	[DateEdited],
	[IdEditedBy],
	[StatusCode],
	[OrderStatus],
	[IdDiscount],
	[Total],
	[ProductsSubtotal],
	[TaxTotal],
	[ShippingTotal],
	[DiscountTotal],
	[IdPaymentMethod],
	[IdShippingAddress],
	[IdAddedBy],
	[POrderStatus],
	[NPOrderStatus],
	[IdOrderSource]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Orders') AND name = N'IX_IdCustomer_ObjectStatus_OrderStatus_ObjectType')
	DROP INDEX [IX_IdCustomer_ObjectStatus_OrderStatus_ObjectType] ON [dbo].[Orders]

GO

CREATE NONCLUSTERED INDEX [IX_IdCustomer_ObjectStatus_OrderStatus_ObjectType] ON [dbo].[Orders]
(
	[IdCustomer] DESC,
	[IdObjectType] ASC,
	[StatusCode] ASC,
	[OrderStatus] ASC,
	[POrderStatus] ASC,
	[NPOrderStatus] ASC
)
INCLUDE ( 	[Id],
	[DateCreated],
	[DateEdited],
	[IdEditedBy],
	[IdDiscount],
	[Total],
	[ProductsSubtotal],
	[TaxTotal],
	[ShippingTotal],
	[DiscountTotal],
	[IdPaymentMethod],
	[IdShippingAddress],
	[IdAddedBy],
	[IdOrderSource]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Orders') AND name = N'IX_Orders_IdOrderSource')
	DROP INDEX [IX_Orders_IdOrderSource] ON [dbo].[Orders]

GO

CREATE NONCLUSTERED INDEX [IX_Orders_IdOrderSource] ON [dbo].[Orders]
(
	[IdOrderSource] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Orders') AND name = N'IX_Orders_ObjectStatus_OrderDate')
	DROP INDEX [IX_Orders_ObjectStatus_OrderDate] ON [dbo].[Orders]

GO

CREATE NONCLUSTERED INDEX [IX_Orders_ObjectStatus_OrderDate] ON [dbo].[Orders]
(
	[DateCreated] DESC,
	[StatusCode] ASC
)
INCLUDE ( 	[Id],
	[IdObjectType],
	[DateEdited],
	[IdEditedBy],
	[OrderStatus],
	[IdCustomer],
	[IdDiscount],
	[Total],
	[ProductsSubtotal],
	[TaxTotal],
	[ShippingTotal],
	[DiscountTotal],
	[IdPaymentMethod],
	[IdShippingAddress],
	[IdAddedBy],
	[POrderStatus],
	[NPOrderStatus],
	[IdOrderSource]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Orders') AND name = N'IX_Orders_OrderStatus_ObjectStatus_ObjectType_OrderDate')
	DROP INDEX [IX_Orders_OrderStatus_ObjectStatus_ObjectType_OrderDate] ON [dbo].[Orders]

GO

CREATE NONCLUSTERED INDEX [IX_Orders_OrderStatus_ObjectStatus_ObjectType_OrderDate] ON [dbo].[Orders]
(
	[DateCreated] DESC,
	[StatusCode] ASC,
	[OrderStatus] ASC,
	[IdObjectType] ASC,
	[POrderStatus] ASC,
	[NPOrderStatus] ASC
)
INCLUDE ( 	[Id],
	[DateEdited],
	[IdEditedBy],
	[IdCustomer],
	[IdDiscount],
	[Total],
	[ProductsSubtotal],
	[TaxTotal],
	[ShippingTotal],
	[DiscountTotal],
	[IdPaymentMethod],
	[IdShippingAddress],
	[IdAddedBy],
	[IdOrderSource]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Orders') AND name = N'IX_Orders_OrderType_Status_Date')
	DROP INDEX [IX_Orders_OrderType_Status_Date] ON [dbo].[Orders]

GO

CREATE NONCLUSTERED INDEX [IX_Orders_OrderType_Status_Date] ON [dbo].[Orders]
(
	[DateCreated] DESC,
	[IdObjectType] ASC,
	[StatusCode] ASC,
	[POrderStatus] ASC,
	[NPOrderStatus] ASC
)
INCLUDE ( 	[Id],
	[OrderStatus],
	[IdCustomer],
	[IdDiscount],
	[Total],
	[DateEdited],
	[IdEditedBy],
	[ProductsSubtotal],
	[TaxTotal],
	[ShippingTotal],
	[DiscountTotal],
	[IdPaymentMethod],
	[IdShippingAddress],
	[IdAddedBy],
	[IdOrderSource]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Customers') AND name = N'IX_IdObjectType')
	DROP INDEX [IX_IdObjectType] ON [dbo].[Customers]

GO	

CREATE NONCLUSTERED INDEX [IX_IdObjectType] ON [dbo].[Customers]
(
	[IdObjectType] ASC
)
INCLUDE ( 	[Id],
	[DateCreated],
	[DateEdited],
	[IdEditedBy],
	[Email],
	[IdDefaultPaymentMethod],
	[StatusCode],
	[PublicId],
	[IdAffiliate],
	[IdProfileAddress]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Customers') AND name = N'IX_StatusCode_DateCreated')
	DROP INDEX [IX_StatusCode_DateCreated] ON [dbo].[Customers]

GO

CREATE NONCLUSTERED INDEX [IX_StatusCode_DateCreated] ON [dbo].[Customers]
(
	[StatusCode] ASC,
	[DateCreated] DESC
)
INCLUDE ( 	[Id],
	[DateEdited],
	[IdEditedBy],
	[Email],
	[IdDefaultPaymentMethod],
	[PublicId],
	[IdAffiliate],
	[IdProfileAddress],
	[IdObjectType]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Customers') AND name = N'IX_StatusCode')
	DROP INDEX [IX_StatusCode] ON [dbo].[Customers]
	
GO

CREATE NONCLUSTERED INDEX [IX_StatusCode] ON [dbo].[Customers]
(
	[StatusCode] ASC
)
INCLUDE ( 	[Id],
	[IdObjectType],
	[DateCreated],
	[DateEdited],
	[IdEditedBy],
	[Email],
	[IdDefaultPaymentMethod],
	[PublicId],
	[IdAffiliate],
	[IdProfileAddress]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


GO


USE [VitalChoice.Ecommerce]

--INSERT INTO Countries
--(CountryCode, CountryName, [Order], StatusCode, IdVisibility)
--SELECT c.countryCode, c.countryName, ROW_NUMBER() OVER(ORDER BY c.countryCode), 2, NULL FROM [vitalchoice2.0].dbo.countries AS c

--INSERT INTO States
--(CountryCode, StateCode, StateName, StatusCode, [Order])
--SELECT c.pcCountryCode, c.stateCode, c.stateName, 2, ROW_NUMBER() OVER(ORDER BY c.stateCode) FROM [vitalchoice2.0].dbo.states AS c
--INNER JOIN Countries AS cc ON cc.CountryCode = c.pcCountryCode

--INSERT INTO Countries
--(CountryCode, CountryName, [Order], StatusCode, IdVisibility)
--SELECT CASE WHEN c.countryName = 'Taiwan' THEN 'TW' ELSE c.countryCode END, c.countryName, ROW_NUMBER() OVER(ORDER BY c.countryCode), 2, 2 FROM [vitalchoice2.0].dbo.countriesCSPortal AS c
--WHERE c.countryCode NOT IN (SELECT cc.countryCode FROM [vitalchoice2.0].dbo.countries AS cc)

--INSERT INTO States
--(CountryCode, StateCode, StateName, StatusCode, [Order])
--SELECT c.pcCountryCode, c.stateCode, c.stateName, 2, ROW_NUMBER() OVER(ORDER BY c.stateCode) FROM [vitalchoice2.0].dbo.statesCSPortal AS c
--INNER JOIN Countries AS cc ON cc.CountryCode = c.pcCountryCode
--WHERE c.pcCountryCode NOT IN (SELECT s.pcCountryCode FROM [vitalchoice2.0].dbo.states AS s)

GO

DECLARE @adminsLowerSpace INT

SET @adminsLowerSpace = (SELECT MAX(Id) + 1 FROM  [VitalChoice.Infrastructure].dbo.AspNetUsers WHERE Id < 1000)

--============================ Move all admin users to lower id space (from 1) ====================================
DECLARE @aspnetUsers TABLE (
	[Id] [int] NOT NULL PRIMARY KEY,
	[IdOld] INT NOT NULL,
	[PublicId] [uniqueidentifier] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[ConcurrencyStamp] [nvarchar](255) NULL,
	[Email] [nvarchar](100) NOT NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[NormalizedEmail] [nvarchar](100) NOT NULL,
	[NormalizedUserName] [nvarchar](100) NULL,
	[PasswordHash] [nvarchar](255) NULL,
	[PhoneNumber] [nvarchar](100) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[LastLoginDate] [datetime2](7) NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NOT NULL,
	[DeletedDate] [datetime2](7) NULL,
	[SecurityStamp] [nvarchar](255) NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[ConfirmationToken] [uniqueidentifier] NOT NULL,
	[TokenExpirationDate] [datetime2](7) NOT NULL,
	[IsConfirmed] [bit] NOT NULL,
	[IdUserType] [int] NOT NULL
)

DECLARE @aspnetuserroles TABLE (
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL)

INSERT INTO @aspnetUsers
(
	Id,
	IdOld, 
	PublicId, 
	[AccessFailedCount], 
	[ConcurrencyStamp], 
	[Email], 
	[EmailConfirmed], 
	[UserName], 
	[FirstName], 
	[LastName], 
	[Status], 
	[LockoutEnabled], 
	[LockoutEnd], 
	[NormalizedEmail], 
	[NormalizedUserName], 
	[PasswordHash], 
	[PhoneNumber], 
	[PhoneNumberConfirmed],
	[LastLoginDate],
	[CreateDate],
	[UpdatedDate],
	[DeletedDate],
	[SecurityStamp],
	[TwoFactorEnabled],
	[ConfirmationToken],
	[TokenExpirationDate],
	[IsConfirmed],
	[IdUserType]
)
SELECT ROW_NUMBER() OVER (ORDER BY u.Id) + @adminsLowerSpace, 
	Id, 
	PublicId, 
	[AccessFailedCount], 
	[ConcurrencyStamp], 
	[Email], 
	[EmailConfirmed], 
	[UserName], 
	[FirstName], 
	[LastName], 
	[Status], 
	[LockoutEnabled], 
	[LockoutEnd], 
	[NormalizedEmail], 
	[NormalizedUserName], 
	[PasswordHash], 
	[PhoneNumber], 
	[PhoneNumberConfirmed],
	[LastLoginDate],
	[CreateDate],
	[UpdatedDate],
	[DeletedDate],
	[SecurityStamp],
	[TwoFactorEnabled],
	[ConfirmationToken],
	[TokenExpirationDate],
	[IsConfirmed],
	[IdUserType] 
FROM [VitalChoice.Infrastructure].dbo.AspNetUsers AS u
WHERE IdUserType = 1 AND Id > 1000

INSERT INTO @aspnetuserroles
(RoleId, UserId)
SELECT RoleId, u.Id FROM [VitalChoice.Infrastructure].dbo.AspNetUserRoles AS r
INNER JOIN @aspnetUsers AS u ON u.IdOld = r.UserId

UPDATE [VitalChoice.Infrastructure].dbo.AspNetUsers
SET PublicId = NEWID()
WHERE IdUserType = 1

SET IDENTITY_INSERT [VitalChoice.Infrastructure].dbo.AspNetUsers ON;

INSERT INTO [VitalChoice.Infrastructure].dbo.AspNetUsers
(
	Id, 
	PublicId, 
	[AccessFailedCount], 
	[ConcurrencyStamp], 
	[Email], 
	[EmailConfirmed], 
	[UserName], 
	[FirstName], 
	[LastName], 
	[Status], 
	[LockoutEnabled], 
	[LockoutEnd], 
	[NormalizedEmail], 
	[NormalizedUserName], 
	[PasswordHash], 
	[PhoneNumber], 
	[PhoneNumberConfirmed],
	[LastLoginDate],
	[CreateDate],
	[UpdatedDate],
	[DeletedDate],
	[SecurityStamp],
	[TwoFactorEnabled],
	[ConfirmationToken],
	[TokenExpirationDate],
	[IsConfirmed],
	[IdUserType]
)
SELECT 
	Id, 
	PublicId, 
	[AccessFailedCount], 
	[ConcurrencyStamp], 
	[Email], 
	[EmailConfirmed], 
	[UserName], 
	[FirstName], 
	[LastName], 
	[Status], 
	[LockoutEnabled], 
	[LockoutEnd], 
	[NormalizedEmail], 
	[NormalizedUserName], 
	[PasswordHash], 
	[PhoneNumber], 
	[PhoneNumberConfirmed],
	[LastLoginDate],
	[CreateDate],
	[UpdatedDate],
	[DeletedDate],
	[SecurityStamp],
	[TwoFactorEnabled],
	[ConfirmationToken],
	[TokenExpirationDate],
	[IsConfirmed],
	[IdUserType] 
FROM @aspnetUsers

SET IDENTITY_INSERT [VitalChoice.Infrastructure].dbo.AspNetUsers OFF

INSERT INTO [VitalChoice.Infrastructure].dbo.AspNetUserRoles
(RoleId, UserId)
SELECT RoleId, UserId FROM @aspnetuserroles

UPDATE [VitalChoice.Infrastructure].dbo.AdminProfiles
SET Id = u.Id
FROM [VitalChoice.Infrastructure].dbo.AdminProfiles AS a
INNER JOIN @aspnetUsers AS u ON u.IdOld = a.Id

UPDATE [VitalChoice.Infrastructure].dbo.BugTickets
SET IdAddedBy = u.Id
FROM [VitalChoice.Infrastructure].dbo.BugTickets AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdAddedBy

UPDATE [VitalChoice.Infrastructure].dbo.BugTickets
SET IdEditedBy = u.Id
FROM [VitalChoice.Infrastructure].dbo.BugTickets AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Infrastructure].dbo.BugTicketComments
SET IdEditedBy = u.Id
FROM [VitalChoice.Infrastructure].dbo.BugTicketComments AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Infrastructure].dbo.ContentAreas
SET IdEditedBy = u.Id
FROM [VitalChoice.Infrastructure].dbo.ContentAreas AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Infrastructure].dbo.CustomPublicStyles
SET IdEditedBy = u.Id
FROM [VitalChoice.Infrastructure].dbo.CustomPublicStyles AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Infrastructure].dbo.MasterContentItems
SET IdEditedBy = u.Id
FROM [VitalChoice.Infrastructure].dbo.MasterContentItems AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Infrastructure].dbo.Articles
SET UserId = u.Id
FROM [VitalChoice.Infrastructure].dbo.Articles AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.UserId

UPDATE [VitalChoice.Infrastructure].dbo.ContentCategories
SET UserId = u.Id
FROM [VitalChoice.Infrastructure].dbo.ContentCategories AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.UserId

UPDATE [VitalChoice.Infrastructure].dbo.ContentPages
SET UserId = u.Id
FROM [VitalChoice.Infrastructure].dbo.ContentPages AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.UserId

UPDATE [VitalChoice.Infrastructure].dbo.EmailTemplates
SET UserId = u.Id
FROM [VitalChoice.Infrastructure].dbo.EmailTemplates AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.UserId

UPDATE [VitalChoice.Infrastructure].dbo.FAQs
SET UserId = u.Id
FROM [VitalChoice.Infrastructure].dbo.FAQs AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.UserId

UPDATE [VitalChoice.Infrastructure].dbo.ProductCategories
SET UserId = u.Id
FROM [VitalChoice.Infrastructure].dbo.ProductCategories AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.UserId

UPDATE [VitalChoice.Infrastructure].dbo.Recipes
SET UserId = u.Id
FROM [VitalChoice.Infrastructure].dbo.Recipes AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.UserId

UPDATE [VitalChoice.Infrastructure].dbo.Redirects
SET IdAddedBy = u.Id
FROM [VitalChoice.Infrastructure].dbo.Redirects AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdAddedBy

UPDATE [VitalChoice.Infrastructure].dbo.Redirects
SET IdEditedBy = u.Id
FROM [VitalChoice.Infrastructure].dbo.Redirects AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

INSERT INTO [VitalChoice.Ecommerce].dbo.Users
(Id)
SELECT Id FROM @aspnetUsers

UPDATE [VitalChoice.Ecommerce].dbo.Addresses
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.Addresses AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.CustomerTypes
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.CustomerTypes AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.InventorySkus
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.InventorySkus AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.Discounts
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.Discounts AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.Discounts
SET IdAddedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.Discounts AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdAddedBy

UPDATE [VitalChoice.Ecommerce].dbo.Promotions
SET IdAddedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.Promotions AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdAddedBy

UPDATE [VitalChoice.Ecommerce].dbo.Promotions
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.Promotions AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.OrderAddresses
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.OrderAddresses AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.OrderNotes
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.OrderNotes AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.PaymentMethods
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.PaymentMethods AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.Customers
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.Customers AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.CustomerPaymentMethods
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.CustomerPaymentMethods AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.CustomerNotes
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.CustomerNotes AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.OrderPaymentMethods
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.OrderPaymentMethods AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.OrderPaymentMethods
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.OrderPaymentMethods AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.Orders
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.Orders AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

UPDATE [VitalChoice.Ecommerce].dbo.Products
SET IdEditedBy = u.Id
FROM [VitalChoice.Ecommerce].dbo.Products AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.IdEditedBy

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUserRoles
WHERE UserId IN (
	SELECT Id FROM [VitalChoice.Infrastructure].dbo.AspNetUsers
	WHERE IdUserType = 1 AND Id > 1000
)

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUsers
WHERE IdUserType = 1 AND Id > 1000

DELETE FROM [VitalChoice.Ecommerce].dbo.Users
WHERE Id > 1000

GO


USE [vitalchoice2.0]
GO
IF EXISTS(SELECT * FROM sys.procedures WHERE name = N'MoveAffiliateTextField')
	DROP PROCEDURE dbo.MoveAffiliateTextField
GO

CREATE PROCEDURE dbo.MoveAffiliateTextField
(@sourceColumnName NVARCHAR(250), @destFieldName NVARCHAR(250), @sourceCondition NVARCHAR(MAX) = NULL)
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX)

	IF @sourceCondition IS NOT NULL

	SET @sql = N'DECLARE @fieldType INT
	DECLARE @textData NVARCHAR(MAX), @idAffiliate INT
	DECLARE @bigId TABLE(Id BIGINT NOT NULL)

	DECLARE src CURSOR FOR
	SELECT a.'+@sourceColumnName+N', p.Id FROM affiliates AS a
	INNER JOIN [VitalChoice.Ecommerce].dbo.affiliates AS p ON p.Id = a.idAffiliate
	WHERE ('+@sourceCondition+N') AND a.'+@sourceColumnName+N' IS NOT NULL AND a.'+@sourceColumnName+N' <> N''''

	SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.AffiliateOptionTypes WHERE Name = N'''+@destFieldName+N''' 

	OPEN src

	FETCH NEXT FROM src
	INTO @textData, @idAffiliate

	WHILE @@FETCH_STATUS = 0
	BEGIN

		INSERT INTO [VitalChoice.Ecommerce].dbo.BigStringValues
		(Value)
		OUTPUT inserted.IdBigString INTO @bigId
		VALUES
		(@textData)

		INSERT INTO [VitalChoice.Ecommerce].dbo.AffiliateOptionValues
		(idAffiliate, IdOptionType, IdBigString)
		VALUES
		(@idAffiliate, @fieldType, (SELECT TOP 1 Id FROM @bigId))

		DELETE FROM @bigId

		FETCH NEXT FROM src
		INTO @textData, @idAffiliate
	END

	CLOSE src;
	DEALLOCATE src;';

	ELSE

	SET @sql = N'DECLARE @fieldType INT
	DECLARE @textData NVARCHAR(MAX), @idAffiliate INT
	DECLARE @bigId TABLE(Id BIGINT NOT NULL)

	DECLARE src CURSOR FOR
	SELECT a.'+@sourceColumnName+N', p.Id FROM affiliates AS a
	INNER JOIN [VitalChoice.Ecommerce].dbo.affiliates AS p ON p.Id = a.idAffiliate
	WHERE a.'+@sourceColumnName+N' IS NOT NULL AND a.'+@sourceColumnName+N' <> N''''

	SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.AffiliateOptionTypes WHERE Name = N'''+@destFieldName+N''' 

	OPEN src

	FETCH NEXT FROM src
	INTO @textData, @idAffiliate

	WHILE @@FETCH_STATUS = 0
	BEGIN

		INSERT INTO [VitalChoice.Ecommerce].dbo.BigStringValues
		(Value)
		OUTPUT inserted.IdBigString INTO @bigId
		VALUES
		(@textData)

		INSERT INTO [VitalChoice.Ecommerce].dbo.AffiliateOptionValues
		(idAffiliate, IdOptionType, IdBigString)
		VALUES
		(@idAffiliate, @fieldType, (SELECT TOP 1 Id FROM @bigId))

		DELETE FROM @bigId

		FETCH NEXT FROM src
		INTO @textData, @idAffiliate
	END

	CLOSE src;
	DEALLOCATE src;';

	EXEC (@sql)
END
GO

IF EXISTS(SELECT * FROM sys.procedures WHERE name = N'MoveAffiliateField')
	DROP PROCEDURE dbo.MoveAffiliateField
GO

CREATE PROCEDURE dbo.MoveAffiliateField
(@destFieldName NVARCHAR(250), @sourceFieldName NVARCHAR(250), @fieldOperation NVARCHAR(MAX) = NULL, @sourceConditions NVARCHAR(MAX) = NULL)
AS
BEGIN
BEGIN TRY
	DECLARE @sql NVARCHAR(MAX)
	
	IF @fieldOperation IS NULL

		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.AffiliateOptionValues
		(IdOptionType, IdAffiliate, Value)
		SELECT t.Id, p.Id, a.'+@sourceFieldName+' FROM [VitalChoice.Ecommerce].dbo.Affiliates AS p
		INNER JOIN [vitalchoice2.0].[dbo].affiliates AS a ON a.IdAffiliate = p.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.AffiliateOptionTypes AS t ON t.Name = N'''+@destFieldName+'''
		WHERE a.'+@sourceFieldName+' IS NOT NULL AND ('+ISNULL(@sourceConditions, '1=1')+')';

	ELSE
		
		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.AffiliateOptionValues
		(IdOptionType, IdAffiliate, Value)
		SELECT t.Id, p.Id, '+@fieldOperation+' FROM [VitalChoice.Ecommerce].dbo.Affiliates AS p
		INNER JOIN [vitalchoice2.0].[dbo].affiliates AS a ON a.IdAffiliate = p.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.AffiliateOptionTypes AS t ON t.Name = N'''+@destFieldName+'''
		WHERE a.'+@sourceFieldName+' IS NOT NULL AND ('+ISNULL(@sourceConditions, '1=1')+')';

	EXEC(@sql)
END TRY
BEGIN CATCH
	SELECT 
		ERROR_MESSAGE() AS [Message],
		@destFieldName AS dest,
		@sourceFieldName AS source
END CATCH
END

GO
--============================ Insert Base Users and Customers Entity ====================================

SET IDENTITY_INSERT [VitalChoice.Infrastructure].dbo.AspNetUsers ON

INSERT INTO [VitalChoice.Infrastructure].dbo.AspNetUsers
(Id,
PublicId, 
Email, 
NormalizedEmail, 
EmailConfirmed, 
UserName, 
NormalizedUserName, 
FirstName, 
LastName,
Status, 
LockoutEnabled, 
PhoneNumber, 
PhoneNumberConfirmed, 
CreateDate, 
UpdatedDate, 
TwoFactorEnabled, 
ConfirmationToken, 
TokenExpirationDate, 
IsConfirmed, 
IdUserType,
AccessFailedCount,
PasswordHash,
SecurityStamp,
ConcurrencyStamp)
SELECT 
	aff.idAffiliate,
	NEWID(), 
	aff.affiliateEmail, 
	UPPER(aff.affiliateEmail),  
	1,
	aff.affiliateEmail,
	UPPER(aff.affiliateEmail),
	ISNULL(aff.affiliateName, ''),
	'',
	1,
	1,
	'',
	0,
	ISNULL(CAST(aff.joinDate AS DATETIME), GETDATE()), 
	ISNULL(CAST(aff.lastUpdated AS DATETIME), GETDATE()), 
	0,
	NEWID(),
	GETDATE(),
	1,
	3,
	0,
	[vitalchoice2.0].dbo.HashPassword([vitalchoice2.0].dbo.RC4Encode(aff.pcaff_Password)),
	LOWER(CAST(NEWID() AS NVARCHAR(250))),
	LOWER(CAST(NEWID() AS NVARCHAR(250)))
FROM [vitalchoice2.0].dbo.affiliates AS aff

SET IDENTITY_INSERT [VitalChoice.Infrastructure].dbo.AspNetUsers OFF

GO

--ALTER TABLE [VitalChoice.Ecommerce].dbo.Affiliates
--DROP COLUMN IdOld

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.Affiliates
(Id, DateCreated, DateEdited, County, CommissionAll, CommissionFirst, Email, IdCountry, IdState, MyAppBalance, Name, StatusCode)
SELECT 
	a.idAffiliate,
	ISNULL(CAST(a.joinDate AS DATETIME), GETDATE()), 
	ISNULL(CAST(a.lastUpdated AS DATETIME), GETDATE()), 
	a.affiliateState, 
	a.commission, 
	a.commission2, 
	a.affiliateEmail, 
	ISNULL((SELECT TOP 1 c.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS c WHERE c.CountryCode = a.affiliatecountryCode), (SELECT TOP 1 c.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS c WHERE c.CountryCode = 'US')), 
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode = a.affiliatecountryCode AND s.StateCode = a.affiliateState), 
	a.ComissionsAmount, 
	a.affiliateName, 
	CASE WHEN a.pcaff_Active <> 0 THEN 2 ELSE 1 END
FROM [vitalchoice2.0].dbo.affiliates AS a

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.Users
(Id)
SELECT Id FROM [VitalChoice.Ecommerce].dbo.Affiliates

EXEC dbo.MoveAffiliateField @destFieldName = 'Profession', @sourceFieldName = 'profession'
EXEC dbo.MoveAffiliateField @destFieldName = 'Reach', @sourceFieldName = 'reach'
EXEC dbo.MoveAffiliateField @destFieldName = 'WebSite', @sourceFieldName = 'pcaff_website'
EXEC dbo.MoveAffiliateField @destFieldName = 'HearAbout', @sourceFieldName = 'heard'
EXEC dbo.MoveAffiliateField @destFieldName = 'Zip', @sourceFieldName = 'affiliatezip'
EXEC dbo.MoveAffiliateField @destFieldName = 'City', @sourceFieldName = 'affiliateCity'
EXEC dbo.MoveAffiliateField @destFieldName = 'Phone', @sourceFieldName = 'affiliatephone'
EXEC dbo.MoveAffiliateField @destFieldName = 'Address2', @sourceFieldName = 'affiliateAddress2'
EXEC dbo.MoveAffiliateField @destFieldName = 'Address1', @sourceFieldName = 'affiliateAddress'
EXEC dbo.MoveAffiliateField @destFieldName = 'Fax', @sourceFieldName = 'affiliatefax'
EXEC dbo.MoveAffiliateField @destFieldName = 'Company', @sourceFieldName = 'affiliateCompany'
EXEC dbo.MoveAffiliateField @destFieldName = 'TaxID', @sourceFieldName = 'taxId'
EXEC dbo.MoveAffiliateField @destFieldName = 'ChecksPayableTo', @sourceFieldName = 'PayableTo'
EXEC dbo.MoveAffiliateField @destFieldName = 'Facebook', @sourceFieldName = 'promoFacebookUrl'
EXEC dbo.MoveAffiliateField @destFieldName = 'Twitter', @sourceFieldName = 'promoTwitterUrl'
EXEC dbo.MoveAffiliateField @destFieldName = 'Blog', @sourceFieldName = 'promoBlogUrl'
EXEC dbo.MoveAffiliateField @destFieldName = 'Tier', @sourceFieldName = 'tier', @fieldOperation = 'CAST(a.tier AS NVARCHAR(2))'
EXEC dbo.MoveAffiliateField @destFieldName = 'PaymentType', @sourceFieldName = 'paymentAs', @fieldOperation = 'CAST(a.paymentAs AS NVARCHAR(2))'
EXEC dbo.MoveAffiliateField @destFieldName = 'ProfessionalPractice', @sourceFieldName = 'promoProffPracticeDropDown', @fieldOperation = 'CAST(a.promoProffPracticeDropDown AS NVARCHAR(2))'
EXEC dbo.MoveAffiliateField @destFieldName = 'PromoteByDrSearsLEANCoachAmbassador', @sourceFieldName = 'drSears', @fieldOperation = 'CASE WHEN a.drSears <> 0 THEN ''True'' ELSE ''False'' END'
EXEC dbo.MoveAffiliateField @destFieldName = 'PromoteByVerticalResponseEmail', @sourceFieldName = 'VerticalResponseEmail', @fieldOperation = 'CASE WHEN a.VerticalResponseEmail <> 0 THEN ''True'' ELSE ''False'' END'
EXEC dbo.MoveAffiliateField @destFieldName = 'PromoteByLeanEmail', @sourceFieldName = 'LeanEmail', @fieldOperation = 'CASE WHEN a.LeanEmail <> 0 THEN ''True'' ELSE ''False'' END'
EXEC dbo.MoveAffiliateField @destFieldName = 'PromoteByProfessionalPractice', @sourceFieldName = 'promoProffPractice', @fieldOperation = 'CASE WHEN a.promoProffPractice <> 0 THEN ''True'' ELSE ''False'' END'
EXEC dbo.MoveAffiliateField @destFieldName = 'PromoteByBlog', @sourceFieldName = 'promoBlog', @fieldOperation = 'CASE WHEN a.promoBlog <> 0 THEN ''True'' ELSE ''False'' END'
EXEC dbo.MoveAffiliateField @destFieldName = 'PromoteByTwitter', @sourceFieldName = 'promoTwitter', @fieldOperation = 'CASE WHEN a.promoTwitter <> 0 THEN ''True'' ELSE ''False'' END'
EXEC dbo.MoveAffiliateField @destFieldName = 'PromoteByFacebook', @sourceFieldName = 'promoFacebook', @fieldOperation = 'CASE WHEN a.promoFacebook <> 0 THEN ''True'' ELSE ''False'' END'
EXEC dbo.MoveAffiliateField @destFieldName = 'PromoteByEmails', @sourceFieldName = 'promoEmailLinks', @fieldOperation = 'CASE WHEN a.promoEmailLinks <> 0 THEN ''True'' ELSE ''False'' END'
EXEC dbo.MoveAffiliateField @destFieldName = 'PromoteByWebsite', @sourceFieldName = 'promoWebSite', @fieldOperation = 'CASE WHEN a.promoWebSite <> 0 THEN ''True'' ELSE ''False'' END'
EXEC dbo.MoveAffiliateField @destFieldName = 'BrickAndMortar', @sourceFieldName = 'BrickNMortar', @fieldOperation = 'CASE WHEN a.BrickNMortar <> 0 THEN ''True'' ELSE ''False'' END'
EXEC dbo.MoveAffiliateField @destFieldName = 'MonthlyEmailsSent', @sourceFieldName = 'promoEmailQty', @fieldOperation = 'CAST(a.promoEmailQty AS NVARCHAR(2))'
EXEC dbo.MoveAffiliateTextField @sourceColumnName = 'notes', @destFieldName = 'Notes'

INSERT INTO [VitalChoice.Infrastructure].dbo.AspNetUserRoles
(RoleId, UserId)
 SELECT 8, Id 
 FROM [VitalChoice.Ecommerce].dbo.Affiliates

GO


USE [vitalchoice2.0]

--============================ Insert Base Users and Customers Entity ====================================

--GO

--ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
--DROP CONSTRAINT UQ_Customers

--GO

--DROP INDEX IX_Addresses_IdCustomer ON [VitalChoice.Ecommerce].dbo.Addresses

--GO

--ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
--DROP COLUMN IdCustomer

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
ADD IdCustomer INT NOT NULL CONSTRAINT UQ_Customers UNIQUE
GO

INSERT INTO [VitalChoice.Ecommerce].dbo.Addresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdCustomer)
SELECT 
	ISNULL(pcCust_DateCreated, GETDATE()), 
	ISNULL(lastEditDate, ISNULL(pcCust_DateCreated, GETDATE())),
	ISNULL (
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = ISNULL(c.countryCode, 'US')), 
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode = ISNULL(c.countryCode, 'US') AND s.StateCode = c.stateCode),
	2,
	1,
	c.[state],
	c.idcustomer
FROM [vitalchoice2.0].dbo.customers AS c
PRINT '====profile address base'

INSERT INTO [VitalChoice.Ecommerce].dbo.Users
(Id)
SELECT idCustomer FROM customers
PRINT '====ecommerce user ids'
	
INSERT INTO [VitalChoice.Ecommerce].dbo.Customers
(Id, DateCreated, DateEdited, Email, IdObjectType, StatusCode, IdDefaultPaymentMethod, PublicId, IdAffiliate, IdProfileAddress)
SELECT 
c.idCustomer, 
ISNULL(c.pcCust_DateCreated, GETDATE()), 
ISNULL(c.lastEditDate, ISNULL(c.pcCust_DateCreated, GETDATE())), 
ISNULL(c.email, 'invalid@e.com'), 
CASE WHEN customerType = 0 THEN 1 WHEN customerType = 1 THEN 2 ELSE 1 END, 
CASE WHEN pcCust_Guest = 1 THEN 1 ELSE CASE WHEN [suspend] = 1 THEN CASE WHEN c.email IS NULL THEN 2 ELSE 1 END ELSE 2 END END, 1, 
NEWID(),
a.Id,
addr.Id
FROM customers AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS addr ON addr.IdCustomer = c.idcustomer
LEFT JOIN [VitalChoice.Ecommerce].dbo.Affiliates AS a ON a.Id = c.idAffiliate
PRINT '====ecommerce customers base'
GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
DROP CONSTRAINT UQ_Customers

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
DROP COLUMN IdCustomer
--============================ Insert Fields ====================================

GO

DECLARE @fieldType INT, @lookupId INT

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'DoNotMail'

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT idCustomer, @fieldType, N'True' 
FROM customers 
WHERE dnm = 1

GO

DECLARE @fieldType INT, @lookupId INT

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'DoNotRent'

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT idCustomer, @fieldType, N'True' 
FROM customers 
WHERE dnr = 1
GO

DECLARE @fieldType INT

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'HasHealthwiseOrders'

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT idCustomer, @fieldType, N'True' 
FROM customers AS c
WHERE EXISTS(SELECT * FROM healthwise AS hw WHERE hw.customerId = c.idcustomer)

GO

DECLARE @fieldType INT, @lookupId INT
DECLARE @SuspensionReason NVARCHAR(MAX), @IdCustomer INT

DECLARE customer_reason CURSOR FOR
SELECT reason, idcustomer FROM customers WHERE [suspend] = 1 AND reason IS NOT NULL AND reason <> '';

OPEN customer_reason

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'SuspensionReason'

FETCH NEXT FROM customer_reason
INTO @SuspensionReason, @IdCustomer

WHILE @@FETCH_STATUS = 0
BEGIN
	INSERT INTO [VitalChoice.Ecommerce].dbo.BigStringValues
	(Value)
	VALUES
	(@SuspensionReason)

	INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
	(IdCustomer, IdOptionType, IdBigString)
	VALUES
	(@IdCustomer, @fieldType, SCOPE_IDENTITY())

	FETCH NEXT FROM customer_reason
	INTO @SuspensionReason, @IdCustomer
END

CLOSE customer_reason;
DEALLOCATE customer_reason;

GO

DECLARE @fieldType INT, @lookupId INT

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'Guest' AND IdObjectType = 1

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT idCustomer, @fieldType, N'True'
FROM customers 
WHERE pcCust_Guest = 1

GO

DECLARE @fieldType INT, @lookupId INT

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'TaxExempt' AND IdObjectType = 2

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT idCustomer, @fieldType, N'2'
FROM customers 
WHERE TaxExempt = 0 AND customerType = 1

GO

DECLARE @fieldType INT, @lookupId INT

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'Website' AND IdObjectType = 2

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT idCustomer, @fieldType, website
FROM customers 
WHERE website IS NOT NULL AND website <> '' AND customerType = 1

GO

DECLARE @fieldType INT, @lookupId INT

DECLARE @PromotingWebsites NVARCHAR(MAX), @IdCustomer INT

DECLARE customer_promowebsites CURSOR FOR
SELECT promowebsites, idCustomer FROM customers WHERE customerType = 1 AND promowebsites IS NOT NULL AND promowebsites <> '';

OPEN customer_promowebsites

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'PromotingWebsites'

FETCH NEXT FROM customer_promowebsites
INTO @PromotingWebsites, @IdCustomer

WHILE @@FETCH_STATUS = 0
BEGIN
	INSERT INTO [VitalChoice.Ecommerce].dbo.BigStringValues
	(Value)
	VALUES
	(@PromotingWebsites)

	INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
	(IdCustomer, IdOptionType, IdBigString)
	VALUES
	(@IdCustomer, @fieldType, SCOPE_IDENTITY())

	FETCH NEXT FROM customer_promowebsites
	INTO @PromotingWebsites, @IdCustomer
END

CLOSE customer_promowebsites;
DEALLOCATE customer_promowebsites;

GO

DECLARE @fieldType INT, @lookupId INT

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'InceptionDate' AND IdObjectType = 2

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT c.idCustomer, @fieldType, CONVERT(NVARCHAR(250), DATEADD(hour, -5,MIN(o.orderDate)), 126)
FROM customers AS c
INNER JOIN orders AS o ON o.idCustomer = c.idcustomer
WHERE o.orderStatus NOT IN (1, 5) AND c.customerType = 1
GROUP BY c.idcustomer
HAVING COUNT(o.idOrder) > 0

GO

DECLARE @fieldType INT, @lookupId INT

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'Tier' AND IdObjectType = 2

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT c.idCustomer, @fieldType, CAST(c.Tier AS NVARCHAR(2))
FROM customers AS c
WHERE c.customerType = 1 AND c.Tier > 1

GO

DECLARE @fieldType INT, @lookupId INT

SELECT TOP 1 @fieldType = Id, @lookupId = IdLookup FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'TradeClass' AND IdObjectType = 2

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT c.idCustomer, @fieldType, CAST(l.Id AS NVARCHAR(3))
FROM customers AS c
INNER JOIN tradeCategories AS t ON t.id = c.tradeCategoryId
INNER JOIN [VitalChoice.Ecommerce].dbo.LookupVariants AS l ON IdLookup = @lookupId AND ValueVariant = t.Name
WHERE c.customerType = 1 AND l.Id <> 1

GO

DECLARE @fieldType INT, @lookupId INT

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'Source'
SELECT TOP 1 @lookupId = Id FROM [VitalChoice.Ecommerce].dbo.Lookups WHERE Name = N'OrderSources'

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT c.idCustomer, @fieldType, CAST(l.Id AS NVARCHAR(3))
FROM customers AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.LookupVariants AS l ON IdLookup = @lookupId AND ValueVariant = c.source
WHERE c.source IS NOT NULL AND c.source <> ''

GO

DECLARE @fieldType INT, @lookupId INT

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'SourceDetails'

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT c.idCustomer, @fieldType, c.sourceDetails
FROM customers AS c
WHERE c.sourceDetails IS NOT NULL AND c.sourceDetails <> ''
GO
--============================ Move Addresses ====================================

INSERT INTO [VitalChoice.Ecommerce].dbo.AddressOptionValues
(IdAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(c.address AS NVARCHAR(255)) AS Address1, 
	c.Address2, 
	c.name AS FirstName, 
	c.LastName, 
	c.customerCompany AS Company, 
	CAST(c.City AS NVARCHAR(255)) AS City, 
	CAST(c.Zip AS NVARCHAR(255)) AS Zip,
	CAST(c.Phone AS NVARCHAR(255)) AS Phone,
	CAST(c.Fax AS NVARCHAR(255)) AS Fax,
	c.Email
FROM [vitalchoice2.0].dbo.customers AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS cc ON cc.Id = c.idcustomer
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.Id = cc.IdProfileAddress) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax, Email)
)AS unpvt
INNER JOIN [VitalChoice.Ecommerce].dbo.AddressOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 1)
PRINT '====address values (profile)'

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
ADD IdCustomer INT NULL
GO

INSERT INTO [VitalChoice.Ecommerce].dbo.Addresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdCustomer)
SELECT 
	ISNULL(pcCust_DateCreated, GETDATE()), 
	ISNULL(lastEditDate, ISNULL(pcCust_DateCreated, GETDATE())),
	ISNULL (
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = ISNULL(c.shippingCountryCode, 'US')), 
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode = ISNULL(c.shippingCountryCode, 'US') AND s.StateCode = c.shippingStateCode),
	2,
	3,
	c.[shippingState],
	c.idcustomer
FROM [vitalchoice2.0].dbo.customers AS c
PRINT '====addresses base (shipping default)'

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerToShippingAddresses
(IdAddress, IdCustomer)
SELECT Id, IdCustomer FROM [VitalChoice.Ecommerce].dbo.Addresses AS a WHERE IdCustomer IS NOT NULL
PRINT '====shipping default connection'

INSERT INTO [VitalChoice.Ecommerce].dbo.AddressOptionValues
(IdAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(c.address AS NVARCHAR(255)) AS Address1, 
	c.Address2, 
	c.name AS FirstName, 
	c.LastName, 
	c.customerCompany AS Company, 
	CAST(c.City AS NVARCHAR(255)) AS City, 
	CAST(c.Zip AS NVARCHAR(255)) AS Zip,
	CAST(c.Phone AS NVARCHAR(255)) AS Phone,
	CAST(c.Fax AS NVARCHAR(255)) AS Fax,
	c.Email,
	CAST(N'True' AS NVARCHAR(255)) AS [Default]
FROM [vitalchoice2.0].dbo.customers AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS cc ON cc.Id = c.idcustomer
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.IdCustomer = c.idcustomer) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax, Email, [Default])
)AS unpvt
INNER JOIN [VitalChoice.Ecommerce].dbo.AddressOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 3)
PRINT '====address values (shipping default)'

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
DROP COLUMN IdCustomer

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
ADD IdCustomer INT NULL, idRecipient INT NULL


GO

INSERT INTO [VitalChoice.Ecommerce].dbo.Addresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdCustomer, idRecipient)
SELECT 
	ISNULL(pcCust_DateCreated, GETDATE()), 
	ISNULL(lastEditDate, ISNULL(pcCust_DateCreated, GETDATE())),
	ISNULL (
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = ISNULL(c.recipient_CountryCode, 'US')), 
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode = ISNULL(c.recipient_CountryCode, 'US') AND s.StateCode = c.recipient_StateCode),
	2,
	3,
	c.[recipient_State],
	c.idcustomer,
	c.idRecipient
FROM [vitalchoice2.0].dbo.recipients AS c
INNER JOIN [vitalchoice2.0].dbo.customers AS cc ON cc.idcustomer = c.idCustomer
PRINT '====addresses (shipping)'

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerToShippingAddresses
(IdAddress, IdCustomer)
SELECT Id, IdCustomer FROM [VitalChoice.Ecommerce].dbo.Addresses AS a WHERE IdCustomer IS NOT NULL
PRINT '====shipping default connection'

INSERT INTO [VitalChoice.Ecommerce].dbo.AddressOptionValues
(IdAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(c.recipient_Address AS NVARCHAR(255)) AS Address1, 
	CAST(c.recipient_Address2 AS NVARCHAR(255)) AS Address2, 
	CAST(c.recipient_FirstName AS NVARCHAR(255)) AS FirstName, 
	CAST(c.recipient_LastName AS NVARCHAR(255)) AS LastName, 
	CAST(c.recipient_Company AS NVARCHAR(255)) AS Company, 
	CAST(c.recipient_City AS NVARCHAR(255)) AS City, 
	CAST(c.recipient_Zip AS NVARCHAR(255)) AS Zip,
	CAST(c.recipient_Phone AS NVARCHAR(255)) AS Phone,
	CAST(c.recipient_Fax AS NVARCHAR(255)) AS Fax,
	CAST(c.recipient_Email AS NVARCHAR(255)) AS Email
FROM [vitalchoice2.0].dbo.recipients AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS cc ON cc.Id = c.idcustomer
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.IdCustomer = c.idcustomer AND a.idRecipient = c.idRecipient) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax, Email)
)AS unpvt
INNER JOIN [VitalChoice.Ecommerce].dbo.AddressOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 3)
PRINT '====address values (shipping)'

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
DROP COLUMN IdCustomer, idRecipient

GO

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.CustomerNotes ON;

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerNotes
(Id, DateCreated, DateEdited, IdCustomer, Note, StatusCode)
SELECT n.id, n.createdDate, n.createdDate, n.idCustomer, n.description, 2 FROM [vitalchoice2.0].dbo.Notes AS n
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS c ON c.Id = n.idCustomer
PRINT '====customer notes'

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.CustomerNotes OFF;

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerNoteOptionValues
(IdCustomerNote, IdOptionType, Value)
SELECT n.id, t.Id, CAST(lv.Id AS NVARCHAR(100)) FROM [vitalchoice2.0].dbo.Notes AS n
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS c ON c.Id = n.idCustomer
INNER JOIN [VitalChoice.Ecommerce].dbo.CustomerNoteOptionTypes AS t ON t.Name = 'Priority'
INNER JOIN [VitalChoice.Ecommerce].dbo.LookupVariants AS lv ON lv.IdLookup = t.IdLookup AND lv.ValueVariant = n.priority
PRINT '====customer notes values'

GO

IF OBJECT_ID('[dbo].[DelimitedSplit8K]') IS NOT NULL
DROP FUNCTION [dbo].[DelimitedSplit8K]
GO

CREATE FUNCTION [dbo].[DelimitedSplit8K]
--===== Define I/O parameters
        (@pString VARCHAR(8000), @pDelimiter CHAR(1))
--WARNING!!! DO NOT USE MAX DATA-TYPES HERE!  IT WILL KILL PERFORMANCE!
RETURNS TABLE WITH SCHEMABINDING AS
 RETURN
--===== "Inline" CTE Driven "Tally Table" produces values from 1 up to 10,000...
     -- enough to cover VARCHAR(8000)
  WITH E1(N) AS (
                 SELECT 1 UNION ALL SELECT 1 UNION ALL SELECT 1 UNION ALL
                 SELECT 1 UNION ALL SELECT 1 UNION ALL SELECT 1 UNION ALL
                 SELECT 1 UNION ALL SELECT 1 UNION ALL SELECT 1 UNION ALL SELECT 1
                ),                          --10E+1 or 10 rows
       E2(N) AS (SELECT 1 FROM E1 a, E1 b), --10E+2 or 100 rows
       E4(N) AS (SELECT 1 FROM E2 a, E2 b), --10E+4 or 10,000 rows max
 cteTally(N) AS (--==== This provides the "base" CTE and limits the number of rows right up front
                     -- for both a performance gain and prevention of accidental "overruns"
                 SELECT TOP (ISNULL(DATALENGTH(@pString),0)) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) FROM E4
                ),
cteStart(N1) AS (--==== This returns N+1 (starting position of each "element" just once for each delimiter)
                 SELECT 1 UNION ALL
                 SELECT t.N+1 FROM cteTally t WHERE SUBSTRING(@pString,t.N,1) = @pDelimiter
                ),
cteLen(N1,L1) AS(--==== Return start and length (for use in substring)
                 SELECT s.N1,
                        ISNULL(NULLIF(CHARINDEX(@pDelimiter,@pString,s.N1),0)-s.N1,8000)
                   FROM cteStart s
                )
--===== Do the actual split. The ISNULL/NULLIF combo handles the length for the final element when no delimiter is found.
 SELECT ItemNumber = ROW_NUMBER() OVER(ORDER BY l.N1),
        Item       = SUBSTRING(@pString, l.N1, l.L1)
   FROM cteLen l
;
GO

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomersToPaymentMethods
(IdCustomer, IdPaymentMethod)
SELECT 
	c.idcustomer, 
	ISNULL(
		CASE 
		d.Item 
		WHEN '0' THEN 1
		WHEN '1' THEN 2
		WHEN '3' THEN 1
		WHEN '2' THEN 3
		WHEN '4' THEN 4
		WHEN '5' THEN 6
		END, 
		1
	)
FROM customers AS c
CROSS APPLY [dbo].[DelimitedSplit8K](c.approvedPaymentMethods, ',') AS d

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomersToPaymentMethods
(IdCustomer, IdPaymentMethod)
SELECT c.Id, 6
FROM [VitalChoice.Ecommerce].dbo.Customers AS c
WHERE NOT EXISTS (SELECT * FROM [VitalChoice.Ecommerce].dbo.CustomersToPaymentMethods AS ps WHERE ps.IdCustomer = c.Id AND ps.IdPaymentMethod = 6)

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomersToPaymentMethods
(IdCustomer, IdPaymentMethod)
SELECT c.Id, 4
FROM [VitalChoice.Ecommerce].dbo.Customers AS c
WHERE NOT EXISTS (SELECT * FROM [VitalChoice.Ecommerce].dbo.CustomersToPaymentMethods AS ps WHERE ps.IdCustomer = c.Id AND ps.IdPaymentMethod = 4)

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
ADD IdCustomer INT NULL

GO

CREATE NONCLUSTERED INDEX IX_Addresses_IdCustomer ON [VitalChoice.Ecommerce].dbo.Addresses(IdCustomer)

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
ADD IdCreditCard INT NULL

GO

CREATE NONCLUSTERED INDEX IX_Addresses_IdCreditCard ON [VitalChoice.Ecommerce].dbo.Addresses(IdCreditCard)

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.Addresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdCustomer, IdCreditCard)
SELECT 
	ISNULL(pcCust_DateCreated, GETDATE()), 
	ISNULL(lastEditDate, ISNULL(pcCust_DateCreated, GETDATE())),
	ISNULL (
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = ISNULL(c.countryCode, 'US')), 
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode = ISNULL(c.countryCode, 'US') AND s.StateCode = c.stateCode),
	2,
	2,
	c.state,
	c.idcustomer,
	c.custCreditCardId
FROM [vitalchoice2.0].dbo.customerCreditCards AS c
INNER JOIN [vitalchoice2.0].dbo.customers AS cc ON cc.idcustomer = c.idCustomer
PRINT '====addresses (cards)'

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.AddressOptionValues
(IdAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(c.address1 AS NVARCHAR(255)) AS Address1, 
	CAST(c.address2 AS NVARCHAR(255)) AS Address2, 
	CAST(c.firstName AS NVARCHAR(255)) AS FirstName, 
	CAST(c.lastName AS NVARCHAR(255)) AS LastName, 
	CAST(c.company AS NVARCHAR(255)) AS Company, 
	CAST(c.city AS NVARCHAR(255)) AS City, 
	CAST(c.zip AS NVARCHAR(255)) AS Zip,
	CAST(c.phone AS NVARCHAR(255)) AS Phone,
	CAST(c.fax AS NVARCHAR(255)) AS Fax,
	CAST(c.email AS NVARCHAR(255)) AS Email
FROM [vitalchoice2.0].dbo.customerCreditCards AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS cc ON cc.Id = c.idcustomer
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.IdCustomer = c.idcustomer AND a.IdCreditCard = c.custCreditCardId) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax, Email)
)AS unpvt
INNER JOIN [VitalChoice.Ecommerce].dbo.AddressOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====address values (cards)'

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerPaymentMethods
(DateCreated, DateEdited, IdAddress, IdCustomer, IdObjectType, StatusCode)
SELECT c.DateCreated, c.DateEdited, a.Id, c.Id, 1, 2 FROM [VitalChoice.Ecommerce].dbo.Customers AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.IdCustomer = c.Id
PRINT '====payment method links'

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerPaymentMethodValues
(IdCustomerPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	cp.Id, 
	CAST(c.NameOnCard AS NVARCHAR(255)) AS NameOnCard, 
	CAST('XXXXXXXXXXXX' + RIGHT(dbo.RegexReplace('[^0-9]',dbo.RC4Encode(c.CCNumber), ''), 4) AS NVARCHAR(255)) AS CardNumber, 
	CAST(
		(SELECT TOP 1 lv.Id FROM [VitalChoice.Ecommerce].dbo.LookupVariants AS lv WHERE lv.IdLookup = 10 AND lv.ValueVariant =
		CASE 
			dbo.RC4Encode(c.CCType) 
			WHEN 'V' THEN 'Visa'
			WHEN 'M' THEN 'MasterCard'
			WHEN 'D' THEN 'Discover'
			WHEN 'A' THEN 'American Express'
		END)
		AS NVARCHAR(255)
	) AS CardType, 
	CAST(CONVERT(NVARCHAR(250), CONVERT(DATE, dbo.RC4Encode([Expiration]), 101), 126) AS NVARCHAR(255)) AS ExpDate, 
	CAST(CASE WHEN RANK() OVER (PARTITION BY cc.Id ORDER BY c.custCreditCardId) = 1 THEN 'True' ELSE NULL END AS NVARCHAR(255)) AS [Default]
FROM [vitalchoice2.0].dbo.customerCreditCards AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS cc ON cc.Id = c.idcustomer
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.IdCustomer = c.idcustomer AND a.IdCreditCard = c.custCreditCardId
INNER JOIN [VitalChoice.Ecommerce].dbo.CustomerPaymentMethods AS cp ON cp.IdCustomer = c.idCustomer AND cp.IdAddress = a.Id
WHERE ISDATE(dbo.RC4Encode([Expiration])) = 1) p
UNPIVOT (Value FOR Name IN 
	(NameOnCard, CardNumber, CardType, ExpDate, [Default])
)AS unpvt
INNER JOIN [VitalChoice.Ecommerce].dbo.CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 1)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====credit card values'

GO

INSERT INTO [VitalChoice.ExportInfo].dbo.CustomerPaymentMethods
(IdCustomer, IdPaymentMethod, CreditCardNumber)
SELECT c.idCustomer, cp.Id, CAST([vitalchoice2.0].dbo.RC4Encode(c.CCNumber) AS VARBINARY)
FROM [vitalchoice2.0].dbo.customerCreditCards AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS cc ON cc.Id = c.idcustomer
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.IdCustomer = c.idcustomer AND a.IdCreditCard = c.custCreditCardId
INNER JOIN [VitalChoice.Ecommerce].dbo.CustomerPaymentMethods AS cp ON cp.IdCustomer = c.idCustomer AND cp.IdAddress = a.Id
WHERE ISDATE(dbo.RC4Encode([Expiration])) = 1
PRINT '====credit card export data'

GO

DELETE FROM [VitalChoice.Ecommerce].dbo.CustomerPaymentMethodValues
WHERE Value IS NULL OR Value = ''

DELETE FROM [VitalChoice.Ecommerce].dbo.AddressOptionValues
WHERE Value IS NULL OR Value = ''

GO

DROP INDEX IX_Addresses_IdCustomer ON [VitalChoice.Ecommerce].dbo.Addresses
GO
DROP INDEX IX_Addresses_IdCreditCard ON [VitalChoice.Ecommerce].dbo.Addresses

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
DROP COLUMN IdCustomer, IdCreditCard

GO
--============================= Check payment method ==========================

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
ADD IdCustomer INT NULL

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.Addresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdCustomer)
SELECT 
	ISNULL(pcCust_DateCreated, GETDATE()), 
	ISNULL(lastEditDate, ISNULL(pcCust_DateCreated, GETDATE())),
	ISNULL (
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = ISNULL(c.countryCode, 'US')), 
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode = ISNULL(c.countryCode, 'US') AND s.StateCode = c.stateCode),
	2,
	2,
	c.state,
	c.idcustomer
FROM [vitalchoice2.0].dbo.CustomerChecks AS c
INNER JOIN [vitalchoice2.0].dbo.customers AS cc ON cc.idcustomer = c.idCustomer
PRINT '====addresses (checks)'

INSERT INTO [VitalChoice.Ecommerce].dbo.AddressOptionValues
(IdAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(c.address AS NVARCHAR(255)) AS Address1, 
	CAST(c.address2 AS NVARCHAR(255)) AS Address2, 
	CAST(c.firstName AS NVARCHAR(255)) AS FirstName, 
	CAST(c.lastName AS NVARCHAR(255)) AS LastName, 
	CAST(c.company AS NVARCHAR(255)) AS Company, 
	CAST(c.city AS NVARCHAR(255)) AS City, 
	CAST(c.zip AS NVARCHAR(255)) AS Zip,
	CAST(c.phone AS NVARCHAR(255)) AS Phone,
	CAST(c.fax AS NVARCHAR(255)) AS Fax
FROM [vitalchoice2.0].dbo.CustomerChecks AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS cc ON cc.Id = c.idcustomer
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.IdCustomer = c.idcustomer) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax)
)AS unpvt
INNER JOIN [VitalChoice.Ecommerce].dbo.AddressOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
PRINT '====address values (checks)'

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerPaymentMethods
(DateCreated, DateEdited, IdAddress, IdCustomer, IdObjectType, StatusCode)
SELECT c.DateCreated, c.DateEdited, a.Id, c.Id, 3, 2 FROM [VitalChoice.Ecommerce].dbo.Customers AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.IdCustomer = c.Id
PRINT '====check payment linking'

GO

DELETE FROM [VitalChoice.Ecommerce].dbo.AddressOptionValues
WHERE Value IS NULL OR Value = ''

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
DROP COLUMN IdCustomer

GO

--============================= OAC payment method ==========================

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
ADD IdCustomer INT NULL

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.Addresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdCustomer)
SELECT 
	ISNULL(pcCust_DateCreated, GETDATE()), 
	ISNULL(lastEditDate, ISNULL(pcCust_DateCreated, GETDATE())),
	ISNULL (
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = ISNULL(c.countryCode, 'US')), 
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode = ISNULL(c.countryCode, 'US') AND s.StateCode = c.stateCode),
	2,
	2,
	c.state,
	c.idcustomer
FROM [vitalchoice2.0].dbo.CustomerOACs AS c
INNER JOIN [vitalchoice2.0].dbo.customers AS cc ON cc.idcustomer = c.idCustomer
PRINT '====addresses (OAC)'

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.AddressOptionValues
(IdAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(c.address AS NVARCHAR(255)) AS Address1, 
	CAST(c.address2 AS NVARCHAR(255)) AS Address2, 
	CAST(c.firstName AS NVARCHAR(255)) AS FirstName, 
	CAST(c.lastName AS NVARCHAR(255)) AS LastName, 
	CAST(c.company AS NVARCHAR(255)) AS Company, 
	CAST(c.city AS NVARCHAR(255)) AS City, 
	CAST(c.zip AS NVARCHAR(255)) AS Zip,
	CAST(c.phone AS NVARCHAR(255)) AS Phone,
	CAST(c.fax AS NVARCHAR(255)) AS Fax
FROM [vitalchoice2.0].dbo.CustomerOACs AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS cc ON cc.Id = c.idcustomer
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.IdCustomer = c.idcustomer) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax)
)AS unpvt
INNER JOIN [VitalChoice.Ecommerce].dbo.AddressOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
PRINT '====address values (OAC)'

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerPaymentMethods
(DateCreated, DateEdited, IdAddress, IdCustomer, IdObjectType, StatusCode)
SELECT c.DateCreated, c.DateEdited, a.Id, c.Id, 2, 2 FROM [VitalChoice.Ecommerce].dbo.Customers AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.IdCustomer = c.Id
PRINT '====OAC payment linking'

GO

DELETE FROM [VitalChoice.Ecommerce].dbo.AddressOptionValues
WHERE Value IS NULL OR Value = ''

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
DROP COLUMN IdCustomer

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomersToOrderNotes
(IdCustomer, IdOrderNote)
SELECT c.idcustomer, n.Id
FROM customers AS c
CROSS APPLY [dbo].[DelimitedSplit8K](c.orderSpecificNotes, ',') AS d
INNER JOIN [VitalChoice.Ecommerce].dbo.OrderNotes AS n ON n.Title = d.Item
PRINT '====customer to order notes'

GO

USE [VitalChoice.Infrastructure]
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND Name = 'OldPassword')
BEGIN
	ALTER TABLE dbo.AspNetUsers
	ADD OldPassword VARBINARY(250), NotSetPasswords BIT NULL

	CREATE NONCLUSTERED INDEX IX_OldPassword ON dbo.AspNetUsers(OldPassword)
END
GO

IF EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND Name = 'NotSetPasswords')
BEGIN
	UPDATE dbo.AspNetUsers
	SET OldPassword = CAST(c.password AS VARBINARY)
	FROM dbo.AspNetUsers AS u
	INNER JOIN [vitalchoice2.0].dbo.customers AS c ON c.idcustomer = u.Id

	ALTER TABLE dbo.AspNetUsers
	DROP COLUMN NotSetPasswords
END
GO

USE [vitalchoice2.0]
GO
--============================ Insert AspNet Users ====================================

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUsers
WHERE IdUserType = 2 AND Id NOT IN (SELECT idCustomer FROM customers)
PRINT '====remove all new customer users (not exist in old DB)'

GO

SET IDENTITY_INSERT [VitalChoice.Infrastructure].dbo.AspNetUsers ON;

INSERT INTO [VitalChoice.Infrastructure].dbo.AspNetUsers
(Id, 
PublicId, 
Email, 
NormalizedEmail, 
EmailConfirmed, 
UserName, 
NormalizedUserName, 
FirstName, 
LastName,
Status, 
LockoutEnabled, 
PhoneNumber, 
PhoneNumberConfirmed, 
CreateDate, 
UpdatedDate, 
TwoFactorEnabled, 
ConfirmationToken, 
TokenExpirationDate, 
IsConfirmed, 
IdUserType,
AccessFailedCount,
PasswordHash,
SecurityStamp,
ConcurrencyStamp,
OldPassword)
SELECT 
	cc.Id, 
	cc.PublicId, 
	cc.Email, 
	UPPER(cc.Email),  
	1,
	cc.Email,
	UPPER(cc.Email),
	ISNULL(c.name, ''),
	ISNULL(c.lastName, ''),
	1,
	1,
	c.phone,
	0,
	cc.DateCreated,
	cc.DateEdited,
	0,
	NEWID(),
	GETDATE(),
	1,
	2,
	0,
	[vitalchoice2.0].dbo.[HashPassword]([vitalchoice2.0].dbo.RC4Encode(ISNULL(c.password, ''))),
	LOWER(CAST(NEWID() AS NVARCHAR(250))),
	LOWER(CAST(NEWID() AS NVARCHAR(250))),
	CAST(c.password AS VARBINARY)
FROM customers AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS cc ON cc.Id = c.idcustomer
WHERE cc.Id NOT IN (SELECT Id FROM [VitalChoice.Infrastructure].dbo.AspNetUsers)
PRINT '====insert new customers'

GO

UPDATE [VitalChoice.Infrastructure].dbo.AspNetUsers
SET PasswordHash = [vitalchoice2.0].dbo.[HashPassword]([vitalchoice2.0].dbo.RC4Encode(ISNULL(c.password, ''))),
	OldPassword = CAST(c.password AS VARBINARY)
FROM [VitalChoice.Infrastructure].dbo.AspNetUsers AS u
INNER JOIN customers AS c ON c.idcustomer = u.Id
WHERE u.OldPassword IS NULL AND c.password IS NOT NULL OR CONVERT(NVARCHAR(200), CAST(c.password AS VARBINARY), 2) <> CONVERT(NVARCHAR(200), u.OldPassword, 2)
PRINT '====update passwords'

SET IDENTITY_INSERT [VitalChoice.Infrastructure].dbo.AspNetUsers OFF;

INSERT INTO [VitalChoice.Infrastructure].dbo.AspNetUserRoles
(RoleId, UserId)
SELECT CASE WHEN customerType = 0 THEN 6 WHEN customerType = 1 THEN 7 ELSE 6 END, idCustomer
FROM customers
PRINT '====set up roles'

GO

USE [VitalChoice.Ecommerce]

GO

DELETE v
FROM CustomerOptionValues AS v
INNER JOIN CustomerOptionTypes AS t ON t.Id = v.IdOptionType
INNER JOIN Customers AS c ON c.Id = v.IdCustomer
WHERE t.DefaultValue = v.Value AND t.IdObjectType <> c.IdObjectType AND t.IdObjectType IS NOT NULL
PRINT '====remove wrong defaults (normalize)'

GO

INSERT INTO CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT c.Id, t.Id, t.DefaultValue FROM Customers AS c
INNER JOIN CustomerOptionTypes AS t ON t.IdObjectType = c.IdObjectType OR t.IdObjectType IS NULL
WHERE t.DefaultValue IS NOT NULL AND NOT EXISTS(SELECT * FROM CustomerOptionValues AS v WHERE v.IdCustomer = c.Id AND v.IdOptionType = t.Id)
PRINT '====setup default values not set (normalize)'

GO

INSERT INTO AffiliateOptionValues
(IdAffiliate, IdOptionType, Value)
SELECT c.Id, t.Id, t.DefaultValue FROM Affiliates AS c
CROSS JOIN AffiliateOptionTypes AS t
WHERE t.DefaultValue IS NOT NULL AND NOT EXISTS(SELECT * FROM AffiliateOptionValues AS v WHERE v.IdAffiliate = c.Id AND v.IdOptionType = t.Id)
PRINT '====setup Affiliate default values not set (normalize)'

GO

INSERT CustomerOptionValues
(IdCustomer, IdOptionType, Value, IdBigString)
(SELECT IdCustomer,11, '10', null FROM CustomerOptionValues
WHERE Value='empty' AND IdCustomer NOT IN
(SELECT IdCustomer  FROM CustomerOptionValues
WHERE IdOptionType=11))

GO

WITH duplicate_emails_active
AS
(
	SELECT email FROM Customers AS c
	INNER JOIN CustomerOptionTypes AS t ON t.Name = N'Guest'
	LEFT JOIN CustomerOptionValues AS cv ON cv.IdCustomer = c.Id AND cv.IdOptionType = t.Id
	WHERE StatusCode = 2 AND (cv.Value IS NULL OR cv.Value <> N'True')
	GROUP BY email
	HAVING COUNT(*) > 1
)
UPDATE cc
SET Status = 
	CASE WHEN cc.Id = 
		(
			SELECT TOP 1 Id FROM Customers AS c
			INNER JOIN duplicate_emails_active AS d ON d.email = c.email
			WHERE c.email = cc.email
			ORDER BY c.DateEdited DESC, c.DateCreated DESC
		)
		THEN cc.Status
		ELSE 0
	END
FROM [VitalChoice.Infrastructure].dbo.AspNetUsers AS cc
WHERE cc.IdUserType = 2 AND email IN (SELECT email FROM duplicate_emails_active)

GO

WITH duplicate_emails_active
AS
(
	SELECT email FROM Customers AS c
	INNER JOIN CustomerOptionTypes AS t ON t.Name = N'Guest'
	LEFT JOIN CustomerOptionValues AS cv ON cv.IdCustomer = c.Id AND cv.IdOptionType = t.Id
	WHERE StatusCode = 2 AND (cv.Value IS NULL OR cv.Value <> N'True')
	GROUP BY email
	HAVING COUNT(*) > 1
)
UPDATE cc
SET StatusCode = 
	CASE WHEN cc.Id = 
		(
			SELECT TOP 1 Id FROM Customers AS c
			INNER JOIN duplicate_emails_active AS d ON d.email = c.email
			WHERE c.email = cc.email
			ORDER BY c.DateEdited DESC, c.DateCreated DESC
		)
		THEN 2
		ELSE 4
	END
FROM Customers AS cc
WHERE email IN (SELECT email FROM duplicate_emails_active)


GO
USE [vitalchoice2.0]
GO

IF OBJECT_ID('dbo.ReplaceUrl') IS NOT NULL
BEGIN
	DROP FUNCTION dbo.ReplaceUrl
END
GO
CREATE FUNCTION dbo.ReplaceUrl
(@text NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
	RETURN REPLACE(REPLACE(REPLACE(ISNULL(@text, N''), 'http://www.vitalchoice.com/shop/pc/catalog/', '/files/catalog/'), '/shop/pc/catalog/', '/files/catalog/'), '../pc/catalog/', '/files/catalog/')
END
GO

ALTER TABLE [VitalChoice.Infrastructure].dbo.ContentItems
ADD TempId INT NULL,
	TempCategoryId INT NULL
GO

DECLARE @contentType INT, @oldContentType INT, @masterName NVARCHAR(50), @categoryMasterName NVARCHAR(50)
SET @contentType = 1
SET @oldContentType = 1
SET @masterName = N'Recipe Individual'
SET @categoryMasterName = N'Recipe Sub Category'

	--============================= Clean ===========================
	DECLARE @masterId INT
	SELECT @masterId = Id FROM [VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = @masterName

	DECLARE @recipesToImport TABLE (Id INT NOT NULL PRIMARY KEY)

	INSERT INTO @recipesToImport
	(Id)
	SELECT a.RecipeId FROM [vitalchoice2.0].[dbo].Recipes AS a
	WHERE a.RecipeId NOT IN (SELECT IdOld FROM [VitalChoice.Infrastructure].dbo.Recipes WHERE IdOld IS NOT NULL)

	--============================= Recipes ===========================

	INSERT [VitalChoice.Infrastructure].dbo.ContentItems
	(Created, Description, MetaDescription, MetaKeywords, Title, Updated, TempId, Template)
	--OUTPUT inserted.Id, inserted.TempId INTO @insertedArticles
	SELECT GETDATE(), dbo.ReplaceUrl(RecipeDescription), LEFT(MetaDescription, 250), NULL, MetaTitle, GETDATE(), RecipeId, N''
	FROM [vitalchoice2.0].[dbo].Recipes AS a
	WHERE a.RecipeId IN (SELECT Id FROM @recipesToImport)
	ORDER BY a.RecipeId

	INSERT INTO [VitalChoice.Infrastructure].dbo.Recipes
	(AboutChef, ContentItemId, MasterContentItemId, Name, Directions, Ingredients, YoutubeVideo, YoutubeImage, StatusCode, SubTitle, Url, FileUrl, IdOld)
	SELECT a.About, i.Id, @masterId, a.RecipeTitle, a.Directions, a.Ingredients, ISNULL(a.VideoUrl, N''), dbo.ReplaceUrl(a.VideoImage), 2/*Active*/, ISNULL(a.SubTitle, N''), REPLACE(RTRIM(LTRIM(LOWER([vitalchoice2.0].[dbo].RegexReplace('[^a-zA-Z0-9]+', a.RecipeTitle, ' ')))),' ','-'), N'/files/catalog/recipe-images/' + a.RecipeImage, a.RecipeId 
	FROM [vitalchoice2.0].[dbo].Recipes AS a
	INNER JOIN [VitalChoice.Infrastructure].dbo.ContentItems AS i ON i.TempId = a.RecipeId
	WHERE a.RecipeId IN (SELECT Id FROM @recipesToImport)

	DECLARE @subMaster INT
	SELECT @subMaster = Id FROM [VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = @categoryMasterName

	INSERT [VitalChoice.Infrastructure].dbo.RecipesToContentCategories
	(RecipeId, ContentCategoryId)
	SELECT a.Id, c.Id FROM [VitalChoice.Infrastructure].dbo.Recipes AS a
	INNER JOIN [vitalchoice2.0].dbo.categories_recipes AS ca ON ca.idRecipe = a.IdOld
	INNER JOIN [VitalChoice.Infrastructure].dbo.ContentCategories AS c ON c.IdOld = ca.idCategory
	WHERE a.Id IN (SELECT Id FROM @recipesToImport)

	--============================= Recipe Cross Sells ===========================

	INSERT [VitalChoice.Infrastructure].dbo.RecipeCrossSells
	(IdRecipe, Image, Title, Subtitle, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.crosssell1Image), rold.crosssell1Title, rold.crosssell1SubTitle, rold.crosssell1Url, 1 
	FROM [VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.RecipeId IN (SELECT Id FROM @recipesToImport) AND rold.crosssell1Image IS NOT NULL AND rold.crosssell1Image <> ''

	INSERT [VitalChoice.Infrastructure].dbo.RecipeCrossSells
	(IdRecipe, Image, Title, Subtitle, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.crosssell2Image), rold.crosssell2Title, rold.crosssell2SubTitle, rold.crosssell2Url, 2
	FROM [VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.RecipeId IN (SELECT Id FROM @recipesToImport) AND rold.crosssell2Image IS NOT NULL AND rold.crosssell2Image <> ''

	INSERT [VitalChoice.Infrastructure].dbo.RecipeCrossSells
	(IdRecipe, Image, Title, Subtitle, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.crosssell3Image), rold.crosssell3Title, rold.crosssell3SubTitle, rold.crosssell3Url, 3
	FROM [VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.RecipeId IN (SELECT Id FROM @recipesToImport) AND rold.crosssell3Image IS NOT NULL AND rold.crosssell3Image <> ''

	--============================= Related Recipes ===========================

	INSERT [VitalChoice.Infrastructure].dbo.RelatedRecipes
	(IdRecipe, Image, Title, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.relatedrecipe1Image), rold.relatedrecipe1Title, rold.relatedrecipe1Url, 1 
	FROM [VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.RecipeId IN (SELECT Id FROM @recipesToImport) AND rold.relatedrecipe1Image IS NOT NULL AND rold.relatedrecipe1Image <> ''

	INSERT [VitalChoice.Infrastructure].dbo.RelatedRecipes
	(IdRecipe, Image, Title, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.relatedrecipe2Image), rold.relatedrecipe2Title, rold.relatedrecipe2Url, 2
	FROM [VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.RecipeId IN (SELECT Id FROM @recipesToImport) AND rold.relatedrecipe2Image IS NOT NULL AND rold.relatedrecipe2Image <> ''

	INSERT [VitalChoice.Infrastructure].dbo.RelatedRecipes
	(IdRecipe, Image, Title, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.relatedrecipe3Image), rold.relatedrecipe3Title, rold.relatedrecipe3Url, 3
	FROM [VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.RecipeId IN (SELECT Id FROM @recipesToImport) AND rold.relatedrecipe3Image IS NOT NULL AND rold.relatedrecipe3Image <> ''

	INSERT [VitalChoice.Infrastructure].dbo.RelatedRecipes
	(IdRecipe, Image, Title, Url, Number)
	SELECT Id, dbo.ReplaceUrl(rold.relatedrecipe4Image), rold.relatedrecipe4Title, rold.relatedrecipe4Url, 4
	FROM [VitalChoice.Infrastructure].dbo.Recipes AS r
	INNER JOIN [vitalchoice2.0].dbo.Recipes AS rold ON rold.RecipeId = r.IdOld
	WHERE rold.RecipeId IN (SELECT Id FROM @recipesToImport) AND rold.relatedrecipe4Image IS NOT NULL AND rold.relatedrecipe4Image <> ''

GO
ALTER TABLE [VitalChoice.Infrastructure].dbo.ContentItems
DROP COLUMN TempId, COLUMN TempCategoryId
GO
UPDATE [VitalChoice.Infrastructure].dbo.Recipes
SET Url = r.Url + N'-' + CAST(j.Number AS NVARCHAR(10))
FROM [VitalChoice.Infrastructure].dbo.Recipes AS r
INNER JOIN 
(
	SELECT ROW_NUMBER() OVER (PARTITION BY r.Url ORDER BY r.Id) AS Number, r.Id
	FROM [VitalChoice.Infrastructure].dbo.Recipes AS r
	WHERE r.Url IN 
	(
		SELECT Url FROM [VitalChoice.Infrastructure].dbo.Recipes
		GROUP BY Url
		HAVING COUNT(Url) > 1
	)
) AS j ON j.Id = r.Id
WHERE j.Number > 1

GO
IF OBJECT_ID('dbo.ReplaceUrl') IS NOT NULL
	DROP FUNCTION dbo.ReplaceUrl

GO


USE [vitalchoice2.0]
GO

IF EXISTS(SELECT * FROM sys.procedures WHERE name = N'MoveProductTextField')
	DROP PROCEDURE dbo.MoveProductTextField

GO

CREATE PROCEDURE dbo.MoveProductTextField
(@sourceColumnName NVARCHAR(250), @destFieldName NVARCHAR(250), @sourceCondition NVARCHAR(MAX) = NULL)
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX)

	IF @sourceCondition IS NOT NULL

	SET @sql = N'DECLARE @fieldType INT
	DECLARE @textData NVARCHAR(MAX), @IdProduct INT, @idObjectType INT
	DECLARE @bigId TABLE(Id BIGINT NOT NULL)

	DECLARE src CURSOR FOR
	SELECT a.'+@sourceColumnName+N', idProduct, p.IdObjectType FROM products AS a
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = a.idProduct
	WHERE a.idProduct IN (SELECT Id FROM TempProductsToMove) AND ('+@sourceCondition+N') AND a.'+@sourceColumnName+N' IS NOT NULL AND a.'+@sourceColumnName+N' <> N''''

	OPEN src

	FETCH NEXT FROM src
	INTO @textData, @IdProduct, @idObjectType

	WHILE @@FETCH_STATUS = 0
	BEGIN

		IF EXISTS(SELECT * FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'''+@destFieldName+N''' AND (IdObjectType = @idObjectType OR IdObjectType IS NULL))
		BEGIN
			SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'''+@destFieldName+N''' AND (IdObjectType = @idObjectType OR IdObjectType IS NULL)

			INSERT INTO [VitalChoice.Ecommerce].dbo.BigStringValues
			(Value)
			OUTPUT inserted.IdBigString INTO @bigId
			VALUES
			(@textData)

			INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
			(IdProduct, IdOptionType, IdBigString)
			VALUES
			(@IdProduct, @fieldType, (SELECT TOP 1 Id FROM @bigId))

			DELETE FROM @bigId
		END
		ELSE
			SELECT '''+@destFieldName+''', @idObjectType

		FETCH NEXT FROM src
		INTO @textData, @IdProduct, @idObjectType
	END

	CLOSE src;
	DEALLOCATE src;';

	ELSE

	SET @sql = N'DECLARE @fieldType INT
	DECLARE @textData NVARCHAR(MAX), @IdProduct INT, @idObjectType INT
	DECLARE @bigId TABLE(Id BIGINT NOT NULL)

	DECLARE src CURSOR FOR
	SELECT a.'+@sourceColumnName+N', idProduct, p.IdObjectType FROM products AS a
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = a.idProduct
	WHERE a.idProduct IN (SELECT Id FROM TempProductsToMove) AND a.'+@sourceColumnName+N' IS NOT NULL AND a.'+@sourceColumnName+N' <> N''''

	OPEN src

	FETCH NEXT FROM src
	INTO @textData, @IdProduct, @idObjectType

	WHILE @@FETCH_STATUS = 0
	BEGIN

		IF EXISTS(SELECT * FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'''+@destFieldName+N''' AND (IdObjectType = @idObjectType OR IdObjectType IS NULL))
		BEGIN
			SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'''+@destFieldName+N''' AND (IdObjectType = @idObjectType OR IdObjectType IS NULL)

			INSERT INTO [VitalChoice.Ecommerce].dbo.BigStringValues
			(Value)
			OUTPUT inserted.IdBigString INTO @bigId
			VALUES
			(@textData)

			INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
			(IdProduct, IdOptionType, IdBigString)
			VALUES
			(@IdProduct, @fieldType, (SELECT TOP 1 Id FROM @bigId))

			DELETE FROM @bigId
		END
		ELSE
			SELECT '''+@destFieldName+''', @idObjectType

		FETCH NEXT FROM src
		INTO @textData, @IdProduct, @idObjectType
	END

	CLOSE src;
	DEALLOCATE src;';

	EXEC (@sql)
END
GO

IF EXISTS(SELECT * FROM sys.procedures WHERE name = N'MoveProductSmallField')
	DROP PROCEDURE dbo.MoveProductSmallField
GO

CREATE PROCEDURE dbo.MoveProductSmallField
(@destFieldName NVARCHAR(250), @sourceFieldName NVARCHAR(250), @fieldOperation NVARCHAR(MAX) = NULL, @sourceConditions NVARCHAR(MAX) = NULL)
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX)
	
	IF @fieldOperation IS NULL

		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
		(IdOptionType, IdProduct, Value)
		SELECT t.Id, p.Id, a.'+@sourceFieldName+' FROM [VitalChoice.Ecommerce].dbo.Products AS p
		INNER JOIN [vitalchoice2.0].dbo.products AS a ON a.idProduct = p.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'''+@destFieldName+'''
		WHERE p.Id IN (SELECT Id FROM TempProductsToMove) AND a.'+@sourceFieldName+' IS NOT NULL AND ('+ISNULL(@sourceConditions, '1=1')+')';

	ELSE
		
		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
		(IdOptionType, IdProduct, Value)
		SELECT t.Id, p.Id, '+@fieldOperation+' FROM [VitalChoice.Ecommerce].dbo.Products AS p
		INNER JOIN [vitalchoice2.0].dbo.products AS a ON a.idProduct = p.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'''+@destFieldName+'''
		WHERE p.Id IN (SELECT Id FROM TempProductsToMove) AND a.'+@sourceFieldName+' IS NOT NULL AND ('+ISNULL(@sourceConditions, '1=1')+')';

	EXEC(@sql)
END

GO

IF EXISTS(SELECT * FROM sys.procedures WHERE name = N'MoveSkuField')
	DROP PROCEDURE dbo.MoveSkuField
GO

CREATE PROCEDURE dbo.MoveSkuField
(@destFieldName NVARCHAR(250), @sourceFieldName NVARCHAR(250), @conversion NVARCHAR(MAX) = NULL, @sourceConditions NVARCHAR(MAX) = NULL)
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX)
	
	IF @conversion IS NULL

		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.SkuOptionValues
		(IdOptionType, IdSku, Value)
		SELECT t.Id, s.Id, a.'+@sourceFieldName+' FROM [VitalChoice.Ecommerce].dbo.Skus AS s
		INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = s.IdProduct
		INNER JOIN [vitalchoice2.0].dbo.products AS a ON a.idProduct = s.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.SkuOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'''+@destFieldName+'''
		WHERE p.Id IN (SELECT Id FROM TempProductsToMove) AND a.'+@sourceFieldName+' IS NOT NULL AND ('+ISNULL(@sourceConditions, '1=1')+')';

	ELSE
		
		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.SkuOptionValues
		(IdOptionType, IdSku, Value)
		SELECT t.Id, s.Id, '+@conversion+' FROM [VitalChoice.Ecommerce].dbo.Skus AS s
		INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = s.IdProduct
		INNER JOIN [vitalchoice2.0].dbo.products AS a ON a.idProduct = s.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.SkuOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'''+@destFieldName+'''
		WHERE p.Id IN (SELECT Id FROM TempProductsToMove) AND a.'+@sourceFieldName+' IS NOT NULL AND ('+ISNULL(@sourceConditions, '1=1')+')';

	EXEC(@sql)
END

GO

--ALTER TABLE [VitalChoice.Infrastructure].dbo.ContentItems
--	DROP COLUMN TempId, COLUMN TempCategoryId

GO

ALTER TABLE [VitalChoice.Infrastructure].dbo.ContentItems
	ADD TempId INT NULL,
		TempCategoryId INT NULL

GO

USE [vitalchoice2.0]
GO

DECLARE @contentType INT, @oldContentType INT, @masterName NVARCHAR(50), @categoryMasterName NVARCHAR(50)
SET @contentType = 9
SET @oldContentType = 0
SET @categoryMasterName = N'Product sub categories'

DECLARE @productCategoryMaster INT
SELECT @productCategoryMaster = Id FROM [VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = @categoryMasterName

INSERT [VitalChoice.Infrastructure].dbo.ContentItems
(Created, Description, MetaDescription, MetaKeywords, Title, Updated, TempCategoryId, Template)
--OUTPUT inserted.Id, inserted.TempCategoryId INTO @insertedArticleCategories
SELECT ISNULL(ca.pcCats_EditedDate, GETDATE()), N'', LEFT(CAST(ca.pcCats_MetaDesc AS NVARCHAR(MAX)), 250), ca.pcCats_MetaKeywords, ca.pcCats_MetaTitle, GETDATE(), ca.idCategory, N''
FROM [vitalchoice2.0].dbo.categories AS ca 
WHERE ca.type=@oldContentType AND ca.idCategory <> 1 AND ca.idCategory NOT IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.ProductCategories)
ORDER BY ca.idParentCategory

INSERT [VitalChoice.Infrastructure].dbo.ProductCategories
(Id, ContentItemId, MasterContentItemId, IdOld, NavLabel, StatusCode, Url, NavIdVisible, FileImageLargeUrl, FileImageSmallUrl, HideLongDescription, HideLongDescriptionBottom, LongDescription, LongDescriptionBottom)
SELECT ca.idCategory, c.Id, @productCategoryMaster, ca.idCategory, REPLACE(ca.categoryDesc, '&amp;', '&'),  
	2/*Active*/, REPLACE(RTRIM(LTRIM(LOWER([vitalchoice2.0].[dbo].RegexReplace('[^a-zA-Z0-9]+', ca.categoryDesc, ' ')))),' ','-'),
	CASE WHEN ISNULL(ca.iBTOhide, 0) <> 0 THEN NULL ELSE CASE WHEN ISNULL(ca.pcCats_RetailHide, 0) <> 0 THEN 2 ELSE 1 END END,
	REPLACE(N'/files/catalog/' + ca.largeimage, '//', '/'), REPLACE(N'/files/catalog/' + ca.image, '//', '/'), CASE WHEN ISNULL(ca.HideDesc, 0) <> 0 THEN 1 ELSE 0 END, CASE WHEN ISNULL(ca.HideDesc, 0) <> 0 THEN 1 ELSE 0 END, ca.LDesc, ca.LDesc2
FROM [vitalchoice2.0].dbo.categories AS ca
INNER JOIN [VitalChoice.Infrastructure].dbo.ContentItems AS c ON c.TempCategoryId = ca.idCategory
WHERE ca.type=@oldContentType AND ca.idCategory <> 1 AND ca.idCategory NOT IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.ProductCategories)
ORDER BY ca.idParentCategory

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.ProductCategories ON;

INSERT [VitalChoice.Ecommerce].dbo.ProductCategories
(Id, Name, StatusCode, [Order])
SELECT ca.idCategory, REPLACE(ca.categoryDesc, '&amp;', '&'), 2/*Active*/, ISNULL(ca.[priority], 0)
FROM [vitalchoice2.0].dbo.categories AS ca
WHERE ca.type=@oldContentType AND ca.idCategory <> 1 AND ca.idCategory NOT IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.ProductCategories)
ORDER BY ca.idParentCategory

GO

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.ProductCategories OFF;

GO

IF OBJECT_ID('dbo.TempProductsToMove') IS NOT NULL
	DROP TABLE dbo.TempProductsToMove

GO

CREATE TABLE TempProductsToMove
(Id INT PRIMARY KEY)

GO

	DECLARE @contentType INT, @oldContentType INT, @masterName NVARCHAR(50), @categoryMasterName NVARCHAR(50)
	SET @contentType = 9
	SET @oldContentType = 0
	SET @masterName = N'Product page'
	SET @categoryMasterName = N'Product sub categories'

	DECLARE @articleMasterId INT
	SELECT @articleMasterId = Id FROM [VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = @masterName

	INSERT INTO TempProductsToMove
	(Id)
	SELECT idProduct FROM products WHERE idProduct NOT IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.Products) AND pcprod_ParentPrd = 0

	INSERT [VitalChoice.Infrastructure].dbo.ContentItems
	(Created, Description, MetaDescription, MetaKeywords, Title, Updated, TempId, Template)
	--OUTPUT inserted.Id, inserted.TempId INTO @insertedArticles
	SELECT ISNULL(a.pcProd_EditedDate, GETDATE()), ISNULL(a.details, N''), LEFT(a.pcProd_MetaDesc, 250), a.pcProd_MetaKeywords, a.pcProd_MetaTitle, ISNULL(a.pcProd_EditedDate, GETDATE()), a.idProduct, N''
	FROM [vitalchoice2.0].[dbo].Products AS a
	WHERE a.pcprod_ParentPrd = 0 AND a.idProduct IN (SELECT Id FROM TempProductsToMove)
	ORDER BY a.idProduct

	INSERT INTO [VitalChoice.Infrastructure].dbo.Products
	(Id, ContentItemId, MasterContentItemId, StatusCode, Url, IdOld)
	SELECT a.idProduct, i.Id, @articleMasterId, CASE WHEN ISNULL(a.removed, 0) <> 0 THEN 3 ELSE CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END END, REPLACE(RTRIM(LTRIM(LOWER([vitalchoice2.0].[dbo].RegexReplace('[^a-zA-Z0-9]+', a.description, ' ')))),' ','-'), a.idProduct
	FROM [vitalchoice2.0].[dbo].Products AS a
	INNER JOIN [VitalChoice.Infrastructure].dbo.ContentItems AS i ON i.TempId = a.idProduct
	WHERE a.pcprod_ParentPrd = 0 AND a.idProduct IN (SELECT Id FROM TempProductsToMove)

	SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Products ON;
	
	--================== Non-Perishable =====================

	INSERT INTO [VitalChoice.Ecommerce].dbo.Products
	(Id, DateCreated, DateEdited, Hidden, IdObjectType, Name, PublicId, StatusCode)
	SELECT 
	a.idProduct, ISNULL(a.pcProd_EditedDate, GETDATE()), ISNULL(a.pcProd_EditedDate, GETDATE()), a.hidden, 1, a.description, NEWID(), CASE WHEN ISNULL(a.removed, 0) <> 0 THEN 3 ELSE CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END END
	FROM [vitalchoice2.0].[dbo].Products AS a
	WHERE a.pcprod_ParentPrd = 0 AND ISNULL(a.pcprod_GC, 0) = 0 AND ISNULL(a.perishable, 0) = 0 AND a.idProduct IN (SELECT Id FROM TempProductsToMove)

	--================== Perishable =========================

	INSERT INTO [VitalChoice.Ecommerce].dbo.Products
	(Id, DateCreated, DateEdited, Hidden, IdObjectType, Name, PublicId, StatusCode)
	SELECT 
	a.idProduct, ISNULL(a.pcProd_EditedDate, GETDATE()), ISNULL(a.pcProd_EditedDate, GETDATE()), a.hidden, 2, a.description, NEWID(), CASE WHEN ISNULL(a.removed, 0) <> 0 THEN 3 ELSE CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END END
	FROM [vitalchoice2.0].[dbo].Products AS a
	WHERE a.pcprod_ParentPrd = 0 AND ISNULL(a.pcprod_GC, 0) = 0 AND ISNULL(a.perishable, 0) = -1 AND a.idProduct IN (SELECT Id FROM TempProductsToMove)

	--================== Gift Certificates =========================

	INSERT INTO [VitalChoice.Ecommerce].dbo.Products
	(Id, DateCreated, DateEdited, Hidden, IdObjectType, Name, PublicId, StatusCode)
	SELECT 
	a.idProduct, ISNULL(a.pcProd_EditedDate, GETDATE()), ISNULL(a.pcProd_EditedDate, GETDATE()), a.hidden, 3, a.description, NEWID(), CASE WHEN ISNULL(a.removed, 0) <> 0 THEN 3 ELSE CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END END
	FROM [vitalchoice2.0].[dbo].Products AS a
	WHERE a.pcprod_ParentPrd = 0 AND ISNULL(a.pcprod_GC, 0) = 1 AND a.sku LIKE 'EGIFT%' AND a.idProduct IN (SELECT Id FROM TempProductsToMove)

	INSERT INTO [VitalChoice.Ecommerce].dbo.Products
	(Id, DateCreated, DateEdited, Hidden, IdObjectType, Name, PublicId, StatusCode)
	SELECT 
	a.idProduct, ISNULL(a.pcProd_EditedDate, GETDATE()), ISNULL(a.pcProd_EditedDate, GETDATE()), a.hidden, 4, a.description, NEWID(), CASE WHEN ISNULL(a.removed, 0) <> 0 THEN 3 ELSE CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END END
	FROM [vitalchoice2.0].[dbo].Products AS a
	WHERE a.pcprod_ParentPrd = 0 AND ISNULL(a.pcprod_GC, 0) = 1 AND a.sku NOT LIKE 'EGIFT%' AND a.idProduct IN (SELECT Id FROM TempProductsToMove)

	SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Products OFF;

-- Move fields

	EXEC dbo.MoveProductTextField @sourceColumnName = N'details', @destFieldName = N'Description'
	EXEC dbo.MoveProductTextField @sourceColumnName = N'ingredients', @destFieldName = N'Ingredients'
	EXEC dbo.MoveProductTextField @sourceColumnName = N'pcProd_PrdNotes', @destFieldName = N'ProductNotes'
	EXEC dbo.MoveProductTextField @sourceColumnName = N'recipes', @destFieldName = N'Recipes'
	EXEC dbo.MoveProductTextField @sourceColumnName = N'serving', @destFieldName = N'Serving'
	EXEC dbo.MoveProductTextField @sourceColumnName = N'sDesc', @destFieldName = N'ShortDescription'

	INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
	(IdOptionType, IdProduct, Value)
	SELECT t.Id, p.Id, data.Item FROM [VitalChoice.Ecommerce].dbo.Products AS p
	INNER JOIN (
		SELECT 
			CASE d.ItemNumber 
				WHEN 1 THEN N'NutritionalTitle'
				WHEN 2 THEN N'ServingSize'
				WHEN 3 THEN N'Servings'
				WHEN 4 THEN N'Calories'
				WHEN 5 THEN N'CaloriesFromFat'
				WHEN 6 THEN N'TotalFat'
				WHEN 7 THEN N'TotalFatPercent'
				WHEN 8 THEN N'SaturatedFat'
				WHEN 9 THEN N'SaturatedFatPercent'
				WHEN 10 THEN N'TransFat'
				WHEN 11 THEN N'TransFatPercent'
				WHEN 12 THEN N'Cholesterol'
				WHEN 13 THEN N'CholesterolPercent'
				WHEN 14 THEN N'Sodium'
				WHEN 15 THEN N'SodiumPercent'
				WHEN 16 THEN N'TotalCarbohydrate'
				WHEN 17 THEN N'TotalCarbohydratePercent'
				WHEN 18 THEN N'DietaryFiber'
				WHEN 19 THEN N'DietaryFiberPercent'
				WHEN 20 THEN N'Sugars'
				WHEN 21 THEN N'SugarsPercent'
				WHEN 22 THEN N'Protein'
				WHEN 23 THEN N'ProteinPercent'
				WHEN 24 THEN N'AdditionalNotes'
			ELSE
				'INVALID'
			END AS FieldName, 
			d.Item, 
			p.idProduct 
		FROM products AS p
		CROSS APPLY [dbo].[DelimitedSplit8K](p.nutritionInfo, '|~|') AS d
		WHERE p.removed = 0 AND p.pcprod_ParentPrd = 0 AND p.nutritionInfo IS NOT NULL AND p.nutritionInfo <> ''
	) AS data ON data.idProduct = p.Id
	INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON t.Name = data.FieldName AND (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL)
	WHERE p.Id IN (SELECT Id FROM TempProductsToMove) AND LEN(data.Item) <= 250

--======================== Additional Long Notes (Nutrition) =================================

	DECLARE @fieldType INT
	DECLARE @textData NVARCHAR(MAX), @IdProduct INT, @idObjectType INT

	DECLARE src CURSOR FOR
	SELECT data.Item, p.Id, p.IdObjectType FROM [VitalChoice.Ecommerce].dbo.Products AS p
	INNER JOIN (
		SELECT 
			CASE d.ItemNumber 
				WHEN 24 THEN N'AdditionalNotes'
			ELSE
				'INVALID'
			END AS FieldName, 
			d.Item, 
			p.idProduct 
		FROM products AS p
		CROSS APPLY [dbo].[DelimitedSplit8K](p.nutritionInfo, '|~|') AS d
		WHERE p.removed = 0 AND p.pcprod_ParentPrd = 0 AND p.nutritionInfo IS NOT NULL AND p.nutritionInfo <> ''
	) AS data ON data.idProduct = p.Id
	INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON t.Name = data.FieldName AND (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL)
	WHERE p.Id IN (SELECT Id FROM TempProductsToMove) AND LEN(data.Item) > 250 AND data.FieldName = N'AdditionalNotes'

	OPEN src

	FETCH NEXT FROM src
	INTO @textData, @IdProduct, @idObjectType

	WHILE @@FETCH_STATUS = 0
	BEGIN

		IF EXISTS(SELECT * FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'AdditionalNotes' AND IdObjectType = @idObjectType)
		BEGIN
			SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.ProductOptionTypes WHERE Name = N'AdditionalNotes' AND (IdObjectType = @idObjectType OR IdObjectType IS NULL)

			INSERT INTO [VitalChoice.Ecommerce].dbo.BigStringValues
			(Value)
			VALUES
			(@textData)

			INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
			(IdProduct, IdOptionType, IdBigString)
			VALUES
			(@IdProduct, @fieldType, SCOPE_IDENTITY())
		END

		FETCH NEXT FROM src
		INTO @textData, @IdProduct, @idObjectType
	END

	CLOSE src;
	DEALLOCATE src;

--======================= Google Category =============================
	DECLARE @googleCategoriesLookup INT

	SELECT TOP 1 @googleCategoriesLookup = Id FROM [VitalChoice.Ecommerce].dbo.Lookups AS l WHERE l.Name = 'GoogleCategories'

	INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
	(IdOptionType, IdProduct, Value)
	SELECT t.Id, p.Id, CAST(l.Id AS NVARCHAR(20)) FROM [VitalChoice.Ecommerce].dbo.Products AS p
	INNER JOIN [vitalchoice2.0].dbo.products AS a ON a.idProduct = p.Id
	INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'GoogleCategory'
	INNER JOIN [VitalChoice.Ecommerce].dbo.LookupVariants AS l ON l.IdLookup = t.IdLookup AND l.ValueVariant = a.google_category
	WHERE p.Id IN (SELECT Id FROM TempProductsToMove)

	INSERT [VitalChoice.Ecommerce].dbo.ProductsToCategories
	(IdProduct, IdCategory, [Order])
	SELECT a.Id, c.Id, ca.POrder FROM [VitalChoice.Infrastructure].dbo.Products AS a
	INNER JOIN [vitalchoice2.0].dbo.categories_products AS ca ON ca.idProduct = a.Id AND ca.idCategory <> 1
	INNER JOIN [VitalChoice.Infrastructure].dbo.ProductCategories AS c ON c.IdOld = ca.idCategory
	--WHERE a.Id IN (SELECT Id FROM TempProductsToMove)
	
	UPDATE [VitalChoice.Infrastructure].dbo.Products
	SET Url = r.Url + N'-' + CAST(j.Number AS NVARCHAR(10))
	FROM [VitalChoice.Infrastructure].dbo.Products AS r
	INNER JOIN 
	(
		SELECT ROW_NUMBER() OVER (PARTITION BY r.Url ORDER BY r.Id) AS Number, r.Id
		FROM [VitalChoice.Infrastructure].dbo.Products AS r
		WHERE r.Url IN 
		(
			SELECT Url FROM [VitalChoice.Infrastructure].dbo.Products
			GROUP BY Url
			HAVING COUNT(Url) > 1
		)
	) AS j ON j.Id = r.Id
	WHERE j.Number > 1

	UPDATE [VitalChoice.Infrastructure].dbo.ProductCategories
	SET Url = r.Url + N'-' + CAST(j.Number AS NVARCHAR(10))
	FROM [VitalChoice.Infrastructure].dbo.ProductCategories AS r
	INNER JOIN 
	(
		SELECT ROW_NUMBER() OVER (PARTITION BY r.Url ORDER BY r.Id) AS Number, r.Id
		FROM [VitalChoice.Infrastructure].dbo.ProductCategories AS r
		WHERE r.Url IN 
		(
			SELECT Url FROM [VitalChoice.Infrastructure].dbo.ProductCategories
			GROUP BY Url
			HAVING COUNT(Url) > 1
		)
	) AS j ON j.Id = r.Id
	WHERE j.Number > 1

	EXEC dbo.MoveProductSmallField @destFieldName = N'SubProductGroupName', @sourceFieldName = N'idProduct', @fieldOperation = N'(SELECT ogi.OptionGroupDesc FROM [vitalchoice2.0].[dbo].[pcProductsOptions] AS po INNER JOIN [vitalchoice2.0].[dbo].optionsGroups AS ogi ON ogi.idOptionGroup = po.idOptionGroup WHERE po.idProduct = a.idProduct)'
	EXEC dbo.MoveProductSmallField @destFieldName = N'SpecialIcon', @sourceFieldName = N'mscicon', @fieldOperation = N'CAST(a.mscicon AS NVARCHAR(250))'
	EXEC dbo.MoveProductSmallField @destFieldName = N'TaxCode', @sourceFieldName = N'TaxCode'
	EXEC dbo.MoveProductSmallField @destFieldName = N'Thumbnail', @sourceFieldName = N'smallImageUrl', @fieldOperation = N'REPLACE(''/files/catalog/'' + a.smallImageUrl, ''//'', ''/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'MainProductImage', @sourceFieldName = N'imageUrl', @fieldOperation = N'REPLACE(''/files/catalog/'' + a.imageUrl, ''//'', ''/'')'
	
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellImage1', @sourceFieldName = N'crossSellImg1', @fieldOperation = N'REPLACE(a.crossSellImg1, ''/shop/pc/catalog/'',''/files/catalog/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellImage2', @sourceFieldName = N'crossSellImg2', @fieldOperation = N'REPLACE(a.crossSellImg2, ''/shop/pc/catalog/'',''/files/catalog/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellImage3', @sourceFieldName = N'crossSellImg3', @fieldOperation = N'REPLACE(a.crossSellImg3, ''/shop/pc/catalog/'',''/files/catalog/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellImage4', @sourceFieldName = N'crossSellImg4', @fieldOperation = N'REPLACE(a.crossSellImg4, ''/shop/pc/catalog/'',''/files/catalog/'')'
	
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl1', @sourceFieldName = N'crossSellUrl1', @fieldOperation = N'''/product/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.Products AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewPrd.asp\?idproduct=([0-9]+).*'', a.crossSellUrl1, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl1 LIKE N''%viewPrd.asp%'''
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl2', @sourceFieldName = N'crossSellUrl2', @fieldOperation = N'''/product/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.Products AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewPrd.asp\?idproduct=([0-9]+).*'', a.crossSellUrl2, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl2 LIKE N''%viewPrd.asp%'''
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl3', @sourceFieldName = N'crossSellUrl3', @fieldOperation = N'''/product/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.Products AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewPrd.asp\?idproduct=([0-9]+).*'', a.crossSellUrl3, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl3 LIKE N''%viewPrd.asp%'''
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl4', @sourceFieldName = N'crossSellUrl4', @fieldOperation = N'''/product/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.Products AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewPrd.asp\?idproduct=([0-9]+).*'', a.crossSellUrl4, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl4 LIKE N''%viewPrd.asp%'''
	
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl1', @sourceFieldName = N'crossSellUrl1', @fieldOperation = N'''/category/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.ProductCategories AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewcategories.asp\?idcategory=([0-9]+).*'', a.crossSellUrl1, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl1 LIKE N''%viewcategories.asp%'''
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl2', @sourceFieldName = N'crossSellUrl2', @fieldOperation = N'''/category/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.ProductCategories AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewcategories.asp\?idcategory=([0-9]+).*'', a.crossSellUrl2, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl2 LIKE N''%viewcategories.asp%'''
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl3', @sourceFieldName = N'crossSellUrl3', @fieldOperation = N'''/category/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.ProductCategories AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewcategories.asp\?idcategory=([0-9]+).*'', a.crossSellUrl3, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl3 LIKE N''%viewcategories.asp%'''
	EXEC dbo.MoveProductSmallField @destFieldName = N'CrossSellUrl4', @sourceFieldName = N'crossSellUrl4', @fieldOperation = N'''/category/'' + (SELECT Url FROM [VitalChoice.Infrastructure].dbo.ProductCategories AS p WHERE p.Id = CAST([vitalchoice2.0].dbo.RegexReplace(''.*shop/pc/viewcategories.asp\?idcategory=([0-9]+).*'', a.crossSellUrl4, ''$1'') AS INT))', @sourceConditions = 'a.crossSellUrl4 LIKE N''%viewcategories.asp%'''

	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeImage1', @sourceFieldName = N'videoImage1', @fieldOperation = N'REPLACE(a.videoImage1, ''/shop/pc/catalog/'',''/files/catalog/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeImage2', @sourceFieldName = N'videoImage2', @fieldOperation = N'REPLACE(a.videoImage2, ''/shop/pc/catalog/'',''/files/catalog/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeImage3', @sourceFieldName = N'videoImage3', @fieldOperation = N'REPLACE(a.videoImage3, ''/shop/pc/catalog/'',''/files/catalog/'')'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeText1', @sourceFieldName = N'text1'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeText2', @sourceFieldName = N'text2'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeText3', @sourceFieldName = N'text3'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeVideo1', @sourceFieldName = N'video1'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeVideo2', @sourceFieldName = N'video2'
	EXEC dbo.MoveProductSmallField @destFieldName = N'YouTubeVideo3', @sourceFieldName = N'video3'

	DELETE FROM [VitalChoice.Ecommerce].dbo.ProductOptionValues
	WHERE (Value IS NULL OR Value = N'') AND IdBigString IS NULL

	DECLARE @additionalSkusToImport TABLE (Id INT NOT NULL PRIMARY KEY)

	INSERT INTO @additionalSkusToImport
	(Id)
	SELECT DISTINCT IdProduct FROM (
	SELECT IdProduct FROM products AS a
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = a.pcprod_ParentPrd
	WHERE idProduct NOT IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.Skus)
	UNION ALL
	SELECT IdProduct FROM products AS a
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = a.idProduct AND ISNULL(a.pcprod_Apparel, 0) = 0 AND a.pcprod_ParentPrd = 0
	WHERE idProduct NOT IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.Skus)
	) f

	SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Skus ON;

	INSERT INTO [VitalChoice.Ecommerce].dbo.Skus
	(Id, Code, DateCreated, DateEdited, Hidden, IdProduct, [Order], Price, WholesalePrice, StatusCode)
	SELECT 
		a.idProduct, 
		a.sku, 
		ISNULL(a.pcProd_EditedDate, GETDATE()), 
		ISNULL(a.pcProd_EditedDate, GETDATE()), 
		ISNULL(a.hidden, 0), 
		p.Id, 
		ROW_NUMBER() OVER (ORDER BY a.idProduct), 
		a.price, 
		a.bToBPrice,
		CASE WHEN ISNULL(a.removed, 0) <> 0 THEN 3 ELSE CASE WHEN ISNULL(a.pcProd_SPInActive, 0) = 0 THEN 2 ELSE 1 END END
	FROM [vitalchoice2.0].dbo.products AS a
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = a.pcprod_ParentPrd
	WHERE (p.Id IN (SELECT Id FROM TempProductsToMove) OR a.idProduct IN (SELECT Id FROM @additionalSkusToImport))

	INSERT INTO [VitalChoice.Ecommerce].dbo.Skus
	(Id, Code, DateCreated, DateEdited, Hidden, IdProduct, [Order], Price, WholesalePrice, StatusCode)
	SELECT 
		a.idProduct, 
		LEFT(a.sku, 20), 
		ISNULL(a.pcProd_EditedDate, GETDATE()), 
		ISNULL(a.pcProd_EditedDate, GETDATE()), 
		ISNULL(a.hidden, 0), 
		p.Id, 
		ROW_NUMBER() OVER (ORDER BY a.idProduct), 
		a.price, 
		a.bToBPrice,
		p.StatusCode
	FROM [vitalchoice2.0].dbo.products AS a
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = a.idProduct AND ISNULL(a.pcprod_Apparel, 0) = 0 AND a.pcprod_ParentPrd = 0
	WHERE p.Id IN (SELECT Id FROM TempProductsToMove) OR a.idProduct IN (SELECT Id FROM @additionalSkusToImport)

	SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Skus OFF;

	INSERT INTO [VitalChoice.Ecommerce].dbo.SkuOptionValues
	(IdOptionType, IdSku, Value)
	SELECT t.Id, pp.idProduct,
	CAST(CASE
		WHEN dbo.IsMatch('^([0-9]+)\s*([dD]ozen|[dD]z)$', o.optionDescrip) = 1 
		THEN CAST(dbo.RegexReplace('^([0-9]+)\s*([dD]ozen|[dD]z)$', o.optionDescrip, '$1') AS INT) * 12
		WHEN dbo.IsMatch('^([0-9]+)-[0-9]+$', o.optionDescrip) = 1
		THEN CAST(dbo.RegexReplace('^([0-9]+)-[0-9]+$', o.optionDescrip, '$1') AS INT)
		ELSE
		CAST(CAST(dbo.RegexReplace('[^0-9\.]', o.optionDescrip, '') AS FLOAT) AS INT) END AS NVARCHAR(250))
	FROM products AS p
	INNER JOIN [vitalchoice2.0].[dbo].[pcProductsOptions] AS po ON po.idProduct = p.idProduct
	INNER JOIN options_optionsGroups AS og ON og.idOptionGroup = po.idOptionGroup AND og.idProduct = p.idProduct
	INNER JOIN options AS o ON o.idOption = og.idOption
	INNER JOIN products AS pp ON (pp.pcProd_Relationship like '%[_]'+ CAST(og.idoptoptgrp AS NVARCHAR(10)) OR (pp.pcProd_Relationship like '%[_]'+CAST(og.idoptoptgrp AS NVARCHAR(10))+'[_]%'))
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS ep ON ep.Id = p.idProduct
	INNER JOIN [VitalChoice.Ecommerce].dbo.SkuOptionTypes AS t ON (t.IdObjectType = ep.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'QTY'
	WHERE ep.Id IN (SELECT Id FROM TempProductsToMove) OR pp.idProduct IN (SELECT Id FROM @additionalSkusToImport)

	DELETE v
	FROM [VitalChoice.Ecommerce].dbo.SkuOptionValues AS v
	INNER JOIN [VitalChoice.Ecommerce].dbo.SkuOptionTypes AS t ON t.Id = v.IdOptionType
	WHERE t.Name IN (N'DisregardStock', N'Stock')

	INSERT INTO [VitalChoice.Ecommerce].dbo.SkuOptionValues
	(IdOptionType, IdSku, Value)
	SELECT t.Id, s.Id, CASE WHEN ISNULL(a.nostock, 0) <> 0 THEN N'True' ELSE N'False' END 
	FROM [VitalChoice.Ecommerce].dbo.Skus AS s
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = s.IdProduct
	INNER JOIN [vitalchoice2.0].dbo.products AS a ON a.idProduct = s.Id
	INNER JOIN [VitalChoice.Ecommerce].dbo.SkuOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'DisregardStock'

	INSERT INTO [VitalChoice.Ecommerce].dbo.SkuOptionValues
	(IdOptionType, IdSku, Value)
	SELECT t.Id, s.Id, CAST(ISNULL(a.stock, 0) AS NVARCHAR(250)) 
	FROM [VitalChoice.Ecommerce].dbo.Skus AS s
	INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = s.IdProduct
	INNER JOIN [vitalchoice2.0].dbo.products AS a ON a.idProduct = s.Id
	INNER JOIN [VitalChoice.Ecommerce].dbo.SkuOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'Stock'

	--EXEC dbo.MoveSkuField @destFieldName = N'DisregardStock', @sourceFieldName = N'nostock', @conversion = N'CASE WHEN ISNULL(a.nostock, 0) <> 0 THEN ''True'' ELSE ''False'' END'
	--EXEC dbo.MoveSkuField @destFieldName = N'Stock', @sourceFieldName = N'stock', @conversion = N'CAST(ISNULL(a.stock, 0) AS NVARCHAR(250))'
	EXEC dbo.MoveSkuField @destFieldName = N'DisallowSingle', @sourceFieldName = N'disallowSingle', @conversion = N'CASE WHEN ISNULL(a.disallowSingle, 0) <> 0 THEN ''True'' ELSE ''False'' END'
	EXEC dbo.MoveSkuField @destFieldName = N'NonDiscountable', @sourceFieldName = N'NonDiscountable', @conversion = N'CASE WHEN ISNULL(a.NonDiscountable, 0) <> 0 THEN ''True'' ELSE ''False'' END'
	EXEC dbo.MoveSkuField @destFieldName = N'OrphanType', @sourceFieldName = N'OrphanType', @conversion = N'CASE WHEN ISNULL(a.OrphanType, 0) <> 0 THEN ''True'' ELSE ''False'' END'
	EXEC dbo.MoveSkuField @destFieldName = N'QTYThreshold', @sourceFieldName = N'QTYThreshold', @conversion = N'CASE WHEN ISNULL(a.OrphanType, 0) <> 0 THEN a.QTYThreshold ELSE NULL END'
	EXEC dbo.MoveSkuField @destFieldName = N'AutoShipProduct', @sourceFieldName = N'autoShip', @conversion = N'CASE WHEN ISNULL(a.autoShip, 0) <> 0 THEN ''True'' ELSE ''False'' END'
	EXEC dbo.MoveSkuField @destFieldName = N'AutoShipFrequency1', @sourceFieldName = N'Schedules', @conversion = N'CASE WHEN ISNULL(a.autoShip, 0) <> 0 THEN CASE WHEN a.Schedules LIKE ''%1%'' THEN ''True'' ELSE ''False'' END ELSE NULL END'
	EXEC dbo.MoveSkuField @destFieldName = N'AutoShipFrequency2', @sourceFieldName = N'Schedules', @conversion = N'CASE WHEN ISNULL(a.autoShip, 0) <> 0 THEN CASE WHEN a.Schedules LIKE ''%2%'' THEN ''True'' ELSE ''False'' END ELSE NULL END'
	EXEC dbo.MoveSkuField @destFieldName = N'AutoShipFrequency3', @sourceFieldName = N'Schedules', @conversion = N'CASE WHEN ISNULL(a.autoShip, 0) <> 0 THEN CASE WHEN a.Schedules LIKE ''%3%'' THEN ''True'' ELSE ''False'' END ELSE NULL END'
	EXEC dbo.MoveSkuField @destFieldName = N'AutoShipFrequency6', @sourceFieldName = N'Schedules', @conversion = N'CASE WHEN ISNULL(a.autoShip, 0) <> 0 THEN CASE WHEN a.Schedules LIKE ''%6%'' THEN ''True'' ELSE ''False'' END ELSE NULL END'
	EXEC dbo.MoveSkuField @destFieldName = N'OffPercent', @sourceFieldName = N'autoShipDiscount', @conversion = N'CASE WHEN ISNULL(a.autoShip, 0) <> 0 THEN CAST(a.autoShipDiscount AS NVARCHAR(250)) ELSE NULL END'
	EXEC dbo.MoveSkuField @destFieldName = N'Seller', @sourceFieldName = N'sellergoogle', @conversion = N'(SELECT v.Id FROM [VitalChoice.Ecommerce].[dbo].[Lookups] AS l INNER JOIN [VitalChoice.Ecommerce].dbo.LookupVariants AS v ON v.IdLookup = l.Id WHERE l.Name = N''ProductSellers'' AND v.ValueVariant = a.sellergoogle)'
	EXEC dbo.MoveSkuField @destFieldName = N'HideFromDataFeed', @sourceFieldName = N'excludegoogle', @conversion = N'CASE WHEN ISNULL(a.excludegoogle, 0) <> 0 THEN ''True'' ELSE ''False'' END'

	DELETE FROM [VitalChoice.Ecommerce].dbo.SkuOptionValues
	WHERE Value IS NULL OR Value = N''

	INSERT [VitalChoice.Infrastructure].dbo.RecipesToProducts
	(IdProduct, IdRecipe)
	SELECT rp.idProduct, r.Id FROM [vitalchoice2.0].dbo.recipes_products AS rp
	INNER JOIN [VitalChoice.Infrastructure].dbo.Recipes AS r ON r.IdOld = rp.idRecipe
	INNER JOIN [VitalChoice.Infrastructure].dbo.Products AS p ON p.Id = rp.idProduct
	WHERE p.Id IN (SELECT Id FROM TempProductsToMove)

INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
(IdOptionType, IdProduct, Value)
SELECT t.Id, p.Id, pp.GoogleFeedTitle FROM [VitalChoice.Ecommerce].dbo.Products AS p
INNER JOIN [vitalchoice2.0].dbo.products AS pp ON pp.idProduct = p.Id
INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON t.IdObjectType = p.IdObjectType AND t.Name = 'GoogleFeedTitle'
WHERE p.Id IN (SELECT Id FROM TempProductsToMove)

INSERT INTO [VitalChoice.Ecommerce].dbo.ProductOptionValues
(IdOptionType, IdProduct, Value)
SELECT t.Id, p.Id, LEFT(pp.GoogleFeedDescription, 250) FROM [VitalChoice.Ecommerce].dbo.Products AS p
INNER JOIN [vitalchoice2.0].dbo.products AS pp ON pp.idProduct = p.Id
INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON t.IdObjectType = p.IdObjectType AND t.Name = 'GoogleFeedDescription' 
WHERE p.Id IN (SELECT Id FROM TempProductsToMove)

GO

UPDATE [VitalChoice.Ecommerce].dbo.Skus
SET WholesalePrice=Price
WHERE IdProduct IN (SELECT Id FROM TempProductsToMove)

GO

USE [VitalChoice.Ecommerce]
GO

DELETE v
FROM ProductOptionValues AS v
INNER JOIN ProductOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.DefaultValue = v.Value

GO

INSERT INTO ProductOptionValues
(IdProduct, IdOptionType, Value)
SELECT c.Id, t.Id, t.DefaultValue FROM Products AS c
INNER JOIN ProductOptionTypes AS t ON t.IdObjectType = c.IdObjectType OR t.IdObjectType IS NULL
WHERE t.DefaultValue IS NOT NULL AND NOT EXISTS(SELECT * FROM ProductOptionValues AS v WHERE v.IdProduct = c.Id AND v.IdOptionType = t.Id)

GO

DELETE p 
FROM ProductOptionValues AS p
INNER JOIN ProductOptionTypes AS t ON t.Id = p.IdOptionType AND t.Name LIKE 'Cross%'
WHERE p.Value = t.DefaultValue

GO

DELETE p 
FROM ProductOptionValues AS p
INNER JOIN ProductOptionTypes AS t ON t.Id = p.IdOptionType AND t.Name LIKE 'Youtube%'
WHERE p.Value = t.DefaultValue

GO

DELETE v
FROM SkuOptionValues AS v
INNER JOIN SkuOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.DefaultValue = v.Value

GO

INSERT INTO SkuOptionValues
(IdSku, IdOptionType, Value)
SELECT c.Id, t.Id, t.DefaultValue FROM Skus AS c
INNER JOIN Products AS p ON p.Id = c.IdProduct
INNER JOIN SkuOptionTypes AS t ON t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL
WHERE t.DefaultValue IS NOT NULL AND NOT EXISTS(SELECT * FROM SkuOptionValues AS v WHERE v.IdSku = c.Id AND v.IdOptionType = t.Id)

GO


UPDATE [VitalChoice.Infrastructure].dbo.ContentItems
SET Description = [vitalchoice2.0].dbo.RegexReplace('<table.*?>.*?<tr>\s*<td.*?>(.*?)</td>\s*</tr>.*?</table>',c.Description,'$1')
FROM [VitalChoice.Infrastructure].dbo.ContentItems AS c
INNER JOIN [VitalChoice.Infrastructure].dbo.Products AS p ON c.Id = p.ContentItemId
WHERE p.Id IN (SELECT Id FROM [vitalchoice2.0].dbo.TempProductsToMove)

UPDATE [VitalChoice.Infrastructure].dbo.ContentItems
SET Description = [vitalchoice2.0].dbo.RegexReplace('<table.*?>.*?<tr>\s*<td.*?>(.*?)</td>\s*</tr>.*?</table>',c.Description,'$1')
FROM [VitalChoice.Infrastructure].dbo.ContentItems AS c
INNER JOIN [VitalChoice.Infrastructure].dbo.Products AS p ON c.Id = p.ContentItemId
WHERE p.Id IN (SELECT Id FROM [vitalchoice2.0].dbo.TempProductsToMove)

UPDATE [VitalChoice.Infrastructure].dbo.ContentItems
SET Description = [vitalchoice2.0].dbo.RegexReplace('\s*style="FONT-WEIGHT:\s*normal"',c.Description,'')
FROM [VitalChoice.Infrastructure].dbo.ContentItems AS c
INNER JOIN [VitalChoice.Infrastructure].dbo.Products AS p ON c.Id = p.ContentItemId
WHERE p.Id IN (SELECT Id FROM [vitalchoice2.0].dbo.TempProductsToMove)


UPDATE [VitalChoice.Ecommerce].dbo.BigStringValues
SET Value = [vitalchoice2.0].dbo.RegexReplace('<table.*?>.*?<tr>\s*<td.*?>(.*?)</td>\s*</tr>.*?</table>',b.Value,'$1')
FROM [VitalChoice.Ecommerce].dbo.BigStringValues AS b
INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionValues AS v ON b.IdBigString = v.IdBigString
INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON t.Name = 'Description' AND t.Id = v.IdOptionType
WHERE v.IdProduct IN (SELECT Id FROM [vitalchoice2.0].dbo.TempProductsToMove)

UPDATE [VitalChoice.Ecommerce].dbo.BigStringValues
SET Value = [vitalchoice2.0].dbo.RegexReplace('<table.*?>.*?<tr>\s*<td.*?>(.*?)</td>\s*</tr>.*?</table>',b.Value,'$1')
FROM [VitalChoice.Ecommerce].dbo.BigStringValues AS b
INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionValues AS v ON b.IdBigString = v.IdBigString
INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON t.Name = 'Description' AND t.Id = v.IdOptionType
WHERE v.IdProduct IN (SELECT Id FROM [vitalchoice2.0].dbo.TempProductsToMove)

UPDATE [VitalChoice.Ecommerce].dbo.BigStringValues
SET Value = [vitalchoice2.0].dbo.RegexReplace('\s*style="FONT-WEIGHT:\s*normal"',b.Value,'')
FROM [VitalChoice.Ecommerce].dbo.BigStringValues AS b
INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionValues AS v ON b.IdBigString = v.IdBigString
INNER JOIN [VitalChoice.Ecommerce].dbo.ProductOptionTypes AS t ON t.Name = 'Description' AND t.Id = v.IdOptionType
WHERE v.IdProduct IN (SELECT Id FROM [vitalchoice2.0].dbo.TempProductsToMove)

GO

DROP TABLE [vitalchoice2.0].dbo.TempProductsToMove

GO

ALTER TABLE [vitalchoice2.0].[dbo].pcReviewsData
ALTER COLUMN pcRD_Comment NVARCHAR(4000) NULL

GO

ALTER TABLE [VitalChoice.Infrastructure].dbo.ContentItems
	DROP COLUMN TempId, COLUMN TempCategoryId

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.ProductReviews
ADD IdOld INT NULL

GO 

INSERT INTO [VitalChoice.Ecommerce].dbo.ProductReviews
(IdProduct, CustomerName, Description, Email, DateCreated, DateEdited, Rating, StatusCode, Title, IdOld)
SELECT r.pcRev_IDProduct, d.Name, d.Comment, ISNULL(d.Email, N''), r.pcRev_Date, r.pcRev_Date, 5, CASE WHEN r.pcRev_Active = 1 THEN 2 ELSE 1 END, d.Title, r.pcRev_IDReview
  FROM [vitalchoice2.0].[dbo].[pcReviews] AS r
  INNER JOIN (
	SELECT [1] AS Name, [2] AS Title, [3] AS Rate, [4] AS Comment, [5] AS Email, pvt.pcRD_IDReview, pvt.pcRD_Rate
	FROM
		(SELECT pcRD_IDField, pcRD_IDReview, pcRD_Comment, pcRD_Rate
		FROM [vitalchoice2.0].[dbo].pcReviewsData) s
		PIVOT (
			MIN(s.pcRD_Comment) FOR s.pcRD_IDField IN ([1], [2], [3], [4], [5])
		) AS pvt
  ) d ON d.pcRD_IDReview = r.pcRev_IDReview
  INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = r.pcRev_IDProduct
  WHERE d.Name IS NOT NULL AND d.Name <> N''

UPDATE [VitalChoice.Ecommerce].dbo.ProductReviews
SET Rating = d.pcRD_Rate
FROM [VitalChoice.Ecommerce].dbo.ProductReviews AS r
INNER JOIN [vitalchoice2.0].[dbo].[pcReviews] AS rr ON rr.pcRev_IDReview = r.IdOld
INNER JOIN [vitalchoice2.0].dbo.pcReviewsData AS d ON d.pcRD_IDReview = rr.pcRev_IDReview
WHERE d.pcRD_Rate > 0
GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.ProductReviews
DROP COLUMN IdOld
GO

GO
USE [VitalChoice.Ecommerce]
GO

INSERT INTO GiftCertificates
(Code, Created, Balance, Email, FirstName, LastName, GCType, IdOrder, IdSku, StatusCode, PublicId)
SELECT 
	g.pcGO_GcCode, 
	ISNULL(g.DateCreated, '2013-01-01'), 
	g.pcGO_Amount, 
	CASE WHEN o.pcOrd_GcReEmail = N'' THEN NULL ELSE o.pcOrd_GcReEmail END,
	CASE WHEN CHARINDEX(' ', o.pcOrd_GcReName, 0) > 1 THEN SUBSTRING(o.pcOrd_GcReName, 0, CHARINDEX(' ', o.pcOrd_GcReName, 0) + 1) ELSE o.pcOrd_GcReName END,
	CASE WHEN CHARINDEX(' ', o.pcOrd_GcReName, 0) > 1 THEN SUBSTRING(o.pcOrd_GcReName, CHARINDEX(' ', o.pcOrd_GcReName, 0), LEN(o.pcOrd_GcReName)) ELSE NULL END,
	CASE WHEN s.Id IS NULL THEN 1 ELSE CASE WHEN pp.IdObjectType = 3 THEN 2 ELSE 3 END END, 
	NULL, 
	s.Id,
	CASE g.pcGO_Status WHEN 0 THEN 1 ELSE 2 END,
	NEWID()
FROM [vitalchoice2.0].dbo.pcGCOrdered AS g
LEFT JOIN [vitalchoice2.0].dbo.products AS p ON g.pcGO_IDProduct = p.idProduct
LEFT JOIN Skus AS s ON s.Id = g.pcGO_IDProduct
LEFT JOIN Products AS pp ON pp.Id = s.IdProduct
LEFT JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = g.pcGO_IDOrder

GO

USE [vitalchoice2.0]
GO

IF EXISTS(SELECT * FROM sys.procedures WHERE name = N'MoveDiscountField')
	DROP PROCEDURE dbo.MoveDiscountField
GO

CREATE PROCEDURE dbo.MoveDiscountField
(@destFieldName NVARCHAR(250), @sourceFieldName NVARCHAR(250), @fieldOperation NVARCHAR(MAX) = NULL, @sourceConditions NVARCHAR(MAX) = NULL)
AS
BEGIN
BEGIN TRY
	DECLARE @sql NVARCHAR(MAX)
	
	IF @fieldOperation IS NULL

		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountOptionValues
		(IdOptionType, IdDiscount, Value)
		SELECT t.Id, p.Id, a.'+@sourceFieldName+' FROM [VitalChoice.Ecommerce].dbo.Discounts AS p
		INNER JOIN [vitalchoice2.0].[dbo].discounts AS a ON a.idDiscount = p.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.DiscountOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'''+@destFieldName+'''
		WHERE a.'+@sourceFieldName+' IS NOT NULL AND ('+ISNULL(@sourceConditions, '1=1')+')';

	ELSE
		
		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountOptionValues
		(IdOptionType, IdDiscount, Value)
		SELECT t.Id, p.Id, '+@fieldOperation+' FROM [VitalChoice.Ecommerce].dbo.Discounts AS p
		INNER JOIN [vitalchoice2.0].[dbo].discounts AS a ON a.idDiscount = p.Id
		INNER JOIN [VitalChoice.Ecommerce].dbo.DiscountOptionTypes AS t ON (t.IdObjectType = p.IdObjectType OR t.IdObjectType IS NULL) AND t.Name = N'''+@destFieldName+'''
		WHERE a.'+@sourceFieldName+' IS NOT NULL AND ('+ISNULL(@sourceConditions, '1=1')+')';

	EXEC(@sql)
END TRY
BEGIN CATCH
	SELECT 
		ERROR_MESSAGE() AS [Message],
		@destFieldName AS dest,
		@sourceFieldName AS source
END CATCH
END

GO

USE [VitalChoice.Ecommerce]
GO


--=========== Import ==============

USE [vitalchoice2.0]

GO

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Discounts ON;

GO
--================== Price =====================

INSERT INTO [VitalChoice.Ecommerce].dbo.Discounts
(Id, DateCreated, DateEdited, IdObjectType, StatusCode, Code, Description, ExcludeCategories, ExcludeSkus, ExpirationDate, StartDate, Assigned)
SELECT 
a.iddiscount, GETDATE(), GETDATE(), 1, CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END, discountcode, a.discountdesc, a.pcDisc_IncExcCat, a.pcDisc_IncExcPrd, ISNULL(a.expDate, DATEADD(year, 1, GETDATE())), ISNULL(a.pcDisc_StartDate, '2010-01-01'), CASE WHEN ISNULL(a.pcWholesaleFlag, 0) <> 0 THEN 2 ELSE CASE WHEN ISNULL(a.pcRetailFlag, 0) <> 0 THEN 1 ELSE NULL END END
FROM [vitalchoice2.0].[dbo].discounts AS a
WHERE ISNULL(a.pricetodiscount, 0) > 0

--================== Percent =========================

INSERT INTO [VitalChoice.Ecommerce].dbo.Discounts
(Id, DateCreated, DateEdited, IdObjectType, StatusCode, Code, Description, ExcludeCategories, ExcludeSkus, ExpirationDate, StartDate, Assigned)
SELECT 
a.iddiscount, GETDATE(), GETDATE(), 2, CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END, discountcode, a.discountdesc, a.pcDisc_IncExcCat, a.pcDisc_IncExcPrd, ISNULL(a.expDate, DATEADD(year, 1, GETDATE())), ISNULL(a.pcDisc_StartDate, '2010-01-01'), CASE WHEN ISNULL(a.pcWholesaleFlag, 0) <> 0 THEN 2 ELSE CASE WHEN ISNULL(a.pcRetailFlag, 0) <> 0 THEN 1 ELSE NULL END END
FROM [vitalchoice2.0].[dbo].discounts AS a
WHERE ISNULL(a.percentagetodiscount, 0) > 0

INSERT INTO [VitalChoice.Ecommerce].dbo.Discounts
(Id, DateCreated, DateEdited, IdObjectType, StatusCode, Code, Description, ExcludeCategories, ExcludeSkus, ExpirationDate, StartDate, Assigned)
SELECT 
a.iddiscount, GETDATE(), GETDATE(), 2, CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END, discountcode, a.discountdesc, a.pcDisc_IncExcCat, a.pcDisc_IncExcPrd, ISNULL(a.expDate, DATEADD(year, 1, GETDATE())), ISNULL(a.pcDisc_StartDate, '2010-01-01'), CASE WHEN ISNULL(a.pcWholesaleFlag, 0) <> 0 THEN 2 ELSE CASE WHEN ISNULL(a.pcRetailFlag, 0) <> 0 THEN 1 ELSE NULL END END
FROM [vitalchoice2.0].[dbo].discounts AS a
WHERE ISNULL(a.singleProductPercent, 0) > 0

--=================== Free Shipping ====================

INSERT INTO [VitalChoice.Ecommerce].dbo.Discounts
(Id, DateCreated, DateEdited, IdObjectType, StatusCode, Code, Description, ExcludeCategories, ExcludeSkus, ExpirationDate, StartDate, Assigned)
SELECT 
a.iddiscount, GETDATE(), GETDATE(), 3, CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END, discountcode, a.discountdesc, a.pcDisc_IncExcCat, a.pcDisc_IncExcPrd, ISNULL(a.expDate, DATEADD(year, 1, GETDATE())), ISNULL(a.pcDisc_StartDate, '2010-01-01'), CASE WHEN ISNULL(a.pcWholesaleFlag, 0) <> 0 THEN 2 ELSE CASE WHEN ISNULL(a.pcRetailFlag, 0) <> 0 THEN 1 ELSE NULL END END
FROM [vitalchoice2.0].[dbo].discounts AS a
INNER JOIN [vitalchoice2.0].[dbo].[pcDFShip] AS s ON s.pcFShip_IDDiscount = a.iddiscount

--================== Threshold =========================

INSERT INTO [VitalChoice.Ecommerce].dbo.Discounts
(Id, DateCreated, DateEdited, IdObjectType, StatusCode, Code, Description, ExcludeCategories, ExcludeSkus, ExpirationDate, StartDate, Assigned)
SELECT 
a.iddiscount, GETDATE(), GETDATE(), 4, CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END, discountcode, a.discountdesc, a.pcDisc_IncExcCat, a.pcDisc_IncExcPrd, ISNULL(a.expDate, DATEADD(year, 1, GETDATE())), ISNULL(a.pcDisc_StartDate, '2010-01-01'), CASE WHEN ISNULL(a.pcWholesaleFlag, 0) <> 0 THEN 2 ELSE CASE WHEN ISNULL(a.pcRetailFlag, 0) <> 0 THEN 1 ELSE NULL END END
FROM [vitalchoice2.0].[dbo].discounts AS a
WHERE ISNULL(a.specialEnabled, 0) <> 0

--================== Tiered =========================

INSERT INTO [VitalChoice.Ecommerce].dbo.Discounts
(Id, DateCreated, DateEdited, IdObjectType, StatusCode, Code, Description, ExcludeCategories, ExcludeSkus, ExpirationDate, StartDate, Assigned)
SELECT 
a.iddiscount, GETDATE(), GETDATE(), 5, CASE WHEN ISNULL(a.active, 0) <> 0 THEN 2/*Active*/ ELSE 1 /*Not Active*/ END, discountcode, a.discountdesc, a.pcDisc_IncExcCat, a.pcDisc_IncExcPrd, ISNULL(a.expDate, DATEADD(year, 1, GETDATE())), ISNULL(a.pcDisc_StartDate, '2010-01-01'), CASE WHEN ISNULL(a.pcWholesaleFlag, 0) <> 0 THEN 2 ELSE CASE WHEN ISNULL(a.pcRetailFlag, 0) <> 0 THEN 1 ELSE NULL END END
FROM [vitalchoice2.0].[dbo].discounts AS a
WHERE a.tiered <> 0

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.Discounts OFF;

--======================= Google Category =============================

EXEC dbo.MoveDiscountField @destFieldName = N'MaxTimesUse', @sourceFieldName = N'onetime', @fieldOperation = N'CASE WHEN a.onetime <> 0 THEN N''1'' ELSE NULL END'
EXEC dbo.MoveDiscountField @destFieldName = N'RequireMinimumPerishable', @sourceFieldName = N'minPerishEnabled', @fieldOperation = N'CASE WHEN a.minPerishEnabled <> 0 THEN N''True'' ELSE N''False'' END'
EXEC dbo.MoveDiscountField @destFieldName = N'RequireMinimumPerishableAmount', @sourceFieldName = N'minPerish', @fieldOperation = N'CASE WHEN a.minPerishEnabled <> 0 THEN CAST(a.minPerish AS NVARCHAR(250)) ELSE NULL END'
EXEC dbo.MoveDiscountField @destFieldName = N'AllowHealthwise', @sourceFieldName = N'allowHealthwise', @fieldOperation = N'CASE WHEN ISNULL(a.allowHealthwise, 0) <> 0 THEN N''True'' ELSE N''False'' END'
EXEC dbo.MoveDiscountField @destFieldName = N'ProductSKU', @sourceFieldName = N'specialSku'
EXEC dbo.MoveDiscountField @destFieldName = N'Threshold', @sourceFieldName = N'specialThreshold', @fieldOperation = N'CAST(a.specialThreshold AS NVARCHAR(250))'
EXEC dbo.MoveDiscountField @destFieldName = N'Amount', @sourceFieldName = N'pricetodiscount', @fieldOperation = N'CAST(a.pricetodiscount AS NVARCHAR(250))', @sourceConditions = 'a.pricetodiscount > 0'
EXEC dbo.MoveDiscountField @destFieldName = N'Percent', @sourceFieldName = N'percentagetodiscount', @fieldOperation = N'CAST(a.percentagetodiscount AS NVARCHAR(250))', @sourceConditions = 'a.percentagetodiscount > 0'
EXEC dbo.MoveDiscountField @destFieldName = N'Percent', @sourceFieldName = N'singleProductPercent', @fieldOperation = N'CAST(a.singleProductPercent AS NVARCHAR(250))', @sourceConditions = 'a.singleProductPercent > 0'
	

INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountsToCategories
(IdCategory, IdDiscount)
SELECT c.pcFCat_IDCategory, c.pcFCat_IDDiscount FROM [vitalchoice2.0].[dbo].[pcDFCats] AS c

INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountsToSkus
(IdSku, IdDiscount)
SELECT s.Id, c.pcFPro_IDDiscount FROM [vitalchoice2.0].[dbo].[pcDFProds] AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Products AS p ON p.Id = c.pcFPro_IDProduct
INNER JOIN [VitalChoice.Ecommerce].dbo.Skus AS s ON s.IdProduct = p.Id

INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountsToSelectedSkus
(IdDiscount, IdSku)
SELECT d.iddiscount, p.idProduct FROM [vitalchoice2.0].[dbo].[discounts] AS d
INNER JOIN [vitalchoice2.0].dbo.productDiscounts AS p ON p.idDiscount = d.iddiscount

INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountTiers
(Amount, [Percent], [From], [To], IdDiscount, IdDiscountType, [Order])
SELECT d.tier1value, d.tier1value, d.tier1thresholdStart, d.tier1thresholdEnd, d.iddiscount, CASE WHEN d.tier1percent IS NOT NULL AND d.tier1percent > 0 THEN 2 ELSE 1 END, 0
FROM [vitalchoice2.0].dbo.discounts AS d
WHERE d.tiered <> 0 AND d.tier1thresholdStart > 0

INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountTiers
(Amount, [Percent], [From], [To], IdDiscount, IdDiscountType, [Order])
SELECT d.tier2value, d.tier2value, d.tier1thresholdEnd + 0.01, d.tier2thresholdEnd, d.iddiscount, CASE WHEN d.tier2percent IS NOT NULL AND d.tier2percent > 0 THEN 2 ELSE 1 END, 1
FROM [vitalchoice2.0].dbo.discounts AS d
WHERE d.tiered <> 0 AND d.tier1thresholdEnd > 0 AND d.tier1thresholdStart > 0

INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountTiers
(Amount, [Percent], [From], [To], IdDiscount, IdDiscountType, [Order])
SELECT d.tier3value, d.tier3value, d.tier2thresholdEnd + 0.01, d.tier3thresholdEnd, d.iddiscount, CASE WHEN d.tier3percent IS NOT NULL AND d.tier3percent > 0 THEN 2 ELSE 1 END, 2
FROM [vitalchoice2.0].dbo.discounts AS d
WHERE d.tiered <> 0 AND d.tier2thresholdEnd > 0 AND d.tier1thresholdEnd > 0 AND d.tier1thresholdStart > 0

GO

DELETE v
FROM [VitalChoice.Ecommerce].dbo.DiscountOptionValues AS v
INNER JOIN [VitalChoice.Ecommerce].dbo.DiscountOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.DefaultValue = v.Value

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.DiscountOptionValues
(IdDiscount, IdOptionType, Value)
SELECT c.Id, t.Id, t.DefaultValue FROM [VitalChoice.Ecommerce].dbo.Discounts AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.DiscountOptionTypes AS t ON t.IdObjectType = c.IdObjectType OR t.IdObjectType IS NULL
WHERE t.DefaultValue IS NOT NULL AND NOT EXISTS(SELECT * FROM [VitalChoice.Ecommerce].dbo.DiscountOptionValues AS v WHERE v.IdDiscount = c.Id AND v.IdOptionType = t.Id)

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.OneTimeDiscountToCustomerUsages
(IdCustomer, IdDiscount, UsageCount)
SELECT DISTINCT ud.idcustomer, dd.Id, 1 FROM [vitalchoice2.0].dbo.used_discounts AS ud
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS c ON c.Id = ud.idcustomer
INNER JOIN [vitalchoice2.0].dbo.discounts AS d ON d.iddiscount = ud.iddiscount
INNER JOIN [VitalChoice.Ecommerce].dbo.Discounts AS dd ON dd.Code = d.discountcode

GO


USE [vitalchoice2.0]
GO

IF OBJECT_ID('[dbo].[DelimitedSplit8K]') IS NOT NULL
	DROP FUNCTION [dbo].[DelimitedSplit8K]

GO

CREATE FUNCTION [dbo].[DelimitedSplit8K]
--===== Define I/O parameters
        (@pString VARCHAR(8000), @pDelimiter NVARCHAR(4))
--WARNING!!! DO NOT USE MAX DATA-TYPES HERE!  IT WILL KILL PERFORMANCE!
RETURNS TABLE WITH SCHEMABINDING AS
 RETURN
--===== "Inline" CTE Driven "Tally Table" produces values from 1 up to 10,000...
     -- enough to cover VARCHAR(8000)
  WITH E1(N) AS (
                 SELECT 1 UNION ALL SELECT 1 UNION ALL SELECT 1 UNION ALL
                 SELECT 1 UNION ALL SELECT 1 UNION ALL SELECT 1 UNION ALL
                 SELECT 1 UNION ALL SELECT 1 UNION ALL SELECT 1 UNION ALL SELECT 1
                ),                          --10E+1 or 10 rows
       E2(N) AS (SELECT 1 FROM E1 a, E1 b), --10E+2 or 100 rows
       E4(N) AS (SELECT 1 FROM E2 a, E2 b), --10E+4 or 10,000 rows max
 cteTally(N) AS (--==== This provides the "base" CTE and limits the number of rows right up front
                     -- for both a performance gain and prevention of accidental "overruns"
                 SELECT TOP (ISNULL(DATALENGTH(@pString),0)) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) FROM E4
                ),
cteStart(N1) AS (--==== This returns N+1 (starting position of each "element" just once for each delimiter)
                 SELECT 1 UNION ALL
                 SELECT t.N+1 FROM cteTally t WHERE SUBSTRING(@pString,t.N,1) = @pDelimiter
                ),
cteLen(N1,L1) AS(--==== Return start and length (for use in substring)
                 SELECT s.N1,
                        ISNULL(NULLIF(CHARINDEX(@pDelimiter,@pString,s.N1),0)-s.N1,8000)
                   FROM cteStart s
                )
--===== Do the actual split. The ISNULL/NULLIF combo handles the length for the final element when no delimiter is found.
 SELECT ItemNumber = ROW_NUMBER() OVER(ORDER BY l.N1),
        Item       = SUBSTRING(@pString, l.N1, l.L1)
   FROM cteLen l
;
GO

USE [VitalChoice.Ecommerce]

GO

DBCC CHECKIDENT('Orders', RESEED, 1)

GO

ALTER TABLE Orders
ADD IdAutoShipOrder INT NULL

CREATE NONCLUSTERED INDEX IX_OrdersIdAutoShipOrder ON Orders (IdAutoShipOrder ASC)

GO

INSERT INTO Orders
(IdObjectType, DateCreated, DateEdited, IdEditedBy, StatusCode, OrderStatus, IdCustomer, IdDiscount, Total, ProductsSubtotal, TaxTotal, ShippingTotal, DiscountTotal, IdAddedBy, IdAutoShipOrder)
SELECT 
	2,
	ISNULL(o.pcOrd_Time, o.orderDate),
	ISNULL(o.pcOrd_Time, o.orderDate),
	p.Id,
	CASE o.orderStatus
		WHEN 100 THEN 3
		ELSE CASE aso.active 
			WHEN 0 THEN 1 
			ELSE 2
		END
	END,
	CASE o.orderStatus
		WHEN 5 THEN 4
		ELSE 2
	END,
	o.idCustomer,
	NULL,
	ISNULL(o.total, 0),
	(SELECT SUM(ISNULL(po.quantity, 0) * ISNULL(po.unitPrice, 0)) FROM [vitalchoice2.0].dbo.ProductsOrdered AS po WHERE po.idOrder = o.idOrder),
	o.taxAmount,
	0,
	0,
	p.Id,
	aso.idAutoShipOrder
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.IdOrder = aso.IdOrder
LEFT JOIN [vitalchoice2.0].dbo.admins AS a ON a.idadmin = o.agentId
LEFT JOIN [VitalChoice.Infrastructure].dbo.AdminProfiles AS p ON p.AgentId = a.AgentID
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS c ON c.Id = o.idCustomer
WHERE o.orderDate IS NOT NULL AND o.idCustomer IS NOT NULL AND EXISTS(SELECT * FROM [vitalchoice2.0].dbo.ProductsOrdered AS po WHERE po.idOrder = o.idOrder)

PRINT '====Insert auto-ship templates'

GO

SET IDENTITY_INSERT Orders ON;

GO

INSERT INTO Orders
(Id, IdObjectType, DateCreated, DateEdited, IdEditedBy, StatusCode, OrderStatus, IdCustomer, IdDiscount, Total, ProductsSubtotal, TaxTotal, ShippingTotal, DiscountTotal, IdAddedBy)
SELECT 
	o.idOrder, 
	CASE 
		WHEN o.keyCode = 'DROPSHIP' THEN 3 
		WHEN o.keyCode = 'GIFT LIST' THEN 4 
		WHEN o.isRefund = 1 THEN 6 
		WHEN o.isReship = 1 THEN 5
		WHEN EXISTS(SELECT * FROM [vitalchoice2.0].dbo.autoShipOrders AS aso WHERE aso.idOrder = o.idOrder) THEN 7
		ELSE 1
	END,
	ISNULL(o.pcOrd_Time, o.orderDate),
	ISNULL(o.pcOrd_Time, o.orderDate),
	(
		SELECT TOP 1 p.Id FROM [vitalchoice2.0].dbo.admins AS a 
		LEFT JOIN [VitalChoice.Infrastructure].dbo.AdminProfiles AS p ON p.AgentId = a.AgentID
		WHERE a.idadmin = o.agentId
	),
	CASE o.orderStatus
		WHEN 100 THEN 3
		ELSE 2
	END,
	CASE o.orderStatus
		WHEN 1 THEN 1
		WHEN 3 THEN 2
		WHEN 4 THEN 3
		WHEN 5 THEN 4
		WHEN 95 THEN 5
		WHEN 98 THEN 6
		WHEN 99 THEN 7
		WHEN 96 THEN 2
		WHEN 97 THEN 2
		WHEN 100 THEN 4
		WHEN 6 THEN 2
		ELSE 1
	END,
	o.idCustomer,
	NULL,
	ISNULL(o.total, 0),
	(SELECT SUM(ISNULL(po.quantity, 0) * ISNULL(po.unitPrice, 0)) FROM [vitalchoice2.0].dbo.ProductsOrdered AS po WHERE po.idOrder = o.idOrder),
	o.taxAmount,
	0,
	0,
	(
		SELECT TOP 1 p.Id FROM [vitalchoice2.0].dbo.admins AS a 
		LEFT JOIN [VitalChoice.Infrastructure].dbo.AdminProfiles AS p ON p.AgentId = a.AgentID
		WHERE a.idadmin = o.agentId
	)
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS c ON c.Id = o.idCustomer
WHERE o.orderDate IS NOT NULL AND o.idCustomer IS NOT NULL AND EXISTS(SELECT * FROM [vitalchoice2.0].dbo.ProductsOrdered AS po WHERE po.idOrder = o.idOrder)

SET IDENTITY_INSERT Orders OFF;

PRINT '====Insert orders'

GO

UPDATE Orders
SET ShippingTotal = ISNULL(TRY_CONVERT(MONEY,[vitalchoice2.0].dbo.RegexReplace('[a-zA-Z ]+,[0-9a-zA-Z +-]+,([0-9]*(\.[0-9]+)?),\w+,\w+,\w+', o.shipmentDetails, '$1')), 0),
	DiscountTotal = 
		CASE 
			WHEN o.discountDetails IS NOT NULL AND o.discountDetails <> 'No discounts applied.' 
				THEN 
					CASE WHEN o.discountDetails LIKE '%(%)%' 
						THEN ISNULL(TRY_CONVERT(MONEY,[vitalchoice2.0].dbo.RegexReplace('[\w\s\.\(\)]+\s*(\([^\(\)]*\))\s-\s\|{2}\s(-?[0-9]+(\.[0-9]+))?', o.discountDetails, '$2')), 0)
						ELSE ISNULL(TRY_CONVERT(MONEY,[vitalchoice2.0].dbo.RegexReplace('([\w\s\.\(\)]+)\s*-\s\|{2}\s(-?[0-9]+(\.[0-9]+))?', o.discountDetails, '$2')), 0)
					END
			ELSE 0
		END,
	IdDiscount = (
		SELECT TOP 1 d.Id FROM Discounts AS d 
		WHERE o.discountDetails IS NOT NULL AND o.discountDetails <> 'No discounts applied.' 
			AND d.Code =
			CASE WHEN o.discountDetails LIKE '%(%)%' 
				THEN [vitalchoice2.0].dbo.RegexReplace('.+?\(([^\(\)]*)\)\s-\s\|{2}\s(-?[0-9]+(\.[0-9]+))?', o.discountDetails, '$1')
				ELSE [vitalchoice2.0].dbo.RegexReplace('([\w\s\.\(\)]+)\s*-\s\|{2}\s(-?[0-9]+(\.[0-9]+))?', o.discountDetails, '$1')
			END
		ORDER BY Id DESC
	)
FROM Orders AS oo
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = oo.Id
PRINT '====Set up discounts and missing totals'
GO

UPDATE oo
SET IdOrderSource = ro.Id
FROM Orders AS oo
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = oo.Id
INNER JOIN [vitalchoice2.0].dbo.reship AS r ON r.idOrder = o.IdOrder
INNER JOIN Orders AS ro ON ro.Id = r.idOrderOriginal
PRINT '====Set up IdOrderSource (reships)'

GO

UPDATE oo
SET IdOrderSource = ro.Id
FROM Orders AS oo
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = oo.Id
INNER JOIN [vitalchoice2.0].dbo.refund AS r ON r.idOrder = o.IdOrder
INNER JOIN Orders AS ro ON ro.Id = r.idOrderOriginal
PRINT '====Set up IdOrderSource (refunds)'

GO

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	o.IdOrder AS Id,
	CAST(CAST(o.shippingSurcharge AS MONEY) AS NVARCHAR(250)) AS AlaskaHawaiiSurcharge,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.giftMessage AS NVARCHAR(250)) AS GiftMessage,
	CAST(CASE WHEN ISNULL(o.giftOrder, 0) <> 0 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS GiftOrder,
	CAST(CASE WHEN hw.id IS NOT NULL THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS IsHealthwise,
	CAST(o.keyCode AS NVARCHAR(250)) AS KeyCode,
	CAST(CASE WHEN o.orderType = 2 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS MailOrder,
	CAST(CASE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(ISNULL(cc.PONum, o.wholesalePONumber) AS NVARCHAR(250)) AS PoNumber,
	CASE WHEN o.shipDelayType > 0 THEN CONVERT(NVARCHAR(250), CAST(CASE o.shipDelayPart WHEN 2 THEN o.shipDelayNonperish ELSE o.shipDelay END AS DATETIME2), 126) ELSE N'' END AS ShipDelayDate,
	CAST(CASE WHEN o.shipDelayType > 0 THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShipDelayType,
	CAST(CAST(o.shippingOverride AS MONEY) AS NVARCHAR(250)) AS ShippingOverride,
	CAST(CAST(o.surchargeOverride AS MONEY) AS NVARCHAR(250)) AS SurchargeOverride,
	CAST(CASE WHEN o.shipmentDetails LIKE '%2Day%' THEN N'2' WHEN o.shipmentDetails LIKE '%Overnight Non-Perishable%' THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShippingUpgradeNP,
	CAST(CASE WHEN o.shipmentDetails LIKE '%Overnight Perishable%' THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShippingUpgradeP,
	CAST(CAST(o.shippingAmount AS MONEY) AS NVARCHAR(250)) AS StandardShippingCharges
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder AND oo.IdObjectType = 1
LEFT JOIN [vitalchoice2.0].dbo.healthwise AS hw ON hw.id = o.healthwiseId
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		AlaskaHawaiiSurcharge,
		--AutoShipFrequency,
		--CanadaSurcharge,
		ConfirmationEmailSent,
		GiftMessage,
		GiftOrder,
		--Guest,
		--IdDiscountTier,
		--IgnoneMinimumPerishableThreshold,
		IsHealthwise,
		KeyCode,
		MailOrder,
		OrderNotes,
		OrderType,
		PoNumber,
		ShipDelayDate,
		ShipDelayType,
		ShippingOverride,
		SurchargeOverride,
		ShippingUpgradeNP,
		ShippingUpgradeP,
		StandardShippingCharges
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name AND (t.IdObjectType IS NULL OR t.IdObjectType = 1)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====fields (Normal order)'

GO

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	o.IdOrder AS Id,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.giftMessage AS NVARCHAR(250)) AS GiftMessage,
	CAST(o.keyCode AS NVARCHAR(250)) AS KeyCode,
	CAST(CASE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(ISNULL(cc.PONum, o.wholesalePONumber) AS NVARCHAR(250)) AS PoNumber,
	CASE WHEN o.shipDelayType > 0 THEN CONVERT(NVARCHAR(250), CAST(CASE o.shipDelayPart WHEN 2 THEN o.shipDelayNonperish ELSE o.shipDelay END AS DATETIME2), 126) ELSE NULL END AS ShipDelayDate,
	CAST(CASE WHEN o.shipDelayType > 0 THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShipDelayType
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder AND oo.IdObjectType = 3
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		ConfirmationEmailSent,
		GiftMessage,
		--IdDiscountTier,
		KeyCode,
		OrderNotes,
		OrderType,
		PoNumber,
		--POrderType,
		ShipDelayDate,
		ShipDelayType
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name AND (t.IdObjectType IS NULL OR t.IdObjectType = 3)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====fields (Drop-Ship order)'

GO

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	o.IdOrder AS Id,
	CAST(CAST(o.shippingSurcharge AS MONEY) AS NVARCHAR(250)) AS AlaskaHawaiiSurcharge,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.giftMessage AS NVARCHAR(250)) AS GiftMessage,
	CAST(CASE WHEN ISNULL(o.giftOrder, 0) <> 0 THEN N'True' ELSE NULL END AS NVARCHAR(250)) AS GiftOrder,
	CAST(CASE WHEN hw.id IS NOT NULL THEN N'True' ELSE NULL END AS NVARCHAR(250)) AS IsHealthwise,
	CAST(o.keyCode AS NVARCHAR(250)) AS KeyCode,
	CAST(CASE WHEN o.orderType = 2 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS MailOrder,
	CAST(CASE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(ISNULL(cc.PONum, o.wholesalePONumber) AS NVARCHAR(250)) AS PoNumber,
	CAST(CASE WHEN o.shipDelayType > 0 THEN CONVERT(NVARCHAR(250), CAST(CASE o.shipDelayPart WHEN 2 THEN o.shipDelayNonperish ELSE o.shipDelay END AS DATETIME2), 126) ELSE NULL END AS NVARCHAR(250)) AS ShipDelayDate,
	CAST(CASE WHEN o.shipDelayType > 0 THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShipDelayType,
	CAST(CAST(o.shippingOverride AS MONEY) AS NVARCHAR(250)) AS ShippingOverride,
	CAST(CAST(o.surchargeOverride AS MONEY) AS NVARCHAR(250)) AS SurchargeOverride,
	CAST(CASE WHEN o.shipmentDetails LIKE '%2Day%' THEN N'2' WHEN o.shipmentDetails LIKE '%Overnight Non-Perishable%' THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShippingUpgradeNP,
	CAST(CASE WHEN o.shipmentDetails LIKE '%Overnight Perishable%' THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShippingUpgradeP,
	CAST(CAST(o.shippingAmount AS MONEY) AS NVARCHAR(250)) AS StandardShippingCharges
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder AND oo.IdObjectType = 4
LEFT JOIN [vitalchoice2.0].dbo.healthwise AS hw ON hw.id = o.healthwiseId
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		p.AlaskaHawaiiSurcharge,
		--AutoShipFrequency,
		--CanadaSurcharge,
		p.ConfirmationEmailSent,
		p.GiftMessage,
		p.GiftOrder,
		--Guest,
		--IdDiscountTier,
		--IgnoneMinimumPerishableThreshold,
		p.IsHealthwise,
		p.KeyCode,
		p.MailOrder,
		p.OrderNotes,
		p.OrderType,
		p.PoNumber,
		p.ShipDelayDate,
		p.ShipDelayType,
		p.ShippingOverride,
		p.SurchargeOverride,
		p.ShippingUpgradeNP,
		p.ShippingUpgradeP,
		p.StandardShippingCharges
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name AND (t.IdObjectType IS NULL OR t.IdObjectType = 4)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

PRINT '====fields (Gift-List order)'

GO

DELETE FROM LookupVariants
WHERE IdLookup = (SELECT Id FROM Lookups WHERE Name = 'ServiceCodes')

INSERT INTO LookupVariants
(Id, IdLookup, [Order], ValueVariant)
SELECT ROW_NUMBER() OVER (ORDER BY serviceCode), (SELECT Id FROM Lookups WHERE Name = 'ServiceCodes'), ROW_NUMBER() OVER (ORDER BY serviceCode), serviceCode FROM
(
SELECT DISTINCT serviceCode FROM (
SELECT serviceCode FROM [vitalchoice2.0].dbo.reship
UNION ALL
SELECT serviceCode FROM [vitalchoice2.0].dbo.refund
) f
) ff
PRINT '====add up missed service codes'

GO

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	o.IdOrder AS Id,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.keyCode AS NVARCHAR(250)) AS KeyCode,
	CAST(CASE LEFT(CAST(ISNULL(r.notes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(ISNULL(cc.PONum, o.wholesalePONumber) AS NVARCHAR(250)) AS PoNumber,
	CAST(CASE WHEN o.shipDelayType > 0 THEN CONVERT(NVARCHAR(250), CAST(CASE o.shipDelayPart WHEN 2 THEN o.shipDelayNonperish ELSE o.shipDelay END AS DATETIME2), 126) ELSE NULL END AS NVARCHAR(250)) AS ShipDelayDate,
	CAST(CASE WHEN o.shipDelayType > 0 THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShipDelayType,
	CAST(CAST(o.shippingOverride AS MONEY) AS NVARCHAR(250)) AS ShippingOverride,
	CAST(CAST(o.surchargeOverride AS MONEY) AS NVARCHAR(250)) AS SurchargeOverride,
	CAST(CASE WHEN o.shipmentDetails LIKE '%2Day%' THEN N'2' WHEN o.shipmentDetails LIKE '%Overnight Non-Perishable%' THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShippingUpgradeNP,
	CAST(CASE WHEN o.shipmentDetails LIKE '%Overnight Perishable%' THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShippingUpgradeP,
	CAST(CAST(o.shippingAmount AS MONEY) AS NVARCHAR(250)) AS StandardShippingCharges,
	CAST(CASE WHEN r.associated <> 0 THEN N'True' ELSE N'False' END AS NVARCHAR(250)) ReturnAssociated,
	CAST(lv.Id AS NVARCHAR(250)) AS ServiceCode
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder AND oo.IdObjectType = 5
INNER JOIN [vitalchoice2.0].dbo.reship AS r ON r.IdOrder = o.IdOrder
INNER JOIN LookupVariants AS lv ON lv.IdLookup = (SELECT Id FROM Lookups WHERE Name = 'ServiceCodes') AND lv.ValueVariant = r.serviceCode
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		p.ConfirmationEmailSent,
		--IdDiscountTier,
		p.KeyCode,
		p.OrderNotes,
		p.OrderType,
		p.PoNumber,
		--POrderType,
		p.ReturnAssociated,
		p.ServiceCode,
		p.ShipDelayDate,
		--ShipDelayDateNP,
		--ShipDelayDateP,
		p.ShipDelayType,
		p.ShippingOverride,
		p.ShippingUpgradeNP,
		p.ShippingUpgradeP,
		p.SurchargeOverride
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name AND (t.IdObjectType IS NULL OR t.IdObjectType = 5)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====fields (Reship order)'

GO

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	o.IdOrder AS Id,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.keyCode AS NVARCHAR(250)) AS KeyCode,
	CAST(CASE LEFT(CAST(ISNULL(r.notes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(CASE WHEN r.associated <> 0 THEN N'True' ELSE N'False' END AS NVARCHAR(250)) ReturnAssociated,
	CAST(lv.Id AS NVARCHAR(250)) AS ServiceCode,
	CAST(r.idOrderOriginal AS NVARCHAR(250)) AS IdOrderRefunded,
	CAST((SELECT SUM(CAST(rr.Value AS MONEY)) FROM [vitalchoice2.0].[dbo].[RefundItems] AS rr WHERE rr.IdRefundOrder = r.idOrder AND rr.ItemType <> 0 AND rr.ItemType <> 3) AS NVARCHAR(250)) AS AutoTotal,
	CAST(CASE WHEN EXISTS(SELECT * FROM [vitalchoice2.0].[dbo].[RefundItems] AS rr WHERE rr.IdRefundOrder = r.idOrder AND rr.ItemType = 2) THEN N'True' ELSE N'False' END AS NVARCHAR(250)) AS ShippingRefunded,
	CAST((SELECT SUM(CAST(rr.Value AS MONEY)) FROM [vitalchoice2.0].[dbo].[RefundItems] AS rr WHERE rr.IdRefundOrder = r.idOrder AND rr.ItemType = 3) AS NVARCHAR(250)) AS ManualRefundOverride
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder AND oo.IdObjectType = 6
INNER JOIN [vitalchoice2.0].dbo.refund AS r ON r.IdOrder = o.IdOrder
INNER JOIN LookupVariants AS lv ON lv.IdLookup = (SELECT Id FROM Lookups WHERE Name = 'ServiceCodes') AND lv.ValueVariant = r.serviceCode
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		AutoTotal,
		ConfirmationEmailSent,
		--IdDiscountTier,
		IdOrderRefunded,
		ManualRefundOverride,
		OrderNotes,
		OrderType,
		--POrderType,
		ReturnAssociated,
		ServiceCode,
		ShippingRefunded
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name AND (t.IdObjectType IS NULL OR t.IdObjectType = 6)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====fields (Refund order)'

GO

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	oo.Id AS Id,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.giftMessage AS NVARCHAR(250)) AS GiftMessage,
	CAST(CASE WHEN ISNULL(o.giftOrder, 0) <> 0 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS GiftOrder,
	CAST(o.keyCode AS NVARCHAR(250)) AS KeyCode,
	CAST(CASE WHEN o.orderType = 2 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS MailOrder,
	CAST(CASE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(ISNULL(cc.PONum, o.wholesalePONumber) AS NVARCHAR(250)) AS PoNumber,
	CAST(CASE WHEN o.shipDelayType > 0 THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShipDelayType,
	CAST(CAST(o.shippingOverride AS MONEY) AS NVARCHAR(250)) AS ShippingOverride,
	CAST(CAST(o.surchargeOverride AS MONEY) AS NVARCHAR(250)) AS SurchargeOverride,
	CAST(CASE WHEN o.shipmentDetails LIKE '%2Day%' THEN N'2' WHEN o.shipmentDetails LIKE '%Overnight Non-Perishable%' THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShippingUpgradeNP,
	CAST(CASE WHEN o.shipmentDetails LIKE '%Overnight Perishable%' THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShippingUpgradeP,
	CAST(aso.schedule AS NVARCHAR(250)) AS AutoShipFrequency,
	CONVERT(NVARCHAR(250), CAST(DATEADD(month, -aso.schedule, aso.nextDate) AS DATETIME2), 126) AS LastAutoShipDate
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		AutoShipFrequency,
		ConfirmationEmailSent,
		GiftMessage,
		GiftOrder,
		--IdDiscountTier,
		KeyCode,
		LastAutoShipDate,
		MailOrder,
		OrderNotes,
		OrderType,
		PoNumber,
		--POrderType,
		--ShipDelayDateNP,
		--ShipDelayDateP,
		ShipDelayType,
		ShippingOverride,
		ShippingUpgradeNP,
		ShippingUpgradeP,
		SurchargeOverride
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name AND (t.IdObjectType IS NULL OR t.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====fields (Source Order for Auto-Ship)'

GO

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	o.IdOrder AS Id,
	CAST(CAST(o.shippingSurcharge AS MONEY) AS NVARCHAR(250)) AS AlaskaHawaiiSurcharge,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.giftMessage AS NVARCHAR(250)) AS GiftMessage,
	CAST(CASE WHEN ISNULL(o.giftOrder, 0) <> 0 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS GiftOrder,
	CAST(o.keyCode AS NVARCHAR(250)) AS KeyCode,
	CAST(CASE WHEN o.orderType = 2 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS MailOrder,
	CAST(CASE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(ISNULL(cc.PONum, o.wholesalePONumber) AS NVARCHAR(250)) AS PoNumber,
	CASE WHEN o.shipDelayType > 0 THEN CONVERT(NVARCHAR(250), CAST(CASE o.shipDelayPart WHEN 2 THEN o.shipDelayNonperish ELSE o.shipDelay END AS DATETIME2), 126) ELSE N'' END AS ShipDelayDate,
	CAST(CASE WHEN o.shipDelayType > 0 THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShipDelayType,
	CAST(CAST(o.shippingOverride AS MONEY) AS NVARCHAR(250)) AS ShippingOverride,
	CAST(CAST(o.surchargeOverride AS MONEY) AS NVARCHAR(250)) AS SurchargeOverride,
	CAST(CASE WHEN o.shipmentDetails LIKE '%2Day%' THEN N'2' WHEN o.shipmentDetails LIKE '%Overnight Non-Perishable%' THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShippingUpgradeNP,
	CAST(CASE WHEN o.shipmentDetails LIKE '%Overnight Perishable%' THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShippingUpgradeP,
	CAST(CAST(o.shippingAmount AS MONEY) AS NVARCHAR(250)) AS StandardShippingCharges,
	CAST(oo.Id AS NVARCHAR(250)) AS AutoShipId,
	CAST(aso.schedule AS NVARCHAR(250)) AS AutoShipFrequency
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN [vitalchoice2.0].dbo.autoShipOrders AS aso ON aso.idOrder = o.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		AlaskaHawaiiSurcharge,
		AutoShipFrequency,
		AutoShipId,
		--CanadaSurcharge,
		ConfirmationEmailSent,
		GiftMessage,
		GiftOrder,
		--Guest,
		--IdDiscountTier,
		--IgnoneMinimumPerishableThreshold,
		KeyCode,
		MailOrder,
		OrderNotes,
		OrderType,
		PoNumber,
		--POrderType,
		ShipDelayDate,
		--ShipDelayDateNP,
		--ShipDelayDateP,
		ShipDelayType,
		ShippingOverride,
		ShippingUpgradeNP,
		ShippingUpgradeP,
		StandardShippingCharges,
		SurchargeOverride
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name AND (t.IdObjectType IS NULL OR t.IdObjectType = 7)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====fields (Auto-Ship order)'

-- billing

GO

ALTER TABLE OrderAddresses
ADD IdOrder INT NOT NULL CONSTRAINT UQ_OrderAddressesOrders UNIQUE
GO

INSERT INTO OrderAddresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdOrder)
SELECT 
	o.orderDate, 
	o.orderDate,
	ISNULL (
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = ISNULL(o.countryCode, 'US')), 
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM States AS s WHERE s.CountryCode = ISNULL(o.countryCode, 'US') AND s.StateCode = o.stateCode),
	2,
	2,
	o.[state],
	o.idOrder
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON o.idOrder = oo.Id
PRINT '====Billing addresses'

GO


INSERT INTO OrderAddresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdOrder)
SELECT 
	o.orderDate, 
	o.orderDate,
	ISNULL (
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = ISNULL(o.countryCode, 'US')), 
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM States AS s WHERE s.CountryCode = ISNULL(o.countryCode, 'US') AND s.StateCode = o.stateCode),
	2,
	2,
	o.[state],
	oo.Id
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
PRINT '====Billing addresses (Auto-Ship)'

GO

ALTER TABLE OrderPaymentMethods
ADD IdOrder INT NOT NULL CONSTRAINT UQ_OrderPaymentMethodsOrders UNIQUE

GO

INSERT INTO OrderPaymentMethods
(DateCreated, DateEdited, IdAddress, IdEditedBy, IdObjectType, StatusCode, IdOrder)
SELECT 
	o.DateCreated, 
	o.DateEdited, 
	a.Id, 
	o.IdEditedBy, 
	CASE [vitalchoice2.0].dbo.RegexReplace('([a-zA-Z\s.]+)\s+\|{2}', oo.paymentDetails, '$1')
		WHEN 'Authorize.Net' THEN 1
		WHEN 'Credit Card' THEN 1
		WHEN 'OAC' THEN 2
		WHEN 'Check' THEN 3
		WHEN 'No Charge' THEN 4
		WHEN 'Prepaid' THEN 6
		ELSE 4
	END,
	2,
	o.Id
FROM Orders AS o
INNER JOIN [vitalchoice2.0].dbo.orders AS oo ON oo.idOrder = o.Id
INNER JOIN OrderAddresses AS a ON a.IdOrder = o.Id
PRINT '====Payment methods'

GO


INSERT INTO OrderPaymentMethods
(DateCreated, DateEdited, IdAddress, IdEditedBy, IdObjectType, StatusCode, IdOrder)
SELECT 
	o.DateCreated, 
	o.DateEdited, 
	a.Id, 
	o.IdEditedBy, 
	CASE [vitalchoice2.0].dbo.RegexReplace('([a-zA-Z\s.]+)\s+\|{2}', oo.paymentDetails, '$1')
		WHEN 'Authorize.Net' THEN 1
		WHEN 'Credit Card' THEN 1
		WHEN 'OAC' THEN 2
		WHEN 'Check' THEN 3
		WHEN 'No Charge' THEN 4
		WHEN 'Prepaid' THEN 6
		ELSE 4
	END,
	2,
	o.Id
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS oo ON oo.idOrder = aso.idOrder
INNER JOIN Orders AS o ON o.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN OrderAddresses AS a ON a.IdOrder = o.Id
PRINT '====Payment methods (Auto-Ship)'

GO


UPDATE Orders 
SET IdPaymentMethod = p.Id
FROM Orders AS o
INNER JOIN OrderPaymentMethods AS p ON p.IdOrder = o.Id
PRINT '====Set up payment ids'

GO

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(cc.nameOnCard AS NVARCHAR(250)) AS NameOnCard, 
	CAST('XXXXXXXXXXXX' + RIGHT([vitalchoice2.0].dbo.RC4Encode(cc.cardnumber), 4) AS NVARCHAR(250)) AS CardNumber, 
	CAST(
		CASE cc.cardtype
			WHEN 'V' THEN N'2'
			WHEN 'M' THEN N'1'
			WHEN 'A' THEN N'3'
			WHEN 'D' THEN N'4'
			ELSE NULL
		END
	AS NVARCHAR(250)) AS CardType, 
	CONVERT(NVARCHAR(250), CAST(cc.expiration AS DATETIME2), 126) AS ExpDate
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 1) p
UNPIVOT (Value FOR Name IN 
	(NameOnCard, CardNumber, CardType, ExpDate)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 1)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====credit cards'

GO

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(cc.nameOnCard AS NVARCHAR(250)) AS NameOnCard, 
	CAST('XXXXXXXXXXXX' + RIGHT([vitalchoice2.0].dbo.RC4Encode(cc.cardnumber), 4) AS NVARCHAR(250)) AS CardNumber, 
	CAST(
		CASE cc.cardtype
			WHEN 'V' THEN N'2'
			WHEN 'M' THEN N'1'
			WHEN 'A' THEN N'3'
			WHEN 'D' THEN N'4'
			ELSE NULL
		END
	AS NVARCHAR(250)) AS CardType, 
	CONVERT(NVARCHAR(250), CAST(cc.expiration AS DATETIME2), 126) AS ExpDate
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 1) p
UNPIVOT (Value FOR Name IN 
	(NameOnCard, CardNumber, CardType, ExpDate)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 1)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====credit cards(auto-ship)'

GO

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(cc.fname + ' ' + cc.lname AS NVARCHAR(250)) AS NameOnCard, 
	CAST('XXXXXXXXXXXX' + RIGHT([vitalchoice2.0].dbo.RC4Encode(cc.ccnum), 4) AS NVARCHAR(250)) AS CardNumber, 
	CAST(
		CASE cc.cctype
			WHEN 'V' THEN N'2'
			WHEN 'M' THEN N'1'
			WHEN 'A' THEN N'3'
			WHEN 'D' THEN N'4'
			ELSE NULL
		END
	AS NVARCHAR(250)) AS CardType, 
	CONVERT(NVARCHAR(250), CAST(DATEFROMPARTS(CAST(SUBSTRING(cc.ccexp, 3, 2) AS INT) + 2000,CAST(SUBSTRING(cc.ccexp, 1, 2) AS INT), 1) AS DATETIME2), 126) AS ExpDate
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN [vitalchoice2.0].dbo.authorders AS cc ON cc.idauthorder = (SELECT TOP 1 idauthorder FROM [vitalchoice2.0].dbo.authorders AS ac WHERE ac.idOrder = o.idOrder ORDER BY idauthorder DESC)
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 1
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS oc ON oc.idOrder = o.idOrder
WHERE oc.idOrder IS NULL) p
UNPIVOT (Value FOR Name IN 
	(NameOnCard, CardNumber, CardType, ExpDate)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS t ON t.Name = unpvt.Name AND (t.IdObjectType IS NULL OR t.IdObjectType = 1)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====credit cards (old auth.net)'

GO

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(cc.fname + ' ' + cc.lname AS NVARCHAR(250)) AS NameOnCard, 
	CAST('XXXXXXXXXXXX' + RIGHT([vitalchoice2.0].dbo.RC4Encode(cc.ccnum), 4) AS NVARCHAR(250)) AS CardNumber, 
	CAST(
		CASE cc.cctype
			WHEN 'V' THEN N'2'
			WHEN 'M' THEN N'1'
			WHEN 'A' THEN N'3'
			WHEN 'D' THEN N'4'
			ELSE NULL
		END
	AS NVARCHAR(250)) AS CardType, 
	CONVERT(NVARCHAR(250), CAST(DATEFROMPARTS(CAST(SUBSTRING(cc.ccexp, 3, 2) AS INT) + 2000,CAST(SUBSTRING(cc.ccexp, 1, 2) AS INT), 1) AS DATETIME2), 126) AS ExpDate
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN [vitalchoice2.0].dbo.authorders AS cc ON cc.idauthorder = (SELECT TOP 1 idauthorder FROM [vitalchoice2.0].dbo.authorders AS ac WHERE ac.idOrder = o.idOrder ORDER BY idauthorder DESC)
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 1
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS oc ON oc.idOrder = o.idOrder
WHERE oc.idOrder IS NULL) p
UNPIVOT (Value FOR Name IN 
	(NameOnCard, CardNumber, CardType, ExpDate)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS t ON t.Name = unpvt.Name AND (t.IdObjectType IS NULL OR t.IdObjectType = 1)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====credit cards (old auth.net) (auto-ship)'

GO

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST([vitalchoice2.0].dbo.RC4Encode(cc.cardnumber) AS NVARCHAR(250)) AS CheckNumber, 
	CAST(
		CASE o.TaxIncluded
			WHEN 0 THEN N'False'
			WHEN 1 THEN N'True'
		END
	AS NVARCHAR(250)) AS PaidInFull
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 3) p
UNPIVOT (Value FOR Name IN 
	(CheckNumber, PaidInFull)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 3)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====check'

GO

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST([vitalchoice2.0].dbo.RC4Encode(cc.cardnumber) AS NVARCHAR(250)) AS CheckNumber, 
	CAST(
		CASE o.TaxIncluded
			WHEN 0 THEN N'False'
			WHEN 1 THEN N'True'
		END
	AS NVARCHAR(250)) AS PaidInFull
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 3) p
UNPIVOT (Value FOR Name IN 
	(CheckNumber, PaidInFull)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 3)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====check(auto-ship)'

GO

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(ISNULL(termslv.Id, 1) AS NVARCHAR(250)) AS Terms, 
	CAST(ISNULL(foblv.Id, 1) AS NVARCHAR(250)) AS Fob
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 2
INNER JOIN Lookups AS termsl ON termsl.Name = 'Terms'
LEFT JOIN LookupVariants AS termslv ON termslv.IdLookup = termsl.Id AND termslv.ValueVariant = cc.Terms
INNER JOIN Lookups AS fobl ON fobl.Name = 'Fob'
LEFT JOIN LookupVariants AS foblv ON foblv.IdLookup = fobl.Id AND foblv.ValueVariant = cc.FOB
) p
UNPIVOT (Value FOR Name IN 
	(Terms, Fob)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====OAC'

GO

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(ISNULL(termslv.Id, 1) AS NVARCHAR(250)) AS Terms, 
	CAST(ISNULL(foblv.Id, 1) AS NVARCHAR(250)) AS Fob
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 2
INNER JOIN Lookups AS termsl ON termsl.Name = 'Terms'
LEFT JOIN LookupVariants AS termslv ON termslv.IdLookup = termsl.Id AND termslv.ValueVariant = cc.Terms
INNER JOIN Lookups AS fobl ON fobl.Name = 'Fob'
LEFT JOIN LookupVariants AS foblv ON foblv.IdLookup = fobl.Id AND foblv.ValueVariant = cc.FOB
) p
UNPIVOT (Value FOR Name IN 
	(Terms, Fob)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====OAC(auto-ship)'

GO

ALTER TABLE OrderPaymentMethods
DROP CONSTRAINT UQ_OrderPaymentMethodsOrders

GO

ALTER TABLE OrderPaymentMethods
DROP COLUMN IdOrder

GO

INSERT INTO OrderAddressOptionValues
(IdOrderAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(o.address AS NVARCHAR(250)) AS Address1, 
	CAST(o.Address2 AS NVARCHAR(250)) AS Address2, 
	CAST(o.firstName AS NVARCHAR(250)) AS FirstName, 
	CAST(o.LastName AS NVARCHAR(250)) AS LastName, 
	CAST(o.company AS NVARCHAR(250)) AS Company, 
	CAST(o.City AS NVARCHAR(250)) AS City, 
	CAST(o.Zip AS NVARCHAR(250)) AS Zip,
	CAST(o.Phone AS NVARCHAR(250)) AS Phone,
	CAST(o.Fax AS NVARCHAR(250)) AS Fax
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN OrderAddresses AS a ON a.IdOrder = oo.Id
INNER JOIN OrderPaymentMethods AS p ON p.Id = oo.IdPaymentMethod
WHERE p.IdObjectType NOT IN (4, 6)) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax)
)AS unpvt
INNER JOIN AddressOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====address options'

GO

INSERT INTO OrderAddressOptionValues
(IdOrderAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(o.address AS NVARCHAR(250)) AS Address1, 
	CAST(o.Address2 AS NVARCHAR(250)) AS Address2, 
	CAST(o.firstName AS NVARCHAR(250)) AS FirstName, 
	CAST(o.LastName AS NVARCHAR(250)) AS LastName, 
	CAST(o.company AS NVARCHAR(250)) AS Company, 
	CAST(o.City AS NVARCHAR(250)) AS City, 
	CAST(o.Zip AS NVARCHAR(250)) AS Zip,
	CAST(o.Phone AS NVARCHAR(250)) AS Phone,
	CAST(o.Fax AS NVARCHAR(250)) AS Fax
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN OrderAddresses AS a ON a.IdOrder = oo.Id
INNER JOIN OrderPaymentMethods AS p ON p.Id = oo.IdPaymentMethod
WHERE p.IdObjectType NOT IN (4, 6)) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax)
)AS unpvt
INNER JOIN AddressOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====address options(auto-ship)'

GO

INSERT INTO OrderAddressOptionValues
(IdOrderAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(c.address AS NVARCHAR(250)) AS Address1, 
	CAST(c.Address2 AS NVARCHAR(250)) AS Address2, 
	CAST(c.name AS NVARCHAR(250)) AS FirstName, 
	CAST(c.LastName AS NVARCHAR(250)) AS LastName, 
	CAST(c.customerCompany AS NVARCHAR(250)) AS Company, 
	CAST(c.City AS NVARCHAR(250)) AS City, 
	CAST(c.Zip AS NVARCHAR(250)) AS Zip,
	CAST(c.Phone AS NVARCHAR(250)) AS Phone,
	CAST(c.Fax AS NVARCHAR(250)) AS Fax
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN [vitalchoice2.0].dbo.customers AS c ON c.idcustomer = o.idCustomer
INNER JOIN OrderAddresses AS a ON a.IdOrder = oo.Id
INNER JOIN OrderPaymentMethods AS p ON p.Id = oo.IdPaymentMethod
WHERE p.IdObjectType IN (4, 6)) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax)
)AS unpvt
INNER JOIN AddressOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====address options, no charge, prepaid'

GO

INSERT INTO OrderAddressOptionValues
(IdOrderAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(c.address AS NVARCHAR(250)) AS Address1, 
	CAST(c.Address2 AS NVARCHAR(250)) AS Address2, 
	CAST(c.name AS NVARCHAR(250)) AS FirstName, 
	CAST(c.LastName AS NVARCHAR(250)) AS LastName, 
	CAST(c.customerCompany AS NVARCHAR(250)) AS Company, 
	CAST(c.City AS NVARCHAR(250)) AS City, 
	CAST(c.Zip AS NVARCHAR(250)) AS Zip,
	CAST(c.Phone AS NVARCHAR(250)) AS Phone,
	CAST(c.Fax AS NVARCHAR(250)) AS Fax
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN [vitalchoice2.0].dbo.customers AS c ON c.idcustomer = o.idCustomer
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN OrderAddresses AS a ON a.IdOrder = oo.Id
INNER JOIN OrderPaymentMethods AS p ON p.Id = oo.IdPaymentMethod
WHERE p.IdObjectType IN (4, 6)) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax)
)AS unpvt
INNER JOIN AddressOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====address options, no charge, prepaid (auto-ship)'

GO

ALTER TABLE OrderAddresses
DROP CONSTRAINT UQ_OrderAddressesOrders

GO

ALTER TABLE OrderAddresses
DROP COLUMN IdOrder

GO
ALTER TABLE OrderAddresses
ADD IdOrder INT NULL

CREATE NONCLUSTERED INDEX IX_OrderAddressesIdOrder ON OrderAddresses (IdOrder)

GO

INSERT INTO OrderAddresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdOrder)
SELECT 
	o.orderDate, 
	o.orderDate,
	ISNULL (
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = ISNULL(o.shippingCountryCode, 'US')), 
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM States AS s WHERE s.CountryCode = ISNULL(o.shippingCountryCode, 'US') AND s.StateCode = o.shippingStateCode),
	2,
	3,
	o.shippingState,
	o.idOrder
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON o.idOrder = oo.Id
PRINT '====shipping addresses'

GO

INSERT INTO OrderAddresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdOrder)
SELECT 
	o.orderDate, 
	o.orderDate,
	ISNULL (
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = ISNULL(o.shippingCountryCode, 'US')), 
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM States AS s WHERE s.CountryCode = ISNULL(o.shippingCountryCode, 'US') AND s.StateCode = o.shippingStateCode),
	2,
	3,
	o.shippingState,
	oo.Id
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
PRINT '====shipping addresses(auto-ship)'

GO


UPDATE Orders
SET IdShippingAddress = a.Id
FROM Orders AS o
INNER JOIN OrderAddresses AS a ON a.IdOrder = o.Id
PRINT '====shipping addresses set up ids'

GO

INSERT INTO OrderAddressOptionValues
(IdOrderAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(o.shippingAddress AS NVARCHAR(250)) AS Address1, 
	CAST(o.shippingAddress2 AS NVARCHAR(250)) AS Address2, 
	CAST(CASE WHEN CHARINDEX(' ', o.ShippingFullName) > 1 THEN SUBSTRING(o.ShippingFullName, 1, CHARINDEX(' ', o.ShippingFullName)) ELSE o.ShippingFullName END AS NVARCHAR(250)) AS FirstName, 
	CAST(CASE WHEN CHARINDEX(' ', o.ShippingFullName) > 1 THEN SUBSTRING(o.ShippingFullName, CHARINDEX(' ', o.ShippingFullName) + 1, LEN(o.ShippingFullName)) ELSE NULL END AS NVARCHAR(250)) AS LastName, 
	CAST(o.shippingCompany AS NVARCHAR(250)) AS Company, 
	CAST(o.shippingCity AS NVARCHAR(250)) AS City, 
	CAST(o.shippingZip AS NVARCHAR(250)) AS Zip,
	CAST(o.pcOrd_ShippingPhone AS NVARCHAR(250)) AS Phone,
	CAST(o.pcOrd_ShippingFax AS NVARCHAR(250)) AS Fax,
	CAST(o.pcOrd_ShippingEmail AS NVARCHAR(250)) AS Email
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN OrderAddresses AS a ON a.IdOrder = oo.Id) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax, Email)
)AS unpvt
INNER JOIN AddressOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 3)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====shipping addresses options'

GO

INSERT INTO OrderAddressOptionValues
(IdOrderAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(o.shippingAddress AS NVARCHAR(250)) AS Address1, 
	CAST(o.shippingAddress2 AS NVARCHAR(250)) AS Address2, 
	CAST(CASE WHEN CHARINDEX(' ', o.ShippingFullName) > 1 THEN SUBSTRING(o.ShippingFullName, 1, CHARINDEX(' ', o.ShippingFullName)) ELSE o.ShippingFullName END AS NVARCHAR(250)) AS FirstName, 
	CAST(CASE WHEN CHARINDEX(' ', o.ShippingFullName) > 1 THEN SUBSTRING(o.ShippingFullName, CHARINDEX(' ', o.ShippingFullName) + 1, LEN(o.ShippingFullName)) ELSE NULL END AS NVARCHAR(250)) AS LastName, 
	CAST(o.shippingCompany AS NVARCHAR(250)) AS Company, 
	CAST(o.shippingCity AS NVARCHAR(250)) AS City, 
	CAST(o.shippingZip AS NVARCHAR(250)) AS Zip,
	CAST(o.pcOrd_ShippingPhone AS NVARCHAR(250)) AS Phone,
	CAST(o.pcOrd_ShippingFax AS NVARCHAR(250)) AS Fax,
	CAST(o.pcOrd_ShippingEmail AS NVARCHAR(250)) AS Email
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN OrderAddresses AS a ON a.IdOrder = oo.Id) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax, Email)
)AS unpvt
INNER JOIN AddressOptionTypes AS o ON o.Name = unpvt.Name AND (o.IdObjectType IS NULL OR o.IdObjectType = 3)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
PRINT '====shipping addresses options(auto-ship)'

GO

DROP INDEX IX_OrderAddressesIdOrder ON OrderAddresses

GO

ALTER TABLE OrderAddresses
DROP COLUMN IdOrder

GO

INSERT INTO OrderToGiftCertificates
(IdOrder, IdGiftCertificate, Amount)
SELECT o.Id, (SELECT TOP 1 g.Id FROM GiftCertificates AS g WHERE g.Code = [vitalchoice2.0].dbo.RegexReplace('(?<code>[^\|]+)(\|[sS]\|)(?<description>[^\|]*)(\|[sS]\|)(?<amount>[0-9]+(\.[0-9]*)?)', gc.Item, '${code}')), CONVERT(MONEY, [vitalchoice2.0].dbo.RegexReplace('(?<code>[^\|]+)(\|[sS]\|)(?<description>[^\|]*)(\|[sS]\|)(?<amount>[0-9]+(\.[0-9]*)?)', gc.Item, '${amount}')) FROM Orders AS o
INNER JOIN [vitalchoice2.0].dbo.orders AS oo ON oo.idOrder = o.Id
CROSS APPLY [vitalchoice2.0].[dbo].[DelimitedSplit8K](oo.pcOrd_GCDetails, '|g|') AS gc
WHERE oo.pcOrd_GCDetails IS NOT NULL AND oo.pcOrd_GCDetails <> '' AND gc.Item IS NOT NULL AND gc.Item <> N'' AND (SELECT TOP 1 g.Id FROM GiftCertificates AS g WHERE g.Code = [vitalchoice2.0].dbo.RegexReplace('(?<code>[^\|]+)(\|[sS]\|)(?<description>[^\|]*)(\|[sS]\|)(?<amount>[0-9]+(\.[0-9]*)?)', gc.Item, '${code}')) IS NOT NULL
PRINT '====gift certificates used in order'

GO

INSERT INTO OrderToSkus
(IdOrder, IdSku, Amount, Quantity)
SELECT po.IdOrder, po.IdProduct, MAX(ISNULL(unitprice, 0)), SUM(quantity) FROM [vitalchoice2.0].[dbo].ProductsOrdered AS po
INNER JOIN Orders AS o ON po.idOrder = o.Id
INNER JOIN Skus AS s ON s.Id = po.IdProduct
WHERE po.quantity > 0
GROUP BY po.IdOrder, po.IdProduct
PRINT '====products ordered'

GO

INSERT INTO OrderToSkus
(IdOrder, IdSku, Amount, Quantity)
SELECT po.IdOrder, s.Id, MAX(ISNULL(unitprice, 0)), SUM(quantity) FROM [vitalchoice2.0].[dbo].ProductsOrdered AS po
INNER JOIN Orders AS o ON po.idOrder = o.Id
INNER JOIN [vitalchoice2.0].dbo.products AS p ON p.IdProduct = po.idProduct
INNER JOIN Skus AS s ON s.StatusCode <> 3 AND s.Code = p.sku
WHERE po.quantity > 0 AND po.IdProduct NOT IN (SELECT Id FROM SKus)
GROUP BY po.IdOrder, s.Id
PRINT '====products ordered (missed sku ids)'

GO

INSERT INTO OrderToSkus
(IdOrder, IdSku, Amount, Quantity)
SELECT o.Id, po.IdProduct, MAX(ISNULL(unitprice, 0)), SUM(quantity) 
FROM [vitalchoice2.0].[dbo].AutoshipOrders AS aso
INNER JOIN Orders AS o ON o.IdAutoShipOrder = aso.IdAutoShipOrder
INNER JOIN [vitalchoice2.0].[dbo].orders AS oo ON oo.IdOrder = aso.IdOrder
INNER JOIN [vitalchoice2.0].[dbo].ProductsOrdered AS po ON po.IdOrder = oo.IdOrder
INNER JOIN Skus AS s ON s.Id = po.IdProduct
WHERE po.quantity > 0
GROUP BY o.Id, po.IdProduct
PRINT '====products ordered (auto-ship)'

GO

INSERT INTO OrderToSkus
(IdOrder, IdSku, Amount, Quantity)
SELECT o.Id, s.Id, MAX(ISNULL(unitprice, 0)), SUM(quantity) 
FROM [vitalchoice2.0].[dbo].AutoshipOrders AS aso
INNER JOIN Orders AS o ON o.IdAutoShipOrder = aso.IdAutoShipOrder
INNER JOIN [vitalchoice2.0].[dbo].orders AS oo ON oo.IdOrder = aso.IdOrder
INNER JOIN [vitalchoice2.0].[dbo].ProductsOrdered AS po ON po.IdOrder = oo.IdOrder
INNER JOIN [vitalchoice2.0].dbo.products AS p ON p.IdProduct = po.idProduct
INNER JOIN Skus AS s ON s.StatusCode <> 3 AND s.Code = p.sku
WHERE po.quantity > 0 AND po.IdProduct NOT IN (SELECT Id FROM SKus)
GROUP BY o.Id, s.Id
PRINT '====products ordered (auto-ship) (missed skus)'

GO

DECLARE @poOrderTypeId INT

SELECT @poOrderTypeId = t.Id FROM OrderOptionTypes AS t
WHERE t.Name = 'POrderType'

DELETE FROM OrderOptionValues
WHERE IdOptionType = @poOrderTypeId
PRINT '====POrderType values clean up';

WITH productTypes AS
(
	SELECT p.IdObjectType, so.IdOrder FROM OrderToSkus AS so
	INNER JOIN Skus AS s ON s.Id = so.IdSku
	INNER JOIN Products AS p ON p.Id = s.IdProduct
)
INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT 
	o.Id, 
	@poOrderTypeId,  
	CAST(
		CASE 
			WHEN (
					1 = ANY(SELECT IdObjectType FROM productTypes AS so
							WHERE IdOrder = o.Id)
					OR
					4 = ANY(SELECT IdObjectType FROM productTypes AS so
							WHERE IdOrder = o.Id)
					OR
					3 = ANY(SELECT IdObjectType FROM productTypes AS so
							WHERE IdOrder = o.Id)
				)
				AND 
				2 = ANY(SELECT IdObjectType FROM productTypes AS so
						WHERE IdOrder = o.Id)
				THEN 3
			WHEN 2 = ANY(SELECT IdObjectType FROM productTypes AS so
						WHERE IdOrder = o.Id) 
				THEN 1
			ELSE 2
		END 
		AS NVARCHAR(250)
	)
FROM Orders AS o
PRINT '====POrderType value'

GO

INSERT INTO RefundSkus
(IdOrder, IdSku, Quantity, Redeem, RefundPercent, RefundPrice, RefundValue)
SELECT o.Id, s.Id, ri.Qty, 1, ri.ProductPercent, ri.Value / (ri.ProductPercent / 100.0), ri.Value FROM Orders AS o
INNER JOIN [vitalchoice2.0].dbo.refund AS r ON r.idOrder = o.Id
INNER JOIN [vitalchoice2.0].dbo.RefundItems AS ri ON ri.ItemType = 1 AND ri.IdRefundOrder = o.Id
INNER JOIN Skus AS s ON s.Id = ri.IdItem
WHERE o.IdObjectType = 6
PRINT '====refund skus'

GO

INSERT INTO RefundSkus
(IdOrder, IdSku, Quantity, Redeem, RefundPercent, RefundPrice, RefundValue)
SELECT o.Id, s.Id, ri.Qty, 1, ri.ProductPercent, ri.Value / (ri.ProductPercent / 100.0), ri.Value FROM Orders AS o
INNER JOIN [vitalchoice2.0].dbo.refund AS r ON r.idOrder = o.Id
INNER JOIN [vitalchoice2.0].dbo.RefundItems AS ri ON ri.ItemType = 1 AND ri.IdRefundOrder = o.Id
INNER JOIN [vitalchoice2.0].dbo.products AS p ON p.IdProduct = ri.IdItem
INNER JOIN Skus AS s ON s.StatusCode <> 3 AND s.Code = p.sku
WHERE o.IdObjectType = 6 AND ri.IdItem NOT IN (SELECT Id FROM SKus)
PRINT '====refund skus(missed skus)'

GO

INSERT INTO ReshipProblemSkus
(IdOrder, IdSku)
SELECT o.Id, so.IdSku FROM Orders AS o
INNER JOIN [vitalchoice2.0].dbo.reship AS r ON r.idOrder = o.Id
INNER JOIN OrderToSkus AS so ON so.IdOrder = o.Id
WHERE r.problemSku = 'All'
PRINT '====reship problem skus'

GO

INSERT INTO ReshipProblemSkus
(IdOrder, IdSku)
SELECT o.Id, s.Id FROM Orders AS o
INNER JOIN [vitalchoice2.0].dbo.reship AS r ON r.idOrder = o.Id
INNER JOIN [vitalchoice2.0].dbo.Orders AS oo ON oo.idOrder = o.Id
INNER JOIN [vitalchoice2.0].dbo.ProductsOrdered AS po ON po.IdOrder = r.idOrderOriginal
INNER JOIN [vitalchoice2.0].dbo.Products AS p ON p.idProduct = po.idProduct
INNER JOIN Skus AS s ON s.Id = p.idProduct
WHERE r.problemSku <> 'All' AND p.sku = r.problemSku AND s.Id NOT IN (SELECT IdSku FROM ReshipProblemSkus)
PRINT '====reship problem skus (individual)'

GO

INSERT INTO ReshipProblemSkus
(IdOrder, IdSku)
SELECT o.Id, s.Id FROM Orders AS o
INNER JOIN [vitalchoice2.0].dbo.reship AS r ON r.idOrder = o.Id
INNER JOIN Skus AS s ON s.Code = r.problemSku AND s.StatusCode <> 3
WHERE r.problemSku <> 'All' AND s.Id NOT IN (SELECT IdSku FROM ReshipProblemSkus)
PRINT '====reship problem skus (individual)'

GO

SET IDENTITY_INSERT HealthwisePeriods ON;

INSERT INTO HealthwisePeriods
(Id, IdCustomer, PaidAmount, PaidDate, StartDate, EndDate)
SELECT 
	hw.id, 
	hw.customerId, 
	hw.billAmount, 
	hw.BillDate, 
	hw.date, 
	CASE WHEN hw.BillDate IS NOT NULL AND DATEADD(YEAR, 1, hw.date) > hw.BillDate THEN hw.BillDate ELSE DATEADD(YEAR, 1, hw.date) END 
FROM [vitalchoice2.0].dbo.healthwise AS hw
INNER JOIN Customers AS c ON c.Id = hw.customerId
WHERE hw.date IS NOT NULL

SET IDENTITY_INSERT HealthwisePeriods OFF;
PRINT '====healthwise periods'

INSERT INTO HealthwiseOrders
(Id, IdHealthwisePeriod)
SELECT o.Id, hw.Id FROM Orders AS o
INNER JOIN [vitalchoice2.0].dbo.orders AS oo ON oo.idOrder = o.Id
INNER JOIN HealthwisePeriods AS hw ON hw.Id = oo.healthwiseId
PRINT '====healthwise orders'

GO

ALTER TABLE AffiliatePayments
ADD IdOrder INT NOT NULL 
CONSTRAINT UQ_AffiliatePaymentsOrder UNIQUE (IdOrder)

GO

INSERT INTO AffiliatePayments
(IdAffiliate, Amount, DateCreated, IdOrder)
SELECT a.Id, o.affiliatePayReport, o.orderDate, oo.Id 
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.IdOrder
INNER JOIN Affiliates AS a ON a.Id = o.idAffiliate
WHERE o.affiliatePayReport > 0 AND o.affiliatePayReport > ISNULL(o.affiliatePay, 0)
PRINT '====affiliate payments'

GO

INSERT INTO AffiliateOrderPayments
(Id, IdAffiliate, IdAffiliatePayment, Amount, NewCustomerOrder, Status)
SELECT 
	oo.Id, 
	a.Id, 
	ap.Id, 
	CASE WHEN o.affiliatePayReport = 0 THEN ISNULL(o.affiliatePay, 0) ELSE o.affiliatePayReport END, 
	(SELECT CASE WHEN o.idOrder <=
	ALL(
		SELECT idOrder 
		FROM [vitalchoice2.0].dbo.orders AS oox 
		WHERE oox.idCustomer = o.idCustomer
		AND oox.idAffiliate = o.idAffiliate
		AND oox.orderStatus NOT IN (1,5,96,97)
		AND oox.IsReship = 0
		AND oox.IsRefund = 0
	) THEN 1 ELSE 0 END), 
	CASE WHEN ap.Id IS NULL THEN 1 ELSE 2 END
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.IdOrder
INNER JOIN Affiliates AS a ON a.Id = o.idAffiliate
LEFT JOIN AffiliatePayments AS ap ON ap.IdOrder = oo.Id
WHERE CASE WHEN o.affiliatePayReport = 0 THEN ISNULL(o.affiliatePay, 0) ELSE o.affiliatePayReport END > 0
PRINT '====affiliate order payments'

GO

ALTER TABLE AffiliatePayments
DROP CONSTRAINT UQ_AffiliatePaymentsOrder

GO

ALTER TABLE AffiliatePayments
DROP COLUMN IdOrder

GO

UPDATE GiftCertificates
SET IdOrder = o.Id
FROM GiftCertificates AS g
INNER JOIN [vitalchoice2.0].dbo.pcGCOrdered AS gc ON gc.pcGO_GcCode = g.Code
INNER JOIN Orders AS o ON o.Id = gc.pcGO_IDOrder
PRINT '====set up created gift certificates order ids'

GO

DELETE v
FROM OrderOptionValues AS v
INNER JOIN OrderOptionTypes AS t ON t.Id = v.IdOptionType
INNER JOIN Orders AS o ON o.Id = v.IdOrder
WHERE t.DefaultValue = v.Value AND (o.IdObjectType <> t.IdObjectType AND t.IdObjectType IS NOT NULL)
PRINT '====default values fixup (remove)'

GO

MERGE INTO OrderOptionValues AS tv
USING (
	SELECT c.Id AS IdOrder, t.Id, t.DefaultValue FROM Orders AS c
	INNER JOIN OrderOptionTypes AS t ON t.IdObjectType = c.IdObjectType OR t.IdObjectType IS NULL
	WHERE t.DefaultValue IS NOT NULL
) AS src ON tv.IdOrder = src.IdOrder AND tv.IdOptionType = src.Id
WHEN NOT MATCHED THEN
INSERT (IdOrder, IdOptionType, Value) VALUES (src.IdOrder, src.Id, src.DefaultValue);
PRINT '====default values fixup (insert)'

--INSERT INTO OrderOptionValues
--(IdOrder, IdOptionType, Value)
--SELECT c.Id, t.Id, t.DefaultValue FROM Orders AS c
--INNER JOIN OrderOptionTypes AS t ON t.IdObjectType = c.IdObjectType OR t.IdObjectType IS NULL
--WHERE t.DefaultValue IS NOT NULL AND NOT EXISTS(SELECT * FROM OrderOptionValues AS v WHERE v.IdOrder = c.Id AND v.IdOptionType = t.Id)

GO

INSERT INTO OrderShippingPackages
(IdOrder, IdSku, DateCreated, IdWarehouse, POrderType, ShipMethodFreightCarrier, ShipMethodFreightService, ShippedDate, TrackingNumber, UPSServiceCode)
SELECT 
	pki.idOrder, 
	s.Id, 
	MAX(pki.pcPackageInfo_ShippedDate), 
	CASE WHEN MAX(pki.Warehouse) = 'VA' THEN 2 ELSE 1 END, 
	NULL, 
	(SELECT TOP 1 RTRIM(k.Item) FROM [vitalchoice2.0].[dbo].[DelimitedSplit8K](MAX(pki.pcPackageInfo_ShipMethod), '-') AS k ORDER BY k.ItemNumber ASC), 
	(SELECT TOP 1 LTRIM(k.Item) FROM [vitalchoice2.0].[dbo].[DelimitedSplit8K](MAX(pki.pcPackageInfo_ShipMethod), '-') AS k ORDER BY k.ItemNumber DESC),
	MAX(pki.pcPackageInfo_ShippedDate), 
	MAX(pki.pcPackageInfo_TrackingNumber),
	MAX(pki.pcPackageInfo_UPSServiceCode)
FROM [vitalchoice2.0].dbo.pcPackageInfo AS pki
INNER JOIN [vitalchoice2.0].dbo.ProductsOrdered AS po ON po.idOrder = pki.idOrder
INNER JOIN [vitalchoice2.0].dbo.products AS p ON p.idProduct = po.idProduct AND p.sku = pki.pcPackageInfo_UPSServiceCode
INNER JOIN Skus AS s ON s.Id = p.idProduct
WHERE pki.pcPackageInfo_ShippedDate IS NOT NULL
GROUP BY pki.idOrder, s.Id
PRINT '====shipping packages'

GO

INSERT INTO [VitalChoice.ExportInfo].dbo.OrderPaymentMethods
(IdOrder, CreditCardNumber)
SELECT 
	oo.Id, 
	CAST([vitalchoice2.0].dbo.RC4Encode(cc.cardnumber) AS VARBINARY)
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN OrderPaymentMethods AS a ON a.Id = oo.IdPaymentMethod AND a.IdObjectType = 1
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
WHERE cc.cardnumber IS NOT NULL
PRINT '====credit cards (export)'

GO

INSERT INTO [VitalChoice.ExportInfo].dbo.OrderPaymentMethods
(IdOrder, CreditCardNumber)
SELECT 
	oo.Id, 
	CAST([vitalchoice2.0].dbo.RC4Encode(cc.cardnumber) AS VARBINARY)
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
INNER JOIN OrderPaymentMethods AS a ON a.Id = oo.IdPaymentMethod AND a.IdObjectType = 1
WHERE cc.cardnumber IS NOT NULL
PRINT '====credit cards(auto-ship) (export)'
GO

INSERT INTO [VitalChoice.ExportInfo].dbo.OrderPaymentMethods
(IdOrder, CreditCardNumber)
SELECT 
	oo.Id, 
	CAST([vitalchoice2.0].dbo.RC4Encode(cc.ccnum) AS VARBINARY)
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN OrderPaymentMethods AS a ON a.Id = oo.IdPaymentMethod AND a.IdObjectType = 1
INNER JOIN [vitalchoice2.0].dbo.authorders AS cc ON cc.idauthorder = (SELECT TOP 1 idauthorder FROM [vitalchoice2.0].dbo.authorders AS ac WHERE ac.idOrder = o.idOrder ORDER BY idauthorder DESC)
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS oc ON oc.idOrder = o.idOrder
WHERE oc.idOrder IS NULL AND cc.ccnum IS NOT NULL
PRINT '====credit cards (export)'

GO

INSERT INTO [VitalChoice.ExportInfo].dbo.OrderPaymentMethods
(IdOrder, CreditCardNumber)
SELECT 
	oo.Id, 
	CAST([vitalchoice2.0].dbo.RC4Encode(cc.ccnum) AS VARBINARY)
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN OrderPaymentMethods AS a ON a.Id = oo.IdPaymentMethod AND a.IdObjectType = 1
INNER JOIN [vitalchoice2.0].dbo.authorders AS cc ON cc.idauthorder = (SELECT TOP 1 idauthorder FROM [vitalchoice2.0].dbo.authorders AS ac WHERE ac.idOrder = o.idOrder ORDER BY idauthorder DESC)
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS oc ON oc.idOrder = o.idOrder
WHERE oc.idOrder IS NULL AND cc.ccnum IS NOT NULL
PRINT '====credit cards(auto-ship) (export)'

GO

GO

DROP INDEX IX_OrdersIdAutoShipOrder ON Orders

GO

ALTER TABLE Orders
DROP COLUMN IdAutoShipOrder

GO

USE [vitalchoice2.0]
GO

ALTER TABLE [VitalChoice.Infrastructure].dbo.ContentItems
ADD TempId INT NULL,
	TempCategoryId INT NULL

GO

DECLARE @contentType INT, @oldContentType INT, @masterName NVARCHAR(50), @categoryMasterName NVARCHAR(50)
SET @contentType = 3
SET @oldContentType = 2
SET @masterName = N'Article Individual'
SET @categoryMasterName = N'Article Sub Category'

DECLARE @articlesToImport TABLE (Id INT NOT NULL PRIMARY KEY)

INSERT INTO @articlesToImport
(Id)
SELECT a.ID 
FROM [vitalchoice2.0].[dbo].Articles AS a 
WHERE a.ID NOT IN (SELECT IdOld FROM [VitalChoice.Infrastructure].dbo.Articles WHERE idOld IS NOT NULL)

DECLARE @articleMasterId INT
SELECT @articleMasterId = Id FROM [VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = @masterName

INSERT [VitalChoice.Infrastructure].dbo.ContentItems
(Created, Description, MetaDescription, MetaKeywords, Title, Updated, TempId, Template)
SELECT CAST(ArticlesDate AS DATE), ISNULL(ArticlesDescription, N''), MetaDescription, NULL, MetaTitle, GETDATE(), Id, N''
FROM [vitalchoice2.0].[dbo].Articles AS a
WHERE a.ID IN (SELECT Id FROM @articlesToImport)
ORDER BY a.ID

INSERT INTO [VitalChoice.Infrastructure].dbo.Articles
(Author, ContentItemId, MasterContentItemId, Name, PublishedDate, StatusCode, SubTitle, Url, FileUrl, IdOld)
SELECT a.ArticlesAuthor, i.Id, @articleMasterId, a.ArticlesTitle, CAST(a.ArticlesDate AS DATE), 2/*Active*/, a.ArticlesSubTitle, REPLACE(RTRIM(LTRIM(LOWER([vitalchoice2.0].[dbo].RegexReplace('[^a-zA-Z0-9]+', a.ArticlesTitle, ' ')))),' ','-'), a.articlesImage, a.ID 
FROM [vitalchoice2.0].[dbo].Articles AS a
INNER JOIN [VitalChoice.Infrastructure].dbo.ContentItems AS i ON i.TempId = a.ID
WHERE a.ID IN (SELECT Id FROM @articlesToImport)

INSERT [VitalChoice.Infrastructure].dbo.ArticlesToContentCategories
(ArticleId, ContentCategoryId)
SELECT a.Id, c.Id FROM [VitalChoice.Infrastructure].dbo.Articles AS a
INNER JOIN [vitalchoice2.0].dbo.categories_articles AS ca ON ca.idArticle = a.IdOld
INNER JOIN [VitalChoice.Infrastructure].dbo.ContentCategories AS c ON c.IdOld = ca.idCategory
WHERE a.Id IN (SELECT Id FROM @articlesToImport)

GO
ALTER TABLE [VitalChoice.Infrastructure].dbo.ContentItems
DROP COLUMN TempId, COLUMN TempCategoryId

GO

UPDATE [VitalChoice.Infrastructure].dbo.Articles
SET Url = r.Url + N'-' + CAST(j.Number AS NVARCHAR(10))
FROM [VitalChoice.Infrastructure].dbo.Articles AS r
INNER JOIN 
(
	SELECT ROW_NUMBER() OVER (PARTITION BY r.Url ORDER BY r.Id) AS Number, r.Id
	FROM [VitalChoice.Infrastructure].dbo.Articles AS r
	WHERE r.Url IN 
	(
		SELECT Url FROM [VitalChoice.Infrastructure].dbo.Articles
		GROUP BY Url
		HAVING COUNT(Url) > 1
	)
) AS j ON j.Id = r.Id
WHERE j.Number > 1

UPDATE [VitalChoice.Infrastructure].dbo.Articles
SET FileUrl = REPLACE(REPLACE(REPLACE(c.FileUrl, 'http://www.vitalchoice.com/shop/pc/catalog/', '/files/catalog/'), '/shop/pc/catalog/', '/files/catalog/'), '../pc/catalog/', '/files/catalog/')
FROM [VitalChoice.Infrastructure].dbo.Articles AS c
WHERE c.FileUrl LIKE '%/catalog/%'

GO

USE [vitalchoice2.0]
GO

IF OBJECT_ID('dbo.ReplaceUrl') IS NOT NULL
BEGIN
DROP FUNCTION dbo.ReplaceUrl
END
GO
CREATE FUNCTION dbo.ReplaceUrl
(@text NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
	RETURN REPLACE(REPLACE(REPLACE(ISNULL(@text, N''), 'http://www.vitalchoice.com/shop/pc/catalog/', '/files/catalog/'), '/shop/pc/catalog/', '/files/catalog/'), '../pc/catalog/', '/files/catalog/')
END
GO

GO
ALTER TABLE [VitalChoice.Infrastructure].dbo.ContentItems
ADD TempId INT NULL,
	TempCategoryId INT NULL
GO

DECLARE @contentType INT, @masterName NVARCHAR(50), @categoryMasterName NVARCHAR(50)
SET @contentType = 5
SET @masterName = N'FAQ Individual'
SET @categoryMasterName = N'FAQ Sub Category'

--============================= Clean ===========================
DECLARE @contentItemsToImport TABLE(Id INT)

INSERT INTO @contentItemsToImport
(Id)
SELECT a.idFAQ
FROM [vitalchoice2.0].[dbo].FAQ AS a 
WHERE a.idFAQ NOT IN (SELECT IdOld FROM [VitalChoice.Infrastructure].dbo.FAQs WHERE idOld IS NOT NULL)

DELETE FROM [VitalChoice.Infrastructure].dbo.Faqs

DECLARE @masterId INT
SELECT @masterId = Id FROM [VitalChoice.Infrastructure].dbo.MasterContentItems WHERE Name = @masterName

--============================= Faqs ===========================

INSERT [VitalChoice.Infrastructure].dbo.ContentItems
(Created, Description, MetaDescription, MetaKeywords, Title, Updated, TempId, Template)
SELECT GETDATE(), dbo.ReplaceUrl(content), N'', NULL, title, GETDATE(), idFAQ, N''
FROM [vitalchoice2.0].[dbo].FAQ AS a
WHERE a.idFAQ IN (SELECT Id FROM @contentItemsToImport)
ORDER BY a.idFAQ

INSERT INTO [VitalChoice.Infrastructure].dbo.Faqs
(ContentItemId, MasterContentItemId, Name, StatusCode, Url, IdOld)
SELECT i.Id, @masterId, a.title, 2/*Active*/, REPLACE(RTRIM(LTRIM(LOWER([vitalchoice2.0].[dbo].RegexReplace('[^a-zA-Z0-9]+', a.title, ' ')))),' ','-'), a.idFAQ 
FROM [vitalchoice2.0].[dbo].FAQ AS a
INNER JOIN [VitalChoice.Infrastructure].dbo.ContentItems AS i ON i.TempId = a.idFAQ
WHERE a.idFAQ IN (SELECT Id FROM @contentItemsToImport)

INSERT [VitalChoice.Infrastructure].dbo.FaqsToContentCategories
(FAQId, ContentCategoryId)
SELECT a.Id, c.Id FROM [VitalChoice.Infrastructure].dbo.Faqs AS a
INNER JOIN [vitalchoice2.0].dbo.FAQtoSLC AS ca ON ca.idFAQ = a.IdOld
INNER JOIN [VitalChoice.Infrastructure].dbo.ContentCategories AS c ON c.IdOld = ca.idSLC
WHERE ca.idFAQ IN (SELECT Id FROM @contentItemsToImport)

GO

UPDATE [VitalChoice.Infrastructure].dbo.Faqs
SET Url = r.Url + N'-' + CAST(j.Number AS NVARCHAR(10))
FROM [VitalChoice.Infrastructure].dbo.Faqs AS r
INNER JOIN 
(
	SELECT ROW_NUMBER() OVER (PARTITION BY r.Url ORDER BY r.Id) AS Number, r.Id
	FROM [VitalChoice.Infrastructure].dbo.Faqs AS r
	WHERE r.Url IN 
	(
		SELECT Url FROM [VitalChoice.Infrastructure].dbo.Faqs
		GROUP BY Url
		HAVING COUNT(Url) > 1
	)
) AS j ON j.Id = r.Id
WHERE j.Number > 1

UPDATE [VitalChoice.Infrastructure].dbo.ContentCategories
SET Url = r.Url + N'-' + CAST(j.Number AS NVARCHAR(10))
FROM [VitalChoice.Infrastructure].dbo.ContentCategories AS r
INNER JOIN 
(
	SELECT ROW_NUMBER() OVER (PARTITION BY r.Url ORDER BY r.Id) AS Number, r.Id
	FROM [VitalChoice.Infrastructure].dbo.ContentCategories AS r
	WHERE r.Url IN 
	(
		SELECT Url FROM [VitalChoice.Infrastructure].dbo.ContentCategories
		WHERE Type = 5
		GROUP BY Url
		HAVING COUNT(Url) > 1
	) AND Type = 5
) AS j ON j.Id = r.Id
WHERE j.Number > 1 AND r.Type = 5

GO

SELECT Url FROM [VitalChoice.Infrastructure].dbo.Faqs
GROUP BY Url
HAVING COUNT(Url) > 1

SELECT Url FROM [VitalChoice.Infrastructure].dbo.ContentCategories
WHERE Type = 5
GROUP BY Url
HAVING COUNT(Url) > 1

GO
IF OBJECT_ID('dbo.ReplaceUrl') IS NOT NULL
BEGIN
DROP FUNCTION dbo.ReplaceUrl
END

GO

ALTER TABLE [VitalChoice.Infrastructure].dbo.ContentItems
DROP COLUMN TempId, COLUMN TempCategoryId
GO