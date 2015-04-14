IF (( SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [name] = N'Order' AND [object_id] = OBJECT_ID(N'ContentCategories')) = 0
AND (SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [object_id] = OBJECT_ID(N'ContentCategories')) = 1)
BEGIN
ALTER TABLE ContentCategories ADD [Order] int NULL

END

GO

IF((SELECT TOP 1 COLUMNPROPERTY(OBJECT_ID('ContentCategories', 'U'), 'Order', 'AllowsNull'))=1)
BEGIN
UPDATE ContentCategories
SET [Order]=0

ALTER TABLE ContentCategories ALTER COLUMN [Order] int NOT NULL
END