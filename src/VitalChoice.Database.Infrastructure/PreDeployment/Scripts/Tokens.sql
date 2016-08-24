IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('Tokens') AND type = 1)
BEGIN
	ALTER TABLE Tokens
	ADD CONSTRAINT PK_Tokens PRIMARY KEY (IdToken)
END