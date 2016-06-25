/****** Object:  Table [dbo].[Newsletters]    Script Date: 6/25/2016 2:11:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Newsletters]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Newsletters](
	[Id] [int] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Newslette__Statu__2BC97F7C]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Newsletters] ADD  DEFAULT ((2)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Newsletters_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[Newsletters]'))
ALTER TABLE [dbo].[Newsletters]  WITH CHECK ADD  CONSTRAINT [FK_Newsletters_ToRecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Newsletters_ToRecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[Newsletters]'))
ALTER TABLE [dbo].[Newsletters] CHECK CONSTRAINT [FK_Newsletters_ToRecordStatusCode]
GO
