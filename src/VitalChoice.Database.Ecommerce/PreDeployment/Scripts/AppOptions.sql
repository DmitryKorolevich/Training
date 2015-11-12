IF OBJECT_ID(N'[dbo].[AppOptions]', N'U') IS NULL
BEGIN

	CREATE TABLE [dbo].[AppOptions]
	(
		[OptionName] [nvarchar](250) NOT NULL PRIMARY KEY, 
		[OptionValue] [nvarchar](250) NOT NULL,
	);

	INSERT INTO [dbo].[AppOptions]
	(OptionName,OptionValue)
	VALUES
	('AffiliateOrderPaymentsCountToDate','2015-11-01 08:00:00.0000000')

END

GO
