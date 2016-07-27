IF OBJECT_ID(N'[dbo].[SPGetProductQualitySaleIssuesReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetProductQualitySaleIssuesReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetProductQualitySaleIssuesReport]
	@from datetime2,
	@to datetime2
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @idservicecode nvarchar(10)
	SET @idservicecode = 
	(SELECT TOP 1 v.Id FROM Lookups l
	JOIN LookupVariants v ON l.Id=v.IdLookup
	WHERE l.Name='ServiceCodes' AND v.ValueVariant='Product - Quality')
	
	DECLARE @issues AS TABLE
    (
        Id int NOT NULL,
        Code nvarchar(250) NOT NULL,
		Issues int NOT NULL
    )

	INSERT INTO @issues
	(
		Id,
		Code,
		Issues
	)
	(
		SELECT
			temp.Id,
			MIN(temp.Code) Code,
			COUNT(*) Issues		
		FROM
		(
			SELECT 
				s.Id,
				s.Code
			FROM Orders o WITH(NOLOCK)
			INNER JOIN ReshipProblemSkus r WITH(NOLOCK) ON o.Id=r.IdOrder
			INNER JOIN Skus s WITH(NOLOCK) ON r.IdSku=s.Id
			LEFT JOIN OrderOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'ServiceCode' AND sopt.IdObjectType = o.IdObjectType
			LEFT JOIN OrderOptionValues AS sval WITH(NOLOCK) ON sval.IdOrder = o.Id AND sval.IdOptionType = sopt.Id	
			WHERE 
				o.IdObjectType=5 AND o.StatusCode!=3 AND
				((o.OrderStatus IS NOT NULL AND o.OrderStatus!=4) OR
				(o.OrderStatus IS NULL AND o.POrderStatus!=4 AND o.NPOrderStatus!=4)) AND
				o.DateCreated>=@from AND o.DateCreated<=@to AND
				sval.[Value]=@idservicecode
			UNION ALL
			SELECT 
				s.Id,
				s.Code
			FROM Orders o WITH(NOLOCK)
			INNER JOIN RefundSkus r WITH(NOLOCK) ON o.Id=r.IdOrder
			INNER JOIN Skus s WITH(NOLOCK) ON r.IdSku=s.Id
			LEFT JOIN OrderOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'ServiceCode' AND sopt.IdObjectType = o.IdObjectType
			LEFT JOIN OrderOptionValues AS sval WITH(NOLOCK) ON sval.IdOrder = o.Id AND sval.IdOptionType = sopt.Id	
			WHERE 
				o.IdObjectType=6 AND o.StatusCode!=3 AND
				((o.OrderStatus IS NOT NULL AND o.OrderStatus!=4) OR
				(o.OrderStatus IS NULL AND o.POrderStatus!=4 AND o.NPOrderStatus!=4)) AND
				o.DateCreated>=@from AND o.DateCreated<=@to AND
				sval.[Value]=@idservicecode
		) temp
		GROUP BY temp.Id
	)

	SELECT
		sales.IdSku ID,
		(SELECT TOP 1 i.Code FROM @issues i WHERE i.Id=sales.IdSku) Code,
		sales.Count Sales,
		(SELECT TOP 1 i.Issues FROM @issues i WHERE i.Id=sales.IdSku) Issues
	FROM
	(
		SELECT 
			temp.IdSku,
			COUNT(*) [Count]
		FROM
		(
			SELECT 
				inos.IdSku
				FROM Orders ino WITH(NOLOCK)
				INNER JOIN OrderToSkus inos WITH(NOLOCK) ON ino.Id=inos.IdOrder
				WHERE 
					inos.IdSku IN (SELECT Id FROM @issues)
					AND ino.DateCreated>=@from AND ino.DateCreated<=@to AND
					ino.IdObjectType!=2 AND ino.StatusCode!=3 AND
					((ino.OrderStatus IS NOT NULL AND ino.OrderStatus!=1 AND ino.OrderStatus!=4) OR
					(ino.OrderStatus IS NULL AND ino.POrderStatus!=1 AND ino.NPOrderStatus!=1 AND ino.POrderStatus!=4 AND ino.NPOrderStatus!=4))
			UNION ALL
			SELECT
				inos.IdSku
				FROM Orders ino WITH(NOLOCK)
				INNER JOIN OrderToSkus inos WITH(NOLOCK) ON ino.Id=inos.IdOrder
				WHERE 
					inos.IdSku IN (SELECT Id FROM @issues)
					AND ino.DateCreated>=@from AND ino.DateCreated<=@to AND
					ino.IdObjectType!=2 AND ino.StatusCode!=3 AND
					((ino.OrderStatus IS NOT NULL AND ino.OrderStatus!=1 AND ino.OrderStatus!=4) OR
					(ino.OrderStatus IS NULL AND ino.POrderStatus!=1 AND ino.NPOrderStatus!=1 AND ino.POrderStatus!=4 AND ino.NPOrderStatus!=4))
		) temp
		GROUP BY temp.IdSku
	) sales
	
END

GO

IF OBJECT_ID(N'[dbo].[SPGetProductQualitySkuIssuesReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetProductQualitySkuIssuesReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetProductQualitySkuIssuesReport]
	@from datetime2,
	@to datetime2,
	@idsku int
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @idservicecode nvarchar(10)
	SET @idservicecode = 
	(SELECT TOP 1 v.Id FROM Lookups l
	JOIN LookupVariants v ON l.Id=v.IdLookup
	WHERE l.Name='ServiceCodes' AND v.ValueVariant='Product - Quality')
	
	SELECT 
		temp.Id,
		temp.IdObjectType,
		temp.IdOrderSource IdOrderSource,
		os.IdObjectType OrderSourceIdObjectType,
		os.DateCreated DateCreated,
		cadlval.Value LastName,
		sval.Value OrderNotes
	FROM
	(
		SELECT
			pro.Id,
			MIN(pro.IdOrderSource) IdOrderSource,
			MIN(pro.IdObjectType) IdObjectType
		FROM
		(
			SELECT 
				o.Id,
				o.IdOrderSource,
				o.IdObjectType
			FROM Orders o WITH(NOLOCK)
			INNER JOIN ReshipProblemSkus r WITH(NOLOCK) ON o.Id=r.IdOrder
			INNER JOIN Skus s WITH(NOLOCK) ON r.IdSku=s.Id
			LEFT JOIN OrderOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'ServiceCode' AND sopt.IdObjectType = o.IdObjectType
			LEFT JOIN OrderOptionValues AS sval WITH(NOLOCK) ON sval.IdOrder = o.Id AND sval.IdOptionType = sopt.Id	
			WHERE 
				s.Id=@idsku AND
				o.IdObjectType=5 AND o.StatusCode!=3 AND
				((o.OrderStatus IS NOT NULL AND o.OrderStatus!=4) OR
				(o.OrderStatus IS NULL AND o.POrderStatus!=4 AND o.NPOrderStatus!=4)) AND
				o.DateCreated>=@from AND o.DateCreated<=@to AND
				sval.[Value]=@idservicecode
			UNION ALL
			SELECT 
				o.Id,
				o.IdOrderSource,
				o.IdObjectType
			FROM Orders o WITH(NOLOCK)
			INNER JOIN RefundSkus r WITH(NOLOCK) ON o.Id=r.IdOrder
			INNER JOIN Skus s WITH(NOLOCK) ON r.IdSku=s.Id
			LEFT JOIN OrderOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'ServiceCode' AND sopt.IdObjectType = o.IdObjectType
			LEFT JOIN OrderOptionValues AS sval WITH(NOLOCK) ON sval.IdOrder = o.Id AND sval.IdOptionType = sopt.Id	
			WHERE 
				s.Id=@idsku AND
				o.IdObjectType=6 AND o.StatusCode!=3 AND
				((o.OrderStatus IS NOT NULL AND o.OrderStatus!=4) OR
				(o.OrderStatus IS NULL AND o.POrderStatus!=4 AND o.NPOrderStatus!=4)) AND
				o.DateCreated>=@from AND o.DateCreated<=@to AND
				sval.[Value]=@idservicecode
		) pro
		GROUP BY pro.Id
	) temp
	INNER JOIN Orders os WITH(NOLOCK) ON temp.IdOrderSource = os.Id
	INNER JOIN Customers c WITH(NOLOCK) ON os.IdCustomer = c.Id
	LEFT JOIN AddressOptionTypes AS cadlopt WITH(NOLOCK) ON cadlopt.Name = N'LastName'
	LEFT JOIN AddressOptionValues AS cadlval WITH(NOLOCK) ON cadlval.IdAddress = c.IdProfileAddress AND cadlval.IdOptionType = cadlopt.Id
	LEFT JOIN OrderOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'OrderNotes' AND sopt.IdObjectType = temp.IdObjectType
	LEFT JOIN OrderOptionValues AS sval WITH(NOLOCK) ON sval.IdOrder = temp.Id AND sval.IdOptionType = sopt.Id
	WHERE 
		os.IdObjectType!=2 AND os.StatusCode!=3 AND
		((os.OrderStatus IS NOT NULL AND os.OrderStatus!=1 AND os.OrderStatus!=4) OR
		(os.OrderStatus IS NULL AND os.POrderStatus!=1 AND os.NPOrderStatus!=1 AND os.POrderStatus!=4 AND os.NPOrderStatus!=4))
	
END

GO