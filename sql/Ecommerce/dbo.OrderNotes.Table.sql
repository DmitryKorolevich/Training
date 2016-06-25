/****** Object:  Table [dbo].[OrderNotes]    Script Date: 6/25/2016 2:11:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderNotes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderNotes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](1000) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[IdEditedBy] [int] NULL,
 CONSTRAINT [PK_OrderNotes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderNotes_RecordStatusCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderNotes]'))
ALTER TABLE [dbo].[OrderNotes]  WITH CHECK ADD  CONSTRAINT [FK_OrderNotes_RecordStatusCodes] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderNotes_RecordStatusCodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderNotes]'))
ALTER TABLE [dbo].[OrderNotes] CHECK CONSTRAINT [FK_OrderNotes_RecordStatusCodes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderNotes_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderNotes]'))
ALTER TABLE [dbo].[OrderNotes]  WITH CHECK ADD  CONSTRAINT [FK_OrderNotes_Users] FOREIGN KEY([IdEditedBy])
REFERENCES [dbo].[Users] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderNotes_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderNotes]'))
ALTER TABLE [dbo].[OrderNotes] CHECK CONSTRAINT [FK_OrderNotes_Users]
GO
