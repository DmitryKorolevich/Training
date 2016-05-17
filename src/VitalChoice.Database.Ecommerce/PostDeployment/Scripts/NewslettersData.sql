IF NOT EXISTS(SELECT * FROM Newsletters)
BEGIN
	INSERT INTO Newsletters
	(Id, Name)
	VALUES
	(1, 'OrderProductReviewEmail')

END

GO