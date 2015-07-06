IF OBJECT_ID(N'dbo.ProductCategories', N'U') IS NULL
BEGIN

CREATE TABLE [dbo].[ProductCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[ParentId] [int] NULL,
	[StatusCode] [int] NOT NULL DEFAULT ((1)),
	[Url] [nvarchar](250) NOT NULL,
	[Order] [int] NOT NULL,
	[Assigned][int] NOT NULL DEFAULT ((1)),
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[ProductCategories]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategories_ToContentCategory] FOREIGN KEY([ParentId])
REFERENCES [dbo].[ProductCategories] ([Id])

ALTER TABLE [dbo].[ProductCategories] CHECK CONSTRAINT [FK_ProductCategories_ToContentCategory]

ALTER TABLE [dbo].[ProductCategories]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategories_RecordStatusCodes] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])

ALTER TABLE [dbo].[ProductCategories] CHECK CONSTRAINT [FK_ProductCategories_RecordStatusCodes]

CREATE NONCLUSTERED INDEX [IX_ProductCategories_ParentId] ON [dbo].[ProductCategories]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

CREATE NONCLUSTERED INDEX [IX_ProductCategories_Url] ON [dbo].[ProductCategories]
(
	[Url] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

SET IDENTITY_INSERT [dbo].[ProductCategories] ON;

INSERT INTO ProductCategories
(Id,Name,ParentId,StatusCode,Url, [Order])
VALUES
(1,'Products Categories',NULL,2,'root',0)

SET IDENTITY_INSERT [dbo].[ProductCategories] OFF;

END