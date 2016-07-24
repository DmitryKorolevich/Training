USE [VitalChoice.Ecommerce]
GO

TRUNCATE TABLE [VitalChoice.ExportInfo].dbo.CustomerPaymentMethods
TRUNCATE TABLE [VitalChoice.ExportInfo].dbo.OrderPaymentMethods

INSERT INTO [VitalChoice.ExportInfo].dbo.OrderPaymentMethods
(IdOrder, CreditCardNumber)
SELECT 
	oo.Id, 
	CAST([vitalchoice2.0].dbo.RC4Encode(cc.cardnumber) AS VARBINARY)
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN OrderPaymentMethods AS a ON a.Id = oo.IdPaymentMethod AND a.IdObjectType = 1
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
WHERE cc.cardnumber IS NOT NULL
PRINT '====credit cards'

GO

INSERT INTO [VitalChoice.ExportInfo].dbo.OrderPaymentMethods
(IdOrder, CreditCardNumber)
SELECT 
	oo.Id, 
	CAST([vitalchoice2.0].dbo.RC4Encode(cc.cardnumber) AS VARBINARY)
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS os ON os.Id = o.idOrder
INNER JOIN Orders AS oo ON oo.Id = os.IdOrderSource
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
INNER JOIN OrderPaymentMethods AS a ON a.Id = oo.IdPaymentMethod AND a.IdObjectType = 1
WHERE cc.cardnumber IS NOT NULL
PRINT '====credit cards(auto-ship)'
GO

INSERT INTO [VitalChoice.ExportInfo].dbo.OrderPaymentMethods
(IdOrder, CreditCardNumber)
SELECT 
	oo.Id, 
	CAST([vitalchoice2.0].dbo.RC4Encode(cc.ccnum) AS VARBINARY)
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN OrderPaymentMethods AS a ON a.Id = oo.IdPaymentMethod AND a.IdObjectType = 1
INNER JOIN [vitalchoice2.0].dbo.authorders AS cc ON cc.idauthorder = (SELECT TOP 1 idauthorder FROM [vitalchoice2.0].dbo.authorders AS ac WHERE ac.idOrder = o.idOrder ORDER BY idauthorder DESC)
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS oc ON oc.idOrder = o.idOrder
WHERE oc.idOrder IS NULL AND cc.ccnum IS NOT NULL
PRINT '====credit cards'

GO

INSERT INTO [VitalChoice.ExportInfo].dbo.OrderPaymentMethods
(IdOrder, CreditCardNumber)
SELECT 
	oo.Id, 
	CAST([vitalchoice2.0].dbo.RC4Encode(cc.ccnum) AS VARBINARY)
FROM [vitalchoice2.0].dbo.autoshipOrders AS aso
INNER JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = aso.idOrder
INNER JOIN Orders AS os ON os.Id = o.idOrder
INNER JOIN Orders AS oo ON oo.Id = os.IdOrderSource
INNER JOIN OrderPaymentMethods AS a ON a.Id = oo.IdPaymentMethod AND a.IdObjectType = 1
INNER JOIN [vitalchoice2.0].dbo.authorders AS cc ON cc.idauthorder = (SELECT TOP 1 idauthorder FROM [vitalchoice2.0].dbo.authorders AS ac WHERE ac.idOrder = o.idOrder ORDER BY idauthorder DESC)
LEFT JOIN [vitalchoice2.0].dbo.creditCards AS oc ON oc.idOrder = o.idOrder
WHERE oc.idOrder IS NULL AND cc.ccnum IS NOT NULL
PRINT '====credit cards(auto-ship)'