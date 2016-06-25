/****** Object:  Table [dbo].[HelpTickets]    Script Date: 6/25/2016 2:11:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HelpTickets]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[HelpTickets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdOrder] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[Priority] [int] NOT NULL,
	[Summary] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](2000) NOT NULL,
 CONSTRAINT [PK_HelpTickets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_HelpTickets_DateCreated]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[HelpTickets]') AND name = N'IX_HelpTickets_DateCreated')
CREATE NONCLUSTERED INDEX [IX_HelpTickets_DateCreated] ON [dbo].[HelpTickets]
(
	[DateCreated] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_HelpTickets_DateEdited]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[HelpTickets]') AND name = N'IX_HelpTickets_DateEdited')
CREATE NONCLUSTERED INDEX [IX_HelpTickets_DateEdited] ON [dbo].[HelpTickets]
(
	[DateEdited] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_HelpTicketsToOrders]') AND parent_object_id = OBJECT_ID(N'[dbo].[HelpTickets]'))
ALTER TABLE [dbo].[HelpTickets]  WITH CHECK ADD  CONSTRAINT [FK_HelpTicketsToOrders] FOREIGN KEY([IdOrder])
REFERENCES [dbo].[Orders] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_HelpTicketsToOrders]') AND parent_object_id = OBJECT_ID(N'[dbo].[HelpTickets]'))
ALTER TABLE [dbo].[HelpTickets] CHECK CONSTRAINT [FK_HelpTicketsToOrders]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_HelpTicketsToStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[HelpTickets]'))
ALTER TABLE [dbo].[HelpTickets]  WITH CHECK ADD  CONSTRAINT [FK_HelpTicketsToStatus] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_HelpTicketsToStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[HelpTickets]'))
ALTER TABLE [dbo].[HelpTickets] CHECK CONSTRAINT [FK_HelpTicketsToStatus]
GO
