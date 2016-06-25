/****** Object:  Table [dbo].[ObjectHistoryLogItems]    Script Date: 6/25/2016 2:11:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ObjectHistoryLogItems]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ObjectHistoryLogItems](
	[IdObjectHistoryLogItem] [bigint] IDENTITY(1,1) NOT NULL,
	[IdObjectType] [int] NOT NULL,
	[IdObject] [int] NOT NULL,
	[IdObjectStatus] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[IdEditedBy] [int] NULL,
	[IdObjectHistoryLogDataItem] [bigint] NULL,
	[OptionalData] [nvarchar](250) NULL,
 CONSTRAINT [PK_ObjectHistoryLogItems] PRIMARY KEY CLUSTERED 
(
	[IdObjectHistoryLogItem] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_IdObject_DateCreated_IdObjectType]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ObjectHistoryLogItems]') AND name = N'IX_IdObject_DateCreated_IdObjectType')
CREATE UNIQUE NONCLUSTERED INDEX [IX_IdObject_DateCreated_IdObjectType] ON [dbo].[ObjectHistoryLogItems]
(
	[DateCreated] DESC,
	[IdObject] DESC,
	[IdObjectType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_IdObject_IdObjectType]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ObjectHistoryLogItems]') AND name = N'IX_IdObject_IdObjectType')
CREATE NONCLUSTERED INDEX [IX_IdObject_IdObjectType] ON [dbo].[ObjectHistoryLogItems]
(
	[IdObject] DESC,
	[IdObjectType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ObjectHis__DateC__2CBDA3B5]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ObjectHistoryLogItems] ADD  DEFAULT (sysdatetime()) FOR [DateCreated]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ObjectHistoryLogItems_ObjectHistoryLogDataItems]') AND parent_object_id = OBJECT_ID(N'[dbo].[ObjectHistoryLogItems]'))
ALTER TABLE [dbo].[ObjectHistoryLogItems]  WITH CHECK ADD  CONSTRAINT [FK_ObjectHistoryLogItems_ObjectHistoryLogDataItems] FOREIGN KEY([IdObjectHistoryLogDataItem])
REFERENCES [dbo].[ObjectHistoryLogDataItems] ([IdObjectHistoryLogDataItem])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ObjectHistoryLogItems_ObjectHistoryLogDataItems]') AND parent_object_id = OBJECT_ID(N'[dbo].[ObjectHistoryLogItems]'))
ALTER TABLE [dbo].[ObjectHistoryLogItems] CHECK CONSTRAINT [FK_ObjectHistoryLogItems_ObjectHistoryLogDataItems]
GO
