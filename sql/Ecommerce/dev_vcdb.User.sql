/****** Object:  User [dev_vcdb]    Script Date: 6/25/2016 2:09:59 PM ******/
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'dev_vcdb')
CREATE USER [dev_vcdb] FOR LOGIN [dev_vcdb_login] WITH DEFAULT_SCHEMA=[dbo]
GO
