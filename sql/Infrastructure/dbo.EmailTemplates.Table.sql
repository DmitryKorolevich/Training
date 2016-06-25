/****** Object:  Table [dbo].[EmailTemplates]    Script Date: 6/25/2016 3:40:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailTemplates]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EmailTemplates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[ContentItemId] [int] NOT NULL,
	[MasterContentItemId] [int] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[UserId] [int] NULL,
	[ModelType] [nvarchar](250) NULL,
	[EmailDescription] [nvarchar](250) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_EmailTemplates_Name]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[EmailTemplates]') AND name = N'IX_EmailTemplates_Name')
CREATE NONCLUSTERED INDEX [IX_EmailTemplates_Name] ON [dbo].[EmailTemplates]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EmailTemplates_ToContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[EmailTemplates]'))
ALTER TABLE [dbo].[EmailTemplates]  WITH CHECK ADD  CONSTRAINT [FK_EmailTemplates_ToContentItem] FOREIGN KEY([ContentItemId])
REFERENCES [dbo].[ContentItems] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EmailTemplates_ToContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[EmailTemplates]'))
ALTER TABLE [dbo].[EmailTemplates] CHECK CONSTRAINT [FK_EmailTemplates_ToContentItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EmailTemplates_ToMasterContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[EmailTemplates]'))
ALTER TABLE [dbo].[EmailTemplates]  WITH CHECK ADD  CONSTRAINT [FK_EmailTemplates_ToMasterContentItem] FOREIGN KEY([MasterContentItemId])
REFERENCES [dbo].[MasterContentItems] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EmailTemplates_ToMasterContentItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[EmailTemplates]'))
ALTER TABLE [dbo].[EmailTemplates] CHECK CONSTRAINT [FK_EmailTemplates_ToMasterContentItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EmailTemplateToAspNetUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[EmailTemplates]'))
ALTER TABLE [dbo].[EmailTemplates]  WITH CHECK ADD  CONSTRAINT [FK_EmailTemplateToAspNetUser] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EmailTemplateToAspNetUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[EmailTemplates]'))
ALTER TABLE [dbo].[EmailTemplates] CHECK CONSTRAINT [FK_EmailTemplateToAspNetUser]
GO
