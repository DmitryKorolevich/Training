GO

IF OBJECT_ID(N'[dbo].[VCustomersWithDublicateEmails]', N'V') IS NOT NULL
DROP VIEW [dbo].[VCustomersWithDublicateEmails]
GO

CREATE VIEW [dbo].[VCustomersWithDublicateEmails]
AS

	SELECT
		Email,
		COUNT(*) [Count]
	FROM Customers
	WHERE 
		StatusCode!=3 AND Email!=''
	GROUP BY Email
	HAVING COUNT(*)>1

GO