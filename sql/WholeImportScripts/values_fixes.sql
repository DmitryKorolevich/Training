UPDATE v
SET Value = vv.Value
FROM OrderPaymentMethodOptionValues AS v
INNER JOIN (SELECT unpvt.Id AS IdOrder, o.Id AS IdOptionType, unpvt.Value FROM
(
SELECT 
	a.Id, 
	CAST(ISNULL(termslv.Id, 1) AS NVARCHAR(250)) AS Terms, 
	CAST(ISNULL(foblv.Id, 1) AS NVARCHAR(250)) AS Fob
FROM [vitalchoice2.0].dbo.orders AS o
INNER JOIN Orders AS oo ON oo.Id = o.idOrder
INNER JOIN [vitalchoice2.0].dbo.creditCards AS cc ON cc.idOrder = o.IdOrder
INNER JOIN OrderPaymentMethods AS a ON a.Id = oo.IdPaymentMethod AND a.IdObjectType = 2
INNER JOIN Lookups AS termsl ON termsl.Name = 'Terms'
LEFT JOIN LookupVariants AS termslv ON termslv.IdLookup = termsl.Id AND termslv.ValueVariant = cc.Terms COLLATE Cyrillic_General_CI_AS
INNER JOIN Lookups AS fobl ON fobl.Name = 'Fob'
LEFT JOIN LookupVariants AS foblv ON foblv.IdLookup = fobl.Id AND foblv.ValueVariant = cc.FOB COLLATE Cyrillic_General_CI_AS
) p
UNPIVOT (Value FOR Name IN 
	(Terms, Fob)
)AS unpvt
INNER JOIN CustomerPaymentMethodOptionTypes AS o ON o.Name = unpvt.Name COLLATE Cyrillic_General_CI_AS AND (o.IdObjectType IS NULL OR o.IdObjectType = 2)
WHERE unpvt.Value IS NOT NULL AND unpvt.Value <> N'') AS vv ON vv.IdOptionType = v.IdOptionType AND v.IdOrderPaymentMethod = vv.IdOrder

GO

MERGE INTO OrderOptionValues AS tv
USING (
	SELECT c.Id AS IdOrder, t.Id, t.DefaultValue FROM Orders AS c
	INNER JOIN OrderOptionTypes AS t ON t.IdObjectType = c.IdObjectType OR t.IdObjectType IS NULL
	WHERE t.DefaultValue IS NOT NULL
) AS src ON tv.IdOrder = src.IdOrder AND tv.IdOptionType = src.Id
WHEN NOT MATCHED THEN
INSERT (IdOrder, IdOptionType, Value) VALUES (src.IdOrder, src.Id, src.DefaultValue);
PRINT '====default values fixup (insert)'