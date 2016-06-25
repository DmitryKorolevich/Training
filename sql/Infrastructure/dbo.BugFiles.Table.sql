/****** Object:  Table [dbo].[BugFiles]    Script Date: 6/25/2016 3:39:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BugFiles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BugFiles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdBugTicket] [int] NULL,
	[IdBugTicketComment] [int] NULL,
	[UploadDate] [datetime2](7) NOT NULL,
	[FileName] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_BugFiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugFiles_BugTicketComments]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugFiles]'))
ALTER TABLE [dbo].[BugFiles]  WITH CHECK ADD  CONSTRAINT [FK_BugFiles_BugTicketComments] FOREIGN KEY([IdBugTicketComment])
REFERENCES [dbo].[BugTicketComments] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugFiles_BugTicketComments]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugFiles]'))
ALTER TABLE [dbo].[BugFiles] CHECK CONSTRAINT [FK_BugFiles_BugTicketComments]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugFiles_BugTickets]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugFiles]'))
ALTER TABLE [dbo].[BugFiles]  WITH CHECK ADD  CONSTRAINT [FK_BugFiles_BugTickets] FOREIGN KEY([IdBugTicket])
REFERENCES [dbo].[BugTickets] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugFiles_BugTickets]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugFiles]'))
ALTER TABLE [dbo].[BugFiles] CHECK CONSTRAINT [FK_BugFiles_BugTickets]
GO
