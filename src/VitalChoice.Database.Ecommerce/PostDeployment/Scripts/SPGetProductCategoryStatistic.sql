IF OBJECT_ID(N'[dbo].[SPGetProductCategoryStatistic]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetProductCategoryStatistic]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE SPGetProductCategoryStatistic
	@from datetime2, @to datetime2
AS
BEGIN

	SELECT pc.Id, SUM(os.Amount*os.Quantity) As Amount 
	FROM OrderToSkus os WITH(NOLOCK)
	INNER JOIN Orders o WITH(NOLOCK) on os.IdOrder=o.Id
	INNER JOIN Skus s WITH(NOLOCK) ON os.IdSku=s.Id
	INNER JOIN ProductsToCategories ptc WITH(NOLOCK) ON s.IdProduct=ptc.IdProduct
	INNER JOIN ProductCategories pc WITH(NOLOCK) ON ptc.IdCategory=pc.Id
	WHERE o.StatusCode!=3 AND o.OrderStatus IN (2,3,5) AND o.IdObjectType NOT IN (5,6)  AND 
	o.DateCreated>=@from AND o.DateCreated<@to
	GROUP BY pc.Id

END

GO