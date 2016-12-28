IF OBJECT_ID(N'[dbo].[SPGetOrdersAbuseReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetOrdersAbuseReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetOrdersAbuseReport]
	@from datetime2,
	@to datetime2,
	@reships int = NULL,
	@refunds int =NULL,
	@reshipsorrefunds int NULL,
	@idservicecode int = NULL,
	@idcustomer int = NULL
AS
BEGIN

	SET NOCOUNT ON

	SELECT
		temp.Id,
		temp.IdObjectType,
		temp.IdCustomer,
		cadfval.Value CustomerFirstName,
		cadlval.Value CustomerLastName,
		temp.DateCreated,
		temp.Total,
		temp.IdServiceCode,
		scnval.Value ServiceCodeNotes,
		temp.IdOrderSource,
		os.DateCreated OrderSourceDateCreated,
		os.ProductsSubtotal OrderSourceProductsSubtotal,
		os.IdAddedBy OrderSourceIdAddedBy
	FROM
	(
		SELECT
			o.Id,
			o.IdObjectType,
			o.IdCustomer,
			o.DateCreated,
			o.Total,
			CAST(scval.Value as INT) as IdServiceCode,
			o.IdOrderSource,
			(
				SELECT 
					COUNT(*)
				FROM Orders ino WITH(NOLOCK) 
				WHERE
					ino.DateCreated>=@from AND ino.DateCreated<@to AND ino.IdCustomer=o.IdCustomer AND
					ino.StatusCode!=3 AND ino.IdObjectType=5 AND
					((ino.OrderStatus IS NOT NULL AND ino.OrderStatus!=1 AND ino.OrderStatus!=4) OR 
					(ino.OrderStatus IS NULL AND (ino.POrderStatus!=1 AND ino.POrderStatus!=4) OR
					(ino.NPOrderStatus!=1 AND ino.NPOrderStatus!=4))) 
			) Reships,
			(
				SELECT 
					COUNT(*)
				FROM Orders ino WITH(NOLOCK) 
				WHERE
					ino.DateCreated>=@from AND ino.DateCreated<@to AND ino.IdCustomer=o.IdCustomer AND
					ino.StatusCode!=3 AND ino.IdObjectType=6 AND
					((ino.OrderStatus IS NOT NULL AND ino.OrderStatus!=1 AND ino.OrderStatus!=4) OR 
					(ino.OrderStatus IS NULL AND (ino.POrderStatus!=1 AND ino.POrderStatus!=4) OR
					(ino.NPOrderStatus!=1 AND ino.NPOrderStatus!=4))) 
			) Refunds
		FROM Orders o WITH(NOLOCK)
		LEFT JOIN OrderOptionTypes AS scopt WITH(NOLOCK) ON scopt.Name = N'ServiceCode' AND scopt.IdObjectType = o.IdObjectType
		LEFT JOIN OrderOptionValues AS scval WITH(NOLOCK) ON scval.IdOrder = o.Id AND scval.IdOptionType = scopt.Id
		WHERE 
			o.DateCreated>=@from AND o.DateCreated<@to AND
			o.StatusCode!=3 AND (o.IdObjectType=5 OR o.IdObjectType=6) AND
			((o.OrderStatus IS NOT NULL AND o.OrderStatus!=1 AND o.OrderStatus!=4) OR 
			(o.OrderStatus IS NULL AND (o.POrderStatus!=1 AND o.POrderStatus!=4) OR
			(o.NPOrderStatus!=1 AND o.NPOrderStatus!=4))) AND
			(@idcustomer IS NULL OR o.IdCustomer = @idcustomer) AND
			(@idservicecode IS NULL OR scval.Value = CAST(@idservicecode as NVARCHAR(250)))
	) temp
	JOIN Customers c WITH(NOLOCK) ON temp.IdCustomer=c.Id
	LEFT JOIN AddressOptionTypes AS cadfopt WITH(NOLOCK) ON cadfopt.Name = N'FirstName'
	LEFT JOIN AddressOptionValues AS cadfval WITH(NOLOCK) ON cadfval.IdAddress = c.IdProfileAddress AND cadfval.IdOptionType = cadfopt.Id
	LEFT JOIN AddressOptionTypes AS cadlopt WITH(NOLOCK) ON cadlopt.Name = N'LastName'
	LEFT JOIN AddressOptionValues AS cadlval WITH(NOLOCK) ON cadlval.IdAddress = c.IdProfileAddress AND cadlval.IdOptionType = cadlopt.Id
	JOIN Orders os WITH(NOLOCK) ON temp.IdOrderSource=os.Id
	LEFT JOIN OrderOptionTypes AS scnopt WITH(NOLOCK) ON scnopt.Name = N'ServiceCodeNotes' AND scnopt.IdObjectType = temp.IdObjectType
	LEFT JOIN OrderOptionValues AS scnval WITH(NOLOCK) ON scnval.IdOrder = temp.Id AND scnval.IdOptionType = scnopt.Id
	WHERE 
		(@reships IS NULL OR temp.Reships >= @reships) AND
		(@refunds IS NULL OR temp.Refunds >= @refunds) AND
		(@reshipsorrefunds IS NULL OR (temp.Reships + temp.Refunds) >= @reshipsorrefunds)
	ORDER BY temp.Id DESC
	OPTION(RECOMPILE)
END

GO

IF OBJECT_ID(N'[dbo].[SPGetCustomersAbuseReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetCustomersAbuseReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetCustomersAbuseReport]
	@from datetime2,
	@to datetime2,
	@reships int = NULL,
	@refunds int =NULL,
	@reshipsorrefunds int NULL,
	@idservicecode int = NULL,
	@idcustomer int = NULL
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @customers AS TABLE
    (
        IdCustomer int NOT NULL,
		CustomerFirstName NVARCHAR(250) NULL,
		CustomerLastName NVARCHAR(250) NULL
    );

	INSERT INTO @customers
	(
		IdCustomer,
		CustomerFirstName,
		CustomerLastName
	)
	(
	SELECT
		temp.IdCustomer,
		cadfval.Value CustomerFirstName,
		cadlval.Value CustomerLastName
	FROM
	(
			SELECT
				tempInner.IdCustomer,
				(
					SELECT 
						COUNT(*)
					FROM Orders ino WITH(NOLOCK) 
					WHERE
						ino.DateCreated>=@from AND ino.DateCreated<@to AND ino.IdCustomer=tempInner.IdCustomer AND
						ino.StatusCode!=3 AND ino.IdObjectType=5 AND
						((ino.OrderStatus IS NOT NULL AND ino.OrderStatus!=1 AND ino.OrderStatus!=4) OR 
						(ino.OrderStatus IS NULL AND (ino.POrderStatus!=1 AND ino.POrderStatus!=4) OR
						(ino.NPOrderStatus!=1 AND ino.NPOrderStatus!=4))) 
				) Reships,
				(
					SELECT 
						COUNT(*)
					FROM Orders ino WITH(NOLOCK) 
					WHERE
						ino.DateCreated>=@from AND ino.DateCreated<@to AND ino.IdCustomer=tempInner.IdCustomer AND
						ino.StatusCode!=3 AND ino.IdObjectType=6 AND
						((ino.OrderStatus IS NOT NULL AND ino.OrderStatus!=1 AND ino.OrderStatus!=4) OR 
						(ino.OrderStatus IS NULL AND (ino.POrderStatus!=1 AND ino.POrderStatus!=4) OR
						(ino.NPOrderStatus!=1 AND ino.NPOrderStatus!=4))) 
				) Refunds
			FROM
			(
				SELECT 
					 DISTINCT o.IdCustomer
				FROM Orders o WITH(NOLOCK)
				LEFT JOIN OrderOptionTypes AS scopt WITH(NOLOCK) ON scopt.Name = N'ServiceCode' AND scopt.IdObjectType = o.IdObjectType
				LEFT JOIN OrderOptionValues AS scval WITH(NOLOCK) ON scval.IdOrder = o.Id AND scval.IdOptionType = scopt.Id
				WHERE 
					o.DateCreated>=@from AND o.DateCreated<@to AND
					o.StatusCode!=3 AND (o.IdObjectType=5 OR o.IdObjectType=6) AND
					((o.OrderStatus IS NOT NULL AND o.OrderStatus!=1 AND o.OrderStatus!=4) OR 
					(o.OrderStatus IS NULL AND (o.POrderStatus!=1 AND o.POrderStatus!=4) OR
					(o.NPOrderStatus!=1 AND o.NPOrderStatus!=4))) AND
					(@idcustomer IS NULL OR o.IdCustomer = @idcustomer) AND
					(@idservicecode IS NULL OR scval.Value = CAST(@idservicecode as NVARCHAR(250)))
			) tempInner		 
		) temp
		JOIN Customers c WITH(NOLOCK) ON temp.IdCustomer=c.Id
		LEFT JOIN AddressOptionTypes AS cadfopt WITH(NOLOCK) ON cadfopt.Name = N'FirstName'
		LEFT JOIN AddressOptionValues AS cadfval WITH(NOLOCK) ON cadfval.IdAddress = c.IdProfileAddress AND cadfval.IdOptionType = cadfopt.Id
		LEFT JOIN AddressOptionTypes AS cadlopt WITH(NOLOCK) ON cadlopt.Name = N'LastName'
		LEFT JOIN AddressOptionValues AS cadlval WITH(NOLOCK) ON cadlval.IdAddress = c.IdProfileAddress AND cadlval.IdOptionType = cadlopt.Id
		WHERE 
			(@reships IS NULL OR temp.Reships >= @reships) AND
			(@refunds IS NULL OR temp.Refunds >= @refunds) AND
			(@reshipsorrefunds IS NULL OR (temp.Reships + temp.Refunds) >= @reshipsorrefunds)		
	)

	SELECT 
		o.Id,
		o.IdObjectType,
		o.DateCreated,
		c.IdCustomer,
		c.CustomerFirstName,
		c.CustomerLastName,
		CAST(scval.Value as INT) as IdServiceCode
	FROM @customers c
	JOIN Orders o WITH(NOLOCK) ON c.IdCustomer=o.IdCustomer
	LEFT JOIN OrderOptionTypes AS scopt WITH(NOLOCK) ON scopt.Name = N'ServiceCode' AND scopt.IdObjectType = o.IdObjectType
	LEFT JOIN OrderOptionValues AS scval WITH(NOLOCK) ON scval.IdOrder = o.Id AND scval.IdOptionType = scopt.Id
	WHERE
		o.DateCreated>=@from AND o.DateCreated<@to AND
		o.StatusCode!=3 AND o.IdObjectType!=2 AND
		((o.OrderStatus IS NOT NULL AND o.OrderStatus!=1 AND o.OrderStatus!=4) OR 
		(o.OrderStatus IS NULL AND (o.POrderStatus!=1 AND o.POrderStatus!=4) OR
		(o.NPOrderStatus!=1 AND o.NPOrderStatus!=4)))
	ORDER BY c.CustomerFirstName+c.CustomerLastName DESC
	OPTION(RECOMPILE)
END

GO