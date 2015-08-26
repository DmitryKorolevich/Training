IF NOT EXISTS(SELECT * FROM Lookups WHERE Name='OrderSources')
BEGIN

	DECLARE @IdLookup INT
	
	INSERT INTO [dbo].[Lookups]
	([LookupValueType], [Name])
	VALUES
	(N'string', N'OrderSources')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookup,	N'Friend/Family'),
	(2, @IdLookup,	N'Search'),
	(3, @IdLookup,	N'Personal Health Advocate'),
	(4, @IdLookup,	N'Celebrity Health Advocate'),
	(5, @IdLookup,	N'Radio'),
	(6, @IdLookup,	N'Article, Press Coverage'),
	(7, @IdLookup,	N'Social Media'),
	(8, @IdLookup,	N'Catalog'),
	(9, @IdLookup,	N'Conference'),
	(10, @IdLookup,	N'Other')

	INSERT INTO [dbo].[Lookups]
	([LookupValueType], [Name])
	VALUES
	(N'string', N'OrderSourcesCelebrityHealthAdvocate')

	SET @IdLookup = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdLookup,	N'Dr. Mehmet Oz'),
	(2, @IdLookup,	N'Dr. Andrew Weil'),
	(3, @IdLookup,	N'Dr. Nicholas Perricone'),
	(4, @IdLookup,	N'Dr. Joseph Mercola'),
	(5, @IdLookup,	N'Dr. William Sears'),
	(6, @IdLookup,	N'Dr. Stephen Sinatra'),
	(7, @IdLookup,	N'Dr. Christiane Northrup'),
	(8, @IdLookup,	N'Dr. Mark Hyman'),
	(9, @IdLookup,	N'Dr. Jonny Bowden'),
	(10, @IdLookup,	N'Dr. Steven Pratt'),
	(11, @IdLookup,	N'Dr. David Williams'),
	(12, @IdLookup,	N'Dr. Dr. Ronald Hoffman'),
	(13, @IdLookup,	N'Chet Day'),
	(14, @IdLookup,	N'Other')

END