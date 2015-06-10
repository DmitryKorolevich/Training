IF NOT EXISTS (SELECT [Id] FROM [dbo].[CustomerTypes] WHERE [Name] = N'Wholesale')
BEGIN
	SET IDENTITY_INSERT [dbo].[CustomerTypes] ON

	INSERT INTO [dbo].[CustomerTypes]
	([Id], [Name], [StatusCode], [DateCreated], [DateEdited], [Order])
	VALUES
	(1, N'Retail', 2, GETDATE(), GETDATE(), 10),
	(2, N'Wholesale', 2, GETDATE(), GETDATE(), 20)

	SET IDENTITY_INSERT [dbo].[CustomerTypes] OFF
END

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