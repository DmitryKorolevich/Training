﻿IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = N'IX_NormalizedEmailUserTypeDeletedDate')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_NormalizedEmailUserTypeDeletedDate] ON [dbo].[AspNetUsers]
	(
		[NormalizedEmail] ASC,
		[IdUserType] ASC,
		[DeletedDate] ASC
	)
	INCLUDE ( 	[Id],
		[AccessFailedCount],
		[ConcurrencyStamp],
		[Email],
		[EmailConfirmed],
		[FirstName],
		[CreateDate],
		[ConfirmationToken],
		[PublicId],
		[UserName],
		[LastName],
		[Status],
		[LockoutEnabled],
		[LockoutEnd],
		[NormalizedUserName],
		[PasswordHash],
		[PhoneNumber],
		[PhoneNumberConfirmed],
		[LastLoginDate],
		[UpdatedDate],
		[SecurityStamp],
		[TwoFactorEnabled],
		[TokenExpirationDate],
		[IsConfirmed]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoles]') AND name = N'IX_RoleName')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_RoleName] ON [dbo].[AspNetRoles]
	(
		[NormalizedName] ASC
	)
	INCLUDE ( 	[Id],
		[ConcurrencyStamp],
		[Name],
		[IdUserType],
		[Order]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END
GO