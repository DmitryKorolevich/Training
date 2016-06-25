/****** Object:  Table [dbo].[BugTicketComments]    Script Date: 6/25/2016 3:39:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BugTicketComments]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BugTicketComments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdBugTicket] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[IdEditedBy] [int] NOT NULL,
	[Order] [int] NOT NULL,
	[Comment] [nvarchar](2000) NOT NULL,
	[PublicId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_BugTicketComments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_BugTicketComments_IdBugTicket]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BugTicketComments]') AND name = N'IX_BugTicketComments_IdBugTicket')
CREATE NONCLUSTERED INDEX [IX_BugTicketComments_IdBugTicket] ON [dbo].[BugTicketComments]
(
	[IdBugTicket] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugTicketCommentsToAspNetUserss]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugTicketComments]'))
ALTER TABLE [dbo].[BugTicketComments]  WITH CHECK ADD  CONSTRAINT [FK_BugTicketCommentsToAspNetUserss] FOREIGN KEY([IdEditedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugTicketCommentsToAspNetUserss]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugTicketComments]'))
ALTER TABLE [dbo].[BugTicketComments] CHECK CONSTRAINT [FK_BugTicketCommentsToAspNetUserss]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugTicketCommentsToBugTickets]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugTicketComments]'))
ALTER TABLE [dbo].[BugTicketComments]  WITH CHECK ADD  CONSTRAINT [FK_BugTicketCommentsToBugTickets] FOREIGN KEY([IdBugTicket])
REFERENCES [dbo].[BugTickets] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugTicketCommentsToBugTickets]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugTicketComments]'))
ALTER TABLE [dbo].[BugTicketComments] CHECK CONSTRAINT [FK_BugTicketCommentsToBugTickets]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugTicketCommentsToStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugTicketComments]'))
ALTER TABLE [dbo].[BugTicketComments]  WITH CHECK ADD  CONSTRAINT [FK_BugTicketCommentsToStatus] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugTicketCommentsToStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugTicketComments]'))
ALTER TABLE [dbo].[BugTicketComments] CHECK CONSTRAINT [FK_BugTicketCommentsToStatus]
GO
