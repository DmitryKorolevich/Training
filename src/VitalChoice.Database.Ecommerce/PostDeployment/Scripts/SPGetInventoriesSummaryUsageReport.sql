IF OBJECT_ID(N'[dbo].[SPGetInventoriesSummaryUsageReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetInventoriesSummaryUsageReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetInventoriesSummaryUsageReport]
	@from datetime2,
	@to datetime2, 
	@sku nvarchar(250) = NULL,
	@invsku nvarchar(250) = NULL,
	@assemble bit = NULL,
	@idsinvcat nvarchar(MAX) = NULL,
	@shipdate bit = 0,
	@frequency int = 3
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @InvIds AS TABLE
    (
        Id int NOT NULL
    );

	DECLARE @orderIds AS TABLE
	(
		Id int NOT NULL,
		DateCreated Datetime2 NOT NULL
	);

	IF(@sku IS NOT NULL OR @invsku IS NOT NULL OR @assemble IS NOT NULL OR @idsinvcat IS NOT NULL)
	BEGIN
		INSERT INTO @InvIds
		(Id)
		SELECT DISTINCT sToInv.IdInventorySku 
		FROM Skus s WITH(NOLOCK)
		JOIN SkusToInventorySkus sToInv WITH(NOLOCK) ON s.Id=sToInv.IdSku
		JOIN InventorySkus inv WITH(NOLOCK) ON inv.Id=sToInv.IdInventorySku
		LEFT JOIN SkuOptionTypes AS oopt WITH(NOLOCK) ON oopt.Name = N'Assemble'
		LEFT JOIN SkuOptionValues AS oval WITH(NOLOCK) ON oval.IdSku = s.Id AND oval.IdOptionType = oopt.Id
		WHERE (@sku IS NULL OR s.Code LIKE '%'+@sku+'%') AND
		(@invsku IS NULL OR inv.Code LIKE '%'+@invsku+'%') AND
		(@idsinvcat IS NULL OR inv.IdInventorySkuCategory IN
		(SELECT Id FROM TFGetTableIdsByString(@idsinvcat, DEFAULT))) AND
		(@assemble IS NULL OR (@assemble=1 AND oval.Value='True') OR (@assemble=0 AND (oval.Value='False' OR oval.Value IS NULL)))
	END

	--monthly
	IF(@frequency=3)
	BEGIN

		IF(@shipdate=0)
		BEGIN
			SELECT dateadd(mm, (DATEPART(Year, DateCreated) - 1900) * 12 + DATEPART(Month, DateCreated) - 1 , 0) As Date,
				temp.IdInventorySku, SUM(temp.Quantity) AS Quantity
				FROM
				(SELECT o.DateCreated, osToInv.IdInventorySku, os.Quantity*osToInv.Quantity AS Quantity 
				FROM Orders o WITH(NOLOCK)
				JOIN OrderToSkus os WITH(NOLOCK) ON o.Id=os.IdOrder
				JOIN OrderToSkusToInventorySkus osToInv WITH(NOLOCK) ON os.IdOrder=osToInv.IdOrder AND os.IdSku=osToInv.IdSku
				LEFT JOIN @InvIds invId ON osToInv.IdInventorySku=invId.Id
				WHERE DateCreated>=@from AND DateCreated<=@to AND StatusCode!=3 AND 
					(OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3) AND 
					((@sku IS NULL AND @invsku IS NULL AND @assemble IS NULL AND @idsinvcat IS NULL) OR invId.Id is NOT NULL)
				UNION ALL
				SELECT o.DateCreated, opToInv.IdInventorySku, op.Quantity*opToInv.Quantity AS Quantity
				FROM Orders o WITH(NOLOCK)
				JOIN OrderToPromos op WITH(NOLOCK) ON o.Id=op.IdOrder AND op.[Disabled]=0
				JOIN OrderToPromosToInventorySkus opToInv WITH(NOLOCK) ON op.IdOrder=opToInv.IdOrder AND op.IdSku=opToInv.IdSku
				LEFT JOIN @InvIds invId ON opToInv.IdInventorySku=invId.Id
				WHERE DateCreated>=@from AND DateCreated<=@to AND StatusCode!=3 AND 
					(OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3) AND 
					((@sku IS NULL AND @invsku IS NULL AND @assemble IS NULL AND @idsinvcat IS NULL) OR invId.Id is NOT NULL)) temp
			GROUP BY DATEPART(Year, temp.DateCreated), DATEPART(Month, temp.DateCreated), temp.IdInventorySku
		END
		ELSE
		BEGIN

			INSERT INTO @orderIds
			(	
				Id,
				DateCreated
			)
			(SELECT 
				o.Id,
				MIN(osp.ShippedDate)		
			FROM Orders o WITH(NOLOCK)
			JOIN OrderShippingPackages osp WITH(NOLOCK) ON o.Id=osp.IdOrder
			WHERE osp.ShippedDate>=@from AND osp.ShippedDate<=@to AND StatusCode!=3 AND 
				(
					OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3
				)
			GROUP BY o.Id
			)

			SELECT 
				dateadd(mm, (DATEPART(Year, DateCreated) - 1900) * 12 + DATEPART(Month, DateCreated) - 1 , 0) As Date,
				temp.IdInventorySku,
				SUM(temp.Quantity) AS Quantity
			FROM
				(
				SELECT
					o.DateCreated, 
					osToInv.IdInventorySku,
					os.Quantity*osToInv.Quantity AS Quantity 
				FROM @orderIds o
				JOIN OrderToSkus os WITH(NOLOCK) ON o.Id=os.IdOrder
				JOIN OrderToSkusToInventorySkus osToInv WITH(NOLOCK) ON os.IdOrder=osToInv.IdOrder AND os.IdSku=osToInv.IdSku
				LEFT JOIN @InvIds invId ON osToInv.IdInventorySku=invId.Id
				WHERE 
					(
						(@sku IS NULL AND @invsku IS NULL AND @assemble IS NULL AND @idsinvcat IS NULL) OR
						invId.Id is NOT NULL
					)
				UNION ALL
				SELECT
					o.DateCreated,
					opToInv.IdInventorySku,
					op.Quantity*opToInv.Quantity AS Quantity
				FROM @orderIds o
				JOIN OrderToPromos op WITH(NOLOCK) ON o.Id=op.IdOrder AND op.[Disabled]=0
				JOIN OrderToPromosToInventorySkus opToInv WITH(NOLOCK) ON op.IdOrder=opToInv.IdOrder AND op.IdSku=opToInv.IdSku
				LEFT JOIN @InvIds invId ON opToInv.IdInventorySku=invId.Id
				WHERE 
					(
						(@sku IS NULL AND @invsku IS NULL AND @assemble IS NULL AND @idsinvcat IS NULL) OR
						invId.Id is NOT NULL
					)
				) temp
			GROUP BY DATEPART(Year, temp.DateCreated), DATEPART(Month, temp.DateCreated), temp.IdInventorySku

		END

	END

	--weekly
	IF(@frequency=2)
	BEGIN

		IF(@shipdate=0)
		BEGIN
			SELECT 
				dateadd(
					day,1 - datepart(dw, dateadd(yy, (DATEPART(Year, temp.DateCreated) - 1900) , 0)) + (DATEPART(week, temp.DateCreated)-1) * 7, 
					dateadd(yy, (DATEPART(Year,  temp.DateCreated) - 1900) , 0)
				) As Date,
				temp.IdInventorySku, SUM(temp.Quantity) AS Quantity
				FROM
				(SELECT o.DateCreated, osToInv.IdInventorySku, os.Quantity*osToInv.Quantity AS Quantity 
				FROM Orders o WITH(NOLOCK)
				JOIN OrderToSkus os WITH(NOLOCK) ON o.Id=os.IdOrder
				JOIN OrderToSkusToInventorySkus osToInv WITH(NOLOCK) ON os.IdOrder=osToInv.IdOrder AND os.IdSku=osToInv.IdSku
				LEFT JOIN @InvIds invId ON osToInv.IdInventorySku=invId.Id
				WHERE DateCreated>=@from AND DateCreated<=@to AND StatusCode!=3 AND 
					(OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3) AND 
					((@sku IS NULL AND @invsku IS NULL AND @assemble IS NULL AND @idsinvcat IS NULL) OR invId.Id is NOT NULL)
				UNION ALL
				SELECT o.DateCreated, opToInv.IdInventorySku, op.Quantity*opToInv.Quantity AS Quantity
				FROM Orders o WITH(NOLOCK)
				JOIN OrderToPromos op WITH(NOLOCK) ON o.Id=op.IdOrder AND op.[Disabled]=0
				JOIN OrderToPromosToInventorySkus opToInv WITH(NOLOCK) ON op.IdOrder=opToInv.IdOrder AND op.IdSku=opToInv.IdSku
				LEFT JOIN @InvIds invId ON opToInv.IdInventorySku=invId.Id
				WHERE DateCreated>=@from AND DateCreated<=@to AND StatusCode!=3 AND 
					(OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3) AND 
					((@sku IS NULL AND @invsku IS NULL AND @assemble IS NULL AND @idsinvcat IS NULL) OR invId.Id is NOT NULL)) temp
			GROUP BY DATEPART(Year, temp.DateCreated), DATEPART(Month, temp.DateCreated), DATEPART(week, temp.DateCreated), temp.IdInventorySku
		END
		ELSE
		BEGIN

			INSERT INTO @orderIds
			(	
				Id,
				DateCreated
			)
			(SELECT 
				o.Id,
				MIN(osp.ShippedDate)		
			FROM Orders o WITH(NOLOCK)
			JOIN OrderShippingPackages osp WITH(NOLOCK) ON o.Id=osp.IdOrder
			WHERE osp.ShippedDate>=@from AND osp.ShippedDate<=@to AND StatusCode!=3 AND 
				(
					OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3
				)
			GROUP BY o.Id
			)

			SELECT 
				dateadd(
					day,1 - datepart(dw, dateadd(yy, (DATEPART(Year, temp.DateCreated) - 1900) , 0)) + (DATEPART(week, temp.DateCreated)-1) * 7, 
					dateadd(yy, (DATEPART(Year,  temp.DateCreated) - 1900) , 0)
				) As Date,
				temp.IdInventorySku,
				SUM(temp.Quantity) AS Quantity
			FROM
				(
				SELECT
					o.DateCreated, 
					osToInv.IdInventorySku,
					os.Quantity*osToInv.Quantity AS Quantity 
				FROM @orderIds o
				JOIN OrderToSkus os WITH(NOLOCK) ON o.Id=os.IdOrder
				JOIN OrderToSkusToInventorySkus osToInv WITH(NOLOCK) ON os.IdOrder=osToInv.IdOrder AND os.IdSku=osToInv.IdSku
				LEFT JOIN @InvIds invId ON osToInv.IdInventorySku=invId.Id
				WHERE 
					(
						(@sku IS NULL AND @invsku IS NULL AND @assemble IS NULL AND @idsinvcat IS NULL) OR
						invId.Id is NOT NULL
					)
				UNION ALL
				SELECT
					o.DateCreated,
					opToInv.IdInventorySku,
					op.Quantity*opToInv.Quantity AS Quantity
				FROM @orderIds o
				JOIN OrderToPromos op WITH(NOLOCK) ON o.Id=op.IdOrder AND op.[Disabled]=0
				JOIN OrderToPromosToInventorySkus opToInv WITH(NOLOCK) ON op.IdOrder=opToInv.IdOrder AND op.IdSku=opToInv.IdSku
				LEFT JOIN @InvIds invId ON opToInv.IdInventorySku=invId.Id
				WHERE 
					(
						(@sku IS NULL AND @invsku IS NULL AND @assemble IS NULL AND @idsinvcat IS NULL) OR
						invId.Id is NOT NULL
					)
				) temp
			GROUP BY DATEPART(Year, temp.DateCreated), DATEPART(Month, temp.DateCreated), DATEPART(week, temp.DateCreated), temp.IdInventorySku

		END

	END

	--annual
	IF(@frequency=4)
	BEGIN

		IF(@shipdate=0)
		BEGIN
			SELECT 
				dateadd(yy, (DATEPART(Year,  temp.DateCreated) - 1900) , 0) As Date,
				temp.IdInventorySku, SUM(temp.Quantity) AS Quantity
				FROM
				(SELECT o.DateCreated, osToInv.IdInventorySku, os.Quantity*osToInv.Quantity AS Quantity 
				FROM Orders o WITH(NOLOCK)
				JOIN OrderToSkus os WITH(NOLOCK) ON o.Id=os.IdOrder
				JOIN OrderToSkusToInventorySkus osToInv WITH(NOLOCK) ON os.IdOrder=osToInv.IdOrder AND os.IdSku=osToInv.IdSku
				LEFT JOIN @InvIds invId ON osToInv.IdInventorySku=invId.Id
				WHERE DateCreated>=@from AND DateCreated<=@to AND StatusCode!=3 AND 
					(OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3) AND 
					((@sku IS NULL AND @invsku IS NULL AND @assemble IS NULL AND @idsinvcat IS NULL) OR invId.Id is NOT NULL)
				UNION ALL
				SELECT o.DateCreated, opToInv.IdInventorySku, op.Quantity*opToInv.Quantity AS Quantity
				FROM Orders o WITH(NOLOCK)
				JOIN OrderToPromos op WITH(NOLOCK) ON o.Id=op.IdOrder AND op.[Disabled]=0
				JOIN OrderToPromosToInventorySkus opToInv WITH(NOLOCK) ON op.IdOrder=opToInv.IdOrder AND op.IdSku=opToInv.IdSku
				LEFT JOIN @InvIds invId ON opToInv.IdInventorySku=invId.Id
				WHERE DateCreated>=@from AND DateCreated<=@to AND StatusCode!=3 AND 
					(OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3) AND 
					((@sku IS NULL AND @invsku IS NULL AND @assemble IS NULL AND @idsinvcat IS NULL) OR invId.Id is NOT NULL)) temp
			GROUP BY DATEPART(Year, temp.DateCreated), temp.IdInventorySku
		END
		ELSE
		BEGIN

			INSERT INTO @orderIds
			(	
				Id,
				DateCreated
			)
			(SELECT 
				o.Id,
				MIN(osp.ShippedDate)		
			FROM Orders o WITH(NOLOCK)
			JOIN OrderShippingPackages osp WITH(NOLOCK) ON o.Id=osp.IdOrder
			WHERE osp.ShippedDate>=@from AND osp.ShippedDate<=@to AND StatusCode!=3 AND 
				(
					OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3
				)
			GROUP BY o.Id
			)

			SELECT 
				dateadd(yy, (DATEPART(Year,  temp.DateCreated) - 1900) , 0) As Date,
				temp.IdInventorySku,
				SUM(temp.Quantity) AS Quantity
			FROM
				(
				SELECT
					o.DateCreated, 
					osToInv.IdInventorySku,
					os.Quantity*osToInv.Quantity AS Quantity 
				FROM @orderIds o
				JOIN OrderToSkus os WITH(NOLOCK) ON o.Id=os.IdOrder
				JOIN OrderToSkusToInventorySkus osToInv WITH(NOLOCK) ON os.IdOrder=osToInv.IdOrder AND os.IdSku=osToInv.IdSku
				LEFT JOIN @InvIds invId ON osToInv.IdInventorySku=invId.Id
				WHERE 
					(
						(@sku IS NULL AND @invsku IS NULL AND @assemble IS NULL AND @idsinvcat IS NULL) OR
						invId.Id is NOT NULL
					)
				UNION ALL
				SELECT
					o.DateCreated,
					opToInv.IdInventorySku,
					op.Quantity*opToInv.Quantity AS Quantity
				FROM @orderIds o
				JOIN OrderToPromos op WITH(NOLOCK) ON o.Id=op.IdOrder AND op.[Disabled]=0
				JOIN OrderToPromosToInventorySkus opToInv WITH(NOLOCK) ON op.IdOrder=opToInv.IdOrder AND op.IdSku=opToInv.IdSku
				LEFT JOIN @InvIds invId ON opToInv.IdInventorySku=invId.Id
				WHERE 
					(
						(@sku IS NULL AND @invsku IS NULL AND @assemble IS NULL AND @idsinvcat IS NULL) OR
						invId.Id is NOT NULL
					)
				) temp
			GROUP BY DATEPART(Year, temp.DateCreated), temp.IdInventorySku

		END

	END

END

GO