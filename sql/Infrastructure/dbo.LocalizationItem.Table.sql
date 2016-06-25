/****** Object:  Table [dbo].[LocalizationItem]    Script Date: 6/25/2016 3:40:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LocalizationItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LocalizationItem](
	[GroupId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[Comment] [nvarchar](max) NOT NULL,
	[GroupName] [nvarchar](max) NOT NULL,
	[ItemName] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_LocalizationItem] PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC,
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
