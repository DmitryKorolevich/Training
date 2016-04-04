IF OBJECT_ID(N'dbo.OneTimeDiscountToCustomerUsages') IS NULL
BEGIN
	CREATE TABLE OneTimeDiscountToCustomerUsages
	(
		IdCustomer INT NOT NULL,
		IdDiscount INT NOT NULL,
		UsageCount INT NOT NULL,
		CONSTRAINT PK_DiscountsOneTimeUsage PRIMARY KEY (IdCustomer DESC, IdDiscount ASC),
		CONSTRAINT FK_OneTimeUsageToCustomer FOREIGN KEY (IdCustomer) REFERENCES dbo.Customers (Id),
		CONSTRAINT FK_OneTimeUsageToDiscount FOREIGN KEY (IdDiscount) REFERENCES dbo.Discounts (Id)
	)
END