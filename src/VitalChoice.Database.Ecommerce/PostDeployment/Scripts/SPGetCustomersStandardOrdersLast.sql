IF OBJECT_ID(N'[dbo].[SPGetCustomersStandardOrdersLast]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetCustomersStandardOrdersLast]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetCustomersStandardOrdersLast]
	@customerids nvarchar(MAX)
AS
BEGIN

	SET NOCOUNT ON

	SELECT 
		o.IdCustomer as Id, MAX(o.DateCreated) as LastOrderDate
	FROM Orders o  WITH(NOLOCK)
	LEFT JOIN TFGetTableIdsByString(@customerids, DEFAULT) idsc ON o.IdCustomer=idsc.Id
	WHERE idsc.Id IS NOT NULL AND
		o.StatusCode!=3 AND o.IdObjectType!=5 AND o.IdObjectType!=6 AND o.IdObjectType!=2 AND 
		((o.OrderStatus IS NOT NULL AND o.OrderStatus!=1 AND o.OrderStatus!=4) OR 
		(o.OrderStatus IS NULL AND o.POrderStatus!=1 AND o.POrderStatus!=4 AND
		o.NPOrderStatus!=1 AND o.NPOrderStatus!=4))
	GROUP BY o.IdCustomer
	
END

GO
