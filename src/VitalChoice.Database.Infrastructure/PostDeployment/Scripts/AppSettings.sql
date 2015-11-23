IF OBJECT_ID(N'dbo.AppSettings', N'U') IS NULL
BEGIN

CREATE TABLE [dbo].[AppSettings](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Value] [nvarchar](250) NULL,
 CONSTRAINT [PK_AppSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT AppSettings
(Id,Name,Value)
VALUES
(1,'GlobalPerishableThreshold', '65')

END

GO

IF NOT EXISTS(SELECT * FROM AppSettings WHERE Name='CreditCardAuthorizations')
BEGIN

INSERT AppSettings
(Id,Name,Value)
VALUES
(2,'CreditCardAuthorizations', 'True')

CREATE UNIQUE NONCLUSTERED INDEX [IX_Name] ON [dbo].[AppSettings]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

END

GO

IF NOT EXISTS(SELECT * FROM AppSettings WHERE Name='HealthwisePeriodMaxItemsCount')
BEGIN

INSERT AppSettings
(Id,Name,Value)
VALUES
(5,'HealthwisePeriodMaxItemsCount', '9')

END

GO