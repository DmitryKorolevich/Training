GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = 'Quantity' AND [object_id] = OBJECT_ID(N'[dbo].[OrderShippingPackages]', N'U'))
BEGIN
	
	ALTER TABLE OrderShippingPackages
	ADD Quantity INT NULL
END

GO

IF(0=(SELECT top 1 COLUMNPROPERTY(OBJECT_ID('OrderShippingPackages', 'U'), 'IdSku', 'AllowsNull')))
BEGIN

ALTER TABLE [dbo].[OrderShippingPackages] DROP CONSTRAINT [FK_OrderShippingPackageToSku]
ALTER TABLE [dbo].[OrderShippingPackages] DROP CONSTRAINT [FK_OrderShippingPackageToOrder]
ALTER TABLE [dbo].[OrderShippingPackages] DROP CONSTRAINT [PK_OrderShippingPackages]
DROP INDEX [IX_OrderShippingPackage_IdOrder] ON [dbo].[OrderShippingPackages]
DROP INDEX [IX_OrderShippingPackage_IdOrder_POrderType] ON [dbo].[OrderShippingPackages]
DROP INDEX [IX_OrderShippingPackage_ShippedDate] ON [dbo].[OrderShippingPackages]

EXEC sp_rename 'OrderShippingPackages', 'OrderShippingPackagesOld';  

CREATE TABLE [dbo].[OrderShippingPackages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdOrder] [int] NOT NULL,
	[IdSku] [int] NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[POrderType] [int] NULL,
	[ShipMethodFreightCarrier] [nvarchar](500) NOT NULL,
	[ShipMethodFreightService] [nvarchar](500) NOT NULL,
	[ShippedDate] [datetime2](7) NOT NULL,
	[TrackingNumber] [nvarchar](500) NOT NULL,
	[UPSServiceCode] [nvarchar](500) NOT NULL,
	[IdWarehouse] [int] NOT NULL,
	[Quantity] [int] NULL,
 CONSTRAINT [PK_OrderShippingPackages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[OrderShippingPackages]  WITH CHECK ADD  CONSTRAINT [FK_OrderShippingPackageToOrder] FOREIGN KEY([IdOrder])
REFERENCES [dbo].[Orders] ([Id])

ALTER TABLE [dbo].[OrderShippingPackages] CHECK CONSTRAINT [FK_OrderShippingPackageToOrder]

ALTER TABLE [dbo].[OrderShippingPackages]  WITH CHECK ADD  CONSTRAINT [FK_OrderShippingPackageToSku] FOREIGN KEY([IdSku])
REFERENCES [dbo].[Skus] ([Id])

ALTER TABLE [dbo].[OrderShippingPackages] CHECK CONSTRAINT [FK_OrderShippingPackageToSku]

CREATE NONCLUSTERED INDEX [IX_OrderShippingPackage_IdOrder] ON [dbo].[OrderShippingPackages]
(
	[IdOrder] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
CREATE NONCLUSTERED INDEX [IX_OrderShippingPackage_IdOrder_POrderType] ON [dbo].[OrderShippingPackages]
(
	[IdOrder] DESC,
	[POrderType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
CREATE NONCLUSTERED INDEX [IX_OrderShippingPackage_ShippedDate] ON [dbo].[OrderShippingPackages]
(
	[ShippedDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

--INSERT INTO [dbo].[OrderShippingPackages]
--           ([IdOrder]
--           ,[IdSku]
--           ,[DateCreated]
--           ,[POrderType]
--           ,[ShipMethodFreightCarrier]
--           ,[ShipMethodFreightService]
--           ,[ShippedDate]
--           ,[TrackingNumber]
--           ,[UPSServiceCode]
--           ,[IdWarehouse]
--           ,[Quantity])
--(SELECT [IdOrder]
--      ,[IdSku]
--      ,[DateCreated]
--      ,[POrderType]
--      ,[ShipMethodFreightCarrier]
--      ,[ShipMethodFreightService]
--      ,[ShippedDate]
--      ,[TrackingNumber]
--      ,[UPSServiceCode]
--      ,[IdWarehouse]
--      ,[Quantity]
--  FROM [OrderShippingPackagesOld])

END

GO

