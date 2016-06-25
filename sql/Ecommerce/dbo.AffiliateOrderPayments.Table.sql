/****** Object:  Table [dbo].[AffiliateOrderPayments]    Script Date: 6/25/2016 2:10:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AffiliateOrderPayments]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AffiliateOrderPayments](
	[Id] [int] NOT NULL,
	[IdAffiliate] [int] NOT NULL,
	[Amount] [money] NOT NULL,
	[Status] [int] NOT NULL,
	[IdAffiliatePayment] [int] NULL,
	[NewCustomerOrder] [bit] NOT NULL,
 CONSTRAINT [PK_AffiliateOrderPayments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_AffiliateOrderPayments_IdAffiliate_Status]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AffiliateOrderPayments]') AND name = N'IX_AffiliateOrderPayments_IdAffiliate_Status')
CREATE NONCLUSTERED INDEX [IX_AffiliateOrderPayments_IdAffiliate_Status] ON [dbo].[AffiliateOrderPayments]
(
	[IdAffiliate] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_AffiliateOrderPayments_IdAffiliatePayment]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AffiliateOrderPayments]') AND name = N'IX_AffiliateOrderPayments_IdAffiliatePayment')
CREATE NONCLUSTERED INDEX [IX_AffiliateOrderPayments_IdAffiliatePayment] ON [dbo].[AffiliateOrderPayments]
(
	[IdAffiliatePayment] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Affiliate__Statu__16CE6296]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AffiliateOrderPayments] ADD  DEFAULT ((1)) FOR [Status]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Affiliate__NewCu__17C286CF]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AffiliateOrderPayments] ADD  DEFAULT ((0)) FOR [NewCustomerOrder]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOrderPayments_Orders]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOrderPayments]'))
ALTER TABLE [dbo].[AffiliateOrderPayments]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateOrderPayments_Orders] FOREIGN KEY([Id])
REFERENCES [dbo].[Orders] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOrderPayments_Orders]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOrderPayments]'))
ALTER TABLE [dbo].[AffiliateOrderPayments] CHECK CONSTRAINT [FK_AffiliateOrderPayments_Orders]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOrderPaymentToAffiliate]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOrderPayments]'))
ALTER TABLE [dbo].[AffiliateOrderPayments]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateOrderPaymentToAffiliate] FOREIGN KEY([IdAffiliate])
REFERENCES [dbo].[Affiliates] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOrderPaymentToAffiliate]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOrderPayments]'))
ALTER TABLE [dbo].[AffiliateOrderPayments] CHECK CONSTRAINT [FK_AffiliateOrderPaymentToAffiliate]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOrderPaymentToAffiliatePayment]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOrderPayments]'))
ALTER TABLE [dbo].[AffiliateOrderPayments]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateOrderPaymentToAffiliatePayment] FOREIGN KEY([IdAffiliatePayment])
REFERENCES [dbo].[AffiliatePayments] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AffiliateOrderPaymentToAffiliatePayment]') AND parent_object_id = OBJECT_ID(N'[dbo].[AffiliateOrderPayments]'))
ALTER TABLE [dbo].[AffiliateOrderPayments] CHECK CONSTRAINT [FK_AffiliateOrderPaymentToAffiliatePayment]
GO
