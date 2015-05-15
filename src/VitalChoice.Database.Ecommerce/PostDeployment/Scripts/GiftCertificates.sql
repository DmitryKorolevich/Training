IF OBJECT_ID(N'dbo.GiftCertificates', N'U') IS NULL
BEGIN

CREATE TABLE [dbo].[GiftCertificates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](250) NOT NULL,
    [Created] DATETIME NOT NULL,
	[Balance] [decimal](6,2) NOT NULL,
	[FirstName] [nvarchar](250) NULL,
	[LastName] [nvarchar](250) NULL,
	[Email] [nvarchar](250) NULL,
	[StatusCode] [int] NOT NULL DEFAULT ((1)),
	[GCType] [int] NOT NULL DEFAULT ((1)),
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[GiftCertificates]
    ADD DEFAULT GETDATE() FOR [Created];

ALTER TABLE [dbo].[GiftCertificates]  WITH CHECK ADD  CONSTRAINT [FK_GiftCertificates_RecordStatusCodes] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])

ALTER TABLE [dbo].[GiftCertificates] CHECK CONSTRAINT [FK_GiftCertificates_RecordStatusCodes]

CREATE NONCLUSTERED INDEX [IX_GiftCertificates_ParentId] ON [dbo].[GiftCertificates]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

END