/****** Object:  Table [dbo].[ProductCategoriesTemp]    Script Date: 6/25/2016 3:40:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductCategoriesTemp]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ProductCategoriesTemp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[ParentId] [int] NULL,
	[StatusCode] [int] NOT NULL,
	[Url] [nvarchar](250) NOT NULL,
	[Order] [int] NOT NULL,
	[Assigned] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ProductCa__Statu__208CD6FA]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ProductCategoriesTemp] ADD  DEFAULT ((1)) FOR [StatusCode]
END

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ProductCa__Assig__2180FB33]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ProductCategoriesTemp] ADD  DEFAULT ((1)) FOR [Assigned]
END

GO
