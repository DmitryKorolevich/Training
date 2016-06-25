/****** Object:  Table [dbo].[FedExZones]    Script Date: 6/25/2016 3:40:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FedExZones]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FedExZones](
	[Id] [int] NOT NULL,
	[Company] [nvarchar](250) NOT NULL,
	[Address] [nvarchar](250) NOT NULL,
	[City] [nvarchar](250) NOT NULL,
	[State] [nvarchar](10) NOT NULL,
	[Zip] [nvarchar](50) NOT NULL,
	[Phone] [nvarchar](250) NOT NULL,
	[Website] [nvarchar](250) NOT NULL,
	[Contact] [nvarchar](250) NOT NULL,
	[StatesCovered] [nvarchar](250) NOT NULL,
	[InUse] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
