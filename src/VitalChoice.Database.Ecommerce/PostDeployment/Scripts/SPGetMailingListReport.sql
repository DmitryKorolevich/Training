IF OBJECT_ID(N'[dbo].[SPGetMailingListReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetMailingListReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetMailingListReport]
	@from datetime2 = NULL,
	@to datetime2 =NULL,
	@idcustomertype int =NULL,
	@fromordercount int =NULL,
	@toordercount int =NULL,
	@fromfirst datetime2 = NULL,
	@tofirst datetime2 =NULL,
	@fromlast datetime2 = NULL,
	@tolast datetime2 =NULL,
	@lastfromtotal decimal = NULL,
	@lasttototal decimal =NULL,
	@dnm bit =NULL,
	@dnr bit =NULL,
	@idcustomerordersource INT = NULL,
	@keycodefirst nvarchar(250) = NULL,
	@discountcodefirst nvarchar(250) = NULL,
	@pageindex int = NULL,
	@pagesize int =NULL
AS
BEGIN

	SET NOCOUNT ON
	
	DECLARE @count INT
	SET @count=0
	DECLARE @sql NVARCHAR(MAX), @params NVARCHAR(4000)

	DECLARE @dnmId INT, @dnrId INT, @sourceId INT
	
	SELECT @dnmId = Id FROM CustomerOptionTypes WITH(NOLOCK) 
	WHERE Name = N'DoNotMail'

	SELECT @dnrId = Id FROM CustomerOptionTypes WITH(NOLOCK) 
	WHERE Name = N'DoNotRent'

	SELECT @sourceId = Id FROM CustomerOptionTypes WITH(NOLOCK) 
	WHERE Name = N'Source'
	
	IF(@pageindex IS NOT NULL)
	BEGIN
		SET @params = N'@from datetime2,
			@to datetime2,
			@idcustomertype int,
			@fromordercount int,
			@toordercount int,
			@fromfirst datetime2,
			@tofirst datetime2,
			@fromlast datetime2,
			@tolast datetime2,
			@lastfromtotal decimal,
			@lasttototal decimal,
			@dnm bit,
			@dnr bit,
			@idcustomerordersource INT,
			@keycodefirst nvarchar(250),
			@discountcodefirst nvarchar(250),
			@pageindex int,
			@pagesize int,
			@count INT OUTPUT'

		SET @sql = 

		N'WITH orderList
		AS
		(
			SELECT o.Id, o.IdCustomer, o.DateCreated, o.IdObjectType, Total, IdPaymentMethod, IdDiscount FROM Orders AS o
			WHERE
				o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
				(
					(o.OrderStatus IS NOT NULL AND o.OrderStatus NOT IN (1, 4)) 
					OR (o.POrderStatus IS NOT NULL AND o.POrderStatus NOT IN (1, 4) AND o.NPOrderStatus NOT IN (1, 4))
				)
		)
		SELECT
			@count = COUNT(c.Id)
		FROM Customers c WITH(NOLOCK)
		INNER JOIN (
			SELECT DISTINCT
				cc.Id,
				MIN(o.DateCreated) OVER (PARTITION BY cc.Id) AS FirstOrderDateCreated,
				FIRST_VALUE(kcval.Value) OVER (PARTITION BY cc.Id ORDER BY o.DateCreated ASC ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) AS FirstKeyCode,
				FIRST_VALUE(d.Code) OVER (PARTITION BY cc.Id ORDER BY o.DateCreated ASC ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) AS FirstDiscountCode,
				MAX(o.DateCreated) OVER (PARTITION BY cc.Id) AS LastOrderDateCreated,
				FIRST_VALUE(o.Total) OVER (PARTITION BY cc.Id ORDER BY o.DateCreated DESC ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) AS LastOrderTotal,
				FIRST_VALUE(o.IdPaymentMethod) OVER (PARTITION BY cc.Id ORDER BY o.DateCreated DESC ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) AS LastOrderIdPaymentMethod,
				COUNT(o.Id) OVER (PARTITION BY cc.Id) AS OrdersCount
			FROM Customers AS cc WITH (NOLOCK)
			LEFT JOIN orderList AS o WITH(NOLOCK) ON o.IdCustomer=cc.Id
			LEFT JOIN OrderOptionTypes AS kcopt WITH(NOLOCK) ON kcopt.Name = N''KeyCode'' AND kcopt.IdObjectType = o.IdObjectType
			LEFT JOIN OrderOptionValues AS kcval WITH(NOLOCK) ON kcval.IdOrder = o.Id AND kcval.IdOptionType = kcopt.Id
			LEFT JOIN Discounts AS d WITH(NOLOCK) ON d.Id = o.IdDiscount'
			IF @from IS NOT NULL AND @to IS NOT NULL
				SET @sql = @sql + N'
				WHERE
					cc.Id IN (
						SELECT IdCustomer FROM orderList AS o
						WHERE 
							o.DateCreated >= @from AND 
							o.DateCreated <= @to
					)'
			ELSE IF @from IS NOT NULL
				SET @sql = @sql + N'
				WHERE
					cc.Id IN (
						SELECT IdCustomer FROM orderList AS o
						WHERE 
							o.DateCreated >= @from
					)'
			ELSE IF @to IS NOT NULL
				SET @sql = @sql + N'
				WHERE
					cc.Id IN (
						SELECT IdCustomer FROM orderList AS o
						WHERE 
							o.DateCreated <= @to
					)'
			ELSE
				SET @sql = @sql + N'
				WHERE
					cc.Id IN (
						SELECT IdCustomer FROM orderList
					)
				'
			
			IF @idcustomertype IS NOT NULL
				SET @sql = @sql + N'
				AND cc.IdObjectType = @idcustomertype'

		SET @sql = @sql + N') AS cd ON cd.Id = c.Id
				LEFT JOIN CustomerOptionValues AS cmval WITH(NOLOCK) ON cmval.IdCustomer = c.Id AND cmval.IdOptionType = '+CAST(@dnmId AS nvarchar(25))+'
				LEFT JOIN CustomerOptionValues AS crval WITH(NOLOCK) ON crval.IdCustomer = c.Id AND crval.IdOptionType = '+CAST(@dnrId AS nvarchar(25))+'
				LEFT JOIN CustomerOptionValues AS sval WITH(NOLOCK) ON sval.IdCustomer = c.Id AND sval.IdOptionType = '+CAST(@sourceId AS nvarchar(25))+'WHERE 1=1'
		
		IF @fromordercount IS NOT NULL
			SET @sql = @sql + N'
			AND cd.OrdersCount >= @fromordercount'
		
		IF @toordercount IS NOT NULL
			SET @sql = @sql + N'
			AND cd.OrdersCount <= @toordercount'

		IF @fromfirst IS NOT NULL
			SET @sql = @sql + N'
			AND cd.FirstOrderDateCreated >= @fromfirst'

		IF @tofirst IS NOT NULL
			SET @sql = @sql + N'
			AND cd.FirstOrderDateCreated <= @tofirst'

		IF @fromlast IS NOT NULL
			SET @sql = @sql + N'
			AND cd.LastOrderDateCreated >= @fromlast'

		IF @tolast IS NOT NULL
			SET @sql = @sql + N'
			AND cd.LastOrderDateCreated <= @tolast'

		IF @lastfromtotal IS NOT NULL
			SET @sql = @sql + N'
			AND cd.LastOrderTotal >= @lastfromtotal'

		IF @lasttototal IS NOT NULL
			SET @sql = @sql + N'
			AND cd.LastOrderTotal <= @lasttototal'

		IF @keycodefirst IS NOT NULL
			SET @sql = @sql + N'
			AND cd.FirstKeyCode = @keycodefirst'

		IF @discountcodefirst IS NOT NULL
			SET @sql = @sql + N'
			AND cd.FirstDiscountCode = @discountcodefirst'

		IF @dnm IS NOT NULL
			SET @sql = @sql + N'
			AND cmval.Value = '''+CASE @dnm WHEN 1 THEN N'True' ELSE N'False' END+''''

		IF @dnr IS NOT NULL
			SET @sql = @sql + N'
			AND crval.Value = '''+CASE @dnr WHEN 1 THEN N'True' ELSE N'False' END+''''

		IF @idcustomerordersource IS NOT NULL
			SET @sql = @sql + N'
			AND sval.Value = '''+CAST(@idcustomerordersource AS nvarchar(25))+''''

		SET @sql = @sql + N'
		OPTION(RECOMPILE)'

		EXEC sp_executesql @sql, @params, 
			@from, 
			@to,
			@idcustomertype,
			@fromordercount,
			@toordercount,
			@fromfirst,
			@tofirst,
			@fromlast,
			@tolast,
			@lastfromtotal,
			@lasttototal,
			@dnm,
			@dnr,
			@idcustomerordersource,
			@keycodefirst,
			@discountcodefirst,
			@pageindex,
			@pagesize,
			@count = @count OUTPUT;
	END

	SET @sql = N'
		
		WITH orderList
		AS
		(
			SELECT o.Id, o.IdCustomer, o.DateCreated, o.IdObjectType, Total, IdPaymentMethod, IdDiscount FROM Orders AS o
			WHERE
				o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
				(
					(o.OrderStatus IS NOT NULL AND o.OrderStatus NOT IN (1, 4)) 
					OR (o.POrderStatus IS NOT NULL AND o.POrderStatus NOT IN (1, 4) AND o.NPOrderStatus NOT IN (1, 4))
				)
		)
		SELECT
			c.Id,
			c.Email,
			c.IdObjectType AS CustomerIdObjectType,
			cd.FirstOrderDateCreated,
			cd.FirstKeyCode,
			cd.FirstDiscountCode,
			cd.LastOrderDateCreated,
			cd.LastOrderTotal,
			cd.LastOrderIdPaymentMethod,
			cd.OrdersCount,
			cd.OrdersTotal,
			CAST(CASE cmval.Value WHEN N''True'' THEN 1 ELSE 0 END AS BIT) AS DoNotMail,
			CAST(CASE crval.Value WHEN N''True'' THEN 1 ELSE 0 END AS BIT) AS DoNotRent,
			CAST(sval.Value AS INT) AS IdCustomerOrderSource,
			options.FirstName,
			options.LastName,
			options.Address1,
			options.Address2,
			options.City,
			options.Phone,
			options.Zip,
			a.IdCountry,
			a.IdState,
			@count AS [Count]
			FROM Customers c WITH(NOLOCK)'

	SET @sql = @sql + N'
	INNER JOIN (
		SELECT DISTINCT
			cc.Id,
			MIN(o.DateCreated) OVER (PARTITION BY cc.Id) AS FirstOrderDateCreated,
			FIRST_VALUE(kcval.Value) OVER (PARTITION BY cc.Id ORDER BY o.DateCreated ASC ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) AS FirstKeyCode,
			FIRST_VALUE(d.Code) OVER (PARTITION BY cc.Id ORDER BY o.DateCreated ASC ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) AS FirstDiscountCode,
			MAX(o.DateCreated) OVER (PARTITION BY cc.Id) AS LastOrderDateCreated,
			FIRST_VALUE(o.Total) OVER (PARTITION BY cc.Id ORDER BY o.DateCreated DESC ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) AS LastOrderTotal,
			FIRST_VALUE(o.IdPaymentMethod) OVER (PARTITION BY cc.Id ORDER BY o.DateCreated DESC ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) AS LastOrderIdPaymentMethod,
			COUNT(o.Id) OVER (PARTITION BY cc.Id) AS OrdersCount,
			SUM(o.Total) OVER (PARTITION BY cc.Id) AS OrdersTotal
		FROM Customers AS cc WITH (NOLOCK)
		LEFT JOIN orderList AS o WITH(NOLOCK) ON o.IdCustomer=cc.Id
		LEFT JOIN OrderOptionTypes AS kcopt WITH(NOLOCK) ON kcopt.Name = N''KeyCode'' AND kcopt.IdObjectType = o.IdObjectType
		LEFT JOIN OrderOptionValues AS kcval WITH(NOLOCK) ON kcval.IdOrder = o.Id AND kcval.IdOptionType = kcopt.Id
		LEFT JOIN Discounts AS d WITH(NOLOCK) ON d.Id = o.IdDiscount'
		IF @from IS NOT NULL AND @to IS NOT NULL
				SET @sql = @sql + N'
				WHERE
					cc.Id IN (
						SELECT IdCustomer FROM orderList AS o
						WHERE 
							o.DateCreated >= @from AND 
							o.DateCreated <= @to
					)
				'
			ELSE IF @from IS NOT NULL
				SET @sql = @sql + N'
				WHERE
					cc.Id IN (
						SELECT IdCustomer FROM orderList AS o
						WHERE 
							o.DateCreated >= @from
					)
				'
			ELSE IF @to IS NOT NULL
				SET @sql = @sql + N'
				WHERE
					cc.Id IN (
						SELECT IdCustomer FROM orderList AS o
						WHERE 
							o.DateCreated <= @to
					)
				'
			ELSE
				SET @sql = @sql + N'
				WHERE
					cc.Id IN (
						SELECT IdCustomer FROM orderList
					)
				'
			IF @idcustomertype IS NOT NULL
				SET @sql = @sql + N'
				AND cc.IdObjectType = @idcustomertype'
	SET @sql = @sql +
		N'
	) AS cd ON cd.Id = c.Id
	LEFT JOIN CustomerOptionValues AS cmval WITH(NOLOCK) ON cmval.IdCustomer = c.Id AND cmval.IdOptionType = '+CAST(@dnmId AS nvarchar(25))+'
	LEFT JOIN CustomerOptionValues AS crval WITH(NOLOCK) ON crval.IdCustomer = c.Id AND crval.IdOptionType = '+CAST(@dnrId AS nvarchar(25))+'
	LEFT JOIN CustomerOptionValues AS sval WITH(NOLOCK) ON sval.IdCustomer = c.Id AND sval.IdOptionType = '+CAST(@sourceId AS nvarchar(25))+'
	LEFT JOIN OrderPaymentMethods pm ON cd.LastOrderIdPaymentMethod=pm.Id
	LEFT JOIN OrderAddresses a ON pm.IdAddress = a.Id
	LEFT OUTER JOIN 
		(
		SELECT
			IdOrderAddress,
			FirstName,
			LastName,
			Address1,
			Address2,
			City,
			Phone,
			Zip
		FROM 
			(
			SELECT
				IdOrderAddress,
				Name,
				Value 
			FROM AddressOptionTypes AS adt
			INNER JOIN OrderAddressOptionValues AS adv ON adt.Id = adv.IdOptionType
			) AS source
			PIVOT 
			(
				MIN([Value]) FOR [Name] IN ([FirstName], [LastName], [Address1], [Address2], [City], [Phone], [Zip])
			) AS piv
		) AS options ON a.Id = options.IdOrderAddress
	WHERE 1=1
	'

	IF @fromordercount IS NOT NULL
		SET @sql = @sql + N'
		AND cd.OrdersCount >= @fromordercount'
		
	IF @toordercount IS NOT NULL
		SET @sql = @sql + N'
		AND cd.OrdersCount <= @toordercount'

	IF @fromfirst IS NOT NULL
		SET @sql = @sql + N'
		AND cd.FirstOrderDateCreated >= @fromfirst'

	IF @tofirst IS NOT NULL
		SET @sql = @sql + N'
		AND cd.FirstOrderDateCreated <= @tofirst'

	IF @fromlast IS NOT NULL
		SET @sql = @sql + N'
		AND cd.LastOrderDateCreated >= @fromlast'

	IF @tolast IS NOT NULL
		SET @sql = @sql + N'
		AND cd.LastOrderDateCreated <= @tolast'

	IF @lastfromtotal IS NOT NULL
		SET @sql = @sql + N'
		AND cd.LastOrderTotal >= @lastfromtotal'

	IF @lasttototal IS NOT NULL
		SET @sql = @sql + N'
		AND cd.LastOrderTotal <= @lasttototal'

	IF @keycodefirst IS NOT NULL
		SET @sql = @sql + N'
		AND cd.FirstKeyCode = @keycodefirst'

	IF @discountcodefirst IS NOT NULL
		SET @sql = @sql + N'
		AND cd.FirstDiscountCode = @discountcodefirst'

	IF @dnm IS NOT NULL
		SET @sql = @sql + N'
		AND cmval.Value = '''+CASE @dnm WHEN 1 THEN N'True' ELSE N'False' END+''''

	IF @dnr IS NOT NULL
		SET @sql = @sql + N'
		AND crval.Value = '''+CASE @dnr WHEN 1 THEN N'True' ELSE N'False' END+''''

	IF @idcustomerordersource IS NOT NULL
		SET @sql = @sql + N'
		AND sval.Value = '''+CAST(@idcustomerordersource AS nvarchar(25))+''''
	
	SET @sql = @sql + N'
		ORDER BY c.Id'
	IF @pageindex IS NOT NULL
	BEGIN
		SET @sql = @sql + N'
			OFFSET '+CAST((@pageindex-1)*@pagesize AS nvarchar(25))+N' ROWS FETCH NEXT '+CAST(@pagesize AS nvarchar(25))+N' ROWS ONLY'
	END

	SET @sql = @sql + N'
		OPTION(RECOMPILE)'

	SET @params = N'@from datetime2,
		@to datetime2,
		@idcustomertype int,
		@fromordercount int,
		@toordercount int,
		@fromfirst datetime2,
		@tofirst datetime2,
		@fromlast datetime2,
		@tolast datetime2,
		@lastfromtotal decimal,
		@lasttototal decimal,
		@dnm bit,
		@dnr bit,
		@idcustomerordersource INT,
		@keycodefirst nvarchar(250),
		@discountcodefirst nvarchar(250),
		@pageindex int,
		@pagesize int,
		@count INT'

	EXEC sp_executesql @sql, @params, 
		@from, 
		@to,
		@idcustomertype,
		@fromordercount,
		@toordercount,
		@fromfirst,
		@tofirst,
		@fromlast,
		@tolast,
		@lastfromtotal,
		@lasttototal,
		@dnm,
		@dnr,
		@idcustomerordersource,
		@keycodefirst,
		@discountcodefirst,
		@pageindex,
		@pagesize,
		@count;
END

GO