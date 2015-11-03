IF ('NO' =(select TOP 1 IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS 
where TABLE_NAME ='Discounts' and COLUMN_NAME ='Assigned'))
BEGIN
ALTER TABLE Discounts ALTER COLUMN Assigned INT NULL

UPDATE Discounts
SET Assigned=NULL
WHERE Assigned=1

UPDATE Discounts
SET Assigned=1
WHERE Assigned=3

ALTER TABLE dbo.Discounts ADD CONSTRAINT
	FK_Discounts_CustomerTypes FOREIGN KEY
	(
	Assigned
	) REFERENCES dbo.CustomerTypes
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

END

GO