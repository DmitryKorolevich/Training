IF (( SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [name] = N'UserId' AND [object_id] = OBJECT_ID(N'MasterContentItems')) = 0
AND (SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [object_id] = OBJECT_ID(N'MasterContentItems')) > 0)
BEGIN

ALTER TABLE MasterContentItems ADD UserId int NULL

ALTER TABLE dbo.MasterContentItems ADD CONSTRAINT
	FK_MasterContentItems_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION

END

GO

IF (( SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [name] = N'UserId' AND [object_id] = OBJECT_ID(N'ContentPages')) = 0
AND (SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [object_id] = OBJECT_ID(N'ContentPages')) > 0)
BEGIN

ALTER TABLE ContentPages ADD UserId int NULL

ALTER TABLE dbo.ContentPages ADD CONSTRAINT
	FK_ContentPages_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION

END

GO

IF (( SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [name] = N'UserId' AND [object_id] = OBJECT_ID(N'Articles')) = 0
AND (SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [object_id] = OBJECT_ID(N'Articles')) > 0)
BEGIN

ALTER TABLE Articles ADD UserId int NULL

ALTER TABLE dbo.Articles ADD CONSTRAINT
	FK_Articles_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION

END

GO

IF (( SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [name] = N'UserId' AND [object_id] = OBJECT_ID(N'FAQs')) = 0
AND (SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [object_id] = OBJECT_ID(N'FAQs')) > 0)
BEGIN

ALTER TABLE FAQs ADD UserId int NULL

ALTER TABLE dbo.FAQs ADD CONSTRAINT
	FK_FAQs_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION

END

GO

IF (( SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [name] = N'UserId' AND [object_id] = OBJECT_ID(N'Recipes')) = 0
AND (SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [object_id] = OBJECT_ID(N'Recipes')) > 0)
BEGIN

ALTER TABLE Recipes ADD UserId int NULL

ALTER TABLE dbo.Recipes ADD CONSTRAINT
	FK_Recipes_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION

END

GO