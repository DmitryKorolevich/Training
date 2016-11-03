DECLARE @ordersCount int, @current int
DECLARE @from DATETIME, @to DATETIME
SET @from='2016-09-01 00:00:00.000'
SET @to='2016-10-12 00:00:00.000'

DELETE OrderToSkusToInventorySkus
WHERE IdOrder IN
(
	SELECT
		o.Id
	FROM Orders o WITH (NOLOCK)
	WHERE
		o.StatusCode!=3 AND o.IdObjectType NOT IN (2,6) AND
		((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) OR 
		(o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND 
		o.NPOrderStatus !=1 AND o.NPOrderStatus !=4)) AND
		o.DateCreated>@from AND o.DateCreated<@to
)		

DELETE OrderToPromosToInventorySkus
WHERE IdOrder IN
(
	SELECT
		o.Id
	FROM Orders o WITH (NOLOCK)
	WHERE
		o.StatusCode!=3 AND o.IdObjectType NOT IN (2,6) AND 
		((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) OR 
		(o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND 
		o.NPOrderStatus !=1 AND o.NPOrderStatus !=4)) AND
		o.DateCreated>@from AND o.DateCreated<@to
)			

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
				Id
			FROM Orders o WITH (NOLOCK)
			WHERE 
				o.StatusCode!=3 AND o.IdObjectType NOT IN (2,6) AND 
				((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) OR 
				(o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND 
				o.NPOrderStatus !=1 AND o.NPOrderStatus !=4)) AND
				o.DateCreated>@from AND o.DateCreated<@to
		) tOrders
	JOIN OrderToSkus os WITH (NOLOCK) ON os.IdOrder=tOrders.Id
	JOIN SkusToInventorySkus sis WITH (NOLOCK) ON os.IdSku=sis.IdSku
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
				Id
			FROM Orders o WITH (NOLOCK)
			WHERE 
				o.StatusCode!=3 AND o.IdObjectType NOT IN (2,6) AND 
				((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) OR 
				(o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND 
				o.NPOrderStatus !=1 AND o.NPOrderStatus !=4)) AND
				o.DateCreated>@from AND o.DateCreated<@to
		) tOrders
	JOIN OrderToPromos op WITH (NOLOCK) ON op.IdOrder=tOrders.Id AND op.Disabled=0
	JOIN SkusToInventorySkus sis WITH (NOLOCK) ON op.IdSku=sis.IdSku
)