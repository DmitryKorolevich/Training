/****** Object:  StoredProcedure [dbo].[SPGetProductCategoryStatistic]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SPGetProductCategoryStatistic]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SPGetProductCategoryStatistic] AS' 
END
GO
ALTER PROCEDURE [dbo].[SPGetProductCategoryStatistic]
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
