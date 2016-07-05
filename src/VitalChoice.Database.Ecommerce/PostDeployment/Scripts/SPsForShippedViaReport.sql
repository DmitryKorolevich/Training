IF OBJECT_ID(N'[dbo].[SPGetShippedViaSummaryReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetShippedViaSummaryReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetShippedViaSummaryReport]
	@from datetime2,
	@to datetime2,
	@idstate int = NULL,
	@idservicecode int = NULL 
	-- -1 - None, 0 - All Codes, null - All Orders
AS
BEGIN

	SET NOCOUNT ON

	SELECT
		temp.IdWarehouse,
		temp.ShipMethodFreightCarrier,
		CASE WHEN temp.ShipMethodFreightService LIKE 'NEXT DAY%' 
		THEN 
			3
		ELSE 
			CASE WHEN temp.ShipMethodFreightService LIKE '2ND DAY%' THEN 2 ELSE 1 END
		END IdShipMethodFreightService,
		COUNT(*) Count
	FROM
	(
		SELECT 
			o.Id,
			osp.POrderType,
			MIN(osp.IdWarehouse) IdWarehouse,
			MIN(osp.ShipMethodFreightCarrier) ShipMethodFreightCarrier,
			MIN(osp.ShipMethodFreightService) ShipMethodFreightService
		FROM OrderShippingPackages osp WITH(NOLOCK)
		JOIN Orders o WITH(NOLOCK) ON osp.IdOrder = o.Id
		LEFT JOIN OrderOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'ServiceCode' AND sopt.IdObjectType = o.IdObjectType
		LEFT JOIN OrderOptionValues AS sval WITH(NOLOCK) ON sval.IdOrder = o.Id AND sval.IdOptionType = sopt.Id
		JOIN OrderAddresses a ON o.IdShippingAddress = a.Id
		WHERE 
			o.IdObjectType NOT IN (2,6) AND
			osp.ShippedDate>=@from AND osp.ShippedDate<=@to AND
			(@idstate IS NULL OR a.IdState = @idstate) AND		
			(@idservicecode IS NULL OR (-1 = @idservicecode AND sval.Value IS NULL) OR 
			(0 = @idservicecode AND sval.Value IS NOT NULL) OR sval.Value = @idservicecode)
		GROUP BY 
			o.Id, osp.POrderType
	) temp
	GROUP BY 
		temp.IdWarehouse, temp.ShipMethodFreightCarrier,
		CASE WHEN temp.ShipMethodFreightService LIKE 'NEXT DAY%' 
		THEN 
			3
		ELSE 
			CASE WHEN temp.ShipMethodFreightService LIKE '2ND DAY%' THEN 2 ELSE 1 END
		END

END

GO

IF OBJECT_ID(N'[dbo].[SPGetShippedViaItemsReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetShippedViaItemsReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetShippedViaItemsReport]
	@from datetime2,
	@to datetime2,
	@idstate int = NULL,
	@idservicecode int = NULL,
	@idwarehouse int = NULL,
	@carrier nvarchar(250) = NULL,
	@idshipservice int = NULL,
	@pageindex int = NULL,
	@pagesize int =NULL
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @count INT
	SET @count=0

	DECLARE @orders AS TABLE
    (
        Id int NOT NULL,
		IdObjectType int NOT NULL, 
		POrderType int NULL, 
		IdWarehouse int NULL,
		ShipMethodFreightCarrier nvarchar(250) NULL,
		IdShipMethodFreightService int NOT NULL,
		ShippedDate DATETIME2 NOT NULL,
		DateCreated DATETIME2 NOT NULL,
		IdState int NULL,
		ServiceCode int NULL,
        IdCustomer int NOT NULL, 
        Total MONEY NOT NULL
    );

	IF(@pageindex IS NOT NULL)
	BEGIN
		SET @count=(
			SELECT 
				COUNT(*)
			FROM
			(
				SELECT 
					o.Id,
					osp.POrderType
				FROM OrderShippingPackages osp WITH(NOLOCK)
				JOIN Orders o WITH(NOLOCK) ON osp.IdOrder = o.Id
				LEFT JOIN OrderOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'ServiceCode' AND sopt.IdObjectType = o.IdObjectType
				LEFT JOIN OrderOptionValues AS sval WITH(NOLOCK) ON sval.IdOrder = o.Id AND sval.IdOptionType = sopt.Id
				JOIN OrderAddresses a ON o.IdShippingAddress = a.Id
				WHERE 
					o.IdObjectType NOT IN (2,6) AND
					osp.ShippedDate>=@from AND osp.ShippedDate<=@to AND
					(@idstate IS NULL OR a.IdState = @idstate) AND		
					(@idservicecode IS NULL OR (-1 = @idservicecode AND sval.Value IS NULL) OR 
					(0 = @idservicecode AND sval.Value IS NOT NULL) OR sval.Value = @idservicecode) AND	
					(@idwarehouse IS NULL OR osp.IdWarehouse = @idwarehouse) AND	
					(@carrier IS NULL OR osp.ShipMethodFreightCarrier = @carrier) AND
					(@idshipservice IS NULL OR (3 = @idshipservice AND osp.ShipMethodFreightService LIKE 'NEXT DAY%') OR
					(2 = @idshipservice AND osp.ShipMethodFreightService LIKE '2ND DAY%') OR
					(1 = @idshipservice AND osp.ShipMethodFreightService NOT LIKE 'NEXT DAY%' AND osp.ShipMethodFreightService NOT LIKE '2ND DAY%'))
				GROUP BY 
					o.Id, osp.POrderType
			) temp
		)
	END

	INSERT INTO @orders
	(
		Id,
		POrderType, 
		IdObjectType,
		IdWarehouse,
		ShipMethodFreightCarrier,
		IdShipMethodFreightService,
		ShippedDate,
		DateCreated,
		IdState,
		ServiceCode,
		IdCustomer, 
		Total
	)
	SELECT
		temp.Id,
		temp.POrderType,
		temp.IdObjectType,
		temp.IdWarehouse,
		temp.ShipMethodFreightCarrier,
		CASE WHEN temp.ShipMethodFreightService LIKE 'NEXT DAY%' 
		THEN 
			3
		ELSE 
			CASE WHEN temp.ShipMethodFreightService LIKE '2ND DAY%' THEN 2 ELSE 1 END
		END IdShipMethodFreightService,
		temp.ShippedDate,
		temp.DateCreated,
		temp.IdState,
		temp.ServiceCode,
		temp.IdCustomer,
		temp.Total
	FROM
	(
		SELECT 
			ROW_NUMBER() OVER (ORDER BY o.Id) as RowNumber,
			o.Id,
			osp.POrderType,
			MIN(o.IdObjectType) IdObjectType,
			MIN(osp.IdWarehouse) IdWarehouse,
			MIN(osp.ShipMethodFreightCarrier) ShipMethodFreightCarrier,
			MIN(osp.ShipMethodFreightService) ShipMethodFreightService,
			MIN(osp.ShippedDate) ShippedDate,
			MIN(o.DateCreated) DateCreated,
			MIN(a.IdState) IdState,
			MIN(sval.Value) ServiceCode,
			MIN(o.IdCustomer) IdCustomer,
			MIN(o.Total) Total
		FROM OrderShippingPackages osp WITH(NOLOCK)
		JOIN Orders o WITH(NOLOCK) ON osp.IdOrder = o.Id
		LEFT JOIN OrderOptionTypes AS sopt WITH(NOLOCK) ON sopt.Name = N'ServiceCode' AND sopt.IdObjectType = o.IdObjectType
		LEFT JOIN OrderOptionValues AS sval WITH(NOLOCK) ON sval.IdOrder = o.Id AND sval.IdOptionType = sopt.Id
		JOIN OrderAddresses a ON o.IdShippingAddress = a.Id
		WHERE 
			o.IdObjectType NOT IN (2,6) AND			
			osp.ShippedDate>=@from AND osp.ShippedDate<=@to AND
			(@idstate IS NULL OR a.IdState = @idstate) AND		
			(@idservicecode IS NULL OR (-1 = @idservicecode AND sval.Value IS NULL) OR 
			(0 = @idservicecode AND sval.Value IS NOT NULL) OR sval.Value = @idservicecode) AND	
			(@idwarehouse IS NULL OR osp.IdWarehouse = @idwarehouse) AND	
			(@carrier IS NULL OR osp.ShipMethodFreightCarrier = @carrier) AND
			(@idshipservice IS NULL OR (3 = @idshipservice AND osp.ShipMethodFreightService LIKE 'NEXT DAY%') OR
			(2 = @idshipservice AND osp.ShipMethodFreightService LIKE '2ND DAY%') OR
			(1 = @idshipservice AND osp.ShipMethodFreightService NOT LIKE 'NEXT DAY%' AND osp.ShipMethodFreightService NOT LIKE '2ND DAY%'))
		GROUP BY 
			o.Id, osp.POrderType
	) temp		
	WHERE @pageindex is NULL OR (temp.RowNumber>(@pageindex-1)*@pagesize AND temp.RowNumber<=@pageindex*@pagesize) 
	ORDER BY temp.Id

	SELECT
		o.Id,
		o.IdObjectType,
		o.POrderType,
		o.IdWarehouse,
		o.ShipMethodFreightCarrier,
		o.IdShipMethodFreightService,
		o.ShippedDate,
		o.DateCreated,
		o.IdState,
		o.ServiceCode,
		o.IdCustomer,
		o.Total,
		cadfval.Value FirstName,
		cadlval.Value LastName,
		@count Count
	FROM @orders o
	JOIN Customers c ON o.IdCustomer=c.Id
	LEFT JOIN AddressOptionTypes AS cadfopt WITH(NOLOCK) ON cadfopt.Name = N'FirstName'
	LEFT JOIN AddressOptionValues AS cadfval WITH(NOLOCK) ON cadfval.IdAddress = c.IdProfileAddress AND cadfval.IdOptionType = cadfopt.Id
	LEFT JOIN AddressOptionTypes AS cadlopt WITH(NOLOCK) ON cadlopt.Name = N'LastName'
	LEFT JOIN AddressOptionValues AS cadlval WITH(NOLOCK) ON cadlval.IdAddress = c.IdProfileAddress AND cadlval.IdOptionType = cadlopt.Id

END

GO