IF NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.GiftCertificates') AND name = N'IdSku')
BEGIN
	ALTER TABLE dbo.GiftCertificates
	ADD IdSku INT NULL
	CONSTRAINT FK_GiftCertificatesToSku FOREIGN KEY (IdSku) REFERENCES dbo.Skus (Id)
END