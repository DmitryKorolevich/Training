/****** Object:  Table [dbo].[Affiliates]    Script Date: 6/25/2016 2:10:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Affiliates]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Affiliates](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[MyAppBalance] [money] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateEdited] [datetime2](7) NOT NULL,
	[IdEditedBy] [int] NULL,
	[CommissionFirst] [decimal](5, 2) NOT NULL,
	[CommissionAll] [decimal](5, 2) NOT NULL,
	[IdCountry] [int] NOT NULL,
	[IdState] [int] NULL,
	[County] [nvarchar](250) NULL,
	[Email] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Affiliates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Affiliate__MyApp__18B6AB08]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Affiliates] ADD  DEFAULT ((0)) FOR [MyAppBalance]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Affiliate__Commi__19AACF41]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Affiliates] ADD  DEFAULT ((0)) FOR [CommissionFirst]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Affiliate__Commi__1A9EF37A]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Affiliates] ADD  DEFAULT ((0)) FOR [CommissionAll]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Affiliate__Email__1B9317B3]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Affiliates] ADD  DEFAULT ('') FOR [Email]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Affiliates_Countries]') AND parent_object_id = OBJECT_ID(N'[dbo].[Affiliates]'))
ALTER TABLE [dbo].[Affiliates]  WITH CHECK ADD  CONSTRAINT [FK_Affiliates_Countries] FOREIGN KEY([IdCountry])
REFERENCES [dbo].[Countries] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Affiliates_Countries]') AND parent_object_id = OBJECT_ID(N'[dbo].[Affiliates]'))
ALTER TABLE [dbo].[Affiliates] CHECK CONSTRAINT [FK_Affiliates_Countries]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Affiliates_States]') AND parent_object_id = OBJECT_ID(N'[dbo].[Affiliates]'))
ALTER TABLE [dbo].[Affiliates]  WITH CHECK ADD  CONSTRAINT [FK_Affiliates_States] FOREIGN KEY([IdState])
REFERENCES [dbo].[States] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Affiliates_States]') AND parent_object_id = OBJECT_ID(N'[dbo].[Affiliates]'))
ALTER TABLE [dbo].[Affiliates] CHECK CONSTRAINT [FK_Affiliates_States]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliatesToUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[Affiliates]'))
ALTER TABLE [dbo].[Affiliates]  WITH CHECK ADD  CONSTRAINT [FK_AffiliatesToUser] FOREIGN KEY([IdEditedBy])
REFERENCES [dbo].[Users] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliatesToUser]') AND parent_object_id = OBJECT_ID(N'[dbo].[Affiliates]'))
ALTER TABLE [dbo].[Affiliates] CHECK CONSTRAINT [FK_AffiliatesToUser]
GO
