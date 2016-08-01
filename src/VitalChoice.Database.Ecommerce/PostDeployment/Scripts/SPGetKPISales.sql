IF OBJECT_ID(N'[dbo].[SPGetKPISales]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetKPISales]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetKPISales]
	@from datetime2,
	@to datetime2
AS
BEGIN

	SET NOCOUNT ON

	SELECT 
		(
			SELECT 
				SUM(Total)
			FROM Orders o
			WHERE
				o.DateCreated BETWEEN @from AND @to AND
				o.IdObjectType NOT IN(2,5,6) AND 
				(
					o.OrderStatus IN(2,3,5,6,7)
					OR 
					(
						o.POrderStatus IN(2,3,5,6,7) 
						OR 
						o.NPOrderStatus IN(2,3,5,6,7)
					)
				)
		) Total,
		(
			SELECT 
				SUM(Total)
			FROM Orders o
			INNER JOIN Customers c ON c.Id = o.IdCustomer
			WHERE
				o.DateCreated BETWEEN @from AND @to AND
				o.IdObjectType NOT IN(2,5,6) AND 
				(
					o.OrderStatus IN(2,3,5,6,7)
					OR 
					(
						o.POrderStatus IN(2,3,5,6,7) 
						OR 
						o.NPOrderStatus IN(2,3,5,6,7)
					)
				) AND
				c.IdObjectType=1 AND--retail
				ISNULL
				(
					c.DateCreated,
					(
						SELECT
							MIN(ord.DateCreated)
						FROM Orders ord
						WHERE
							ord.IdCustomer = c.Id
					)
				) BETWEEN @from AND @to
		) NewCustomers,
		(
			SELECT 
				SUM(Total)
			FROM Orders o
			INNER JOIN Customers c ON c.Id = o.IdCustomer
			WHERE
				o.DateCreated BETWEEN @from AND @to AND
				o.IdObjectType NOT IN(2,5,6) AND 
				(
					o.OrderStatus IN(2,3,5,6,7)
					OR 
					(
						o.POrderStatus IN(2,3,5,6,7) 
						OR 
						o.NPOrderStatus IN(2,3,5,6,7)
					)
				) AND
				c.IdObjectType=2--wholesale
		) Wholesales,
		(
			SELECT 
				SUM(Total)
			FROM Orders o
			INNER JOIN Customers c ON c.Id = o.IdCustomer
			WHERE
				o.DateCreated BETWEEN @from AND @to AND
				o.IdObjectType NOT IN(2,5,6) AND 
				(
					o.OrderStatus IN(2,3,5,6,7)
					OR 
					(
						o.POrderStatus IN(2,3,5,6,7) 
						OR 
						o.NPOrderStatus IN(2,3,5,6,7)
					)
				) AND
				c.IdObjectType=2 AND--wholesale
				ISNULL
				(
					c.DateCreated,
					(
						SELECT
							MIN(ord.DateCreated)
						FROM Orders ord
						WHERE
							ord.IdCustomer = c.Id
					)
				) BETWEEN @from AND @to
		) NewWholesales,
		(
			SELECT 
				SUM(Total)
			FROM Orders o
			INNER JOIN Customers c ON c.Id = o.IdCustomer
			WHERE
				o.DateCreated BETWEEN @from AND @to AND
				o.IdObjectType NOT IN(2,5,6) AND 
				(
					o.OrderStatus IN(2,3,5,6,7)
					OR 
					(
						o.POrderStatus IN(2,3,5,6,7) 
						OR 
						o.NPOrderStatus IN(2,3,5,6,7)
					)
				) AND
				c.IdObjectType=1 AND--retail
				c.IdAffiliate IS NOT NULL
		) Affiliates,
		(
			SELECT 
				SUM(Total)
			FROM Orders o
			INNER JOIN Customers c ON c.Id = o.IdCustomer
			WHERE
				o.DateCreated BETWEEN @from AND @to AND
				o.IdObjectType NOT IN(2,5,6) AND 
				(
					o.OrderStatus IN(2,3,5,6,7)
					OR 
					(
						o.POrderStatus IN(2,3,5,6,7) 
						OR 
						o.NPOrderStatus IN(2,3,5,6,7)
					)
				) AND
				c.IdObjectType=1 AND--retail
				ISNULL
				(
					c.DateCreated,
					(
						SELECT
							MIN(ord.DateCreated)
						FROM Orders ord
						WHERE
							ord.IdCustomer = c.Id
					)
				) BETWEEN @from AND @to AND 
				c.IdAffiliate IS NOT NULL
		) NewAffiliates
END

GO
