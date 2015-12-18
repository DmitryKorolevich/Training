IF OBJECT_ID(N'dbo.ContentAreas', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ContentAreas](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](250) NOT NULL,
		[Template] [nvarchar](max) NULL,
		[StatusCode] [int] NOT NULL,
		[Created] [datetime] NOT NULL,
		[Updated] [datetime] NOT NULL,
		[IdEditedBy] [int] NULL,
	 CONSTRAINT [PK_ContentAreas] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)) 

	ALTER TABLE [dbo].[ContentAreas]  WITH CHECK ADD  CONSTRAINT [FK_ContentAreas_AspNetUsers] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[AspNetUsers] ([Id])

	ALTER TABLE [dbo].[ContentAreas] CHECK CONSTRAINT [FK_ContentAreas_AspNetUsers]

	ALTER TABLE [dbo].[ContentAreas]  WITH CHECK ADD  CONSTRAINT [FK_ContentAreas_RecordStatusCodes] FOREIGN KEY([StatusCode])
	REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])

	ALTER TABLE [dbo].[ContentAreas] CHECK CONSTRAINT [FK_ContentAreas_RecordStatusCodes]

END

UPDATE [dbo].[ContentAreas] 
SET [Name] = N'Secondary Wholesale Menu'
WHERE [Name] = N'Secondary Menu for Wholesale'

GO

UPDATE [dbo].[ContentAreas] 
SET [Name] = N'Footer Links'
WHERE [Name] = N'Footer Nav'

GO

UPDATE [dbo].[ContentAreas] 
SET [Name] = N'Home Bottom Banners'
WHERE [Name] = N'Home Banners'

GO

UPDATE [dbo].[ContentAreas] 
SET [Name] = N'Home Banner Carousel'
WHERE [Name] = N'Home Carousel'

GO

UPDATE [dbo].[ContentAreas] 
SET [Name] = N'Secondary Retail Menu'
WHERE [Name] = N'Secondary Menu'

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas])
BEGIN
	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Secondary Retail Menu', N'
					<li>
						<a href="#">New</a>
					</li>
					<li>
						<a href="#">Signature Products</a>
					</li>
					<li>
						<a href="#">Special Offers</a>
					</li>
					<li>
						<a href="#">Recipes</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">All Recipes</a>
									</li>
									<li>
										<a href="#">Recipe Videos</a>
									</li>
									<li>
										<a href="#">Seafood Cooking Tips</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<a href="#">Why Vital Choice?</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">About Us</a>
									</li>
									<li>
										<a href="#">Our Mission</a>
									</li>
									<li>
										<a href="#">Testimonials</a>
									</li>
									<li>
										<a href="#">Giving Back</a>
									</li>
									<li>
										<a href="#">News Room</a>
									</li>
									<li>
										<a href="#">Vital Green™</a>
									</li>
									<li>
										<a href="#">Sustainability</a>
									</li>
									<li>
										<a href="#">Product Purity</a>
									</li>
									<li>
										<a href="#">Randy''s Podcasts</a>
									</li>
									<li>
										<a href="#">Customer Rewards</a>
									</li>
									<li>
										<a href="#">Frequent Questions (FAQs)</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<a href="#">Health & Nutrition</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">Omega-3 Basics</a>
									</li>
									<li>
										<a href="#">Seafood Benefits</a>
									</li>
									<li>
										<a href="#">Omega-3/6 Balance</a>
									</li>
									<li>
										<a href="#">Healthy Mom & Baby</a>
									</li>
									<li>
										<a href="#">Seafood Purity & Safety</a>
									</li>
									<li>
										<a href="#">Recommended Reading</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<a href="#">Catalog</a>
					</li>
					<li class="last-child">
						<a href="#">Newsletter</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">Sign Up</a>
									</li>
									<li>
										<a href="#">Archives</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
				', 2, GETDATE(), GETDATE())

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Home Banner Carousel', N'<a href="#"><img src="/Assets/images/King_C3_cover_slide_v2.jpg" alt=""/></a>
		<a href="#"><img src="/Assets/images/chicken-hp-banner-v6.jpg" alt=""/></a>
		<a href="#"><img src="/Assets/images/HP_banner_bonebroth_web.jpg" alt=""/></a>
		<a href="#"><img src="/Assets/images/HP_banner_sardines_2.jpg" alt=""/></a>', 2, GETDATE(), GETDATE())

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Home Bottom Banners', N'<div>
			<img src="/Assets/images/homebanner_sausagebaconburger_v2.jpg" usemap="#btmbanner2">
		</div>
		<map name="btmbanner2" id="btmbanner2">
			<area shape="rect" coords="23,0,219,260" href="#">
			<area shape="rect" coords="227,14,440,252" href="#">
			<area shape="rect" coords="477,14,690,252" href="#">
			<area shape="rect" coords="728,14,941,252" href="#">
		</map>
		<div>
			<a href="#" target="_blank"><img src="/Assets/images/labor-day-hp-v3.jpg"></a>
		</div>
		<div>
			<img src="/Assets/images/canned-banner-v3.jpg" usemap="#btmbanner1">
		</div>
		<map name="btmbanner1" id="btmbanner1">
			<area shape="rect" coords="0,0,233,260" href="#">
			<area shape="rect" coords="234,14,447,252" href="#">
			<area shape="rect" coords="487,14,700,252" href="#">
			<area shape="rect" coords="738,14,951,252" href="#">
		</map>', 2, GETDATE(), GETDATE())
END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name] = N'Footer Links')
BEGIN
	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Footer Links', N'<div class="footer-links-column">
						<span>Need Help?</span>
						<ul>
							<li><a href="#">Customer Service</a></li>
							<li><a href="#">Request a Catalog</a></li>
							<li><a href="#">Customer Rewards</a></li>
							<li><a href="#">Returns & Exchanges</a></li>
							<li><a href="#">Shipping Information</a></li>
						</ul>
					</div>
					<div class="footer-links-column">
						<span>Our Company</span>
						<ul>
							<li><a href="#">About Us</a></li>
							<li><a href="#">Guarantee</a></li>
							<li><a href="#">Testimonials</a></li>
							<li><a href="#">Purity Standards</a></li>
							<li><a href="#">Sustainability Policy</a></li>
							<li><a href="#">Vital Green™</a></li>
							<li><a href="#">News Room</a></li>
							<li><a href="#">Giving Back</a></li>
							<li><a href="#">Privacy Policy</a></li>
							<li><a href="#">Careers</a></li>
						</ul>
					</div>
					<div class="footer-links-column">
						<span>Programs</span>
						<ul>
							<li><a href="#">Affiliate Accounts</a></li>
							<li><a href="#">Customer Rewards</a></li>
							<li><a href="#">Wholesale Accounts</a></li>
						</ul>
					</div>
					<div class="footer-links-column">
						<span>Learn</span>
						<ul>
							<li><a href="#">FAQs</a></li>
							<li><a href="#">Recipes</a></li>
							<li><a href="#">Seafood Cooking Tips</a></li>
							<li><a href="#">Newsletter Archive</a></li>
							<li><a href="#">Healthy Mom & Baby</a></li>
							<li><a href="#">Seafood Health Benefits</a></li>
							<li><a href="#">Omega-3 Facts & Sources</a></li>
						</ul>
					</div>', 2, GETDATE(), GETDATE())

	ALTER TABLE [dbo].[ContentAreas]
	ADD CONSTRAINT UQ_ContentArea UNIQUE(Name)
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name] = N'Secondary Wholesale Menu')
BEGIN
	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Secondary Wholesale Menu', N'
					<li>
						<a href="#">New</a>
					</li>
					<li>
						<a href="#">Special Offers</a>
					</li>
					<li>
						<a href="#">Recipes</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">All Recipes</a>
									</li>
									<li>
										<a href="#">Recipe Videos</a>
									</li>
									<li>
										<a href="#">Seafood Cooking Tips</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<a href="#">Why Vital Choice?</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">About Us</a>
									</li>
									<li>
										<a href="#">Our Mission</a>
									</li>
									<li>
										<a href="#">Testimonials</a>
									</li>
									<li>
										<a href="#">Giving Back</a>
									</li>
									<li>
										<a href="#">News Room</a>
									</li>
									<li>
										<a href="#">Vital Green™</a>
									</li>
									<li>
										<a href="#">Sustainability</a>
									</li>
									<li>
										<a href="#">Product Purity</a>
									</li>
									<li>
										<a href="#">Randy''s Podcasts</a>
									</li>
									<li>
										<a href="#">Customer Rewards</a>
									</li>
									<li>
										<a href="#">Frequent Questions (FAQs)</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<a href="#">Health & Nutrition</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">Omega-3 Basics</a>
									</li>
									<li>
										<a href="#">Seafood Benefits</a>
									</li>
									<li>
										<a href="#">Omega-3/6 Balance</a>
									</li>
									<li>
										<a href="#">Healthy Mom & Baby</a>
									</li>
									<li>
										<a href="#">Seafood Purity & Safety</a>
									</li>
									<li>
										<a href="#">Recommended Reading</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<a href="#">Wholesale</a>
					</li>
					<li>
						<a href="#">Catalog</a>
					</li>
					<li class="last-child">
						<a href="#">Newsletter</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">Sign Up</a>
									</li>
									<li>
										<a href="#">Archives</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
				', 2, GETDATE(), GETDATE())
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name] = N'Secondary Wholesale Menu' AND [Updated] = N'09/09/2015')
BEGIN
	UPDATE [dbo].[ContentAreas]
	SET [Updated] = N'09/09/2015',
		[Template] = N'
					<li>
						<a href="#">New</a>
					</li>
					<li>
						<a href="#">Signature Products</a>
					</li>
					<li>
						<a href="#">Special Offers</a>
					</li>
					<li>
						<a href="#">Recipes</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">All Recipes</a>
									</li>
									<li>
										<a href="#">Recipe Videos</a>
									</li>
									<li>
										<a href="#">Seafood Cooking Tips</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<a href="#">Why Vital Choice?</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">About Us</a>
									</li>
									<li>
										<a href="#">Our Mission</a>
									</li>
									<li>
										<a href="#">Testimonials</a>
									</li>
									<li>
										<a href="#">Giving Back</a>
									</li>
									<li>
										<a href="#">News Room</a>
									</li>
									<li>
										<a href="#">Vital Green™</a>
									</li>
									<li>
										<a href="#">Sustainability</a>
									</li>
									<li>
										<a href="#">Product Purity</a>
									</li>
									<li>
										<a href="#">Randy''s Podcasts</a>
									</li>
									<li>
										<a href="#">Customer Rewards</a>
									</li>
									<li>
										<a href="#">Frequent Questions (FAQs)</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<a href="#">Health & Nutrition</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">Omega-3 Basics</a>
									</li>
									<li>
										<a href="#">Seafood Benefits</a>
									</li>
									<li>
										<a href="#">Omega-3/6 Balance</a>
									</li>
									<li>
										<a href="#">Healthy Mom & Baby</a>
									</li>
									<li>
										<a href="#">Seafood Purity & Safety</a>
									</li>
									<li>
										<a href="#">Recommended Reading</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<a href="#" class="highlighted-sec-menu">Wholesale</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">Canned Wild Seafood</a>
									</li>
									<li>
										<a href="#">Dietary Supplements</a>
									</li>
									<li>
										<a href="#">Organic Foods</a>
									</li>
									<div class="sub-sec-menu-separator"></div>
									<li>
										<a href="#">Wholesale Promotions</a>
									</li>
									<li>
										<a href="#">FAQ</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li class="last-child">
						<a href="#">Newsletter</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">Sign Up</a>
									</li>
									<li>
										<a href="#">Archives</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
				'
	WHERE [Name] = N'Secondary Wholesale Menu'
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name]='Affiliate Ad Banners')
BEGIN

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Affiliate Ad Banners', N'<script>
    window.addEventListener("load", function() {
        var element = document.createElement("script");
        element.src = "/app/modules/affiliateprofile/adBanners.js";
        document.body.appendChild(element);
    }, false);
</script>
<div class="border-form-left ad-banners">
    <h4 class="h-step1">Step 1 - Select Banner Size</h4>
    <img class="main-banner" src="/Assets/images/adbanners/logo.jpg">
    <div class="banner-size banner-size-728" data-banner-size="728" data-width="728" data-height="90">
        <strong>728 x 90</strong>
    </div>
    <div class="banner-size banner-size-125" data-banner-size="125">
        <strong>125 x 125</strong>
    </div>
    <div class="banner-size banner-size-600" data-banner-size="600" data-width="600" data-height="100">
        <strong>600 x 100</strong>
    </div>
	<div class="clear"></div>
	<div class="banner-size banner-size-160" data-banner-size="160">
        <strong>160 x 600</strong>
    </div>
	<div class="banner-size banner-size-120-600" data-banner-size="120-600">
        <strong>120 x 600</strong>
    </div>
    <div class="banner-size banner-size-300-250" data-banner-size="300-250">
        <strong>300 x 250</strong>
    </div>
    <div class="banner-size banner-size-83" data-banner-size="83">
        <strong>83 x 31</strong>
    </div>
   <div class="banner-size banner-size-250" data-banner-size="250">
        <strong>250 x 250</strong>
    </div>
    <div class="banner-size banner-size-120-90" data-banner-size="120-90">
        <strong>120 x 90</strong>
    </div>
    <div class="banner-size banner-size-234" data-banner-size="234">
        <strong>234 x 60</strong>
    </div>
    <div class="clear"></div>
    <div class="banner-size banner-size-468" data-banner-size="468">
        <strong>468 x 60</strong>
    </div>
    <div class="banner-size banner-size-300-100" data-banner-size="300-100">
        <strong>300 x 100</strong>
    </div>
	<div class="clear"></div>
	<h4 class="h-step2">Step 2 - Select Banner</h4>
	<div class="banners banners-728">
	    <img src="/Assets/images/adbanners/VC_canned_728x90_B-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VC_salmon728x90_A-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VC-canned_728x90_A-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VC_grilling_728x90_A-5-15.jpg">
	</div>
	<div class="banners banners-125">
	    <img src="/Assets/images/adbanners/VC-affiliate-ad-6-2014-125x125-eat-better-feel-A.jpg">
	    <img src="/Assets/images/adbanners/VC-affiliate-ad-6-2014-125x125-eat-wild-A.jpg">
	    <img src="/Assets/images/adbanners/VC-affiliate-ad-6-2014-125x125-healing-power-B.jpg">
	    <img src="/Assets/images/adbanners/VC-affiliate-ad-6-2014-125x125-vitality-A.jpg">
	</div>
	<div class="banners banners-600">
	    <img src="/Assets/images/adbanners/VC_canned600x100_A-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VC_supplements_600x100_A 2-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/vc-600x100-bf-cm.jpg">
	</div>
	<div class="banners banners-160">
	    <img src="/Assets/images/adbanners/VC_canned_160x600_A_FDS-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VC_canned_160x600_C_FDS-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VC_supplements_160x600_A-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VC_supplements_160x600_B-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VCSalmon160x600_A_-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VC_grilling_160x600_A-5-15-v2.jpg">
	</div>
	<div class="banners banners-120-600">
	    <img src="/Assets/images/adbanners/NY-120x600.jpg">
	</div>
	<div class="banners banners-300-250">
	    <img src="/Assets/images/adbanners/VC_canned_300x250_A-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VC_salmon_300x250_A-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VC_canned_300x250_B-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VC_supplements_300x250_A-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VC_supplements_300x250_B-4-30-13.jpg">
	    <img src="/Assets/images/adbanners/VC_grilling_300x250_A-5-15.jpg">
	</div>
	<div class="banners banners-83">
	    <img src="/Assets/images/adbanners/Ad_88_31.jpg">
	</div>
	<div class="banners banners-250">
	    <img src="/Assets/images/adbanners/VC-affiliate-ad-6-2014-250x250-eat-better-A.jpg">
	    <img src="/Assets/images/adbanners/VC-affiliate-ad-6-2014-250x250-eat-wild-A.jpg">
	    <img src="/Assets/images/adbanners/VC-affiliate-ad-6-2014-250x250-healing-power-A.jpg">
	    <img src="/Assets/images/adbanners/VC-affiliate-ad-6-2014-250x250-vitality-A.jpg">
	</div>
	<div class="banners banners-120-90">
	    <img src="/Assets/images/adbanners/Ad_120_90.jpg">
	</div>
	<div class="banners banners-234">
	    <img src="/Assets/images/adbanners/NY-234x60.jpg">
	</div>
	<div class="banners banners-468">
	    <img src="/Assets/images/adbanners/NY-468x60.jpg">
	</div>
	<div class="banners banners-300-100">
	    <img src="/Assets/images/adbanners/VC_grilling_300x100_A-5-15.jpg">
	</div>
	<h4 class="h-step3">Step 3 - Banner Code</h4>
	<textarea class="code-area" rows="5"></textarea>
</div>
', 2, GETDATE(), GETDATE())

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name]='Affiliate Landing Area')
BEGIN

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Affiliate Landing Area', N'<h4>Vital Choice Web Affiliate Program</h4>
<p>
    Welcome {Name}, 
</p>
<p>
    Thank you for joining the Vital Choice Web Affiliate Program. We invite you to see the Quick Start Guide below built with your special links for 
    Vital Choice products, content, and banner ads that contain your affiliate ID: {Id}. These are set up for you to use immediately.
</p>
<p>
    The simplest way to direct your visitors to our products and begin earning commission is to include this HTML code link in your website, 
    blog and emails: <a href="https://{PublicHost}/?idaffiliate={Id}">https://{PublicHost}/?idaffiliate={Id}</a>
</p>
<p>
    Here is the same link with your affiliate number embedded in the code: <a href="https://{PublicHost}/?idaffiliate={Id}">Vital Choice</a>
</p>
<p>
    When your readers make their first purchase, you will earn {CommissionFirst}% of the sale. From that point on, all additional purchases made by that customer 
    (by calling us, using vitalchoice.com, or via your product and banner links) earn you {CommissionAll}% of each sale. You also receive a commission when your
    readers respond to special offers such as our Weekly Bonus Offers promoted through the Vital Choice Newsletter and through occasional Discount Codes.
    See the Program Overview for more information.
</p>
<p>
    Vital Choice extends this commission and the First Order 10% Discount to your personal purchase, so that you can try our products and share your first-hand experience when reaching out to new customers.
    To set this, simply click on any of these links, use the Discount Code VCAF10 and make a purchase. As with your customers, you only need to do this once to start earning commissions on all future purchases. 
</p>
<p>
    Vital Choice also offers a Wholesale Program for purchasing larger volumes for your company.
</p>
<p>
    We appreciate your advocacy and interest in promoting wild seafood nutrition. We look forward to working together.
</p>
<p>
    Please feel free to contact us anytime with questions or comments at <a href="mailto:affiliatesupport@vitalchoice.com ">affiliatesupport@vitalchoice.com</a>
</p>
<p>
    Yours in good health,<br/>
    The Vital Choice Team
</p>', 2, GETDATE(), GETDATE())

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name]='Alert Top Section')
BEGIN

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Alert Top Section', N'<a href="#"><img src="/Assets/images/holiday-shipping-v3.jpg" alt=""/></a>', 2, GETDATE(), GETDATE())

END

GO