IF OBJECT_ID(N'[dbo].[VProductSkus]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VProductSkus]
GO
CREATE VIEW [dbo].[VProductSkus]
AS 
SELECT 
	s.Id,
	s.Code, 
	s.Price, 
	s.WholesalePrice, 
	p.StatusCode, 
	s.IdProduct,
	p.DateCreated,
	p.DateEdited,
	p.Hidden,
	p.IdProductType,
	p.Name,
	p.Url,
	ISNULL(val.Value, opt.DefaultValue) AS Thumbnail
	FROM Skus AS s
	INNER JOIN Products AS p ON p.Id = s.IdProduct
	INNER JOIN ProductOptionTypes AS opt ON opt.Name = N'Thumbnail' AND opt.IdProductType = p.IdProductType
	LEFT JOIN ProductOptionValues AS val ON val.IdProduct = p.Id AND val.IdOptionType = opt.Id