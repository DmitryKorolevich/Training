DECLARE @date DateTime
SET @date =(SELECT TOP 1 STATS_DATE(so.object_id, index_id)
	FROM sys.indexes si
	inner join sys.tables so on so.object_id = si.object_id
	WHERE si.name='IX_Lookups_Name' AND si.object_id = OBJECT_ID('Lookups'))

IF('2016-08-12 00:09:00.100'>@date OR @date IS NULL)
BEGIN

IF(@date IS NOT NULL)
	DROP INDEX [IX_Lookups_Name] ON [dbo].[Lookups]

CREATE NONCLUSTERED INDEX [IX_Lookups_Name] ON [dbo].[Lookups]
(
	[Name] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

GO

DECLARE @date DateTime
SET @date =(SELECT TOP 1 STATS_DATE(so.object_id, index_id)
	FROM sys.indexes si
	inner join sys.tables so on so.object_id = si.object_id
	WHERE si.name='IX_LookupVariants_IdLookup' AND si.object_id = OBJECT_ID('LookupVariants'))

IF('2016-08-12 00:09:00.100'>@date OR @date IS NULL)
BEGIN

IF(@date IS NOT NULL)
	DROP INDEX [IX_LookupVariants_IdLookup] ON [dbo].[LookupVariants]

CREATE NONCLUSTERED INDEX [IX_LookupVariants_IdLookup] ON [dbo].[LookupVariants]
(
	[IdLookup] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

GO

DECLARE @date DateTime
SET @date =(SELECT TOP 1 STATS_DATE(so.object_id, index_id)
	FROM sys.indexes si
	inner join sys.tables so on so.object_id = si.object_id
	WHERE si.name='IX_LookupVariants_IdLookup' AND si.object_id = OBJECT_ID('LookupVariants'))

IF('2016-08-12 00:09:00.100'>@date OR @date IS NULL)
BEGIN

IF(@date IS NOT NULL)
	DROP INDEX [IX_LookupVariants_IdLookup] ON [dbo].[LookupVariants]

CREATE NONCLUSTERED INDEX [IX_LookupVariants_IdLookup] ON [dbo].[LookupVariants]
(
	[IdLookup] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

GO

DECLARE @date DateTime
SET @date =(SELECT TOP 1 STATS_DATE(so.object_id, index_id)
	FROM sys.indexes si
	inner join sys.tables so on so.object_id = si.object_id
	WHERE si.name='IX_Orders_OrderType_Status' AND si.object_id = OBJECT_ID('Orders'))

IF('2016-08-12 00:09:00.100'>@date OR @date IS NULL)
BEGIN

IF(@date IS NOT NULL)
	DROP INDEX [IX_Orders_OrderType_Status] ON [dbo].[Orders]

CREATE NONCLUSTERED INDEX [IX_Orders_OrderType_Status] ON [dbo].[Orders]
(
	[Id] ASC,
	[IdObjectType] ASC,
	[StatusCode] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

GO