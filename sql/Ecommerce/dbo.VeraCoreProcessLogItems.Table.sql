/****** Object:  Table [dbo].[VeraCoreProcessLogItems]    Script Date: 6/25/2016 2:13:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VeraCoreProcessLogItems]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[VeraCoreProcessLogItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[FileName] [nvarchar](250) NOT NULL,
	[FileDate] [datetime2](7) NOT NULL,
	[FileSize] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_VeraCoreProcessLogItems_FileName]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[VeraCoreProcessLogItems]') AND name = N'IX_VeraCoreProcessLogItems_FileName')
CREATE NONCLUSTERED INDEX [IX_VeraCoreProcessLogItems_FileName] ON [dbo].[VeraCoreProcessLogItems]
(
	[FileName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_VeraCoreProcessLogItems_FileName_FileDate_FileSize]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[VeraCoreProcessLogItems]') AND name = N'IX_VeraCoreProcessLogItems_FileName_FileDate_FileSize')
CREATE NONCLUSTERED INDEX [IX_VeraCoreProcessLogItems_FileName_FileDate_FileSize] ON [dbo].[VeraCoreProcessLogItems]
(
	[FileName] ASC,
	[FileDate] ASC,
	[FileSize] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
