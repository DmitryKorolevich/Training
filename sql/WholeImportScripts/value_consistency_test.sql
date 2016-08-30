SELECT v.Value, t.Name, t.Id, t.DefaultValue, t.IdFieldType FROM CustomerNoteOptionValues AS v
INNER JOIN CustomerNoteOptionTypes AS t ON t.Id = v.IdOptionType
WHERE v.Value IS NOT NULL AND 
	CASE 
		WHEN t.IdFieldType = 1 AND TRY_CONVERT(money, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 2 AND TRY_CONVERT(float, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 3 AND TRY_CONVERT(int, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 5 AND v.Value <> N'True' AND v.Value <> N'False' THEN 1
		WHEN t.IdFieldType = 6 AND TRY_CONVERT(datetime2, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 7 AND TRY_CONVERT(bigint, v.Value) IS NULL THEN 1
	ELSE 0
	END = 1

SELECT v.Value, t.Name, t.Id, t.DefaultValue, t.IdFieldType FROM CustomerOptionValues AS v
INNER JOIN CustomerOptionTypes AS t ON t.Id = v.IdOptionType
WHERE v.Value IS NOT NULL AND 
	CASE 
		WHEN t.IdFieldType = 1 AND TRY_CONVERT(money, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 2 AND TRY_CONVERT(float, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 3 AND TRY_CONVERT(int, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 5 AND v.Value <> N'True' AND v.Value <> N'False' THEN 1
		WHEN t.IdFieldType = 6 AND TRY_CONVERT(datetime2, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 7 AND TRY_CONVERT(bigint, v.Value) IS NULL THEN 1
	ELSE 0
	END = 1

SELECT v.Value, t.Name, t.Id, t.DefaultValue, t.IdFieldType FROM DiscountOptionValues AS v
INNER JOIN DiscountOptionTypes AS t ON t.Id = v.IdOptionType
WHERE v.Value IS NOT NULL AND 
	CASE 
		WHEN t.IdFieldType = 1 AND TRY_CONVERT(money, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 2 AND TRY_CONVERT(float, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 3 AND TRY_CONVERT(int, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 5 AND v.Value <> N'True' AND v.Value <> N'False' THEN 1
		WHEN t.IdFieldType = 6 AND TRY_CONVERT(datetime2, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 7 AND TRY_CONVERT(bigint, v.Value) IS NULL THEN 1
	ELSE 0
	END = 1

SELECT v.Value, t.Name, t.Id, t.DefaultValue, t.IdFieldType FROM InventorySkuOptionValues AS v
INNER JOIN InventorySkuOptionTypes AS t ON t.Id = v.IdOptionType
WHERE v.Value IS NOT NULL AND 
	CASE 
		WHEN t.IdFieldType = 1 AND TRY_CONVERT(money, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 2 AND TRY_CONVERT(float, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 3 AND TRY_CONVERT(int, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 5 AND v.Value <> N'True' AND v.Value <> N'False' THEN 1
		WHEN t.IdFieldType = 6 AND TRY_CONVERT(datetime2, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 7 AND TRY_CONVERT(bigint, v.Value) IS NULL THEN 1
	ELSE 0
	END = 1

SELECT v.Value, t.Name, t.Id, t.DefaultValue, t.IdFieldType FROM OrderAddressOptionValues AS v
INNER JOIN AddressOptionTypes AS t ON t.Id = v.IdOptionType
WHERE v.Value IS NOT NULL AND 
	CASE 
		WHEN t.IdFieldType = 1 AND TRY_CONVERT(money, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 2 AND TRY_CONVERT(float, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 3 AND TRY_CONVERT(int, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 5 AND v.Value <> N'True' AND v.Value <> N'False' THEN 1
		WHEN t.IdFieldType = 6 AND TRY_CONVERT(datetime2, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 7 AND TRY_CONVERT(bigint, v.Value) IS NULL THEN 1
	ELSE 0
	END = 1

SELECT v.Value, t.Name, t.Id, t.DefaultValue, t.IdFieldType FROM OrderOptionValues AS v
INNER JOIN OrderOptionTypes AS t ON t.Id = v.IdOptionType
WHERE v.Value IS NOT NULL AND 
	CASE 
		WHEN t.IdFieldType = 1 AND TRY_CONVERT(money, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 2 AND TRY_CONVERT(float, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 3 AND TRY_CONVERT(int, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 5 AND v.Value <> N'True' AND v.Value <> N'False' THEN 1
		WHEN t.IdFieldType = 6 AND TRY_CONVERT(datetime2, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 7 AND TRY_CONVERT(bigint, v.Value) IS NULL THEN 1
	ELSE 0
	END = 1

SELECT v.Value, t.Name, t.Id, t.DefaultValue, t.IdFieldType FROM OrderPaymentMethodOptionValues AS v
INNER JOIN CustomerPaymentMethodOptionTypes AS t ON t.Id = v.IdOptionType
WHERE v.Value IS NOT NULL AND 
	CASE 
		WHEN t.IdFieldType = 1 AND TRY_CONVERT(money, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 2 AND TRY_CONVERT(float, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 3 AND TRY_CONVERT(int, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 5 AND v.Value <> N'True' AND v.Value <> N'False' THEN 1
		WHEN t.IdFieldType = 6 AND TRY_CONVERT(datetime2, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 7 AND TRY_CONVERT(bigint, v.Value) IS NULL THEN 1
	ELSE 0
	END = 1

SELECT v.Value, t.Name, t.Id, t.DefaultValue, t.IdFieldType FROM ProductOptionValues AS v
INNER JOIN ProductOptionTypes AS t ON t.Id = v.IdOptionType
WHERE v.Value IS NOT NULL AND 
	CASE 
		WHEN t.IdFieldType = 1 AND TRY_CONVERT(money, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 2 AND TRY_CONVERT(float, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 3 AND TRY_CONVERT(int, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 5 AND v.Value <> N'True' AND v.Value <> N'False' THEN 1
		WHEN t.IdFieldType = 6 AND TRY_CONVERT(datetime2, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 7 AND TRY_CONVERT(bigint, v.Value) IS NULL THEN 1
	ELSE 0
	END = 1

SELECT v.Value, t.Name, t.Id, t.DefaultValue, t.IdFieldType FROM PromotionOptionValues AS v
INNER JOIN PromotionOptionTypes AS t ON t.Id = v.IdOptionType
WHERE v.Value IS NOT NULL AND 
	CASE 
		WHEN t.IdFieldType = 1 AND TRY_CONVERT(money, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 2 AND TRY_CONVERT(float, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 3 AND TRY_CONVERT(int, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 5 AND v.Value <> N'True' AND v.Value <> N'False' THEN 1
		WHEN t.IdFieldType = 6 AND TRY_CONVERT(datetime2, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 7 AND TRY_CONVERT(bigint, v.Value) IS NULL THEN 1
	ELSE 0
	END = 1

SELECT v.Value, t.Name, t.Id, t.DefaultValue, t.IdFieldType FROM SkuOptionValues AS v
INNER JOIN SkuOptionTypes AS t ON t.Id = v.IdOptionType
WHERE v.Value IS NOT NULL AND 
	CASE 
		WHEN t.IdFieldType = 1 AND TRY_CONVERT(money, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 2 AND TRY_CONVERT(float, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 3 AND TRY_CONVERT(int, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 5 AND v.Value <> N'True' AND v.Value <> N'False' THEN 1
		WHEN t.IdFieldType = 6 AND TRY_CONVERT(datetime2, v.Value) IS NULL THEN 1
		WHEN t.IdFieldType = 7 AND TRY_CONVERT(bigint, v.Value) IS NULL THEN 1
	ELSE 0
	END = 1