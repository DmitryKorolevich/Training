IF db_id('VitalChoice.Ecommerce') is not null
BEGIN
	IF EXISTS(SELECT 1 FROM [VitalChoice.Ecommerce].sys.tables WHERE NAME = 'Users')
	BEGIN
		IF NOT EXISTS (SELECT [Id] FROM [VitalChoice.Ecommerce].[dbo].[Users])
		BEGIN
			INSERT INTO [dbo].[Users]
			SELECT [Id] FROM [dbo].[AspNetUsers]
		END
	END
END