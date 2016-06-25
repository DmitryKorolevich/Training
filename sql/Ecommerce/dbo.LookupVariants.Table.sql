/****** Object:  Table [dbo].[LookupVariants]    Script Date: 6/25/2016 2:11:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LookupVariants]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LookupVariants](
	[Id] [int] NOT NULL,
	[IdLookup] [int] NOT NULL,
	[ValueVariant] [nvarchar](250) NULL,
	[Order] [int] NOT NULL,
 CONSTRAINT [PK_LookupVariants] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[IdLookup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__LookupVar__Order__2AD55B43]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[LookupVariants] ADD  DEFAULT ((1)) FOR [Order]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LookupVariants_ToLookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[LookupVariants]'))
ALTER TABLE [dbo].[LookupVariants]  WITH CHECK ADD  CONSTRAINT [FK_LookupVariants_ToLookup] FOREIGN KEY([IdLookup])
REFERENCES [dbo].[Lookups] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LookupVariants_ToLookup]') AND parent_object_id = OBJECT_ID(N'[dbo].[LookupVariants]'))
ALTER TABLE [dbo].[LookupVariants] CHECK CONSTRAINT [FK_LookupVariants_ToLookup]
GO
