IF OBJECT_ID(N'[dbo].[VProductSkus]', N'V') IS NOT NULL
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
	s.StatusCode AS SkuStatusCode, 
	s.[Hidden] AS SkuHidden, 
	p.Id AS IdProduct,
	p.DateCreated,
	p.DateEdited,
	p.IdEditedBy,
	p.IdVisibility,
	p.IdObjectType AS IdProductType,
	p.Name,
	ISNULL(val.Value, opt.DefaultValue) AS Thumbnail,	
	ISNULL(tval.Value, topt.DefaultValue) AS TaxCode,
	ISNULL(sval.Value, sopt.DefaultValue) AS SubTitle,
	ISNULL(sdval.Value, sopt.DefaultValue) AS ShortDescription,
	CAST(disval.Value as BIT) AS DisregardStock,
	CAST(insval.Value as INT) AS Stock,
	s.[Order] SkuOrder
	FROM Products AS p
	LEFT JOIN Skus AS s ON p.Id = s.IdProduct
	LEFT JOIN ProductOptionTypes AS opt ON opt.Name = N'Thumbnail' AND opt.IdObjectType = p.IdObjectType
	LEFT JOIN ProductOptionValues AS val ON val.IdProduct = p.Id AND val.IdOptionType = opt.Id
	LEFT JOIN ProductOptionTypes AS topt ON topt.Name = N'TaxCode' AND topt.IdObjectType = p.IdObjectType
	LEFT JOIN ProductOptionValues AS tval ON tval.IdProduct = p.Id AND tval.IdOptionType = topt.Id
	LEFT JOIN ProductOptionTypes AS sopt ON sopt.Name = N'SubTitle' AND sopt.IdObjectType = p.IdObjectType
	LEFT JOIN ProductOptionValues AS sval ON sval.IdProduct = p.Id AND sval.IdOptionType = sopt.Id
	LEFT JOIN ProductOptionTypes AS sdopt ON sdopt.Name = N'ShortDescription' AND sdopt.IdObjectType = p.IdObjectType
	LEFT JOIN ProductOptionValues AS sdval ON sdval.IdProduct = p.Id AND sdval.IdOptionType = sdopt.Id
	LEFT JOIN SkuOptionTypes AS disopt ON disopt.Name = N'DisregardStock' AND disopt.IdObjectType = p.IdObjectType
	LEFT JOIN SkuOptionValues AS disval ON disval.IdSku = s.Id AND disval.IdOptionType = disopt.Id
	LEFT JOIN SkuOptionTypes AS insopt ON insopt.Name = N'Stock' AND insopt.IdObjectType = p.IdObjectType
	LEFT JOIN SkuOptionValues AS insval ON insval.IdSku = s.Id AND insval.IdOptionType = insopt.Id

GO