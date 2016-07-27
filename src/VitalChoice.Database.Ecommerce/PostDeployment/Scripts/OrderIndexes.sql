DECLARE @date DateTime
SET @date =(SELECT TOP 1 STATS_DATE(so.object_id, index_id)
	FROM sys.indexes si
	inner join sys.tables so on so.object_id = si.object_id
	WHERE si.name='IX_IdCustomer' AND si.object_id = OBJECT_ID('Orders'))

IF('2016-07-27 15:09:00.100'>@date OR @date IS NULL)
BEGIN

IF(@date IS NOT NULL)
	DROP INDEX [IX_IdCustomer] ON [dbo].[Orders]

CREATE NONCLUSTERED INDEX [IX_IdCustomer] ON [dbo].[Orders]
(
	[IdCustomer] DESC
)
INCLUDE ( 	[Id],
	[IdObjectType],
	[DateCreated],
	[DateEdited],
	[IdEditedBy],
	[StatusCode],
	[OrderStatus],
	[IdDiscount],
	[Total],
	[ProductsSubtotal],
	[TaxTotal],
	[ShippingTotal],
	[DiscountTotal],
	[IdPaymentMethod],
	[IdShippingAddress],
	[IdAddedBy],
	[POrderStatus],
	[NPOrderStatus],
	[IdOrderSource]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

GO

DECLARE @date DateTime
SET @date =(SELECT TOP 1 STATS_DATE(so.object_id, index_id)
	FROM sys.indexes si
	inner join sys.tables so on so.object_id = si.object_id
	WHERE si.name='IX_IdCustomer_ObjectStatus_OrderStatus_ObjectType' AND si.object_id = OBJECT_ID('Orders'))

IF('2016-07-27 15:09:00.100'>@date OR @date IS NULL)
BEGIN

IF(@date IS NOT NULL)
	DROP INDEX [IX_IdCustomer_ObjectStatus_OrderStatus_ObjectType] ON [dbo].[Orders]

CREATE NONCLUSTERED INDEX [IX_IdCustomer_ObjectStatus_OrderStatus_ObjectType] ON [dbo].[Orders]
(
	[IdCustomer] DESC,
	[IdObjectType] ASC,
	[StatusCode] ASC,
	[OrderStatus] ASC,
	[POrderStatus] ASC,
	[NPOrderStatus] ASC
)
INCLUDE ( 	[Id],
	[DateCreated],
	[DateEdited],
	[IdEditedBy],
	[IdDiscount],
	[Total],
	[ProductsSubtotal],
	[TaxTotal],
	[ShippingTotal],
	[DiscountTotal],
	[IdPaymentMethod],
	[IdShippingAddress],
	[IdAddedBy],
	[IdOrderSource]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

GO

DECLARE @date DateTime
SET @date =(SELECT TOP 1 STATS_DATE(so.object_id, index_id)
	FROM sys.indexes si
	inner join sys.tables so on so.object_id = si.object_id
	WHERE si.name='IX_Orders_IdOrderSource' AND si.object_id = OBJECT_ID('Orders'))

IF('2016-07-27 15:09:00.100'>@date OR @date IS NULL)
BEGIN

IF(@date IS NOT NULL)
	DROP INDEX [IX_Orders_IdOrderSource] ON [dbo].[Orders]

CREATE NONCLUSTERED INDEX [IX_Orders_IdOrderSource] ON [dbo].[Orders]
(
	[IdOrderSource] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

GO

DECLARE @date DateTime
SET @date =(SELECT TOP 1 STATS_DATE(so.object_id, index_id)
	FROM sys.indexes si
	inner join sys.tables so on so.object_id = si.object_id
	WHERE si.name='IX_Orders_ObjectStatus_OrderDate' AND si.object_id = OBJECT_ID('Orders'))

IF('2016-07-27 15:09:00.100'>@date OR @date IS NULL)
BEGIN

IF(@date IS NOT NULL)
	DROP INDEX [IX_Orders_ObjectStatus_OrderDate] ON [dbo].[Orders]

CREATE NONCLUSTERED INDEX [IX_Orders_ObjectStatus_OrderDate] ON [dbo].[Orders]
(
	[DateCreated] DESC,
	[StatusCode] ASC
)
INCLUDE ( 	[Id],
	[IdObjectType],
	[DateEdited],
	[IdEditedBy],
	[OrderStatus],
	[IdCustomer],
	[IdDiscount],
	[Total],
	[ProductsSubtotal],
	[TaxTotal],
	[ShippingTotal],
	[DiscountTotal],
	[IdPaymentMethod],
	[IdShippingAddress],
	[IdAddedBy],
	[POrderStatus],
	[NPOrderStatus],
	[IdOrderSource]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

GO

DECLARE @date DateTime
SET @date =(SELECT TOP 1 STATS_DATE(so.object_id, index_id)
	FROM sys.indexes si
	inner join sys.tables so on so.object_id = si.object_id
	WHERE si.name='IX_Orders_OrderStatus_ObjectStatus_ObjectType_OrderDate' AND si.object_id = OBJECT_ID('Orders'))

IF('2016-07-27 15:09:00.100'>@date OR @date IS NULL)
BEGIN

IF(@date IS NOT NULL)
	DROP INDEX [IX_Orders_OrderStatus_ObjectStatus_ObjectType_OrderDate] ON [dbo].[Orders]

CREATE NONCLUSTERED INDEX [IX_Orders_OrderStatus_ObjectStatus_ObjectType_OrderDate] ON [dbo].[Orders]
(
	[DateCreated] DESC,
	[StatusCode] ASC,
	[OrderStatus] ASC,
	[IdObjectType] ASC,
	[POrderStatus] ASC,
	[NPOrderStatus] ASC
)
INCLUDE ( 	[Id],
	[DateEdited],
	[IdEditedBy],
	[IdCustomer],
	[IdDiscount],
	[Total],
	[ProductsSubtotal],
	[TaxTotal],
	[ShippingTotal],
	[DiscountTotal],
	[IdPaymentMethod],
	[IdShippingAddress],
	[IdAddedBy],
	[IdOrderSource]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

GO

DECLARE @date DateTime
SET @date =(SELECT TOP 1 STATS_DATE(so.object_id, index_id)
	FROM sys.indexes si
	inner join sys.tables so on so.object_id = si.object_id
	WHERE si.name='IX_Orders_OrderType_Status_Date' AND si.object_id = OBJECT_ID('Orders'))

IF('2016-07-27 15:09:00.100'>@date OR @date IS NULL)
BEGIN

IF(@date IS NOT NULL)
	DROP INDEX [IX_Orders_OrderType_Status_Date] ON [dbo].[Orders]

CREATE NONCLUSTERED INDEX [IX_Orders_OrderType_Status_Date] ON [dbo].[Orders]
(
	[DateCreated] DESC,
	[IdObjectType] ASC,
	[StatusCode] ASC,
	[POrderStatus] ASC,
	[NPOrderStatus] ASC
)
INCLUDE ( 	[Id],
	[OrderStatus],
	[IdCustomer],
	[IdDiscount],
	[Total],
	[DateEdited],
	[IdEditedBy],
	[ProductsSubtotal],
	[TaxTotal],
	[ShippingTotal],
	[DiscountTotal],
	[IdPaymentMethod],
	[IdShippingAddress],
	[IdAddedBy],
	[IdOrderSource]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

GO

DECLARE @date DateTime
SET @date =(SELECT TOP 1 STATS_DATE(so.object_id, index_id)
	FROM sys.indexes si
	inner join sys.tables so on so.object_id = si.object_id
	WHERE si.name='IX_IdObjectType' AND si.object_id = OBJECT_ID('Customers'))

IF('2016-07-27 15:09:00.100'>@date OR @date IS NULL)
BEGIN

IF(@date IS NOT NULL)
	DROP INDEX [IX_IdObjectType] ON [dbo].[Customers]

CREATE NONCLUSTERED INDEX [IX_IdObjectType] ON [dbo].[Customers]
(
	[IdObjectType] ASC
)
INCLUDE ( 	[Id],
	[DateCreated],
	[DateEdited],
	[IdEditedBy],
	[Email],
	[IdDefaultPaymentMethod],
	[StatusCode],
	[PublicId],
	[IdAffiliate],
	[IdProfileAddress]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

GO

DECLARE @date DateTime
SET @date =(SELECT TOP 1 STATS_DATE(so.object_id, index_id)
	FROM sys.indexes si
	inner join sys.tables so on so.object_id = si.object_id
	WHERE si.name='IX_StatusCode_DateCreated' AND si.object_id = OBJECT_ID('Customers'))

IF('2016-07-27 15:09:00.100'>@date OR @date IS NULL)
BEGIN

IF(@date IS NOT NULL)
	DROP INDEX [IX_StatusCode_DateCreated] ON [dbo].[Customers]

CREATE NONCLUSTERED INDEX [IX_StatusCode_DateCreated] ON [dbo].[Customers]
(
	[StatusCode] ASC,
	[DateCreated] DESC
)
INCLUDE ( 	[Id],
	[DateEdited],
	[IdEditedBy],
	[Email],
	[IdDefaultPaymentMethod],
	[PublicId],
	[IdAffiliate],
	[IdProfileAddress],
	[IdObjectType]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

GO

DECLARE @date DateTime
SET @date =(SELECT TOP 1 STATS_DATE(so.object_id, index_id)
	FROM sys.indexes si
	inner join sys.tables so on so.object_id = si.object_id
	WHERE si.name='IX_StatusCode' AND si.object_id = OBJECT_ID('Customers'))

IF('2016-07-27 15:09:00.100'>@date OR @date IS NULL)
BEGIN

IF(@date IS NOT NULL)
	DROP INDEX [IX_StatusCode] ON [dbo].[Customers]

CREATE NONCLUSTERED INDEX [IX_StatusCode] ON [dbo].[Customers]
(
	[StatusCode] ASC
)
INCLUDE ( 	[Id],
	[IdObjectType],
	[DateCreated],
	[DateEdited],
	[IdEditedBy],
	[Email],
	[IdDefaultPaymentMethod],
	[PublicId],
	[IdAffiliate],
	[IdProfileAddress]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

GO