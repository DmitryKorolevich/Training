IF NOT EXISTS(SELECT * FROM [dbo].[ContentCrossSells])
BEGIN
	INSERT INTO [dbo].[ContentCrossSells]
	([Type], [Title], [Price], [ImageUrl], [IdSku],	[IdEditedBy], [DateCreated], [DateEdited], [Order])
	VALUES
	(1, 'Cross Sell 1', 0, '/Assets/images/cart/NRT501_alabcore_pouched_30z_218.jpg', null, null, GETDATE(), GETDATE(), 1),
	(1, 'Cross Sell 2', 0, '/Assets/images/cart/ntm1_trail-mix_no_pistachio_218.jpg', null, null, GETDATE(), GETDATE(), 2),
	(1, 'Cross Sell 3', 0, '/Assets/images/cart/seaweedsalad_218.jpg', null, null, GETDATE(), GETDATE(), 3),
	(1, 'Cross Sell 4', 0, '/Assets/images/cart/NSC201_salmonoil_1000mg_90_218.jpg', null, null, GETDATE(), GETDATE(), 4)
END

GO
