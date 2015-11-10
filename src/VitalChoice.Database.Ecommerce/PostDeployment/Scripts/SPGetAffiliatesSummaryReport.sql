IF OBJECT_ID(N'[dbo].[SPGetAffiliatesSummaryReport]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SPGetAffiliatesSummaryReport]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE SPGetAffiliatesSummaryReport
	@from datetime, @to datetime
AS
BEGIN
	SELECT @from As [From],temp.IdType, COUNT(*) As [Count], SUM(temp.Amount) [Sum] FROM
	(SELECT
	(CASE WHEN op.Id <=
			ALL(
			SELECT opIn.Id 
			FROM AffiliateOrderPayments AS opIn
			INNER JOIN Orders AS oIn ON oIn.Id = opIn.Id
			INNER JOIN Affiliates afIn ON afIn.Id=opIn.IdAffiliate
			WHERE oIn.idCustomer = o.idCustomer
			AND opIn.IdAffiliate = op.idAffiliate
			AND afIn.StatusCode!=3
			AND oIn.StatusCode!=3
			AND (oIn.OrderStatus=2 OR oIn.OrderStatus=3 OR oIn.OrderStatus=5)
			) THEN 1 ELSE 2 END) As IdType,
	op.Amount As Amount
	FROM AffiliateOrderPayments AS op
	INNER JOIN Orders AS o ON o.Id = op.Id
	INNER JOIN Affiliates af ON af.Id=op.IdAffiliate
	WHERE af.StatusCode!=3
	AND o.StatusCode!=3
	AND (o.OrderStatus=2 OR o.OrderStatus=3 OR o.OrderStatus=5)
	AND @from<=o.DateCreated
	AND @to>o.DateCreated) temp
	GROUP BY temp.IdType
END

GO