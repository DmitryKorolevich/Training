CREATE TABLE [dbo].[MasterContentItems]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Name] NVARCHAR(250) NOT NULL UNIQUE, 
    [TypeId] INT NOT NULL, 
    [Template] NVARCHAR(MAX) NOT NULL, 
    [Created] DATETIME NOT NULL DEFAULT GETDATE(), 
    [Updated] DATETIME NOT NULL DEFAULT GETDATE(), 
    [StatusCode] INT NOT NULL DEFAULT 1,	
    CONSTRAINT [FK_MasterContentItems_ToContentTypes] FOREIGN KEY (TypeId) REFERENCES ContentTypes([Id]),
)

GO

CREATE TRIGGER TG_MasterContentItems_Update
   ON MasterContentItems 
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	UPDATE MasterContentItems
	SET Updated=GETDATE()
	WHERE Id IN
	(SELECT Id FROM inserted)
END
GO



