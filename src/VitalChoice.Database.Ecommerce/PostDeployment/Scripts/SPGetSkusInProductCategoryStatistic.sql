IF OBJECT_ID(N'[dbo].[SPGetSkusInProductCategoryStatistic]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetSkusInProductCategoryStatistic]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE SPGetSkusInProductCategoryStatistic
	@from datetime2, @to datetime2, @idcategory int
AS
BEGIN

	SELECT s.Code, MIN(p.Name) As ProductName, MIN(pc.Name) As Category, MIN(rpc.Name) As ParentCategory,
	MIN(s.Price) Price, MIN(s.WholesalePrice) WholesalePrice, SUM(os.Quantity) Quantity, SUM(os.Quantity*os.Amount) Amount
	FROM OrderToSkus os
	INNER JOIN Orders o on os.IdOrder=o.Id
	INNER JOIN Skus s ON os.IdSku=s.Id
	INNER JOIN Products p ON p.Id=s.IdProduct
	INNER JOIN ProductsToCategories ptc ON s.IdProduct=ptc.IdProduct aND ptc.IdCategory=@idcategory
	INNER JOIN ProductCategories pc ON ptc.IdCategory=pc.Id
	LEFT JOIN ProductCategories rpc ON pc.ParentId=rpc.Id
	WHERE o.StatusCode!=3 AND o.OrderStatus IN (2,3,5) AND o.IdObjectType NOT IN (5,6)  AND 
	o.DateCreated>=@from AND o.DateCreated<@to
	GROUP BY s.Code

END

GO