/****** Object:  StoredProcedure [dbo].[SPGetOrderIdsForWholesaleDropShipReport]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SPGetOrderIdsForWholesaleDropShipReport]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SPGetOrderIdsForWholesaleDropShipReport] AS' 
END
GO
ALTER PROCEDURE [dbo].[SPGetOrderIdsForWholesaleDropShipReport]
	@from datetime2,
	@to datetime2,
	@shipfrom datetime2 = NULL,
	@shipto datetime2 = NULL, 
	@idcustomertype int = NULL,
	@idtradeclass int = NULL,
	@customerfirstname nvarchar(250) = NULL,
	@customerlastname nvarchar(250) = NULL,
	@shipfirstname nvarchar(250) = NULL,
	@shiplastname nvarchar(250) = NULL,
	@shipidconfirm nvarchar(250) = NULL,
	@idorder int = NULL,
	@ponumber nvarchar(250) = NULL,
	@pageindex int = NULL,
	@pagesize int =NULL,
	@getcount bit=NULL
AS
BEGIN

	SET NOCOUNT ON

	IF(@customerfirstname IS NOT NULL)
	BEGIN 
		SET @customerfirstname='%'+@customerfirstname+'%'
	END
	IF(@customerlastname IS NOT NULL)
	BEGIN 
		SET @customerlastname='%'+@customerlastname+'%'
	END
	IF(@shipfirstname IS NOT NULL)
	BEGIN 
		SET @shipfirstname='%'+@shipfirstname+'%'
	END
	IF(@shiplastname IS NOT NULL)
	BEGIN 
		SET @shiplastname='%'+@shiplastname+'%'
	END

	IF(@getcount=1)
	BEGIN
		;WITH orderids(Id) As
		(
			SELECT temp.IdOrder FROM
				(SELECT o.Id As IdOrder, o.IdShippingAddress, o.IdObjectType, o.IdCustomer, c.IdObjectType As CustomerIdObjectType, c.IdProfileAddress 
				FROM Orders o WITH(NOLOCK)
				JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
				WHERE
					o.DateCreated>=@from AND o.DateCreated<=@to AND
					o.StatusCode!=3 AND o.IdObjectType!=5 AND o.IdObjectType!=6 AND o.IdObjectType!=2 AND
					((o.OrderStatus IS NOT NULL AND o.OrderStatus!=1 AND o.OrderStatus!=4) OR 
					(o.OrderStatus IS NULL AND o.POrderStatus!=1 AND o.POrderStatus!=4 AND
					o.NPOrderStatus!=1 AND o.NPOrderStatus!=4)) AND
					(@idorder IS NULL OR o.Id = @idorder) AND	
					(@idcustomertype IS NULL OR c.IdObjectType = @idcustomertype)) temp	
			LEFT JOIN OrderOptionTypes AS pnopt WITH(NOLOCK) ON pnopt.Name = N'PoNumber' AND pnopt.IdObjectType = temp.IdObjectType
			LEFT JOIN OrderOptionValues AS pnval WITH(NOLOCK) ON pnval.IdOrder = temp.IdOrder AND pnval.IdOptionType = pnopt.Id
			LEFT JOIN CustomerOptionTypes AS tcopt WITH(NOLOCK) ON tcopt.Name = N'TradeClass' AND tcopt.IdObjectType = temp.CustomerIdObjectType
			LEFT JOIN CustomerOptionValues AS tcval WITH(NOLOCK) ON tcval.IdCustomer = temp.IdCustomer AND tcval.IdOptionType = tcopt.Id
			LEFT JOIN AddressOptionTypes AS cadfopt WITH(NOLOCK) ON cadfopt.Name = N'FirstName'
			LEFT JOIN AddressOptionValues AS cadfval WITH(NOLOCK) ON cadfval.IdAddress = temp.IdProfileAddress AND cadfval.IdOptionType = cadfopt.Id
			LEFT JOIN AddressOptionTypes AS cadlopt WITH(NOLOCK) ON cadlopt.Name = N'LastName'
			LEFT JOIN AddressOptionValues AS cadlval WITH(NOLOCK) ON cadlval.IdAddress = temp.IdProfileAddress AND cadlval.IdOptionType = cadlopt.Id
			LEFT JOIN AddressOptionTypes AS sadfopt WITH(NOLOCK) ON sadfopt.Name = N'FirstName'
			LEFT JOIN OrderAddressOptionValues AS sadfval WITH(NOLOCK) ON sadfval.IdOrderAddress = temp.IdShippingAddress AND sadfval.IdOptionType = sadfopt.Id
			LEFT JOIN AddressOptionTypes AS sadlopt WITH(NOLOCK) ON sadlopt.Name = N'LastName'
			LEFT JOIN OrderAddressOptionValues AS sadlval WITH(NOLOCK) ON sadlval.IdOrderAddress = temp.IdShippingAddress AND sadlval.IdOptionType = sadlopt.Id
			WHERE 
				(@ponumber IS NULL OR pnval.Value = @ponumber) AND 
				(@idtradeclass IS NULL OR tcval.Value = @idtradeclass) AND			
				(@customerfirstname IS NULL OR cadfval.Value LIKE @customerfirstname) AND
				(@customerlastname IS NULL OR cadlval.Value LIKE @customerlastname) AND
				(@shipfirstname IS NULL OR sadfval.Value LIKE @shipfirstname) AND
				(@shiplastname IS NULL OR sadlval.Value LIKE @shiplastname)
		)		

		SELECT 1 as Id, COUNT(*) as Count FROM orderids	
	END
	ELSE
	BEGIN
		;WITH orderids(Id) As
		(
			SELECT temp.IdOrder FROM
				(SELECT o.Id As IdOrder, o.IdShippingAddress, o.IdObjectType, o.IdCustomer, c.IdObjectType As CustomerIdObjectType, c.IdProfileAddress 
				FROM Orders o WITH(NOLOCK)
				JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
				WHERE
					o.DateCreated>=@from AND o.DateCreated<=@to AND
					o.StatusCode!=3 AND o.IdObjectType!=5 AND o.IdObjectType!=6 AND o.IdObjectType!=2 AND 
					((o.OrderStatus IS NOT NULL AND o.OrderStatus!=1 AND o.OrderStatus!=4) OR 
					(o.OrderStatus IS NULL AND o.POrderStatus!=1 AND o.POrderStatus!=4 AND
					o.NPOrderStatus!=1 AND o.NPOrderStatus!=4)) AND
					(@idorder IS NULL OR o.Id = @idorder) AND	
					(@idcustomertype IS NULL OR c.IdObjectType = @idcustomertype)) temp	
			LEFT JOIN OrderOptionTypes AS pnopt WITH(NOLOCK) ON pnopt.Name = N'PoNumber' AND pnopt.IdObjectType = temp.IdObjectType
			LEFT JOIN OrderOptionValues AS pnval WITH(NOLOCK) ON pnval.IdOrder = temp.IdOrder AND pnval.IdOptionType = pnopt.Id
			LEFT JOIN CustomerOptionTypes AS tcopt WITH(NOLOCK) ON tcopt.Name = N'TradeClass' AND tcopt.IdObjectType = temp.CustomerIdObjectType
			LEFT JOIN CustomerOptionValues AS tcval WITH(NOLOCK) ON tcval.IdCustomer = temp.IdCustomer AND tcval.IdOptionType = tcopt.Id
			LEFT JOIN AddressOptionTypes AS cadfopt WITH(NOLOCK) ON cadfopt.Name = N'FirstName'
			LEFT JOIN AddressOptionValues AS cadfval WITH(NOLOCK) ON cadfval.IdAddress = temp.IdProfileAddress AND cadfval.IdOptionType = cadfopt.Id
			LEFT JOIN AddressOptionTypes AS cadlopt WITH(NOLOCK) ON cadlopt.Name = N'LastName'
			LEFT JOIN AddressOptionValues AS cadlval WITH(NOLOCK) ON cadlval.IdAddress = temp.IdProfileAddress AND cadlval.IdOptionType = cadlopt.Id
			LEFT JOIN AddressOptionTypes AS sadfopt WITH(NOLOCK) ON sadfopt.Name = N'FirstName'
			LEFT JOIN OrderAddressOptionValues AS sadfval WITH(NOLOCK) ON sadfval.IdOrderAddress = temp.IdShippingAddress AND sadfval.IdOptionType = sadfopt.Id
			LEFT JOIN AddressOptionTypes AS sadlopt WITH(NOLOCK) ON sadlopt.Name = N'LastName'
			LEFT JOIN OrderAddressOptionValues AS sadlval WITH(NOLOCK) ON sadlval.IdOrderAddress = temp.IdShippingAddress AND sadlval.IdOptionType = sadlopt.Id
			WHERE 
				(@ponumber IS NULL OR pnval.Value = @ponumber) AND 
				(@idtradeclass IS NULL OR tcval.Value = @idtradeclass) AND			
				(@customerfirstname IS NULL OR cadfval.Value LIKE @customerfirstname) AND
				(@customerlastname IS NULL OR cadlval.Value LIKE @customerlastname) AND
				(@shipfirstname IS NULL OR sadfval.Value LIKE @shipfirstname) AND
				(@shiplastname IS NULL OR sadlval.Value LIKE @shiplastname)
		)

		SELECT Id FROM
			(SELECT Id, ROW_NUMBER() OVER (ORDER BY Id DESC) AS RowNumber FROM orderids) temp
		WHERE @pageindex is NULL OR (RowNumber>(@pageindex-1)*@pagesize AND RowNumber<=@pageindex*@pagesize)
		ORDER BY Id DESC
	END	 

END


GO
