/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 6/25/2016 3:39:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PublicId] [uniqueidentifier] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[ConcurrencyStamp] [nvarchar](255) NULL,
	[Email] [nvarchar](100) NOT NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[NormalizedEmail] [nvarchar](100) NOT NULL,
	[NormalizedUserName] [nvarchar](100) NULL,
	[PasswordHash] [nvarchar](255) NULL,
	[PhoneNumber] [nvarchar](100) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[LastLoginDate] [datetime2](7) NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NOT NULL,
	[DeletedDate] [datetime2](7) NULL,
	[SecurityStamp] [nvarchar](255) NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[ConfirmationToken] [uniqueidentifier] NOT NULL,
	[TokenExpirationDate] [datetime2](7) NOT NULL,
	[IsConfirmed] [bit] NOT NULL,
	[IdUserType] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_AspNetUsers]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = N'IX_AspNetUsers')
CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetUsers] ON [dbo].[AspNetUsers]
(
	[PublicId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_EmailDeletedDateUserType]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = N'IX_EmailDeletedDateUserType')
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
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_NormalizedEmailUserTypeDeletedDate]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = N'IX_NormalizedEmailUserTypeDeletedDate')
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
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_NormalizedNameTypeDate]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = N'IX_NormalizedNameTypeDate')
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
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserTypeDeletedDate]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = N'IX_UserTypeDeletedDate')
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
GO
