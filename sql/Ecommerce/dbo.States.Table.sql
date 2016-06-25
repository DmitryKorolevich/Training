/****** Object:  Table [dbo].[States]    Script Date: 6/25/2016 2:13:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[States]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[States](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StateCode] [nvarchar](3) NOT NULL,
	[CountryCode] [nvarchar](3) NOT NULL,
	[StateName] [nvarchar](250) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[Order] [int] NOT NULL,
 CONSTRAINT [PK_States] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_States_Countries]') AND parent_object_id = OBJECT_ID(N'[dbo].[States]'))
ALTER TABLE [dbo].[States]  WITH CHECK ADD  CONSTRAINT [FK_States_Countries] FOREIGN KEY([CountryCode])
REFERENCES [dbo].[Countries] ([CountryCode])
ON UPDATE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_States_Countries]') AND parent_object_id = OBJECT_ID(N'[dbo].[States]'))
ALTER TABLE [dbo].[States] CHECK CONSTRAINT [FK_States_Countries]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_States_RecordStatusCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[States]'))
ALTER TABLE [dbo].[States]  WITH CHECK ADD  CONSTRAINT [FK_States_RecordStatusCodes] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_States_RecordStatusCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[States]'))
ALTER TABLE [dbo].[States] CHECK CONSTRAINT [FK_States_RecordStatusCodes]
GO
