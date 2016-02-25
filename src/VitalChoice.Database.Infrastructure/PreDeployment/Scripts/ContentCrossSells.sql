IF OBJECT_ID(N'[dbo].[ContentCrossSells]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].ContentCrossSells(
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Type] [tinyint] NOT NULL,
		[Title] [nvarchar](250) NOT NULL,
		[Price] [money] NOT NULL,
		[ImageUrl] [nvarchar](250) NOT NULL,
		[IdSku] [int] NULL,
		[IdEditedBy] [int] NULL,
		[Order] [int] NOT NULL,
		[DateCreated] [datetime2](7) NOT NULL,
		[DateEdited] [datetime2](7) NOT NULL
	 CONSTRAINT [PK_ContentCrossSells] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	))
END

GO

IF COL_LENGTH('[dbo].[ContentCrossSells]','Price') IS NOT NULL
BEGIN
	ALTER TABLE [dbo].[ContentCrossSells]
	DROP COLUMN [Price]
END

GO