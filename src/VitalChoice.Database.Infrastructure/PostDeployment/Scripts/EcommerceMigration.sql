﻿IF db_id('VitalChoice.Ecommerce') is not null
BEGIN
	--IF OBJECT_ID(N'[VitalChoice.Ecommerce].[dbo].[Users]', N'U') IS NULL
	--BEGIN
		IF NOT EXISTS (SELECT [Id] FROM [VitalChoice.Ecommerce].[dbo].[Users])
		BEGIN
			INSERT INTO [dbo].[Users]
			SELECT [Id] FROM [dbo].[AspNetUsers]
		END
	--END
END