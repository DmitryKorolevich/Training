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

	DECLARE @customers AS TABLE
    (
		Id INT,
		Email NVARCHAR(250),
		CustomerIdObjectType INT,
		FirstOrderDateCreated DATETIME2 NULL,
		FirstKeyCode NVARCHAR(250) NULL,
		FirstDiscountCode NVARCHAR(250) NULL,
		LastOrderDateCreated DATETIME2 NULL,
		LastOrderTotal MONEY NULL,
		LastOrderIdPaymentMethod INT NULL,
		OrdersCount INT,
		OrdersTotal MONEY,
		DoNotMail BIT,
		DoNotRent BIT,
		IdCustomerOrderSource INT NULL
    );
	
	IF(@pageindex IS NOT NULL)
	BEGIN
		SET @count =
			(SELECT 
				COUNT(*)
			FROM
				(
				SELECT 
					c.Id,
					MIN(c.Email) Email,
					MIN(c.IdObjectType) CustomerIdObjectType,
					(SELECT TOP 1
								oin.DateCreated
							FROM Orders oin WITH(NOLOCK)
							WHERE 
								oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
								(
									(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
									OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
								)
							ORDER BY oin.DateCreated ASC
					) FirstOrderDateCreated,
					(SELECT TOP 1
								kcval.Value
							FROM Orders oin WITH(NOLOCK)
							JOIN OrderOptionTypes AS kcopt WITH(NOLOCK) ON kcopt.Name = N'KeyCode' AND kcopt.IdObjectType = oin.IdObjectType
							JOIN OrderOptionValues AS kcval WITH(NOLOCK) ON kcval.IdOrder = oin.Id AND kcval.IdOptionType = kcopt.Id
							WHERE 
								oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
								(
									(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
									OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
								)
							ORDER BY oin.DateCreated ASC
					) FirstKeyCode,
					(SELECT TOP 1
								d.Code
							FROM Orders oin WITH(NOLOCK)		
							JOIN Discounts d WITH(NOLOCK) ON oin.IdDiscount=d.Id
							WHERE 
								oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
								(
									(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
									OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
								)
							ORDER BY oin.DateCreated ASC
					) FirstDiscountCode,
					(SELECT TOP 1
								oin.DateCreated
							FROM Orders oin WITH(NOLOCK)
							WHERE 
								oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
								(
									(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
									OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
								)
							ORDER BY oin.DateCreated DESC
					) LastOrderDateCreated,
					(SELECT TOP 1
								oin.Total
							FROM Orders oin WITH(NOLOCK)
							WHERE 
								oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
								(
									(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
									OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
								)
							ORDER BY oin.DateCreated DESC
					) LastOrderTotal,
					(SELECT COUNT(*)
							FROM Orders oin WITH(NOLOCK)
							WHERE 
								oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
								(
									(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
									OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
								)
					) OrdersCount,
					MIN(cmval.Value) DoNotMail,
					MIN(crval.Value) DoNotRent,
					MIN(sval.Value) IdCustomerOrderSource
				FROM Orders o WITH(NOLOCK)
				JOIN Customers c WITH(NOLOCK) ON o.IdCustomer = c.Id
				LEFT JOIN CustomerOptionTypes AS cmopt WITH(NOLOCK) ON cmopt.Name = N'DoNotMail'
				LEFT JOIN CustomerOptionValues AS cmval WITH(NOLOCK) ON cmval.IdCustomer = c.Id AND cmval.IdOptionType = cmopt.Id
				LEFT JOIN CustomerOptionTypes AS cropt WITH(NOLOCK) ON cropt.Name = N'DoNotRent'
				LEFT JOIN CustomerOptionValues AS crval WITH(NOLOCK) ON crval.IdCustomer = c.Id AND crval.IdOptionType = cropt.Id
				LEFT JOIN CustomerOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'Source'
				LEFT JOIN CustomerOptionValues AS sval WITH(NOLOCK) ON sval.IdCustomer = c.Id AND sval.IdOptionType = sopt.Id
				WHERE
					o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
					(
						(o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) 
						OR (o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND o.NPOrderStatus !=1 AND o.NPOrderStatus !=4)
					) AND
					(@from IS NULL OR o.DateCreated >= @from) AND 
					(@to IS NULL OR o.DateCreated <= @to)
				GROUP BY c.Id
				) temp
			WHERE 
				(@idcustomertype IS NULL OR temp.CustomerIdObjectType = @idcustomertype) AND
				(@fromordercount IS NULL OR temp.OrdersCount >= @fromordercount) AND
				(@toordercount IS NULL OR temp.OrdersCount <= @toordercount) AND
				(@fromfirst IS NULL OR temp.FirstOrderDateCreated >= @fromfirst) AND
				(@tofirst IS NULL OR temp.FirstOrderDateCreated <= @tofirst) AND
				(@fromlast IS NULL OR temp.LastOrderDateCreated >= @fromlast) AND
				(@tolast IS NULL OR temp.LastOrderDateCreated <= @tolast) AND
				(@lastfromtotal IS NULL OR temp.LastOrderTotal >= @lastfromtotal) AND
				(@lasttototal IS NULL OR temp.LastOrderTotal <= @lasttototal) AND
				(@dnm IS NULL OR temp.DoNotMail = @dnm) AND
				(@dnr IS NULL OR temp.DoNotRent = @dnr) AND
				(@idcustomerordersource IS NULL OR temp.IdCustomerOrderSource = @idcustomerordersource) AND
				(@keycodefirst IS NULL OR temp.FirstKeyCode = @keycodefirst) AND
				(@discountcodefirst IS NULL OR temp.FirstDiscountCode = @discountcodefirst)				
			)
	END

	INSERT 
	INTO @customers
		(
		Id,
		Email,
		CustomerIdObjectType,
		FirstOrderDateCreated,
		FirstKeyCode,
		FirstDiscountCode,
		LastOrderDateCreated,
		LastOrderTotal,
		LastOrderIdPaymentMethod,
		OrdersCount,
		OrdersTotal,
		DoNotMail,
		DoNotRent,
		IdCustomerOrderSource
		)
	(
	SELECT
		tempOuter.Id,
		tempOuter.Email,
		tempOuter.CustomerIdObjectType,
		tempOuter.FirstOrderDateCreated,
		tempOuter.FirstKeyCode,
		tempOuter.FirstDiscountCode,
		tempOuter.LastOrderDateCreated,
		tempOuter.LastOrderTotal,
		tempOuter.LastOrderIdPaymentMethod,
		tempOuter.OrdersCount,
		tempOuter.OrdersTotal,
		CASE WHEN tempOuter.DoNotMail='True' THEN 1 ELSE 0 END,
		CASE WHEN tempOuter.DoNotRent='True' THEN 1 ELSE 0 END,
		tempOuter.IdCustomerOrderSource
	FROM
		(
		SELECT 
			ROW_NUMBER() OVER (ORDER BY temp.Id) as RowNumber,
			temp.Id,
			temp.Email,
			temp.CustomerIdObjectType,
			temp.FirstOrderDateCreated,
			temp.FirstKeyCode,
			temp.FirstDiscountCode,
			temp.LastOrderDateCreated,
			temp.LastOrderTotal,
			temp.LastOrderIdPaymentMethod,
			temp.OrdersCount,
			temp.OrdersTotal,
			temp.DoNotMail,
			temp.DoNotRent,
			temp.IdCustomerOrderSource
		FROM
			(
			SELECT 
				c.Id,
				MIN(c.Email) Email,
				MIN(c.IdObjectType) CustomerIdObjectType,
				(SELECT TOP 1
							oin.DateCreated
						FROM Orders oin WITH(NOLOCK)
						WHERE 
							oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
							(
								(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
								OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
							)
						ORDER BY oin.DateCreated ASC
				) FirstOrderDateCreated,
				(SELECT TOP 1
							kcval.Value
						FROM Orders oin WITH(NOLOCK)
						JOIN OrderOptionTypes AS kcopt WITH(NOLOCK) ON kcopt.Name = N'KeyCode' AND kcopt.IdObjectType = oin.IdObjectType
						JOIN OrderOptionValues AS kcval WITH(NOLOCK) ON kcval.IdOrder = oin.Id AND kcval.IdOptionType = kcopt.Id
						WHERE 
							oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
							(
								(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
								OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
							)
						ORDER BY oin.DateCreated ASC
				) FirstKeyCode,
				(SELECT TOP 1
							d.Code
						FROM Orders oin WITH(NOLOCK)		
						JOIN Discounts d WITH(NOLOCK) ON oin.IdDiscount=d.Id
						WHERE 
							oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
							(
								(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
								OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
							)
						ORDER BY oin.DateCreated ASC
				) FirstDiscountCode,
				(SELECT TOP 1
							oin.DateCreated
						FROM Orders oin WITH(NOLOCK)
						WHERE 
							oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
							(
								(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
								OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
							)
						ORDER BY oin.DateCreated DESC
				) LastOrderDateCreated,
				(SELECT TOP 1
							oin.Total
						FROM Orders oin WITH(NOLOCK)
						WHERE 
							oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
							(
								(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
								OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
							)
						ORDER BY oin.DateCreated DESC
				) LastOrderTotal,
				(SELECT TOP 1
							oin.IdPaymentMethod
						FROM Orders oin WITH(NOLOCK)
						WHERE 
							oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
							(
								(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
								OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
							)
						ORDER BY oin.DateCreated DESC
				) LastOrderIdPaymentMethod,
				(SELECT COUNT(*)
						FROM Orders oin WITH(NOLOCK)
						WHERE 
							oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
							(
								(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
								OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
							)
				) OrdersCount,
				(SELECT SUM(oin.Total)
						FROM Orders oin WITH(NOLOCK)
						WHERE 
							oin.IdCustomer=c.Id AND oin.StatusCode!=3 AND oin.IdObjectType NOT IN (2,5,6) AND 
							(
								(oin.OrderStatus IS NOT NULL AND oin.OrderStatus !=1 AND oin.OrderStatus !=4) 
								OR (oin.OrderStatus IS NULL AND oin.POrderStatus !=1 AND oin.POrderStatus !=4 AND oin.NPOrderStatus !=1 AND oin.NPOrderStatus !=4)
							)
				) OrdersTotal,
				MIN(cmval.Value) DoNotMail,
				MIN(crval.Value) DoNotRent,
				MIN(sval.Value) IdCustomerOrderSource
			FROM Orders o WITH(NOLOCK)
			JOIN Customers c WITH(NOLOCK) ON o.IdCustomer = c.Id
			LEFT JOIN CustomerOptionTypes AS cmopt WITH(NOLOCK) ON cmopt.Name = N'DoNotMail'
			LEFT JOIN CustomerOptionValues AS cmval WITH(NOLOCK) ON cmval.IdCustomer = c.Id AND cmval.IdOptionType = cmopt.Id
			LEFT JOIN CustomerOptionTypes AS cropt WITH(NOLOCK) ON cropt.Name = N'DoNotRent'
			LEFT JOIN CustomerOptionValues AS crval WITH(NOLOCK) ON crval.IdCustomer = c.Id AND crval.IdOptionType = cropt.Id
			LEFT JOIN CustomerOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'Source'
			LEFT JOIN CustomerOptionValues AS sval WITH(NOLOCK) ON sval.IdCustomer = c.Id AND sval.IdOptionType = sopt.Id
			WHERE
				o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
				(
					(o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4) 
					OR (o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND o.NPOrderStatus !=1 AND o.NPOrderStatus !=4)
				) AND
				(@from IS NULL OR o.DateCreated >= @from) AND 
				(@to IS NULL OR o.DateCreated <= @to)
			GROUP BY c.Id
			) temp
		WHERE 
			(@idcustomertype IS NULL OR temp.CustomerIdObjectType = @idcustomertype) AND
			(@fromordercount IS NULL OR temp.OrdersCount >= @fromordercount) AND
			(@toordercount IS NULL OR temp.OrdersCount <= @toordercount) AND
			(@fromfirst IS NULL OR temp.FirstOrderDateCreated >= @fromfirst) AND
			(@tofirst IS NULL OR temp.FirstOrderDateCreated <= @tofirst) AND
			(@fromlast IS NULL OR temp.LastOrderDateCreated >= @fromlast) AND
			(@tolast IS NULL OR temp.LastOrderDateCreated <= @tolast) AND
			(@lastfromtotal IS NULL OR temp.LastOrderTotal >= @lastfromtotal) AND
			(@lasttototal IS NULL OR temp.LastOrderTotal <= @lasttototal) AND
			(@dnm IS NULL OR temp.DoNotMail = @dnm) AND
			(@dnr IS NULL OR temp.DoNotRent = @dnr) AND
			(@idcustomerordersource IS NULL OR temp.IdCustomerOrderSource = @idcustomerordersource) AND
			(@keycodefirst IS NULL OR temp.FirstKeyCode = @keycodefirst) AND
			(@discountcodefirst IS NULL OR temp.FirstDiscountCode = @discountcodefirst)
		) tempOuter
		WHERE 
			@pageindex is NULL OR (tempOuter.RowNumber>(@pageindex-1)*@pagesize AND tempOuter.RowNumber<=@pageindex*@pagesize)
	)

	SELECT 
		c.Id,
		c.Email,
		c.CustomerIdObjectType,
		c.FirstOrderDateCreated,
		c.FirstKeyCode,
		c.FirstDiscountCode,
		c.LastOrderDateCreated,
		c.LastOrderTotal,
		c.LastOrderIdPaymentMethod,
		c.OrdersCount,
		c.OrdersTotal,
		c.DoNotMail,
		c.DoNotRent,
		c.IdCustomerOrderSource,
		options.FirstName,
		options.LastName,
		options.Address1,
		options.Address2,
		options.City,
		options.Phone,
		options.Zip,
		a.IdCountry,
		a.IdState,
		@count Count
	FROM @customers c
	LEFT JOIN OrderPaymentMethods pm ON c.LastOrderIdPaymentMethod=pm.Id
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

END

GO