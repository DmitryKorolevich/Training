IF OBJECT_ID(N'[dbo].[SPGetShortOrders]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetShortOrders]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetShortOrders]
	@idcustomer int,
	@idfilter nvarchar(250),
	@idobjecttype int,
	@pageindex int,
	@pagesize int
AS
BEGIN

	SET NOCOUNT ON;

	WITH tempOrders
	(
		Id, 
		IdObjectType, 
		IdCustomer,
		OrderStatus,
		POrderStatus, 
		NPOrderStatus, 
		DateCreated,
		DateEdited,
		ProductsSubtotal, 
		Total
	)
	AS
	(
	SELECT
		o.Id,
		o.IdObjectType,
		o.IdCustomer, 
		o.OrderStatus,
		o.POrderStatus,
		o.NPOrderStatus,
		o.DateCreated,
		o.DateEdited,
		o.ProductsSubtotal,
		o.Total
	FROM Orders o WITH(NOLOCK)
	WHERE
		o.StatusCode!=3 AND o.IdObjectType!=2 AND 
		(@idobjecttype IS NULL OR o.IdObjectType = @idobjecttype) AND
		(@idcustomer IS NULL OR o.IdCustomer = @idcustomer) AND
		(o.Id LIKE @idfilter)
	)

	SELECT 
		* 
	FROM
		(SELECT *, ROW_NUMBER() OVER (ORDER BY Id ASC) AS RowNumber FROM tempOrders) temp
	CROSS JOIN 
		(SELECT Count(*) AS TotalCount FROM tempOrders)
	AS tCountOrders
	WHERE 
		@pageindex is NULL OR (RowNumber>(@pageindex-1)*@pagesize AND RowNumber<=@pageindex*@pagesize)
	ORDER BY Id ASC
	OPTION(RECOMPILE)
	
END

GO
