IF NOT EXISTS(SELECT * FROM AdminTeams)
BEGIN

	INSERT AdminTeams
	(Name)
	VALUES
	('Crystal Creek'),
	('Taction'),
	('Vital Choice')

END

GO
