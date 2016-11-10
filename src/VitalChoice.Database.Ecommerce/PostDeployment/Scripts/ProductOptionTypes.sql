IF NOT EXISTS(SELECT * FROM ProductOptionTypes WHERE IdObjectType=2 AND Name='Shellfish')
BEGIN

	INSERT INTO [dbo].[ProductOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'Shellfish',5, NULL, 2, 'False')

END
GO