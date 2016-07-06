IF OBJECT_ID(N'[dbo].[SettingOptionTypes]', N'U') IS NULL
BEGIN

	CREATE TABLE [dbo].[SettingOptionTypes] (
		[Id] INT NOT NULL
			CONSTRAINT PK_SettingOptionTypes PRIMARY KEY (Id) IDENTITY,
		[Name] NVARCHAR(250) NOT NULL,
		[IdLookup] INT NULL
			CONSTRAINT FK_SettingOptionTypes_Lookups FOREIGN KEY (IdLookup) REFERENCES dbo.Lookups (Id),
		[IdFieldType] INT NOT NULL
			CONSTRAINT FK_SettingOptionTypes_FieldTypes FOREIGN KEY (IdFieldType) REFERENCES dbo.FieldTypes (Id),
		[IdObjectType] INT NULL,
		[DefaultValue] NVARCHAR(4000) NULL
	)

	CREATE NONCLUSTERED INDEX IX_SettingOptionTypes_Name ON dbo.[SettingOptionTypes] (Name)
	CREATE UNIQUE NONCLUSTERED INDEX IX_UQ_SettingOptionTypes ON [dbo].[SettingOptionTypes]
	(
		[Name] ASC,
		[IdObjectType] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

END

GO