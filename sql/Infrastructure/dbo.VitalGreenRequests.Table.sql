/****** Object:  Table [dbo].[VitalGreenRequests]    Script Date: 6/25/2016 3:41:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VitalGreenRequests]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[VitalGreenRequests](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](250) NULL,
	[LastName] [nvarchar](250) NULL,
	[Email] [nvarchar](250) NULL,
	[ZoneId] [int] NULL,
	[DateView] [datetime2](7) NULL,
	[DateCompleted] [datetime2](7) NULL,
	[Address] [nvarchar](250) NULL,
	[Address2] [nvarchar](250) NULL,
	[City] [nvarchar](250) NULL,
	[State] [nvarchar](10) NULL,
	[Zip] [nvarchar](50) NULL,
	[Phone] [nvarchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VitalGreenRequest_ToFedExZone]') AND parent_object_id = OBJECT_ID(N'[dbo].[VitalGreenRequests]'))
ALTER TABLE [dbo].[VitalGreenRequests]  WITH CHECK ADD  CONSTRAINT [FK_VitalGreenRequest_ToFedExZone] FOREIGN KEY([ZoneId])
REFERENCES [dbo].[FedExZones] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VitalGreenRequest_ToFedExZone]') AND parent_object_id = OBJECT_ID(N'[dbo].[VitalGreenRequests]'))
ALTER TABLE [dbo].[VitalGreenRequests] CHECK CONSTRAINT [FK_VitalGreenRequest_ToFedExZone]
GO
