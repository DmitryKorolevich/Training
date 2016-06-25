/****** Object:  StoredProcedure [dbo].[SPGetSkuBreakDownReport]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SPGetSkuBreakDownReport]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SPGetSkuBreakDownReport] AS' 
END
GO
ALTER PROCEDURE [dbo].[SPGetSkuBreakDownReport]
	@from datetime2,
	@to datetime2
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @orders AS TABLE
    (
        IdOrder int NOT NULL, CustomerIdObjectType int NOT NULL
    );

	INSERT INTO @orders
	(
		IdOrder, CustomerIdObjectType
	)
	(SELECT 
		o.Id, c.IdObjectType
		FROM Orders o WITH(NOLOCK)
		JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
		WHERE o.DateCreated>=@from AND o.DateCreated<=@to AND o.StatusCode!=3 AND
			o.IdObjectType NOT IN (2,5,6) AND 
			((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) OR 
			(o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND 
			o.NPOrderStatus !=1 AND o.NPOrderStatus !=4))
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
			UNION ALL
			SELECT 
				o.CustomerIdObjectType, op.IdSku, op.Quantity, (op.Quantity*op.Amount) as Amount
				FROM @orders o
				JOIN OrderToPromos op WITH(NOLOCK) ON o.IdOrder=op.IdOrder AND op.Disabled=0) tempInner
		GROUP BY tempInner.IdSku, tempInner.CustomerIdObjectType) temp
	JOIN Skus s ON temp.IdSku=s.Id	

END


GO
