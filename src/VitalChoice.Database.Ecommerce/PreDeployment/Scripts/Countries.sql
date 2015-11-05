IF OBJECT_ID(N'dbo.Countries', N'U') IS NULL
BEGIN

CREATE TABLE [dbo].[Countries](
	[Id] [int] IDENTITY NOT NULL,
	[CountryCode] [nvarchar](3) UNIQUE NOT NULL,
	[CountryName] [nvarchar](250) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[Order] [int] NOT NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[Countries]  WITH CHECK ADD  CONSTRAINT [FK_Countries_RecordStatusCodes] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])

ALTER TABLE [dbo].[Countries] CHECK CONSTRAINT [FK_Countries_RecordStatusCodes]

CREATE TABLE [dbo].[States](
	[Id] [int] IDENTITY NOT NULL,
	[StateCode] [nvarchar](3) NOT NULL,
	[CountryCode] [nvarchar](3) NOT NULL,
	[StateName] [nvarchar](250) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[Order] [int] NOT NULL,
 CONSTRAINT [PK_States] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[States]  WITH CHECK ADD  CONSTRAINT [FK_States_RecordStatusCodes] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])

ALTER TABLE [dbo].[States] CHECK CONSTRAINT [FK_States_RecordStatusCodes]

ALTER TABLE [dbo].[States]  WITH CHECK ADD  CONSTRAINT [FK_States_Countries] FOREIGN KEY([CountryCode])
REFERENCES [dbo].[Countries] ([CountryCode])
ON UPDATE CASCADE

ALTER TABLE [dbo].[States] CHECK CONSTRAINT [FK_States_Countries]

INSERT INTO Countries
([CountryCode]
           ,[CountryName]
           ,[StatusCode]
           ,[Order])
VALUES
('US','USA',2,1)

END