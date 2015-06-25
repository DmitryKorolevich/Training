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
GO
IF NOT EXISTS(SELECT * FROM FieldTypes WHERE TypeName = 'bool')
	INSERT INTO FieldTypes
	(Id, TypeName)
	VALUES
	(5, 'bool')
GO
IF NOT EXISTS(SELECT * FROM FieldTypes WHERE TypeName = 'DateTime')
	INSERT INTO FieldTypes
	(Id, TypeName)
	VALUES
	(6, 'DateTime'),
	(7, 'long'),
	(8, 'LargeString')