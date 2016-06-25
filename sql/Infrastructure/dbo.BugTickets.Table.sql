/****** Object:  Table [dbo].[BugTickets]    Script Date: 6/25/2016 3:39:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BugTickets]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BugTickets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[IdAddedBy] [int] NOT NULL,
	[IdEditedBy] [int] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[Priority] [int] NOT NULL,
	[Summary] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](2000) NOT NULL,
	[PublicId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_BugTickets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
 CONSTRAINT [UQ_BugTicketComments_PublicId] UNIQUE NONCLUSTERED 
(
	[PublicId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
 CONSTRAINT [UQ_BugTickets_PublicId] UNIQUE NONCLUSTERED 
(
	[PublicId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_BugTickets_DateCreated]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BugTickets]') AND name = N'IX_BugTickets_DateCreated')
CREATE NONCLUSTERED INDEX [IX_BugTickets_DateCreated] ON [dbo].[BugTickets]
(
	[DateCreated] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_BugTickets_DateEdited]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BugTickets]') AND name = N'IX_BugTickets_DateEdited')
CREATE NONCLUSTERED INDEX [IX_BugTickets_DateEdited] ON [dbo].[BugTickets]
(
	[DateEdited] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugTicketsAddedByToAspNetUserss]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugTickets]'))
ALTER TABLE [dbo].[BugTickets]  WITH CHECK ADD  CONSTRAINT [FK_BugTicketsAddedByToAspNetUserss] FOREIGN KEY([IdAddedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugTicketsAddedByToAspNetUserss]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugTickets]'))
ALTER TABLE [dbo].[BugTickets] CHECK CONSTRAINT [FK_BugTicketsAddedByToAspNetUserss]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugTicketsEditedByToAspNetUserss]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugTickets]'))
ALTER TABLE [dbo].[BugTickets]  WITH CHECK ADD  CONSTRAINT [FK_BugTicketsEditedByToAspNetUserss] FOREIGN KEY([IdEditedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BugTicketsEditedByToAspNetUserss]') AND parent_object_id = OBJECT_ID(N'[dbo].[BugTickets]'))
ALTER TABLE [dbo].[BugTickets] CHECK CONSTRAINT [FK_BugTicketsEditedByToAspNetUserss]
GO
