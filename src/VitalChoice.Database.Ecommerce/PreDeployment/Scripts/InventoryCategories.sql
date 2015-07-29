IF OBJECT_ID(N'dbo.InventoryCategories', N'U') IS NULL
BEGIN

CREATE TABLE [dbo].[InventoryCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[ParentId] [int] NULL,
	[StatusCode] [int] NOT NULL DEFAULT ((1)),
	[Order] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[InventoryCategories]  WITH CHECK ADD  CONSTRAINT [FK_InventoryCategories_RecordStatusCodes] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])

ALTER TABLE [dbo].[InventoryCategories] CHECK CONSTRAINT [FK_InventoryCategories_RecordStatusCodes]

CREATE NONCLUSTERED INDEX [IX_InventoryCategories_ParentId] ON [dbo].[InventoryCategories]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)

END