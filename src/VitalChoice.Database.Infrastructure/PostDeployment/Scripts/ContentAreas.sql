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

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name]='Top Menu')
BEGIN

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Top Menu', N'<div class="top-menu-wrapper">
    <ul class="top-menu">
    <li><a href="#" class="drop">Shop</a>
    
        
        <div class="dropdown_2columns">
        <div class="shadowhide"></div>
            <div class="col_1">
                
                <span class="ddtitle">FISH</span>
                <ul>
                    
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=279">Salmon</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=312">Cod</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=310">Tuna</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=309">Halibut</a></li>
                    <li><a href="/shop/pc/viewPrd.asp?idproduct=2139">Petrale Sole</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=311">Sablefish</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=213">Smoked Fish & Lox</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=270">Salmon Burgers & Sausage</a></li>
                </ul>
                <span class="ddtitle">SHELLFISH</span>
                <ul>
                    <li><a href="/shop/pc/viewPrd.asp?idproduct=571">Wild Shrimp</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=301">Scallops</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=807">Clams, Oysters & Mussels</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=809">Lobster</a></li>
                    <li><a href="/shop/pc/viewPrd.asp?idproduct=1452">Calamari</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=850">King Crab</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=300">See All</a></li>
                </ul>
                
            </div>
    
            <div class="col_1">
              
                <span class="ddtitle">CANNED & POUCHED  SEAFOOD</span>
                <ul>
                    
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=851">Anchovies</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=223">Sardines</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=222">Salmon</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=224">Tuna</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=225">Canned Samplers</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=221">See All</a></li>
                    
                </ul>
                
                <span class="ddtitle">ORGANIC FOOD & SEASONINGS</span>
                <ul>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=746">Soups & Meals</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=865">Organic Bone Broth</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=264">Chocolate</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=254">Seasonings</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=867">Seaweed & Kelp</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=275">Dried Fruit</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=234">Frozen Berries</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=274">Nuts</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=257">Oils & Vinegars</a></li>
                    <li><a href="/shop/pc/viewPrd.asp?idproduct=1358">Garlic Flower</a></li>
                </ul>
                <span class="ddtitle">COOKS GIFTS & BOOKS</span>
                <ul>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=639">Gift Certificates</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=260">Kitchen Tools</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=261">Books</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=260">See All</a></li>
                </ul>
            </div>
            <div class="col_1">
                
                <span class="ddtitle">MEAT</span>
                <ul>
                    <li><a href="/shop/pc/viewcategories.asp?idcategory=878">Grass-Fed Bison</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=791">Grass Fed Beef</a></li>
                    <li><a href="/shop/pc/viewcategories.asp?idcategory=868">Heritage Chicken</a></li>
                    
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=865">Bone Broth</a></li>
                    
                    
                </ul>
                <span class="ddtitle">OMEGA 3''s & SUPPLEMENTS</span>
                <ul>
                    
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=239">Omega-3 Salmon Oil</a></li>
                    <li><a href="/shop/pc/viewPrd.asp?idproduct=2334">DHA Prenatal Therapy</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=242">Daily Packs</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=642">Omega-3 Krill Oil</a></li>
                    <li><a href="/shop/pc/viewPrd.asp?idproduct=1421">Vital Omega-3 & 6 Test&#8482; Kit</a></li>
                    <li><a href="/shop/pc/viewPrd.asp?idproduct=1547">Curcumin in Wild Salmon Oil</a></li>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=238">See All</a></li>
                    
                </ul>
                <span class="ddtitle">SAMPLERS & GIFT PACKS</span>
                <ul>
                    <li><a href="/shop/pc/viewCategories.asp?idCategory=703">Gift Packs</a></li>
                    <li><a href="/shop/pc/viewcategories.asp?idcategory=802">Health Advisor Packs</a></li>
                    <li><a href="/shop/pc/viewcategories.asp?idcategory=801">Samplers</a></li>
                </ul>
            </div>
            <div class="col_1">
                
                <div class="navsaleimage"><a href="/shop/pc/viewCategories.asp?idCategory=842"><img src="/Assets/images/nav-ad-230x180-A.jpg" border="0" /></a></div>
                
                <span class="ddtitle"><a href="/shop/pc/viewCategories.asp?idCategory=232">VALUE PICKS</a></span>
                <span class="ddtitle"><a href="/shop/pc/viewCategories.asp?idCategory=743">TOP SELLERS</a></span>
                <span class="ddtitle"><a href="/shop/pc/viewcategories.asp?idcategory=880">RANDY''S PICKS</a></span>
                <span class="ddtitle"><a href="/shop/pc/viewCategories.asp?idCategory=845">NEW PRODUCTS</a></span>
                
                
                
            </div>
        </div>
    </li>

    <li><a href="#" class="drop">Why Vital Choice</a>
        <div class="dropdown_2columns second">
            <div class="shadowhide"></div>
            <div class="col_1">
                
                <span class="ddtitle">WHO WE ARE :: OUR STORY</span>
                <ul>
                    
                    <li><a href="/shop/pc/viewContent.asp?idpage=2">Why Vital Choice</a></li>
                    <li><a href="/shop/pc/viewContent.asp?idpage=6">Our Mission</a></li>
                    
                    
                </ul>
                
                
            </div>
    
            <div class="col_1">
              
                <span class="ddtitle">WHY WE ARE DIFFERENT</span>
                <ul>
                    
                    <li><a href="/shop/pc/viewContent.asp?idpage=12">Purity Promise</a></li>
                    <li><a href="/shop/pc/viewContent.asp?idpage=7">Vital Green&#8482;</a></li>
                    <li><a href="/shop/pc/viewContent.asp?idpage=3">Sustainability</a></li>
                    
                    
                </ul>
                
                
            </div>
            <div class="col_1">
                
                <span class="ddtitle">WHAT WE BELIEVE</span>
                <ul>
                    
                    <li><a href="/shop/pc/viewContent.asp?idpage=639">Randy''s Podcasts</a></li>
                    <li><a href="/shop/pc/viewContent.asp?idpage=152">Giving Back</a></li>
                    <li><a href="/shop/pc/viewContent.asp?idpage=4">Testimonials</a></li>
               
                    
                </ul>
                
                
            </div>
            <div class="col_1">
                <div class="navsaleimage2"><a href="http://www.bcorporation.net/community/vital-choice" target="_blank"><img src="/Assets/images/nav-ad-230x130-B-Corp.jpg" border="0" /></a></div>
                
                
                
            </div>
        </div>
    </li>

    <li><a href="#" class="drop">Learn</a>
        <div class="dropdown_2columns third">
            <div class="shadowhide"></div>
            <div class="col_1">
                
                <span class="ddtitle">RECOMMENDED READING</span>
                <ul>
                    
                    <li><a href="/shop/pc/newsletter-signup.asp">Newsletter</a></li>
                    <li><a href="/shop/pc/viewPrd.asp?idproduct=1421">Take the VitalTest</a></li>
                    
                    <li><a href="/shop/pc/articles.asp?cat=672">Vitamin D</a></li>
                    <li><a href="/shop/pc/articles.asp">Newsletter Articles Archive</a></li>
                    <li><a href="/shop/pc/faq.asp">FAQ''s</a></li>
                    
                </ul>
                
                
            </div>
    
            <div class="col_1">
              
                <span class="ddtitle">HEALTH & NUTRITION</span>
                <ul>
                    <li><a href="/shop/pc/viewContent.asp?idpage=238">Omega 3 Basics</a></li>
                    
                    <li><a href="/shop/pc/viewContent.asp?idpage=10">Seafood Benefits</a></li>
                    <li><a href="/shop/pc/viewContent.asp?idpage=208">Omega3/6 Balance</a></li>
                    <li><a href="/shop/pc/viewContent.asp?idpage=14">Healthy Mom & Baby</a></li>
                    
                    
                </ul>
                
                
            </div>
            <div class="col_1">
                
                <span class="ddtitle">ABOUT VITAL CHOICE PRODUCTS</span>
                <ul>
                    <li><a href="/shop/pc/viewContent.asp?idpage=713">How to: Prepare Seafood</a></li>
                    <li><a href="/shop/pc/viewContent.asp?idpage=2#Organic">Kosher & Organic Certification</a></li>
                    <li><a href="/shop/pc/viewContent.asp?idpage=2#Flash-Frozen">The Flash-Frozen Advantage</a></li>
                    <li><a href="/shop/pc/viewContent.asp?idpage=2#Coastal%20Communities">Support for Coastal Communities</a></li>
                    <li><a href="/shop/pc/viewContent.asp?idpage=2#Community%20Connections">Vital Community Connections</a></li>
                    <li><a href="/shop/pc/viewContent.asp?idpage=12">Seafood Purity and Safety</a></li>
                    <li><a href="/shop/pc/viewContent.asp?idpage=2#Superior%20Salmon">Superior Salmon, Naturally</a></li>
                </ul>
                
                
            </div>
            <div class="col_1">
                <div class="navsaleimage2"><a href="/shop/pc/viewCategories.asp?idCategory=865"><img src="/Assets/images/nav-ad-230x180-B.jpg" border="0" /></a></div>
                
                
                
            </div>
        </div>
    </li>
    
    <li><a href="#" class="drop">Cook</a>
        <div class="dropdown_2columns fourth">
            <div class="shadowhide"></div>
            <div class="col_1">
                <span class="ddtitle"><a href="/shop/pc/viewrecipes.asp?RecipeId=1490">COOKING VIDEOS</a></span>
                <span class="ddtitle">CELEBRITY CHEF VIDEOS</span>
                <ul>
                    
                    <li><a href="/shop/pc/recipes.asp?idCategory=833">Becky Selengut</a></li>
                    <li><a href="/shop/pc/recipes.asp?idCategory=834">Myra Kornfeld</a></li>
                    <li><a href="/shop/pc/recipes.asp?idCategory=835">Rebecca Katz</a></li>
                    
                </ul>
                
                
            </div>
    
            <div class="col_1">
              
                <span class="ddtitle">RECIPES BY CATEGORY</span>
                <ul>
                    
                    <li><a href="/shop/pc/recipes.asp?idCategory=586">Salmon</a></li>
                    <li><a href="/shop/pc/recipes.asp?idCategory=621">Shellfish</a></li>
                    <li><a href="/shop/pc/recipes.asp?idCategory=818">Grass Fed Beef</a></li>
                    <li><a href="/shop/pc/recipes.asp?idCategory=870">Chicken</a></li>
                    <li><a href="/shop/pc/in-the-kitchen.asp">See All Recipes</a></li>
                    
                </ul>
                
                
            </div>
            <div class="col_1">
                
                <span class="ddtitle">SEAFOOD HOW TO VIDEOS</span>
                <ul>
                    
                    <li><a href="/shop/pc/viewrecipes.asp?RecipeId=1484">How to Broil Salmon</a></li>
                    <li><a href="/shop/pc/viewrecipes.asp?RecipeId=1486">How to Sautee Salmon</a></li>
                    <li><a href="/shop/pc/viewrecipes.asp?RecipeId=1479">How to Clean Spot Prawns</a></li>
                    
                    
                </ul>
                <span class="ddtitle"><a href="/shop/pc/viewContent.asp?idpage=713">HOW TO PREPARE SEAFOOD</a></span>
                
            </div>
            <div class="col_1">
                <div class="navsaleimage2"><a href="/shop/pc/viewrecipes.asp?RecipeId=1484"><img src="/Assets/images/nav-ad-230x180-C.jpg" border="0" /></a></div>
                
                
                
            </div>
        </div>
    </li>
</ul>
</div>', 2, GETDATE(), GETDATE())

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name]='Affiliate Landing LEAN Area')
BEGIN

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Affiliate Landing LEAN Area', N'<h4>Vital Choice Web Affiliate Program</h4>
<p>
    Greetings {Name}, 
</p>
<p>
    Your special Dr. Sears LEAN affiliate account is ready for use.<br />
    FOR THE SPECIAL LEAN DISCOUNT FOR YOU AND YOUR CLIENTS PLEASE DIRECT READERS TO: 
    <a href="https://{PublicHost}/?idaffiliate={Id}">https://{PublicHost}/?idaffiliate={Id}</a> 
    This hyperlink includes you affiliate code so any sales from this link would be counted as a commission.
</p>
<p>
    Here is the same link with your affiliate number embedded in the code:
    <a href="https://{PublicHost}/?idaffiliate={Id}">Vital Choice LEAN Program</a> 
</p>
<p>
    When your readers make their purchase using the discount code they get 10% off their purchase and you get 8% of the first sale. 
    Then all lifetime purchases made by that customer (by calling us, using vitalchoice.com, or via your product and banner links) earn you 5% of each sale.
    You get a commission when your readers continue to use the LEAN discount code or respond to other special offers such as our 
    Weekly Bonus Offers promoted through the Vital Choice Newsletter and through occasional Discount Codes. 
    For general Affiliate Program information see the Program overview for more information.
<p>
    Vital Choice extends this commission and the LEAN discount to your personal purchase, so that you can try our products and share your first hand experience when reaching out to new customers.
    To set this, you simply click the above link and make a purchase. 
    Just like your customers, you only need to do it once to start earning commissions on all future purchases. 
</p>
<p>
    Vital Choice also offers a Wholesale Program for purchasing larger volumes for your company.<br />
    We appreciate your advocacy and interest in promoting wild seafood nutrition. We are looking forward to working together.<br />
    Questions or comments? Contact us at <a href="mailto:affiliatesupport@vitalchoice.com ">affiliatesupport@vitalchoice.com</a>
</p>
<p>
    Kind regards,<br />
    The Vital Choice Team
</p>', 2, GETDATE(), GETDATE())

END

GO