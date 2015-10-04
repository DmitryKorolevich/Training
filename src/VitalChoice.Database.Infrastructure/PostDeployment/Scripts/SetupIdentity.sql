IF(NOT EXISTS (SELECT [Id] FROM [dbo].[AspNetRoles]))
BEGIN
	SET IDENTITY_INSERT [dbo].[AspNetRoles] ON

	INSERT INTO [dbo].[AspNetRoles]
	([Id], [ConcurrencyStamp], [Name], [NormalizedName])
	VALUES
	(1, N'adminuserstamp', N'Admin User', 'AdminUser'),
	(2, N'contentuserstamp', N'Content User', 'ContentUser'),
	(3, N'orderuserstamp', N'Order User', 'OrderUser'),
	(4, N'productuserstamp', N'Product User', 'ProductUser'),
	(5, N'superadminuserstamp', N'Super Admin User', 'SuperAdminUser')

	SET IDENTITY_INSERT [dbo].[AspNetRoles] OFF
END

/*stupid hack to avoid errors*/
IF COL_LENGTH('[dbo].[AdminProfiles]','ConfirmationToken') IS NULL
BEGIN
	ALTER TABLE [dbo].[AdminProfiles]
		ADD [ConfirmationToken]   UNIQUEIDENTIFIER NULL,
			[TokenExpirationDate] DATETIME2 (7)    NULL,
			[IsConfirmed]         BIT              NULL
END