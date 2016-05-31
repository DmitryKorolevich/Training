IF OBJECT_ID(N'[dbo].[SPGetSkuPOrderTypeBreakDownReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetSkuPOrderTypeBreakDownReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetSkuPOrderTypeBreakDownReport]
	@from datetime2,
	@to datetime2
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @orders AS TABLE
    (
        IdOrder int NOT NULL, DateCreated DATETIME2, POrderType int NULL
    );


	INSERT INTO @orders
	(
		IdOrder, DateCreated, POrderType
	)
	(SELECT 
		o.Id, o.DateCreated, CAST(pval.Value as INT)
		FROM Orders o WITH(NOLOCK)
		LEFT JOIN OrderOptionTypes AS popt WITH(NOLOCK) ON popt.Name = N'POrderType'
		LEFT JOIN OrderOptionValues AS pval WITH(NOLOCK) ON pval.IdOrder = o.Id AND pval.IdOptionType = popt.Id	
		WHERE o.DateCreated>=@from AND o.DateCreated<=@to AND o.StatusCode!=3 AND
			o.IdObjectType NOT IN (2,5,6) AND 
			((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) OR 
			(o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND 
			o.NPOrderStatus !=1 AND o.NPOrderStatus !=4))
		
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
				UNION ALL
				SELECT 
					o.IdOrder, o.DateCreated, o.POrderType, op.IdSku, op.Quantity
					FROM @orders o
					JOIN OrderToPromos op WITH(NOLOCK) ON o.IdOrder=op.IdOrder AND op.Disabled=0) tempInner
			GROUP BY tempInner.IdOrder,tempInner.IdSku)	temp	
		JOIN Skus s ON temp.IdSku=s.Id	

END

GO