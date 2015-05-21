IF OBJECT_ID(N'[dbo].[ProductTypes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProductTypes]
	(
		[Id] INT NOT NULL PRIMARY KEY, 
		[Name] NVARCHAR(50) NOT NULL
	);

	INSERT INTO ProductTypes
	(Id, Name)
	SELECT 1, 'Non-Perishable'
	UNION
	SELECT 2, 'Perishable'
	UNION
	SELECT 3, 'Electronic Gift Certificate'
	UNION
	SELECT 4, 'Paper Gift Certificate'
END