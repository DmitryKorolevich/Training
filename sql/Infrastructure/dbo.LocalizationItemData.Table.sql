/****** Object:  Table [dbo].[LocalizationItemData]    Script Date: 6/25/2016 3:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LocalizationItemData]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LocalizationItemData](
	[GroupId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[CultureId] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_LocalizationItemData] PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC,
	[ItemId] ASC,
	[CultureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LocalizationItemData_LocalizationItem_GroupId_ItemId]') AND parent_object_id = OBJECT_ID(N'[dbo].[LocalizationItemData]'))
ALTER TABLE [dbo].[LocalizationItemData]  WITH CHECK ADD  CONSTRAINT [FK_LocalizationItemData_LocalizationItem_GroupId_ItemId] FOREIGN KEY([GroupId], [ItemId])
REFERENCES [dbo].[LocalizationItem] ([GroupId], [ItemId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_LocalizationItemData_LocalizationItem_GroupId_ItemId]') AND parent_object_id = OBJECT_ID(N'[dbo].[LocalizationItemData]'))
ALTER TABLE [dbo].[LocalizationItemData] CHECK CONSTRAINT [FK_LocalizationItemData_LocalizationItem_GroupId_ItemId]
GO
