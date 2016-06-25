/****** Object:  Table [dbo].[Countries]    Script Date: 6/25/2016 2:10:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Countries]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Countries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CountryCode] [nvarchar](3) NOT NULL,
	[CountryName] [nvarchar](250) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[Order] [int] NOT NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
UNIQUE NONCLUSTERED 
(
	[CountryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Countries_RecordStatusCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Countries]'))
ALTER TABLE [dbo].[Countries]  WITH CHECK ADD  CONSTRAINT [FK_Countries_RecordStatusCodes] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Countries_RecordStatusCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Countries]'))
ALTER TABLE [dbo].[Countries] CHECK CONSTRAINT [FK_Countries_RecordStatusCodes]
GO
