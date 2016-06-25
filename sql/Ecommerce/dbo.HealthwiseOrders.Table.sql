/****** Object:  Table [dbo].[HealthwiseOrders]    Script Date: 6/25/2016 2:11:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HealthwiseOrders]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[HealthwiseOrders](
	[Id] [int] NOT NULL,
	[IdHealthwisePeriod] [int] NOT NULL,
 CONSTRAINT [PK_HealthwiseOrders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_HealthwiseOrders_IdHealthwisePeriod]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[HealthwiseOrders]') AND name = N'IX_HealthwiseOrders_IdHealthwisePeriod')
CREATE NONCLUSTERED INDEX [IX_HealthwiseOrders_IdHealthwisePeriod] ON [dbo].[HealthwiseOrders]
(
	[IdHealthwisePeriod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_HealthwiseOrderToHealthwisePeriod]') AND parent_object_id = OBJECT_ID(N'[dbo].[HealthwiseOrders]'))
ALTER TABLE [dbo].[HealthwiseOrders]  WITH CHECK ADD  CONSTRAINT [FK_HealthwiseOrderToHealthwisePeriod] FOREIGN KEY([IdHealthwisePeriod])
REFERENCES [dbo].[HealthwisePeriods] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_HealthwiseOrderToHealthwisePeriod]') AND parent_object_id = OBJECT_ID(N'[dbo].[HealthwiseOrders]'))
ALTER TABLE [dbo].[HealthwiseOrders] CHECK CONSTRAINT [FK_HealthwiseOrderToHealthwisePeriod]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_HealthwiseOrderToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[HealthwiseOrders]'))
ALTER TABLE [dbo].[HealthwiseOrders]  WITH CHECK ADD  CONSTRAINT [FK_HealthwiseOrderToOrder] FOREIGN KEY([Id])
REFERENCES [dbo].[Orders] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_HealthwiseOrderToOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[HealthwiseOrders]'))
ALTER TABLE [dbo].[HealthwiseOrders] CHECK CONSTRAINT [FK_HealthwiseOrderToOrder]
GO
