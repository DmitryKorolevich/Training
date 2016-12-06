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

	SET @idDiscount =
	(
		SELECT 
			Id
		FROM Discounts
		WHERE Code=@discount AND StatusCode!=3
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
					o.OrderStatus IS NOT NULL AND o.OrderStatus NOT IN(1, 4)
					OR 
					(
						o.POrderStatus IS NOT NULL AND o.POrderStatus NOT IN(1, 4) 
						OR 
						o.NPOrderStatus IS NOT NULL AND o.NPOrderStatus NOT IN(1, 4)
					)
				) 
				AND
				(@discount IS NULL OR o.IdDiscount = @idDiscount)
		)

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
		FROM Orders o WITH(NOLOCK)
			INNER JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
			LEFT JOIN AddressOptionTypes AS cadfopt WITH(NOLOCK) ON cadfopt.Name = N'FirstName'
			LEFT JOIN AddressOptionValues AS cadfval WITH(NOLOCK) ON cadfval.IdAddress = c.IdProfileAddress AND cadfval.IdOptionType = cadfopt.Id
			LEFT JOIN AddressOptionTypes AS cadlopt WITH(NOLOCK) ON cadlopt.Name = N'LastName'
			LEFT JOIN AddressOptionValues AS cadlval WITH(NOLOCK) ON cadlval.IdAddress = c.IdProfileAddress AND cadlval.IdOptionType = cadlopt.Id
			LEFT JOIN OrderOptionTypes AS otopt WITH(NOLOCK) ON otopt.Name = N'IdDiscountTier'
			LEFT JOIN OrderOptionValues AS otval WITH(NOLOCK) ON otval.IdOrder = o.Id AND otval.IdOptionType = otopt.Id
		WHERE
			o.DateCreated>=@from AND o.DateCreated<@to AND
			o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
			(
				o.OrderStatus IS NOT NULL AND o.OrderStatus NOT IN(1, 4)
				OR 
				(
					o.POrderStatus IS NOT NULL AND o.POrderStatus NOT IN(1, 4) 
					OR 
					o.NPOrderStatus IS NOT NULL AND o.NPOrderStatus NOT IN(1, 4)
				)
			) 
			AND
			(@discount IS NULL OR o.IdDiscount = @idDiscount)
		ORDER BY o.Id DESC
		OFFSET (@pageindex-1)*@pagesize ROWS FETCH NEXT @pagesize ROWS ONLY
		OPTION(RECOMPILE)
	END
	ELSE
	BEGIN
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
		FROM Orders o WITH(NOLOCK)
			INNER JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
			LEFT JOIN AddressOptionTypes AS cadfopt WITH(NOLOCK) ON cadfopt.Name = N'FirstName'
			LEFT JOIN AddressOptionValues AS cadfval WITH(NOLOCK) ON cadfval.IdAddress = c.IdProfileAddress AND cadfval.IdOptionType = cadfopt.Id
			LEFT JOIN AddressOptionTypes AS cadlopt WITH(NOLOCK) ON cadlopt.Name = N'LastName'
			LEFT JOIN AddressOptionValues AS cadlval WITH(NOLOCK) ON cadlval.IdAddress = c.IdProfileAddress AND cadlval.IdOptionType = cadlopt.Id
			LEFT JOIN OrderOptionTypes AS otopt WITH(NOLOCK) ON otopt.Name = N'IdDiscountTier'
			LEFT JOIN OrderOptionValues AS otval WITH(NOLOCK) ON otval.IdOrder = o.Id AND otval.IdOptionType = otopt.Id
		WHERE
			o.DateCreated>=@from AND o.DateCreated<@to AND
			o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
			(
				o.OrderStatus IS NOT NULL AND o.OrderStatus NOT IN(1, 4)
				OR 
				(
					o.POrderStatus IS NOT NULL AND o.POrderStatus NOT IN(1, 4) 
					OR 
					o.NPOrderStatus IS NOT NULL AND o.NPOrderStatus NOT IN(1, 4)
				)
			) 
			AND
			(@discount IS NULL OR o.IdDiscount = @idDiscount)
		ORDER BY o.Id DESC
		OPTION(RECOMPILE)
	END
END

GO