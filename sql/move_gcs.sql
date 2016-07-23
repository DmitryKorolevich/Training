DELETE FROM CartToGiftCertificates
DELETE FROM RefundOrderToGiftCertificates
DELETE FROM OrderToGiftCertificates
DELETE FROM GiftCertificates
DELETE FROM ObjectHistoryLogItems
DELETE FROM ObjectHistoryLogDataItems

GO

INSERT INTO GiftCertificates
(Code, Created, Balance, Email, FirstName, LastName, GCType, IdOrder, IdSku, StatusCode, PublicId)
SELECT 
	g.pcGO_GcCode, 
	ISNULL(g.DateCreated, '2013-01-01'), 
	g.pcGO_Amount, 
	CASE WHEN o.pcOrd_GcReEmail = N'' THEN NULL ELSE o.pcOrd_GcReEmail END,
	CASE WHEN CHARINDEX(' ', o.pcOrd_GcReName, 0) > 1 THEN SUBSTRING(o.pcOrd_GcReName, 0, CHARINDEX(' ', o.pcOrd_GcReName, 0) + 1) ELSE o.pcOrd_GcReName END,
	CASE WHEN CHARINDEX(' ', o.pcOrd_GcReName, 0) > 1 THEN SUBSTRING(o.pcOrd_GcReName, CHARINDEX(' ', o.pcOrd_GcReName, 0), LEN(o.pcOrd_GcReName)) ELSE NULL END,
	CASE WHEN s.Id IS NULL THEN 1 ELSE CASE WHEN pp.IdObjectType = 3 THEN 2 ELSE 3 END END, 
	NULL, 
	s.Id,
	CASE g.pcGO_Status WHEN 0 THEN 1 ELSE 2 END,
	NEWID()
FROM [vitalchoice2.0].dbo.pcGCOrdered AS g
LEFT JOIN [vitalchoice2.0].dbo.products AS p ON g.pcGO_IDProduct = p.idProduct
LEFT JOIN Skus AS s ON s.Id = g.pcGO_IDProduct
LEFT JOIN Products AS pp ON pp.Id = s.IdProduct
LEFT JOIN [vitalchoice2.0].dbo.orders AS o ON o.idOrder = g.pcGO_IDOrder