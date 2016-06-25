/****** Object:  Table [dbo].[OneTimeDiscountToCustomerUsages]    Script Date: 6/25/2016 2:11:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OneTimeDiscountToCustomerUsages]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OneTimeDiscountToCustomerUsages](
	[IdCustomer] [int] NOT NULL,
	[IdDiscount] [int] NOT NULL,
	[UsageCount] [int] NOT NULL,
 CONSTRAINT [PK_DiscountsOneTimeUsage] PRIMARY KEY CLUSTERED 
(
	[IdCustomer] DESC,
	[IdDiscount] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OneTimeUsageToCustomer]') AND parent_object_id = OBJECT_ID(N'[dbo].[OneTimeDiscountToCustomerUsages]'))
ALTER TABLE [dbo].[OneTimeDiscountToCustomerUsages]  WITH CHECK ADD  CONSTRAINT [FK_OneTimeUsageToCustomer] FOREIGN KEY([IdCustomer])
REFERENCES [dbo].[Customers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OneTimeUsageToCustomer]') AND parent_object_id = OBJECT_ID(N'[dbo].[OneTimeDiscountToCustomerUsages]'))
ALTER TABLE [dbo].[OneTimeDiscountToCustomerUsages] CHECK CONSTRAINT [FK_OneTimeUsageToCustomer]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OneTimeUsageToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[OneTimeDiscountToCustomerUsages]'))
ALTER TABLE [dbo].[OneTimeDiscountToCustomerUsages]  WITH CHECK ADD  CONSTRAINT [FK_OneTimeUsageToDiscount] FOREIGN KEY([IdDiscount])
REFERENCES [dbo].[Discounts] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OneTimeUsageToDiscount]') AND parent_object_id = OBJECT_ID(N'[dbo].[OneTimeDiscountToCustomerUsages]'))
ALTER TABLE [dbo].[OneTimeDiscountToCustomerUsages] CHECK CONSTRAINT [FK_OneTimeUsageToDiscount]
GO
