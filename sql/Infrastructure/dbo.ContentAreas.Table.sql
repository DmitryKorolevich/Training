/****** Object:  Table [dbo].[ContentAreas]    Script Date: 6/25/2016 3:39:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContentAreas]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ContentAreas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Template] [nvarchar](max) NULL,
	[StatusCode] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[IdEditedBy] [int] NULL,
 CONSTRAINT [PK_ContentAreas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
 CONSTRAINT [UQ_ContentArea] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentAreas_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentAreas]'))
ALTER TABLE [dbo].[ContentAreas]  WITH CHECK ADD  CONSTRAINT [FK_ContentAreas_AspNetUsers] FOREIGN KEY([IdEditedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentAreas_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentAreas]'))
ALTER TABLE [dbo].[ContentAreas] CHECK CONSTRAINT [FK_ContentAreas_AspNetUsers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentAreas_RecordStatusCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentAreas]'))
ALTER TABLE [dbo].[ContentAreas]  WITH CHECK ADD  CONSTRAINT [FK_ContentAreas_RecordStatusCodes] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ContentAreas_RecordStatusCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ContentAreas]'))
ALTER TABLE [dbo].[ContentAreas] CHECK CONSTRAINT [FK_ContentAreas_RecordStatusCodes]
GO
