IF OBJECT_ID(N'[dbo].[VeraCoreProcessItems]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[VeraCoreProcessItems]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[Attempt] INT NOT NULL DEFAULT 0,
		[DateCreated] DATETIME2 NOT NULL,
		[FileName] NVARCHAR(250) NOT NULL, 
		[FileDate] DATETIME2 NOT NULL,
		[FileSize] BIGINT NOT NULL,
		[Data] NVARCHAR(MAX) NOT NULL,
		[IdType] INT NOT NULL
	);

END

GO

IF OBJECT_ID(N'[dbo].[VeraCoreProcessLogItems]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[VeraCoreProcessLogItems]
	(
		[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
		[DateCreated] DATETIME2 NOT NULL,
		[FileName] NVARCHAR(250) NOT NULL, 
		[FileDate] DATETIME2 NOT NULL,
		[FileSize] BIGINT NOT NULL
	);
		
	CREATE INDEX [IX_VeraCoreProcessLogItems_FileName] ON [dbo].[VeraCoreProcessLogItems] ([FileName])
	CREATE INDEX [IX_VeraCoreProcessLogItems_FileName_FileDate_FileSize] ON [dbo].[VeraCoreProcessLogItems] ([FileName],[FileDate],[FileSize])

END

GO