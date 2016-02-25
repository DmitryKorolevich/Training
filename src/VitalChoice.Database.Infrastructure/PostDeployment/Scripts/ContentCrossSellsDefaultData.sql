IF EXISTS(SELECT * FROM [dbo].[ContentCrossSells] WHERE [Type] = 1 AND [IdSku] is null)
BEGIN
	DELETE FROM [dbo].[ContentCrossSells] WHERE [Type] = 1

	ALTER TABLE [dbo].[ContentCrossSells]
	ALTER COLUMN [IdSku] [int] NOT NULL
END

GO