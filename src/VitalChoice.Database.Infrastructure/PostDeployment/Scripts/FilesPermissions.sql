IF(NOT EXISTS (SELECT [RoleId] FROM [dbo].[AspNetRoleClaims] WHERE [ClaimValue]=14))
BEGIN
	INSERT INTO [dbo].[AspNetRoleClaims]
	([ClaimType], [ClaimValue], [RoleId])
	VALUES
	(N'Permission', 14, 1),
	(N'Permission', 14, 3)
END