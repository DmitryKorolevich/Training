IF OBJECT_ID(N'[dbo].[VSkus]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VSkus]
GO
CREATE VIEW [dbo].[VSkus]
AS 
SELECT 
	s.Id AS SkuId,
	s.Code, 
	s.Price, 
	s.WholesalePrice, 
	p.StatusCode,  
	s.StatusCode AS SkuStatusCode, 
	p.Id AS IdProduct,
	p.DateCreated,
	p.DateEdited,
	p.Hidden,
	p.IdProductType,
	p.Name,
	p.Url
	FROM Skus AS s
	JOIN Products AS p ON p.Id = s.IdProduct

GO


