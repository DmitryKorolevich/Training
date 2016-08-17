USE [VitalChoice.Ecommerce]
GO
/****** Object:  StoredProcedure [dbo].[SPGetShortOrders]    Script Date: 8/12/2016 17:34:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SPGetShortOrders]
	@idcustomer int,
	@idfilter nvarchar(250),
	@idobjecttype int,
	@pageindex int,
	@pagesize int,
	@onlytop bit=1
AS
BEGIN

	SET NOCOUNT ON;

	IF(@onlytop=1)
	BEGIN

		SELECT
			TOP(@pagesize)
			o.Id,
			o.IdObjectType,
			o.IdCustomer, 
			o.OrderStatus,
			o.POrderStatus,
			o.NPOrderStatus,
			o.DateCreated,
			o.DateEdited,
			o.ProductsSubtotal,
			o.Total,
			0 TotalCount
		FROM Orders o WITH(NOLOCK)
		WHERE
			(o.Id LIKE @idfilter) AND
			o.StatusCode!=3 AND o.IdObjectType!=2 AND 
			(@idobjecttype IS NULL OR o.IdObjectType = @idobjecttype) AND
			(@idcustomer IS NULL OR o.IdCustomer = @idcustomer)
		ORDER BY Id ASC
		OPTION(RECOMPILE)

	END
	ELSE
	BEGIN

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
			(o.Id LIKE @idfilter) AND
			o.StatusCode!=3 AND o.IdObjectType!=2 AND 
			(@idobjecttype IS NULL OR o.IdObjectType = @idobjecttype) AND
			(@idcustomer IS NULL OR o.IdCustomer = @idcustomer)
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
	
END

GO

