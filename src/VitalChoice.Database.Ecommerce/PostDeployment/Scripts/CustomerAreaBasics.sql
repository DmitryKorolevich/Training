IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NULL
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

IF(NOT EXISTS (SELECT [Id] FROM [dbo].[Users]))
BEGIN
	INSERT INTO [dbo].[Users]
	SELECT [Id] FROM [VitalChoice.Infrastructure].[dbo].[AspNetUsers]
END