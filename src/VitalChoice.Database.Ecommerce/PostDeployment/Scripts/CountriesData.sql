IF EXISTS(SELECT * FROM Countries WHERE StatusCode=1)
BEGIN

	UPDATE Countries
	SET 
		IdVisibility=2,
		StatusCode=2
	WHERE StatusCode=1

END

GO

IF EXISTS(SELECT * FROM States WHERE StatusCode=1)
BEGIN

	UPDATE States
	SET 
		StatusCode=2
	WHERE StatusCode=1

END

GO