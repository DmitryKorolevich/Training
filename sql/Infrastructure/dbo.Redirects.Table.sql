/****** Object:  Table [dbo].[Redirects]    Script Date: 6/25/2016 3:40:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Redirects]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Redirects](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[From] [nvarchar](250) NOT NULL,
	[To] [nvarchar](250) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[IdAddedBy] [int] NULL,
	[IdEditedBy] [int] NULL,
 CONSTRAINT [PK_Redirects] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RedirectsToAddedUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[Redirects]'))
ALTER TABLE [dbo].[Redirects]  WITH CHECK ADD  CONSTRAINT [FK_RedirectsToAddedUser] FOREIGN KEY([IdAddedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RedirectsToAddedUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[Redirects]'))
ALTER TABLE [dbo].[Redirects] CHECK CONSTRAINT [FK_RedirectsToAddedUser]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RedirectsToEditedUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[Redirects]'))
ALTER TABLE [dbo].[Redirects]  WITH CHECK ADD  CONSTRAINT [FK_RedirectsToEditedUser] FOREIGN KEY([IdEditedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RedirectsToEditedUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[Redirects]'))
ALTER TABLE [dbo].[Redirects] CHECK CONSTRAINT [FK_RedirectsToEditedUser]
GO
