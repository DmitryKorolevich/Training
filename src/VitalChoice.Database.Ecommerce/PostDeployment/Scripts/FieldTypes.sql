IF OBJECT_ID(N'[dbo].[FieldTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[FieldTypes]
	(
		[Id] INT NOT NULL PRIMARY KEY, 
		[TypeName] NVARCHAR(250) NOT NULL
	)

	INSERT INTO FieldTypes
	(Id, TypeName)
	SELECT 1, 'decimal'
	UNION
	SELECT 2, 'double'
	UNION
	SELECT 3, 'int'
	UNION
	SELECT 4, 'string'
END