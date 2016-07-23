USE [vitalchoice2.0]
GO

IF OBJECT_ID('[dbo].[DelimitedSplit8K]') IS NOT NULL
	DROP FUNCTION [dbo].[DelimitedSplit8K]

GO

CREATE FUNCTION [dbo].[DelimitedSplit8K]
--===== Define I/O parameters
        (@pString VARCHAR(8000), @pDelimiter NVARCHAR(4))
--WARNING!!! DO NOT USE MAX DATA-TYPES HERE!  IT WILL KILL PERFORMANCE!
RETURNS TABLE WITH SCHEMABINDING AS
 RETURN
--===== "Inline" CTE Driven "Tally Table" produces values from 1 up to 10,000...
     -- enough to cover VARCHAR(8000)
  WITH E1(N) AS (
                 SELECT 1 UNION ALL SELECT 1 UNION ALL SELECT 1 UNION ALL
                 SELECT 1 UNION ALL SELECT 1 UNION ALL SELECT 1 UNION ALL
                 SELECT 1 UNION ALL SELECT 1 UNION ALL SELECT 1 UNION ALL SELECT 1
                ),                          --10E+1 or 10 rows
       E2(N) AS (SELECT 1 FROM E1 a, E1 b), --10E+2 or 100 rows
       E4(N) AS (SELECT 1 FROM E2 a, E2 b), --10E+4 or 10,000 rows max
 cteTally(N) AS (--==== This provides the "base" CTE and limits the number of rows right up front
                     -- for both a performance gain and prevention of accidental "overruns"
                 SELECT TOP (ISNULL(DATALENGTH(@pString),0)) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) FROM E4
                ),
cteStart(N1) AS (--==== This returns N+1 (starting position of each "element" just once for each delimiter)
                 SELECT 1 UNION ALL
                 SELECT t.N+1 FROM cteTally t WHERE SUBSTRING(@pString,t.N,1) = @pDelimiter
                ),
cteLen(N1,L1) AS(--==== Return start and length (for use in substring)
                 SELECT s.N1,
                        ISNULL(NULLIF(CHARINDEX(@pDelimiter,@pString,s.N1),0)-s.N1,8000)
                   FROM cteStart s
                )
--===== Do the actual split. The ISNULL/NULLIF combo handles the length for the final element when no delimiter is found.
 SELECT ItemNumber = ROW_NUMBER() OVER(ORDER BY l.N1),
        Item       = SUBSTRING(@pString, l.N1, l.L1)
   FROM cteLen l
;
GO

USE [VitalChoice.Ecommerce]
GO

DELETE FROM AffiliateOrderPayments
GO
DELETE FROM AffiliatePayments
GO
TRUNCATE TABLE CartToSkus
GO
DELETE FROM CartToGiftCertificates
GO
DELETE FROM Carts
GO
DELETE FROM RefundOrderToGiftCertificates
GO

UPDATE GiftCertificates
SET IdOrder = NULL
WHERE IdOrder IS NOT NULL
GO
TRUNCATE TABLE HealthwiseOrders
GO
DELETE FROM HealthwisePeriods
GO
DELETE FROM HelpTicketComments
GO
DELETE FROM HelpTickets
GO
TRUNCATE TABLE OrderOptionValues
GO
DELETE FROM OrderShippingPackages
GO
DELETE FROM OrderToGiftCertificates
GO
DELETE FROM OrderToPromosToInventorySkus
GO
DELETE FROM OrderToPromos
GO
DELETE FROM OrderToSkusToInventorySkus
GO
DELETE FROM OrderToSkus
GO
DELETE FROM RefundSkus
GO
DELETE FROM ReshipProblemSkus
GO
TRUNCATE TABLE OrderAddressOptionValues
GO
DELETE FROM Orders
GO
TRUNCATE TABLE OrderPaymentMethodOptionValues
GO
DELETE FROM OrderPaymentMethods
GO
DELETE FROM OrderAddresses

GO

DBCC CHECKIDENT('Orders', RESEED, 1)

ALTER TABLE Orders
ADD IdAutoShipOrder INT NULL

CREATE NONCLUSTERED INDEX IX_OrdersIdAutoShipOrder ON Orders (IdAutoShipOrder ASC)

GO

INSERT INTO Orders
(IdObjectType, DateCreated, DateEdited, IdEditedBy, StatusCode, OrderStatus, IdCustomer, IdDiscount, Total, ProductsSubtotal, TaxTotal, ShippingTotal, DiscountTotal, IdAddedBy, IdAutoShipOrder)
SELECT 
	2,
	ISNULL(o.pcOrd_Time, o.orderDate),
	ISNULL(o.pcOrd_Time, o.orderDate),
	p.Id,
	CASE o.orderStatus
		WHEN 100 THEN 3
		ELSE 2
	END,
	CASE o.orderStatus
		WHEN 1 THEN 1
		WHEN 3 THEN 2
		WHEN 4 THEN 3
		WHEN 5 THEN 4
		WHEN 95 THEN 5
		WHEN 98 THEN 6
		WHEN 99 THEN 7
		WHEN 96 THEN 2
		WHEN 97 THEN 2
		WHEN 100 THEN 4
		WHEN 6 THEN 2
		ELSE 1
	END,
	o.idCustomer,
	NULL,
	ISNULL(o.total, 0),
	(SELECT SUM(ISNULL(po.quantity, 0) * ISNULL(po.unitPrice, 0)) FROM [vitalchoice2.0].dbo.ProductsOrdered AS po WHERE po.idOrder = o.idOrder),
	o.taxAmount,
	0,
	0,
	p.Id,
	aso.idAutoShipOrder
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.IdOrder = aso.IdOrder
LEFT JOIN [vitalchoice2.0].dbo.admins AS a ON a.idadmin = o.agentId
LEFT JOIN [VitalChoice.Infrastructure].dbo.AdminProfiles AS p ON p.AgentId = a.AgentID COLLATE Cyrillic_General_CI_AS
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS c ON c.Id = o.idCustomer
WHERE o.orderDate IS NOT NULL AND o.idCustomer IS NOT NULL AND EXISTS(SELECT * FROM [vitalchoice2.0].dbo.ProductsOrdered AS po WHERE po.idOrder = o.idOrder)

GO

SET IDENTITY_INSERT Orders ON;

INSERT INTO Orders
(Id, IdObjectType, DateCreated, DateEdited, IdEditedBy, StatusCode, OrderStatus, IdCustomer, IdDiscount, Total, ProductsSubtotal, TaxTotal, ShippingTotal, DiscountTotal, IdAddedBy, IdOrderSource)
SELECT 
	o.idOrder, 
	CASE 
		WHEN o.keyCode = 'DROPSHIP' THEN 3 
		WHEN o.keyCode = 'GIFT LIST' THEN 4 
		WHEN o.isRefund = 1 THEN 6 
		WHEN o.isReship = 1 THEN 5
		WHEN EXISTS(SELECT * FROM [vitalchoice2.0].dbo.autoShipOrders AS aso WHERE aso.idOrder = o.idOrder) THEN 7
		ELSE 1
	END,
	ISNULL(o.pcOrd_Time, o.orderDate),
	ISNULL(o.pcOrd_Time, o.orderDate),
	(
		SELECT TOP 1 p.Id FROM [vitalchoice2.0].dbo.admins AS a 
		LEFT JOIN [VitalChoice.Infrastructure].dbo.AdminProfiles AS p ON p.AgentId = a.AgentID COLLATE Cyrillic_General_CI_AS
		WHERE a.idadmin = o.agentId
	),
	CASE o.orderStatus
		WHEN 100 THEN 3
		ELSE 2
	END,
	CASE o.orderStatus
		WHEN 1 THEN 1
		WHEN 3 THEN 2
		WHEN 4 THEN 3
		WHEN 5 THEN 4
		WHEN 95 THEN 5
		WHEN 98 THEN 6
		WHEN 99 THEN 7
		WHEN 96 THEN 2
		WHEN 97 THEN 2
		WHEN 100 THEN 4
		WHEN 6 THEN 2
		ELSE 1
	END,
	o.idCustomer,
	NULL,
	ISNULL(o.total, 0),
	(SELECT SUM(ISNULL(po.quantity, 0) * ISNULL(po.unitPrice, 0)) FROM [vitalchoice2.0].dbo.ProductsOrdered AS po WHERE po.idOrder = o.idOrder),
	o.taxAmount,
	0,
	0,
	(
		SELECT TOP 1 p.Id FROM [vitalchoice2.0].dbo.admins AS a 
		LEFT JOIN [VitalChoice.Infrastructure].dbo.AdminProfiles AS p ON p.AgentId = a.AgentID COLLATE Cyrillic_General_CI_AS
		WHERE a.idadmin = o.agentId
	),
	oo.Id
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN [VitalChoice.Ecommerce].dbo.Customers AS c ON c.Id = o.idCustomer
LEFT JOIN [vitalchoice2.0].dbo.autoshipOrders AS aso ON aso.IdOrder = o.IdOrder
LEFT JOIN Orders AS oo ON oo.IdAutoshipOrder = aso.IdAutoshipOrder
WHERE o.orderDate IS NOT NULL AND o.idCustomer IS NOT NULL AND EXISTS(SELECT * FROM [vitalchoice2.0].dbo.ProductsOrdered AS po WHERE po.idOrder = o.idOrder)

SET IDENTITY_INSERT Orders OFF;

GO

UPDATE Orders
SET ShippingTotal = ISNULL(TRY_CONVERT(MONEY,[vitalchoice2.0].dbo.RegexReplace('[a-zA-Z ]+,[0-9a-zA-Z +-]+,([0-9]*(\.[0-9]+)?),\w+,\w+,\w+', o.shipmentDetails, '$1')), 0),
	DiscountTotal = 
		CASE 
			WHEN o.discountDetails IS NOT NULL AND o.discountDetails <> 'No discounts applied.' 
				THEN 
					CASE WHEN o.discountDetails LIKE '%(%)%' 
						THEN ISNULL(TRY_CONVERT(MONEY,[vitalchoice2.0].dbo.RegexReplace('[\w\s\.\(\)]+\s*(\([^\(\)]*\))\s-\s\|{2}\s(-?[0-9]+(\.[0-9]+))?', o.discountDetails, '$2')), 0)
						ELSE ISNULL(TRY_CONVERT(MONEY,[vitalchoice2.0].dbo.RegexReplace('([\w\s\.\(\)]+)\s*-\s\|{2}\s(-?[0-9]+(\.[0-9]+))?', o.discountDetails, '$2')), 0)
					END
			ELSE 0
		END,
	IdDiscount = (
		SELECT TOP 1 d.Id FROM Discounts AS d 
		WHERE o.discountDetails IS NOT NULL AND o.discountDetails <> 'No discounts applied.' 
			AND d.Code =
			CASE WHEN o.discountDetails LIKE '%(%)%' 
				THEN [vitalchoice2.0].dbo.RegexReplace('.+?\(([^\(\)]*)\)\s-\s\|{2}\s(-?[0-9]+(\.[0-9]+))?', o.discountDetails, '$1')
				ELSE [vitalchoice2.0].dbo.RegexReplace('([\w\s\.\(\)]+)\s*-\s\|{2}\s(-?[0-9]+(\.[0-9]+))?', o.discountDetails, '$1')
			END
		ORDER BY Id DESC
	)
FROM Orders AS oo
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = oo.Id

GO

--fields (Normal order)

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	o.IdOrder AS Id,
	CAST(CAST(o.shippingSurcharge AS MONEY) AS NVARCHAR(250)) AS AlaskaHawaiiSurcharge,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.giftMessage AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS GiftMessage,
	CAST(CASE WHEN ISNULL(o.giftOrder, 0) <> 0 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS GiftOrder,
	CAST(CASE WHEN hw.id IS NOT NULL THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS IsHealthwise,
	CAST(o.keyCode AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS KeyCode,
	CAST(CASE WHEN o.orderType = 2 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS MailOrder,
	CAST(CASE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(cc.PONum AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS PoNumber,
	CASE WHEN o.shipDelayType > 0 THEN CONVERT(NVARCHAR(250), CAST(CASE o.shipDelayPart WHEN 2 THEN o.shipDelayNonperish ELSE o.shipDelay END AS DATETIME2), 126) ELSE N'' END AS ShipDelayDate,
	CAST(CASE WHEN o.shipDelayType > 0 THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShipDelayType,
	CAST(CAST(o.shippingOverride AS MONEY) AS NVARCHAR(250)) AS ShippingOverride,
	CAST(CAST(o.surchargeOverride AS MONEY) AS NVARCHAR(250)) AS SurchargeOverride,
	CAST(CASE WHEN o.shipmentDetails LIKE '%2Day%' THEN N'2' WHEN o.shipmentDetails LIKE '%Overnight Non-Perishable%' THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShippingUpgradeNP,
	CAST(CASE WHEN o.shipmentDetails LIKE '%Overnight Perishable%' THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShippingUpgradeP,
	CAST(CAST(o.shippingAmount AS MONEY) AS NVARCHAR(250)) AS StandardShippingCharges
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder AND oo.IdObjectType = 1
LEFT JOIN [vitalchoice2.0].dbo.healthwise AS hw ON hw.id = o.healthwiseId
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		AlaskaHawaiiSurcharge,
		--AutoShipFrequency,
		--CanadaSurcharge,
		ConfirmationEmailSent,
		GiftMessage,
		GiftOrder,
		--Guest,
		--IdDiscountTier,
		--IgnoneMinimumPerishableThreshold,
		IsHealthwise,
		KeyCode,
		MailOrder,
		OrderNotes,
		OrderType,
		PoNumber,
		ShipDelayDate,
		ShipDelayType,
		ShippingOverride,
		SurchargeOverride,
		ShippingUpgradeNP,
		ShippingUpgradeP,
		StandardShippingCharges
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (t.IdObjectType IS NULL OR t.IdObjectType = 1)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO
--fields (Drop-Ship order)

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	o.IdOrder AS Id,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.giftMessage AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS GiftMessage,
	CAST(o.keyCode AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS KeyCode,
	CAST(CASE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(cc.PONum AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS PoNumber,
	CASE WHEN o.shipDelayType > 0 THEN CONVERT(NVARCHAR(250), CAST(CASE o.shipDelayPart WHEN 2 THEN o.shipDelayNonperish ELSE o.shipDelay END AS DATETIME2), 126) ELSE NULL END AS ShipDelayDate,
	CAST(CASE WHEN o.shipDelayType > 0 THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShipDelayType
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder AND oo.IdObjectType = 3
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		ConfirmationEmailSent,
		GiftMessage,
		--IdDiscountTier,
		KeyCode,
		OrderNotes,
		OrderType,
		PoNumber,
		--POrderType,
		ShipDelayDate,
		ShipDelayType
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (t.IdObjectType IS NULL OR t.IdObjectType = 3)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO
--fields (Gift-List order)

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	o.IdOrder AS Id,
	CAST(CAST(o.shippingSurcharge AS MONEY) AS NVARCHAR(250)) AS AlaskaHawaiiSurcharge,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.giftMessage AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS GiftMessage,
	CAST(CASE WHEN ISNULL(o.giftOrder, 0) <> 0 THEN N'True' ELSE NULL END AS NVARCHAR(250)) AS GiftOrder,
	CAST(CASE WHEN hw.id IS NOT NULL THEN N'True' ELSE NULL END AS NVARCHAR(250)) AS IsHealthwise,
	CAST(o.keyCode AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS KeyCode,
	CAST(CASE WHEN o.orderType = 2 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS MailOrder,
	CAST(CASE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(cc.PONum AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS PoNumber,
	CAST(CASE WHEN o.shipDelayType > 0 THEN CONVERT(NVARCHAR(250), CAST(CASE o.shipDelayPart WHEN 2 THEN o.shipDelayNonperish ELSE o.shipDelay END AS DATETIME2), 126) ELSE NULL END AS NVARCHAR(250)) AS ShipDelayDate,
	CAST(CASE WHEN o.shipDelayType > 0 THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShipDelayType,
	CAST(CAST(o.shippingOverride AS MONEY) AS NVARCHAR(250)) AS ShippingOverride,
	CAST(CAST(o.surchargeOverride AS MONEY) AS NVARCHAR(250)) AS SurchargeOverride,
	CAST(CASE WHEN o.shipmentDetails LIKE '%2Day%' THEN N'2' WHEN o.shipmentDetails LIKE '%Overnight Non-Perishable%' THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShippingUpgradeNP,
	CAST(CASE WHEN o.shipmentDetails LIKE '%Overnight Perishable%' THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShippingUpgradeP,
	CAST(CAST(o.shippingAmount AS MONEY) AS NVARCHAR(250)) AS StandardShippingCharges
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder AND oo.IdObjectType = 4
LEFT JOIN [vitalchoice2.0].dbo.healthwise AS hw ON hw.id = o.healthwiseId
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		p.AlaskaHawaiiSurcharge,
		--AutoShipFrequency,
		--CanadaSurcharge,
		p.ConfirmationEmailSent,
		p.GiftMessage,
		p.GiftOrder,
		--Guest,
		--IdDiscountTier,
		--IgnoneMinimumPerishableThreshold,
		p.IsHealthwise,
		p.KeyCode,
		p.MailOrder,
		p.OrderNotes,
		p.OrderType,
		p.PoNumber,
		p.ShipDelayDate,
		p.ShipDelayType,
		p.ShippingOverride,
		p.SurchargeOverride,
		p.ShippingUpgradeNP,
		p.ShippingUpgradeP,
		p.StandardShippingCharges
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (t.IdObjectType IS NULL OR t.IdObjectType = 4)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO

--add up missed service codes

DELETE FROM LookupVariants
WHERE IdLookup = (SELECT Id FROM Lookups WHERE Name = 'ServiceCodes')

INSERT INTO LookupVariants
(Id, IdLookup, [Order], ValueVariant)
SELECT ROW_NUMBER() OVER (ORDER BY serviceCode), (SELECT Id FROM Lookups WHERE Name = 'ServiceCodes'), ROW_NUMBER() OVER (ORDER BY serviceCode), serviceCode FROM
(
SELECT DISTINCT serviceCode FROM (
SELECT serviceCode FROM [vitalchoice2.0].dbo.reship
UNION ALL
SELECT serviceCode FROM [vitalchoice2.0].dbo.refund
) f
) ff

GO

--fields (Reship order)

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	o.IdOrder AS Id,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.keyCode AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS KeyCode,
	CAST(CASE LEFT(CAST(ISNULL(r.notes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(cc.PONum AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS PoNumber,
	CAST(CASE WHEN o.shipDelayType > 0 THEN CONVERT(NVARCHAR(250), CAST(CASE o.shipDelayPart WHEN 2 THEN o.shipDelayNonperish ELSE o.shipDelay END AS DATETIME2), 126) ELSE NULL END AS NVARCHAR(250)) AS ShipDelayDate,
	CAST(CASE WHEN o.shipDelayType > 0 THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShipDelayType,
	CAST(CAST(o.shippingOverride AS MONEY) AS NVARCHAR(250)) AS ShippingOverride,
	CAST(CAST(o.surchargeOverride AS MONEY) AS NVARCHAR(250)) AS SurchargeOverride,
	CAST(CASE WHEN o.shipmentDetails LIKE '%2Day%' THEN N'2' WHEN o.shipmentDetails LIKE '%Overnight Non-Perishable%' THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShippingUpgradeNP,
	CAST(CASE WHEN o.shipmentDetails LIKE '%Overnight Perishable%' THEN N'1' ELSE NULL END AS NVARCHAR(250)) AS ShippingUpgradeP,
	CAST(CAST(o.shippingAmount AS MONEY) AS NVARCHAR(250)) AS StandardShippingCharges,
	CAST(CASE WHEN r.associated <> 0 THEN N'True' ELSE N'False' END AS NVARCHAR(250)) ReturnAssociated,
	CAST(lv.Id AS NVARCHAR(250)) AS ServiceCode
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder AND oo.IdObjectType = 5
INNER JOIN [vitalchoice2.0].dbo.reship AS r ON r.IdOrder = o.IdOrder
INNER JOIN LookupVariants AS lv ON lv.IdLookup = (SELECT Id FROM Lookups WHERE Name = 'ServiceCodes') AND lv.ValueVariant = r.serviceCode COLLATE Cyrillic_General_CI_AS
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		p.ConfirmationEmailSent,
		--IdDiscountTier,
		p.KeyCode,
		p.OrderNotes,
		p.OrderType,
		p.PoNumber,
		--POrderType,
		p.ReturnAssociated,
		p.ServiceCode,
		p.ShipDelayDate,
		--ShipDelayDateNP,
		--ShipDelayDateP,
		p.ShipDelayType,
		p.ShippingOverride,
		p.ShippingUpgradeNP,
		p.ShippingUpgradeP,
		p.SurchargeOverride
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (t.IdObjectType IS NULL OR t.IdObjectType = 5)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO
--fields (Refund order)

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	o.IdOrder AS Id,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.keyCode AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS KeyCode,
	CAST(CASE LEFT(CAST(ISNULL(r.notes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(CASE WHEN r.associated <> 0 THEN N'True' ELSE N'False' END AS NVARCHAR(250)) ReturnAssociated,
	CAST(lv.Id AS NVARCHAR(250)) AS ServiceCode,
	CAST(r.idOrderOriginal AS NVARCHAR(250)) AS IdOrderRefunded,
	CAST((SELECT SUM(CAST(rr.Value AS MONEY)) FROM [vitalchoice2.0].[dbo].[RefundItems] AS rr WHERE rr.IdRefundOrder = r.idOrder AND rr.ItemType <> 0 AND rr.ItemType <> 3) AS NVARCHAR(250)) AS AutoTotal,
	CAST((SELECT SUM(CAST(rr.Value AS MONEY)) FROM [vitalchoice2.0].[dbo].[RefundItems] AS rr WHERE rr.IdRefundOrder = r.idOrder AND rr.ItemType = 2) AS NVARCHAR(250)) AS ShippingRefunded,
	CAST((SELECT SUM(CAST(rr.Value AS MONEY)) FROM [vitalchoice2.0].[dbo].[RefundItems] AS rr WHERE rr.IdRefundOrder = r.idOrder AND rr.ItemType = 3) AS NVARCHAR(250)) AS ManualRefundOverride
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder AND oo.IdObjectType = 6
INNER JOIN [vitalchoice2.0].dbo.refund AS r ON r.IdOrder = o.IdOrder
INNER JOIN LookupVariants AS lv ON lv.IdLookup = (SELECT Id FROM Lookups WHERE Name = 'ServiceCodes') AND lv.ValueVariant = r.serviceCode COLLATE Cyrillic_General_CI_AS
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		AutoTotal,
		ConfirmationEmailSent,
		--IdDiscountTier,
		IdOrderRefunded,
		ManualRefundOverride,
		OrderNotes,
		OrderType,
		--POrderType,
		ReturnAssociated,
		ServiceCode,
		ShippingRefunded
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (t.IdObjectType IS NULL OR t.IdObjectType = 6)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO
--fields (Source Order for Auto-Ship)

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	oo.Id AS Id,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.giftMessage AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS GiftMessage,
	CAST(CASE WHEN ISNULL(o.giftOrder, 0) <> 0 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS GiftOrder,
	CAST(o.keyCode AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS KeyCode,
	CAST(CASE WHEN o.orderType = 2 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS MailOrder,
	CAST(CASE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(cc.PONum AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS PoNumber,
	CAST(CASE WHEN o.shipDelayType > 0 THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShipDelayType,
	CAST(CAST(o.shippingOverride AS MONEY) AS NVARCHAR(250)) AS ShippingOverride,
	CAST(CAST(o.surchargeOverride AS MONEY) AS NVARCHAR(250)) AS SurchargeOverride,
	CAST(CASE WHEN o.shipmentDetails LIKE '%2Day%' THEN N'2' WHEN o.shipmentDetails LIKE '%Overnight Non-Perishable%' THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShippingUpgradeNP,
	CAST(CASE WHEN o.shipmentDetails LIKE '%Overnight Perishable%' THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShippingUpgradeP,
	CAST(aso.schedule AS NVARCHAR(250)) AS AutoShipFrequency,
	CONVERT(NVARCHAR(250), CAST(DATEADD(month, -aso.schedule, aso.nextDate) AS DATETIME2), 126) AS LastAutoShipDate
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		AutoShipFrequency,
		ConfirmationEmailSent,
		GiftMessage,
		GiftOrder,
		--IdDiscountTier,
		KeyCode,
		LastAutoShipDate,
		MailOrder,
		OrderNotes,
		OrderType,
		PoNumber,
		--POrderType,
		--ShipDelayDateNP,
		--ShipDelayDateP,
		ShipDelayType,
		ShippingOverride,
		ShippingUpgradeNP,
		ShippingUpgradeP,
		SurchargeOverride
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (t.IdObjectType IS NULL OR t.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO
--fields (Auto-Ship order)

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	o.IdOrder AS Id,
	CAST(CAST(o.shippingSurcharge AS MONEY) AS NVARCHAR(250)) AS AlaskaHawaiiSurcharge,
	CAST(CASE WHEN o.PostShipMailSent = 0 THEN N'False' ELSE N'True' END AS NVARCHAR(250)) AS ConfirmationEmailSent,
	CAST(o.giftMessage AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS GiftMessage,
	CAST(CASE WHEN ISNULL(o.giftOrder, 0) <> 0 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS GiftOrder,
	CAST(o.keyCode AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS KeyCode,
	CAST(CASE WHEN o.orderType = 2 THEN N'True' ELSE N'' END AS NVARCHAR(250)) AS MailOrder,
	CAST(CASE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) WHEN N'Null' THEN N'' ELSE LEFT(CAST(ISNULL(o.specificNotes, N'') AS NVARCHAR(MAX)), 250) END AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS OrderNotes,
	CAST(CASE ISNULL(o.orderType, 0) WHEN 0 THEN N'1' WHEN 1 THEN N'2' WHEN 2 THEN N'3' END AS NVARCHAR(250)) AS OrderType,
	CAST(cc.PONum AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS PoNumber,
	CASE WHEN o.shipDelayType > 0 THEN CONVERT(NVARCHAR(250), CAST(CASE o.shipDelayPart WHEN 2 THEN o.shipDelayNonperish ELSE o.shipDelay END AS DATETIME2), 126) ELSE N'' END AS ShipDelayDate,
	CAST(CASE WHEN o.shipDelayType > 0 THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShipDelayType,
	CAST(CAST(o.shippingOverride AS MONEY) AS NVARCHAR(250)) AS ShippingOverride,
	CAST(CAST(o.surchargeOverride AS MONEY) AS NVARCHAR(250)) AS SurchargeOverride,
	CAST(CASE WHEN o.shipmentDetails LIKE '%2Day%' THEN N'2' WHEN o.shipmentDetails LIKE '%Overnight Non-Perishable%' THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShippingUpgradeNP,
	CAST(CASE WHEN o.shipmentDetails LIKE '%Overnight Perishable%' THEN N'1' ELSE N'' END AS NVARCHAR(250)) AS ShippingUpgradeP,
	CAST(CAST(o.shippingAmount AS MONEY) AS NVARCHAR(250)) AS StandardShippingCharges,
	CAST(autoo.Id AS NVARCHAR(250)) AS AutoShipId,
	CAST(aso.schedule AS NVARCHAR(250)) AS AutoShipFrequency
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder AND oo.IdObjectType = 7
INNER JOIN Orders AS autoo ON autoo.IdOrderSource = o.idAutoShipOrder
LEFT JOIN [vitalchoice2.0].dbo.autoShipOrders AS aso ON aso.idAutoShipOrder = o.idAutoShipOrder
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
) p
UNPIVOT (
	Value FOR Name IN (
		AlaskaHawaiiSurcharge,
		AutoShipFrequency,
		AutoShipId,
		--CanadaSurcharge,
		ConfirmationEmailSent,
		GiftMessage,
		GiftOrder,
		--Guest,
		--IdDiscountTier,
		--IgnoneMinimumPerishableThreshold,
		KeyCode,
		MailOrder,
		OrderNotes,
		OrderType,
		PoNumber,
		--POrderType,
		ShipDelayDate,
		--ShipDelayDateNP,
		--ShipDelayDateP,
		ShipDelayType,
		ShippingOverride,
		ShippingUpgradeNP,
		ShippingUpgradeP,
		StandardShippingCharges,
		SurchargeOverride
	)
) AS unpvt
INNER JOIN OrderOptionTypes AS t ON t.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (t.IdObjectType IS NULL OR t.IdObjectType = 7)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

-- billing

GO

ALTER TABLE OrderAddresses
ADD IdOrder INT NOT NULL CONSTRAINT UQ_OrderAddressesOrders UNIQUE
GO

INSERT INTO OrderAddresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdOrder)
SELECT 
	o.orderDate, 
	o.orderDate,
	ISNULL (
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = ISNULL(o.countryCode COLLATE Cyrillic_General_CI_AS, 'US')), 
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM States AS s WHERE s.CountryCode = ISNULL(o.countryCode COLLATE Cyrillic_General_CI_AS, 'US') AND s.StateCode = o.stateCode COLLATE Cyrillic_General_CI_AS),
	2,
	2,
	o.[state],
	o.idOrder
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON o.idOrder = oo.Id

GO
--auto-ship template

INSERT INTO OrderAddresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdOrder)
SELECT 
	o.orderDate, 
	o.orderDate,
	ISNULL (
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = ISNULL(o.countryCode COLLATE Cyrillic_General_CI_AS, 'US')), 
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM States AS s WHERE s.CountryCode = ISNULL(o.countryCode COLLATE Cyrillic_General_CI_AS, 'US') AND s.StateCode = o.stateCode COLLATE Cyrillic_General_CI_AS),
	2,
	2,
	o.[state],
	oo.Id
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder

GO

ALTER TABLE OrderPaymentMethods
ADD IdOrder INT NOT NULL CONSTRAINT UQ_OrderPaymentMethodsOrders UNIQUE

GO

INSERT INTO OrderPaymentMethods
(DateCreated, DateEdited, IdAddress, IdEditedBy, IdObjectType, StatusCode, IdOrder)
SELECT 
	o.DateCreated, 
	o.DateEdited, 
	a.Id, 
	o.IdEditedBy, 
	CASE [vitalchoice2.0].dbo.RegexReplace('([a-zA-Z\s.]+)\s+\|{2}', oo.paymentDetails, '$1')
		WHEN 'Authorize.Net' COLLATE Cyrillic_General_CI_AS THEN 1
		WHEN 'Credit Card' COLLATE Cyrillic_General_CI_AS THEN 1
		WHEN 'OAC' COLLATE Cyrillic_General_CI_AS THEN 2
		WHEN 'Check' COLLATE Cyrillic_General_CI_AS THEN 3
		WHEN 'No Charge' COLLATE Cyrillic_General_CI_AS THEN 4
		WHEN 'Prepaid' COLLATE Cyrillic_General_CI_AS THEN 6
		ELSE 4
	END,
	2,
	o.Id
FROM Orders AS o
INNER JOIN [vitalchoice2.0].dbo.orders AS oo ON oo.idOrder = o.Id
INNER JOIN OrderAddresses AS a ON a.IdOrder = o.Id

GO
--auto-ship template

INSERT INTO OrderPaymentMethods
(DateCreated, DateEdited, IdAddress, IdEditedBy, IdObjectType, StatusCode, IdOrder)
SELECT 
	o.DateCreated, 
	o.DateEdited, 
	a.Id, 
	o.IdEditedBy, 
	CASE [vitalchoice2.0].dbo.RegexReplace('([a-zA-Z\s.]+)\s+\|{2}', oo.paymentDetails, '$1')
		WHEN 'Authorize.Net' COLLATE Cyrillic_General_CI_AS THEN 1
		WHEN 'Credit Card' COLLATE Cyrillic_General_CI_AS THEN 1
		WHEN 'OAC' COLLATE Cyrillic_General_CI_AS THEN 2
		WHEN 'Check' COLLATE Cyrillic_General_CI_AS THEN 3
		WHEN 'No Charge' COLLATE Cyrillic_General_CI_AS THEN 4
		WHEN 'Prepaid' COLLATE Cyrillic_General_CI_AS THEN 6
		ELSE 4
	END,
	2,
	o.Id
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS oo ON oo.idOrder = aso.idOrder
INNER JOIN Orders AS o ON o.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN OrderAddresses AS a ON a.IdOrder = o.Id

GO

UPDATE Orders 
SET IdPaymentMethod = p.Id
FROM Orders AS o
INNER JOIN OrderPaymentMethods AS p ON p.IdOrder = o.Id

GO

--credit cards

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(cc.nameOnCard AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS NameOnCard, 
	CAST('XXXXXXXXXXXX' + RIGHT([vitalchoice2.0].dbo.RC4Encode(cc.cardnumber), 4) AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS CardNumber, 
	CAST(
		CASE cc.cardtype
			WHEN 'V' THEN N'2'
			WHEN 'M' THEN N'1'
			WHEN 'A' THEN N'3'
			WHEN 'D' THEN N'4'
			ELSE NULL
		END
	AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS CardType, 
	CONVERT(NVARCHAR(250), CAST(cc.expiration AS DATETIME2), 126) AS ExpDate
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = oo.Id
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 1) p
UNPIVOT (Value FOR Name IN 
	(NameOnCard, CardNumber, CardType, ExpDate)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 1)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO
--auto-ship template

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(cc.nameOnCard AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS NameOnCard, 
	CAST('XXXXXXXXXXXX' + RIGHT([vitalchoice2.0].dbo.RC4Encode(cc.cardnumber), 4) AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS CardNumber, 
	CAST(
		CASE cc.cardtype
			WHEN 'V' THEN N'2'
			WHEN 'M' THEN N'1'
			WHEN 'A' THEN N'3'
			WHEN 'D' THEN N'4'
			ELSE NULL
		END
	AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS CardType, 
	CONVERT(NVARCHAR(250), CAST(cc.expiration AS DATETIME2), 126) AS ExpDate
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = oo.Id
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 1) p
UNPIVOT (Value FOR Name IN 
	(NameOnCard, CardNumber, CardType, ExpDate)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 1)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO


--credit cards (old auth.net)

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(cc.fname + ' ' + cc.lname AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS NameOnCard, 
	CAST('XXXXXXXXXXXX' + RIGHT([vitalchoice2.0].dbo.RC4Encode(cc.ccnum), 4) AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS CardNumber, 
	CAST(
		CASE cc.cctype
			WHEN 'V' THEN N'2'
			WHEN 'M' THEN N'1'
			WHEN 'A' THEN N'3'
			WHEN 'D' THEN N'4'
			ELSE NULL
		END
	AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS CardType, 
	CONVERT(NVARCHAR(250), CAST(DATEFROMPARTS(CAST(SUBSTRING(cc.ccexp, 3, 2) AS INT) + 2000,CAST(SUBSTRING(cc.ccexp, 1, 2) AS INT), 1) AS DATETIME2), 126) AS ExpDate
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN [vitalchoice2.0].dbo.authorders AS cc ON cc.idauthorder = (SELECT TOP 1 idauthorder FROM [vitalchoice2.0].dbo.authorders AS ac WHERE ac.idOrder = o.idOrder ORDER BY idauthorder DESC)
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 1
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS oc ON oc.idOrder = o.idOrder
WHERE oc.idOrder IS NULL) p
UNPIVOT (Value FOR Name IN 
	(NameOnCard, CardNumber, CardType, ExpDate)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS t ON t.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (t.IdObjectType IS NULL OR t.IdObjectType = 1)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO
--auto-ship template

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, t.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(cc.fname + ' ' + cc.lname AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS NameOnCard, 
	CAST('XXXXXXXXXXXX' + RIGHT([vitalchoice2.0].dbo.RC4Encode(cc.ccnum), 4) AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS CardNumber, 
	CAST(
		CASE cc.cctype
			WHEN 'V' THEN N'2'
			WHEN 'M' THEN N'1'
			WHEN 'A' THEN N'3'
			WHEN 'D' THEN N'4'
			ELSE NULL
		END
	AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS CardType, 
	CONVERT(NVARCHAR(250), CAST(DATEFROMPARTS(CAST(SUBSTRING(cc.ccexp, 3, 2) AS INT) + 2000,CAST(SUBSTRING(cc.ccexp, 1, 2) AS INT), 1) AS DATETIME2), 126) AS ExpDate
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN [vitalchoice2.0].dbo.authorders AS cc ON cc.idauthorder = (SELECT TOP 1 idauthorder FROM [vitalchoice2.0].dbo.authorders AS ac WHERE ac.idOrder = o.idOrder ORDER BY idauthorder DESC)
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 1
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS oc ON oc.idOrder = o.idOrder
WHERE oc.idOrder IS NULL) p
UNPIVOT (Value FOR Name IN 
	(NameOnCard, CardNumber, CardType, ExpDate)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS t ON t.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (t.IdObjectType IS NULL OR t.IdObjectType = 1)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO
--check

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST([vitalchoice2.0].dbo.RC4Encode(cc.cardnumber) AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS CheckNumber, 
	CAST(
		CASE o.TaxIncluded
			WHEN 0 THEN N'False'
			WHEN 1 THEN N'True'
		END
	AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS PaidInFull
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = oo.Id
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 3) p
UNPIVOT (Value FOR Name IN 
	(CheckNumber, PaidInFull)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 3)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO
--auto-ship template

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST([vitalchoice2.0].dbo.RC4Encode(cc.cardnumber) AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS CheckNumber, 
	CAST(
		CASE o.TaxIncluded
			WHEN 0 THEN N'False'
			WHEN 1 THEN N'True'
		END
	AS NVARCHAR(250)) COLLATE Cyrillic_General_CI_AS AS PaidInFull
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = oo.Id
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 3) p
UNPIVOT (Value FOR Name IN 
	(CheckNumber, PaidInFull)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 3)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''
GO


--OAC

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(cc.Terms AS NVARCHAR(250)) AS Terms, 
	CAST(cc.FOB AS NVARCHAR(250)) AS Fob
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = oo.Id
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 2) p
UNPIVOT (Value FOR Name IN 
	(Terms, Fob)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO
--auto-ship template

INSERT INTO OrderPaymentMethodOptionValues
(IdOrderPaymentMethod, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(cc.Terms AS NVARCHAR(250)) AS Terms, 
	CAST(cc.FOB AS NVARCHAR(250)) AS Fob
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = oo.Id
INNER JOIN OrderPaymentMethods AS a ON a.IdOrder = oo.Id AND a.IdObjectType = 2) p
UNPIVOT (Value FOR Name IN 
	(Terms, Fob)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO

ALTER TABLE OrderPaymentMethods
DROP CONSTRAINT UQ_OrderPaymentMethodsOrders

ALTER TABLE OrderPaymentMethods
DROP COLUMN IdOrder

GO

INSERT INTO OrderAddressOptionValues
(IdOrderAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(o.address AS NVARCHAR(250)) AS Address1, 
	CAST(o.Address2 AS NVARCHAR(250)) AS Address2, 
	CAST(o.firstName AS NVARCHAR(250)) AS FirstName, 
	CAST(o.LastName AS NVARCHAR(250)) AS LastName, 
	CAST(o.company AS NVARCHAR(250)) AS Company, 
	CAST(o.City AS NVARCHAR(250)) AS City, 
	CAST(o.Zip AS NVARCHAR(250)) AS Zip,
	CAST(o.Phone AS NVARCHAR(250)) AS Phone,
	CAST(o.Fax AS NVARCHAR(250)) AS Fax
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN OrderAddresses AS a ON a.IdOrder = oo.Id) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax)
)AS unpvt
INNER JOIN AddressOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO
--auto-ship template

INSERT INTO OrderAddressOptionValues
(IdOrderAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(o.address AS NVARCHAR(250)) AS Address1, 
	CAST(o.Address2 AS NVARCHAR(250)) AS Address2, 
	CAST(o.firstName AS NVARCHAR(250)) AS FirstName, 
	CAST(o.LastName AS NVARCHAR(250)) AS LastName, 
	CAST(o.company AS NVARCHAR(250)) AS Company, 
	CAST(o.City AS NVARCHAR(250)) AS City, 
	CAST(o.Zip AS NVARCHAR(250)) AS Zip,
	CAST(o.Phone AS NVARCHAR(250)) AS Phone,
	CAST(o.Fax AS NVARCHAR(250)) AS Fax
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN OrderAddresses AS a ON a.IdOrder = oo.Id) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax)
)AS unpvt
INNER JOIN AddressOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO

ALTER TABLE OrderAddresses
DROP CONSTRAINT UQ_OrderAddressesOrders

ALTER TABLE OrderAddresses
DROP COLUMN IdOrder

GO
--shipping address

ALTER TABLE OrderAddresses
ADD IdOrder INT NULL

CREATE NONCLUSTERED INDEX IX_OrderAddressesIdOrder ON OrderAddresses (IdOrder)

GO

INSERT INTO OrderAddresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdOrder)
SELECT 
	o.orderDate, 
	o.orderDate,
	ISNULL (
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = ISNULL(o.shippingCountryCode COLLATE Cyrillic_General_CI_AS, 'US')), 
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM States AS s WHERE s.CountryCode = ISNULL(o.shippingCountryCode COLLATE Cyrillic_General_CI_AS, 'US') AND s.StateCode = o.shippingStateCode COLLATE Cyrillic_General_CI_AS),
	2,
	3,
	o.shippingState,
	o.idOrder
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON o.idOrder = oo.Id

GO

--auto-ship template address

INSERT INTO OrderAddresses
(DateCreated, DateEdited, IdCountry, IdState, StatusCode, IdObjectType, County, IdOrder)
SELECT 
	o.orderDate, 
	o.orderDate,
	ISNULL (
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = ISNULL(o.shippingCountryCode COLLATE Cyrillic_General_CI_AS, 'US')), 
		(SELECT TOP 1 cn.Id FROM Countries AS cn WHERE cn.CountryCode = 'US')
	),
	(SELECT TOP 1 s.Id FROM States AS s WHERE s.CountryCode = ISNULL(o.shippingCountryCode COLLATE Cyrillic_General_CI_AS, 'US') AND s.StateCode = o.shippingStateCode COLLATE Cyrillic_General_CI_AS),
	2,
	3,
	o.shippingState,
	oo.Id
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder

GO

UPDATE Orders
SET IdShippingAddress = a.Id
FROM Orders AS o
INNER JOIN OrderAddresses AS a ON a.IdOrder = o.Id

GO

INSERT INTO OrderAddressOptionValues
(IdOrderAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(o.shippingAddress AS NVARCHAR(250)) AS Address1, 
	CAST(o.shippingAddress2 AS NVARCHAR(250)) AS Address2, 
	CAST(CASE WHEN CHARINDEX(' ', o.ShippingFullName) > 1 THEN SUBSTRING(o.ShippingFullName, 1, CHARINDEX(' ', o.ShippingFullName)) ELSE o.ShippingFullName END AS NVARCHAR(250)) AS FirstName, 
	CAST(CASE WHEN CHARINDEX(' ', o.ShippingFullName) > 1 THEN SUBSTRING(o.ShippingFullName, CHARINDEX(' ', o.ShippingFullName) + 1, LEN(o.ShippingFullName)) ELSE NULL END AS NVARCHAR(250)) AS LastName, 
	CAST(o.shippingCompany AS NVARCHAR(250)) AS Company, 
	CAST(o.shippingCity AS NVARCHAR(250)) AS City, 
	CAST(o.shippingZip AS NVARCHAR(250)) AS Zip,
	CAST(o.pcOrd_ShippingPhone AS NVARCHAR(250)) AS Phone,
	CAST(o.pcOrd_ShippingFax AS NVARCHAR(250)) AS Fax,
	CAST(o.pcOrd_ShippingEmail AS NVARCHAR(250)) AS Email
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN OrderAddresses AS a ON a.IdOrder = oo.Id) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax, Email)
)AS unpvt
INNER JOIN AddressOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 3)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO
--auto-ship template address

INSERT INTO OrderAddressOptionValues
(IdOrderAddress, IdOptionType, Value)
SELECT unpvt.Id, o.Id, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(o.shippingAddress AS NVARCHAR(250)) AS Address1, 
	CAST(o.shippingAddress2 AS NVARCHAR(250)) AS Address2, 
	CAST(CASE WHEN CHARINDEX(' ', o.ShippingFullName) > 1 THEN SUBSTRING(o.ShippingFullName, 1, CHARINDEX(' ', o.ShippingFullName)) ELSE o.ShippingFullName END AS NVARCHAR(250)) AS FirstName, 
	CAST(CASE WHEN CHARINDEX(' ', o.ShippingFullName) > 1 THEN SUBSTRING(o.ShippingFullName, CHARINDEX(' ', o.ShippingFullName) + 1, LEN(o.ShippingFullName)) ELSE NULL END AS NVARCHAR(250)) AS LastName, 
	CAST(o.shippingCompany AS NVARCHAR(250)) AS Company, 
	CAST(o.shippingCity AS NVARCHAR(250)) AS City, 
	CAST(o.shippingZip AS NVARCHAR(250)) AS Zip,
	CAST(o.pcOrd_ShippingPhone AS NVARCHAR(250)) AS Phone,
	CAST(o.pcOrd_ShippingFax AS NVARCHAR(250)) AS Fax,
	CAST(o.pcOrd_ShippingEmail AS NVARCHAR(250)) AS Email
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS oo ON oo.IdAutoShipOrder = aso.idAutoShipOrder
INNER JOIN OrderAddresses AS a ON a.IdOrder = oo.Id) p
UNPIVOT (Value FOR Name IN 
	(Address1, Address2, FirstName, LastName, Company, City, Zip, Phone, Fax, Email)
)AS unpvt
INNER JOIN AddressOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 3)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N''

GO

DROP INDEX IX_OrderAddressesIdOrder ON OrderAddresses

ALTER TABLE OrderAddresses
DROP COLUMN IdOrder

GO

INSERT INTO OrderToGiftCertificates
(IdOrder, IdGiftCertificate, Amount)
SELECT o.Id, (SELECT TOP 1 g.Id FROM GiftCertificates AS g WHERE g.Code = [vitalchoice2.0].dbo.RegexReplace('(?<code>[^\|]+)(\|[sS]\|)(?<description>[^\|]*)(\|[sS]\|)(?<amount>[0-9]+(\.[0-9]*)?)', gc.Item, '${code}') COLLATE Cyrillic_General_CI_AS), CONVERT(MONEY, [vitalchoice2.0].dbo.RegexReplace('(?<code>[^\|]+)(\|[sS]\|)(?<description>[^\|]*)(\|[sS]\|)(?<amount>[0-9]+(\.[0-9]*)?)', gc.Item, '${amount}')) FROM Orders AS o
INNER JOIN [vitalchoice2.0].dbo.orders AS oo ON oo.idOrder = o.Id
CROSS APPLY [vitalchoice2.0].[dbo].[DelimitedSplit8K](oo.pcOrd_GCDetails, '|g|') AS gc
WHERE oo.pcOrd_GCDetails IS NOT NULL AND oo.pcOrd_GCDetails <> '' AND gc.Item IS NOT NULL AND gc.Item <> N'' AND (SELECT TOP 1 g.Id FROM GiftCertificates AS g WHERE g.Code = [vitalchoice2.0].dbo.RegexReplace('(?<code>[^\|]+)(\|[sS]\|)(?<description>[^\|]*)(\|[sS]\|)(?<amount>[0-9]+(\.[0-9]*)?)', gc.Item, '${code}') COLLATE Cyrillic_General_CI_AS) IS NOT NULL

GO

INSERT INTO OrderToSkus
(IdOrder, IdSku, Amount, Quantity)
SELECT po.IdOrder, po.IdProduct, MAX(ISNULL(unitprice, 0)), SUM(quantity) FROM [vitalchoice2.0].[dbo].ProductsOrdered AS po
INNER JOIN Orders AS o ON po.idOrder = o.Id
INNER JOIN Skus AS s ON s.Id = po.IdProduct
WHERE po.quantity > 0
GROUP BY po.IdOrder, po.IdProduct

GO

INSERT INTO OrderToSkus
(IdOrder, IdSku, Amount, Quantity)
SELECT po.IdOrder, s.Id, MAX(ISNULL(unitprice, 0)), SUM(quantity) FROM [vitalchoice2.0].[dbo].ProductsOrdered AS po
INNER JOIN Orders AS o ON po.idOrder = o.Id
INNER JOIN [vitalchoice2.0].dbo.products AS p ON p.IdProduct = po.idProduct
INNER JOIN Skus AS s ON s.StatusCode <> 3 AND s.Code = p.sku COLLATE Cyrillic_General_CI_AS
WHERE po.quantity > 0 AND po.IdProduct NOT IN (SELECT Id FROM SKus)
GROUP BY po.IdOrder, s.Id

GO

INSERT INTO OrderToSkus
(IdOrder, IdSku, Amount, Quantity)
SELECT o.Id, po.IdProduct, MAX(ISNULL(unitprice, 0)), SUM(quantity) 
FROM [vitalchoice2.0].[dbo].AutoshipOrders AS aso
INNER JOIN Orders AS o ON o.IdAutoShipOrder = aso.IdAutoShipOrder
INNER JOIN [vitalchoice2.0].[dbo].orders AS oo ON oo.IdOrder = aso.IdOrder
INNER JOIN [vitalchoice2.0].[dbo].ProductsOrdered AS po ON po.IdOrder = oo.IdOrder
INNER JOIN Skus AS s ON s.Id = po.IdProduct
WHERE po.quantity > 0
GROUP BY o.Id, po.IdProduct

GO

INSERT INTO OrderToSkus
(IdOrder, IdSku, Amount, Quantity)
SELECT o.Id, s.Id, MAX(ISNULL(unitprice, 0)), SUM(quantity) 
FROM [vitalchoice2.0].[dbo].AutoshipOrders AS aso
INNER JOIN Orders AS o ON o.IdAutoShipOrder = aso.IdAutoShipOrder
INNER JOIN [vitalchoice2.0].[dbo].orders AS oo ON oo.IdOrder = aso.IdOrder
INNER JOIN [vitalchoice2.0].[dbo].ProductsOrdered AS po ON po.IdOrder = oo.IdOrder
INNER JOIN [vitalchoice2.0].dbo.products AS p ON p.IdProduct = po.idProduct
INNER JOIN Skus AS s ON s.StatusCode <> 3 AND s.Code = p.sku COLLATE Cyrillic_General_CI_AS
WHERE po.quantity > 0 AND po.IdProduct NOT IN (SELECT Id FROM SKus)
GROUP BY o.Id, s.Id

GO

DROP INDEX IX_OrdersIdAutoShipOrder ON Orders

ALTER TABLE Orders
DROP COLUMN IdAutoShipOrder

GO

INSERT INTO RefundSkus
(IdOrder, IdSku, Quantity, Redeem, RefundPercent, RefundPrice, RefundValue)
SELECT o.Id, s.Id, ri.Qty, 1, ri.ProductPercent, ri.Value / (ri.ProductPercent / 100.0), ri.Value FROM Orders AS o
INNER JOIN [vitalchoice2.0].dbo.refund AS r ON r.idOrder = o.Id
INNER JOIN [vitalchoice2.0].dbo.RefundItems AS ri ON ri.ItemType = 1 AND ri.IdRefundOrder = o.Id
INNER JOIN Skus AS s ON s.Id = ri.IdItem
WHERE o.IdObjectType = 6

GO

INSERT INTO RefundSkus
(IdOrder, IdSku, Quantity, Redeem, RefundPercent, RefundPrice, RefundValue)
SELECT o.Id, s.Id, ri.Qty, 1, ri.ProductPercent, ri.Value / (ri.ProductPercent / 100.0), ri.Value FROM Orders AS o
INNER JOIN [vitalchoice2.0].dbo.refund AS r ON r.idOrder = o.Id
INNER JOIN [vitalchoice2.0].dbo.RefundItems AS ri ON ri.ItemType = 1 AND ri.IdRefundOrder = o.Id
INNER JOIN [vitalchoice2.0].dbo.products AS p ON p.IdProduct = ri.IdItem
INNER JOIN Skus AS s ON s.StatusCode <> 3 AND s.Code = p.sku COLLATE Cyrillic_General_CI_AS
WHERE o.IdObjectType = 6 AND ri.IdItem NOT IN (SELECT Id FROM SKus)

GO

SET IDENTITY_INSERT HealthwisePeriods ON;

INSERT INTO HealthwisePeriods
(Id, IdCustomer, PaidAmount, PaidDate, StartDate, EndDate)
SELECT 
	hw.id, 
	hw.customerId, 
	hw.billAmount, 
	hw.BillDate, 
	hw.date, 
	CASE WHEN hw.BillDate IS NOT NULL AND DATEADD(YEAR, 1, hw.date) > hw.BillDate THEN hw.BillDate ELSE DATEADD(YEAR, 1, hw.date) END 
FROM [vitalchoice2.0].dbo.healthwise AS hw
INNER JOIN Customers AS c ON c.Id = hw.customerId
WHERE hw.date IS NOT NULL

SET IDENTITY_INSERT HealthwisePeriods OFF;

INSERT INTO HealthwiseOrders
(Id, IdHealthwisePeriod)
SELECT o.Id, hw.Id FROM Orders AS o
INNER JOIN [vitalchoice2.0].dbo.orders AS oo ON oo.idOrder = o.Id
INNER JOIN HealthwisePeriods AS hw ON hw.Id = oo.healthwiseId

GO

ALTER TABLE AffiliatePayments
ADD IdOrder INT NOT NULL 
CONSTRAINT UQ_AffiliatePaymentsOrder UNIQUE (IdOrder)

GO

INSERT INTO AffiliatePayments
(IdAffiliate, Amount, DateCreated, IdOrder)
SELECT a.Id, o.affiliatePayReport, o.orderDate, oo.Id 
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.IdOrder
INNER JOIN Affiliates AS a ON a.Id = o.idAffiliate
WHERE o.affiliatePayReport > o.affiliatePay

GO

INSERT INTO AffiliateOrderPayments
(Id, IdAffiliate, IdAffiliatePayment, Amount, NewCustomerOrder, Status)
SELECT 
	oo.Id, 
	a.Id, 
	ap.Id, 
	ISNULL(o.affiliatePay, o.affiliatePayReport), 
	(SELECT CASE WHEN o.idOrder <=
	ALL(
		SELECT idOrder 
		FROM [vitalchoice2.0].dbo.orders AS oox 
		WHERE oox.idCustomer = o.idCustomer
		AND oox.idAffiliate = o.idAffiliate
		AND oox.orderStatus NOT IN (1,5,96,97)
		AND oox.IsReship = 0
		AND oox.IsRefund = 0
	) THEN 1 ELSE 0 END), 
	CASE WHEN ap.Id IS NULL THEN 1 ELSE 2 END
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.IdOrder
INNER JOIN Affiliates AS a ON a.Id = o.idAffiliate
LEFT JOIN AffiliatePayments AS ap ON ap.IdOrder = oo.Id

GO

ALTER TABLE AffiliatePayments
DROP CONSTRAINT UQ_AffiliatePaymentsOrder

ALTER TABLE AffiliatePayments
DROP COLUMN IdOrder

GO

UPDATE GiftCertificates
SET IdOrder = o.Id
FROM GiftCertificates AS g
INNER JOIN [vitalchoice2.0].dbo.pcGCOrdered AS gc ON gc.pcGO_GcCode COLLATE Cyrillic_General_CI_AS = g.Code
INNER JOIN Orders AS o ON o.Id = gc.pcGO_IDOrder

GO

DELETE v
FROM OrderOptionValues AS v
INNER JOIN OrderOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.DefaultValue = v.Value

GO

INSERT INTO OrderOptionValues
(IdOrder, IdOptionType, Value)
SELECT c.Id, t.Id, t.DefaultValue FROM Orders AS c
INNER JOIN OrderOptionTypes AS t ON t.IdObjectType = c.IdObjectType OR t.IdObjectType IS NULL
WHERE t.DefaultValue IS NOT NULL AND NOT EXISTS(SELECT * FROM OrderOptionValues AS v WHERE v.IdOrder = c.Id AND v.IdOptionType = t.Id)

GO

INSERT INTO OrderShippingPackages
(IdOrder, IdSku, DateCreated, IdWarehouse, POrderType, ShipMethodFreightCarrier, ShipMethodFreightService, ShippedDate, TrackingNumber, UPSServiceCode)
SELECT 
	pki.idOrder, 
	s.Id, 
	MAX(pki.pcPackageInfo_ShippedDate), 
	CASE WHEN MAX(pki.Warehouse) = 'VA' THEN 2 ELSE 1 END, 
	NULL, 
	(SELECT TOP 1 k.Item FROM [vitalchoice2.0].[dbo].[DelimitedSplit8K](MAX(pki.pcPackageInfo_ShipMethod), ' - ') AS k ORDER BY k.ItemNumber ASC), 
	(SELECT TOP 1 k.Item FROM [vitalchoice2.0].[dbo].[DelimitedSplit8K](MAX(pki.pcPackageInfo_ShipMethod), ' - ') AS k ORDER BY k.ItemNumber DESC),
	MAX(pki.pcPackageInfo_ShippedDate), 
	MAX(pki.pcPackageInfo_TrackingNumber),
	MAX(pki.pcPackageInfo_UPSServiceCode)
FROM [vitalchoice2.0].dbo.pcPackageInfo AS pki
INNER JOIN [vitalchoice2.0].dbo.ProductsOrdered AS po ON po.idOrder = pki.idOrder
INNER JOIN [vitalchoice2.0].dbo.products AS p ON p.idProduct = po.idProduct AND p.sku = pki.pcPackageInfo_UPSServiceCode
INNER JOIN Skus AS s ON s.Id = p.idProduct
WHERE pki.pcPackageInfo_ShippedDate IS NOT NULL
GROUP BY pki.idOrder, s.Id