/****** Object:  Table [dbo].[ContentTypes]    Script Date: 6/25/2016 3:40:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContentTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ContentTypes](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[DefaultMasterContentItemId] [int] NULL,
 CONSTRAINT [PK_ContentTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
