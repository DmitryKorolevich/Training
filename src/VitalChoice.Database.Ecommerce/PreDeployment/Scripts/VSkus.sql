GO

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
	p.IdObjectType AS IdProductType,
	p.Name,
	p.Url,
	p.Name +ISNULL(' - '+pval.Value, '')+ISNULL(' ('+sval.Value+')', '') AS DescriptionName,
	CONVERT(bit,ISNULL(aval.Value, 0)) AS AutoShipProduct
	FROM Skus AS s
	JOIN Products AS p ON p.Id = s.IdProduct	
	LEFT JOIN ProductOptionTypes AS popt ON popt.Name = N'SubProductGroupName' AND popt.IdObjectType = p.IdObjectType
	LEFT JOIN ProductOptionValues AS pval ON pval.IdProduct = p.Id AND pval.IdOptionType = popt.Id	
	LEFT JOIN ProductOptionTypes AS sopt ON sopt.Name = N'QTY' AND sopt.IdObjectType = p.IdObjectType
	LEFT JOIN ProductOptionValues AS sval ON sval.IdSku = s.Id AND sval.IdOptionType = sopt.Id
	LEFT JOIN ProductOptionTypes AS aopt ON aopt.Name = N'AutoShipProduct' AND aopt.IdObjectType = p.IdObjectType
	LEFT JOIN ProductOptionValues AS aval ON aval.IdSku = s.Id AND aval.IdOptionType = aopt.Id

GO

