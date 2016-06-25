/****** Object:  Table [dbo].[MasterContentItems]    Script Date: 6/25/2016 3:40:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MasterContentItems]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MasterContentItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[TypeId] [int] NOT NULL,
	[Template] [nvarchar](max) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[UserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_TypeIdStatusCodeName]    Script Date: 6/25/2016 3:41:10 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[MasterContentItems]') AND name = N'IX_TypeIdStatusCodeName')
CREATE NONCLUSTERED INDEX [IX_TypeIdStatusCodeName] ON [dbo].[MasterContentItems]
(
	[TypeId] ASC,
	[StatusCode] ASC,
	[Name] ASC
)
INCLUDE ( 	[Id],
	[Template],
	[Created],
	[Updated],
	[UserId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__MasterCon__Creat__1BC821DD]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[MasterContentItems] ADD  DEFAULT (getdate()) FOR [Created]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__MasterCon__Updat__1DB06A4F]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[MasterContentItems] ADD  DEFAULT (getdate()) FOR [Updated]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__MasterCon__Statu__1CBC4616]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[MasterContentItems] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MasterContentItems_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[MasterContentItems]'))
ALTER TABLE [dbo].[MasterContentItems]  WITH CHECK ADD  CONSTRAINT [FK_MasterContentItems_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MasterContentItems_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[MasterContentItems]'))
ALTER TABLE [dbo].[MasterContentItems] CHECK CONSTRAINT [FK_MasterContentItems_AspNetUsers]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MasterContentItems_ToContentTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[MasterContentItems]'))
ALTER TABLE [dbo].[MasterContentItems]  WITH CHECK ADD  CONSTRAINT [FK_MasterContentItems_ToContentTypes] FOREIGN KEY([TypeId])
REFERENCES [dbo].[ContentTypes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MasterContentItems_ToContentTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[MasterContentItems]'))
ALTER TABLE [dbo].[MasterContentItems] CHECK CONSTRAINT [FK_MasterContentItems_ToContentTypes]
GO
/****** Object:  Trigger [dbo].[TG_MasterContentItems_Update]    Script Date: 6/25/2016 3:41:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TG_MasterContentItems_Update]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TG_MasterContentItems_Update]
   ON [dbo].[MasterContentItems] 
   AFTER UPDATE
AS 
BEGIN
	UPDATE MasterContentItems
	SET Updated=GETDATE()
	WHERE Id IN
	(SELECT Id FROM inserted)
END
' 
GO
ALTER TABLE [dbo].[MasterContentItems] ENABLE TRIGGER [TG_MasterContentItems_Update]
GO
