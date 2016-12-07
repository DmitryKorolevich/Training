IF OBJECT_ID(N'[dbo].[SPGetSkuPOrderTypeFutureBreakDownReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetSkuPOrderTypeFutureBreakDownReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetSkuPOrderTypeFutureBreakDownReport]
	@from datetime2,
	@to datetime2,
	@code nvarchar(250)
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @sFrom NVARCHAR(20), @sTo NVARCHAR(20)
	SET @sFrom = CONVERT(NVARCHAR(25), @from, 126)
	SET @sTo = CONVERT(NVARCHAR(25), @to, 126)

	DECLARE @orders AS TABLE
    (
        IdOrder int NOT NULL, 
		ShipDelayDate DATETIME2 NULL, 
		ShipDelayDateP DATETIME2 NULL, 
		ShipDelayDateNP DATETIME2 NULL,
		POrderType int NULL,
		POrderStatus int NULL,
		NPOrderStatus int NULL
    );

	DECLARE @skus AS TABLE
	(
		IdSku int NOT NULL
	);

	IF(@code IS NOT NULL)
	BEGIN
		INSERT INTO @skus
		(
			IdSku
		)
		(
			SELECT 
				s.Id
			FROM Skus s
			WHERE 
				s.StatusCode!=3 AND s.Code LIKE @code+'%'
		)
	END

	INSERT INTO @orders
	(
		IdOrder,
		ShipDelayDate,
		ShipDelayDateP, 
		ShipDelayDateNP, 
		POrderType,		
		POrderStatus,
		NPOrderStatus
	)
	(SELECT 
		o.Id, 
		CONVERT(DATETIME2,sdval.Value,126),
		CONVERT(DATETIME2,sdpval.Value,126), 
		CONVERT(DATETIME2,sdnpval.Value,126),
		CAST(pval.Value as INT),
		o.POrderStatus,
		o.NPOrderStatus
		FROM Orders o WITH(NOLOCK)
		LEFT JOIN OrderOptionTypes AS popt WITH(NOLOCK) ON popt.Name = N'POrderType'
		LEFT JOIN OrderOptionValues AS pval WITH(NOLOCK) ON pval.IdOrder = o.Id AND pval.IdOptionType = popt.Id	
		LEFT JOIN OrderOptionTypes AS sdopt WITH(NOLOCK) ON sdopt.Name = N'ShipDelayDate' AND sdopt.IdObjectType=o.IdObjectType
		LEFT JOIN OrderOptionValues AS sdval WITH(NOLOCK) ON sdval.IdOrder = o.Id AND sdval.IdOptionType = sdopt.Id	
		LEFT JOIN OrderOptionTypes AS sdpopt WITH(NOLOCK) ON sdpopt.Name = N'ShipDelayDateP' AND sdpopt.IdObjectType=o.IdObjectType
		LEFT JOIN OrderOptionValues AS sdpval WITH(NOLOCK) ON sdpval.IdOrder = o.Id AND sdpval.IdOptionType = sdpopt.Id	
		LEFT JOIN OrderOptionTypes AS sdnpopt WITH(NOLOCK) ON sdnpopt.Name = N'ShipDelayDateNP' AND sdnpopt.IdObjectType=o.IdObjectType
		LEFT JOIN OrderOptionValues AS sdnpval WITH(NOLOCK) ON sdnpval.IdOrder = o.Id AND sdnpval.IdOptionType = sdnpopt.Id	
		WHERE ((sdval.Value>=@sFrom AND sdval.Value<=@sTo) OR (sdpval.Value>=@sFrom AND sdpval.Value<=@sTo) OR
			(sdnpval.Value>=@sFrom AND sdnpval.Value<=@sTo)) AND 
			o.StatusCode!=3 AND	o.IdObjectType NOT IN (2,5,6) AND
			((o.OrderStatus IS NOT NULL AND o.OrderStatus=6) OR 
			(o.OrderStatus IS NULL AND (o.POrderStatus =6 OR 
			o.NPOrderStatus =6))) AND
			(
				@code IS NULL OR
				EXISTS
				(
					SELECT
						TOP 1 os.IdOrder
					FROM OrderToSkus os
					WHERE os.IdOrder=o.Id AND os.IdSku IN (SELECT IdSku FROM @skus)
				) OR
				EXISTS
				(
					SELECT
						TOP 1 op.IdOrder
					FROM OrderToPromos op
					WHERE op.IdOrder=o.Id AND op.Disabled=0 AND op.IdSku IN (SELECT IdSku FROM @skus)
				)
			)		
	)

	SELECT 
		ROW_NUMBER() OVER (ORDER BY temp.IdOrder) as RowNumber,
		temp.IdOrder, temp.IdSku, s.Code, s.IdProduct, temp.Quantity, p.IdObjectType as ProductIdObjectType,
		temp.ShipDelayDate, temp.ShipDelayDateP, temp.ShipDelayDateNP, CAST(temp.POrderType as INT) as POrderType
		FROM
		(SELECT
			 tempInner.IdOrder, tempInner.IdSku, SUM(tempInner.Quantity) as Quantity, 
			MIN(tempInner.ShipDelayDate) as ShipDelayDate, MIN(tempInner.ShipDelayDateP) as ShipDelayDateP,
			MIN(tempInner.ShipDelayDateNP) as ShipDelayDateNP, MIN(tempInner.POrderType) as POrderType
			FROM
			(SELECT 
					o.IdOrder, o.ShipDelayDate, o.ShipDelayDateP, o.ShipDelayDateNP, o.POrderType, os.IdSku, os.Quantity
					FROM @orders o
					JOIN OrderToSkus os WITH(NOLOCK) ON o.IdOrder=os.IdOrder
					JOIN Skus s WITH(NOLOCK) ON os.IdSku=s.Id
					JOIN Products p WITH(NOLOCK) ON s.IdProduct=p.Id
					WHERE 
						(o.POrderStatus IS NULL AND o.NPOrderStatus IS NULL) OR
						(
							(o.POrderStatus!=4 OR (o.POrderStatus=4 AND (p.IdObjectType=1 OR p.IdObjectType=3 OR p.IdObjectType=4))) AND
							(o.NPOrderStatus!=4 OR (o.NPOrderStatus=4 AND (p.IdObjectType=2)))
						)
				UNION ALL
				SELECT 
					o.IdOrder, o.ShipDelayDate, o.ShipDelayDateP, o.ShipDelayDateNP, o.POrderType, op.IdSku, op.Quantity
					FROM @orders o
					JOIN OrderToPromos op WITH(NOLOCK) ON o.IdOrder=op.IdOrder AND op.Disabled=0
					JOIN Skus s WITH(NOLOCK) ON op.IdSku=s.Id
					JOIN Products p WITH(NOLOCK) ON s.IdProduct=p.Id
					WHERE 
						(o.POrderStatus IS NULL AND o.NPOrderStatus IS NULL) OR
						(
							(o.POrderStatus!=4 OR (o.POrderStatus=4 AND (p.IdObjectType=1 OR p.IdObjectType=3 OR p.IdObjectType=4))) AND
							(o.NPOrderStatus!=4 OR (o.NPOrderStatus=4 AND (p.IdObjectType=2)))
						)
			) tempInner
			GROUP BY tempInner.IdOrder,tempInner.IdSku)	temp	
		JOIN Skus s ON temp.IdSku=s.Id	
		JOIN Products p ON s.IdProduct=p.Id	

END

GO