IF (1=(select TOP 1 columnproperty(object_id('Affiliates'),'Id','IsIdentity')))
BEGIN

ALTER TABLE [dbo].[Customers] DROP CONSTRAINT [FK_CustomersToAffiliates]

DROP TABLE [dbo].[AffiliateOptionValues]

DROP TABLE [dbo].[AffiliateOptionTypes]

DROP TABLE [dbo].[Affiliates]

DELETE LookupVariants
WHERE IdLookup IN
(SELECT Id FROM Lookups
WHERE Name='AffiliateProfessionalPractices')

DELETE Lookups
WHERE Name='AffiliateProfessionalPractices'

DELETE LookupVariants
WHERE IdLookup IN
(SELECT Id FROM Lookups
WHERE Name='AffiliateMonthlyEmailsSentOptions')

DELETE Lookups
WHERE Name='AffiliateMonthlyEmailsSentOptions'

DELETE LookupVariants
WHERE IdLookup IN
(SELECT Id FROM Lookups
WHERE Name='AffiliateTiers')

DELETE Lookups
WHERE Name='AffiliateTiers'

DELETE LookupVariants
WHERE IdLookup IN
(SELECT Id FROM Lookups
WHERE Name='AffiliatePaymentTypes')

DELETE Lookups
WHERE Name='AffiliatePaymentTypes'

END

GO

IF OBJECT_ID(N'[dbo].[Affiliates]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Affiliates] (
		Id INT NOT NULL CONSTRAINT PK_Affiliates PRIMARY KEY,
		Name NVARCHAR(50) NOT NULL,
		MyAppBalance MONEY NOT NULL DEFAULT(0),
		[StatusCode] INT NOT NULL,
		[DateCreated] [datetime2] NOT NULL,
		[DateEdited] [datetime2] NOT NULL,
		[IdEditedBy] [int] NULL
			CONSTRAINT FK_AffiliatesToUser FOREIGN KEY (IdEditedBy) REFERENCES dbo.Users (Id),			
		CommissionFirst DECIMAL(5,2) NOT NULL DEFAULT(0),		
		CommissionAll DECIMAL(5,2) NOT NULL DEFAULT(0),
		[IdCountry] INT NOT NULL,
		[IdState] INT NULL,
		[County] NVARCHAR(250) NULL
	)

	ALTER TABLE [dbo].[Affiliates]  WITH CHECK ADD  CONSTRAINT [FK_Affiliates_Countries] FOREIGN KEY([IdCountry])
		REFERENCES [dbo].[Countries] ([Id])

	ALTER TABLE [dbo].[Affiliates]  WITH CHECK ADD  CONSTRAINT [FK_Affiliates_States] FOREIGN KEY([IdState])
		REFERENCES [dbo].[States] ([Id])

	CREATE TABLE [dbo].[AffiliateOptionTypes] (
		[Id] INT NOT NULL
			CONSTRAINT PK_AffiliateOptionTypes PRIMARY KEY (Id) IDENTITY,
		[Name] NVARCHAR(250) NOT NULL,
		[IdLookup] INT NULL
			CONSTRAINT FK_AffiliateOptionTypesToLookup FOREIGN KEY (IdLookup) REFERENCES dbo.Lookups (Id),
		[IdFieldType] INT NOT NULL
			CONSTRAINT FK_AffiliateOptionTypesToFieldType FOREIGN KEY (IdFieldType) REFERENCES dbo.FieldTypes (Id),
		[IdObjectType] INT NULL,
		[DefaultValue] NVARCHAR(250) NULL
	)

	CREATE NONCLUSTERED INDEX IX_AffiliateOptionTypes_Name ON dbo.AffiliateOptionTypes (Name)

	CREATE TABLE [dbo].[AffiliateOptionValues] (
		[Id] INT NOT NULL
			CONSTRAINT PK_AffiliateOptionValues PRIMARY KEY (Id) IDENTITY,
		[IdOptionType] INT NOT NULL
			CONSTRAINT FK_AffiliateOptionValuesToAffiliateOptionType FOREIGN KEY (IdOptionType) REFERENCES dbo.AffiliateOptionTypes (Id),
		[IdAffiliate] INT NOT NULL
			CONSTRAINT FK_AffiliateOptionValuesToAffiliate FOREIGN KEY (IdAffiliate) REFERENCES dbo.Affiliates (Id),
		[Value] NVARCHAR(250) NULL,
		[IdBigString] BIGINT NULL,
			CONSTRAINT [FK_AffiliateOptionValuesToBigStringValues] FOREIGN KEY ([IdBigString]) REFERENCES [BigStringValues]([IdBigString]) ON DELETE CASCADE
	)

	CREATE NONCLUSTERED INDEX IX_AffiliateOptionValues_Value ON AffiliateOptionValues (Value)
END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = 'IdAffiliate' AND [object_id] = OBJECT_ID(N'[dbo].[Customers]', N'U'))
BEGIN

	ALTER TABLE dbo.Customers
	ADD IdAffiliate INT NULL
		CONSTRAINT FK_CustomersToAffiliates FOREIGN KEY (IdAffiliate) REFERENCES dbo.Affiliates(Id)
END

GO

IF OBJECT_ID(N'[dbo].[AffiliatePayments]', N'U') IS NULL
BEGIN

	CREATE TABLE [dbo].[AffiliatePayments] (
		[Id] INT NOT NULL
			CONSTRAINT PK_AffiliatePayments PRIMARY KEY (Id) IDENTITY,
		[IdAffiliate] INT NOT NULL
			CONSTRAINT FK_AffiliatePaymentToAffiliate FOREIGN KEY (IdAffiliate) REFERENCES dbo.Affiliates (Id),
		[DateCreated] [datetime2] NOT NULL,
		[Amount] MONEY NOT NULL,
	)


	CREATE TABLE [dbo].[AffiliateOrderPayments] (
		[Id] INT NOT NULL
			CONSTRAINT PK_AffiliateOrderPayments PRIMARY KEY (Id) IDENTITY,
		[IdAffiliate] INT NOT NULL
			CONSTRAINT FK_AffiliateOrderPaymentToAffiliate FOREIGN KEY (IdAffiliate) REFERENCES dbo.Affiliates (Id),
		[IdOrder] INT NOT NULL
			CONSTRAINT FK_AffiliateOrderPaymentToOrder FOREIGN KEY (IdOrder) REFERENCES dbo.Orders (Id),
		[Amount] MONEY NOT NULL,
		[Status] INT NOT NULL DEFAULT(1),
		[IdAffiliatePayment] INT NULL		
			CONSTRAINT FK_AffiliateOrderPaymentToAffiliatePayment FOREIGN KEY (IdAffiliatePayment) REFERENCES dbo.AffiliatePayments (Id),
	)

	CREATE NONCLUSTERED INDEX IX_AffiliateOrderPayments_IdOrder ON AffiliateOrderPayments (IdOrder)
	CREATE NONCLUSTERED INDEX IX_AffiliateOrderPayments_IdAffiliate_Status ON AffiliateOrderPayments (IdAffiliate,Status)
	CREATE NONCLUSTERED INDEX IX_AffiliateOrderPayments_IdAffiliatePayment ON AffiliateOrderPayments (IdAffiliatePayment)

	CREATE UNIQUE NONCLUSTERED INDEX [UQ_AffiliateOrderPayments] ON [dbo].[AffiliateOrderPayments]
	(
		[IdOrder] ASC
	)
END

GO

IF OBJECT_ID(N'[dbo].[VCustomersInAffiliates]', N'V') IS NOT NULL
DROP VIEW [dbo].[VCustomersInAffiliates]
GO

CREATE VIEW [dbo].[VCustomersInAffiliates]
AS
SELECT a.Id, MIN(a.Name) AS Name, count(*) AS Count FROM Affiliates a
JOIN Customers c ON a.Id=c.IdAffiliate
GROUP BY a.Id

GO

IF NOT EXISTS (SELECT object_id FROM sys.indexes WHERE name='IX_Customers_IdAffiliate' AND object_id = OBJECT_ID('Customers'))
BEGIN

CREATE NONCLUSTERED INDEX [IX_Customers_IdAffiliate] ON [dbo].[Customers]
(
	[IdAffiliate] ASC
)WITH (FILLFACTOR = 80)

END

GO

IF EXISTS(SELECT * FROM sys.columns WHERE name = 'IdOrder' AND [object_id] = OBJECT_ID(N'[dbo].[AffiliateOrderPayments]', N'U'))
BEGIN
	DROP TABLE [dbo].[AffiliateOrderPayments]

	CREATE TABLE [dbo].[AffiliateOrderPayments] (
		[Id] INT NOT NULL
			CONSTRAINT PK_AffiliateOrderPayments PRIMARY KEY (Id),
		[IdAffiliate] INT NOT NULL
			CONSTRAINT FK_AffiliateOrderPaymentToAffiliate FOREIGN KEY (IdAffiliate) REFERENCES dbo.Affiliates (Id),
		[Amount] MONEY NOT NULL,
		[Status] INT NOT NULL DEFAULT(1),
		[IdAffiliatePayment] INT NULL		
			CONSTRAINT FK_AffiliateOrderPaymentToAffiliatePayment FOREIGN KEY (IdAffiliatePayment) REFERENCES dbo.AffiliatePayments (Id),
	)

	CREATE NONCLUSTERED INDEX IX_AffiliateOrderPayments_IdAffiliate_Status ON AffiliateOrderPayments (IdAffiliate,Status)
	CREATE NONCLUSTERED INDEX IX_AffiliateOrderPayments_IdAffiliatePayment ON AffiliateOrderPayments (IdAffiliatePayment)

	ALTER TABLE dbo.AffiliateOrderPayments ADD CONSTRAINT
	FK_AffiliateOrderPayments_Orders FOREIGN KEY
	(
	Id
	) REFERENCES dbo.Orders
	(
	Id
	)
END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = 'NewCustomerOrder' AND [object_id] = OBJECT_ID(N'[dbo].[AffiliateOrderPayments]', N'U'))
BEGIN

	ALTER TABLE dbo.AffiliateOrderPayments
	ADD NewCustomerOrder BIT NOT NULL DEFAULT 0
END

GO

IF NOT EXISTS (SELECT object_id FROM sys.foreign_keys WHERE name='FK_Customers_Affiliates' AND parent_object_id = object_id('Customers'))
BEGIN

ALTER TABLE dbo.Customers ADD CONSTRAINT
	FK_Customers_Affiliates FOREIGN KEY
	(
	IdAffiliate
	) REFERENCES dbo.Affiliates
	(
	Id
	)

END

GO

IF (100 = (SELECT TOP 1 max_length FROM sys.columns WHERE name = 'Name' AND [object_id] = OBJECT_ID(N'[dbo].[Affiliates]', N'U')))
BEGIN

	ALTER TABLE dbo.Affiliates
	ALTER COLUMN Name NVARCHAR(100) NOT NULL

END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_UQ_NameTypeAffiliateOption')
BEGIN
	CREATE UNIQUE NONCLUSTERED INDEX IX_UQ_NameTypeAffiliateOption ON [dbo].[AffiliateOptionTypes]
	(
		[Name] ASC,
		[IdObjectType] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END

GO