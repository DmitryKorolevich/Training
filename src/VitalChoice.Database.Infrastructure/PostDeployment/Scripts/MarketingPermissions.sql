IF(NOT EXISTS (SELECT [RoleId] FROM [dbo].[AspNetRoleClaims] WHERE [ClaimValue]=12))
BEGIN
	INSERT INTO [dbo].[AspNetRoleClaims]
	([ClaimType], [ClaimValue], [RoleId])
	VALUES
	(N'Permission', 12, 2),
	(N'Permission', 12, 4)
END