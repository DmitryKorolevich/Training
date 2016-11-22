IF OBJECT_ID(N'[dbo].[SkuOOSHistoryItems]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[SkuOOSHistoryItems]
	(
		[Id] INT NOT NULL IDENTITY(1,1),
		[IdSku] INT NOT NULL,
		[StartDate] DATETIME2 NOT NULL,
		[EndDate] DATETIME2 NULL,
		[IsCurrent] BIT NOT NULL,
	CONSTRAINT [PK_SkuOOSHistoryItems] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[SkuOOSHistoryItems]  WITH CHECK ADD  CONSTRAINT [FK_SkuOOSHistoryItemToSku] FOREIGN KEY([IdSku])
	REFERENCES [dbo].[Skus] ([Id])

	ALTER TABLE [dbo].[SkuOOSHistoryItems] CHECK CONSTRAINT [FK_SkuOOSHistoryItemToSku]

	CREATE NONCLUSTERED INDEX [IX_SkuOOSHistoryItems_IdSku_IsCurrent] ON [dbo].[SkuOOSHistoryItems]
	(
		[IdSku] DESC,
		[IsCurrent] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

	CREATE NONCLUSTERED INDEX [IX_SkuOOSHistoryItems_IdSku] ON [dbo].[SkuOOSHistoryItems]
	(
		[IdSku] DESC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

END

GO

--For one time use only, do not uncomment
--INSERT SkuOOSHistoryItems
--(IdSku, StartDate, IsCurrent)
--SELECT
--s.Id, GETDATE(), 1
--FROM Skus s WITH(NOLOCK)
--JOIN Products p WITH(NOLOCK) ON s.IdProduct=p.Id
--LEFT JOIN SkuOptionTypes AS dopt WITH(NOLOCK) ON dopt.Name = N'DisregardStock' AND dopt.IdObjectType=p.IdObjectType
--LEFT JOIN SkuOptionValues AS dval WITH(NOLOCK) ON dval.IdSku = s.Id AND dval.IdOptionType = dopt.Id
--LEFT JOIN SkuOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'Stock' AND sopt.IdObjectType=p.IdObjectType
--LEFT JOIN SkuOptionValues AS sval WITH(NOLOCK) ON sval.IdSku = s.Id AND sval.IdOptionType = sopt.Id
--WHERE 
--	p.IdObjectType IN (1,2) AND
--	(
--		ISNULL(dval.Value, 'False')='False' AND
--		CAST(ISNULL(sval.Value, '0') as INT)<=0
--	)
