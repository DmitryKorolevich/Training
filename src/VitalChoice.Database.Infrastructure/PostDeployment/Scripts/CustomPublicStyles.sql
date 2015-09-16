IF OBJECT_ID(N'dbo.CustomPublicStyles', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[CustomPublicStyles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Styles] [nvarchar](max) NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[IdEditedBy] [int] NULL,
 CONSTRAINT [PK_CustomPublicStyles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)

ALTER TABLE [dbo].[CustomPublicStyles]  WITH CHECK ADD  CONSTRAINT [FK_CustomPublicStyles_AspNetUsers] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[AspNetUsers] ([Id])

ALTER TABLE [dbo].[CustomPublicStyles] CHECK CONSTRAINT [FK_CustomPublicStyles_AspNetUsers]


END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[CustomPublicStyles])
BEGIN
	INSERT INTO [dbo].[CustomPublicStyles]
	([Name], [Styles], [Created],[Updated])
	VALUES
	(N'Custom Public Styles', NULL, GETDATE(), GETDATE())

	ALTER TABLE [dbo].[CustomPublicStyles]
	ADD CONSTRAINT UQ_CustomPublicStyles UNIQUE(Name)
END

GO	