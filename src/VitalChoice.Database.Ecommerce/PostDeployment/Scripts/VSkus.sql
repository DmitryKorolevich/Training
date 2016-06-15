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
	p.IdVisibility As ProductIdVisibility,
	s.Hidden,
	p.IdObjectType AS IdProductType,
	p.Name,
	s.[Order], 
	psval.Value as SubTitle,
	p.Name +ISNULL(' '+psval.Value, '')+ISNULL(' ('+sval.Value+')', '') AS DescriptionName,
	CONVERT(bit,ISNULL(aval.Value, 0)) AS AutoShipProduct,
	CONVERT(bit,ISNULL(aval1.Value, 0)) AS AutoShipFrequency1,
	CONVERT(bit,ISNULL(aval2.Value, 0)) AS AutoShipFrequency2,
	CONVERT(bit,ISNULL(aval3.Value, 0)) AS AutoShipFrequency3,
	CONVERT(bit,ISNULL(aval6.Value, 0)) AS AutoShipFrequency6,
	CONVERT(bit,ISNULL(dsval.Value, 1)) AS DisregardStock,
	CONVERT(int,ISNULL(stval.Value, 0)) AS Stock
	FROM Skus AS s
	JOIN Products AS p ON p.Id = s.IdProduct	
	INNER JOIN ProductOptionTypes AS psopt ON psopt.Name = N'SubTitle' AND (psopt.IdObjectType = p.IdObjectType OR psopt.IdObjectType IS NULL)
	LEFT JOIN ProductOptionValues AS psval ON psval.IdProduct = p.Id AND psval.IdOptionType = psopt.Id	
	INNER JOIN SkuOptionTypes AS sopt ON sopt.Name = N'QTY' AND (sopt.IdObjectType = p.IdObjectType OR sopt.IdObjectType IS NULL)
	LEFT JOIN SkuOptionValues AS sval ON sval.IdSku = s.Id AND sval.IdOptionType = sopt.Id
	INNER JOIN SkuOptionTypes AS aopt ON aopt.Name = N'AutoShipProduct' AND (aopt.IdObjectType = p.IdObjectType OR aopt.IdObjectType IS NULL)
	LEFT JOIN SkuOptionValues AS aval ON aval.IdSku = s.Id AND aval.IdOptionType = aopt.Id
	INNER JOIN SkuOptionTypes AS aopt1 ON aopt1.Name = N'AutoShipFrequency1' AND (aopt1.IdObjectType = p.IdObjectType OR aopt1.IdObjectType IS NULL)
	LEFT JOIN SkuOptionValues AS aval1 ON aval1.IdSku = s.Id AND aval1.IdOptionType = aopt1.Id
	INNER JOIN SkuOptionTypes AS aopt2 ON aopt2.Name = N'AutoShipFrequency2' AND (aopt2.IdObjectType = p.IdObjectType OR aopt2.IdObjectType IS NULL)
	LEFT JOIN SkuOptionValues AS aval2 ON aval2.IdSku = s.Id AND aval2.IdOptionType = aopt2.Id
	INNER JOIN SkuOptionTypes AS aopt3 ON aopt3.Name = N'AutoShipFrequency3' AND (aopt3.IdObjectType = p.IdObjectType OR aopt3.IdObjectType IS NULL)
	LEFT JOIN SkuOptionValues AS aval3 ON aval3.IdSku = s.Id AND aval3.IdOptionType = aopt3.Id
	INNER JOIN SkuOptionTypes AS aopt6 ON aopt6.Name = N'AutoShipFrequency6' AND (aopt6.IdObjectType = p.IdObjectType OR aopt6.IdObjectType IS NULL)
	LEFT JOIN SkuOptionValues AS aval6 ON aval6.IdSku = s.Id AND aval6.IdOptionType = aopt6.Id
	INNER JOIN SkuOptionTypes AS dsopt ON dsopt.Name = N'DisregardStock' AND (dsopt.IdObjectType = p.IdObjectType OR dsopt.IdObjectType IS NULL)
	LEFT JOIN SkuOptionValues AS dsval ON dsval.IdSku = s.Id AND dsval.IdOptionType = dsopt.Id
	INNER JOIN SkuOptionTypes AS stopt ON stopt.Name = N'Stock' AND (stopt.IdObjectType = p.IdObjectType OR stopt.IdObjectType IS NULL)
	LEFT JOIN SkuOptionValues AS stval ON stval.IdSku = s.Id AND stval.IdOptionType = stopt.Id

GO

