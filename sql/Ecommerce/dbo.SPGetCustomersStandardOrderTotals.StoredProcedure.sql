/****** Object:  StoredProcedure [dbo].[SPGetCustomersStandardOrderTotals]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SPGetCustomersStandardOrderTotals]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SPGetCustomersStandardOrderTotals] AS' 
END
GO
ALTER PROCEDURE [dbo].[SPGetCustomersStandardOrderTotals]
	@from datetime2,
	@to datetime2,
	@customerids nvarchar(MAX)
AS
BEGIN

	SET NOCOUNT ON

	SELECT 
		o.IdCustomer as Id, SUM(o.Total) as Total
	FROM Orders o  WITH(NOLOCK)
	LEFT JOIN TFGetTableIdsByString(@customerids, DEFAULT) idsc ON o.IdCustomer=idsc.Id
	WHERE o.DateCreated>=@from AND o.DateCreated<=@to AND idsc.Id IS NOT NULL AND
		o.StatusCode!=3 AND o.IdObjectType!=5 AND o.IdObjectType!=6 AND o.IdObjectType!=2 AND 
		((o.OrderStatus IS NOT NULL AND o.OrderStatus!=1 AND o.OrderStatus!=4) OR 
		(o.OrderStatus IS NULL AND o.POrderStatus!=1 AND o.POrderStatus!=4 AND
		o.NPOrderStatus!=1 AND o.NPOrderStatus!=4))
	GROUP BY o.IdCustomer
	
END


GO
