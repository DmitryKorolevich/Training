IF COL_LENGTH('[dbo].[AspNetUsers]','IdUserType') IS NULL
BEGIN

	ALTER TABLE [dbo].[AspNetUsers]
	ADD [IdUserType] INT NULL;

END

GO

IF ('YES' =(select TOP 1 IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME ='AspNetUsers' and COLUMN_NAME ='IdUserType'))
BEGIN

	UPDATE AspNetUsers
	SET IdUserType=1
	WHERE IsAdminUser=1

	UPDATE AspNetUsers
	SET IdUserType=2
	WHERE IsAdminUser=0

	ALTER TABLE [dbo].[AspNetUsers]
	ALTER COLUMN [IdUserType] INT NOT NULL

	ALTER TABLE [dbo].[AspNetUsers]
	DROP COLUMN [IsAdminUser]

END

GO

IF COL_LENGTH('[dbo].[AspNetRoles]','IdUserType') IS NULL
BEGIN

	ALTER TABLE [dbo].[AspNetRoles]
	ADD [IdUserType] INT NULL

END

GO

IF ('YES' =(select TOP 1 IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME ='AspNetRoles' and COLUMN_NAME ='IdUserType'))
BEGIN

	UPDATE AspNetRoles
	SET IdUserType=1
	WHERE IsStorefrontRole=0

	UPDATE AspNetRoles
	SET IdUserType=2
	WHERE IsStorefrontRole=1
	
	ALTER TABLE [dbo].[AspNetRoles]
	ALTER COLUMN [IdUserType] INT NOT NULL

	ALTER TABLE [dbo].[AspNetRoles] DROP CONSTRAINT [DF_AspNetRoles_IsStorefrontRole]

	ALTER TABLE [dbo].[AspNetRoles]
	DROP COLUMN [IsStorefrontRole]

	UPDATE AspNetRoles
	SET IdUserType=3
	WHERE Name='Affiliate'

END

GO