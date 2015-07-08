﻿IF NOT EXISTS (SELECT [Id] FROM [dbo].[CustomerTypes] WHERE [Name] = N'Wholesale')
BEGIN
	SET IDENTITY_INSERT [dbo].[CustomerTypes] ON

	INSERT INTO [dbo].[CustomerTypes]
	([Id], [Name], [StatusCode], [DateCreated], [DateEdited], [Order])
	VALUES
	(1, N'Retail', 2, GETDATE(), GETDATE(), 10),
	(2, N'Wholesale', 2, GETDATE(), GETDATE(), 20)

	SET IDENTITY_INSERT [dbo].[CustomerTypes] OFF
END

IF NOT EXISTS (SELECT [Id] FROM [dbo].[AddressTypes] WHERE [Name] = N'Billing')
BEGIN
	SET IDENTITY_INSERT [dbo].[AddressTypes] ON

	INSERT INTO [dbo].[AddressTypes]
	([Id], [Name])
	VALUES
	(1, N'Profile'),
	(2, N'Billing'),
	(3, N'Shipping')

	SET IDENTITY_INSERT [dbo].[AddressTypes] OFF
END

GO

IF NOT EXISTS (SELECT [Id] FROM [dbo].[PaymentMethods] WHERE [Name] = N'Credit Card')
BEGIN
	SET IDENTITY_INSERT [dbo].[PaymentMethods] ON

	INSERT INTO [dbo].[PaymentMethods]
	([Id], [Name], [StatusCode], [DateCreated], [DateEdited], [Order])
	VALUES
	(1, N'Credit Card', 2, GETDATE(), GETDATE(), 10),
	(2, N'On Approved Credit', 2, GETDATE(), GETDATE(), 20),
	(3, N'Check', 2, GETDATE(), GETDATE(), 30),
	(4, N'No Charge', 2, GETDATE(), GETDATE(), 40),
	(5, N'Prepaid', 2, GETDATE(), GETDATE(), 50)

	INSERT INTO [dbo].[PaymentMethodsToCustomerTypes]
	SELECT pm.[Id], ct.[Id]
	FROM [dbo].[PaymentMethods] pm
	CROSS JOIN [dbo].[CustomerTypes] ct

	SET IDENTITY_INSERT [dbo].[PaymentMethods] OFF
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[CustomerOptionTypes])
BEGIN
	INSERT INTO [dbo].[CustomerOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'LinkedToAffiliate', 4, NULL, NULL, NULL),
	(N'DoNotMail', 5, NULL, NULL, NULL),
	(N'DoNotRent', 5, NULL, NULL, NULL),
	(N'SuspensionReason', 4, NULL, NULL, NULL)

	DECLARE @IdLookup INT
	
	INSERT INTO [dbo].[Lookups]
	([LookupValueType])
	VALUES
	(N'string')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookup, 'Yes, Current Certificate'),
	(2, @IdLookup, 'Sales Tax will be Paid')

	INSERT INTO [dbo].[CustomerOptionTypes]
	([Name], [IdFieldType], [IdObjectType], [IdLookup], [DefaultValue])
	VALUES
	(N'TaxExempt', 3, 2, @IdLookup, '1'),
	(N'Website', 4, 2, NULL, NULL),
	(N'PromotingWebsites', 4, 2, NULL, NULL),
	(N'InceptionDate', 4, 2, NULL, NULL)

	INSERT INTO [dbo].[Lookups]
	([LookupValueType])
	VALUES
	(N'string')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookup, 'Tier 1'),
	(2, @IdLookup, 'Tier 2'),
	(3, @IdLookup, 'Tier 3')

	INSERT INTO [dbo].[CustomerOptionTypes]
	([Name], [IdFieldType], [IdObjectType], [IdLookup], [DefaultValue])
	VALUES
	(N'Tier', 3, 2, @IdLookup, '1')

	INSERT INTO [dbo].[Lookups]
	([LookupValueType])
	VALUES
	(N'string')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookup, 'Retail - General'),
	(2, @IdLookup, 'Retail - Specialty Foods'),
	(3, @IdLookup, 'Retail - Health & Supplements'),
	(4, @IdLookup, 'Retail - Online'),
	(5, @IdLookup, 'Retail - All Other'),
	(6, @IdLookup, 'Retail - Drop Ship'),
	(7, @IdLookup, 'Buying Group & Co-Ops'),
	(8, @IdLookup, 'Clinical - General Practice'),
	(9, @IdLookup, 'Clinical - Optometrist'),
	(10, @IdLookup, 'Clinical - Chiropractor'),
	(11, @IdLookup, 'Clinical - Alternative Medicine'),
	(12, @IdLookup, 'Clinical - Homeopath'),
	(13, @IdLookup, 'Clinical - Naturopath'),
	(14, @IdLookup, 'Clinical - Osteopath'),
	(15, @IdLookup, 'Clinical - Traditional Chinese Medicine'),
	(16, @IdLookup, 'Clinical - All Other'),
	(17, @IdLookup, 'Spa & Massage'),
	(18, @IdLookup, 'Private Label & Bulk'),
	(19, @IdLookup, 'International')

	INSERT INTO [dbo].[CustomerOptionTypes]
	([Name], [IdFieldType], [IdObjectType], [IdLookup], [DefaultValue])
	VALUES
	(N'TradeClass', 3, 2, @IdLookup, '1')

END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[AddressOptionTypes])
BEGIN
	INSERT INTO [dbo].[AddressOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'Company', 4, NULL, NULL, NULL),
	(N'FirstName', 4, NULL, NULL, NULL),
	(N'LastName', 4, NULL, NULL, NULL),
	(N'Company', 4, NULL, NULL, NULL),
	(N'Address1', 4, NULL, NULL, NULL),
	(N'Address2', 4, NULL, NULL, NULL),
	(N'City', 4, NULL, NULL, NULL),
	(N'Zip', 3, NULL, NULL, NULL),
	(N'Phone', 4, NULL, NULL, NULL),
	(N'Fax', 4, NULL, NULL, NULL),
	(N'Email', 4, NULL, 1, NULL),
	(N'Email', 4, NULL, 3, NULL)
END

GO

IF (SELECT COUNT(*) FROM [AddressOptionTypes] WHERE Name = 'Company' AND [IdObjectType] IS NULL) > 1
BEGIN
	DELETE TOP (1) FROM [AddressOptionTypes]
	WHERE Name = 'Company' AND [IdObjectType] IS NULL
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[AddressOptionTypes] WHERE [Name] = N'Zip' AND [IdFieldType] = 4)
BEGIN
	UPDATE [dbo].[AddressOptionTypes]
	SET [IdFieldType] = 4
	WHERE [Name] = N'Zip' AND [IdFieldType] = 3
END

GO

IF OBJECT_ID(N'[dbo].[CustomersToOrderNotes]', N'U') IS NULL
BEGIN
	EXEC sp_rename 'CustomerToOrderNotes', 'CustomersToOrderNotes'
END

GO

IF OBJECT_ID(N'[dbo].[CustomersToPaymentMethods]', N'U') IS NULL
BEGIN
	EXEC sp_rename 'CustomerToPaymentMethods', 'CustomersToPaymentMethods'
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[CustomerPaymentMethodOptionTypes])
BEGIN
	DECLARE @Card INT

	SET @Card = (SELECT [Id] FROM [dbo].[PaymentMethods] WHERE [Name] = N'Credit Card')

	INSERT INTO [dbo].[CustomerPaymentMethodOptionTypes]
	([Name],[IdFieldType], [IdLookup], [IdObjectType],[DefaultValue])
	VALUES
	(N'NameOnCard', 4, NULL, @Card, NULL),
	(N'CardNumber', 4, NULL, @Card, NULL),
	(N'ExpDateMonth', 4, NULL, @Card, NULL),
	(N'ExpDateYear', 4, NULL, @Card, NULL)

	INSERT INTO [dbo].[Lookups]
	([LookupValueType])
	VALUES
	(N'string')

	DECLARE @IdLookup INT

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookup, 'MasterCard'),
	(2, @IdLookup, 'Visa'),
	(3, @IdLookup, 'American Express'),
	(4, @IdLookup, 'Discover')

	INSERT INTO [dbo].[CustomerPaymentMethodOptionTypes]
	([Name],[IdFieldType], [IdLookup], [IdObjectType],[DefaultValue])
	VALUES
	(N'CardType', 3, @IdLookup, 1, '1')

	DECLARE @CheckId INT

	SET @CheckId = (SELECT [Id] FROM [dbo].[PaymentMethods] WHERE [Name] = N'Check')

	INSERT INTO [dbo].[CustomerPaymentMethodOptionTypes]
	([Name],[IdFieldType], [IdLookup], [IdObjectType],[DefaultValue])
	VALUES
	(N'CheckNumber', 4, NULL, @CheckId, NULL),
	(N'PaidInFull', 5, NULL, @CheckId, NULL)
END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[CustomerNoteOptionTypes])
BEGIN
	INSERT INTO [dbo].[Lookups]
	([LookupValueType])
	VALUES
	(N'string')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookup, 'High Priority'),
	(2, @IdLookup, 'Normal Priority')

	INSERT INTO [dbo].[CustomerNoteOptionTypes]
	([Name],[IdFieldType], [IdLookup], [DefaultValue])
	VALUES
	(N'Priority', 3, @IdLookup, '1')
END

IF NOT EXISTS (SELECT [Id] FROM [dbo].[Users] WHERE [Id] = 1012)
BEGIN
	INSERT INTO [dbo].[Users]
	([Id])
	VALUES
	(1012)
END