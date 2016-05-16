IF OBJECT_ID(N'[dbo].[SPGetTransactionAndRefundReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetTransactionAndRefundReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetTransactionAndRefundReport]
	@from datetime2,
	@to datetime2,
	@idcustomertype int = NULL,
	@idservicecode int = NULL,
	@idcustomer int = NULL,
	@customerfirstname nvarchar(250) = NULL,
	@customerlastname nvarchar(250) = NULL,
	@idorder int = NULL,
	@idorderstatus int = NULL,
	@idordertype int = NULL,
	@pageindex int = NULL,
	@pagesize int =NULL
AS
BEGIN

	SET NOCOUNT ON
	
	DECLARE @count INT
	SET @count=0
	
	DECLARE @orders AS TABLE
    (
        IdOrder int NOT NULL, IdOrderSource int NULL, Rank int NOT NULL,
		IdObjectType int NOT NULL, OrderStatus int NULL, POrderStatus int NULL, NPOrderStatus int NULL, ServiceCode int NULL,
		IdCustomer int NOT NULL, CustomerIdObjectType int NOT NULL, CustomerFirstName NVARCHAR(250) NULL, CustomerLastName NVARCHAR(250) NULL,
		IdPaymentMethod int NOT NULL, IdDiscount int NULL, ProductsSubtotal MONEY NOT NULL, DiscountTotal MONEY NOT NULL,
		ShippingTotal MONEY NOT NULL, TaxTotal MONEY NOT NULL, Total MONEY NOT NULL,
		ReturnAssociated bit NULL, PaymentMethodIdObjectType int NOT NULL, DiscountIdObjectType int NULL, DiscountPercent MONEY NULL
    );

	IF(@customerfirstname IS NOT NULL)
	BEGIN 
		SET @customerfirstname='%'+@customerfirstname+'%'
	END
	IF(@customerlastname IS NOT NULL)
	BEGIN 
		SET @customerlastname='%'+@customerlastname+'%'
	END

	IF(@pageindex IS NOT NULL)
	BEGIN
		SET @count=(
			SELECT COUNT(*)		
			FROM				
				(SELECT o.Id, c.IdProfileAddress, o.IdObjectType
				FROM Orders o
				JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
				WHERE 
					o.DateCreated>=@from AND o.DateCreated<=@to AND o.StatusCode!=3 AND 
					((o.OrderStatus IS NOT NULL AND o.OrderStatus!=1) OR 
					(o.OrderStatus IS NULL AND o.POrderStatus!=1 AND o.NPOrderStatus!=1)) AND
					(@idorder IS NULL OR o.Id = @idorder) AND
					(@idcustomer IS NULL OR o.IdCustomer = @idcustomer) AND
					(@idorderstatus IS NULL OR o.OrderStatus = @idorderstatus OR o.POrderStatus = @idorderstatus OR o.NPOrderStatus = @idorderstatus) AND
					(@idordertype IS NULL OR o.IdObjectType = @idordertype) AND 
					(@idcustomertype IS NULL OR c.IdObjectType = @idcustomertype)
				) tempInner
				LEFT JOIN OrderOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'ServiceCode' AND sopt.IdObjectType = tempInner.IdObjectType
				LEFT JOIN OrderOptionValues AS sval WITH(NOLOCK) ON sval.IdOrder = tempInner.Id AND sval.IdOptionType = sopt.Id	
				LEFT JOIN AddressOptionTypes AS cadfopt WITH(NOLOCK) ON cadfopt.Name = N'FirstName'
				LEFT JOIN AddressOptionValues AS cadfval WITH(NOLOCK) ON cadfval.IdAddress = tempInner.IdProfileAddress AND cadfval.IdOptionType = cadfopt.Id
				LEFT JOIN AddressOptionTypes AS cadlopt WITH(NOLOCK) ON cadlopt.Name = N'LastName'
				LEFT JOIN AddressOptionValues AS cadlval WITH(NOLOCK) ON cadlval.IdAddress = tempInner.IdProfileAddress AND cadlval.IdOptionType = cadlopt.Id
				WHERE 
					(@idservicecode IS NULL OR sval.Value = @idservicecode) AND	
					(@customerfirstname IS NULL OR cadfval.Value LIKE @customerfirstname) AND
					(@customerlastname IS NULL OR cadlval.Value LIKE @customerlastname)
		)
	END

	INSERT INTO @orders
	(
		IdOrder , IdOrderSource, Rank, 
		IdObjectType, OrderStatus, POrderStatus, NPOrderStatus, ServiceCode,
		IdCustomer, CustomerIdObjectType, CustomerFirstName, CustomerLastName,
		IdPaymentMethod, IdDiscount, ProductsSubtotal, DiscountTotal, ShippingTotal, TaxTotal, Total,
		ReturnAssociated, PaymentMethodIdObjectType, DiscountIdObjectType, DiscountPercent
	)
	(SELECT 
		temp.IdOrder, temp.IdOrderSource, temp.Rank, 
		temp.IdObjectType, temp.OrderStatus, temp.POrderStatus, temp.NPOrderStatus, temp.ServiceCode,
		temp.IdCustomer, temp.CustomerIdObjectType, temp.CustomerFirstName, temp.CustomerLastName,
		temp.IdPaymentMethod, temp.IdDiscount, temp.ProductsSubtotal, temp.DiscountTotal, temp.ShippingTotal, temp.TaxTotal, temp.Total,
		rval.Value as ReturnAssociated, opm.IdObjectType as PaymentMethodIdObjectType, d.IdObjectType as DiscountIdObjectType, dpval.Value as DiscountPercent
		FROM
		(SELECT * FROM
			(SELECT tempInner.Id as IdOrder, tempInner.IdOrderSource, ROW_NUMBER() OVER (ORDER BY tempInner.Rank) RowNumber, tempInner.Rank, 
				tempInner.IdObjectType, tempInner.OrderStatus, tempInner.POrderStatus, tempInner.NPOrderStatus, sval.Value as ServiceCode,
				tempInner.IdCustomer, tempInner.CustomerIdObjectType as CustomerIdObjectType, cadfval.Value as CustomerFirstName, cadlval.Value as CustomerLastName,
				tempInner.IdPaymentMethod, tempInner.IdDiscount, tempInner.ProductsSubtotal, tempInner.DiscountTotal, tempInner.ShippingTotal, tempInner.TaxTotal, tempInner.Total
				FROM 
				(SELECT o.Id, o.IdOrderSource, DENSE_RANK() OVER (ORDER BY ISNULL(o.IdOrderSource, o.Id) DESC) AS Rank,
					o.IdObjectType, o.OrderStatus, o.POrderStatus, o.NPOrderStatus,
					o.IdCustomer, o.CustomerIdObjectType as CustomerIdObjectType, o.IdProfileAddress,
					o.IdPaymentMethod, o.IdDiscount, o.ProductsSubtotal, o.DiscountTotal, o.ShippingTotal, o.TaxTotal, o.Total
					FROM 
					(SELECT o.Id, o.IdOrderSource, o.IdObjectType, o.OrderStatus, o.POrderStatus, o.NPOrderStatus,
					o.IdCustomer, c.IdObjectType as CustomerIdObjectType, c.IdProfileAddress,
					o.IdPaymentMethod, o.IdDiscount, o.ProductsSubtotal, o.DiscountTotal, o.ShippingTotal, o.TaxTotal, o.Total
					FROM Orders o
					JOIN Customers c WITH(NOLOCK) ON o.IdCustomer=c.Id
					WHERE 
						o.DateCreated>=@from AND o.DateCreated<=@to AND o.StatusCode!=3 AND 
						((o.OrderStatus IS NOT NULL AND o.OrderStatus!=1) OR 
						(o.OrderStatus IS NULL AND o.POrderStatus!=1 AND o.NPOrderStatus!=1)) AND
						(@idorder IS NULL OR o.Id = @idorder) AND
						(@idcustomer IS NULL OR o.IdCustomer = @idcustomer) AND
						(@idorderstatus IS NULL OR o.OrderStatus = @idorderstatus OR o.POrderStatus = @idorderstatus OR o.NPOrderStatus = @idorderstatus) AND
						(@idordertype IS NULL OR o.IdObjectType = @idordertype) AND 
						(@idcustomertype IS NULL OR c.IdObjectType = @idcustomertype)
					) o
				) tempInner
				LEFT JOIN OrderOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'ServiceCode' AND sopt.IdObjectType = tempInner.IdObjectType
				LEFT JOIN OrderOptionValues AS sval WITH(NOLOCK) ON sval.IdOrder = tempInner.Id AND sval.IdOptionType = sopt.Id	
				LEFT JOIN AddressOptionTypes AS cadfopt WITH(NOLOCK) ON cadfopt.Name = N'FirstName'
				LEFT JOIN AddressOptionValues AS cadfval WITH(NOLOCK) ON cadfval.IdAddress = tempInner.IdProfileAddress AND cadfval.IdOptionType = cadfopt.Id
				LEFT JOIN AddressOptionTypes AS cadlopt WITH(NOLOCK) ON cadlopt.Name = N'LastName'
				LEFT JOIN AddressOptionValues AS cadlval WITH(NOLOCK) ON cadlval.IdAddress = tempInner.IdProfileAddress AND cadlval.IdOptionType = cadlopt.Id
				WHERE 
					(@idservicecode IS NULL OR sval.Value = @idservicecode) AND	
					(@customerfirstname IS NULL OR cadfval.Value LIKE @customerfirstname) AND
					(@customerlastname IS NULL OR cadlval.Value LIKE @customerlastname)
			) temp
		WHERE @pageindex is NULL OR (RowNumber>(@pageindex-1)*@pagesize AND RowNumber<=@pageindex*@pagesize)
		) temp
	LEFT JOIN OrderOptionTypes AS ropt WITH(NOLOCK) ON ropt.Name = N'ReturnAssociated' AND ropt.IdObjectType = temp.IdObjectType
	LEFT JOIN OrderOptionValues AS rval WITH(NOLOCK) ON rval.IdOrder = temp.IdOrder AND rval.IdOptionType = ropt.Id					
	JOIN OrderPaymentMethods opm WITH(NOLOCK) ON temp.IdPaymentMethod=opm.Id				
	LEFT JOIN Discounts d WITH(NOLOCK) ON temp.IdDiscount=d.Id
	LEFT JOIN DiscountOptionTypes AS dpopt WITH(NOLOCK) ON dpopt.Name = N'Percent' AND dpopt.IdObjectType = d.IdObjectType
	LEFT JOIN DiscountOptionValues AS dpval WITH(NOLOCK) ON dpval.IdDiscount = d.Id AND dpval.IdOptionType = dpopt.Id	
	)

	SELECT 
		ROW_NUMBER() OVER (ORDER BY temp.Rank) as RowNumber, @count as TotalCount,
		temp.IdOrder, temp.IdOrderSource, temp.Rank, 
		temp.IdObjectType, temp.OrderStatus, temp.POrderStatus, temp.NPOrderStatus, temp.ServiceCode,
		temp.IdCustomer, temp.CustomerIdObjectType, temp.CustomerFirstName, temp.CustomerLastName,
		temp.IdPaymentMethod, temp.IdDiscount, temp.ProductsSubtotal, temp.DiscountTotal, temp.ShippingTotal, temp.TaxTotal, temp.Total,
		temp.ReturnAssociated, temp.PaymentMethodIdObjectType, temp.DiscountIdObjectType, temp.DiscountPercent,
		temp.IdSku, temp.IdProduct, temp.Code, temp.ProductName, temp.ProductSubTitle, CAST(temp.SkuQTY as int) as SkuQTY, temp.OrderQuantity,
		temp.ProductIdObjectType, temp.Price, CAST(temp.RefundIdRedeemType as int) as RefundIdRedeemType, CAST(temp.RefundProductPercent as money) as RefundProductPercent
		FROM
		(SELECT 
			ots.IdOrder, ots.IdOrderSource, ots.Rank, 
			ots.IdObjectType, ots.OrderStatus, ots.POrderStatus, ots.NPOrderStatus, ots.ServiceCode,
			ots.IdCustomer, ots.CustomerIdObjectType, ots.CustomerFirstName, ots.CustomerLastName,
			ots.IdPaymentMethod, ots.IdDiscount, ots.ProductsSubtotal, ots.DiscountTotal, ots.ShippingTotal, ots.TaxTotal, ots.Total,
			ots.ReturnAssociated, ots.PaymentMethodIdObjectType, ots.DiscountIdObjectType, ots.DiscountPercent,
			os.IdSku, oss.IdProduct, oss.Code, osp.Name as ProductName, pSubTitleval.Value as ProductSubTitle, sQTYval.Value as SkuQTY, os.Quantity as OrderQuantity,
			osp.IdObjectType as ProductIdObjectType, os.Amount as Price, NULL as RefundIdRedeemType, NULL as RefundProductPercent
		FROM @orders ots
		JOIN OrderToSkus os WITH(NOLOCK) ON ots.IdOrder=os.IdOrder
		JOIN Skus oss WITH(NOLOCK) ON os.IdSku=oss.Id
		JOIN Products osp WITH(NOLOCK) ON oss.IdProduct= osp.Id
		LEFT JOIN ProductOptionTypes AS pSubTitleopt WITH(NOLOCK) ON pSubTitleopt.Name = N'SubTitle' AND pSubTitleopt.IdObjectType = osp.IdObjectType
		LEFT JOIN ProductOptionValues AS pSubTitleval WITH(NOLOCK) ON pSubTitleval.IdProduct = osp.Id AND pSubTitleval.IdOptionType = pSubTitleopt.Id		
		LEFT JOIN ProductOptionTypes AS sQTYopt WITH(NOLOCK) ON sQTYopt.Name = N'QTY' AND sQTYopt.IdObjectType = osp.IdObjectType
		LEFT JOIN SkuOptionValues AS sQTYval WITH(NOLOCK) ON sQTYval.IdSku = oss.Id AND sQTYval.IdOptionType = sQTYopt.Id
		UNION All
		SELECT 
			ots.IdOrder, ots.IdOrderSource, ots.Rank, 
			ots.IdObjectType, ots.OrderStatus, ots.POrderStatus, ots.NPOrderStatus, ots.ServiceCode,
			ots.IdCustomer, ots.CustomerIdObjectType, ots.CustomerFirstName, ots.CustomerLastName,
			ots.IdPaymentMethod, ots.IdDiscount, ots.ProductsSubtotal, ots.DiscountTotal, ots.ShippingTotal, ots.TaxTotal, ots.Total,
			ots.ReturnAssociated, ots.PaymentMethodIdObjectType, ots.DiscountIdObjectType, ots.DiscountPercent,
			op.IdSku, ops.IdProduct, ops.Code, opp.Name as ProductName, pSubTitleval.Value as ProductSubTitle, sQTYval.Value as SkuQTY, op.Quantity as OrderQuantity,
			opp.IdObjectType as ProductIdObjectType, op.Amount as Price, NULL as RefundIdRedeemType, NULL as RefundProductPercent
		FROM @orders ots
		JOIN OrderToPromos op WITH(NOLOCK) ON ots.IdOrder=op.IdOrder AND op.Disabled=0
		JOIN Skus ops WITH(NOLOCK) ON op.IdSku=ops.Id
		JOIN Products opp WITH(NOLOCK) ON ops.IdProduct= opp.Id
		LEFT JOIN ProductOptionTypes AS pSubTitleopt WITH(NOLOCK) ON pSubTitleopt.Name = N'SubTitle' AND pSubTitleopt.IdObjectType = opp.IdObjectType
		LEFT JOIN ProductOptionValues AS pSubTitleval WITH(NOLOCK) ON pSubTitleval.IdProduct = opp.Id AND pSubTitleval.IdOptionType = pSubTitleopt.Id		
		LEFT JOIN ProductOptionTypes AS sQTYopt WITH(NOLOCK) ON sQTYopt.Name = N'QTY' AND sQTYopt.IdObjectType = opp.IdObjectType
		LEFT JOIN SkuOptionValues AS sQTYval WITH(NOLOCK) ON sQTYval.IdSku = ops.Id AND sQTYval.IdOptionType = sQTYopt.Id
		UNION All
		SELECT 
			ots.IdOrder, ots.IdOrderSource, ots.Rank, 
			ots.IdObjectType, ots.OrderStatus, ots.POrderStatus, ots.NPOrderStatus, ots.ServiceCode,
			ots.IdCustomer, ots.CustomerIdObjectType, ots.CustomerFirstName, ots.CustomerLastName,
			ots.IdPaymentMethod, ots.IdDiscount, ots.ProductsSubtotal, ots.DiscountTotal, ots.ShippingTotal, ots.TaxTotal, ots.Total,
			ots.ReturnAssociated, ots.PaymentMethodIdObjectType, ots.DiscountIdObjectType, ots.DiscountPercent,
			orr.IdSku, ors.IdProduct, ors.Code, orp.Name as ProductName, pSubTitleval.Value as ProductSubTitle, sQTYval.Value as SkuQTY, orr.Quantity as OrderQuantity,
			orp.IdObjectType as ProductIdObjectType, orr.RefundPrice as Price, orr.Redeem as RefundIdRedeemType, orr.RefundPercent as RefundProductPercent
		FROM @orders ots
		JOIN RefundSkus orr WITH(NOLOCK) ON ots.IdOrder=orr.IdOrder
		JOIN Skus ors WITH(NOLOCK) ON orr.IdSku=ors.Id
		JOIN Products orp WITH(NOLOCK) ON ors.IdProduct= orp.Id
		LEFT JOIN ProductOptionTypes AS pSubTitleopt WITH(NOLOCK) ON pSubTitleopt.Name = N'SubTitle' AND pSubTitleopt.IdObjectType = orp.IdObjectType
		LEFT JOIN ProductOptionValues AS pSubTitleval WITH(NOLOCK) ON pSubTitleval.IdProduct = orp.Id AND pSubTitleval.IdOptionType = pSubTitleopt.Id		
		LEFT JOIN ProductOptionTypes AS sQTYopt WITH(NOLOCK) ON sQTYopt.Name = N'QTY' AND sQTYopt.IdObjectType = orp.IdObjectType
		LEFT JOIN SkuOptionValues AS sQTYval WITH(NOLOCK) ON sQTYval.IdSku = ors.Id AND sQTYval.IdOptionType = sQTYopt.Id
		) temp	
	ORDER BY temp.[Rank] ASC, temp.IdOrder ASC

END

GO