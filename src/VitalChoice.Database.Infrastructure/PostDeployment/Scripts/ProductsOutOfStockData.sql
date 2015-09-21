IF (250=(SELECT CHARACTER_MAXIMUM_LENGTH 
FROM INFORMATION_SCHEMA.COLUMNS
WHERE 
     TABLE_NAME = 'AppSettings' AND 
     COLUMN_NAME = 'Value'))
	 BEGIN
	 ALTER TABLE AppSettings ALTER COLUMN Value NVARCHAR(MAX) NULL
	 END

GO

IF((SELECT COUNT(*) FROM [dbo].[AppSettings] WHERE Name='ProductOutOfStockEmailTemplate')=0)
BEGIN
INSERT INTO [dbo].[AppSettings]
           ([Id]
           ,[Name]
           ,[Value])
     VALUES
           (4
           ,'ProductOutOfStockEmailTemplate'
           ,'Hello {CUSTOMER_NAME},
<br/><br/>
Good news! The item {PRODUCT_NAME} you had inquired about is now available. We''re delighted to have it back in stock and invite you to use this link to the product page where you can place your order:
<br/><br/>
{PRODUCT_URL}
<br/><br/>
Thank you for choosing Vital Choice. And please let me know if I can help with anything else.
<br/><br/>
Karen Long<br/>
Customer Service and Quality Liaison<br/>
Vital Choice Wild Seafood & Organics<br/>
800-608-4825')

END

GO


