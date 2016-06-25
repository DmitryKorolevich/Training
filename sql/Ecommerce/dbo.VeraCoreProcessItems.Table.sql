/****** Object:  Table [dbo].[VeraCoreProcessItems]    Script Date: 6/25/2016 2:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VeraCoreProcessItems]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[VeraCoreProcessItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Attempt] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[FileName] [nvarchar](250) NOT NULL,
	[FileDate] [datetime2](7) NOT NULL,
	[FileSize] [bigint] NOT NULL,
	[Data] [nvarchar](max) NOT NULL,
	[IdType] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__VeraCoreP__Attem__695C9DA1]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[VeraCoreProcessItems] ADD  DEFAULT ((0)) FOR [Attempt]
END

GO
