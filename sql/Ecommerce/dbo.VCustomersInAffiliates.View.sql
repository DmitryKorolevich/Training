/****** Object:  View [dbo].[VCustomersInAffiliates]    Script Date: 6/25/2016 2:13:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[VCustomersInAffiliates]'))
EXEC dbo.sp_executesql @statement = N'
CREATE VIEW [dbo].[VCustomersInAffiliates]
AS
SELECT a.Id, MIN(a.Name) AS Name, count(*) AS Count FROM Affiliates a
JOIN Customers c ON a.Id=c.IdAffiliate
GROUP BY a.Id

' 
GO
