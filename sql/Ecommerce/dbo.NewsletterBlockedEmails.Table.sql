/****** Object:  Table [dbo].[NewsletterBlockedEmails]    Script Date: 6/25/2016 2:11:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NewsletterBlockedEmails]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[NewsletterBlockedEmails](
	[IdNewsletter] [int] NOT NULL,
	[Email] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_NewsletterBlockedEmails] PRIMARY KEY CLUSTERED 
(
	[IdNewsletter] DESC,
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Email]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[NewsletterBlockedEmails]') AND name = N'IX_Email')
CREATE NONCLUSTERED INDEX [IX_Email] ON [dbo].[NewsletterBlockedEmails]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_NewsletterBlockedEmails_ToNewsletters]') AND parent_object_id = OBJECT_ID(N'[dbo].[NewsletterBlockedEmails]'))
ALTER TABLE [dbo].[NewsletterBlockedEmails]  WITH CHECK ADD  CONSTRAINT [FK_NewsletterBlockedEmails_ToNewsletters] FOREIGN KEY([IdNewsletter])
REFERENCES [dbo].[Newsletters] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_NewsletterBlockedEmails_ToNewsletters]') AND parent_object_id = OBJECT_ID(N'[dbo].[NewsletterBlockedEmails]'))
ALTER TABLE [dbo].[NewsletterBlockedEmails] CHECK CONSTRAINT [FK_NewsletterBlockedEmails_ToNewsletters]
GO
