IF NOT EXISTS(SELECT * FROM Lookups WHERE Name='AffiliateProfessionalPractices')
BEGIN

	DECLARE @IdAffiliateProfessionalPractices INT, @IdAffiliateMonthlyEmailsSent INT, @IdAffiliatePaymentType INT

	INSERT INTO [dbo].[Lookups]
	([LookupValueType], [Name])
	VALUES
	(N'string', N'AffiliateProfessionalPractices')

	SET @IdAffiliateProfessionalPractices = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdAffiliateProfessionalPractices,	N'Medical Doctor'),
	(2, @IdAffiliateProfessionalPractices,	N'Integrative Medicine'),
	(3, @IdAffiliateProfessionalPractices,	N'Naturopathic Medicine'),
	(4, @IdAffiliateProfessionalPractices,	N'Chiropractor'),
	(5, @IdAffiliateProfessionalPractices,	N'Hospital'),
	(6, @IdAffiliateProfessionalPractices,	N'Nutritionist'),
	(7, @IdAffiliateProfessionalPractices,	N'Dentists'),
	(8, @IdAffiliateProfessionalPractices,	N'Optometrist'),
	(9, @IdAffiliateProfessionalPractices,	N'Physical Therapist'),
	(10, @IdAffiliateProfessionalPractices,	N'Fitness Center'),
	(11, @IdAffiliateProfessionalPractices,	N'Other')

	INSERT INTO [dbo].[Lookups]
	([LookupValueType], [Name])
	VALUES
	(N'string', N'AffiliateMonthlyEmailsSentOptions')

	SET @IdAffiliateMonthlyEmailsSent = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdAffiliateMonthlyEmailsSent,	N'1-10'),
	(2, @IdAffiliateMonthlyEmailsSent,	N'11-50'),
	(3, @IdAffiliateMonthlyEmailsSent,	N'50-250'),
	(4, @IdAffiliateMonthlyEmailsSent,	N'more than 250')

	INSERT INTO [dbo].[Lookups]
	([LookupValueType], [Name])
	VALUES
	(N'string', N'AffiliatePaymentTypes')

	SET @IdAffiliatePaymentType = SCOPE_IDENTITY()

	INSERT INTO [dbo].[LookupVariants]
	([Id], [IdLookup], [ValueVariant])
	VALUES
	(1, @IdAffiliatePaymentType,	N'Cash'),
	(2, @IdAffiliatePaymentType,	N'Credit')

	INSERT INTO [dbo].[AffiliateOptionTypes]
	([Name], [IdFieldType], [IdLookup], [DefaultValue])
	VALUES
	(N'BrickAndMortar', 5, NULL, N'False'),
	(N'PromoteByWebsite', 5, NULL, N'False'),
	(N'PromoteByEmails', 5, NULL, N'False'),
	(N'MonthlyEmailsSent', 3, @IdAffiliateMonthlyEmailsSent, NULL),
	(N'PromoteByFacebook', 5, NULL, N'False'),
	(N'Facebook', 4, NULL, NULL),
	(N'PromoteByTwitter', 5, NULL, N'False'),
	(N'Twitter', 4, NULL, NULL),
	(N'PromoteByBlog', 5, NULL, N'False'),
	(N'Blog', 4, NULL, NULL),
	(N'PromoteByProfessionalPractice', 5, NULL, N'False'),
	(N'ProfessionalPractice', 3, @IdAffiliateProfessionalPractices, NULL),
	(N'PromoteByDrSearsLEANCoachAmbassador', 5, NULL, N'False'),
	(N'ChecksPayableTo', 4, NULL, NULL),
	(N'Email', 4, NULL, NULL),
	(N'TaxID', 4, NULL, NULL),
	(N'Company', 4, NULL, NULL),
	(N'Phone', 4, NULL, NULL),
	(N'Fax', 4, NULL, NULL),
	(N'Address1', 4, NULL, NULL),
	(N'Address2', 4, NULL, NULL),
	(N'City', 4, NULL, NULL),
	(N'Zip', 4, NULL, NULL),
	(N'HearAbout', 4, NULL, NULL),
	(N'WebSite', 4, NULL, NULL),
	(N'Reach', 4, NULL, NULL),
	(N'Profession', 4, NULL, NULL),
	(N'Tier', 3, NULL, NULL),
	(N'Notes', 8, NULL, NULL),
	(N'PaymentType', 3, @IdAffiliatePaymentType, NULL)

END

GO