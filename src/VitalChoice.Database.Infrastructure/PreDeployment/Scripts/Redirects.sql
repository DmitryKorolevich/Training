﻿IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = 'IgnoreQuery' AND [object_id] = OBJECT_ID(N'[dbo].[Redirects]', N'U'))
BEGIN
	
	ALTER TABLE Redirects
	ADD IgnoreQuery BIT NOT NULL DEFAULT(0)
END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = 'FutureRedirectData' AND [object_id] = OBJECT_ID(N'[dbo].[Redirects]', N'U'))
BEGIN
	
	ALTER TABLE Redirects
	ADD FutureRedirectData nvarchar(max) NULL
END

GO