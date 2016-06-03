IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = N'IX_NormalizedEmailUserTypeDeletedDate')
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

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = N'IX_UserTypeDeletedDate')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_UserTypeDeletedDate] ON [dbo].[AspNetUsers]
	(
		[DeletedDate] ASC,
		[IdUserType] ASC
	)
	INCLUDE ( 	[Id],
		[PublicId],
		[AccessFailedCount],
		[ConcurrencyStamp],
		[Email],
		[EmailConfirmed],
		[UserName],
		[FirstName],
		[LastName],
		[Status],
		[LockoutEnabled],
		[LockoutEnd],
		[NormalizedEmail],
		[NormalizedUserName],
		[PasswordHash],
		[PhoneNumber],
		[PhoneNumberConfirmed],
		[LastLoginDate],
		[CreateDate],
		[UpdatedDate],
		[SecurityStamp],
		[TwoFactorEnabled],
		[ConfirmationToken],
		[TokenExpirationDate],
		[IsConfirmed]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END
GO


IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = N'IX_NormalizedNameTypeDate')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_NormalizedNameTypeDate] ON [dbo].[AspNetUsers]
	(
		[NormalizedUserName] ASC,
		[DeletedDate] ASC,
		[IdUserType] ASC
	)
	INCLUDE ( 	[Id],
		[PublicId],
		[AccessFailedCount],
		[ConcurrencyStamp],
		[Email],
		[EmailConfirmed],
		[UserName],
		[FirstName],
		[LastName],
		[Status],
		[LockoutEnabled],
		[LockoutEnd],
		[NormalizedEmail],
		[PasswordHash],
		[PhoneNumber],
		[PhoneNumberConfirmed],
		[LastLoginDate],
		[CreateDate],
		[UpdatedDate],
		[SecurityStamp],
		[TwoFactorEnabled],
		[ConfirmationToken],
		[TokenExpirationDate],
		[IsConfirmed]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = N'IX_EmailDeletedDateUserType')
BEGIN
	CREATE NONCLUSTERED INDEX [IX_EmailDeletedDateUserType] ON [dbo].[AspNetUsers]
	(
		[Email] ASC,
		[DeletedDate] ASC,
		[IdUserType] ASC
	)
	INCLUDE ( 	[Id],
		[PublicId],
		[AccessFailedCount],
		[ConcurrencyStamp],
		[EmailConfirmed],
		[UserName],
		[FirstName],
		[LastName],
		[Status],
		[LockoutEnabled],
		[LockoutEnd],
		[NormalizedEmail],
		[NormalizedUserName],
		[PasswordHash],
		[PhoneNumber],
		[PhoneNumberConfirmed],
		[LastLoginDate],
		[CreateDate],
		[UpdatedDate],
		[SecurityStamp],
		[TwoFactorEnabled],
		[ConfirmationToken],
		[TokenExpirationDate],
		[IsConfirmed]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
END
GO


