IF OBJECT_ID(N'[dbo].[Tokens]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].Tokens (
		[IdToken] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Tokens_Id]  DEFAULT (newid()),
		[DateCreated] [datetime2] NOT NULL,
		[DateExpired] [datetime2] NOT NULL,
		[IdTokenType] [int] NOT NULL,
		[Data] NVARCHAR(MAX) NOT NULL,
	)

END