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

IF(NOT EXISTS (SELECT [Id] FROM [dbo].[AspNetRoles] WHERE [NormalizedName]='Customer User'))
BEGIN
	SET IDENTITY_INSERT [dbo].[AspNetRoles] ON

	INSERT INTO [dbo].[AspNetRoles]
	([Id], [ConcurrencyStamp], [Name], [NormalizedName], IdUserType)
	VALUES
	(14, N'customeruserstamp', N'Customer User', 'Customer User', 1),
	(15, N'reportuserstamp', N'Report User', 'Report User',1),
	(16, N'useruserstamp', N'User Area User', 'User Area User',1),
	(17, N'settinguserstamp', N'Setting User', 'Setting User',1)

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

IF(NOT EXISTS (SELECT [Id] FROM [dbo].[AspNetRoles] WHERE [NormalizedName]='Inventory User'))
BEGIN
	SET IDENTITY_INSERT [dbo].[AspNetRoles] ON

	INSERT INTO [dbo].[AspNetRoles]
	([Id], [ConcurrencyStamp], [Name], [NormalizedName], IdUserType)
	VALUES
	(18, N'inventoryuserstamp', N'Inventory User', 'Inventory User', 1)

	SET IDENTITY_INSERT [dbo].[AspNetRoles] OFF

	INSERT INTO AspNetRoleClaims
	(ClaimType,ClaimValue,RoleId)
	VALUES
	('Permission',17,18)

	ALTER TABLE  [dbo].[AspNetRoles]
	ADD [Order] INT NULL

	EXEC('UPDATE AspNetRoles
	SET [Order]=0
	WHERE Id=5')

	EXEC('UPDATE AspNetRoles
	SET [Order]=1
	WHERE Id=1')

	EXEC('UPDATE AspNetRoles
	SET [Order]=2
	WHERE Id=14')

	EXEC('UPDATE AspNetRoles
	SET [Order]=3
	WHERE Id=3')

	EXEC('UPDATE AspNetRoles
	SET [Order]=4
	WHERE Id=4')

	EXEC('UPDATE AspNetRoles
	SET [Order]=5
	WHERE Id=9')

	EXEC('UPDATE AspNetRoles
	SET [Order]=6
	WHERE Id=15')

	EXEC('UPDATE AspNetRoles
	SET [Order]=7
	WHERE Id=10')

	EXEC('UPDATE AspNetRoles
	SET [Order]=8
	WHERE Id=2')

	EXEC('UPDATE AspNetRoles
	SET [Order]=9
	WHERE Id=16')

	EXEC('UPDATE AspNetRoles
	SET [Order]=10
	WHERE Id=17')

	EXEC('UPDATE AspNetRoles
	SET [Order]=11
	WHERE Id=18')

END

GO