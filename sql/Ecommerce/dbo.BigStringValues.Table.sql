/****** Object:  Table [dbo].[BigStringValues]    Script Date: 6/25/2016 2:10:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BigStringValues]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BigStringValues](
	[IdBigString] [bigint] IDENTITY(1,1) NOT NULL,
	[Value] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdBigString] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
