IF OBJECT_ID(N'[dbo].[ArticleBonusLinks]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].ArticleBonusLinks (
		[Id] INT NOT NULL 
			CONSTRAINT PK_ArticleBonusLinks PRIMARY KEY (Id) IDENTITY,
		[Url] nvarchar(250) NOT NULL,
		[StartDate] datetime2 NOT NULL,
	)

END

GO