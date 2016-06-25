/****** Object:  Table [dbo].[ProductOutOfStockRequests]    Script Date: 6/25/2016 2:12:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductOutOfStockRequests]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ProductOutOfStockRequests](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdProduct] [int] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Email] [nvarchar](250) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ProductOu__DateC__318258D2]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ProductOutOfStockRequests] ADD  DEFAULT (sysdatetime()) FOR [DateCreated]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductOutOfStockRequests_ToProducts]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductOutOfStockRequests]'))
ALTER TABLE [dbo].[ProductOutOfStockRequests]  WITH CHECK ADD  CONSTRAINT [FK_ProductOutOfStockRequests_ToProducts] FOREIGN KEY([IdProduct])
REFERENCES [dbo].[Products] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProductOutOfStockRequests_ToProducts]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProductOutOfStockRequests]'))
ALTER TABLE [dbo].[ProductOutOfStockRequests] CHECK CONSTRAINT [FK_ProductOutOfStockRequests_ToProducts]
GO
