GO

IF OBJECT_ID(N'[dbo].[VCustomerFavorites]', N'V') IS NOT NULL
	DROP VIEW [dbo].[VCustomerFavorites]
GO
CREATE VIEW [dbo].[VCustomerFavorites]
AS
SELECT temp.Id, temp.ProductName, ISNULL(val.Value, opt.DefaultValue) AS ProductThumbnail,
	temp.IdCustomer, ISNULL(stval.Value, stopt.DefaultValue) AS ProductSubTitle, temp.Quantity FROM
	(SELECT p.Id, p.IdObjectType, p.Name AS ProductName, o.IdCustomer, SUM(ots.Quantity) AS Quantity
	FROM            dbo.OrderToSkus AS ots INNER JOIN
							 dbo.Orders AS o ON ots.IdOrder = o.Id AND (
							 o.OrderStatus = 2 OR
							 o.OrderStatus = 3 OR
							 o.OrderStatus = 5 OR
							 o.OrderStatus = 6 OR
							 o.OrderStatus = 7 OR
							 o.POrderStatus = 2 OR
							 o.POrderStatus = 3 OR
							 o.POrderStatus = 5 OR
							 o.POrderStatus = 6 OR
							 o.POrderStatus = 7 OR
							 o.NPOrderStatus = 2 OR
							 o.NPOrderStatus = 3 OR
							 o.NPOrderStatus = 5 OR
							 o.NPOrderStatus = 6 OR
							 o.NPOrderStatus = 7) INNER JOIN
							 dbo.Skus AS s ON ots.IdSku = s.Id INNER JOIN
							 dbo.Products AS p ON s.IdProduct = p.Id AND p.IdVisibility IS NOT NULL
	GROUP BY p.Id, p.IdObjectType, p.Name, o.IdCustomer) temp
LEFT OUTER JOIN dbo.ProductOptionTypes AS opt ON opt.Name = N'Thumbnail' AND opt.IdObjectType = temp.IdObjectType
LEFT OUTER JOIN dbo.ProductOptionValues AS val ON val.IdProduct = temp.Id AND val.IdOptionType = opt.Id
LEFT OUTER JOIN dbo.ProductOptionTypes AS stopt ON stopt.Name = N'SubTitle' AND stopt.IdObjectType = temp.IdObjectType
LEFT OUTER JOIN dbo.ProductOptionValues AS stval ON stval.IdProduct = temp.Id AND stval.IdOptionType = stopt.Id


GO

