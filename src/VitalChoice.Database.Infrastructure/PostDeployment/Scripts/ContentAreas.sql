IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name]='Wholesale Welcome Area')
BEGIN

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Wholesale Welcome Area', N'<div class="wholesale-wrapper">
		<div class="wholesale-header">
			<img src="/files/catalog/VC30images/wholesale/welcome-to-wholesale.jpg" width="710" height="215">
		</div>
		<ul class="wholesale-slugs">
			<a href="/shop/pc/viewCategories.asp?idCategory=827">
				<li>
					<img src="/files/catalog/VC30images/wholesale/canned-slug.jpg">
					<span>Canned Seafood</span>
				</li>
			</a>
			<a href="/shop/pc/viewCategories.asp?idCategory=828">
			<li>
				<img src="/files/catalog/VC30images/wholesale/supp-slug.jpg">
				<span>Dietary Supplements</span>
			</li>
			</a>
			<a href="/shop/pc/viewCategories.asp?idCategory=829">
			<li>
				<img src="/files/catalog/VC30images/wholesale/organics-slug.jpg">
				<span>Organic Foods</span>
			</li>
			</a>
		</ul>
		<div class="clear"></div>
		<div class="wholesale-message">
			<p>
				<b>To our valued wholesale customers,</b> we offer a wide variety of premium canned and pouched seafood, marine supplements, and a limited selection of organic foods at greatly reduced prices. In addition, we extend a special 15% discount on large-quantity orders of our regular frozen seafood, grass-fed beef and other products in conjunction with your wholesale purchase. Just look for the discount offers in red type as you browse our website through this wholesale portal.
			</p>
			<p>
				Questions? Please feel free to call <a href="tel:8664825887">866-482-5887</a> or email <a href="mailto:wholesale@vitalchoice.com">wholesale@vitalchoice.com</a>. You may also find answers and other important information at <a href="/shop/pc/viewContent.asp?idpage=543">FAQ</a>. 
			</p>
			<p>
				Thank you for choosing Vital Choice.
			</p>
		</div>
</div>', 2, GETDATE(), GETDATE())

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name]='Alert Top Section Above Nav')
BEGIN

	UPDATE [dbo].[ContentAreas]
	SET Name='Alert Top Section Below Nav'
	WHERE Name='Alert Top Section'

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Alert Top Section Above Nav', N'<div class="top-banner">
    <img src="/Assets/images/flood-alert-8-2016.jpg">
</div>', 2, GETDATE(), GETDATE())

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name]='Admin Below Nav Critical Alert Message')
BEGIN

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Admin Below Nav Critical Alert Message', N'<div class="padding-bottom-5px padding-top-5px">Some Message</div>', 2, GETDATE(), GETDATE())

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name]='View Cart Only Above All Elements Alert')
BEGIN

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'View Cart Only Above All Elements Alert', N'<div class="center-block" style="width:984px">
        <img src="/assets/images/storm-alert-checkout-12-2016.jpg">
    </div>', 2, GETDATE(), GETDATE())

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name]='Retail Top Mega Menu' AND Updated<'2016-12-20 00:00:00.000')
BEGIN

	UPDATE [dbo].[ContentAreas]
	SET [Template]='<div class="top-menu-wrapper">
    <ul class="top-menu">
    <li><a href="#" class="drop">Shop</a>
    
        
        <div class="dropdown_2columns">
        <div class="shadowhide">
            <img class="close-button" src="/Assets/images/cart/Close-128.png"/>
        </div>
            <div class="col_1">
                
                <span class="ddtitle">FISH</span>
                <ul>
                    
                    <li><a href="/products/wild-salmon">Salmon</a></li>
                    <li><a href="/products/wild-alaskan-halibut">Halibut</a></li>
                    <li><a href="/products/wild-alaskan-cod">Cod</a></li>
                    <li><a href="/products/wild-alaskan-sablefish-aka-black-cod-butterfish">Sablefish</a></li>
                    <li><a href="/products/wild-pacific-albacore-tuna-small-troll-caught">Tuna</a></li>
                    <li><a href="/products/smoked-fish-and-lox">Smoked Fish & Lox</a></li>
                    <li><a href="/products/wild-salmon-sausage-bacon-and-burgers">Sausage, Burgers & Bacon</a></li>
                    <li><a href="/product/wild-petrale-sole-portions-4-6-oz-1-lb-bags">Petrale Sole</a></li>
                </ul>
               <span class="ddtitle"><a href="/products/wild-and-eco-grown-shellfish">SHELLFISH</a></span>
                <ul>
                    <li><a href="/products/wild-sea-scallops">Scallops</a></li>
                    <li><a href="/products/cultured-shellfish-mussels-clams-oysters">Clams, Oysters & Mussels</a>
                    <li><a href="/product/oregon-pink-shrimp-cooked">Wild Shrimp</a></li>
                    <li><a href="/products/wild-pacific-spot-prawns">Spot Prawns</a></li>
                    <li><a href="/products/crab">Crab</a></li>
                    <li><a href="/products/wild-maine-lobster">Lobster</a></li>
                    <li><a href="/product/wild-pacific-calamari-8-oz">Calamari</a></li>
                    <li><a href="/products/wild-and-eco-grown-shellfish">See All</a></li>
                </ul>
                 
                <span class="ddtitle">MEAT</span>
                <ul>
                    <li><a href="/products/grass-fed-bison">Grass-Fed Bison</a></li>
                    <li><a href="/products/organic-grass-fed-beef-by-skagit-river-ranch">Grass Fed Beef</a></li>
                    <li><a href="/products/heritage-chicken">Heritage Chicken</a></li>
                    <li><a href="/products/bone-broth-fish-chicken-and-beef">Bone Broth</a></li>
                    <li><a href="/products/burgers-and-hot-dogs">Burgers & Hotdogs: Fish and Meat</a></li>
                </ul>
            </div>
   
            <div class="col_1">
              
                <span class="ddtitle"><a href="/products/canned-and-pouched-wild-seafood">CANNED AND POUCHED</a></span>
                <ul>
                    
                    <li><a href="/products/wild-canned-sockeye-salmon">Salmon</a></li>
                    <li><a href="/products/canned-wild-portuguese-sardines-bone-in-and-fillets">Sardines</a></li>
                    <li><a href="/products/canned-and-pouched-tuna">Tuna</a></li>
                    <li><a href="/products/anchovies">Anchovies</a></li>
                    <li><a href="/products/wild-oregon-tiny-pink-shrimp">Shrimp</a></li>
                    <li><a href="/products/canned-wild-pacific-dungeness-crab">Crab</a></li>
                    <li><a href="/products/canned-wild-portuguese-mackerel">Mackerel</a></li>
                    <li><a href="/products/smoked-mussels-cultured">Mussels</a></li>
                    <li><a href="/products/canned-wild-seafood-samplers">Canned Samplers</a></li>
                    <li><a href="/products/canned-and-pouched-wild-seafood">See All</a></li>
                </ul>
                
                <span class="ddtitle"><a href="/products/organic-food-meat-and-poultry">ORGANIC FOODS & SEASONINGS</a></span>
                <ul>
                    
                    <li><a href="/products/meals-broth-and-soups-organic-and-natural">Broth, Soups & Meals</a></li>
                    <li><a href="/products/organic-berries-frozen">Frozen Berries</a></li>
                    <li><a href="/products/organic-oils-and-vinegar">Oils & Vinegars</a></li>
                    <li><a href="/products/organic-marinade-rub-mixes-and-seasonings">Garlic, Marinades & Seasonings</a></li>
                    <li><a href="/products/natural-jerky">Natural Jerky - Salmon & Bison</a></li>
                    <li><a href="/products/organic-dried-fruits">Dried Fruit</a></li>
                    <li><a href="/products/organic-extra-dark-chocolate">Chocolate</a></li>
                    <li><a href="https://www.vitalchoice.com/products/organic-trail-mix">Trail Mix</a></li>
                    <!--<li><a href="/products/organic-nuts-raw-and-roasted">Nuts</a></li>-->
                    <li><a href="/products/seaweed-salad-and-kelp-cubes">Seaweed & Kelp</a></li>
                    <li><a href="/products/organic-food-meat-and-poultry">See All</a></li>
               </ul>      
            </div>
            <div class="col_1">
                
                
                <span class="ddtitle"><a href="/products/omega-3s-herbs-tests-vitamin-d-and-astaxanthin">OMEGA-3s & SUPPLEMENTS</a></span>
                <ul>
                    
                    <li><a href="/products/omega-3-wild-salmon-oil-supplements">Omega-3 Wild Salmon Oil</a></li>
                    <li><a href="/products/vitamin-d3-omega-3-combos">Vitamin D3 +  Omega-3 Combos</a></li>
                    <li><a href="/product/high-dha-prenatal-therapy-vitamin-d3-180-softgels">High DHA Prenatal Therapy</a></li>
                    <li><a href="/products/daily-supplement-packs">Daily Dose Packs</a></li>
                    <li><a href="/products/omega-3-krill-oil">Omega-3 Krill Oil</a></li>
                    <li><a href="/products/vital-omega-3-6-hufa-test">Vital Omega-3/6 Test&#8482; Kit</a></li>
                    <li><a href="/product/curcumin-in-wild-alaskan-salmon-oil-250mg-120-ct">Curcumin in Wild Salmon Oil</a></li>
                    <li><a href="/products/omega-3s-herbs-tests-vitamin-d-and-astaxanthin">See All</a></li>
                    
                </ul>
                <span class="ddtitle">SAMPLERS & GIFT PACKS</span>
                <ul>
                    <li><a href="/products/samplers">Samplers</a></li>
                    <li><a href="/products/health-advisors-packs">Health Advisor Packs</a></li>
                    <li><a href="/products/pet-products">Pet Treats</a></li>
                    <li><a href="/content/healthy-mom-baby">Healthy Mom & Baby</a></li>
                    <li><a href="/products/doctors-favorites">Doctor''s Favorites</a></li>
                    <li><a href="/products/gift-packs-and-certificates">All Gifts & Packs</a></li>
                    <li><a href="/products/gift-certificates">Gift Certificates</a></li>
                </ul>
                <span class="ddtitle">COOKS TOOLS & BOOKS</span>
                <ul>
                    <li><a href="/product/vital-choice-gift-certificate?cat=278">Gift Certificates</a></li>
                    <li><a href="/products/cookbooks-and-cooking-accessories">Cooks Tools & Books</a></li>
                    <li><a href="/products/grilling-accessories">Grilling Accessories</a></li>
                </ul>
            </div>
            <div class="col_1">
                
                <div class="navsaleimage"><a href="/products/top-sellers"><img src="/files/top-seller-nav.jpg" border="0" /></a></div>
                
                <span class="ddtitle"><a href="/products/special-offers">SPECIAL OFFERS / VALUE PICKS</a></span>
                <span class="ddtitle"><a href="/content/holiday-gifting"><font color="red">OUR HOLIDAY GIFT GUIDE</font></a></span>
                <span class="ddtitle"><a href="/products/top-sellers">TOP SELLERS</a></span>
                <span class="ddtitle"><a href="/products/randy-s-picks">RANDY''S PICKS</a></span>
                <span class="ddtitle"><a href="/products/new-at-vital-choice">NEW PRODUCTS</a></span>
                <span class="ddtitle"><a href="/content/TAILGATING">TAILGATING</a></span> 
                
                
            </div>
        </div>
    </li>

    <li><a href="#" class="drop">About Vital Choice</a>
        <div class="dropdown_2columns second">
            <div class="shadowhide">
                <img class="close-button" src="/Assets/images/cart/Close-128.png"/>
            </div>
            <div class="col_1">
                
                <span class="ddtitle">WHO WE ARE: OUR STORY</span>
                <ul>
                    
                    <li><a href="/content/about-vital-choice">About Vital Choice</a></li>
                    <li><a href="/content/our-mission">Our Mission</a></li>
                    <li><a href="/content/what-are-people-saying-about-vital-choice">Customer & Expert Reviews</a></li>  
                    
                </ul>
                
                
            </div>
    
            <div class="col_1">
              
                <span class="ddtitle">WHY WE ARE DIFFERENT</span>
                <ul>
                    
                    <li><a href="/content/purity-story">Purity Standards</a></li>
                    <li><a href="/content/vital-green-environmental-stewardship-program">Vital Green&#8482; Eco Programs</a></li>
                    <li><a href="/content/sustainability">Sustainable Seafood</a></li>
                    
                    
                </ul>
                
                
            </div>
            <div class="col_1">
                
                <span class="ddtitle">WHAT WE BELIEVE</span>
                <ul>
                    <li><a href="/content/a-letter-from-randy-hartnell">A Letter from Randy Hartnell</a></li>
                    <li><a href="/content/the-antidote-podcast-series">Randy''s Podcasts</a></li>
                    <li><a href="/content/giving-back-to-the-community">Giving Back</a></li>
                
               
                    
                </ul>
                
                
            </div>
            <div class="col_1">
                <div class="navsaleimage2"><a href="/content/come-home-to-real-food" target="_blank"><img src="/files/randy-brand-nav.jpg.png" border="0" /></a></div>
                
                
                
            </div>
        </div>
    </li>

    <li><a href="#" class="drop">Learn</a>
        <div class="dropdown_2columns third">
            <div class="shadowhide">
                <img class="close-button" src="/Assets/images/cart/Close-128.png"/>
            </div>
            <div class="col_1">
                
                <span class="ddtitle">RECOMMENDED READING</span>
                <ul>
                    
                    <li><a href="/content/newsletter-sign-up">Newsletter Sign-up</a></li>
                    <li><a href="/articles/">Newsletter Articles Archive</a></li>
                    <li><a href="/faqs">FAQs</a></li>
                    
                </ul>
                
                
            </div>
    
            <div class="col_1">
              
                <span class="ddtitle">HEALTH & NUTRITION</span>
                <ul>
                    <li><a href="/product/the-vital-omega-3-6-hufa-test-trade">Take the Vital Omega-3/6 Test&#8482;</a></li>
                    <li><a href="/content/omega-3-facts-sources">Omega-3 Facts</a></li>
                    <li><a href="/content/health-benefits-of-fish">Seafood Benefits</a></li>
                    <li><a href="/content/omega-3-6-balance-scores-how-to-use-them">Omega 3/6 Balance</a></li>
                    <li><a href="/content/healthy-mom-baby">Healthy Mom & Baby</a></li>
                    
                    
                </ul>
                
                
            </div>
            <div class="col_1">
                
                <span class="ddtitle">ABOUT VITAL CHOICE PRODUCTS</span>
                <ul>
                    <li><a href="/content/cooking-tips">How to Prepare Seafood</a></li>
                    <li><a href="/content/about-vital-choice#Organic">Kosher & Organic Certification</a></li>
                    <li><a href="/content/about-vital-choice#Flash-Frozen">The Flash-Frozen Advantage</a></li>
                    <li><a href="/content/purity-story">Seafood Purity & Safety</a></li>
                    <li><a href="/content/about-vital-choice#Superior%20Salmon">Superior Salmon, Naturally</a></li>
                </ul>
                
                
            </div>
            <div class="col_1">
                <div class="navsaleimage2"><a href="/products/bone-broth-fish-chicken-and-beef"><img src="/Assets/images/nav-ad-230x180-B.jpg" border="0" /></a></div>
                
                
                
            </div>
        </div>
    </li>
    
    <li><a href="#" class="drop">Cook</a>
        <div class="dropdown_2columns fourth">
            <div class="shadowhide">
                <img class="close-button" src="/Assets/images/cart/Close-128.png"/>
            </div>
            <div class="col_1">
                <span class="ddtitle"><a href="/recipe/vital-choice-seafood-cooking-videos">COOKING VIDEOS</a></span>
                <span class="ddtitle">CELEBRITY CHEF VIDEOS</span>
                <ul>
                    
                    <li><a href="/recipes/becky-selengut">Becky Selengut</a></li>
                    <li><a href="/recipes/myra-kornfeld">Myra Kornfeld</a></li>
                    <li><a href="/recipes/rebecca-katz">Rebecca Katz</a></li>
                    
                </ul>
                
                
            </div>
    
            <div class="col_1">
              
                <span class="ddtitle">RECIPES BY CATEGORY</span>
                <ul>
                    
                    <li><a href="/recipes/wild-salmon">Salmon</a></li>
                    <li><a href="/recipes/shellfish">Shellfish</a></li>
                    <li><a href="/recipes/grass-fed-beef">Grass Fed Beef</a></li>
                    <li><a href="/recipes/heritage-chicken">Chicken</a></li>
                    <li><a href="/content/in-the-kitchen">See All Recipes</a></li>
                    
                </ul>
                
                
            </div>
            <div class="col_1">
                
                <span class="ddtitle">SEAFOOD HOW-TO VIDEOS</span>
                <ul>
                    
                    <li><a href="/recipe/how-to-broil-salmon">How-to Broil Salmon</a></li>
                    <li><a href="/recipe/how-to-saut-salmon">How-to Saut&eacute; Salmon</a></li>
                    <li><a href="/recipe/how-to-clean-spot-prawns">How-to Clean Spot Prawns</a></li>
                    <li><a href="/content/in-the-kitchen">See All Videos</a></li>
                    
                </ul>
                <span class="ddtitle"><a href="/content/cooking-tips">GUIDE: HOW-TO PREPARE SEAFOOD</a>
                <span class="ddtitle"><a href="/content/in-the-kitchen">SEE ALL RECIPES & VIDEOS</a></span>
            </div>
            <div class="col_1">
                <div class="navsaleimage2"><a href="/content/Seafood-Storage-Cooking-Tips"><img src="/files/guide-to-prepare_navimage.jpg" border="0" /></a></div>
                
                
                
            </div>
        </div>
    </li>
</ul>
<div class="callanytimenav"><img src="/files/vc-phone-number.png" border="0"></div>
</div>',
	Updated=GETDATE()
	WHERE [Name]='Retail Top Mega Menu'
END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name]='Wholesale Top Mega Menu' AND Updated<'2016-12-20 00:00:00.000')
BEGIN

	UPDATE [dbo].[ContentAreas]
	SET [Template]='<div class="top-menu-wrapper">
    <ul class="top-menu">
    <li><a href="#" class="drop">Shop</a>
    
        
        <div class="dropdown_2columns">
        <div class="shadowhide">
            <img class="close-button" src="/Assets/images/cart/Close-128.png"/>
        </div>
            <div class="col_1">
                               
                <span class="ddtitle">FISH</span>
                <ul>
                    
                    <li><a href="/products/wild-salmon">Salmon</a></li>
                    <li><a href="/products/wild-alaskan-halibut">Halibut</a></li>
                    <li><a href="/products/wild-alaskan-cod">Cod</a></li>
                    <li><a href="/products/wild-alaskan-sablefish-aka-black-cod-butterfish">Sablefish</a></li>
                    <li><a href="/products/wild-pacific-albacore-tuna-small-troll-caught">Tuna</a></li>
                    <li><a href="/products/smoked-fish-and-lox">Smoked Fish & Lox</a></li>
                    <li><a href="/products/wild-salmon-sausage-bacon-and-burgers">Sausage, Burgers & Bacon</a></li>
                    <li><a href="/product/wild-petrale-sole-portions-4-6-oz-1-lb-bags">Petrale Sole</a></li>
                </ul>
                <span class="ddtitle">SHELLFISH</span>
                <ul>
                    <li><a href="/products/wild-sea-scallops">Scallops</a></li>
                    <li><a href="/products/cultured-shellfish-mussels-clams-oysters">Clams, Oysters & Mussels</a>
                    <li><a href="/product/oregon-pink-shrimp-cooked">Wild Shrimp</a></li>
                    <li><a href="/products/wild-pacific-spot-prawns">Spot Prawns</a></li>
                    <li><a href="/products/crab">Crab</a></li>
                    <li><a href="/products/wild-maine-lobster">Lobster</a></li>
                    <li><a href="/product/wild-pacific-calamari-8-oz">Calamari</a></li>
                    <li><a href="/products/wild-and-eco-grown-shellfish">See All</a></li>
                </ul>
                 
                <span class="ddtitle">MEAT</span>
                <ul>
                    <li><a href="/products/grass-fed-bison">Grass-Fed Bison</a></li>
                    <li><a href="/products/organic-grass-fed-beef-by-skagit-river-ranch">Grass Fed Beef</a></li>
                    <li><a href="/products/heritage-chicken">Heritage Chicken</a></li>
                    <li><a href="/products/bone-broth-fish-chicken-and-beef">Bone Broth</a></li>
                    <li><a href="/products/burgers-and-hot-dogs">Burgers & Hotdogs: Fish and Meat</a></li>
                </ul>
            </div>
   
            <div class="col_1">
              
                <span class="ddtitle">CANNED & POUCHED  SEAFOOD</span>
                <ul>
                    
                    <li><a href="/products/wild-canned-sockeye-salmon">Salmon</a></li>
                    <li><a href="/products/canned-wild-portuguese-sardines-bone-in-and-fillets">Sardines</a></li>
                    <li><a href="/products/canned-wild-albacore-tuna-troll-caught">Tuna</a></li>
                    <li><a href="/products/anchovies">Anchovies</a></li>
                    <li><a href="/products/wild-oregon-tiny-pink-shrimp">Shrimp</a></li>
                    <li><a href="/products/canned-wild-pacific-dungeness-crab">Crab</a></li>
                    <li><a href="/products/canned-wild-portuguese-mackerel">Mackerel</a></li>
                    <li><a href="/products/smoked-mussels-cultured">Mussels</a></li>
                    <li><a href="/products/canned-wild-seafood-samplers">Canned Samplers</a></li>
                    <li><a href="/products/canned-and-pouched-wild-seafood">See All</a></li>
                </ul>
                
                <span class="ddtitle">ORGANIC FOOD & SEASONINGS</span>
                <ul>
                    <li><a href="/products/meals-broth-and-soups-organic-and-natural">Broth, Soups & Meals</a></li>
                    <li><a href="/products/organic-berries-frozen">Frozen Berries</a></li>
                    <li><a href="/products/organic-oils-and-vinegar">Oils & Vinegars</a></li>
                    <li><a href="/products/organic-marinade-rub-mixes-and-seasonings">Garlic, Marinades & Seasonings</a></li>
                    <li><a href="/products/natural-jerky">Natural Jerky - Salmon & Bison</a></li>
                    <li><a href="/products/organic-dried-fruits">Dried Fruit</a></li>
                    <li><a href="/products/organic-extra-dark-chocolate">Chocolate</a></li>
                    <!--<li><a href="/products/organic-nuts-raw-and-roasted">Nuts</a></li>-->
                    <li><a href="/products/seaweed-salad-and-kelp-cubes">Seaweed & Kelp</a></li>
                    <li><a href="/products/organic-food-meat-and-poultry">See All</a></li>
               </ul>      
                <span class="ddtitle">COOKS GIFTS & BOOKS</span>
                <ul>
                    <li><a href="/product/vital-choice-gift-certificate?cat=278">Gift Certificates</a></li>
                    <li><a href="/products/cookbooks-and-cooking-accessories">Kitchen Tools</a></li>
                    <li><a href="/products/favorite-books-and-cookbooks">Books</a></li>
                    <li><a href="/products/cookbooks-and-cooking-accessories">See All</a></li>
                </ul>
            </div>
            <div class="col_1">
                
                                <span class="ddtitle">OMEGA-3s & SUPPLEMENTS</span>
                <ul>
                    
                    <li><a href="/products/omega-3-wild-salmon-oil-supplements">Omega-3 Wild Salmon Oil</a></li>
                    <li><a href="/product/high-dha-prenatal-therapy-vitamin-d3-180-softgels">High DHA Prenatal Therapy</a></li>
                    <li><a href="/products/daily-supplement-packs">Daily Dose Packs</a></li>
                    <li><a href="/products/omega-3-krill-oil">Omega-3 Krill Oil</a></li>
                    <li><a href="/products/vital-omega-3-6-hufa-test">Vital Omega-3/6 Test&#8482; Kit</a></li>
                    <li><a href="/product/curcumin-in-wild-alaskan-salmon-oil-250mg-120-ct">Curcumin in Wild Salmon Oil</a></li>
                    <li><a href="/products/omega-3s-herbs-tests-vitamin-d-and-astaxanthin">See All</a></li>
                    
                </ul>
                <span class="ddtitle">SAMPLERS & GIFT PACKS</span>
                <ul>
                    <li><a href="/products/samplers">Samplers</a></li>
                    <li><a href="/products/health-advisors-packs">Health Advisor Packs</a></li>
                    <li><a href="/products/pet-products">Pet Treats</a></li>
                    <li><a href="/content/healthy-mom-baby">Healthy Mom & Baby</a></li>
                    <li><a href="/products/gift-packs-and-certificates">All Gifts & Packs</a></li>
                    <li><a href="/products/gift-certificates">Gift Certificates</a></li>
                    </ul>
                <span class="ddtitle">WHOLESALE</span>
                <ul>
                    <li><a href="/products/canned-wild-seafood">Canned Wild Seafood</a></li>
                    <li><a href="/products/dietary-supplements">Dietary Supplements</a></li>
                    <li><a href="/products/organic-foods">Organic Foods</a></li>
                    <li><a href="/products/new-to-wholesale">New to Wholesale</a></li>
                </ul>
            </div>
            <div class="col_1">
                
                <!-- SHOP NAV SALE IMAGE -->
                <div class="navsaleimage">
                	<a href="/products/top-sellers">
                		<img src="/files/top-seller-nav.jpg" border="0" />
                	</a>
                </div>
                
                <span class="ddtitle"><a href="/products/special-offers">SPECIAL OFFERS / VALUE PICKS</a></span>
                <span class="ddtitle"><a href="/products/top-sellers">TOP SELLERS</a></span>
                <span class="ddtitle"><a href="/products/randy-s-picks">RANDY''S PICKS</a></span>
                <span class="ddtitle"><a href="/products/new-at-vital-choice">NEW PRODUCTS</a></span>
                <span class="ddtitle"><a href="/content/time-to-savor-summer">GRILLING FAVORITES</a></span> 
                
                     
            </div>
        </div>
    </li>

    <li><a href="#" class="drop">About Vital Choice</a>
        <div class="dropdown_2columns second">
            <div class="shadowhide">
                <img class="close-button" src="/Assets/images/cart/Close-128.png"/>
            </div>
            <div class="col_1">
                
            <span class="ddtitle">WHO WE ARE: OUR STORY</span>
                <ul>
                    
                    <li><a href="/content/about-vital-choice">About Vital Choice</a></li>
                    <li><a href="/content/our-mission">Our Mission</a></li>
                    <li><a href="/content/what-are-people-saying-about-vital-choice">Customer & Expert Reviews</a></li>  
                    
                </ul>
                
                
            </div>
    
            <div class="col_1">
              
                 <span class="ddtitle">WHY WE ARE DIFFERENT</span>
                <ul>
                    
                    <li><a href="/content/purity-story">Purity Standards</a></li>
                    <li><a href="/content/vital-green-environmental-stewardship-program">Vital Green&#8482; Eco Programs</a></li>
                    <li><a href="/content/sustainability">Sustainable Seafood</a></li>
                    
                    
                </ul>
                
                
            </div>
            <div class="col_1">
                
                 <span class="ddtitle">WHAT WE BELIEVE</span>
                <ul>
                    <li><a href="/content/a-letter-from-randy-hartnell">A Letter from Randy Hartnell</a></li>
                    <li><a href="/content/the-antidote-podcast-series">Randy''s Podcasts</a></li>
                    <li><a href="/content/giving-back-to-the-community">Giving Back</a></li>
                
               
                    
                </ul>
                
                
            </div>
            <div class="col_1">
            
                <!-- ABOUT NAV SALE IMAGE -->
                <div class="navsaleimage2">
                	<a href="/content/come-home-to-real-food" target="_blank">
                		<img src="/files/randy-brand-nav.jpg.png" border="0" />
                	</a>
                </div>
                
            </div>
        </div>
    </li>

    <li><a href="#" class="drop">Learn</a>
        <div class="dropdown_2columns third">
            <div class="shadowhide">
                <img class="close-button" src="/Assets/images/cart/Close-128.png"/>
            </div>
            <div class="col_1">
                
                <span class="ddtitle">RECOMMENDED READING</span>
                <ul>
                    
                    <li><a href="/content/newsletter-sign-up">Newsletter Sign-up</a></li>
                    <li><a href="/articles/">Newsletter Articles Archive</a></li>
                    <li><a href="/faqs">FAQs</a></li>
                    
                </ul>
                
                
           </div>
    
            <div class="col_1">
              
                <span class="ddtitle">HEALTH & NUTRITION</span>
                <ul>
                    <li><a href="/product/the-vital-omega-3-6-hufa-test-trade">Take the VitalTest</a></li>
                    <li><a href="/content/omega-3-facts-sources">Omega-3 Facts</a></li>
                    
                    <li><a href="/content/health-benefits-of-fish">Seafood Benefits</a></li>
                    <li><a href="/content/omega-3-6-balance-scores-how-to-use-them">Omega 3/6 Balance</a></li>
                    <li><a href="/content/healthy-mom-baby">Healthy Mom & Baby</a></li>
                    
                    
                </ul>
                
                
            </div>
            <div class="col_1">
                
                <span class="ddtitle">ABOUT VITAL CHOICE PRODUCTS</span>
                <ul>
                    <li><a href="/content/cooking-tips">How to Prepare Seafood</a></li>
                    <li><a href="/content/about-vital-choice#Organic">Kosher & Organic Certification</a></li>
                    <li><a href="/content/about-vital-choice#Flash-Frozen">The Flash-Frozen Advantage</a></li>
                    <li><a href="/content/purity-story">Seafood Purity & Safety</a></li>
                    <li><a href="/content/about-vital-choice#Superior%20Salmon">Superior Salmon, Naturally</a></li>
                </ul>
                
                
            </div>
            <div class="col_1">
                
                <!-- LEARN NAV SALE IMAGE -->
                <div class="navsaleimage2">
                	<a href="/products/bone-broth-fish-chicken-and-beef">
                		<img src="/Assets/images/nav-ad-230x180-B.jpg" border="0" />
                	</a>
                </div>
                                
            </div>
        </div>
    </li>
    
    <li><a href="#" class="drop">Cook</a>
        <div class="dropdown_2columns fourth">
            <div class="shadowhide">
                <img class="close-button" src="/Assets/images/cart/Close-128.png"/>
            </div>
            <div class="col_1">
                <span class="ddtitle"><a href="/recipe/vital-choice-seafood-cooking-videos">COOKING VIDEOS</a></span>
                <span class="ddtitle">CELEBRITY CHEF VIDEOS</span>
                <ul>
                    
                    <li><a href="/recipes/becky-selengut">Becky Selengut</a></li>
                    <li><a href="/recipes/myra-kornfeld">Myra Kornfeld</a></li>
                    <li><a href="/recipes/rebecca-katz">Rebecca Katz</a></li>
                    
                </ul>
                
                
            </div>
    
            <div class="col_1">
              
                <span class="ddtitle">RECIPES BY CATEGORY</span>
                <ul>
                    
                    <li><a href="/recipes/wild-salmon">Salmon</a></li>
                    <li><a href="/recipes/shellfish">Shellfish</a></li>
                    <li><a href="/recipes/grass-fed-beef">Grass Fed Beef</a></li>
                    <li><a href="/recipes/heritage-chicken">Chicken</a></li>
                    <li><a href="/content/in-the-kitchen">See All Recipes</a></li>
                    
                </ul>
                
                
            </div>
            <div class="col_1">
                
                <span class="ddtitle">SEAFOOD HOW TO VIDEOS</span>
                <ul>
                    
                    <li><a href="/recipe/how-to-broil-salmon">How to Broil Salmon</a></li>
                    <li><a href="/recipe/how-to-saut-salmon">How to Saut&eacute; Salmon</a></li>
                    <li><a href="/recipe/how-to-clean-spot-prawns">How to Clean Spot Prawns</a></li>
                    
                    
                </ul>
                <span class="ddtitle"><a href="/content/cooking-tips">HOW TO PREPARE SEAFOOD</a></span>
                
            </div>
            <div class="col_1">
            
                <!-- COOK NAV SALE IMAGE -->
                <div class="navsaleimage2">
                	<a href="/content/Seafood-Storage-Cooking-Tips">
                		<img src="/files/guide-to-prepare_navimage.jpg" border="0" />
                	</a>
                </div>  
                              
            </div>
        </div>
    </li>
</ul>
<div class="callanytimenav"><img src="/files/vc-phone-number.png" border="0"></div>
</div>',
	Updated=GETDATE()
	WHERE [Name]='Wholesale Top Mega Menu'
END

GO

