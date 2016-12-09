IF OBJECT_ID(N'[dbo].[SPGetInventorySkusUsageReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetInventorySkusUsageReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE SPGetInventorySkusUsageReport
	@from datetime2,
	@to datetime2, 
	@skus nvarchar(MAX),
	@invskus nvarchar(MAX)
AS
BEGIN		
	SELECT
	CAST(ROW_NUMBER() OVER (ORDER BY s.Code DESC) AS INT) As Id,
	invStatPart.IdSku,
	invStatPart.IdInventorySku,
	skuStatPart.TotalSkuQuantity,
	invStatPart.TotalInvSkuQuantity,
	s.Code AS SkuCode,
	invs.Code As InvSkuCode,
	invs.Description As InvDescription,
	invs.IdInventorySkuCategory,
	CAST(invOptions.ProductSource AS INT) AS ProductSource,
	CAST(invOptions.Quantity As INT) AS InvQuantity,
	CAST(invOptions.UnitOfMeasure AS INT) AS UnitOfMeasure,
	CAST(invOptions.UnitOfMeasureAmount As MONEY) AS UnitOfMeasureAmount,
	CAST(invOptions.PurchaseUnitOfMeasure AS INT) AS PurchaseUnitOfMeasure,
	CAST(invOptions.PurchaseUnitOfMeasureAmount As INT) AS PurchaseUnitOfMeasureAmount,
	CAST(skuOptions.BornDate As DATETIME) AS BornDate,
	CAST(skuOptions.InventorySkuChannel As INT) AS InventorySkuChannel,
	CAST((CASE WHEN skuOptions.Assemble='True' THEN 1 ELSE 0 END) As BIT) AS Assemble
	FROM
		(SELECT tempInvStatPart.IdSku,tempInvStatPart.IdInventorySku, SUM(tempInvStatPart.SkuQuantity*tempInvStatPart.InvSkuQuantity) AS TotalInvSkuQuantity FROM
			(SELECT os.Id,os.IdSku, osToinvs.IdInventorySku, os.Quantity AS SkuQuantity, osToinvs.Quantity AS InvSkuQuantity FROM
				(SELECT o.Id,os.IdSku,os.Quantity FROM
					(SELECT Id FROM Orders WITH(NOLOCK)
					WHERE DateCreated>=@from AND DateCreated<=@to AND StatusCode!=3 AND 
					(OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3)) o
				JOIN OrderToSkus os WITH(NOLOCK) ON o.Id=os.IdOrder
				LEFT JOIN TFGetTableIdsByString(@skus, DEFAULT) idskus ON os.IdSku=idskus.Id
				WHERE @skus IS NULL OR idskus.Id IS NOT NULL) os
			LEFT JOIN OrderToSkusToInventorySkus osToinvs WITH(NOLOCK) ON os.Id=osToinvs.IdOrder AND os.IdSku=osToinvs.IdSku
			LEFT JOIN TFGetTableIdsByString(@invskus, DEFAULT) idinvskus ON osToinvs.IdInventorySku=idinvskus.Id
			WHERE @invskus IS NULL OR idinvskus.Id IS NOT NULL

			UNION ALL
			SELECT op.Id,op.IdSku, opToinvs.IdInventorySku, op.Quantity AS SkuQuantity, opToinvs.Quantity AS InvSkuQuantity FROM
				(SELECT o.Id,op.IdSku,op.Quantity FROM
					(SELECT Id FROM Orders WITH(NOLOCK)
					WHERE DateCreated>=@from AND DateCreated<=@to AND StatusCode!=3 AND 
					(OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3)) o
				JOIN OrderToPromos op WITH(NOLOCK) ON o.Id=op.IdOrder AND op.[Disabled]=0
				LEFT JOIN TFGetTableIdsByString(@skus, DEFAULT) idskus ON op.IdSku=idskus.Id
				WHERE @skus IS NULL OR idskus.Id IS NOT NULL) op
			LEFT JOIN OrderToPromosToInventorySkus opToinvs WITH(NOLOCK) ON op.Id=opToinvs.IdOrder AND op.IdSku=opToinvs.IdSku
			LEFT JOIN TFGetTableIdsByString(@invskus, DEFAULT) idinvskus ON opToinvs.IdInventorySku=idinvskus.Id
			WHERE @invskus IS NULL OR idinvskus.Id IS NOT NULL) tempInvStatPart
		GROUP BY tempInvStatPart.IdSku,tempInvStatPart.IdInventorySku) invStatPart

		JOIN 
			(SELECT tempSkuStatPart.IdSku, SUM(tempSkuStatPart.Quantity) AS TotalSkuQuantity FROM 
				(SELECT o.Id,os.IdSku,os.Quantity FROM
					(SELECT Id FROM Orders
					WHERE DateCreated>=@from AND DateCreated<=@to AND StatusCode!=3 AND 
					(OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3)) o
				JOIN OrderToSkus os ON o.Id=os.IdOrder

				UNION ALL
				SELECT o.Id,op.IdSku,op.Quantity FROM
					(SELECT Id FROM Orders
					WHERE DateCreated>=@from AND DateCreated<=@to AND StatusCode!=3 AND 
					(OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3)) o
				JOIN OrderToPromos op ON o.Id=op.IdOrder AND op.[Disabled]=0) tempSkuStatPart
			GROUP BY tempSkuStatPart.IdSku) skuStatPart
		ON invStatPart.IdSku=skuStatPart.IdSku

		JOIN Skus s ON invStatPart.IdSku=s.Id
		LEFT JOIN 
			(SELECT IdSku, BornDate, InventorySkuChannel, Assemble
				FROM 
				(SELECT IdSku, [Name], [Value] FROM SkuOptionTypes AS adt
				INNER JOIN SkuOptionValues AS adv on adt.Id = adv.IdOptionType) As source
				PIVOT(
					MIN([Value]) FOR [Name] in (BornDate, InventorySkuChannel, Assemble)
				) AS piv
			) 			
		AS skuOptions ON invStatPart.IdSku = skuOptions.IdSku

		LEFT JOIN InventorySkus invs ON invStatPart.IdInventorySku=invs.Id
		LEFT JOIN 
			(SELECT IdInventorySku, ProductSource, Quantity, UnitOfMeasure, UnitOfMeasureAmount, PurchaseUnitOfMeasure, PurchaseUnitOfMeasureAmount
				FROM 
				(SELECT IdInventorySku, [Name], [Value] FROM InventorySkuOptionTypes AS adt
				INNER JOIN InventorySkuOptionValues AS adv on adt.Id = adv.IdOptionType) As source
				PIVOT(
					MIN([Value]) FOR [Name] in (ProductSource, Quantity, UnitOfMeasure, UnitOfMeasureAmount, PurchaseUnitOfMeasure, PurchaseUnitOfMeasureAmount)
				) AS piv
			) 			
		AS invOptions ON invStatPart.IdInventorySku = invOptions.IdInventorySku

		ORDER BY s.Code

END

GO