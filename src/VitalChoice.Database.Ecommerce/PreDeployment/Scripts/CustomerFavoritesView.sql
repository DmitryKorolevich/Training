IF OBJECT_ID(N'[dbo].[VCustomerFavorites]', N'V') IS NOT NULL
DROP VIEW [dbo].[VCustomerFavorites]
GO

CREATE VIEW [dbo].[VCustomerFavorites]
AS
SELECT        p.Id, p.Name AS ProductName, o.IdCustomer, ISNULL(val.Value, opt.DefaultValue) AS ProductThumbnail, SUM(ots.Quantity) AS Quantity
FROM            dbo.OrderToSkus AS ots INNER JOIN
                         dbo.Orders AS o ON ots.IdOrder = o.Id AND (o.OrderStatus = 2 OR
                         o.OrderStatus = 3 OR
                         o.OrderStatus = 5) INNER JOIN
                         dbo.Skus AS s ON ots.IdSku = s.Id INNER JOIN
                         dbo.Products AS p ON s.IdProduct = p.Id AND p.Hidden = 0 LEFT OUTER JOIN
                         dbo.ProductOptionTypes AS opt ON opt.Name = N'Thumbnail' AND opt.IdObjectType = p.IdObjectType LEFT OUTER JOIN
                         dbo.ProductOptionValues AS val ON val.IdProduct = p.Id AND val.IdOptionType = opt.Id
GROUP BY p.Id, p.Name, p.Url, o.IdCustomer, val.Value, opt.DefaultValue

GO