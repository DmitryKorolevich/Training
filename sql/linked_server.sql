EXEC sp_addlinkedserver   
   @server=N'AZURE', 
   @srvproduct=N'Azure SQL Db',
   @provider=N'SQLNCLI', 
   @datasrc=N'vw4v86cs0h.database.windows.net,1433',
   @catalog='VitalChoice.DbName';
GO
EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = 'AZURE', 
    @useself = 'FALSE', 
    @locallogin=NULL,
    @rmtuser = 'user@vw4v86cs0h',
    @rmtpassword = ''
GO

sp_testlinkedserver AZURE;
GO