/****** Object:  View [dbo].[VProductsWithReviews]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VProductsWithReviews]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[VProductsWithReviews]
AS 
SELECT 
	pr.IdProduct AS IdProduct,
	pr.StatusCode,
	MIN(p.Name) AS ProductName,
	COUNT(*) AS Count,
	MAX(pr.DateCreated) AS DateCreated,
	CONVERT(DECIMAL(5,2), AVG(CONVERT(DECIMAL,pr.Rating))) AS Rating
	FROM ProductReviews AS pr
	JOIN Products AS p ON pr.IdProduct = p.Id
	WHERE pr.StatusCode IN (1,2)
	GROUP BY pr.StatusCode, pr.IdProduct

' 
GO
