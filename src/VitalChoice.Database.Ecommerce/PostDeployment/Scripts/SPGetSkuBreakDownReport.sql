IF OBJECT_ID(N'[dbo].[SPGetSkuBreakDownReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetSkuBreakDownReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetSkuBreakDownReport]
	@from datetime2,
	@to datetime2
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @orders AS TABLE
    (
        IdOrder int NOT NULL,
		CustomerIdObjectType int NOT NULL,
		POrderStatus int NULL,
		NPOrderStatus int NULL
    );

	INSERT INTO @orders
	(
		IdOrder,
		CustomerIdObjectType,
		POrderStatus,
		NPOrderStatus
	)
	(SELECT 
		o.Id,
		c.IdObjectType,
		o.POrderStatus,
		o.NPOrderStatus
		FROM Orders o WITH(NOLOCK)
		JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
		WHERE o.DateCreated>=@from AND o.DateCreated<=@to AND o.StatusCode!=3 AND
			o.IdObjectType NOT IN (2,6) AND 
			((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) OR 
			(o.OrderStatus IS NULL AND (o.POrderStatus !=1 AND o.POrderStatus !=4) OR 
			(o.NPOrderStatus !=1 AND o.NPOrderStatus !=4)))
	)

	SELECT
		temp.IdSku, s.IdProduct, s.Code, temp.CustomerIdObjectType, temp.Quantity, temp.Amount
		FROM
		(SELECT 
			tempInner.IdSku, tempInner.CustomerIdObjectType, SUM(tempInner.Quantity) as Quantity, SUM(tempInner.Amount) as Amount
			FROM
			(SELECT 
				o.CustomerIdObjectType, os.IdSku, os.Quantity, (os.Quantity*os.Amount) as Amount
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
				o.CustomerIdObjectType, op.IdSku, op.Quantity, (op.Quantity*op.Amount) as Amount
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
		GROUP BY tempInner.IdSku, tempInner.CustomerIdObjectType) temp
	JOIN Skus s ON temp.IdSku=s.Id	

END

GO