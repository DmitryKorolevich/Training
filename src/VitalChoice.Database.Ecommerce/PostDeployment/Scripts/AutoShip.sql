IF NOT EXISTS(SELECT * FROM Lookups WHERE Name ='AutoShipSchedule')
BEGIN

	DECLARE @IdLookup INT
	
	INSERT INTO [dbo].[Lookups]
	([LookupValueType], [Name], [Description])
	VALUES
	(N'string', N'AutoShipSchedule', N'Auto-Ship Schedule Options')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant], [Order])
	VALUES
	(1, @IdLookup,	N'1 Month' ,10),
	(2, @IdLookup,	N'2 Months', 20),
	(3, @IdLookup,	N'3 Months',30),
	(6, @IdLookup,	N'6 Months', 60)
END
GO