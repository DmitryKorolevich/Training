IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = 'Hidden' AND
	[object_id] = OBJECT_ID(N'[dbo].[LookupVariants]', N'U'))
BEGIN
	
	ALTER TABLE LookupVariants
	ADD [Hidden] BIT NOT NULL DEFAULT(0)
END

GO