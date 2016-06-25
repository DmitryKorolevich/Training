/****** Object:  View [dbo].[VProductSkus]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VProductSkus]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[VProductSkus]
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
	p.IdEditedBy,
	p.IdVisibility,
	p.IdObjectType AS IdProductType,
	p.Name,
	ISNULL(val.Value, opt.DefaultValue) AS Thumbnail,	
	ISNULL(tval.Value, topt.DefaultValue) AS TaxCode,
	ISNULL(sval.Value, sopt.DefaultValue) AS SubTitle
	FROM Products AS p
	LEFT JOIN Skus AS s ON p.Id = s.IdProduct
	LEFT JOIN ProductOptionTypes AS opt ON opt.Name = N''Thumbnail'' AND opt.IdObjectType = p.IdObjectType
	LEFT JOIN ProductOptionValues AS val ON val.IdProduct = p.Id AND val.IdOptionType = opt.Id
	LEFT JOIN ProductOptionTypes AS topt ON topt.Name = N''TaxCode'' AND topt.IdObjectType = p.IdObjectType
	LEFT JOIN ProductOptionValues AS tval ON tval.IdProduct = p.Id AND tval.IdOptionType = topt.Id
	LEFT JOIN ProductOptionTypes AS sopt ON sopt.Name = N''SubTitle'' AND sopt.IdObjectType = p.IdObjectType
	LEFT JOIN ProductOptionValues AS sval ON sval.IdProduct = p.Id AND sval.IdOptionType = sopt.Id

' 
GO
