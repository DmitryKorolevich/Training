IF NOT EXISTS(SELECT * FROM SettingOptionTypes WHERE Name='AffiliateEmailTemplateDrSears')
BEGIN

	INSERT INTO SettingOptionTypes
	(
		[Name]
		,[IdLookup]
		,[IdFieldType]
	    ,[IdObjectType]
        ,[DefaultValue]	
	)
	VALUES
	(
		'AffiliateEmailTemplateDrSears',
		NULL,
		4,
		NULL,
		N'Greetings {AFFILIATE_NAME},
<br/><br/>
Your special Dr. Sears LEAN affiliate account is ready for use. See the LEAN HTML Quick Start Codes below or <a href=“{PUBLIC_URL}affiliateaccount/login”>log into your account</a> to get other links to Vital Choice that contain your affiliate ID: {AFFILIATE_ID}. Your User Name is your e-mail address ({AFFILIATE_EMAIL}). You do not have to log in to start embedding these HTML codes; they are setup for you to use immediately.
<br/><br/>
FOR THE SPECIAL LEAN DISCOUNT FOR YOU AND YOUR CLIENTS PLEASE DIRECT READERS TO: {PUBLIC_URL}content/sears-lean?idaffiliate={AFFILIATE_ID} This hyperlink includes you affiliate code so any sales from this link would be counted as a commission.
<br/><br/>
Here is the same link with your affiliate number embedded in the code: <a href=“{PUBLIC_URL}content/sears-lean?idaffiliate={AFFILIATE_ID}”>Vital Choice LEAN Program</a> 
<br/><br/>
When your readers make their purchase using the discount code they get 10% off their purchase and you get 8% of the first sale. Then all lifetime purchases made by that customer (by calling us, using vitalchoice.com, or via your product and banner links) earn you 5% of each sale. You get a commission when your readers continue to use the LEAN discount code or respond to other special offers such as our Weekly Bonus Offers promoted through the Vital Choice Newsletter and through occasional Discount Codes. For general Affiliate Program information see the <a href=“{PUBLIC_URL}content/web-affiliate-program”>Program overview</a> for more information.
<br/><br/>
Vital Choice extends this commission and the LEAN discount to your personal purchase, so that you can try our products and share your first hand experience when reaching out to new customers. To set this, you simply click the above link and make a purchase.  Just like your customers, you only need to do it once to start earning commissions on all future purchases. 
<br/><br/>
Vital Choice also offers a <a href=“{PUBLIC_URL}content/wholesale-registration”>Wholesale Program</a> for purchasing larger volumes for your company.<br/>
We appreciate your advocacy and interest in promoting wild seafood nutrition. We are looking forward to working together.
<br/><br/>
Questions or comments? Contact us at <a href=“mailto:”affiliatesupport@vitalchoice.com”>affiliatesupport@vitalchoice.com</a>
<br/><br/>
Kind regards,<br/>
The Vital Choice Team'
	)

	DELETE SettingOptionTypes WHERE Name='AffiliateEmailTemplate'

	INSERT INTO SettingOptionTypes
	(
		[Name]
		,[IdLookup]
		,[IdFieldType]
	    ,[IdObjectType]
        ,[DefaultValue]	
	)
	VALUES
	(
		'AffiliateEmailTemplate',
		NULL,
		4,
		NULL,
		N'{AFFILIATE_NAME},
<br/><br/>
Your Vital Choice Affiliate account is ready for use. You can log into your account at {PUBLIC_URL}affiliateaccount/login to retrieve special links of Vital Choice text and banner ads that contain your affiliate ID: {AFFILIATE_ID}
<br/><br/>
Program overview: {PUBLIC_URL}content/web-affiliate-program
<br/><br/>
We appreciate your advocacy and interest in promoting wild seafood nutrition. Looking forward to working together. 
<br/><br/>
Questions or comments? Contact us at affiliatesupport@vitalchoice.com 
<br/><br/>
Kind regards,<br/>
The Vital Choice Team'
	)

END

GO
