USE [VitalChoice.Ecommerce]
GO

DECLARE @alter_index NVARCHAR(1000)

DECLARE ind CURSOR FOR 
	SELECT 'ALTER INDEX ' + I.name + ' ON ' + T.name + ' REBUILD' 
	FROM sys.indexes I
	INNER JOIN sys.tables T ON I.object_id = T.object_id
	WHERE I.type_desc = 'NONCLUSTERED' AND I.name IS NOT NULL

OPEN ind;

FETCH NEXT FROM ind
INTO @alter_index

WHILE @@FETCH_STATUS = 0
BEGIN
	EXEC sp_executesql @alter_index

	FETCH NEXT FROM ind
	INTO @alter_index
END

CLOSE ind;
DEALLOCATE ind;

GO

USE [VitalChoice.Infrastructure]
GO

DECLARE @alter_index NVARCHAR(1000)

DECLARE ind CURSOR FOR 
	SELECT 'ALTER INDEX ' + I.name + ' ON ' + T.name + ' REBUILD' 
	FROM sys.indexes I
	INNER JOIN sys.tables T ON I.object_id = T.object_id
	WHERE I.type_desc = 'NONCLUSTERED' AND I.name IS NOT NULL

OPEN ind;

FETCH NEXT FROM ind
INTO @alter_index

WHILE @@FETCH_STATUS = 0
BEGIN
	EXEC sp_executesql @alter_index

	FETCH NEXT FROM ind
	INTO @alter_index
END

CLOSE ind;
DEALLOCATE ind;

GO