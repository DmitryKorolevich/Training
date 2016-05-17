IF OBJECT_ID(N'[dbo].[Newsletters]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Newsletters]
	(
		[Id] INT NOT NULL PRIMARY KEY, 
		[StatusCode] INT NOT NULL DEFAULT 2,
		[Name] NVARCHAR(250) NOT NULL, 
		CONSTRAINT [FK_Newsletters_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [RecordStatusCodes] ([StatusCode])
	);

END

IF OBJECT_ID(N'[dbo].[NewsletterBlockedEmails]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[NewsletterBlockedEmails]
	(
		[IdNewsletter] INT NOT NULL, 
		[Email] NVARCHAR(250) NOT NULL, 
		CONSTRAINT [FK_NewsletterBlockedEmails_ToNewsletters] FOREIGN KEY ([IdNewsletter]) REFERENCES [Newsletters] ([Id]),
		CONSTRAINT [PK_NewsletterBlockedEmails] PRIMARY KEY ([IdNewsletter] DESC, [Email] ASC)
	);
END