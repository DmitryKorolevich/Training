IF((SELECT TOP 1 COLUMNPROPERTY(OBJECT_ID('ContentItems', 'U'), 'MetaKeywords', 'AllowsNull'))=1)
BEGIN
UPDATE ContentItems
SET [Description]=''
ALTER TABLE ContentItems ALTER COLUMN [Description] nvarchar(max) NOT NULL
END

GO 

UPDATE ContentTypes
SET Name='Recipe Category'
WHERE Id=1