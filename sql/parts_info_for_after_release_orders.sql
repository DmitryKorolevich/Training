DECLARE @ordersCount int, @current int
DECLARE @from DATETIME, @to DATETIME
SET @from='2016-09-01 00:00:00.000'
SET @to='2016-10-10 00:00:00.000'

SET @current=0
SET @ordersCount = (
	SELECT COUNT(*) 
	FROM Orders o
	WHERE 
		o.StatusCode!=3 AND o.IdObjectType NOT IN (2,6) AND o.OrderStatus NOT IN(1,4) AND
		o.DateCreated>@from AND o.DateCreated<@to AND
		NOT EXISTS
		(
			SELECT s.IdOrder FROM OrderToSkusToInventorySkus s WHERE s.IdOrder= o.Id
		) AND
		NOT EXISTS
		(
			SELECT s.IdOrder FROM OrderToPromosToInventorySkus s WHERE s.IdOrder= o.Id
		)
)

WHILE (@current<@ordersCount)
BEGIN

	INSERT INTO OrderToSkusToInventorySkus
	(
		IdOrder,
		IdSku,
		IdInventorySku,
		Quantity
	)
	(
		SELECT
			tOrders.Id,
			sis.IdSku,
			sis.IdInventorySku,
			sis.Quantity
		FROM 
			(
				SELECT 
					temp.Id
					FROM
					(
						SELECT 
							Id,
							ROW_NUMBER() OVER(ORDER BY Id ASC) AS Row  
						FROM Orders o
						WHERE 
							o.StatusCode!=3 AND o.IdObjectType NOT IN (2,6) AND o.OrderStatus NOT IN(1,4) AND
							o.DateCreated>@from AND o.DateCreated<@to AND
							NOT EXISTS
							(
								SELECT s.IdOrder FROM OrderToSkusToInventorySkus s WHERE s.IdOrder= o.Id
							) AND
							NOT EXISTS
							(
								SELECT s.IdOrder FROM OrderToPromosToInventorySkus s WHERE s.IdOrder= o.Id
							)
					) temp
				WHERE temp.Row>@current AND temp.Row<=(@current+10000)
			) tOrders
		JOIN OrderToSkus os ON os.IdOrder=tOrders.Id
		JOIN SkusToInventorySkus sis ON os.IdSku=sis.IdSku
	)

	INSERT INTO OrderToPromosToInventorySkus
	(
		IdOrder,
		IdSku,
		IdInventorySku,
		Quantity
	)
	(
		SELECT
			tOrders.Id,
			sis.IdSku,
			sis.IdInventorySku,
			sis.Quantity
		FROM 
			(
				SELECT 
					temp.Id
					FROM
					(
						SELECT 
							Id,
							ROW_NUMBER() OVER(ORDER BY Id ASC) AS Row  
						FROM Orders o
						WHERE 
							o.StatusCode!=3 AND o.IdObjectType NOT IN (2,6) AND o.OrderStatus NOT IN(1,4) AND
							o.DateCreated>@from AND o.DateCreated<@to AND
							NOT EXISTS
							(
								SELECT s.IdOrder FROM OrderToSkusToInventorySkus s WHERE s.IdOrder= o.Id
							) AND
							NOT EXISTS
							(
								SELECT s.IdOrder FROM OrderToPromosToInventorySkus s WHERE s.IdOrder= o.Id
							)
					) temp
				WHERE temp.Row>@current AND temp.Row<=(@current+10000)
			) tOrders
		JOIN OrderToPromos op ON op.IdOrder=tOrders.Id AND op.Disabled=0
		JOIN SkusToInventorySkus sis ON op.IdSku=sis.IdSku
	)

	SET @current=@current+10000

END