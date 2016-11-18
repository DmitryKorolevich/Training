IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = 'IdEditedBy' AND [object_id] = OBJECT_ID(N'[dbo].[GiftCertificates]', N'U'))
BEGIN
	
	ALTER TABLE GiftCertificates
	ADD IdEditedBy INT NULL
END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = 'ExpirationDate' AND [object_id] = OBJECT_ID(N'[dbo].[GiftCertificates]', N'U'))
BEGIN
	
	ALTER TABLE GiftCertificates
	ADD ExpirationDate datetime2 NULL

	ALTER TABLE GiftCertificates
	ADD Tag nvarchar(250) NULL

END

GO