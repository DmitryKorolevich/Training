/****** Object:  Table [dbo].[CustomerNotes]    Script Date: 6/25/2016 2:10:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerNotes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomerNotes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdCustomer] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[IdEditedBy] [int] NULL,
	[Note] [nvarchar](1000) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[IdAddedBy] [int] NULL,
 CONSTRAINT [PK_CustomerNotes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerNotes_Customers]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerNotes]'))
ALTER TABLE [dbo].[CustomerNotes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerNotes_Customers] FOREIGN KEY([IdCustomer])
REFERENCES [dbo].[Customers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerNotes_Customers]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerNotes]'))
ALTER TABLE [dbo].[CustomerNotes] CHECK CONSTRAINT [FK_CustomerNotes_Customers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerNotes_RecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerNotes]'))
ALTER TABLE [dbo].[CustomerNotes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerNotes_RecordStatusCode] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerNotes_RecordStatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerNotes]'))
ALTER TABLE [dbo].[CustomerNotes] CHECK CONSTRAINT [FK_CustomerNotes_RecordStatusCode]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerNotes_Users_EditedBy]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerNotes]'))
ALTER TABLE [dbo].[CustomerNotes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerNotes_Users_EditedBy] FOREIGN KEY([IdEditedBy])
REFERENCES [dbo].[Users] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerNotes_Users_EditedBy]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerNotes]'))
ALTER TABLE [dbo].[CustomerNotes] CHECK CONSTRAINT [FK_CustomerNotes_Users_EditedBy]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerNotes_Users_IdAddedBy]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerNotes]'))
ALTER TABLE [dbo].[CustomerNotes]  WITH CHECK ADD  CONSTRAINT [FK_CustomerNotes_Users_IdAddedBy] FOREIGN KEY([IdAddedBy])
REFERENCES [dbo].[Users] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerNotes_Users_IdAddedBy]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerNotes]'))
ALTER TABLE [dbo].[CustomerNotes] CHECK CONSTRAINT [FK_CustomerNotes_Users_IdAddedBy]
GO
