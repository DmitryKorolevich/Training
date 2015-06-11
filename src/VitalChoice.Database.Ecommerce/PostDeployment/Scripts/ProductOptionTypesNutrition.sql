IF NOT EXISTS(SELECT * FROM ProductOptionTypes WHERE IdProductType=2 AND Name='NutritionalTitle')
BEGIN

--non-perishable product type

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name)
	SELECT NULL, 4, 1, N'ServingSize'
	UNION
	SELECT NULL, 4, 1, N'Servings'
	UNION
	SELECT NULL, 4, 1, N'Calories'
	UNION
	SELECT NULL, 4, 1, N'CaloriesFromFat'
	UNION
	SELECT NULL, 4, 1, N'TotalFat'
	UNION
	SELECT NULL, 4, 1, N'TotalFatPercent'
	UNION
	SELECT NULL, 4, 1, N'SaturatedFat'
	UNION
	SELECT NULL, 4, 1, N'SaturatedFatPercent'
	UNION
	SELECT NULL, 4, 1, N'TransFat'
	UNION
	SELECT NULL, 4, 1, N'TransFatPercent'
	UNION
	SELECT NULL, 4, 1, N'Cholesterol'
	UNION
	SELECT NULL, 4, 1, N'CholesterolPercent'
	UNION
	SELECT NULL, 4, 1, N'Sodium'
	UNION
	SELECT NULL, 4, 1, N'SodiumPercent'
	UNION
	SELECT NULL, 4, 1, N'TotalCarbohydrate'
	UNION
	SELECT NULL, 4, 1, N'TotalCarbohydratePercent'
	UNION
	SELECT NULL, 4, 1, N'DietaryFiber'
	UNION
	SELECT NULL, 4, 1, N'DietaryFiberPercent'
	UNION
	SELECT NULL, 4, 1, N'Sugars'
	UNION
	SELECT NULL, 4, 1, N'SugarsPercent'
	UNION
	SELECT NULL, 4, 1, N'Protein'
	UNION
	SELECT NULL, 4, 1, N'ProteinPercent'
	UNION
	SELECT NULL, 4, 1, N'AdditionalNotes'


--perishable product type

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name)
	SELECT NULL, 4, 2, N'ServingSize'
	UNION
	SELECT NULL, 4, 2, N'Servings'
	UNION
	SELECT NULL, 4, 2, N'Calories'
	UNION
	SELECT NULL, 4, 2, N'CaloriesFromFat'
	UNION
	SELECT NULL, 4, 2, N'TotalFat'
	UNION
	SELECT NULL, 4, 2, N'TotalFatPercent'
	UNION
	SELECT NULL, 4, 2, N'SaturatedFat'
	UNION
	SELECT NULL, 4, 2, N'SaturatedFatPercent'
	UNION
	SELECT NULL, 4, 2, N'TransFat'
	UNION
	SELECT NULL, 4, 2, N'TransFatPercent'
	UNION
	SELECT NULL, 4, 2, N'Cholesterol'
	UNION
	SELECT NULL, 4, 2, N'CholesterolPercent'
	UNION
	SELECT NULL, 4, 2, N'Sodium'
	UNION
	SELECT NULL, 4, 2, N'SodiumPercent'
	UNION
	SELECT NULL, 4, 2, N'TotalCarbohydrate'
	UNION
	SELECT NULL, 4, 2, N'TotalCarbohydratePercent'
	UNION
	SELECT NULL, 4, 2, N'DietaryFiber'
	UNION
	SELECT NULL, 4, 2, N'DietaryFiberPercent'
	UNION
	SELECT NULL, 4, 2, N'Sugars'
	UNION
	SELECT NULL, 4, 2, N'SugarsPercent'
	UNION
	SELECT NULL, 4, 2, N'Protein'
	UNION
	SELECT NULL, 4, 2, N'ProteinPercent'
	UNION
	SELECT NULL, 4, 2, N'AdditionalNotes'
	

END