IF NOT EXISTS(SELECT [Id] FROM [dbo].[AspNetRoles] WHERE [Name]='Gift Certificates User')
BEGIN

	INSERT INTO [dbo].[AspNetRoles]
	(
		ConcurrencyStamp,
		Name,
		NormalizedName,
		IdUserType,
		[Order]
	)
	VALUES
	(
		'giftcertificateuserstamp',
		'Gift Certificates User',
		'Gift Certificates User',
		1,
		12
	)

	INSERT INTO [dbo].[AspNetRoleClaims]
	([ClaimType], [ClaimValue], [RoleId])
	VALUES
	(N'Permission', 18, (SELECT Id FROM [dbo].[AspNetRoles] WHERE Name = 'Gift Certificates User')),
	(N'Permission', 11, (SELECT Id FROM [dbo].[AspNetRoles] WHERE Name = 'Gift Certificates User')),
	(N'Permission', 11, (SELECT Id FROM [dbo].[AspNetRoles] WHERE Name = 'Inventory User'))

END

GO


