--wipe out affiliates

USE [VitalChoice.Ecommerce]
GO

DELETE FROM [VitalChoice.Ecommerce].dbo.Users WHERE Id > 1500

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUserClaims
WHERE UserId IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.Affiliates)

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUserLogins
WHERE UserId IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.Affiliates)

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUserRoles
WHERE UserId IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.Affiliates)

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUsers
WHERE Id IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.Affiliates)

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

ALTER TABLE [VitalChoice.Ecommerce].dbo.Affiliates
DROP COLUMN IdOld
GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Affiliates
ADD IdOld INT NULL
GO

ALTER TABLE [VitalChoice.Ecommerce].[dbo].[Customers]
DROP CONSTRAINT [FK_Customers_Affiliates]
GO

ALTER TABLE [VitalChoice.Ecommerce].[dbo].[AffiliateOrderPayments]
DROP CONSTRAINT [FK_AffiliateOrderPaymentToAffiliate]
GO

ALTER TABLE [VitalChoice.Ecommerce].[dbo].[AffiliatePayments]
DROP CONSTRAINT [FK_AffiliatePaymentToAffiliate]
GO

ALTER TABLE [VitalChoice.Ecommerce].[dbo].AffiliateOptionValues
DROP CONSTRAINT FK_AffiliateOptionValuesToAffiliate
GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Affiliates
DROP CONSTRAINT [PK_Affiliates]
GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Affiliates
DROP COLUMN Id
GO

ALTER TABLE [VitalChoice.Ecommerce].dbo.Affiliates
ADD Id INT NOT NULL IDENTITY(1,1) CONSTRAINT [PK_Affiliates] PRIMARY KEY (Id)
GO

DECLARE @maxId INT
SELECT @maxId = MAX(Id) + 1 FROM [VitalChoice.Ecommerce].dbo.Users

DBCC CHECKIDENT ('[VitalChoice.Ecommerce].dbo.Affiliates', RESEED, @maxId);

GO

ALTER TABLE [VitalChoice.Ecommerce].[dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_Affiliates] FOREIGN KEY([IdAffiliate])
REFERENCES [VitalChoice.Ecommerce].[dbo].[Affiliates] ([Id])
GO

ALTER TABLE [VitalChoice.Ecommerce].[dbo].[Customers] CHECK CONSTRAINT [FK_Customers_Affiliates]
GO

ALTER TABLE [VitalChoice.Ecommerce].[dbo].[AffiliateOrderPayments]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateOrderPaymentToAffiliate] FOREIGN KEY([IdAffiliate])
REFERENCES [VitalChoice.Ecommerce].[dbo].[Affiliates] ([Id])
GO

ALTER TABLE [VitalChoice.Ecommerce].[dbo].[AffiliateOrderPayments] CHECK CONSTRAINT [FK_AffiliateOrderPaymentToAffiliate]
GO

ALTER TABLE [VitalChoice.Ecommerce].[dbo].[AffiliatePayments]  WITH CHECK ADD  CONSTRAINT [FK_AffiliatePaymentToAffiliate] FOREIGN KEY([IdAffiliate])
REFERENCES [VitalChoice.Ecommerce].[dbo].[Affiliates] ([Id])
GO

ALTER TABLE [VitalChoice.Ecommerce].[dbo].[AffiliatePayments] CHECK CONSTRAINT [FK_AffiliatePaymentToAffiliate]

GO
USE [VitalChoice.Ecommerce]
GO

ALTER TABLE [dbo].[AffiliateOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateOptionValuesToAffiliate] FOREIGN KEY([IdAffiliate])
REFERENCES [dbo].[Affiliates] ([Id])
GO

ALTER TABLE [dbo].[AffiliateOptionValues] CHECK CONSTRAINT [FK_AffiliateOptionValuesToAffiliate]
GO

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
	INNER JOIN [VitalChoice.Ecommerce].dbo.affiliates AS p ON p.IdOld = a.idAffiliate
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
	INNER JOIN [VitalChoice.Ecommerce].dbo.affiliates AS p ON p.IdOld = a.idAffiliate
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
		INNER JOIN [vitalchoice2.0].[dbo].affiliates AS a ON a.IdAffiliate = p.IdOld
		INNER JOIN [VitalChoice.Ecommerce].dbo.AffiliateOptionTypes AS t ON t.Name = N'''+@destFieldName+'''
		WHERE a.'+@sourceFieldName+' IS NOT NULL AND ('+ISNULL(@sourceConditions, '1=1')+')';

	ELSE
		
		SET @sql = N'INSERT INTO [VitalChoice.Ecommerce].dbo.AffiliateOptionValues
		(IdOptionType, IdAffiliate, Value)
		SELECT t.Id, p.Id, '+@fieldOperation+' FROM [VitalChoice.Ecommerce].dbo.Affiliates AS p
		INNER JOIN [vitalchoice2.0].[dbo].affiliates AS a ON a.IdAffiliate = p.IdOld
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

INSERT INTO [VitalChoice.Ecommerce].dbo.Affiliates
(DateCreated, DateEdited, County, CommissionAll, CommissionFirst, Email, IdCountry, IdState, MyAppBalance, Name, StatusCode, IdOld)
SELECT 
	ISNULL(a.joinDate, GETDATE()), 
	ISNULL(a.lastUpdated, GETDATE()), 
	a.affiliateState, 
	a.commission, 
	a.commission2, 
	a.affiliateEmail, 
	ISNULL((SELECT TOP 1 c.Id FROM [VitalChoice.Ecommerce].dbo.Countries AS c WHERE c.CountryCode COLLATE SQL_Latin1_General_CP1_CI_AS = a.affiliatecountryCode), N'1'), 
	ISNULL((SELECT TOP 1 s.Id FROM [VitalChoice.Ecommerce].dbo.States AS s WHERE s.CountryCode COLLATE SQL_Latin1_General_CP1_CI_AS = a.affiliatecountryCode AND s.StateCode COLLATE SQL_Latin1_General_CP1_CI_AS = a.affiliateState), N'1'), 
	a.ComissionsAmount, 
	a.affiliateName, 
	CASE WHEN a.pcaff_Active <> 0 THEN 2 ELSE 1 END,
	a.idAffiliate
FROM [vitalchoice2.0].dbo.affiliates AS a

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
AccessFailedCount)
SELECT 
	aff.Id, 
	NEWID(), 
	aff.Email, 
	UPPER(aff.Email),  
	1,
	aff.Email,
	UPPER(aff.Email),
	ISNULL(aff.Name, ''),
	'',
	1,
	1,
	'',
	0,
	aff.DateCreated,
	aff.DateEdited,
	0,
	NEWID(),
	GETDATE(),
	1,
	3,
	0
FROM [vitalchoice2.0].dbo.affiliates AS a
INNER JOIN [VitalChoice.Ecommerce].dbo.Affiliates AS aff ON aff.IdOld = a.idAffiliate

SET IDENTITY_INSERT [VitalChoice.Infrastructure].dbo.AspNetUsers OFF;

INSERT INTO [VitalChoice.Infrastructure].dbo.AspNetUserRoles
(RoleId, UserId)
 SELECT 8, Id 
 FROM [VitalChoice.Ecommerce].dbo.Affiliates

GO