/****** Object:  Table [dbo].[CustomerFiles]    Script Date: 6/25/2016 2:10:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerFiles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomerFiles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdCustomer] [int] NOT NULL,
	[UploadDate] [datetime2](7) NOT NULL,
	[FileName] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](500) NULL,
 CONSTRAINT [PK_CustomerFiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerFiles_Customers]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerFiles]'))
ALTER TABLE [dbo].[CustomerFiles]  WITH CHECK ADD  CONSTRAINT [FK_CustomerFiles_Customers] FOREIGN KEY([IdCustomer])
REFERENCES [dbo].[Customers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CustomerFiles_Customers]') AND parent_object_id = OBJECT_ID(N'[dbo].[CustomerFiles]'))
ALTER TABLE [dbo].[CustomerFiles] CHECK CONSTRAINT [FK_CustomerFiles_Customers]
GO
