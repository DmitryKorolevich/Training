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