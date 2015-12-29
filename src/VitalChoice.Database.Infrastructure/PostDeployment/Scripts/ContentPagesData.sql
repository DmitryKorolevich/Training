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