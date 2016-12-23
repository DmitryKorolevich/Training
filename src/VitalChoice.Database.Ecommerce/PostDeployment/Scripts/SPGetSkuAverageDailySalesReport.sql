IF OBJECT_ID(N'[dbo].[SPGetSkuAverageDailySalesReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetSkuAverageDailySalesReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE SPGetSkuAverageDailySalesReport
	@from datetime2,
	@to datetime2, 
	@skus nvarchar(MAX),
	@idcustomertype int,
	@mode int
AS
BEGIN		
		
	DECLARE @skuIds AS TABLE
    (
        Id int NOT NULL
    );

	DECLARE @skusStatistic AS TABLE
    (
        Id int NOT NULL,
		TotalAmount money NOT NULL,
		TotalQuantity INT NOT NULL
    );
	
	IF(@skus IS NOT NULL)
	BEGIN
		INSERT INTO @skuIds
		(Id)
		SELECT DISTINCT s.Id
		FROM Skus s WITH(NOLOCK)
		WHERE 
			s.Id IN (SELECT Id FROM TFGetTableIdsByString(@skus, DEFAULT))

		IF(@mode=2)--by product
		BEGIN

			INSERT INTO @skuIds
			(Id)
			SELECT 
				DISTINCT al.Id
			FROM @skuIds cs
			JOIN Skus s ON cs.Id=s.Id
			JOIN Skus al ON s.IdProduct=al.IdProduct
			WHERE al.Id NOT IN 
			(
				SELECT Id FROM @skuIds
			)

		END
	END
	
	INSERT INTO @skusStatistic
	(
		Id,
		TotalAmount,
		TotalQuantity
	)
	(
		SELECT 
			temp.IdSku,
			SUM(temp.Amount) TotalAmount,
			SUM(temp.Quantity) TotalQuantity
		FROM 
		(
			SELECT 
				os.IdSku,
				os.Quantity*os.Amount Amount,
				os.Quantity Quantity
			FROM Orders o WITH(NOLOCK)
			JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
			JOIN OrderToSkus os WITH(NOLOCK) ON o.Id=os.IdOrder
			LEFT JOIN @skuIds sids ON os.IdSku=sids.Id
			WHERE
				o.DateCreated>=@from AND o.DateCreated<@to AND
				o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
				(
					o.OrderStatus IN(2,3,5,6,7)
					OR 
					(
						o.POrderStatus IN(2,3,5,6,7) 
						OR 
						o.NPOrderStatus IN(2,3,5,6,7)
					)
				) 
				AND
				(@idcustomertype IS NULL OR c.IdObjectType = @idcustomertype) AND
				(@skus IS NULL OR sids.Id IS NOT NULL)
			UNION ALL
			SELECT 
				op.IdSku,
				op.Quantity*op.Amount Amount,
				op.Quantity Quantity
			FROM Orders o WITH(NOLOCK)
			JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
			JOIN OrderToPromos op WITH(NOLOCK) ON o.Id=op.IdOrder AND op.Disabled=0
			LEFT JOIN @skuIds sids ON op.IdSku=sids.Id
			WHERE
				o.DateCreated>=@from AND o.DateCreated<@to AND
				o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
				(
					o.OrderStatus IN(2,3,5,6,7)
					OR 
					(
						o.POrderStatus IN(2,3,5,6,7) 
						OR 
						o.NPOrderStatus IN(2,3,5,6,7)
					)
				) 
				AND
				(@idcustomertype IS NULL OR c.IdObjectType = @idcustomertype) AND
				(@skus IS NULL OR sids.Id IS NOT NULL)
		) temp
		GROUP BY temp.IdSku
	)

	SELECT
		s.Id,
		ISNULL(st.TotalAmount, 0.00) TotalAmount,
		ISNULL(st.TotalQuantity, 0) TotalQuantity,
		STUFF
		(
			(
				SELECT 
					', ',
					pc.Name AS [text()]
				FROM ProductsToCategories ptpc WITH(NOLOCK)
				JOIN ProductCategories pc WITH(NOLOCK) ON ptpc.IdCategory=pc.Id
				WHERE ptpc.IdProduct = s.IdProduct
				FOR XML PATH ('')
			), 1, 2, ''
		) as ProductCategories,
		s.Code,
		s.StatusCode,
		s.Hidden,
		p.Id IdProduct,
		p.StatusCode ProductStatusCode,
		p.IdVisibility ProductIdVisibility,
		p.IdObjectType ProductIdObjectType,
		p.Name,
		psval.Value as SubTitle,
		CAST(sqval.Value as INT) as QTY,
		CAST(sdsval.Value as BIT) as DisregardStock,
		CAST(ssval.Value as INT) as Stock
	FROM Skus s WITH(NOLOCK)
	LEFT JOIN @skusStatistic st ON s.Id=st.Id
	JOIN Products p WITH(NOLOCK) ON s.IdProduct=p.Id
	LEFT JOIN ProductOptionTypes AS psopt WITH(NOLOCK) ON psopt.Name = N'SubTitle' AND psopt.IdObjectType = p.IdObjectType
	LEFT JOIN ProductOptionValues AS psval WITH(NOLOCK) ON psval.IdProduct = p.Id AND psval.IdOptionType = psopt.Id		
	LEFT JOIN SkuOptionTypes AS sqopt WITH(NOLOCK) ON sqopt.Name = N'QTY'
	LEFT JOIN SkuOptionValues AS sqval WITH(NOLOCK) ON sqval.IdSku = s.Id AND sqval.IdOptionType = sqopt.Id		
	LEFT JOIN SkuOptionTypes AS sdsopt WITH(NOLOCK) ON sdsopt.Name = N'DisregardStock' AND sdsopt.IdObjectType = p.IdObjectType
	LEFT JOIN SkuOptionValues AS sdsval WITH(NOLOCK) ON sdsval.IdSku = s.Id AND sdsval.IdOptionType = sdsopt.Id		
	LEFT JOIN SkuOptionTypes AS ssopt WITH(NOLOCK) ON ssopt.Name = N'Stock' AND ssopt.IdObjectType = p.IdObjectType
	LEFT JOIN SkuOptionValues AS ssval WITH(NOLOCK) ON ssval.IdSku = s.Id AND ssval.IdOptionType = ssopt.Id	
	WHERE s.StatusCode!=3
	OPTION(RECOMPILE)

END

GO