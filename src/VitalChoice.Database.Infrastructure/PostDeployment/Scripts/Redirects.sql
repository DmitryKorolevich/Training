IF OBJECT_ID(N'[dbo].[Redirects]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].Redirects (
		Id INT NOT NULL CONSTRAINT PK_Redirects PRIMARY KEY IDENTITY,
		[From] NVARCHAR(250) NOT NULL,
		[To] NVARCHAR(250) NOT NULL,
		[StatusCode] INT NOT NULL,
		[DateCreated] [datetime2] NOT NULL,
		[DateEdited] [datetime2] NOT NULL,
		[IdAddedBy] [int] NULL
			CONSTRAINT FK_RedirectsToAddedUser FOREIGN KEY (IdAddedBy) REFERENCES dbo.AspNetUsers (Id),	
		[IdEditedBy] [int] NULL
			CONSTRAINT FK_RedirectsToEditedUser FOREIGN KEY (IdEditedBy) REFERENCES dbo.AspNetUsers (Id),	
	)
END

GO