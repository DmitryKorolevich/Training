/****** Object:  Table [dbo].[ContentCrossSells]    Script Date: 6/25/2016 3:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContentCrossSells]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ContentCrossSells](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [tinyint] NOT NULL,
	[Title] [nvarchar](250) NOT NULL,
	[ImageUrl] [nvarchar](250) NOT NULL,
	[IdSku] [int] NOT NULL,
	[IdEditedBy] [int] NULL,
	[Order] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ContentCrossSells] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
