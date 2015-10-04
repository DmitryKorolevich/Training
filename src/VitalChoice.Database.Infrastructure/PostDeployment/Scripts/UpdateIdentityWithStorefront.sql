IF COL_LENGTH('[dbo].[AspNetRoles]','IsStorefrontRole') IS NULL
BEGIN
	ALTER TABLE [dbo].[AspNetRoles]
	ADD [IsStorefrontRole] BIT CONSTRAINT DF_AspNetRoles_IsStorefrontRole DEFAULT(0) NOT NULL
END

GO

IF(NOT EXISTS (SELECT [Id] FROM [dbo].[AspNetRoles] WHERE [Name] = N'Wholesale'))
BEGIN
	SET IDENTITY_INSERT [dbo].[AspNetRoles] ON

	INSERT INTO [dbo].[AspNetRoles]
	([Id], [ConcurrencyStamp], [Name], [NormalizedName], [IsStorefrontRole])
	VALUES
	(6, N'retailstamp', N'Retail', N'Retail', 1),
	(7, N'wholesalestamp', N'Wholesale', N'Wholesale', 1),
	(8, N'affiliatestamp', N'Affiliate', N'Affiliate', 1)

	SET IDENTITY_INSERT [dbo].[AspNetRoles] OFF
END

GO

IF(NOT EXISTS (SELECT [RoleId] FROM [dbo].[AspNetRoleClaims] WHERE [ClaimType] = N'CustomerRole' ))
BEGIN
	INSERT INTO [dbo].[AspNetRoleClaims]
	([ClaimType], [ClaimValue], [RoleId])
	VALUES
	(N'CustomerRole', N'true', 6),
	(N'CustomerRole', N'true', 7)
END

GO

IF COL_LENGTH('[dbo].[AspNetUsers]','ConfirmationToken') IS NULL
BEGIN

	ALTER TABLE [dbo].[AspNetUsers]
	ADD [ConfirmationToken]   UNIQUEIDENTIFIER CONSTRAINT DF_AspNetUsers_ConfirmationToken DEFAULT(N'EB530CCF-169E-4D04-9D94-4B01D2CE1AE3') NOT NULL,
		[TokenExpirationDate] DATETIME2 (7)    CONSTRAINT DF_AspNetUsers_TokenExpirationDate DEFAULT(GETDATE()) NOT NULL,
		[IsConfirmed]         BIT              CONSTRAINT DF_AspNetUsers_IsConfirmed DEFAULT(0) NOT NULL

END

GO

IF COL_LENGTH('[dbo].[AdminProfiles]','ConfirmationToken') IS NOT NULL
BEGIN
IF EXISTS(SELECT [ConfirmationToken] FROM [dbo].[AspNetUsers] WHERE [ConfirmationToken] = N'EB530CCF-169E-4D04-9D94-4B01D2CE1AE3')
BEGIN
	
	UPDATE [dbo].[AspNetUsers] 
	SET	 [ConfirmationToken] = ap.[ConfirmationToken],
		 [TokenExpirationDate] = ap.[TokenExpirationDate],
		 [IsConfirmed] = ap.[IsConfirmed]
	FROM [dbo].[AspNetUsers] AS au
	INNER JOIN [dbo].[AdminProfiles] AS ap ON ap.Id = au.Id

END
END

GO

IF OBJECT_ID(N'DF_AspNetUsers_ConfirmationToken', N'D') IS NOT NULL
BEGIN
	ALTER TABLE [dbo].[AspNetUsers]
	DROP CONSTRAINT DF_AspNetUsers_ConfirmationToken

	ALTER TABLE [dbo].[AspNetUsers]
	DROP CONSTRAINT DF_AspNetUsers_TokenExpirationDate

	ALTER TABLE [dbo].[AspNetUsers]
	DROP CONSTRAINT DF_AspNetUsers_IsConfirmed

	ALTER TABLE [dbo].[AdminProfiles]
	DROP COLUMN [ConfirmationToken]

	ALTER TABLE [dbo].[AdminProfiles]
	DROP COLUMN [TokenExpirationDate]

	ALTER TABLE [dbo].[AdminProfiles]
	DROP COLUMN [IsConfirmed]
END

IF COL_LENGTH('[dbo].[AspNetUsers]','IsAdminUser') IS NULL
BEGIN
	ALTER TABLE [dbo].[AspNetUsers]
	ADD [IsAdminUser] BIT CONSTRAINT DF_AspNetUsers_IsAdminUser DEFAULT(1) NOT NULL

	ALTER TABLE [dbo].[AspNetUsers]
	DROP CONSTRAINT DF_AspNetUsers_IsAdminUser
END

/*stupid hack to avoid errors*/
IF COL_LENGTH('[dbo].[AdminProfiles]','ConfirmationToken') IS NOT NULL
BEGIN
	ALTER TABLE [dbo].[AdminProfiles]
	DROP COLUMN [ConfirmationToken]

	ALTER TABLE [dbo].[AdminProfiles]
	DROP COLUMN [TokenExpirationDate]

	ALTER TABLE [dbo].[AdminProfiles]
	DROP COLUMN [IsConfirmed]
END