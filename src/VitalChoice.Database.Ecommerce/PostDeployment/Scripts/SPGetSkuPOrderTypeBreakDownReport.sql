IF OBJECT_ID(N'[dbo].[SPGetSkuPOrderTypeBreakDownReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetSkuPOrderTypeBreakDownReport]

GO

SET ANSI_NULLS ON

GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetSkuPOrderTypeBreakDownReport]
	@from datetime2,
	@to datetime2,
	@code nvarchar(250)
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @orders AS TABLE
    (
        IdOrder int NOT NULL,
		DateCreated DATETIME2, 
		POrderType int NULL,
		POrderStatus int NULL,
		NPOrderStatus int NULL
    );

	DECLARE @skus AS TABLE
	(
		IdSku int NOT NULL
	);

	IF(@code IS NOT NULL)
	BEGIN
		INSERT INTO @skus
		(
			IdSku
		)
		(
			SELECT 
				s.Id
			FROM Skus s
			WHERE 
				s.StatusCode!=3 AND s.Code LIKE @code+'%'
		)
	END

	INSERT INTO @orders
	(
		IdOrder,
		DateCreated, 
		POrderType,
		POrderStatus,
		NPOrderStatus
	)
	(SELECT 
		o.Id, 
		o.DateCreated,
		CAST(pval.Value as INT),
		o.POrderStatus,
		o.NPOrderStatus
		FROM Orders o WITH(NOLOCK)
		LEFT JOIN OrderOptionTypes AS popt WITH(NOLOCK) ON popt.Name = N'POrderType'
		LEFT JOIN OrderOptionValues AS pval WITH(NOLOCK) ON pval.IdOrder = o.Id AND pval.IdOptionType = popt.Id	
		WHERE o.DateCreated>=@from AND o.DateCreated<=@to AND o.StatusCode!=3 AND
			o.IdObjectType NOT IN (2,5,6) AND 
			((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) OR 
			(o.OrderStatus IS NULL AND ((o.POrderStatus !=1 AND o.POrderStatus !=4) OR 
			(o.NPOrderStatus !=1 AND o.NPOrderStatus !=4)))) AND
			(
				@code IS NULL OR
				EXISTS
				(
					SELECT
						TOP 1 os.IdOrder
					FROM OrderToSkus os
					WHERE os.IdOrder=o.Id AND os.IdSku IN (SELECT IdSku FROM @skus)
				) OR
				EXISTS
				(
					SELECT
						TOP 1 op.IdOrder
					FROM OrderToPromos op
					WHERE op.IdOrder=o.Id AND op.Disabled=0 AND op.IdSku IN (SELECT IdSku FROM @skus)
				)
			)		
	)

	SELECT 
		ROW_NUMBER() OVER (ORDER BY temp.IdOrder) as RowNumber,
		temp.IdOrder, temp.IdSku, s.Code, s.IdProduct, temp.Quantity, temp.DateCreated, CAST(temp.POrderType as INT) as POrderType
		FROM
		(SELECT
			 tempInner.IdOrder, tempInner.IdSku, SUM(tempInner.Quantity) as Quantity, 
			MIN(tempInner.DateCreated) as DateCreated, MIN(tempInner.POrderType) as POrderType
			FROM
			(SELECT 
					o.IdOrder, o.DateCreated, o.POrderType, os.IdSku, os.Quantity
					FROM @orders o
					JOIN OrderToSkus os WITH(NOLOCK) ON o.IdOrder=os.IdOrder
					JOIN Skus s WITH(NOLOCK) ON os.IdSku=s.Id
					JOIN Products p WITH(NOLOCK) ON s.IdProduct=p.Id
					WHERE 
						(o.POrderStatus IS NULL AND o.NPOrderStatus IS NULL) OR
						(
							(o.POrderStatus!=4 OR (o.POrderStatus=4 AND (p.IdObjectType=1 OR p.IdObjectType=3 OR p.IdObjectType=4))) AND
							(o.NPOrderStatus!=4 OR (o.NPOrderStatus=4 AND (p.IdObjectType=2)))
						)
				UNION ALL
				SELECT 
					o.IdOrder, o.DateCreated, o.POrderType, op.IdSku, op.Quantity
					FROM @orders o
					JOIN OrderToPromos op WITH(NOLOCK) ON o.IdOrder=op.IdOrder AND op.Disabled=0
					JOIN Skus s WITH(NOLOCK) ON op.IdSku=s.Id
					JOIN Products p WITH(NOLOCK) ON s.IdProduct=p.Id
					WHERE 
						(o.POrderStatus IS NULL AND o.NPOrderStatus IS NULL) OR
						(
							(o.POrderStatus!=4 OR (o.POrderStatus=4 AND (p.IdObjectType=1 OR p.IdObjectType=3 OR p.IdObjectType=4))) AND
							(o.NPOrderStatus!=4 OR (o.NPOrderStatus=4 AND (p.IdObjectType=2)))
						)
			) tempInner
			GROUP BY tempInner.IdOrder,tempInner.IdSku)	temp	
		JOIN Skus s ON temp.IdSku=s.Id	

END

GO