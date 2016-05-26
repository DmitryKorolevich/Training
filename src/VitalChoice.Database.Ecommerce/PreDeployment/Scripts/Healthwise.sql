IF OBJECT_ID(N'[dbo].[HealthwisePeriods]', N'U') IS NULL
BEGIN

	CREATE TABLE [dbo].[HealthwisePeriods] (
		[Id] INT NOT NULL
			CONSTRAINT PK_HealthwiseYears PRIMARY KEY (Id) IDENTITY,
		[IdCustomer] INT NOT NULL
			CONSTRAINT FK_HealthwisePeriodToCustomer FOREIGN KEY (IdCustomer) REFERENCES dbo.Customers (Id),
		[StartDate] [datetime2] NOT NULL,
		[EndDate] [datetime2] NOT NULL,
		[PaidAmount] MONEY NULL,
		[PaidDate] [datetime2] NULL,
	)


	CREATE TABLE [dbo].[HealthwiseOrders] (
		[Id] INT NOT NULL
			CONSTRAINT PK_HealthwiseOrders PRIMARY KEY (Id),
		[IdHealthwisePeriod] INT NOT NULL
			CONSTRAINT FK_HealthwiseOrderToHealthwisePeriod FOREIGN KEY (IdHealthwisePeriod) REFERENCES dbo.HealthwisePeriods (Id),
	)

	ALTER TABLE [dbo].[HealthwiseOrders] ADD CONSTRAINT
	FK_HealthwiseOrderToOrder FOREIGN KEY
	(
	Id
	) REFERENCES dbo.Orders
	(
	Id
	)

	CREATE NONCLUSTERED INDEX IX_HealthwisePeriods_IdCustomer ON HealthwisePeriods (IdCustomer)
	CREATE NONCLUSTERED INDEX IX_HealthwiseOrders_IdHealthwisePeriod ON HealthwiseOrders (IdHealthwisePeriod)

END

GO

IF ((SELECt delete_referential_action FROM sys.foreign_keys
WHERE name='FK_HealthwiseOrderToHealthwisePeriod')=0)
BEGIN

ALTER TABLE [dbo].[HealthwiseOrders] DROP CONSTRAINT [FK_HealthwiseOrderToHealthwisePeriod]

ALTER TABLE [dbo].[HealthwiseOrders]  WITH CHECK ADD  CONSTRAINT [FK_HealthwiseOrderToHealthwisePeriod] FOREIGN KEY([IdHealthwisePeriod])
REFERENCES [dbo].[HealthwisePeriods] ([Id])
ON DELETE CASCADE

ALTER TABLE [dbo].[HealthwiseOrders] CHECK CONSTRAINT [FK_HealthwiseOrderToHealthwisePeriod]

END

GO

