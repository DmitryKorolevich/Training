/****** Object:  Table [dbo].[HealthwisePeriods]    Script Date: 6/25/2016 2:11:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HealthwisePeriods]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[HealthwisePeriods](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdCustomer] [int] NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[PaidAmount] [money] NULL,
	[PaidDate] [datetime2](7) NULL,
 CONSTRAINT [PK_HealthwiseYears] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
/****** Object:  Index [IX_HealthwisePeriods_IdCustomer]    Script Date: 6/25/2016 2:13:30 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[HealthwisePeriods]') AND name = N'IX_HealthwisePeriods_IdCustomer')
CREATE NONCLUSTERED INDEX [IX_HealthwisePeriods_IdCustomer] ON [dbo].[HealthwisePeriods]
(
	[IdCustomer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_HealthwisePeriodToCustomer]') AND parent_object_id = OBJECT_ID(N'[dbo].[HealthwisePeriods]'))
ALTER TABLE [dbo].[HealthwisePeriods]  WITH CHECK ADD  CONSTRAINT [FK_HealthwisePeriodToCustomer] FOREIGN KEY([IdCustomer])
REFERENCES [dbo].[Customers] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_HealthwisePeriodToCustomer]') AND parent_object_id = OBJECT_ID(N'[dbo].[HealthwisePeriods]'))
ALTER TABLE [dbo].[HealthwisePeriods] CHECK CONSTRAINT [FK_HealthwisePeriodToCustomer]
GO
