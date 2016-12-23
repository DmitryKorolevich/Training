IF NOT EXISTS(SELECT * FROM CustomerOptionTypes WHERE Name='IgnoreAbusePolicy ')
BEGIN

	INSERT INTO [dbo].[CustomerOptionTypes]
	([Name], [IdFieldType], [IdLookup], [IdObjectType], [DefaultValue])
	VALUES
	(N'IgnoreAbusePolicy', 5, NULL, NULL, 'False')

END
GO