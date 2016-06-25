/****** Object:  Table [dbo].[RefundSkus]    Script Date: 6/25/2016 2:13:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefundSkus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RefundSkus](
	[IdOrder] [int] NOT NULL,
	[IdSku] [int] NOT NULL,
	[Redeem] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[RefundValue] [money] NOT NULL,
	[RefundPercent] [float] NOT NULL,
	[RefundPrice] [money] NOT NULL,
 CONSTRAINT [PK_RefundSkus] PRIMARY KEY CLUSTERED 
(
	[IdOrder] DESC,
	[IdSku] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RefundSkusToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[RefundSkus]'))
ALTER TABLE [dbo].[RefundSkus]  WITH CHECK ADD  CONSTRAINT [FK_RefundSkusToOrder] FOREIGN KEY([IdOrder])
REFERENCES [dbo].[Orders] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RefundSkusToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[RefundSkus]'))
ALTER TABLE [dbo].[RefundSkus] CHECK CONSTRAINT [FK_RefundSkusToOrder]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RefundSkusToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[RefundSkus]'))
ALTER TABLE [dbo].[RefundSkus]  WITH CHECK ADD  CONSTRAINT [FK_RefundSkusToSku] FOREIGN KEY([IdSku])
REFERENCES [dbo].[Skus] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RefundSkusToSku]') AND parent_object_id = OBJECT_ID(N'[dbo].[RefundSkus]'))
ALTER TABLE [dbo].[RefundSkus] CHECK CONSTRAINT [FK_RefundSkusToSku]
GO
