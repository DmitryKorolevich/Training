IF OBJECT_ID(N'[dbo].[SPGetCustomerSkuUsageReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetCustomerSkuUsageReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE SPGetCustomerSkuUsageReport
	@from datetime2,
	@to datetime2, 
	@skus nvarchar(MAX)= NULL,
	@idcategory int= NULL,
	@idcustomertype int= NULL,
	@pageindex int = NULL,
	@pagesize int =NULL
AS
BEGIN	

	DECLARE @skusFilterUsed BIT=0
	DECLARE @doNotMailId INT, @firstNameId INT,@lastNameId INT, @addres1Id INT, @addres2Id INT, @cityId INT,@zipId INT

	DECLARE @skusIdsFromSkuFilter AS TABLE
    (
        Id int NOT NULL
    );
	DECLARE @skusIds AS TABLE
    (
        Id int NOT NULL
    );

	DECLARE @customerSkuData AS TABLE
	(
		IdCustomer int NOT NULL,
		IdSku int NOT NULL,
		CustomerIdObjectType int NOT NULL,
		Quantity int NOT NULL,
		LastIdOrder int NOT NULL,
		TotalCount INT NOT NULL
	);
	
	SET @doNotMailId =
	(
		SELECT 
		TOP 1 Id
		FROM CustomerOptionTypes
		WHERE Name='DoNotMail'
	);
	SET @firstNameId =
	(
		SELECT 
		TOP 1 Id
		FROM AddressOptionTypes
		WHERE Name='FirstName'
	);
	SET @lastNameId =
	(
		SELECT 
		TOP 1 Id
		FROM AddressOptionTypes
		WHERE Name='LastName'
	);
	SET @addres1Id =
	(
		SELECT 
		TOP 1 Id
		FROM AddressOptionTypes
		WHERE Name='Address1' AND IdObjectType=3
	);
	SET @addres2Id =
	(
		SELECT 
		TOP 1 Id
		FROM AddressOptionTypes
		WHERE Name='Address2' AND IdObjectType=3
	);
	SET @cityId =
	(
		SELECT 
		TOP 1 Id
		FROM AddressOptionTypes
		WHERE Name='City' AND IdObjectType=3
	);
	SET @zipId =
	(
		SELECT 
		TOP 1 Id
		FROM AddressOptionTypes
		WHERE Name='Zip' AND IdObjectType=3
	);

	IF(@idcategory IS NOT NULL)
	BEGIN

		INSERT INTO @skusIdsFromSkuFilter
			(Id)
		(SELECT
			Id
		FROM TFGetTableIdsByString(@skus, DEFAULT))
		
		INSERT INTO @skusIds
		(Id)
		(
			SELECT 
				s.Id
			FROM ProductsToCategories pc
			JOIN Skus s ON pc.IdProduct=s.IdProduct
			WHERE 
				pc.IdCategory=@idcategory AND
				(@skus IS NULL OR s.Id IN 
				(
					SELECT Id FROM @skusIdsFromSkuFilter
				))
		)
		SET @skusFilterUsed=1

	END
	ELSE
	BEGIN
		
		INSERT INTO @skusIds
			(Id)
		(SELECT
			Id
		FROM TFGetTableIdsByString(@skus, DEFAULT))

		IF(@skus IS NOT NULL)
		BEGIN
			SET @skusFilterUsed=1
		END
	END;
	
	WITH tempQuery(
		IdCustomer,
		IdSku, 
		CustomerIdObjectType,
		Quantity,
		LastIdOrder
	) As
	(
		SELECT 
			temp.IdCustomer,
			temp.IdSku,
			MIN(temp.CustomerIdObjectType) CustomerIdObjectType,
			SUM(temp.Quantity) Quantity,
			MAX(temp.IdOrder) LastIdOrder
		FROM
		(
			SELECT
				o.IdCustomer,
				c.IdObjectType CustomerIdObjectType,
				os.IdSku,
				os.Quantity,
				o.Id IdOrder
			FROM Orders o WITH(NOLOCK)
			JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
			JOIN OrderToSkus os WITH(NOLOCK) ON o.Id=os.IdOrder
			WHERE 
				o.DateCreated>=@from AND o.DateCreated<@to AND o.StatusCode!=3 AND
				o.IdObjectType NOT IN (2,6) AND 
				(
					(o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) OR 
					(o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND 
					o.NPOrderStatus !=1 AND o.NPOrderStatus !=4)
				) AND
				(@idcustomertype IS NULL OR c.IdObjectType = @idcustomertype) AND 
				(@skusFilterUsed=0 OR os.IdSku IN 
				(
					SELECT Id FROM @skusIds
				))
			UNION ALL
				SELECT 
				o.IdCustomer,
				c.IdObjectType CustomerIdObjectType,
				op.IdSku,
				op.Quantity,
				o.Id IdOrder
			FROM Orders o WITH(NOLOCK)
			JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
			JOIN OrderToPromos op WITH(NOLOCK) ON o.Id=op.IdOrder AND op.Disabled=0
			WHERE 
				o.DateCreated>=@from AND o.DateCreated<@to AND o.StatusCode!=3 AND
				o.IdObjectType NOT IN (2,6) AND 
				(
					(o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) OR 
					(o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND 
					o.NPOrderStatus !=1 AND o.NPOrderStatus !=4)
				) AND
				(@idcustomertype IS NULL OR c.IdObjectType = @idcustomertype) AND 
				(@skusFilterUsed=0 OR op.IdSku IN 
				(
					SELECT Id FROM @skusIds
				))
		) temp
		GROUP BY temp.IdCustomer, temp.IdSku
	)

	INSERT @customerSkuData
	(
		IdCustomer,
		IdSku,
		CustomerIdObjectType,
		Quantity,
		LastIdOrder,
		TotalCount
	)
	SELECT 
		IdCustomer,
		IdSku,
		CustomerIdObjectType,
		Quantity,
		LastIdOrder,
		TotalCount
	FROM
	(
		SELECT 
			IdCustomer,
			IdSku,
			CustomerIdObjectType,
			Quantity,
			LastIdOrder,
			ROW_NUMBER() OVER (ORDER BY IdCustomer DESC, IdSku ASC) AS RowNumber
		FROM tempQuery
	) temp
	CROSS JOIN (SELECT Count(*) AS TotalCount FROM tempQuery) AS tCount
	WHERE @pageindex is NULL OR (RowNumber>(@pageindex-1)*@pagesize AND RowNumber<=@pageindex*@pagesize)
	ORDER BY temp.IdCustomer DESC, IdSku ASC
	OPTION(RECOMPILE)

	SELECT
		d.IdCustomer,
		d.IdSku,
		d.CustomerIdObjectType,
		d.Quantity,
		d.LastIdOrder,
		d.TotalCount,
		o.DateCreated LastOrderDateCreated,
		c.Email,
		s.Code Code,
		stuff((
			SELECT
				',' + CAST(pc.IdCategory as nvarchar(10))
			FROM ProductsToCategories pc WITH(NOLOCK)
			WHERE pc.IdProduct=s.IdProduct
			ORDER BY pc.IdCategory
			FOR XML PATH('')
			)
		,1,1,'') as CategoryIds,
		CAST(cval.Value as BIT) DoNotMail,
		ofnval.Value ShippingFirstName,
		olnval.Value ShippingLastName,
		oa1val.Value ShippingAddress1,
		oa2val.Value ShippingAddress2,
		ocval.Value ShippingCity,
		ozval.Value ShippingZip,
		oad.IdCountry ShippingIdCountry,
		oad.IdState ShippingIdState,
		oad.County ShippingCounty
	FROM @customerSkuData d
	JOIN Customers c WITH(NOLOCK) ON d.IdCustomer=c.Id
	JOIN Orders o WITH(NOLOCK) ON d.LastIdOrder=o.Id
	JOIN Skus s WITH(NOLOCK) ON d.IdSku=s.Id
	LEFT JOIN CustomerOptionValues AS cval WITH(NOLOCK) ON cval.IdCustomer = d.IdCustomer AND cval.IdOptionType = @doNotMailId
	LEFT JOIN OrderAddresses oad WITH(NOLOCK) ON oad.Id=o.IdShippingAddress
	LEFT JOIN OrderAddressOptionValues AS ofnval WITH(NOLOCK) ON ofnval.IdOrderAddress = o.IdShippingAddress AND ofnval.IdOptionType = @firstNameId
	LEFT JOIN OrderAddressOptionValues AS olnval WITH(NOLOCK) ON olnval.IdOrderAddress = o.IdShippingAddress AND olnval.IdOptionType = @lastNameId
	LEFT JOIN OrderAddressOptionValues AS oa1val WITH(NOLOCK) ON oa1val.IdOrderAddress = o.IdShippingAddress AND oa1val.IdOptionType = @addres1Id
	LEFT JOIN OrderAddressOptionValues AS oa2val WITH(NOLOCK) ON oa2val.IdOrderAddress = o.IdShippingAddress AND oa2val.IdOptionType = @addres2Id
	LEFT JOIN OrderAddressOptionValues AS ocval WITH(NOLOCK) ON ocval.IdOrderAddress = o.IdShippingAddress AND ocval.IdOptionType = @cityId
	LEFT JOIN OrderAddressOptionValues AS ozval WITH(NOLOCK) ON ozval.IdOrderAddress = o.IdShippingAddress AND ozval.IdOptionType = @zipId
	ORDER BY IdCustomer DESC, IdSku ASC
	OPTION(RECOMPILE)
END

GO