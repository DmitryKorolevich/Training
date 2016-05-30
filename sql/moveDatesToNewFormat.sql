UPDATE AddressOptionValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM AddressOptionValues AS v
INNER JOIN AddressOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6

UPDATE OrderOptionValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM OrderOptionValues AS v
INNER JOIN OrderOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6

UPDATE AffiliateOptionValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM AffiliateOptionValues AS v
INNER JOIN AffiliateOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6

UPDATE CustomerNoteOptionValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM CustomerNoteOptionValues AS v
INNER JOIN CustomerNoteOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6

UPDATE CustomerOptionValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM CustomerOptionValues AS v
INNER JOIN CustomerOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6

UPDATE CustomerPaymentMethodValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM CustomerPaymentMethodValues AS v
INNER JOIN CustomerPaymentMethodOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6

UPDATE DiscountOptionValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM DiscountOptionValues AS v
INNER JOIN DiscountOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6

UPDATE InventorySkuOptionValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM InventorySkuOptionValues AS v
INNER JOIN InventorySkuOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6

UPDATE OrderAddressOptionValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM OrderAddressOptionValues AS v
INNER JOIN AddressOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6

UPDATE OrderPaymentMethodOptionValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM OrderPaymentMethodOptionValues AS v
INNER JOIN CustomerPaymentMethodOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6

UPDATE ProductOptionValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM ProductOptionValues AS v
INNER JOIN ProductOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6

UPDATE PromotionOptionValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM PromotionOptionValues AS v
INNER JOIN PromotionOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6

UPDATE SkuOptionValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM SkuOptionValues AS v
INNER JOIN ProductOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6

UPDATE SkuOptionValues
SET Value = CONVERT(NVARCHAR(250), CONVERT(DATETIME2, Value, 101), 126)
FROM SkuOptionValues AS v
INNER JOIN ProductOptionTypes AS t ON t.Id = v.IdOptionType
WHERE t.IdFieldType = 6