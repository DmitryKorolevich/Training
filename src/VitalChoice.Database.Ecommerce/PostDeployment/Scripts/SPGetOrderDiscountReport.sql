IF OBJECT_ID(N'[dbo].[SPGetOrderDiscountReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetOrderDiscountReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetOrderDiscountReport]
	@from datetime2,
	@to datetime2,
	@discount nvarchar(250) = NULL,
	@pageindex int = NULL,
	@pagesize int =NULL
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @idDiscount INT, @count INT
	DECLARE @orders AS TABLE
    (
        Id INT NOT NULL,
		IdObjectType INT NOT NULL,
		OrderStatus INT NULL,
		POrderStatus INT NULL,
		NPOrderStatus INT NULL,
		DateCreated DATETIME2 NOT NULL,
		IdCustomer INT NOT NULL,
		Total MONEY NOT NULL,
		DiscountTotal MONEY NOT NULL,
		IdDiscount INT NULL
    );

	SET @idDiscount =
	(
		SELECT 
			Id
		FROM Discounts
		WHERE Code=@discount AND StatusCode!=3
	)

	INSERT INTO @orders
	(
	    Id,
		IdObjectType,
		OrderStatus,
		POrderStatus,
		NPOrderStatus,
		DateCreated,
		IdCustomer,
		Total,
		DiscountTotal,
		IdDiscount
	)
	(
		SELECT
			temp.Id,
			temp.IdObjectType,
			temp.OrderStatus,
			temp.POrderStatus,
			temp.NPOrderStatus,
			temp.DateCreated,
			temp.IdCustomer,
			temp.Total,
			temp.DiscountTotal,
			temp.IdDiscount
		FROM
		(
			SELECT 
				ROW_NUMBER() OVER (ORDER BY o.Id DESC) AS RowNumber,
				o.Id,
				o.IdObjectType,
				o.OrderStatus,
				o.POrderStatus,
				o.NPOrderStatus,
				o.DateCreated,
				o.IdCustomer,
				o.Total,
				o.DiscountTotal,
				o.IdDiscount
			FROM Orders o WITH(NOLOCK)
			WHERE
				o.DateCreated>=@from AND o.DateCreated<@to AND
				o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
				(
					o.OrderStatus IN(2,3,5,7)
					OR 
					(
						o.POrderStatus IN(2,3,5,7) 
						OR 
						o.NPOrderStatus IN(2,3,5,7)
					)
				) 
				AND
				(@discount IS NULL OR o.IdDiscount = @idDiscount)

		) temp
		WHERE @pageindex is NULL OR (RowNumber>(@pageindex-1)*@pagesize AND RowNumber<=@pageindex*@pagesize)	
	)

	IF(@pageindex IS NOT NULL)
	BEGIN
		SET @count = 
		(
			SELECT 
				COUNT(*) 
			FROM Orders o WITH(NOLOCK)
			WHERE
				o.DateCreated>=@from AND o.DateCreated<@to AND
				o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
				(
					o.OrderStatus IN(2,3,5,7)
					OR 
					(
						o.POrderStatus IN(2,3,5,7) 
						OR 
						o.NPOrderStatus IN(2,3,5,7)
					)
				) 
				AND
				(@discount IS NULL OR o.IdDiscount = @idDiscount)
		)
	END


	SELECT
		o.Id,
		o.IdObjectType,
		o.OrderStatus,
		o.POrderStatus,
		o.NPOrderStatus,
		o.DateCreated,
		o.IdCustomer,
		o.Total,
		o.DiscountTotal,		
		o.IdDiscount,
		c.IdObjectType CustomerIdObjectType,
		c.Email CustomerEmail,
		cadfval.Value as CustomerFirstName,
		cadlval.Value as CustomerLastName,
		CAST(otval.Value as INT) OrderIdDiscountTier,
		@count TotalCount
	FROM @orders o
	JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
	LEFT JOIN AddressOptionTypes AS cadfopt WITH(NOLOCK) ON cadfopt.Name = N'FirstName'
	LEFT JOIN AddressOptionValues AS cadfval WITH(NOLOCK) ON cadfval.IdAddress = c.IdProfileAddress AND cadfval.IdOptionType = cadfopt.Id
	LEFT JOIN AddressOptionTypes AS cadlopt WITH(NOLOCK) ON cadlopt.Name = N'LastName'
	LEFT JOIN AddressOptionValues AS cadlval WITH(NOLOCK) ON cadlval.IdAddress = c.IdProfileAddress AND cadlval.IdOptionType = cadlopt.Id
	LEFT JOIN OrderOptionTypes AS otopt WITH(NOLOCK) ON otopt.Name = N'IdDiscountTier'
	LEFT JOIN OrderOptionValues AS otval WITH(NOLOCK) ON otval.IdOrder = o.Id AND otval.IdOptionType = otopt.Id
	ORDER BY o.Id DESC
	OPTION(RECOMPILE)

END

GO