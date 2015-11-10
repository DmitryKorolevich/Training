IF OBJECT_ID(N'[dbo].[SPGetEngangedAffiliatesCount]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetEngangedAffiliatesCount]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE SPGetEngangedAffiliatesCount
AS
BEGIN
	SELECT 1 as Id,COUNT(DISTINCT aff.idAffiliate) As Count FROM
	(
		SELECT op.IdAffiliate
		FROM AffiliateOrderPayments AS op
		INNER JOIN Orders AS o ON o.Id = op.Id
		INNER JOIN Affiliates af ON af.Id=op.IdAffiliate
		WHERE af.StatusCode!=3
		AND o.StatusCode!=3
		AND (o.OrderStatus=2 OR o.OrderStatus=3 OR o.OrderStatus=5)
		AND o.Id <=
		ALL
		(
			SELECT opIn.Id 
			FROM AffiliateOrderPayments AS opIn
			INNER JOIN Orders AS oIn ON oIn.Id = opIn.Id
			INNER JOIN Affiliates afIn ON afIn.Id=opIn.IdAffiliate
			WHERE o.idCustomer = oIn.idCustomer 
			AND op.IdAffiliate = opIn.IdAffiliate
			AND afIn.StatusCode!=3 
			AND oIn.StatusCode!=3
			AND (oIn.OrderStatus=2 OR oIn.OrderStatus=3 OR oIn.OrderStatus=5)
		)
		GROUP BY op.IdAffiliate
		HAVING COUNT(DISTINCT op.Id) > 1
	) aff
END

GO