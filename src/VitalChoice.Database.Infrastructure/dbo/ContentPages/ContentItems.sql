CREATE TABLE [dbo].[ContentItems]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Created] DATETIME NOT NULL DEFAULT GETDATE(), 
    [Updated] DATETIME NOT NULL DEFAULT GETDATE(), 
    [Template] NVARCHAR(MAX) NOT NULL, 
	[Description]  NVARCHAR(MAX) NOT NULL, 
    [Title] NVARCHAR(250) NULL, 
    [MetaKeywords] NVARCHAR(250) NULL, 
    [MetaDescription] NVARCHAR(250) NULL, 
)

GO

CREATE TRIGGER TG_ContentItems_Update
   ON ContentItems 
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	UPDATE ContentItems
	SET Updated=GETDATE()
	WHERE Id IN
	(SELECT Id FROM inserted)
END
GO
