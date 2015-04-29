IF(NOT EXISTS (SELECT [RoleId] FROM [dbo].[AspNetRoleClaims]))
BEGIN
	INSERT INTO [dbo].[AspNetRoleClaims]
	([ClaimType], [ClaimValue], [RoleId])
	VALUES
	(N'Permission', 1, 1),
	(N'Permission', 2, 1),
	(N'Permission', 3, 1),
	(N'Permission', 4, 1),
	(N'Permission', 5, 1),
	(N'Permission', 6, 1),
	(N'Permission', 8, 1),
	(N'Permission', 9, 1),
	(N'Permission', 10, 1),
	(N'Permission', 11, 1),

	(N'Permission', 6, 2),
	(N'Permission', 11, 2),

	(N'Permission', 1, 3),
	(N'Permission', 2, 3),
	(N'Permission', 11, 3),

	(N'Permission', 4, 4),
	(N'Permission', 11, 4)
END

UPDATE [dbo].[AspNetRoles]
SET [NormalizedName] = N'Admin User'
WHERE [NormalizedName] = N'AdminUser'

GO

UPDATE [dbo].[AspNetRoles]
SET [NormalizedName] = N'Content User'
WHERE [NormalizedName] = N'ContentUser'

GO

UPDATE [dbo].[AspNetRoles]
SET [NormalizedName] = N'Order User'
WHERE [NormalizedName] = N'OrderUser'

GO

UPDATE [dbo].[AspNetRoles]
SET [NormalizedName] = N'Product User'
WHERE [NormalizedName] = N'ProductUser'

GO

UPDATE [dbo].[AspNetRoles]
SET [NormalizedName] = N'Super Admin User'
WHERE [NormalizedName] = N'SuperAdminUser'

GO