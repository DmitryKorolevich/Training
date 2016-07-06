IF NOT EXISTS(SELECT * FROM [SettingOptionTypes])
BEGIN

	INSERT INTO [dbo].[SettingOptionTypes]
	([Name], [IdFieldType], [IdLookup], [DefaultValue])
	VALUES
	(N'GlobalPerishableThreshold', 1, NULL, N'55'),
	(N'CreditCardAuthorizations', 5, NULL, N'False'),
	(N'HealthwisePeriodMaxItemsCount', 3, NULL, N'9'),
	(N'ProductOutOfStockEmailTemplate', 4, NULL, N'Hello {CUSTOMER_NAME},
<br/><br/>
Good news! The item {PRODUCT_NAME} you had inquired about is now available. We''re delighted to have it back in stock and invite you to use this link to the product page where you can place your order:
<br/><br/>
{PRODUCT_URL}
<br/><br/>
Thank you for choosing Vital Choice. And please let me know if I can help with anything else.
<br/><br/>
Karen Long<br/>
Customer Service and Quality Liaison<br/>
Vital Choice Wild Seafood & Organics<br/>
800-608-4825'),
	(N'AffiliateEmailTemplate', 4, NULL, N'{AFFILIATE_NAME},

Your Vital Choice Affiliate account is ready for use. You can log into your account at {PUBLIC_URL}affiliateaccount/login to retrieve special links of Vital Choice text and banner ads that contain your affiliate ID: {AFFILIATE_ID}

Program overview: {PUBLIC_URL}content/web-affiliate-program

We appreciate your advocacy and interest in promoting wild seafood nutrition. Looking forward to working together. 

Questions or comments? Contact us at affiliatesupport@vitalchoice.com 

Kind regards,
The Vital Choice Team')

END

GO