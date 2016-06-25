/****** Object:  Table [dbo].[ReshipProblemSkus]    Script Date: 6/25/2016 2:13:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReshipProblemSkus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ReshipProblemSkus](
	[IdOrder] [int] NOT NULL,
	[IdSku] [int] NOT NULL,
 CONSTRAINT [PK_ReshipProblemSkus] PRIMARY KEY CLUSTERED 
(
	[IdOrder] DESC,
	[IdSku] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReshipProblemSkusToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReshipProblemSkus]'))
ALTER TABLE [dbo].[ReshipProblemSkus]  WITH CHECK ADD  CONSTRAINT [FK_ReshipProblemSkusToOrder] FOREIGN KEY([IdOrder])
REFERENCES [dbo].[Orders] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReshipProblemSkusToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReshipProblemSkus]'))
ALTER TABLE [dbo].[ReshipProblemSkus] CHECK CONSTRAINT [FK_ReshipProblemSkusToOrder]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReshipProblemSkusToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReshipProblemSkus]'))
ALTER TABLE [dbo].[ReshipProblemSkus]  WITH CHECK ADD  CONSTRAINT [FK_ReshipProblemSkusToSku] FOREIGN KEY([IdSku])
REFERENCES [dbo].[Skus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReshipProblemSkusToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReshipProblemSkus]'))
ALTER TABLE [dbo].[ReshipProblemSkus] CHECK CONSTRAINT [FK_ReshipProblemSkusToSku]
GO
