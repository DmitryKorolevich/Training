IF OBJECT_ID(N'[dbo].[SPGetAAFESReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetAAFESReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetAAFESReport]
	@from datetime2=NULL,
	@to datetime2=NULL,
	@shipfrom datetime2=NULL,
	@shipto datetime2=NULL
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @KeyCodeId INT, @OrderNotesId INT
	DECLARE @temp TABLE
	(
		IdOrder INT,
		IdSku INT,
		OrderNotes NVARCHAR(250),
		ShipMethodFreightCarrier NVARCHAR(500),
		TrackingNumber NVARCHAR(500),
		Quantity INT,
		DateCreated DATETIME2,
		ShippedDate DATETIME2
	)

	SET @KeyCodeId =
	(
		SELECT 
		TOP 1 Id
		FROM OrderOptionTypes
		WHERE Name='KeyCode' AND (IdObjectType IS NULL OR IdObjectType=3)--drop ship
	);
	SET @OrderNotesId =
	(
		SELECT 
		TOP 1 Id
		FROM OrderOptionTypes
		WHERE Name='OrderNotes' AND (IdObjectType IS NULL OR IdObjectType=3)--drop ship
	);

	INSERT INTO @temp
	(
		IdOrder,
		IdSku, 
		OrderNotes,
		ShipMethodFreightCarrier,
		TrackingNumber,
		Quantity,
		DateCreated,
		ShippedDate
	)
	SELECT 
		o.Id,
		sp.IdSku,
		ons.Value OrderNotes,
		sp.ShipMethodFreightCarrier,
		sp.TrackingNumber,
		sp.Quantity,
		o.DateCreated,
		sp.ShippedDate
	FROM Orders o WITH(NOLOCK)
	JOIN OrderOptionValues okc WITH(NOLOCK) ON okc.IdOrder = o.Id AND okc.IdOptionType=@KeyCodeId
	LEFT JOIN OrderOptionValues ons WITH(NOLOCK) ON ons.IdOrder = o.Id AND ons.IdOptionType=@OrderNotesId
	JOIN OrderShippingPackages sp WITH(NOLOCK) ON sp.IdOrder=o.Id
	WHERE 
		okc.Value='AAFES ORDER' AND 
		(@from IS NULL OR o.DateCreated>=@from) AND
		(@to IS NULL OR o.DateCreated<=@to) AND
		(@shipfrom IS NULL OR sp.ShippedDate>=@shipfrom) AND
		(@shipto IS NULL OR sp.ShippedDate<=@shipto) AND
		o.StatusCode!=3 AND o.IdObjectType=3 AND
		(OrderStatus=3 OR POrderStatus=3 OR NPOrderStatus=3)

	SELECT
		ROW_NUMBER() OVER (ORDER BY IdOrder DESC) RowNumber,
		*
	FROM
	(
		SELECT
			t.IdOrder,
			t.IdSku,
			t.OrderNotes,
			t.ShipMethodFreightCarrier,
			t.TrackingNumber,
			t.DateCreated,
			t.ShippedDate,
			ISNULL(t.Quantity, ots.Quantity) Quantity,
			s.Code
		FROM @temp t 
		JOIN OrderToSkus ots ON t.IdOrder=ots.IdOrder AND t.IdSku=ots.IdSku
		JOIN Skus s ON ots.IdSku=s.Id
		UNION ALL
		SELECT
			t.IdOrder,
			t.IdSku,
			t.OrderNotes,
			t.ShipMethodFreightCarrier,
			t.TrackingNumber,
			t.DateCreated,
			t.ShippedDate,
			ISNULL(t.Quantity, otp.Quantity) Quantity,
			s.Code
		FROM @temp t 
		JOIN OrderToPromos otp ON t.IdOrder=otp.IdOrder AND t.IdSku=otp.IdSku AND otp.Disabled=0
		JOIN Skus s ON otp.IdSku=s.Id
	) temp

END

GO
