/****** Object:  Table [dbo].[ArticleBonusLinks]    Script Date: 6/25/2016 3:39:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArticleBonusLinks]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ArticleBonusLinks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](250) NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ArticleBonusLinks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
