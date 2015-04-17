DECLARE @contentTypes TABLE (Id INT PRIMARY KEY, Name NVARCHAR(250));

INSERT INTO @contentTypes
(Id, Name)
SELECT 1 AS Id, 'Recipe Category' AS Name
UNION
SELECT 2 AS Id, 'Recipe' AS Name
UNION
SELECT 3 AS Id, 'Article Category' AS Name
UNION
SELECT 4 AS Id, 'Article' AS Name
UNION
SELECT 5 AS Id, 'FAQ Category' AS Name
UNION
SELECT 6 AS Id, 'FAQ' AS Name
UNION
SELECT 7 AS Id, 'Content Category' AS Name
UNION
SELECT 8 AS Id, 'Content' AS Name

INSERT INTO ContentTypes
(Id, Name)
SELECT Id, Name FROM @contentTypes AS ct
WHERE ct.Id NOT IN (SELECT Id FROM ContentTypes)