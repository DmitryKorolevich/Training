﻿IF OBJECT_ID(N'[dbo].[VProductSkus]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VProductSkus]
GO
CREATE VIEW [dbo].[VProductSkus]
AS 
SELECT 
	s.Id AS SkuId,
	s.Code, 
	s.Price, 
	s.WholesalePrice, 
	p.StatusCode, 
	p.Id AS IdProduct,
	p.DateCreated,
	p.DateEdited,
	p.Hidden,
	p.IdProductType,
	p.Name,
	p.Url,
	ISNULL(val.Value, opt.DefaultValue) AS Thumbnail
	FROM Products AS p
	LEFT JOIN Skus AS s ON p.Id = s.IdProduct
	LEFT JOIN ProductOptionTypes AS opt ON opt.Name = N'Thumbnail' AND opt.IdProductType = p.IdProductType
	LEFT JOIN ProductOptionValues AS val ON val.IdProduct = p.Id AND val.IdOptionType = opt.Id