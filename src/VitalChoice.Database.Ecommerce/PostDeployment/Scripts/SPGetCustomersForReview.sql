GO

IF OBJECT_ID(N'[dbo].[SPGetCustomersForReview]', N'P') IS NOT NULL
BEGIN
	DROP PROCEDURE [dbo].[SPGetCustomersForReview]
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetCustomersForReview]
	@from datetime2=NULL,
	@to datetime2=NULL,
	@customerids nvarchar(MAX)
AS
BEGIN

	SET NOCOUNT ON

	SELECT 
		temp.Id
	FROM
	(
		SELECT 
			c.Id,
			(
				SELECT 
					COUNT(*) 
				FROM Orders o WITH(NOLOCK)
				WHERE o.IdCustomer=c.Id AND
					o.DateCreated>=@from AND o.DateCreated<@to AND o.StatusCode!=3 AND o.IdObjectType=5 AND 
					((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) OR 
					(o.OrderStatus IS NULL AND (o.POrderStatus !=1 AND o.POrderStatus !=4) OR 
					(o.NPOrderStatus !=1 AND o.NPOrderStatus !=4)))
			) Reships,
			(
				SELECT 
					SUM(o.Total) 
				FROM Orders o WITH(NOLOCK)
				WHERE o.IdCustomer=c.Id AND
					o.DateCreated>=@from AND o.DateCreated<@to AND o.StatusCode!=3 AND o.IdObjectType=6 AND 
					((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) OR 
					(o.OrderStatus IS NULL AND (o.POrderStatus !=1 AND o.POrderStatus !=4) OR 
					(o.NPOrderStatus !=1 AND o.NPOrderStatus !=4)))
			) RefundsAmount
		FROM Customers c WITH(NOLOCK)
		LEFT JOIN TFGetTableIdsByString(@customerids, DEFAULT) ids ON c.Id=ids.Id
		WHERE ids.Id IS NOT NULL
	) temp
	JOIN CustomerOptionTypes AS copt WITH(NOLOCK) ON copt.Name = N'IgnoreAbusePolicy'
	LEFT JOIN CustomerOptionValues AS cval WITH(NOLOCK) ON cval.IdCustomer = temp.Id AND cval.IdOptionType = copt.Id			 
	WHERE (temp.Reships>=2 OR temp.RefundsAmount>=200) AND (cval.Value IS NULL OR cval.Value='False')

END

GO