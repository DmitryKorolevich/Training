IF OBJECT_ID(N'[dbo].[ObjectHistoryLogItems]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ObjectHistoryLogItems] (
		IdObjectHistoryLogItem BIGINT NOT NULL IDENTITY CONSTRAINT PK_ObjectHistoryLogItems PRIMARY KEY,
		IdObjectType INT NOT NULL,
		IdObject INT NOT NULL,		
		IdObjectStatus INT NOT NULL,
		[DateCreated] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
		[IdEditedBy] INT NULL,
		[IdObjectHistoryLogDataItem] BIGINT NULL,
		[OptionalData] NVARCHAR(250) NULL
	)

		CREATE TABLE [dbo].[ObjectHistoryLogDataItems] (
		IdObjectHistoryLogDataItem BIGINT NOT NULL IDENTITY CONSTRAINT PK_ObjectHistoryLogDataItems PRIMARY KEY,
		[Data] NVARCHAR(MAX) NOT NULL
	)

		ALTER TABLE [dbo].[ObjectHistoryLogItems]  WITH CHECK ADD  CONSTRAINT [FK_ObjectHistoryLogItems_ObjectHistoryLogDataItems] 
			FOREIGN KEY([IdObjectHistoryLogDataItem])
			REFERENCES [dbo].[ObjectHistoryLogDataItems] ([IdObjectHistoryLogDataItem])

		CREATE UNIQUE NONCLUSTERED INDEX [IX_IdObject_DateCreated_IdObjectType] ON [dbo].[ObjectHistoryLogItems]
		(
			[DateCreated] DESC,
			[IdObject] DESC,
			[IdObjectType] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

		CREATE NONCLUSTERED INDEX [IX_IdObject_IdObjectType] ON [dbo].[ObjectHistoryLogItems]
		(
			[IdObject] DESC,
			[IdObjectType] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

END

GO