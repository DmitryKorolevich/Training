IF NOT EXISTS(SELECT * FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[dbo].[ContentCategories]', N'U') AND Name = 'UserId')
BEGIN

ALTER TABLE dbo.ContentCategories ADD
	UserId int NULL

ALTER TABLE dbo.ContentCategories ADD CONSTRAINT
	FK_ContentCategories_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.ProductCategories ADD
	UserId int NULL

ALTER TABLE dbo.ProductCategories ADD CONSTRAINT
	FK_ProductCategories_AspNetUsers FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION

END