IF OBJECT_ID(N'[dbo].[EmailTemplates]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[EmailTemplates] (
		[Id]                  INT            IDENTITY (1, 1) NOT NULL,
		[Name]                NVARCHAR (250) NOT NULL,
		[ContentItemId]       INT            NOT NULL,
		[MasterContentItemId] INT            NOT NULL,
		[StatusCode]          INT            NOT NULL,
		[UserId]			  INT			 NULL		
			CONSTRAINT FK_EmailTemplateToAspNetUser FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers (Id),
		[ModelType]			  NVARCHAR (250) NULL,
		[EmailDescription]	  NVARCHAR (250) NOT NULL,
		PRIMARY KEY CLUSTERED ([Id] ASC)
	);

	ALTER TABLE [dbo].[EmailTemplates]
    ADD CONSTRAINT [FK_EmailTemplates_ToContentItem] FOREIGN KEY ([ContentItemId]) REFERENCES [dbo].[ContentItems] ([Id]);

	ALTER TABLE [dbo].[EmailTemplates]
    ADD CONSTRAINT [FK_EmailTemplates_ToMasterContentItem] FOREIGN KEY ([MasterContentItemId]) REFERENCES [dbo].[MasterContentItems] ([Id]);

	
	CREATE NONCLUSTERED INDEX IX_EmailTemplates_Name ON EmailTemplates ([Name])

	INSERT INTO ContentTypes
	(Id, Name)
	SELECT 11, 'Email'

	INSERT [dbo].[MasterContentItems] ([Name], [TypeId], [Template], [Created], [Updated], [StatusCode], [UserId])
	VALUES 
	('Admin Email Template', 11, N'<%    
	<body>
	{{
	}}
    
	<default> -> (Model)
	{{
		@body()
	}} :: dynamic
	%>', GETDATE(), GETDATE(),2, NULL)

	DECLARE @id INT

	SET @id=@@identity

	UPDATE ContentTypes
	SET DefaultMasterContentItemId=@id
	WHERE Id=11

END

GO
