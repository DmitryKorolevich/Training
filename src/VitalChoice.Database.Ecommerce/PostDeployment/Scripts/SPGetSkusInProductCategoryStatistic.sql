IF OBJECT_ID(N'[dbo].[SPGetSkusInProductCategoryStatistic]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetSkusInProductCategoryStatistic]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE SPGetSkusInProductCategoryStatistic
	@from datetime2, @to datetime2, @idcategory int
AS
BEGIN

	SELECT s.Code, MIN(p.Name) As ProductName, MIN(pc.Name) As Category, MIN(rpc.Name) As ParentCategory,
	MIN(s.Price) Price, MIN(s.WholesalePrice) WholesalePrice, SUM(os.Quantity) Quantity, SUM(os.Quantity*os.Amount) Amount
	FROM OrderToSkus os WITH(NOLOCK)
	INNER JOIN Orders o WITH(NOLOCK) on os.IdOrder=o.Id
	INNER JOIN Skus s WITH(NOLOCK) ON os.IdSku=s.Id
	INNER JOIN Products p WITH(NOLOCK) ON p.Id=s.IdProduct
	INNER JOIN ProductsToCategories ptc WITH(NOLOCK) ON s.IdProduct=ptc.IdProduct aND ptc.IdCategory=@idcategory
	INNER JOIN ProductCategories pc WITH(NOLOCK) ON ptc.IdCategory=pc.Id
	LEFT JOIN ProductCategories rpc WITH(NOLOCK) ON pc.ParentId=rpc.Id
	WHERE 
		o.StatusCode!=3 AND 
		(
			(o.OrderStatus IS NOT NULL AND o.OrderStatus IN (2,3,5)) OR
			(o.OrderStatus IS NULL AND (o.POrderStatus IN (2,3,5) OR o.NPOrderStatus IN (2,3,5)))
		) AND
		o.IdObjectType NOT IN (2, 5,6)  AND 
		o.DateCreated>=@from AND o.DateCreated<@to
	GROUP BY s.Code

END

GO