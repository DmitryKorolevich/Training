IF(NOT EXISTS (SELECT [Id] FROM [dbo].[AspNetRoles]))
BEGIN
	INSERT INTO [dbo].[AspNetRoles]
	([Id], [ConcurrencyStamp], [Name], [NormalizedName])
	VALUES
	(1, N'adminuserstamp', N'Admin User', 'AdminUser'),
	(2, N'contentuserstamp', N'Content User', 'ContentUser'),
	(3, N'orderuserstamp', N'Order User', 'OrderUser'),
	(4, N'productuserstamp', N'Product User', 'ProductUser'),
	(5, N'superadminuserstamp', N'Super Admin User', 'SuperAdminUser')
END