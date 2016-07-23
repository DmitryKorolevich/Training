IF OBJECT_ID(N'[dbo].[Customers]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Customers](
		[Id] [int] NOT NULL,
		[IdCustomerType] [int] NOT NULL,
		[DateCreated] [datetime2](7) NOT NULL,
		[DateEdited] [datetime2](7) NOT NULL,
		[IdEditedBy] [int] NULL,
	 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	)

	CREATE TABLE [dbo].[CustomerTypes](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](50) NOT NULL,
		[StatusCode] [int] NOT NULL,
		[DateCreated] [datetime2](7) NOT NULL,
		[DateEdited] [datetime2](7) NOT NULL,
		[IdEditedBy] [int] NULL,
		[Order] [int] NULL,
	 CONSTRAINT [PK_CustomerTypes] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	)

	CREATE TABLE [dbo].[OrderNotes](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Title] [nvarchar](50) NOT NULL,
		[Description] [nvarchar](1000) NOT NULL,
		[StatusCode] [int] NOT NULL,
		[DateCreated] [datetime2](7) NOT NULL,
		[DateEdited] [datetime2](7) NOT NULL,
		[IdEditedBy] [int] NULL,
	 CONSTRAINT [PK_OrderNotes] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	)

	CREATE TABLE [dbo].[OrderNotesToCustomerTypes](
		[IdOrderNote] [int] NOT NULL,
		[IdCustomerType] [int] NOT NULL,
	 CONSTRAINT [PK_OrderNotesToCustomerTypes] PRIMARY KEY CLUSTERED 
	(
		[IdOrderNote] ASC,
		[IdCustomerType] ASC
	)
	)

	CREATE TABLE [dbo].[PaymentMethods](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](50) NOT NULL,
		[StatusCode] [int] NOT NULL,
		[DateCreated] [datetime2](7) NOT NULL,
		[DateEdited] [datetime2](7) NOT NULL,
		[IdEditedBy] [int] NULL,
		[Order] [int] NULL,
	 CONSTRAINT [PK_PaymentMethods] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	),
	 CONSTRAINT [IX_PaymentMethods_Name] UNIQUE NONCLUSTERED 
	(
		[Name] ASC
	)
	)

	CREATE TABLE [dbo].[PaymentMethodsToCustomerTypes](
		[IdPaymentMethod] [int] NOT NULL,
		[IdCustomerType] [int] NOT NULL,
	 CONSTRAINT [PK_PaymentMethodsToCustomerTypes] PRIMARY KEY CLUSTERED 
	(
		[IdPaymentMethod] ASC,
		[IdCustomerType] ASC
	)
	)

	CREATE TABLE [dbo].[Users](
		[Id] [int] IDENTITY(1,1) NOT NULL,
	 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	)

	ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_CustomerTypes] FOREIGN KEY([IdCustomerType])
	REFERENCES [dbo].[CustomerTypes] ([Id])

	ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_CustomerTypes]

	ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_Users] FOREIGN KEY([Id])
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_Users]

	ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_Users_EditedBy] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_Users_EditedBy]

	ALTER TABLE [dbo].[CustomerTypes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerTypes_RecordStatusCodes] FOREIGN KEY([StatusCode])
	REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])

	ALTER TABLE [dbo].[CustomerTypes] CHECK CONSTRAINT [FK_CustomerTypes_RecordStatusCodes]

	ALTER TABLE [dbo].[CustomerTypes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerTypes_Users] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[CustomerTypes] CHECK CONSTRAINT [FK_CustomerTypes_Users]

	ALTER TABLE [dbo].[OrderNotes]  WITH CHECK ADD  CONSTRAINT [FK_OrderNotes_RecordStatusCodes] FOREIGN KEY([StatusCode])
	REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])

	ALTER TABLE [dbo].[OrderNotes] CHECK CONSTRAINT [FK_OrderNotes_RecordStatusCodes]

	ALTER TABLE [dbo].[OrderNotes]  WITH CHECK ADD  CONSTRAINT [FK_OrderNotes_Users] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[OrderNotes] CHECK CONSTRAINT [FK_OrderNotes_Users]

	ALTER TABLE [dbo].[OrderNotesToCustomerTypes]  WITH CHECK ADD  CONSTRAINT [FK_OrderNotesToCustomerTypes_CustomerTypes] FOREIGN KEY([IdCustomerType])
	REFERENCES [dbo].[CustomerTypes] ([Id])

	ALTER TABLE [dbo].[OrderNotesToCustomerTypes] CHECK CONSTRAINT [FK_OrderNotesToCustomerTypes_CustomerTypes]

	ALTER TABLE [dbo].[OrderNotesToCustomerTypes]  WITH CHECK ADD  CONSTRAINT [FK_OrderNotesToCustomerTypes_OrderNotes] FOREIGN KEY([IdOrderNote])
	REFERENCES [dbo].[OrderNotes] ([Id])

	ALTER TABLE [dbo].[OrderNotesToCustomerTypes] CHECK CONSTRAINT [FK_OrderNotesToCustomerTypes_OrderNotes]

	ALTER TABLE [dbo].[PaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethods_RecordStatusCodes] FOREIGN KEY([StatusCode])
	REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])

	ALTER TABLE [dbo].[PaymentMethods] CHECK CONSTRAINT [FK_PaymentMethods_RecordStatusCodes]

	ALTER TABLE [dbo].[PaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethods_Users] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[PaymentMethods] CHECK CONSTRAINT [FK_PaymentMethods_Users]

	ALTER TABLE [dbo].[PaymentMethodsToCustomerTypes]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethodsToCustomerTypes_CustomerTypes] FOREIGN KEY([IdCustomerType])
	REFERENCES [dbo].[CustomerTypes] ([Id])

	ALTER TABLE [dbo].[PaymentMethodsToCustomerTypes] CHECK CONSTRAINT [FK_PaymentMethodsToCustomerTypes_CustomerTypes]

	ALTER TABLE [dbo].[PaymentMethodsToCustomerTypes]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethodsToCustomerTypes_PaymentMethods] FOREIGN KEY([IdPaymentMethod])
	REFERENCES [dbo].[PaymentMethods] ([Id])

	ALTER TABLE [dbo].[PaymentMethodsToCustomerTypes] CHECK CONSTRAINT [FK_PaymentMethodsToCustomerTypes_PaymentMethods]

END

IF((SELECT OBJECTPROPERTY( OBJECT_ID(N'Users'), 'TableHasIdentity')) = 1)
BEGIN
	ALTER TABLE [dbo].[Customers] 
	DROP CONSTRAINT [FK_Customers_Users]

	ALTER TABLE [dbo].[Customers] DROP CONSTRAINT [FK_Customers_Users_EditedBy]

	ALTER TABLE [dbo].[CustomerTypes]  DROP CONSTRAINT [FK_CustomerTypes_Users]

	ALTER TABLE [dbo].[OrderNotes]  DROP CONSTRAINT [FK_OrderNotes_Users]

	ALTER TABLE [dbo].[PaymentMethods] DROP CONSTRAINT [FK_PaymentMethods_Users]

	DROP TABLE [dbo].[Users]
	
	CREATE TABLE [dbo].[Users](
		[Id] [int] NOT NULL,
	 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	)

	ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_Users] FOREIGN KEY([Id])
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_Users]

	ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_Users_EditedBy] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_Users_EditedBy]

	ALTER TABLE [dbo].[CustomerTypes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerTypes_Users] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[CustomerTypes] CHECK CONSTRAINT [FK_CustomerTypes_Users]

	ALTER TABLE [dbo].[OrderNotes]  WITH CHECK ADD  CONSTRAINT [FK_OrderNotes_Users] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[OrderNotes] CHECK CONSTRAINT [FK_OrderNotes_Users]

	ALTER TABLE [dbo].[PaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethods_Users] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[PaymentMethods] CHECK CONSTRAINT [FK_PaymentMethods_Users]
END

GO

IF COL_LENGTH('[dbo].[Customers]','Email') IS NULL
BEGIN
	ALTER TABLE [dbo].[Customers]
	ADD [FirstName] NVARCHAR(250) NOT NULL,
		[LastName] NVARCHAR(250) NOT NULL,
		[Email] NVARCHAR(100) NOT NULL,
		[IdDefaultPaymentMethod] INT NOT NULL,
		[StatusCode] INT NOT NULL

	ALTER TABLE [dbo].[Customers]  WITH CHECK ADD CONSTRAINT [FK_Customers_RecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes] ([StatusCode])

	ALTER TABLE [dbo].[Customers]  WITH CHECK ADD CONSTRAINT [FK_Customers_DefaultPaymentMethod] FOREIGN KEY([IdDefaultPaymentMethod])
	REFERENCES [dbo].[PaymentMethods] ([Id])

	CREATE UNIQUE NONCLUSTERED INDEX [IX_Email_Customers]
    ON [dbo].[Customers]([Email] ASC);
END

IF COL_LENGTH('[dbo].[Customers]','FirstName') IS NOT NULL
BEGIN
	ALTER TABLE [dbo].[Customers]
	DROP COLUMN [FirstName], [LastName]
END

GO

IF OBJECT_ID(N'[dbo].[CustomerToPaymentMethods]', N'U') IS NULL AND OBJECT_ID(N'[dbo].[CustomersToPaymentMethods]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CustomerToPaymentMethods]
	([IdCustomer] INT NOT NULL,
	 [IdPaymentMethod] INT NOT NULL,
	  CONSTRAINT [PK_CustomerToPaymentMethods] PRIMARY KEY CLUSTERED 
	(
		[IdCustomer] ASC,
		[IdPaymentMethod] ASC
	)
	)

	ALTER TABLE [dbo].[CustomerToPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_CustomerToPaymentMethods_Customers] FOREIGN KEY([IdCustomer])
	REFERENCES [dbo].[Customers] ([Id])

	ALTER TABLE [dbo].[CustomerToPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_CustomerToPaymentMethods_PaymentMethods] FOREIGN KEY([IdPaymentMethod])
	REFERENCES [dbo].[PaymentMethods] ([Id])

END

GO

IF OBJECT_ID(N'[dbo].[CustomerToOrderNotes]', N'U') IS NULL AND OBJECT_ID(N'[dbo].[CustomersToOrderNotes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CustomerToOrderNotes]
	([IdCustomer] INT NOT NULL,
	 [IdOrderNote] INT NOT NULL,
	  CONSTRAINT [PK_CustomerToOrderNotes] PRIMARY KEY CLUSTERED 
	(
		[IdCustomer] ASC,
		[IdOrderNote] ASC
	)
	)

	ALTER TABLE [dbo].[CustomerToOrderNotes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerToOrderNotes_Customers] FOREIGN KEY([IdCustomer])
	REFERENCES [dbo].[Customers] ([Id])

	ALTER TABLE [dbo].[CustomerToOrderNotes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerToOrderNotes_OrderNotes] FOREIGN KEY([IdOrderNote])
	REFERENCES [dbo].[OrderNotes] ([Id])

END

GO

IF OBJECT_ID(N'[dbo].[AddressTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[AddressTypes]
	([Id] INT IDENTITY(1,1) NOT NULL,
	 [Name] NVARCHAR(50) NOT NULL,
	 CONSTRAINT [PK_AddressTypes] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	)
END

GO

IF OBJECT_ID(N'[dbo].[Addresses]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Addresses]
	([Id] INT IDENTITY(1,1) NOT NULL,
	 [IdCustomer] INT NOT NULL,
	 [IdCountry] INT NOT NULL,
	 [IdState] INT NOT NULL,
	 [IdAddressType] INT NOT NULL,
	 [County] NVARCHAR(250) NOT NULL,
	 [DateCreated] [datetime2](7) NOT NULL,
	 [DateEdited] [datetime2](7) NOT NULL,
	 [IdEditedBy] [int] NULL,
	 [StatusCode] INT NOT NULL

	  CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	)

	ALTER TABLE [dbo].[Addresses]  WITH CHECK ADD CONSTRAINT [FK_Addresses_RecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes] ([StatusCode])

	ALTER TABLE [dbo].[Addresses]  WITH CHECK ADD  CONSTRAINT [FK_Addresses_Customers] FOREIGN KEY([IdCustomer])
	REFERENCES [dbo].[Customers] ([Id])

	ALTER TABLE [dbo].[Addresses]  WITH CHECK ADD  CONSTRAINT [FK_Addresses_Countries] FOREIGN KEY([IdCountry])
	REFERENCES [dbo].[Countries] ([Id])

	ALTER TABLE [dbo].[Addresses]  WITH CHECK ADD  CONSTRAINT [FK_Addresses_States] FOREIGN KEY([IdState])
	REFERENCES [dbo].[States] ([Id])

	ALTER TABLE [dbo].[Addresses]  WITH CHECK ADD  CONSTRAINT [FK_Addresses_AddressTypes] FOREIGN KEY([IdAddressType])
	REFERENCES [dbo].[AddressTypes] ([Id])

	ALTER TABLE [dbo].[Addresses]  WITH CHECK ADD  CONSTRAINT [FK_Addresses_Users_EditedBy] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[Users] ([Id])

END

GO

IF OBJECT_ID(N'[dbo].[CustomerOptionTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CustomerOptionTypes]
	(
		[Id] INT IDENTITY(1,1) NOT NULL, 
		[Name] NVARCHAR(50) NOT NULL, 
		[IdFieldType] INT NOT NULL, 
		[IdLookup] INT NULL, 
		[IdCustomerType] INT NULL, 
		[DefaultValue] NVARCHAR(250) NULL, 
		 CONSTRAINT [PK_CustomerOptionTypes] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		),
		CONSTRAINT [FK_CustomerOptionTypes_Lookup] FOREIGN KEY ([IdLookup]) REFERENCES [Lookups]([Id]), 
		CONSTRAINT [FK_CustomerOptionTypes_FieldType] FOREIGN KEY ([IdFieldType]) REFERENCES [FieldTypes]([Id]), 
		CONSTRAINT [FK_CustomerOptionTypes_CustomerType] FOREIGN KEY ([IdCustomerType]) REFERENCES [CustomerTypes]([Id])
	);

	CREATE INDEX [IX_CustomerOptionTypes_Name] ON [dbo].[CustomerOptionTypes] ([Name]) INCLUDE (Id, IdFieldType, IdCustomerType)
END

GO

IF OBJECT_ID(N'[dbo].[CustomerOptionValues]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CustomerOptionValues]
	(
		[Id] INT IDENTITY(1,1) NOT NULL, 
		[IdCustomer] INT NULL, 
		[IdOptionType] INT NOT NULL, 
		[Value] NVARCHAR(250) NULL, 
		 CONSTRAINT [PK_CustomerOptionValues] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		),
		CONSTRAINT [FK_CustomerOptionValues_CustomerOptionTypes] FOREIGN KEY ([IdOptionType]) REFERENCES [CustomerOptionTypes]([Id]), 
		CONSTRAINT [FK_CustomerOptionValues_Customer] FOREIGN KEY ([IdCustomer]) REFERENCES [Customers]([Id])
	);

END

GO

IF OBJECT_ID(N'[dbo].[CustomerNotes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CustomerNotes]
	([Id] INT IDENTITY(1,1) NOT NULL,
	 [IdCustomer] INT NOT NULL,
	 [DateCreated] [datetime2](7) NOT NULL,
	 [DateEdited] [datetime2](7) NOT NULL,
	 [IdEditedBy] [int] NULL,
	  CONSTRAINT [PK_CustomerNotes] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	)

	ALTER TABLE [dbo].[CustomerNotes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerNotes_Customers] FOREIGN KEY([IdCustomer])
	REFERENCES [dbo].[Customers] ([Id])

	ALTER TABLE [dbo].[CustomerNotes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerNotes_Users_EditedBy] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[Users] ([Id])

END

GO

IF COL_LENGTH('[dbo].[CustomerNotes]','Note') IS NULL
BEGIN
	ALTER TABLE [dbo].[CustomerNotes]
	ADD [Note] NVARCHAR(1000) NOT NULL,
		[StatusCode] INT NOT NULL

	ALTER TABLE [dbo].[CustomerNotes]  WITH CHECK ADD CONSTRAINT [FK_CustomerNotes_RecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes] ([StatusCode])
END

IF OBJECT_ID(N'[dbo].[CustomerNoteOptionTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CustomerNoteOptionTypes]
	(
		[Id] INT IDENTITY(1,1) NOT NULL, 
		[Name] NVARCHAR(50) NOT NULL, 
		[IdFieldType] INT NOT NULL, 
		[IdLookup] INT NULL, 
		[DefaultValue] NVARCHAR(250) NULL, 
		 CONSTRAINT [PK_CustomerNoteOptionTypes] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		),
		CONSTRAINT [FK_CustomerNoteOptionTypes_Lookup] FOREIGN KEY ([IdLookup]) REFERENCES [Lookups]([Id]), 
		CONSTRAINT [FK_CustomerNoteOptionTypes_FieldType] FOREIGN KEY ([IdFieldType]) REFERENCES [FieldTypes]([Id])
	);

	CREATE INDEX [IX_CustomerNoteOptionTypes_Name] ON [dbo].[CustomerNoteOptionTypes] ([Name]) INCLUDE (Id, IdFieldType)
END

GO

IF OBJECT_ID(N'[dbo].[CustomerNoteOptionValues]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CustomerNoteOptionValues]
	(
		[Id] INT IDENTITY(1,1) NOT NULL, 
		[IdCustomerNote] INT NULL, 
		[IdOptionType] INT NOT NULL, 
		[Value] NVARCHAR(250) NULL, 
		 CONSTRAINT [PK_CustomerNoteOptionValues] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		),
		CONSTRAINT [FK_CustomerNoteOptionValues_CustomerNoteOptionTypes] FOREIGN KEY ([IdOptionType]) REFERENCES [CustomerNoteOptionTypes]([Id]), 
		CONSTRAINT [FK_CustomerNoteOptionValues_CustomerNotes] FOREIGN KEY ([IdCustomerNote]) REFERENCES [CustomerNotes]([Id])
	);

	CREATE INDEX [IX_CustomerNoteOptionValues_Value] ON [dbo].[CustomerNoteOptionValues] ([Value]) INCLUDE (Id, IdCustomerNote, IdOptionType)
END

GO

IF OBJECT_ID(N'[dbo].[AddressOptionTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[AddressOptionTypes]
	(
		[Id] INT IDENTITY(1,1) NOT NULL, 
		[Name] NVARCHAR(50) NOT NULL, 
		[IdFieldType] INT NOT NULL, 
		[IdLookup] INT NULL, 
		[IdAddressType] INT NULL, 
		[DefaultValue] NVARCHAR(250) NULL, 
		 CONSTRAINT [PK_AddressOptionTypes] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		),
		CONSTRAINT [FK_AddressOptionTypes_Lookup] FOREIGN KEY ([IdLookup]) REFERENCES [Lookups]([Id]), 
		CONSTRAINT [FK_AddressOptionTypes_FieldType] FOREIGN KEY ([IdFieldType]) REFERENCES [FieldTypes]([Id]), 
		CONSTRAINT [FK_AddressOptionTypes_AddressType] FOREIGN KEY ([IdAddressType]) REFERENCES [AddressTypes]([Id])
	);

	CREATE INDEX [IX_AddressOptionTypes_Name] ON [dbo].[AddressOptionTypes] ([Name]) INCLUDE (Id, IdFieldType, IdAddressType)
END

GO

IF OBJECT_ID(N'[dbo].[AddressOptionValues]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[AddressOptionValues]
	(
		[Id] INT IDENTITY(1,1) NOT NULL, 
		[IdAddress] INT NULL, 
		[IdOptionType] INT NOT NULL, 
		[Value] NVARCHAR(250) NULL, 
		 CONSTRAINT [PK_AddressOptionValues] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		),
		CONSTRAINT [FK_AddressOptionValues_AddressOptionTypes] FOREIGN KEY ([IdOptionType]) REFERENCES [AddressOptionTypes]([Id]), 
		CONSTRAINT [FK_AddressOptionValues_Address] FOREIGN KEY ([IdAddress]) REFERENCES [Addresses]([Id])
	);

	CREATE INDEX [IX_AddressOptionValues_Value] ON [dbo].[AddressOptionValues] ([Value]) INCLUDE (Id, IdAddress, IdOptionType)
END

GO

IF OBJECT_ID(N'[dbo].[CustomerPaymentMethods]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CustomerPaymentMethods](
		[Id] INT IDENTITY(1,1) NOT NULL, 
		[IdPaymentMethod] INT NOT NULL, 
		[IdAddress] INT NOT NULL, 
		[IdCustomer] INT NOT NULL, 
		[DateCreated] [datetime2](7) NOT NULL,
		[DateEdited] [datetime2](7) NOT NULL,
		[IdEditedBy] [int] NULL,
		[StatusCode] INT NOT NULL
		CONSTRAINT [PK_CustomerPaymentMethods] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		),
		CONSTRAINT [FK_CustomerPaymentMethods_PaymentMethod] FOREIGN KEY ([IdPaymentMethod]) REFERENCES [PaymentMethods]([Id])
	);

	ALTER TABLE [dbo].[CustomerPaymentMethods]  WITH CHECK ADD CONSTRAINT [FK_CustomerPaymentMethods_RecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes] ([StatusCode])

	ALTER TABLE [dbo].[CustomerPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPaymentMethods_Customers] FOREIGN KEY([IdCustomer])
	REFERENCES [dbo].[Customers] ([Id])

	ALTER TABLE [dbo].[CustomerPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPaymentMethods_Users_EditedBy] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[CustomerPaymentMethods]  WITH CHECK ADD  CONSTRAINT [FK_CustomerPaymentMethods_Addresses_EditedBy] FOREIGN KEY([IdAddress])
	REFERENCES [dbo].[Addresses] ([Id])

END

GO

IF OBJECT_ID(N'[dbo].[CustomerPaymentMethodOptionTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CustomerPaymentMethodOptionTypes]
	(
		[Id] INT IDENTITY(1,1) NOT NULL, 
		[Name] NVARCHAR(50) NOT NULL, 
		[IdPaymentMethod] INT NOT NULL, 
		[IdFieldType] INT NOT NULL, 
		[IdLookup] INT NULL, 
		[DefaultValue] NVARCHAR(250) NULL, 
		 CONSTRAINT [PK_CustomerPaymentMethodOptionTypes] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		),
		CONSTRAINT [FK_CustomerPaymentMethodOptionTypes_PaymentMethod] FOREIGN KEY ([IdPaymentMethod]) REFERENCES [PaymentMethods]([Id]),
		CONSTRAINT [FK_CustomerPaymentMethodOptionTypes_Lookup] FOREIGN KEY ([IdLookup]) REFERENCES [Lookups]([Id]), 
		CONSTRAINT [FK_CustomerPaymentMethodOptionTypes_FieldType] FOREIGN KEY ([IdFieldType]) REFERENCES [FieldTypes]([Id])
	);

	CREATE INDEX [IX_CustomerPaymentMethodOptionTypes_Name] ON [dbo].[CustomerPaymentMethodOptionTypes] ([Name]) INCLUDE (Id, IdFieldType)
END

GO

IF OBJECT_ID(N'[dbo].[CustomerPaymentMethodValues]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CustomerPaymentMethodValues]
	(
		[Id] INT IDENTITY(1,1) NOT NULL, 
		[IdCustomerPaymentMethod] INT NULL, 
		[IdOptionType] INT NOT NULL, 
		[Value] NVARCHAR(250) NULL, 
		 CONSTRAINT [PK_CustomerPaymentMethodValues] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		),
		CONSTRAINT [FK_CustomerPaymentMethodValues_CustomerPaymentMethodOptionTypes] FOREIGN KEY ([IdOptionType]) REFERENCES [CustomerPaymentMethodOptionTypes]([Id]), 
		CONSTRAINT [FK_CustomerPaymentMethodValues_CustomerPaymentMethods] FOREIGN KEY ([IdCustomerPaymentMethod]) REFERENCES [CustomerPaymentMethods]([Id])
	);

	CREATE INDEX [IX_CustomerPaymentMethodValues_Value] ON [dbo].[CustomerPaymentMethodValues] ([Value]) INCLUDE (Id, IdCustomerPaymentMethod, IdOptionType)
END

GO

IF NOT EXISTS (SELECT [Id] FROM [dbo].[Addresses])
BEGIN
	ALTER TABLE [dbo].[Addresses]
		ALTER COLUMN [IdState] INT NULL

	ALTER TABLE [dbo].[Addresses]
		ALTER COLUMN [County] NVARCHAR(250) NULL
END

GO

IF EXISTS(SELECT * FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[dbo].[Customers]', N'U') AND Name = 'IdCustomerType')
BEGIN
	EXEC sp_rename 'dbo.Customers.IdCustomerType', 'IdObjectType', 'COLUMN';
	EXEC sp_rename 'dbo.CustomerOptionTypes.IdCustomerType', 'IdObjectType', 'COLUMN';
	EXEC sp_rename 'dbo.CustomerPaymentMethods.IdPaymentMethod', 'IdObjectType', 'COLUMN';
	EXEC sp_rename 'dbo.CustomerPaymentMethodOptionTypes.IdPaymentMethod', 'IdObjectType', 'COLUMN';
	EXEC sp_rename 'dbo.Addresses.IdAddressType', 'IdObjectType', 'COLUMN';
	EXEC sp_rename 'dbo.AddressOptionTypes.IdAddressType', 'IdObjectType', 'COLUMN';
END

IF NOT EXISTS(SELECT * FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[dbo].[CustomerOptionValues]', N'U') AND Name = 'IdBigString')
	ALTER TABLE [dbo].[CustomerOptionValues]
	ADD [IdBigString] BIGINT NULL,
	CONSTRAINT [FK_CustomerOptionValue_ToBigStringValue] FOREIGN KEY ([IdBigString]) REFERENCES [BigStringValues]([IdBigString]) ON DELETE CASCADE

GO

IF COL_LENGTH('[dbo].[Lookups]','Name') IS NULL
BEGIN
	ALTER TABLE [dbo].[Lookups]
	ADD [Name] NVARCHAR(100) CONSTRAINT DF_Name_Default DEFAULT(N'1') NOT NULL

END

IF EXISTS(SELECT * FROM sys.columns WHERE Name = 'IdCustomer' AND [object_id] = OBJECT_ID('CustomerOptionValues') AND is_nullable = 1)
BEGIN
	IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerOptionValues]') AND name = N'IX_CustomerOptionValues_Value')
		DROP INDEX [IX_CustomerOptionValues_Value] ON [dbo].[CustomerOptionValues]

	ALTER TABLE CustomerOptionValues
	ALTER COLUMN IdCustomer INT NOT NULL

	CREATE INDEX [IX_CustomerOptionValues_Value] ON [dbo].[CustomerOptionValues] ([Value]) INCLUDE (Id, IdCustomer, IdOptionType)

	IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerNoteOptionValues]') AND name = N'IX_CustomerNoteOptionValues_Value')
		DROP INDEX [IX_CustomerNoteOptionValues_Value] ON [dbo].[CustomerNoteOptionValues]

	ALTER TABLE CustomerNoteOptionValues
	ALTER COLUMN IdCustomerNote INT NOT NULL

	CREATE INDEX [IX_CustomerNoteOptionValues_Value] ON [dbo].[CustomerNoteOptionValues] ([Value]) INCLUDE (Id, IdCustomerNote, IdOptionType)

	IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AddressOptionValues]') AND name = N'IX_AddressOptionValues_Value')
		DROP INDEX [IX_AddressOptionValues_Value] ON [dbo].[AddressOptionValues]

	ALTER TABLE AddressOptionValues
	ALTER COLUMN IdAddress INT NOT NULL

	CREATE INDEX [IX_AddressOptionValues_Value] ON [dbo].[AddressOptionValues] ([Value]) INCLUDE (Id, IdAddress, IdOptionType)

	IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CustomerPaymentMethodValues]') AND name = N'IX_CustomerPaymentMethodValues_Value')
		DROP INDEX [IX_CustomerPaymentMethodValues_Value] ON [dbo].[CustomerPaymentMethodValues]

	ALTER TABLE [CustomerPaymentMethodValues]
	ALTER COLUMN IdCustomerPaymentMethod INT NOT NULL

	CREATE INDEX [IX_CustomerPaymentMethodValues_Value] ON [dbo].[CustomerPaymentMethodValues] ([Value]) INCLUDE (Id, IdCustomerPaymentMethod, IdOptionType)
END
GO


IF EXISTS(SELECT * FROM sys.columns WHERE Name = 'IdAddress' AND [object_id] = OBJECT_ID('CustomerPaymentMethods') AND is_nullable = 0)
BEGIN

	ALTER TABLE CustomerPaymentMethods
	ALTER COLUMN IdAddress INT NULL

END

IF OBJECT_ID(N'[dbo].[CustomerFiles]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CustomerFiles](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[IdCustomer] [int] NOT NULL,
		[UploadDate] [datetime2](7) NOT NULL,
		[FileName] [nvarchar](250) NOT NULL,
		[Description] [nvarchar](500) NOT NULL
	 CONSTRAINT [PK_CustomerFiles] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	) ON [PRIMARY]


	ALTER TABLE [dbo].[CustomerFiles]  WITH CHECK ADD  CONSTRAINT [FK_CustomerFiles_Customers] FOREIGN KEY([IdCustomer])
	REFERENCES [dbo].[Customers] ([Id])


	ALTER TABLE [dbo].[CustomerFiles] CHECK CONSTRAINT [FK_CustomerFiles_Customers]

END

IF COL_LENGTH('[dbo].[Customers]','PublicId') IS NULL
BEGIN
	ALTER TABLE [dbo].[Customers]
	ADD [PublicId] UNIQUEIDENTIFIER CONSTRAINT DF_PublicId_Default DEFAULT(NEWID()) NOT NULL

	ALTER TABLE [dbo].[Customers]
	ADD CONSTRAINT UQ_Customer UNIQUE(PublicId)

	ALTER TABLE [dbo].[Customers]
	DROP CONSTRAINT DF_PublicId_Default
END

GO

ALTER TABLE  [dbo].[CustomerFiles]
	ALTER COLUMN [Description] [nvarchar](500) NULL

GO

IF (OBJECT_ID('FK_Customers_RecordStatusCode', 'F') IS NOT NULL)
BEGIN
	ALTER TABLE [dbo].[Customers] 
	DROP CONSTRAINT [FK_Customers_RecordStatusCode]
END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = 'IdAddedBy' AND [object_id] = OBJECT_ID('CustomerNotes'))
BEGIN

	ALTER TABLE CustomerNotes
	ADD IdAddedBy INT NULL
	
	ALTER TABLE [dbo].[CustomerNotes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerNotes_Users_IdAddedBy] FOREIGN KEY([IdAddedBy])
	REFERENCES [dbo].[Users] ([Id])
	
	ALTER TABLE [dbo].[CustomerNotes] CHECK CONSTRAINT [FK_CustomerNotes_Users_IdAddedBy]

END

GO

IF EXISTS(SELECT * FROM sys.columns WHERE Name = 'Email' AND [object_id] = OBJECT_ID('Customers') AND is_nullable = 0)
BEGIN

	ALTER TABLE Customers
	ALTER COLUMN Email NVARCHAR(100) NULL

	DROP INDEX [IX_Email_Customers] ON [dbo].[Customers]
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_UQ_NameTypeCustomerOption')
BEGIN
	CREATE UNIQUE NONCLUSTERED INDEX IX_UQ_NameTypeCustomerOption ON [dbo].[CustomerOptionTypes]
	(
		[Name] ASC,
		[IdObjectType] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_UQ_NameTypeCustomerPaymentOption')
BEGIN
	CREATE UNIQUE NONCLUSTERED INDEX IX_UQ_NameTypeCustomerPaymentOption ON [dbo].[CustomerPaymentMethodOptionTypes]
	(
		[Name] ASC,
		[IdObjectType] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END



GO