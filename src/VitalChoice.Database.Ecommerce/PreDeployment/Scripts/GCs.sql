IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = 'IdEditedBy' AND [object_id] = OBJECT_ID(N'[dbo].[GiftCertificates]', N'U'))
BEGIN
	
	ALTER TABLE GiftCertificates
	ADD IdEditedBy INT NULL
END

GO