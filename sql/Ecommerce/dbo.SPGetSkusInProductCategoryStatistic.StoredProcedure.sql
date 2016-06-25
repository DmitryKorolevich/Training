/****** Object:  StoredProcedure [dbo].[SPGetSkusInProductCategoryStatistic]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SPGetSkusInProductCategoryStatistic]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SPGetSkusInProductCategoryStatistic] AS' 
END
GO
ALTER PROCEDURE [dbo].[SPGetSkusInProductCategoryStatistic]
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
	WHERE o.StatusCode!=3 AND o.OrderStatus IN (2,3,5) AND o.IdObjectType NOT IN (5,6)  AND 
	o.DateCreated>=@from AND o.DateCreated<@to
	GROUP BY s.Code

END


GO
