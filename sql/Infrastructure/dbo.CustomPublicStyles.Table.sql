/****** Object:  Table [dbo].[CustomPublicStyles]    Script Date: 6/25/2016 3:40:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomPublicStyles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomPublicStyles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Styles] [nvarchar](max) NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[IdEditedBy] [int] NULL,
 CONSTRAINT [PK_CustomPublicStyles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
 CONSTRAINT [UQ_CustomPublicStyles] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomPublicStyles_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomPublicStyles]'))
ALTER TABLE [dbo].[CustomPublicStyles]  WITH CHECK ADD  CONSTRAINT [FK_CustomPublicStyles_AspNetUsers] FOREIGN KEY([IdEditedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomPublicStyles_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomPublicStyles]'))
ALTER TABLE [dbo].[CustomPublicStyles] CHECK CONSTRAINT [FK_CustomPublicStyles_AspNetUsers]
GO
