IF OBJECT_ID(N'[dbo].[VProductsWithReviews]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VProductsWithReviews]
GO
CREATE VIEW [dbo].[VProductsWithReviews]
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

GO


