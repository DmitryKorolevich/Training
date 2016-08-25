GO

USE [master] 
GO
ALTER DATABASE [VitalChoice.ExportInfo] SET RECOVERY SIMPLE WITH NO_WAIT
USE [VitalChoice.ExportInfo]
GO
DBCC SHRINKFILE([VitalChoice.ExportInfo_Log], 1)
GO

USE [master] 
GO
ALTER DATABASE [VitalChoice.Infrastructure] SET RECOVERY SIMPLE WITH NO_WAIT
USE [VitalChoice.Infrastructure]
GO
DBCC SHRINKFILE([VitalChoice.Infrastructure_Log], 1)
GO

USE [master] 
GO
ALTER DATABASE [VitalChoice.Ecommerce] SET RECOVERY SIMPLE WITH NO_WAIT
USE [VitalChoice.Ecommerce]
GO
DBCC SHRINKFILE([VitalChoice.Ecommerce_Log], 1)
GO

USE [VitalChoice.Ecommerce]
GO

DECLARE @alter_index NVARCHAR(1000)

DECLARE ind CURSOR FOR 
	SELECT 'ALTER INDEX ' + I.name + ' ON ' + T.name + ' DISABLE' 
	FROM sys.indexes I
	INNER JOIN sys.tables T ON I.object_id = T.object_id
	WHERE I.type_desc = 'NONCLUSTERED' AND I.name IS NOT NULL

OPEN ind;

FETCH NEXT FROM ind
INTO @alter_index

WHILE @@FETCH_STATUS = 0
BEGIN
	EXEC sp_executesql @alter_index

	FETCH NEXT FROM ind
	INTO @alter_index
END

CLOSE ind;
DEALLOCATE ind;

GO

USE [VitalChoice.Infrastructure]
GO

DECLARE @alter_index NVARCHAR(1000)

DECLARE ind CURSOR FOR 
	SELECT 'ALTER INDEX ' + I.name + ' ON ' + T.name + ' DISABLE' 
	FROM sys.indexes I
	INNER JOIN sys.tables T ON I.object_id = T.object_id
	WHERE I.type_desc = 'NONCLUSTERED' AND I.name IS NOT NULL

OPEN ind;

FETCH NEXT FROM ind
INTO @alter_index

WHILE @@FETCH_STATUS = 0
BEGIN
	EXEC sp_executesql @alter_index

	FETCH NEXT FROM ind
	INTO @alter_index
END

CLOSE ind;
DEALLOCATE ind;

USE [VitalChoice.Ecommerce]
PRINT '====Wipe out all data'
--============================ Wipe Customers ====================================
TRUNCATE TABLE [VitalChoice.ExportInfo].dbo.CustomerPaymentMethods
TRUNCATE TABLE [VitalChoice.ExportInfo].dbo.OrderPaymentMethods
GO

DELETE FROM AffiliateOrderPayments
DELETE FROM AffiliatePayments
GO
TRUNCATE TABLE CartToSkus
DELETE FROM Carts
DELETE FROM OrderToSkus
DELETE FROM RefundOrderToGiftCertificates
DELETE FROM CartToGiftCertificates
DELETE FROM OrderToGiftCertificates
UPDATE GiftCertificates
SET IdOrder = NULL
WHERE IdOrder IS NOT NULL
DELETE FROM HealthwiseOrders
DELETE FROM HelpTicketComments
DELETE FROM HelpTickets
DELETE FROM RefundSkus
DELETE FROM ReshipProblemSkus
DELETE FROM OrderToPromos
TRUNCATE TABLE OrderOptionValues
DELETE FROM OrderShippingPackages
DELETE FROM Orders
TRUNCATE TABLE OrderPaymentMethodOptionValues
DELETE FROM OrderPaymentMethods
TRUNCATE TABLE OrderAddressOptionValues
DELETE FROM OrderAddresses
DELETE FROM CustomerToShippingAddresses
DELETE FROM CustomersToPaymentMethods
DELETE FROM CustomersToOrderNotes
TRUNCATE TABLE CustomerPaymentMethodValues
DELETE FROM CustomerPaymentMethods
TRUNCATE TABLE CustomerNoteOptionValues
DELETE FROM CustomerNotes
DELETE FROM OneTimeDiscountToCustomerUsages
DELETE FROM HealthwisePeriods
DELETE FROM BigStringValues
WHERE IdBigString IN (SELECT cv.IdBigString FROM CustomerOptionValues AS cv WHERE cv.IdBigString IS NOT NULL)
TRUNCATE TABLE CustomerOptionValues
DELETE FROM CustomerFiles
DELETE FROM Customers
TRUNCATE TABLE AddressOptionValues
DELETE FROM Addresses
--remove customers from ecommerce DB
DELETE FROM Users WHERE Id IN (SELECT id FROM [VitalChoice.Infrastructure].dbo.AspNetUsers WHERE IdUserType = 2)
GO
USE [VitalChoice.Infrastructure]
GO
DELETE FROM AspNetUserClaims
WHERE UserId IN (SELECT id FROM AspNetUsers WHERE IdUserType = 2)
GO
DELETE FROM AspNetUserLogins
WHERE UserId IN (SELECT id FROM AspNetUsers WHERE IdUserType = 2)
GO
DELETE FROM AspNetUserRoles
WHERE UserId IN (SELECT id FROM AspNetUsers WHERE IdUserType = 2)
GO
DELETE FROM AspNetUsers
WHERE IdUserType = 2
GO

--wipe out affiliates

USE [VitalChoice.Ecommerce]
GO

DELETE FROM Users WHERE Id IN (SELECT id FROM [VitalChoice.Infrastructure].dbo.AspNetUsers WHERE IdUserType = 3)

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUserClaims
WHERE UserId IN (SELECT Id FROM [VitalChoice.Infrastructure].dbo.AspNetUsers WHERE IdUserType = 3)

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUserLogins
WHERE UserId IN (SELECT Id FROM [VitalChoice.Infrastructure].dbo.AspNetUsers WHERE IdUserType = 3)

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUserRoles
WHERE UserId IN (SELECT Id FROM [VitalChoice.Infrastructure].dbo.AspNetUsers WHERE IdUserType = 3)

DELETE FROM [VitalChoice.Infrastructure].dbo.AspNetUsers
WHERE IdUserType = 3

DELETE FROM [VitalChoice.Ecommerce].dbo.Users
WHERE Id IN (SELECT Id FROM [VitalChoice.Ecommerce].dbo.Affiliates)

DELETE FROM [VitalChoice.Ecommerce].dbo.BigStringValues
WHERE IdBigString IN (SELECT ao.IdBigString FROM [VitalChoice.Ecommerce].dbo.AffiliateOptionValues AS ao WHERE ao.IdBigString IS NOT NULL)

DELETE FROM [VitalChoice.Ecommerce].dbo.AffiliateOptionValues

UPDATE [VitalChoice.Ecommerce].dbo.Customers
SET IdAffiliate = NULL
WHERE IdAffiliate IS NOT NULL

DELETE FROM [VitalChoice.Ecommerce].dbo.Affiliates

GO

USE [VitalChoice.Ecommerce]

DELETE FROM CatalogRequestAddressOptionValues
DELETE FROM CatalogRequestAddresses
DELETE FROM CartToGiftCertificates
DELETE FROM RefundOrderToGiftCertificates
DELETE FROM OrderToGiftCertificates
DELETE FROM GiftCertificates
DELETE FROM PromotionsToBuySkus
DELETE FROM PromotionsToGetSkus
DELETE FROM PromotionsToSelectedCategories
DELETE FROM PromotionOptionValues
DELETE FROM Promotions
DELETE FROM ProductReviews
DELETE FROM ObjectHistoryLogItems
DELETE FROM ObjectHistoryLogDataItems

GO

USE [VitalChoice.Ecommerce]
GO

TRUNCATE TABLE CartToSkus
GO
DELETE FROM CartToGiftCertificates
GO
DELETE FROM Carts
GO
DELETE FROM RefundOrderToGiftCertificates
GO

UPDATE GiftCertificates
SET IdOrder = NULL
WHERE IdOrder IS NOT NULL
GO
TRUNCATE TABLE HealthwiseOrders
GO
DELETE FROM HealthwisePeriods
GO
DELETE FROM HelpTicketComments
GO
DELETE FROM HelpTickets
GO
TRUNCATE TABLE OrderOptionValues
GO
TRUNCATE TABLE OrderShippingPackages
GO
DELETE FROM OrderToGiftCertificates
GO
DELETE FROM OrderToPromosToInventorySkus
GO
DELETE FROM OrderToPromos
GO
DELETE FROM OrderToSkusToInventorySkus
GO
DELETE FROM OrderToSkus
GO
DELETE FROM RefundSkus
GO
DELETE FROM ReshipProblemSkus
GO
TRUNCATE TABLE OrderAddressOptionValues
GO
DELETE FROM Orders
GO
TRUNCATE TABLE OrderPaymentMethodOptionValues
GO
DELETE FROM OrderPaymentMethods
GO
DELETE FROM OrderAddresses
GO
TRUNCATE TABLE DiscountOptionValues
DELETE FROM DiscountsToCategories
DELETE FROM DiscountsToSelectedSkus
DELETE FROM DiscountsToSkus
DELETE FROM DiscountTiers
TRUNCATE TABLE DiscountToSelectedCategories
DELETE FROM OneTimeDiscountToCustomerUsages
DELETE FROM Discounts

GO

USE [VitalChoice.Ecommerce]
GO

DECLARE @alter_index NVARCHAR(1000)

DECLARE ind CURSOR FOR 
	SELECT 'ALTER INDEX ' + I.name + ' ON ' + T.name + ' REBUILD' 
	FROM sys.indexes I
	INNER JOIN sys.tables T ON I.object_id = T.object_id
	WHERE I.type_desc = 'NONCLUSTERED' AND I.name IS NOT NULL

OPEN ind;

FETCH NEXT FROM ind
INTO @alter_index

WHILE @@FETCH_STATUS = 0
BEGIN
	EXEC sp_executesql @alter_index

	FETCH NEXT FROM ind
	INTO @alter_index
END

CLOSE ind;
DEALLOCATE ind;

GO

USE [VitalChoice.Infrastructure]
GO

DECLARE @alter_index NVARCHAR(1000)

DECLARE ind CURSOR FOR 
	SELECT 'ALTER INDEX ' + I.name + ' ON ' + T.name + ' REBUILD' 
	FROM sys.indexes I
	INNER JOIN sys.tables T ON I.object_id = T.object_id
	WHERE I.type_desc = 'NONCLUSTERED' AND I.name IS NOT NULL

OPEN ind;

FETCH NEXT FROM ind
INTO @alter_index

WHILE @@FETCH_STATUS = 0
BEGIN
	EXEC sp_executesql @alter_index

	FETCH NEXT FROM ind
	INTO @alter_index
END

CLOSE ind;
DEALLOCATE ind;


GO


USE [VitalChoice.Ecommerce]

GO

DELETE FROM SkuOptionValues
WHERE IdSku IN (
	SELECT Id FROM Skus WHERE IdProduct = 2635
)

DELETE FROM Skus
WHERE IdProduct = 2635


DELETE FROM ProductOptionValues
WHERE IdProduct = 2635

DELETE FROM Products
WHERE Id = 2635

GO

USE [VitalChoice.Infrastructure]

GO

DECLARE @contentTodelete TABLE(Id INT NOT NULL PRIMARY KEY)

INSERT INTO @contentTodelete
(Id)
SELECT ContentItemId FROM Products 
WHERE Id = 2635

DELETE FROM ContentItems
WHERE Id IN (SELECT Id FROM @contentTodelete)

DELETE FROM Products 
WHERE Id = 2635
GO
PRINT '====Wipe out all data === END'