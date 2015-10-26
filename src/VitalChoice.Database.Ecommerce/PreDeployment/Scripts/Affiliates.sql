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

ALTER TABLE [dbo].[Affiliates] DROP CONSTRAINT [FK_AffiliatesToStatus]

END

GO

IF OBJECT_ID(N'[dbo].[Affiliates]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Affiliates] (
		Id INT NOT NULL CONSTRAINT PK_Affiliates PRIMARY KEY,
		Name NVARCHAR(50) NOT NULL,
		MyAppBalance MONEY NOT NULL DEFAULT(0),
		[StatusCode] INT NOT NULL
			CONSTRAINT FK_AffiliatesToStatus FOREIGN KEY (StatusCode) REFERENCES dbo.RecordStatusCodes (StatusCode),
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