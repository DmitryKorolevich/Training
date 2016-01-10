GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentPages] WHERE [Name] = 'Request Catalog')
BEGIN

DECLARE @contentItemId int

INSERT INTO [dbo].[ContentItems]
           ([Created]
           ,[Updated]
           ,[Template]
           ,[Description]
           ,[Title]
           ,[MetaKeywords]
           ,[MetaDescription])
     VALUES
           (GETDATE()
           ,GETDATE()
           ,'<%
<body:body>
{{
    @script(){{
        <script src="https://www.google.com/recaptcha/api.js?onload=onloadRecaptchaCallback&render=explicit" async defer></script>
        <script src="/app/common/dataAccess.js"></script>
        <script src="/app/modules/auth/registration.js"></script>
    }}
    <h4>Request a Catalog</h4>
    <p>
        <strong>The award-winning* Vital Choice catalog is an easy way to shop and offers a convenient way to plan your next order.</strong>
    </p>
    <p>
        Call toll free 800-608-4825 to order catalogs ...
        ... and to send some to your friends and family!
    </p>
    <p>
        Or, fill in the form below, and click "Sign Up".
    </p>
    <p>
        We''ll send one of our new catalogs to that address ... simply repeat the process to send our catalog to other addresses.
    </p>
    @razor(@null){{~/Views/Help/_RequestCatalog.cshtml}}
    <div class="clear"></div>
    <p>
        *Our 2009 Holiday Season catalog won the 2010 Gold Award from Multichannel Merchant. 
    </p>
    <p>
        <strong>To our International Friends</strong>
    </p>
    <ul>
        <li>We can only mail our catalogs to U.S. and Canadian addresses. </li>
        <li>
            Our ability to ship packages outside of the United States and Canada is very limited.
        </li>
        <li>
            We can only ship non-perishable items to U.S. territories and a few other destinations
            outside the U.S. and Canada.
        </li>
        <li>Shipments of perishable items to Hawaii and Alaska involve extra fees. </li>
        <li>All shipments to Canada incur surcharges. </li>
    </ul>
    <p>
        For full information, see our <a href="#">Shipping Policies</a>. 
    </p>
    <p>
        If you still have questions about the feasibility and cost of a shipment to an address outside the continental US, send your inquiries to <a href="mailto:info@{@}@vitalchoice.com">info@{@}@vitalchoice.com</a>.
    </p>  
}}
%>'
           ,'<p>empty</p>'
           ,'Request Catalog'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[ContentPages]
           ([Url]
           ,[Name]
           ,[FileUrl]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[Assigned]
           ,[UserId])
     VALUES
           ('request-catalog'
           ,'Request Catalog'
           ,NULL
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Content Individual')
           ,2
           ,1
           ,NULL)

INSERT INTO [dbo].[ContentItems]
           ([Created]
           ,[Updated]
           ,[Template]
           ,[Description]
           ,[Title]
           ,[MetaKeywords]
           ,[MetaDescription])
     VALUES
           (GETDATE()
           ,GETDATE()
           ,'<%
<body:body>
{{
    @script(){{
        <script src="https://www.google.com/recaptcha/api.js?onload=onloadRecaptchaCallback&render=explicit" async defer></script>
    }}
    <h4>Contact Customer Service</h4>
    <p>
        <strong>At Vital Choice, we strive to provide the best customer service possible.</strong>
    </p>
    <p>
        You''ll find the answers to many common questions by visiting our <a href="#">FAQ section</a>.
        If you are wondering about the status of your shipment, you should have received a "Shipment Notification" email that lets you track the shipping of your order.
    </p>
    <p>
        <strong>Customer Service by Email</strong>
    </p>
    <ul>
        <li>
            <strong>To send order-related emails</strong>,
             leave the "Send My Comments to" menu below set to Customer Service, enter your information, and click Send. To receive a faster response, please include the order number provided in the email receipt we sent to you. </>
        </li>
        <li>
            <strong>To send general comments and suggestions</strong>, 
            set the "Send My Comments to" menu below to Feedback, enter your information, and click Send. 
        </li>
    </ul>
    @razor(@null){{~/Views/Help/_ContactCustomerService.cshtml}}
    <div class="clear"></div>
    <p>
        <strong>Call to Order Anytime</strong>
    </p>
    <p>
        Prefer talking to clicking? Shop by calling our toll-free Sales Line at 800-608-4825, where we take orders 24 hours a day, 365 days a year.
    </p>
    <p>
        For detailed account and product information, call our special Customer Service Line.
    </p>
    <p>
        <strong>Customer Service Line - Open 5 days a week (specified hours only).</strong>
    </p>
    <p>
        Our friendly Customer Service representatives are here to take orders and answer account and product questions. Call them toll-free at 866-482-5887, Monday through Friday
        from 7:00 am to 4:00 pm Pacific Time.
    </p>
    <p>
        <strong>Mailing address</strong>
    </p>
    <p>
        Vital Choice Wild Seafood & Organics<br />
        PO Box 4121<br />
        Bellingham, WA 98227
    </p>
    <p>
        <strong>Return Policy</strong>
    </p>
    <p>
        We hope you will be delighted with your purchase. If you are not satisfied and would like to return a product, please be aware of the following guidelines:
    </p>
    <ul>
        <li>
            Refund must be requested within 30 days of purchase date.
            <br/><br />
        </li>
        <li>
            Product must have been purchased from Vital Choice Seafood, Inc.
            <br /><br />
        </li>
        <li>
            Shipping charges are refundable only if products were delivered late or damaged in transit.
        </li>
    </ul>
    <p>
        Non-perishable products must be returned to:
    </p>
    <p>
        Vital Choice Wild Seafood & Organics<br/>
        2460 Salashan Loop<br />
        Ferndale, WA 98248
    </p>
    <p>
        ATTN: Returns Dept.
    </p>
    <hr />
    <p>
        <a href="#">Your Suggestions</a> will help make Vital Choice even better!
    </p>
}}
%>'
           ,'<p>empty</p>'
           ,'Contact Customer Service'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[ContentPages]
           ([Url]
           ,[Name]
           ,[FileUrl]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[Assigned]
           ,[UserId])
     VALUES
           ('contact-customer-service'
           ,'Contact Customer Service'
           ,NULL
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Content Individual')
           ,2
           ,1
           ,NULL)

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentPages] WHERE [Url] = 'not-found')
BEGIN

DECLARE @contentItemId int

INSERT INTO [dbo].[ContentItems]
           ([Created]
           ,[Updated]
           ,[Template]
           ,[Description]
           ,[Title]
           ,[MetaKeywords]
           ,[MetaDescription])
     VALUES
           (GETDATE()
           ,GETDATE()
           ,''
           ,'<h4>Page not found</h4>'
           ,'Page Not Found'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[ContentPages]
           ([Url]
           ,[Name]
           ,[FileUrl]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[Assigned]
           ,[UserId])
     VALUES
           ('not-found'
           ,'Not Found'
           ,NULL
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Content Individual Empty')
           ,2
           ,1
           ,NULL)

END

GO


IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentPages] WHERE [Url] = 'in-the-kitchen')
BEGIN

DECLARE @contentItemId int

INSERT INTO [dbo].[ContentItems]
           ([Created]
           ,[Updated]
           ,[Template]
           ,[Description]
           ,[Title]
           ,[MetaKeywords]
           ,[MetaDescription])
     VALUES
           (GETDATE()
           ,GETDATE()
           ,'<%
<center:center>
{{
    @script(){{
        <script src="https://www.youtube.com/iframe_api"></script>
        <script src="/app/modules/content/in-the-kitchen.js"></script>
    }}
   <div class="center-content-pane in-the-kitchen">
        <div class="center-wrapper youtube-container-wrapper">
            <a href="#" data-video-id="MjH2Zfq1uDQ" class="youtube-item">
                <img id="maining" src="/assets/images/main-video-468px-a.jpg">
            </a>
            <div id="youtube-container">
                <a href="#" class="youtube-close">
                    <img alt="Close" src="/assets/images/close_button.png">
                </a>
                <div id="youtube-player"></div>
            </div>
        </div>
        <div class="center-wrapper">
            <div class="current-item-wrapper" style="display: none;">
                <h4></h4>
                <div class="body"></div>
                <a href="#"> - Click here for full text version of this recipe</a>
            </div>
        </div>
        <div class="center-wrapper">
            <a href="#">
                <img src="/assets/images/guest-chef-banner-6-10-2014c.jpg">
            </a>
        </div>
        <div>
            <a href="#">
                <img src="/assets/images/basic-techniques-banner-6-10-2014c.jpg">
            </a>
        </div>
	</div>
}}
<right:right>
{{
    <div class="right-content-pane in-the-kitchen">
        <div class="right-wrapper panel panel-border">
    	    <div class="sub-title">Seafood How-To Videos</div><br/>
    	    <div class="seafood-basics">
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="MjH2Zfq1uDQ" data-tooltip-title="How to Broil Salmon" data-tooltip-body="Many don&#8217;t realize how incredibly simple it is to broil wild Alaskan silver salmon perfectly, until seeing this short guide by Chef Becky Selengut.">
        	                <img src="/assets/images/broiling-salmon-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                How to Broil Silver Salmon
    	            </div>
    	        </div>
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="38FwAMSnFAA" data-tooltip-title="How to Saut&eacute; Salmon" data-tooltip-body="Using only seafood marinade and organic olive oil, Chef Becky Selengut shows just how simple it is to cook salmon beautifully in a frying pan.">
        	                <img src="/assets/images/saute-sockeye-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                How to Sauté Sockeye Salmon
    	            </div>
    	        </div>
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="fiCKvcizzUY" data-tooltip-title="Steamed Wild Halibut" data-tooltip-body="A remarkably simple yet highly sophisticated recipe for wild Alaskan halibut using caviar, spinach, carrots and sesame seeds, by Chef Becky Selengut.">
        	                <img src="/assets/images/steam-halibut-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                How to Steam Halibut
    	            </div>
    	        </div>
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="U3Wcs0fV2L0" data-tooltip-title="How to Clean Spot Prawns" data-tooltip-body="Chef Becky Selengut demystifies the process of cleaning spot prawns, demonstrating how to de-vein and shell these delicacies for cooking.">
        	                <img src="/assets/images/clean-prawns-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                How to Clean Spot Prawns
    	            </div>
    	        </div>
    	        
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="JVz0uPA0_TY" data-tooltip-title="Smoked Salmon & Egg Skillet" data-tooltip-body="Chef Becky Selengut demonstrates how simple it is to prepare a tasty, nutrient-packed breakfast that will have your family begging for more.">
        	                <img src="/assets/images/skillet-eggs-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                Smoked Salmon & Egg Skillet
    	            </div>
    	        </div>
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="1wpdOffvZIA"  data-tooltip-title="Crab Salad & Thai Coconut Soup" data-tooltip-body="Using canned Dungeness crab as a centerpiece, Chef Becky Selengut shows how a few simple ingredients combine for a showpiece meal.">
        	                <img src="/assets/images/cucumber-soup-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                Crab Salad & Thai Coconut Soup
    	            </div>
    	        </div>
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="eSjAajxRwd0"  data-tooltip-title="Wild Pacific Albacore Tuna Tataki" data-tooltip-body="Unbelievably simple yet so delicious, this tuna recipe by Chef Becky Selengut makes an elegant appetizer for health-conscious foodies.">
        	                <img src="/assets/images/tuna-tataki-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                Wild Pacific Albacore Tuna Tataki
    	            </div>
    	        </div>
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="btlfoO75kfI"  data-tooltip-title="Mixed Seafood Snack Plate" data-tooltip-body="Organic almonds, pickled onions and olives complement a healthful combo of sardines, salmon and sablefish for a simple, crowd-pleasing appetizer.">
        	                <img src="/assets/images/snack-plate-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">	
    	                Mixed Seafood Snack Plate
    	            </div>
    	        </div>
    	        
    	       <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="1Rv5PMILK3M"  data-tooltip-title="Blueberry & Strawberry Smoothie" data-tooltip-body="Chef Becky Selengut walks through the simple steps that lead to a frosty, nutrient-packed smoothie with organic berries, spinach and chia seeds.">
        	                <img src="/assets/images/smoothie-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">	
    	                Blueberry & Strawberry Smoothie
    	            </div>
    	        </div>
    	       <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="5236Z6x5l08"  data-tooltip-title="Smoked Nova Lox Sandwiches" data-tooltip-body="Your family or guests will love Chef Becky Selengut&#8217;s idea for simple, artfully arranged, open-faced sandwiches featuring lox and tomatoes.">
        	                <img src="/assets/images/smoked-salmon-sandwich-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">	
    	                Smoked Nova Lox Sandwiches
    	            </div>
    	        </div>
    	       <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="HH7B5R9fujA"  data-tooltip-title="Pasta with Wild Spot Prawns" data-tooltip-body="Chef Becky Selengut shows how simple it is to create a mouthwatering meal using wild prawns, pasta, goat cheese, capers, olives and herbs.">
        	                <img src="/assets/images/linguine-prawns-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">	
    	                Pasta with Wild Spot Prawns
    	            </div>
    	        </div>
    	       <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="vss-fcG5xyU"  data-tooltip-title="Lemon Vinaigrette Salad Dressing" data-tooltip-body="Make your salads come to life with Chef Becky Selengut&#8217;s simple recipe for a classic vinaigrette with fresh lemons, white balsamic and organic olive oil.">
        	                <img src="/assets/images/vinaigrette-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">	
    	                Lemon Vinaigrette Salad Dressing
    	            </div>
    	        </div>
    	        
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="3d587pEY2EI"  data-tooltip-title="Canned Wild Sockeye Salad" data-tooltip-body="Without Chef Becky Selengut&#8217;s easy-to-follow recipe, we might never know how simple it is to prepare such a nutrient-rich, wild salmon pasta salad.">
        	                <img src="/assets/images/salmon-pasta-salad-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">	
    	                Canned Wild Sockeye Salad
    	            </div>
    	        </div>
    	       <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="v6IimKHX0_k"  data-tooltip-title="Wild Albacore Tuna Salad" data-tooltip-body="Using wild Pacific albacore tuna loin medallions, Chef Becky Selengut takes us through the simple process of preparing a classic French favorite.">
        	                <img src="/assets/images/tuna-nicoise-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">	
    	                Wild Albacore Tuna Salad
    	            </div>
    	        </div>
    	       <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="bPYQNNLYePw"  data-tooltip-title="Oat Fruit Nut Granola" data-tooltip-body="Why buy store-bought when even-more delicious and nutritious granola is as simple as this mixing and baking method by Chef Becky Selengut.">
        	                <img src="/assets/images/granola-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">	
    	                Oat Fruit Nut Granola
    	            </div>
    	        </div>
    	       <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered youtube-item" data-video-id="vCsRTamxWuw"  data-tooltip-title="An interview with Chef Becky Selengut" data-tooltip-body="A peak into Chef Becky Selengut&#8217;s passion for good fish and her  philosophy of standing back and letting the flavor of good fish shine.">
        	                <img src="/assets/images/becky-interview-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">	
    	                An interview with Chef Becky Selengut
    	            </div>
    	        </div>
    	    </div>
    	    <div class="clear"></div>
        </div>
	</div>
}}
%>'
           ,'<p>empty</p>'
           ,'Vital Choice - In The Kitchen'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[ContentPages]
           ([Url]
           ,[Name]
           ,[FileUrl]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[Assigned]
           ,[UserId])
     VALUES
           ('in-the-kitchen'
           ,'Vital Choice - In The Kitchen'
           ,NULL
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Content Individual with Recipe Categories')
           ,2
           ,1
           ,NULL)

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentPages] WHERE [Url] = 'vitalgreen')
BEGIN

DECLARE @contentItemId int

INSERT INTO [dbo].[ContentItems]
           ([Created]
           ,[Updated]
           ,[Template]
           ,[Description]
           ,[Title]
           ,[MetaKeywords]
           ,[MetaDescription])
     VALUES
           (GETDATE()
           ,GETDATE()
           ,'<%
<body:body>
{{
    @script(){{
        <script src="/app/common/dataAccess.js"></script>
        <script src="/app/modules/content/vitalgreen.js"></script>
    }}
    <div class="vitalgreen relative">
        <div data-ng-show="show" class="overlay hide"><div class="loading">Loading…</div></div>
        <div>
            <div class="header">
                <img src="/assets/images/VitalGreen_350x72.jpg">
            </div>
            <div class="step1">
                <h4>Vital Green Step #1: Provide Your Address and Information</h4>
                <span class="form-control-hint">
                    Please fill in all the fields marked with a red asterisk (leave the Address 2 field blank if it does not apply to you).<br />
                    Click "Continue" to see a list of FedEx shipping centers near you.<br />
                    Click where indicated at the top of the next page, to view and print out a shipping label for the foam recycling center nearest you.<br />
                </span>
            </div>
            @razor(@(new VC.Public.Models.VitalGreenRequestModel())){{~/Views/VitalGreen/_Step1.cshtml}}
            <div class="step2 hide">
                <h4>
                    Vital Green Step #2:<br />
                    Get FedEx Center Addresses and Your Foam-Recycling Center Shipping Label
                </h4>
                <div class="form-regular">
                    <span class="form-control-hint">
                        A FedEx shipping label has been created for your foam shipping cube. To view and
                        print it, please click <a href="#" id="link" target="_blank">HERE</a>
                        <br />
                        Affix the label to your clean foam shipping cube and drop the cube off at the FedEx
                        center nearest you.
                        <br />
                        Below is a list of FedEx drop-off centers near you, from which your foam shipping
                        cube will be sent to the nearest recycling center, at no charge.
                        <br />
                        We suggest that you copy the address of the nearest center, or print this page for
                        future reference.
                        <br />
                        <br />
                    </span>
                    <div class="items">
                    </div>
                </div>
            </div>
        </div>
    </div>
}}
%>'
           ,'<p>empty</p>'
           ,'Vital Green'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[ContentPages]
           ([Url]
           ,[Name]
           ,[FileUrl]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[Assigned]
           ,[UserId])
     VALUES
           ('vitalgreen'
           ,'Vital Green'
           ,NULL
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Content Individual')
           ,2
           ,1
           ,NULL)

END

GO

IF EXISTS(SELECT Id FROM ContentItems
WHERE Id IN 
(SELECT ContentItemId FROM ContentCategories 
WHERE ParentId IS NULL)
AND Title !='')
BEGIN

UPDATE ContentItems
SET Title=''
WHERE Id IN 
(SELECT ContentItemId FROM ContentCategories 
WHERE ParentId IS NULL)

END

GO