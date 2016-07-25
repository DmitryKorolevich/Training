IF OBJECT_ID(N'[dbo].[SPGetOrdersForOrdersSummarySalesReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetOrdersForOrdersSummarySalesReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetOrdersForOrdersSummarySalesReport]
	@from datetime2,
	@to datetime2,
	@shipfrom datetime2 = NULL,
	@shipto datetime2 = NULL, 
	@idcustomersource int = NULL,
	@customersourcedetails nvarchar(250) = NULL,
	@fromcount int = NULL,
	@tocount int = null,
	@keycode nvarchar(250) = NULL,
	@idcustomer int = NULL,
	@firstorderfrom datetime2 =NULL,
	@firstorderto datetime2 =NULL,
	@idcustomertype int = NULL,
	@discountcode nvarchar(250) = NULL,
	@isaffiliate bit =NULL,
	@pageindex int = NULL,
	@pagesize int =NULL
AS
BEGIN

	SET NOCOUNT ON

	CREATE TABLE #orders
    (
        Id INT NOT NULL PRIMARY KEY,
		IdObjectType INT NOT NULL, IdCustomer INT NOT NULL, CustomerIdObjectType INT NOT NULL, 
		IdDiscount INT NULL, CustomerDateCreated DATETIME2 NOT NULL, IdProfileAddress INT NOT NULL, OrderStatus INT NULL,
		POrderStatus INT NULL, NPOrderStatus INT NULL, DateCreated DATETIME2 NOT NULL, ProductsSubtotal MONEY NOT NULL, Total MONEY NOT NULL
    );

	INSERT INTO #orders
	(Id, IdObjectType, IdCustomer, CustomerIdObjectType,
	IdDiscount, CustomerDateCreated, IdProfileAddress, OrderStatus,
	POrderStatus, NPOrderStatus, DateCreated, ProductsSubtotal, Total)
	SELECT o.Id, o.IdObjectType, o.IdCustomer, c.IdObjectType As CustomerIdObjectType, o.IdDiscount, c.DateCreated AS CustomerDateCreated,
		c.IdProfileAddress, o.OrderStatus, o.POrderStatus, o.NPOrderStatus, o.DateCreated, o.ProductsSubtotal, o.Total
	FROM Orders o WITH(NOLOCK)
	JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
	WHERE
		o.DateCreated>=@from AND o.DateCreated<=@to AND
		o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
		((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4 AND o.OrderStatus !=6 ) OR 
		(o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND o.POrderStatus !=6 AND 
		o.NPOrderStatus !=1 AND o.NPOrderStatus !=4 AND o.NPOrderStatus !=6)) AND
		(@idcustomer IS NULL OR o.IdCustomer = @idcustomer) AND	
		(@idcustomertype IS NULL OR c.IdObjectType = @idcustomertype) AND
		(@shipfrom IS NULL OR EXISTS
		(SELECT
			TOP 1 s.IdOrder
		FROM OrderShippingPackages s
		WHERE s.IdOrder=o.Id AND s.ShippedDate>=@shipfrom
		)) AND
		(@shipto IS NULL OR EXISTS
		(SELECT
			TOP 1 s.IdOrder
		FROM OrderShippingPackages s
		WHERE s.IdOrder=o.Id AND s.ShippedDate<=@shipto
		))

		;WITH tempOrders(
			Id, IdObjectType, IdCustomer, CustomerIdObjectType, IdDiscount, CustomerDateCreated,
			OrderStatus, POrderStatus, NPOrderStatus, DateCreated, ProductsSubtotal, Total,
			CustomerFirstName, CustomerLastName, CustomerCompany,
			DiscountCode, IdAffiliate, KeyCode, [Source],
			SourceDetails, OrdersCount, FirstOrderDate
		) As
		(
			SELECT 
				temp.Id, temp.IdObjectType, temp.IdCustomer, temp.CustomerIdObjectType, temp.IdDiscount, temp.CustomerDateCreated,
				temp.OrderStatus, temp.POrderStatus, temp.NPOrderStatus, temp.DateCreated, temp.ProductsSubtotal, temp.Total,
				cadfval.Value as CustomerFirstName,	cadlval.Value as CustomerLastName,	cadcval.Value as CustomerCompany,
				d.Code as DiscountCode, aop.IdAffiliate, kcval.Value as KeyCode, CAST(sval.Value as int) as Source,
				sdval.Value as SourceDetails, occ.Count as OrdersCount, ISNULL(foc.DateCreated, temp.CustomerDateCreated) as FirstOrderDate
				FROM
				#orders as temp	
			LEFT JOIN OrderOptionTypes AS kcopt WITH(NOLOCK) ON kcopt.Name = N'KeyCode' AND kcopt.IdObjectType = temp.IdObjectType
			LEFT JOIN OrderOptionValues AS kcval WITH(NOLOCK) ON kcval.IdOrder = temp.Id AND kcval.IdOptionType = kcopt.Id
			LEFT JOIN CustomerOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'Source'
			LEFT JOIN CustomerOptionValues AS sval WITH(NOLOCK) ON sval.IdCustomer = temp.IdCustomer AND sval.IdOptionType = sopt.Id
			LEFT JOIN CustomerOptionTypes AS sdopt WITH(NOLOCK) ON sdopt.Name = N'SourceDetails'
			LEFT JOIN CustomerOptionValues AS sdval WITH(NOLOCK) ON sdval.IdCustomer = temp.IdCustomer AND sdval.IdOptionType = sdopt.Id
			LEFT JOIN VOrderCountOnCustomers As occ WITH(NOLOCK) ON occ.IdCustomer = temp.IdCustomer
			LEFT JOIN VFirstOrderOnCustomers As foc WITH(NOLOCK) ON foc.IdCustomer = temp.IdCustomer
			LEFT JOIN Discounts As d WITH(NOLOCK) ON d.Id = temp.IdDiscount
			LEFT JOIN AffiliateOrderPayments As aop WITH(NOLOCK) ON aop.Id = temp.Id
			INNER JOIN AddressOptionTypes AS cadfopt WITH(NOLOCK) ON cadfopt.Name = N'FirstName'
			INNER JOIN AddressOptionValues AS cadfval WITH(NOLOCK) ON cadfval.IdAddress = temp.IdProfileAddress AND cadfval.IdOptionType = cadfopt.Id
			INNER JOIN AddressOptionTypes AS cadlopt WITH(NOLOCK) ON cadlopt.Name = N'LastName'
			INNER JOIN AddressOptionValues AS cadlval WITH(NOLOCK) ON cadlval.IdAddress = temp.IdProfileAddress AND cadlval.IdOptionType = cadlopt.Id
			INNER JOIN AddressOptionTypes AS cadcopt WITH(NOLOCK) ON cadcopt.Name = N'Company'
			INNER JOIN AddressOptionValues AS cadcval WITH(NOLOCK) ON cadcval.IdAddress = temp.IdProfileAddress AND cadcval.IdOptionType = cadcopt.Id
			WHERE 
				(@keycode IS NULL OR kcval.Value = @keycode) AND 
				(@idcustomersource IS NULL OR sval.Value = @idcustomersource) AND
				(@customersourcedetails IS NULL OR sdval.Value = @customersourcedetails) AND 		
				(@fromcount IS NULL OR @fromcount<=occ.Count) AND		
				(@tocount IS NULL OR @tocount>=occ.Count) AND	
				(@firstorderfrom IS NULL OR (foc.DateCreated IS NOT NULL AND @firstorderfrom<=foc.DateCreated) 
				OR (foc.DateCreated IS NULL AND @firstorderfrom<=temp.CustomerDateCreated)) AND		
				(@firstorderto IS NULL OR (foc.DateCreated IS NOT NULL AND @firstorderto>=foc.DateCreated) 
				OR (foc.DateCreated IS NULL AND @firstorderto>=temp.CustomerDateCreated)) AND		
				(@discountcode IS NULL OR @discountcode=d.Code) AND
				(@isaffiliate IS NULL OR @isaffiliate=0 OR (@isaffiliate=1 AND aop.IdAffiliate IS NOT NULL))
		)

		SELECT 
			* FROM
			(SELECT *, ROW_NUMBER() OVER (ORDER BY Id DESC) AS RowNumber FROM tempOrders) temp
		CROSS JOIN (SELECT Count(*) AS TotalCount FROM tempOrders) AS tCountOrders
		WHERE @pageindex is NULL OR (RowNumber>(@pageindex-1)*@pagesize AND RowNumber<=@pageindex*@pagesize)
		ORDER BY Id DESC

END

GO

IF OBJECT_ID(N'[dbo].[SPGetOrderStatisticByTypeForOrdersSummarySalesReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetOrderStatisticByTypeForOrdersSummarySalesReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetOrderStatisticByTypeForOrdersSummarySalesReport]
	@from datetime2,
	@to datetime2,
	@shipfrom datetime2 = NULL,
	@shipto datetime2 = NULL, 
	@idcustomersource int = NULL,
	@customersourcedetails nvarchar(250) = NULL,
	@fromcount int = NULL,
	@tocount int = null,
	@keycode nvarchar(250) = NULL,
	@idcustomer int = NULL,
	@firstorderfrom datetime2 =NULL,
	@firstorderto datetime2 =NULL,
	@idcustomertype int = NULL,
	@discountcode nvarchar(250) = NULL,
	@isaffiliate bit =NULL
AS
BEGIN

	SET NOCOUNT ON


		;WITH tempOrders(
			OrderType, Id, Total
		) As
		(
			SELECT 
				otval.Value as OrderType , temp.Id, temp.Total
				FROM
				(SELECT 
					o.Id, o.IdObjectType, o.IdCustomer, c.IdObjectType As CustomerIdObjectType, o.IdDiscount, c.DateCreated AS CustomerDateCreated,
					o.Total
				FROM Orders o WITH(NOLOCK)
				JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
				WHERE
					o.DateCreated>=@from AND o.DateCreated<=@to AND
					o.StatusCode!=3 AND o.IdObjectType NOT IN (2,5,6) AND 
					((o.OrderStatus IS NOT NULL AND o.OrderStatus !=1 AND o.OrderStatus !=4 AND o.OrderStatus !=6 ) OR 
					(o.OrderStatus IS NULL AND o.POrderStatus !=1 AND o.POrderStatus !=4 AND o.POrderStatus !=6 AND 
					o.NPOrderStatus !=1 AND o.NPOrderStatus !=4 AND o.NPOrderStatus !=6)) AND
					(@idcustomer IS NULL OR o.IdCustomer = @idcustomer) AND	
					(@idcustomertype IS NULL OR c.IdObjectType = @idcustomertype) AND
					(@shipfrom IS NULL OR EXISTS
					(SELECT
						TOP 1 s.IdOrder
					FROM OrderShippingPackages s
					WHERE s.IdOrder=o.Id AND s.ShippedDate>=@shipfrom
					 )) AND
					(@shipto IS NULL OR EXISTS
					(SELECT
						TOP 1 s.IdOrder
					FROM OrderShippingPackages s
					WHERE s.IdOrder=o.Id AND s.ShippedDate<=@shipto
					 ))
				) temp	
			LEFT JOIN OrderOptionTypes AS kcopt WITH(NOLOCK) ON kcopt.Name = N'KeyCode' AND kcopt.IdObjectType = temp.IdObjectType
			LEFT JOIN OrderOptionValues AS kcval WITH(NOLOCK) ON kcval.IdOrder = temp.Id AND kcval.IdOptionType = kcopt.Id
			JOIN OrderOptionTypes AS otopt WITH(NOLOCK) ON otopt.Name = N'OrderType'
			JOIN OrderOptionValues AS otval WITH(NOLOCK) ON otval.IdOrder = temp.Id AND otval.IdOptionType = otopt.Id
			LEFT JOIN CustomerOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'Source'
			LEFT JOIN CustomerOptionValues AS sval WITH(NOLOCK) ON sval.IdCustomer = temp.IdCustomer AND sval.IdOptionType = sopt.Id
			LEFT JOIN CustomerOptionTypes AS sdopt WITH(NOLOCK) ON sdopt.Name = N'SourceDetails'
			LEFT JOIN CustomerOptionValues AS sdval WITH(NOLOCK) ON sdval.IdCustomer = temp.IdCustomer AND sdval.IdOptionType = sdopt.Id
			LEFT JOIN VOrderCountOnCustomers As occ WITH(NOLOCK) ON occ.IdCustomer = temp.IdCustomer
			LEFT JOIN VFirstOrderOnCustomers As foc WITH(NOLOCK) ON foc.IdCustomer = temp.IdCustomer
			LEFT JOIN Discounts As d WITH(NOLOCK) ON d.Id = temp.IdDiscount
			LEFT JOIN AffiliateOrderPayments As aop WITH(NOLOCK) ON aop.Id = temp.Id
			WHERE 
				(@keycode IS NULL OR kcval.Value = @keycode) AND 
				(@idcustomersource IS NULL OR sval.Value = @idcustomersource) AND
				(@customersourcedetails IS NULL OR sdval.Value = @customersourcedetails) AND 		
				(@fromcount IS NULL OR @fromcount<=occ.Count) AND		
				(@tocount IS NULL OR @tocount>=occ.Count) AND	
				(@firstorderfrom IS NULL OR (foc.DateCreated IS NOT NULL AND @firstorderfrom<=foc.DateCreated) 
				OR (foc.DateCreated IS NULL AND @firstorderfrom<=temp.CustomerDateCreated)) AND		
				(@firstorderto IS NULL OR (foc.DateCreated IS NOT NULL AND @firstorderto>=foc.DateCreated) 
				OR (foc.DateCreated IS NULL AND @firstorderto>=temp.CustomerDateCreated)) AND		
				(@discountcode IS NULL OR @discountcode=d.Code) AND
				(@isaffiliate IS NULL OR @isaffiliate=0 OR (@isaffiliate=1 AND aop.IdAffiliate IS NOT NULL))
		)

		SELECT CAST(OrderType as int) as Id, COUNT(*) Count, SUM(Total) Total FROM tempOrders
		GROUP BY OrderType

END

GO