IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = 'Quantity' AND [object_id] = OBJECT_ID(N'[dbo].[OrderShippingPackages]', N'U'))
BEGIN
	
	ALTER TABLE OrderShippingPackages
	ADD Quantity INT NULL
END

GO