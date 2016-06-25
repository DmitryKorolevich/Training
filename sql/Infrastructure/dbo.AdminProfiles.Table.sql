/****** Object:  Table [dbo].[AdminProfiles]    Script Date: 6/25/2016 3:39:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AdminProfiles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AdminProfiles](
	[Id] [int] NOT NULL,
	[AgentId] [nvarchar](15) NOT NULL,
	[ConfirmationToken] [uniqueidentifier] NULL,
	[TokenExpirationDate] [datetime2](7) NULL,
	[IsConfirmed] [bit] NULL,
	[IdAdminTeam] [int] NULL,
 CONSTRAINT [PK_AdminProfiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AdminProfiles_AdminTeams]') AND parent_object_id = OBJECT_ID(N'[dbo].[AdminProfiles]'))
ALTER TABLE [dbo].[AdminProfiles]  WITH CHECK ADD  CONSTRAINT [FK_AdminProfiles_AdminTeams] FOREIGN KEY([IdAdminTeam])
REFERENCES [dbo].[AdminTeams] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AdminProfiles_AdminTeams]') AND parent_object_id = OBJECT_ID(N'[dbo].[AdminProfiles]'))
ALTER TABLE [dbo].[AdminProfiles] CHECK CONSTRAINT [FK_AdminProfiles_AdminTeams]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AdminProfiles_AspNetUsers_Id]') AND parent_object_id = OBJECT_ID(N'[dbo].[AdminProfiles]'))
ALTER TABLE [dbo].[AdminProfiles]  WITH CHECK ADD  CONSTRAINT [FK_AdminProfiles_AspNetUsers_Id] FOREIGN KEY([Id])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AdminProfiles_AspNetUsers_Id]') AND parent_object_id = OBJECT_ID(N'[dbo].[AdminProfiles]'))
ALTER TABLE [dbo].[AdminProfiles] CHECK CONSTRAINT [FK_AdminProfiles_AspNetUsers_Id]
GO
