/****** Object:  Table [dbo].[Tokens]    Script Date: 6/25/2016 3:41:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tokens]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Tokens](
	[IdToken] [uniqueidentifier] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateExpired] [datetime2](7) NOT NULL,
	[IdTokenType] [int] NOT NULL,
	[Data] [nvarchar](max) NOT NULL
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Tokens_Id]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Tokens] ADD  CONSTRAINT [DF_Tokens_Id]  DEFAULT (newid()) FOR [IdToken]
END

GO
