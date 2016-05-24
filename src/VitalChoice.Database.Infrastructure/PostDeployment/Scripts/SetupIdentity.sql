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

GO

IF(NOT EXISTS (SELECT [Id] FROM [dbo].[AspNetRoles] WHERE [NormalizedName]='CustomerUser'))
BEGIN
	SET IDENTITY_INSERT [dbo].[AspNetRoles] ON

	INSERT INTO [dbo].[AspNetRoles]
	([Id], [ConcurrencyStamp], [Name], [NormalizedName])
	VALUES
	(14, N'customeruserstamp', N'Customer User', 'Customer User'),
	(15, N'reportuserstamp', N'Report User', 'Report User'),
	(16, N'useruserstamp', N'User Area User', 'User Area User'),
	(17, N'settinguserstamp', N'Setting User', 'Setting User')

	SET IDENTITY_INSERT [dbo].[AspNetRoles] OFF

	INSERT INTO AspNetRoleClaims
	(ClaimType,ClaimValue,RoleId)
	VALUES
	('Permission',1,14),
	('Permission',3,15),
	('Permission',9,16),
	('Permission',10,17),
	('Permission',11,14),
	('Permission',11,15),
	('Permission',11,16),
	('Permission',11,17)

	DELETE AspNetRoleClaims
	WHERE ClaimValue='8'
	
	DELETE AspNetRoleClaims
	WHERE RoleId=3 AND (ClaimValue='1' OR ClaimValue='14')

	DELETE AspNetRoleClaims
	WHERE RoleId=4 AND (ClaimValue='12')

END

GO