IF OBJECT_ID(N'[dbo].[VShortProducts]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VShortProducts]
GO
CREATE VIEW [dbo].[VShortProducts]
AS 
	SELECT 
		p.Id,
		p.Name+(CASE WHEN val.Value IS NULL OR val.Value ='' THEN ' - ' +CAST(p.Id as NVARCHAR(10)) ELSE ' '+val.Value+' - ' +CAST(p.Id as NVARCHAR(10)) END) [Description]
	FROM Products AS p WITH(NOLOCK)
	LEFT JOIN ProductOptionTypes AS opt ON opt.Name = N'SubTitle' AND opt.IdObjectType = p.IdObjectType
	LEFT JOIN ProductOptionValues AS val ON val.IdProduct = p.Id AND val.IdOptionType = opt.Id
	WHERE p.StatusCode!=3

GO