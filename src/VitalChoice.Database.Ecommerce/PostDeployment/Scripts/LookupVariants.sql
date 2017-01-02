IF NOT EXISTS(SELECT * FROM LookupVariants WHERE 
	IdLookup = (SELECT TOP 1 Id FROM Lookups WHERE Name = 'MarketingPromotionType') AND
	[Hidden]=1)
BEGIN

	UPDATE LookupVariants 
	SET [Hidden]=1
	WHERE IdLookup = (SELECT TOP 1 Id FROM Lookups WHERE Name = 'MarketingPromotionType')

	INSERT LookupVariants
	(Id,IdLookup,ValueVariant,[Order])
	VALUES
	(4, (SELECT TOP 1 Id FROM Lookups WHERE Name = 'MarketingPromotionType'),'Donation to Non-Profit (501c3)' ,4),
	(5, (SELECT TOP 1 Id FROM Lookups WHERE Name = 'MarketingPromotionType'),'Corporate Gift from VC' ,5),
	(6, (SELECT TOP 1 Id FROM Lookups WHERE Name = 'MarketingPromotionType'),'Marketing Gift from VC' ,6),
	(7, (SELECT TOP 1 Id FROM Lookups WHERE Name = 'MarketingPromotionType'),'Company Event' ,7),
	(8, (SELECT TOP 1 Id FROM Lookups WHERE Name = 'MarketingPromotionType'),'Vendor Event/Relationship' ,8),
	(9, (SELECT TOP 1 Id FROM Lookups WHERE Name = 'MarketingPromotionType'),'Product Development' ,9),
	(10, (SELECT TOP 1 Id FROM Lookups WHERE Name = 'MarketingPromotionType'),'Tradeshow/Conf/Seminar' ,10)

END
GO