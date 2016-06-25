/****** Object:  Table [dbo].[ContentItems]    Script Date: 6/25/2016 3:40:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContentItems]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ContentItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[Template] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Title] [nvarchar](250) NULL,
	[MetaKeywords] [nvarchar](250) NULL,
	[MetaDescription] [nvarchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ContentIt__Creat__17F790F9]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ContentItems] ADD  DEFAULT (getdate()) FOR [Created]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ContentIt__Updat__17036CC0]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ContentItems] ADD  DEFAULT (getdate()) FOR [Updated]
END

GO
/****** Object:  Trigger [dbo].[TG_ContentItems_Update]    Script Date: 6/25/2016 3:41:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TG_ContentItems_Update]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TG_ContentItems_Update]
   ON [dbo].[ContentItems] 
   AFTER UPDATE
AS 
BEGIN
	UPDATE ContentItems
	SET Updated=GETDATE()
	WHERE Id IN
	(SELECT Id FROM inserted)
END

PRINT N''Creating [dbo].[TG_MasterContentItems_Update]...'';

' 
GO
ALTER TABLE [dbo].[ContentItems] ENABLE TRIGGER [TG_ContentItems_Update]
GO
