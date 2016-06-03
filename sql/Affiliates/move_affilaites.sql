USE [VitalChoice.Ecommerce]

--============================ Wipe Customers ====================================
DELETE FROM AffiliateOrderPayments
DELETE FROM CartToSkus
DELETE FROM Carts
DELETE FROM OrderToSkus
DELETE FROM RefundOrderToGiftCertificates
DELETE FROM CartToGiftCertificates
DELETE FROM OrderToGiftCertificates
UPDATE GiftCertificates
SET IdOrder = NULL
DELETE FROM HealthwiseOrders
DELETE FROM HelpTicketComments
DELETE FROM HelpTickets
DELETE FROM RefundSkus
DELETE FROM ReshipProblemSkus
DELETE FROM OrderToPromos
DELETE FROM OrderOptionValues
DELETE FROM Orders
DELETE FROM OrderPaymentMethodOptionValues
DELETE FROM OrderPaymentMethods
DELETE FROM OrderAddressOptionValues
DELETE FROM OrderAddresses
DELETE FROM CustomerToShippingAddresses
DELETE FROM CustomersToPaymentMethods
DELETE FROM CustomersToOrderNotes
DELETE FROM CustomerPaymentMethodValues
DELETE FROM CustomerPaymentMethods
DELETE FROM CustomerNoteOptionValues
DELETE FROM CustomerNotes
DELETE FROM OneTimeDiscountToCustomerUsages
DELETE FROM HealthwisePeriods
DELETE FROM BigStringValues
WHERE IdBigString IN (SELECT cv.IdBigString FROM CustomerOptionValues AS cv WHERE cv.IdBigString IS NOT NULL)
DELETE FROM CustomerOptionValues
DELETE FROM CustomerFiles
DELETE FROM Customers
DELETE FROM AddressOptionValues
DELETE FROM Addresses
DELETE FROM Users WHERE Id IN (SELECT id FROM [VitalChoice.Infrastructure].dbo.AspNetUsers WHERE IdUserType = 2)
GO
USE [VitalChoice.Infrastructure]
GO
DELETE FROM AspNetUserClaims
WHERE UserId IN (SELECT id FROM AspNetUsers WHERE IdUserType = 2)
GO
DELETE FROM AspNetUserLogins
WHERE UserId IN (SELECT id FROM AspNetUsers WHERE IdUserType = 2)
GO
DELETE FROM AspNetUserRoles
WHERE UserId IN (SELECT id FROM AspNetUsers WHERE IdUserType = 2)
GO
DELETE FROM AspNetUsers
WHERE IdUserType = 2
GO

--wipe out affiliates

USE [VitalChoice.Ecommerce]
GO

DELETE FROM [VitalChoice.Ecommerce].dbo.Users WHERE Id > 1500

DELETE FROM Users WHERE Id IN (SELECT id FROM [VitalChoice.Infrastructure].dbo.AspNetUsers WHERE IdUserType = 3)

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUserClaims
WHERE UserId IN (SELECT Id FROM [VitalChoice.Infrastructure].dbo.AspNetUsers WHERE IdUserType = 3)

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUserLogins
WHERE UserId IN (SELECT Id FROM [VitalChoice.Infrastructure].dbo.AspNetUsers WHERE IdUserType = 3)

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUserRoles
WHERE UserId IN (SELECT Id FROM [VitalChoice.Infrastructure].dbo.AspNetUsers WHERE IdUserType = 3)

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUsers
WHERE IdUserType = 3

DELETE FROM [VitalChoice.Ecommerce].dbo.Users
WHERE Id IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.Affiliates)

DELETE FROM [VitalChoice.Ecommerce].dbo.BigStringValues
WHERE IdBigString IN (SELECT ao.IdBigString FROM [VitalChoice.Ecommerce].dbo.AffiliateOptionValues AS ao WHERE ao.IdBigString IS NOT NULL)

DELETE FROM [VitalChoice.Ecommerce].dbo.AffiliateOptionValues

DELETE FROM [VitalChoice.Ecommerce].dbo.AffiliatePayments

DELETE FROM [VitalChoice.Ecommerce].dbo.AffiliateOrderPayments

UPDATE [VitalChoice.Ecommerce].dbo.Customers
SET IdAffiliate = NULL

DELETE FROM [VitalChoice.Ecommerce].dbo.Affiliates

GO

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUserRoles WHERE UserId < 1000
DELETE FROM [VitalChoice.Infrastructure].dbo.AdminProfiles WHERE Id < 1000
DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUsers WHERE Id < 1000
DELETE FROM [VitalChoice.Ecommerce].dbo.Users WHERE Id < 1000

GO

USE [VitalChoice.Ecommerce]

GO

DELETE FROM CatalogRequestAddressOptionValues
DELETE FROM CatalogRequestAddresses
DELETE FROM States
DELETE FROM Countries

INSERT INTO Countries
(CountryCode, CountryName, [Order], StatusCode)
SELECT c.countryCode, c.countryName, ROW_NUMBER() OVER(ORDER BY c.countryCode), 2 FROM [vitalchoice2.0].dbo.countries AS c

INSERT INTO States
(CountryCode, StateCode, StateName, StatusCode, [Order])
SELECT c.pcCountryCode, c.stateCode, c.stateName, 2, ROW_NUMBER() OVER(ORDER BY c.stateCode) FROM [vitalchoice2.0].dbo.states AS c
INNER JOIN Countries AS cc ON cc.CountryCode = c.pcCountryCode COLLATE Cyrillic_General_CI_AS

INSERT INTO Countries
(CountryCode, CountryName, [Order], StatusCode)
SELECT CASE WHEN c.countryName = 'Taiwan' THEN 'TW' ELSE c.countryCode END, c.countryName, ROW_NUMBER() OVER(ORDER BY c.countryCode), 1 FROM [vitalchoice2.0].dbo.countriesCSPortal AS c
WHERE c.countryCode NOT IN (SELECT cc.countryCode FROM [vitalchoice2.0].dbo.countries AS cc)

INSERT INTO States
(CountryCode, StateCode, StateName, StatusCode, [Order])
SELECT c.pcCountryCode, c.stateCode, c.stateName, 1, ROW_NUMBER() OVER(ORDER BY c.stateCode) FROM [vitalchoice2.0].dbo.statesCSPortal AS c
INNER JOIN Countries AS cc ON cc.CountryCode = c.pcCountryCode COLLATE Cyrillic_General_CI_AS
WHERE c.pcCountryCode NOT IN (SELECT s.pcCountryCode FROM [vitalchoice2.0].dbo.states AS s)

GO

--============================ Move all admin users to lower id space (from 1) ====================================
DECLARE @aspnetUsers TABLE (
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
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
SELECT * FROM [VitalChoice.Infrastructure].dbo.AspNetUsers
WHERE IdUserType = 1

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
SET UserId = u.Id
FROM [VitalChoice.Infrastructure].dbo.MasterContentItems AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.UserId

UPDATE [VitalChoice.Infrastructure].dbo.MasterContentItems
SET UserId = u.Id
FROM [VitalChoice.Infrastructure].dbo.MasterContentItems AS b
INNER JOIN @aspnetUsers AS u ON u.IdOld = b.UserId

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
	WHERE IdUserType = 1 AND Id NOT IN (SELECT Id FROM @aspnetUsers)
)

DELETE FROM [VitalChoice.Ecommerce].dbo.Users
WHERE Id IN (
	SELECT Id FROM [VitalChoice.Infrastructure].dbo.AspNetUsers
	WHERE IdUserType = 1 AND Id NOT IN (SELECT Id FROM @aspnetUsers)
)

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUsers
WHERE IdUserType = 1 AND Id NOT IN (SELECT Id FROM @aspnetUsers)

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
PasswordHash)
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
	[vitalchoice2.0].dbo.HashPassword([vitalchoice2.0].dbo.RC4Encode(aff.pcaff_Password))
FROM [vitalchoice2.0].dbo.affiliates AS aff

SET IDENTITY_INSERT [VitalChoice.Infrastructure].dbo.AspNetUsers OFF

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Affiliates
DROP COLUMN IdOld

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
	ISNULL((SELECT TOP 1 c.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS c WHERE c.CountryCode COLLATE SQL_Latin1_General_CP1_CI_AS = a.affiliatecountryCode), (SELECT TOP 1 c.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS c WHERE c.CountryCode = 'US')), 
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode COLLATE SQL_Latin1_General_CP1_CI_AS = a.affiliatecountryCode AND s.StateCode COLLATE SQL_Latin1_General_CP1_CI_AS = a.affiliateState), 
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

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
ADD IdCustomer INT NOT NULL CONSTRAINT UQ_Customers UNIQUE
GO

INSERT INTO [VitalChoice.Ecommerce].dbo.Addresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdCustomer)
SELECT 
	ISNULL(pcCust_DateCreated, GETDATE()), 
	ISNULL(lastEditDate, ISNULL(pcCust_DateCreated, GETDATE())),
	ISNULL (
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = ISNULL(c.countryCode COLLATE Cyrillic_General_CI_AS, 'US')), 
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode = ISNULL(c.countryCode COLLATE Cyrillic_General_CI_AS, 'US') AND s.StateCode = c.stateCode COLLATE Cyrillic_General_CI_AS),
	2,
	1,
	c.[state],
	c.idcustomer
FROM [vitalchoice2.0].dbo.customers AS c

INSERT INTO [VitalChoice.Ecommerce].dbo.Users
(Id)
SELECT idCustomer FROM customers
	
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
GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
DROP CONSTRAINT UQ_Customers

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
DROP COLUMN IdCustomer
--============================ Insert Fields ====================================

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

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'HasHealthwiseOrders' AND IdObjectType = 1

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT idCustomer, @fieldType, N'True'
FROM customers AS c
INNER JOIN healthwise AS hw ON hw.customerId = c.idcustomer
WHERE c.customerType <> 1
GROUP BY c.idcustomer

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
INNER JOIN [VitalChoice.Ecommerce].dbo.LookupVariants AS l ON IdLookup = @lookupId AND ValueVariant = t.Name COLLATE Cyrillic_General_CI_AS
WHERE c.customerType = 1 AND l.Id <> 1

GO

DECLARE @fieldType INT, @lookupId INT

SELECT TOP 1 @fieldType = Id FROM [VitalChoice.Ecommerce].dbo.CustomerOptionTypes WHERE Name = N'Source'
SELECT TOP 1 @lookupId = Id FROM [VitalChoice.Ecommerce].dbo.Lookups WHERE Name = N'OrderSources'

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerOptionValues
(IdCustomer, IdOptionType, Value)
SELECT c.idCustomer, @fieldType, CAST(l.Id AS NVARCHAR(3))
FROM customers AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.LookupVariants AS l ON IdLookup = @lookupId AND ValueVariant = c.source COLLATE Cyrillic_General_CI_AS
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
INNER JOIN [VitalChoice.Ecommerce].dbo.AddressOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 1)

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
ADD IdCustomer INT NULL
GO

INSERT INTO [VitalChoice.Ecommerce].dbo.Addresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdCustomer)
SELECT 
	ISNULL(pcCust_DateCreated, GETDATE()), 
	ISNULL(lastEditDate, ISNULL(pcCust_DateCreated, GETDATE())),
	ISNULL (
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = ISNULL(c.shippingCountryCode COLLATE Cyrillic_General_CI_AS, 'US')), 
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode = ISNULL(c.shippingCountryCode COLLATE Cyrillic_General_CI_AS, 'US') AND s.StateCode = c.shippingStateCode COLLATE Cyrillic_General_CI_AS),
	2,
	3,
	c.[shippingState],
	c.idcustomer
FROM [vitalchoice2.0].dbo.customers AS c

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerToShippingAddresses
(IdAddress, IdCustomer)
SELECT Id, IdCustomer FROM [VitalChoice.Ecommerce].dbo.Addresses AS a WHERE IdCustomer IS NOT NULL

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
INNER JOIN [VitalChoice.Ecommerce].dbo.AddressOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 3)

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
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = ISNULL(c.recipient_CountryCode COLLATE Cyrillic_General_CI_AS, 'US')), 
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode = ISNULL(c.recipient_CountryCode COLLATE Cyrillic_General_CI_AS, 'US') AND s.StateCode = c.recipient_StateCode COLLATE Cyrillic_General_CI_AS),
	2,
	3,
	c.[recipient_State],
	c.idcustomer,
	c.idRecipient
FROM [vitalchoice2.0].dbo.recipients AS c
INNER JOIN [vitalchoice2.0].dbo.customers AS cc ON cc.idcustomer = c.idCustomer

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerToShippingAddresses
(IdAddress, IdCustomer)
SELECT Id, IdCustomer FROM [VitalChoice.Ecommerce].dbo.Addresses AS a WHERE IdCustomer IS NOT NULL

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
INNER JOIN [VitalChoice.Ecommerce].dbo.AddressOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 3)

GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
DROP COLUMN IdCustomer, idRecipient

GO

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.CustomerNotes ON;

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerNotes
(Id, DateCreated, DateEdited, IdCustomer, Note, StatusCode)
SELECT n.id, n.createdDate, n.createdDate, n.idCustomer, n.description, 2 FROM [vitalchoice2.0].dbo.Notes AS n
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS c ON c.Id = n.idCustomer

SET IDENTITY_INSERT [VitalChoice.Ecommerce].dbo.CustomerNotes OFF;

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerNoteOptionValues
(IdCustomerNote, IdOptionType, Value)
SELECT n.id, t.Id, CAST(lv.Id AS NVARCHAR(100)) FROM [vitalchoice2.0].dbo.Notes AS n
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS c ON c.Id = n.idCustomer
INNER JOIN [VitalChoice.Ecommerce].dbo.CustomerNoteOptionTypes AS t ON t.Name = 'Priority'
INNER JOIN [VitalChoice.Ecommerce].dbo.LookupVariants AS lv ON lv.IdLookup = t.IdLookup AND lv.ValueVariant = n.priority COLLATE Cyrillic_General_CI_AS

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

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
ADD IdCustomer INT NULL, IdCreditCard INT NULL


GO

INSERT INTO [VitalChoice.Ecommerce].dbo.Addresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdCustomer, IdCreditCard)
SELECT 
	ISNULL(pcCust_DateCreated, GETDATE()), 
	ISNULL(lastEditDate, ISNULL(pcCust_DateCreated, GETDATE())),
	ISNULL (
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = ISNULL(c.countryCode COLLATE Cyrillic_General_CI_AS, 'US')), 
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode = ISNULL(c.countryCode COLLATE Cyrillic_General_CI_AS, 'US') AND s.StateCode = c.stateCode COLLATE Cyrillic_General_CI_AS),
	2,
	2,
	c.state,
	c.idcustomer,
	c.custCreditCardId
FROM [vitalchoice2.0].dbo.customerCreditCards AS c
INNER JOIN [vitalchoice2.0].dbo.customers AS cc ON cc.idcustomer = c.idCustomer

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
INNER JOIN [VitalChoice.Ecommerce].dbo.AddressOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerPaymentMethods
(DateCreated, DateEdited, IdAddress, IdCustomer, IdObjectType, StatusCode)
SELECT c.DateCreated, c.DateEdited, a.Id, c.Id, 1, 2 FROM [VitalChoice.Ecommerce].dbo.Customers AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.IdCustomer = c.Id

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
INNER JOIN [VitalChoice.Ecommerce].dbo.CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 1)

GO

DELETE FROM [VitalChoice.Ecommerce].dbo.CustomerPaymentMethodValues
WHERE Value IS NULL OR Value = ''

DELETE FROM [VitalChoice.Ecommerce].dbo.AddressOptionValues
WHERE Value IS NULL OR Value = ''

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
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = ISNULL(c.countryCode COLLATE Cyrillic_General_CI_AS, 'US')), 
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode = ISNULL(c.countryCode COLLATE Cyrillic_General_CI_AS, 'US') AND s.StateCode = c.stateCode COLLATE Cyrillic_General_CI_AS),
	2,
	2,
	c.state,
	c.idcustomer
FROM [vitalchoice2.0].dbo.CustomerChecks AS c
INNER JOIN [vitalchoice2.0].dbo.customers AS cc ON cc.idcustomer = c.idCustomer

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
INNER JOIN [VitalChoice.Ecommerce].dbo.AddressOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerPaymentMethods
(DateCreated, DateEdited, IdAddress, IdCustomer, IdObjectType, StatusCode)
SELECT c.DateCreated, c.DateEdited, a.Id, c.Id, 3, 2 FROM [VitalChoice.Ecommerce].dbo.Customers AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.IdCustomer = c.Id

GO

DELETE FROM [VitalChoice.Ecommerce].dbo.AddressOptionValues
WHERE Value IS NULL OR Value = ''

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
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = ISNULL(c.countryCode COLLATE Cyrillic_General_CI_AS, 'US')), 
		(SELECT TOP 1 cn.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode = ISNULL(c.countryCode COLLATE Cyrillic_General_CI_AS, 'US') AND s.StateCode = c.stateCode COLLATE Cyrillic_General_CI_AS),
	2,
	2,
	c.state,
	c.idcustomer
FROM [vitalchoice2.0].dbo.CustomerOACs AS c
INNER JOIN [vitalchoice2.0].dbo.customers AS cc ON cc.idcustomer = c.idCustomer

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
INNER JOIN [VitalChoice.Ecommerce].dbo.AddressOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomerPaymentMethods
(DateCreated, DateEdited, IdAddress, IdCustomer, IdObjectType, StatusCode)
SELECT c.DateCreated, c.DateEdited, a.Id, c.Id, 2, 2 FROM [VitalChoice.Ecommerce].dbo.Customers AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Addresses AS a ON a.IdCustomer = c.Id

GO

DELETE FROM [VitalChoice.Ecommerce].dbo.AddressOptionValues
WHERE Value IS NULL OR Value = ''

ALTER TABLE [VitalChoice.Ecommerce].dbo.Addresses
DROP COLUMN IdCustomer

GO

INSERT INTO [VitalChoice.Ecommerce].dbo.CustomersToOrderNotes
(IdCustomer, IdOrderNote)
SELECT c.idcustomer, n.Id
FROM customers AS c
CROSS APPLY [dbo].[DelimitedSplit8K](c.orderSpecificNotes, ',') AS d
INNER JOIN [VitalChoice.Ecommerce].dbo.OrderNotes AS n ON n.Title = d.Item COLLATE Cyrillic_General_CI_AS

GO

--============================ Insert AspNet Users ====================================

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
PasswordHash)
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
	[vitalchoice2.0].dbo.[HashPassword]([vitalchoice2.0].dbo.RC4Encode(ISNULL(c.password, '')))
FROM customers AS c
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS cc ON cc.Id = c.idcustomer

SET IDENTITY_INSERT [VitalChoice.Infrastructure].dbo.AspNetUsers OFF;

INSERT INTO [VitalChoice.Infrastructure].dbo.AspNetUserRoles
(RoleId, UserId)
SELECT CASE WHEN customerType = 0 THEN 6 WHEN customerType = 1 THEN 7 ELSE 1 END, idCustomer
FROM customers

GO

USE [VitalChoice.Infrastructure]
GO

/****** Object:  Index [IX_NormalizedEmailUserTypeDeletedDate]    Script Date: 6/2/2016 9:27:49 PM ******/
CREATE NONCLUSTERED INDEX [IX_NormalizedEmailUserTypeDeletedDate] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC,
	[IdUserType] ASC,
	[DeletedDate] ASC
)
INCLUDE ( 	[Id],
	[AccessFailedCount],
	[ConcurrencyStamp],
	[Email],
	[EmailConfirmed],
	[FirstName],
	[CreateDate],
	[ConfirmationToken],
	[PublicId],
	[UserName],
	[LastName],
	[Status],
	[LockoutEnabled],
	[LockoutEnd],
	[NormalizedUserName],
	[PasswordHash],
	[PhoneNumber],
	[PhoneNumberConfirmed],
	[LastLoginDate],
	[UpdatedDate],
	[SecurityStamp],
	[TwoFactorEnabled],
	[TokenExpirationDate],
	[IsConfirmed]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


USE [VitalChoice.Infrastructure]
GO

/****** Object:  Index [IX_RoleName]    Script Date: 6/2/2016 9:27:59 PM ******/
CREATE NONCLUSTERED INDEX [IX_RoleName] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
INCLUDE ( 	[Id],
	[ConcurrencyStamp],
	[Name],
	[IdUserType],
	[Order]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

USE [VitalChoice.Ecommerce]
GO

/****** Object:  Index [IX_Email]    Script Date: 6/2/2016 9:29:01 PM ******/
CREATE NONCLUSTERED INDEX [IX_Email] ON [dbo].[Customers]
(
	[Email] ASC
)
INCLUDE ( 	[Id],
	[IdObjectType],
	[DateCreated],
	[DateEdited],
	[IdEditedBy],
	[IdDefaultPaymentMethod],
	[StatusCode],
	[PublicId],
	[IdAffiliate],
	[IdProfileAddress]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

DROP FUNCTION [vitalchoice2.0].[dbo].[DelimitedSplit8K]
GO