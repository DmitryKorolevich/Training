IF (( SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [name] = N'UserId' AND [object_id] = OBJECT_ID(N'GiftCertificates')) = 0
AND (SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [object_id] = OBJECT_ID(N'GiftCertificates')) > 0)
BEGIN

ALTER TABLE GiftCertificates ADD UserId int NULL

CREATE NONCLUSTERED INDEX [IX_GiftCertificates_FirstName] ON [dbo].[GiftCertificates]
(
	[FirstName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

CREATE NONCLUSTERED INDEX [IX_GiftCertificates_LastName] ON [dbo].[GiftCertificates]
(
	[LastName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)


CREATE NONCLUSTERED INDEX [IX_GiftCertificates_Email] ON [dbo].[GiftCertificates]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

END

IF (( SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [name] = N'PublicId' AND [object_id] = OBJECT_ID(N'GiftCertificates')) = 0
AND (SELECT Count(*) AS existFlag FROM sys.columns 
WHERE [object_id] = OBJECT_ID(N'GiftCertificates')) > 0)
BEGIN

ALTER TABLE GiftCertificates ADD PublicId UNIQUEIDENTIFIER NOT NULL UNIQUE DEFAULT NEWSEQUENTIALID()

END

GO

ALTER TABLE GiftCertificates ALTER COLUMN [Balance] money NOT NULL

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[dbo].[GiftCertificates]', N'U') AND Name = 'IdOrder')
BEGIN
	ALTER TABLE GiftCertificates
	ADD IdOrder INT NULL

	ALTER TABLE dbo.GiftCertificates ADD CONSTRAINT
	FK_GiftCertificates_Orders FOREIGN KEY
	(
	IdOrder
	) REFERENCES dbo.Orders
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
END

GO
