EXEC sp_addlinkedserver   
   @server=N'PARTS', 
   @srvproduct=N'Firebird/InterBase(r) driver',
   @provider=N'MSDASQL', 
   @provstr=N'Firebird/InterBase(r) driver',
   @datasrc=N'd:\DB\dddd\AutoDealer_AC.fdb'
GO
EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = 'PARTS', 
    @useself = 'TRUE',
    @rmtuser = 'SYSDBA',
    @rmtpassword = 'masterkey'
GO

sp_testlinkedserver PARTS;
GO