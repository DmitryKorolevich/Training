IF OBJECT_ID(N'[dbo].[AdminTeams]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].AdminTeams (
		[Id] INT NOT NULL 
			CONSTRAINT PK_AdminTeams PRIMARY KEY (Id) IDENTITY,
		[Name] nvarchar(250) NOT NULL,
	)

	ALTER TABLE dbo.AdminProfiles
	ADD IdAdminTeam INT NULL

	ALTER TABLE dbo.AdminProfiles ADD CONSTRAINT
	FK_AdminProfiles_AdminTeams FOREIGN KEY
	(
	IdAdminTeam
	) REFERENCES dbo.AdminTeams
	(
	Id
	)

END

GO