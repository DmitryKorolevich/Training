GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'UserPasswordForgot')
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
	<p>
	    Dear @(FirstName) @(LastName),</p><p>Please click the following 
	    <a href="@(Link)">link</a> to setup a new password
	</p>
	<p></p>
	<p>Vital Choice Administration,</p>
	<p></p>
	<p>
	    This is an automated message. Do not reply. This mailbox is not monitored.
	</p>
}}
%>'
           ,''
           ,'Vital Choice - Recover Your Forgot Password'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('UserPasswordForgot'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'PasswordReset'
           ,'Password reset via web')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'AdminPasswordForgot')
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
	<p>Dear @(FirstName) @(LastName),</p>
	<p>Please click the following <a href="@(Link)">link</a> to reset your password</p>
	<p></p><p>Vital Choice Administration,</p>
	<p></p>
	<p>This is an automated message. Do not reply. This mailbox is not monitored.</p>
}}
%>'
           ,''
           ,'Your Vital Choice User Password Reset'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('AdminPasswordForgot'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'PasswordReset'
           ,'Admin user password reset')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'AdminRegistration')
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
	<p>Dear @(FirstName) @(LastName),</p>
	<p>Please click the following <a href="@(Link)">link</a> to activate your account</p>
	<p></p><p>Vital Choice Administration,</p>
	<p></p><p>This is an automated message. Do not reply. This mailbox is not monitored.</p>
}}
%>'
           ,''
           ,'Your Vital Choice User Activation'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('AdminRegistration'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'UserActivation'
           ,'Admin registration')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'CustomerRegistrationViaWeb')
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
	<p>Dear @(FirstName) @(LastName), thank you for registering with our store!</p>
	<p>At any time you can log into your account to check order status, update your billing information, add multiple shipping addresses, and much more. To log in, use <a href="@(ProfileLink)">link</a></p>
	<p>Thanks again for visiting our store. Let us know if there is anything we can do to make your experience with us a better one!</p>
}}
%>'
           ,''
           ,'Vital Choice - Confirmation of Customer Registration'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('CustomerRegistrationViaWeb'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'SuccessfulUserRegistration'
           ,'Customer registration via web')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'AffiliateRegistrationViaWeb')
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
	<p>Dear @(FirstName) @(LastName), thank you for registering with our store!</p>
    <p>At any time you can log into your affiliate account. To log in, use <a href="@(ProfileLink)">link</a></p>
}}
%>'
           ,''
           ,'Vital Choice - Confirmation of Affiliate Registration'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('AffiliateRegistrationViaWeb'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'SuccessfulUserRegistration'
           ,'Affiliate registration via web')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'EmailArticle')
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
	<p>Dear @(RecipentName),</p>
    <p>@(FromName) (@(FromEmail)) has recommended you read this article from Vital Choice: <a href="@(Url)" target="_blank">@(Name)</a></p>
    <p>Personal message from @(FromName)@{:}@ @(Message)</p>
}}
%>'
           ,''
           ,'Check out this article I found on Vital Choice: @(Name)'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('EmailArticle'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'ContentUrlNotificationEmail'
           ,'Email article')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'EmailRecipe')
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
	<p>Dear @(RecipentName),</p>
    <p>@(FromName) (@(FromEmail)) has recommended you read this recipe from Vital Choice: <a href="@(Url)" target="_blank">@(Name)</a></p>
    <p>Personal message from @(FromName)@{:}@ @(Message)</p>
}}
%>'
           ,''
           ,'Check out this recipe I found on Vital Choice: @(Name)'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('EmailRecipe'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'ContentUrlNotificationEmail'
           ,'Email recipe')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'CustomerRegistrationViaAdmin')
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
	<p>Dear @(FirstName) @(LastName),</p>
	<p>Our records show that you recently had an account created for you. Your account is currently only available for phone orders. To begin using your account on our storefront please click the following 
	<a href="@(Link)">link</a> to setup a password and activate your account for online ordering</p>
	<p></p><p>Vital Choice Administration,</p><p></p><p>This is an automated message. Do not reply. This mailbox is not monitored.</p>
}}
%>'
           ,''
           ,'Vital Choice - Setup Your Account To Order Online'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('CustomerRegistrationViaAdmin'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'UserActivation'
           ,'Customer registration via admin')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'CustomerPasswordResetViaAdmin')
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
	<p>Dear @(FirstName) @(LastName),</p>
	<p>Please click the following <a href="@(Link)">link</a> to reset your password</p>
	<p></p><p>Vital Choice Administration,</p>
	<p></p>
	<p>This is an automated message. Do not reply. This mailbox is not monitored.</p>
}}
%>'
           ,''
           ,'Your Vital Choice User Password Reset'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('CustomerPasswordResetViaAdmin'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'PasswordReset'
           ,'Password reset via admin')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'CustomerWholesaleRegistrationViaWeb')
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
	<p>Dear @(FirstName) @(LastName), Thank you for submitting your wholesale application. Vital Choice Seafood.</p>
}}
%>'
           ,''
           ,'Vital Choice - Confirmation of Submitting Your Wholesale Application'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('CustomerWholesaleRegistrationViaWeb'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'SuccessfulUserRegistration'
           ,'Wholesale registration via web')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'AffiliateRegistrationViaAdmin')
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
	<p>Dear @(FirstName) @(LastName),</p>
	<p>Our records show that you recently had an account created for you. To begin using your account on our storefront please click the following 
	<a href="@(Link)">link</a> to setup a password and activate your account.</p>
	<p></p>
	<p>Vital Choice Administration,</p>
	<p></p><p>This is an automated message. Do not reply. This mailbox is not monitored.</p>
}}
%>'
           ,''
           ,'Vital Choice - Setup Your Affiliate Account'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('AffiliateRegistrationViaAdmin'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'UserActivation'
           ,'Affiliate registration via admin')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'CustomerServiceRequestEmail')
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
	<p>Name - @(Name)</p>
	<p>Email - @(Email)</p>
	<p>Comment - @(Comment)</p>
	<p>Vital Choice Administration,</p><p></p><p>This is an automated message. Do not reply. This mailbox is not monitored.</p>
}}
%>'
           ,''
           ,'Vital Choice - Customer Service Email'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('CustomerServiceRequestEmail'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'CustomerServiceEmail'
           ,'Customer service request email')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'HelpTicketUpdateCustomerNotification')
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
	<p>Dear @(Customer),</p><p>Details regarding your help desk ticket that you submitted regarding order #@(IdOrder) has been updated. 
	<a href="@(Url)">Please click here to review</a> or log into your Vital Choice customer profile to review your help desk tickets.</p><br/>
    <p>Please note that this is an automated message and this mailbox is not monitored. To make changes to your help desk tickets please submit a reply within the help desk ticket system found within your customer profile.</p><br/>
    <p>Sincerely,</p>
    <p>Vital Choice</p>
}}
%>'
           ,''
           ,'Your Vital Choice Help Desk #@(Id) Has Been Updated'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('HelpTicketUpdateCustomerNotification'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'HelpTicketEmail'
           ,'Help ticket update customer notification')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'BugTicketUpdateSuperAdminNotification')
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
	<p>New bug ticket was added - <a href="@(Url)">@(Url)</a></p>
}}
%>'
           ,''
           ,'Vital Choice - new bug ticket was added #@(Id)'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('BugTicketUpdateSuperAdminNotification'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'BugTicketEmail'
           ,'Bug ticket update super admin notification')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'BugTicketUpdateAuthorNotification')
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
	<p>Dear @(Customer),</p><p>Details regarding your help desk ticket that you submitted has been updated. 
	<a href="@(Url)">Please click here to review</a> or log into your Vital Choice customer profile to review your help desk tickets.</p><br/>
    <p>Please note that this is an automated message and this mailbox is not monitored. To make changes to your help desk tickets please submit a reply within the help desk ticket system found within your customer profile.</p><br/>
    <p>Sincerely,</p>
    <p>Vital Choice</p>
}}
%>'
           ,''
           ,'Your Vital Choice Bug Ticket #@(Id) Has Been Updated'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('BugTicketUpdateAuthorNotification'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'BugTicketEmail'
           ,'Bug ticket update super admin notification')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'HealthwiseSendGCEmail')
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
	<p>Your Vital Choice Gift Certificate(s):</p>
	@(BalancesBlock)
}}
%>'
           ,''
           ,'Your Vital Choice Gift Certificate(s)'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('HealthwiseSendGCEmail'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'GCNotificationEmail'
           ,'Healthwise send GC email')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'PrivacyRequestEmail')
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
	<p>Name - @(Name)</p>
	<p>Mailing Address - @(MailingAddress)</p>
	<p>Other Name - @(OtherName)</p>
	<p>Other Address - @(OtherAddress)</p>
	<p>Comment - @(Comment)</p>
}}
%>'
           ,''
           ,'Vital Choice - Privacy Request Email'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('PrivacyRequestEmail'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'PrivacyRequestEmail'
           ,'Privacy request email')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'GLOrdersImportEmail')
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
	<p>Summary of the following Gift List Import completed @date(Date) {{MM''/''dd''/''yyyy hh:mm tt}}</p>
	<p>Agent: @(Agent)</p>
	<p>Customer Name: @(CustomerFirstName) @(CustomerLastName)</p>
	<p>Customer #: @(IdCustomer)</p>
	<p>Total # of imported orders: @(@string.Format("{0:c}",model.ImportedOrdersAmount))</p>
	<p>Total $ amount of orders imported: @(ImportedOrdersAmount)</p>
	<p>Credit Card Selected: @(CardNumber)</p>
}}
%>'
           ,''
           ,'Vital Choice - "Gift List" orders were imported'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('GLOrdersImportEmail'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'GLOrdersImportEmail'
           ,'Gift List successful orders import email')

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [ModelType] = 'GLOrdersImportEmail')
BEGIN

UPDATE [dbo].[EmailTemplates]
SET [ModelType]='VitalChoice.Ecommerce.Domain.Mail.'+[ModelType]

UPDATE [dbo].[EmailTemplates] 
SET [ModelType]=REPLACE([ModelType],'VitalChoice.Ecommerce.Domain.Mail.','VitalChoice.Infrastructure.Domain.Mail.')
WHERE [ModelType] = 'VitalChoice.Ecommerce.Domain.Mail.GLOrdersImportEmail' OR [ModelType]='VitalChoice.Ecommerce.Domain.Mail.PrivacyRequestEmail'

ENd

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'OrderConfirmationEmail')
BEGIN

INSERT [dbo].[MasterContentItems] ([Name], [TypeId], [Template], [Created], [Updated], [StatusCode], [UserId])
VALUES 
('StoreFront Email Template', 11, 
N'<%
<body:body>
{{
    <!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
    <html>
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>Vital Choice Order Confirmation</title>
    </head>

    <body style="background-color: #e3e2e2;font-family: Tahoma, Geneva, sans-serif;font-size: 13px;color: #333;">
        <div align="center">
            <table width="800" border="0" cellspacing="0" cellpadding="0">
                <tr>
                    <td valign="top">
                        <table width="800" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="550" valign="top" bgcolor="#FFFFFF">
                                    <div align="left"><img src="https://@(PublicHost)/Assets/images/logo_email.jpg" width="215" height="76" border="0" usemap="#Map"></div>
                                </td>
                                <td width="250" bgcolor="#FFFFFF">
                                    <div align="left">
                                        <span style="font-size:20px;color:#c21722;">Order Confirmation</span><br />
                                        <span style="font-size:15px;">Ordered @date(DateCreated){{MM''/''dd''/''yyyy}}</span><br />
    		                            @if(ShipDelayDate){{
    		                                <span style="font-size:15px;">Ship Delay @date(ShipDelayDate){{MM''/''dd''/''yyyy}}</span><br />
    		                            }}
    		                            @ifnot(ShipDelayDate){{
    		                                @if(ShipDelayDateNP){{
    		                                    <span style="font-size:15px;">Non-Perishable Ship Delay @date(ShipDelayDateNP){{MM''/''dd''/''yyyy}}</span><br />
    		                                }}
    		                                @if(ShipDelayDateP){{
    		                                    <span style="font-size:15px;">Perishable Ship Delay @date(ShipDelayDateP){{MM''/''dd''/''yyyy}}</span><br />
    		                                }}
    		                            }}
                                        <span style="font-size:15px;">Order Number(s):</span> 
                                        <span style="color:#c21722;font-size:15px;">@(Id)</span>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td valign="top" bgcolor="#FFFFFF">
                		<div align="center">
                			<table width="760" border="0" cellspacing="0" cellpadding="0">
                				<tr>
                					<td width="530" valign="middle">
                						<div align="left">
                							<strong>Thank you for shopping at Vital Choice!</strong><br />
                							You''ve made a wise decision by choosing the finest<br />
                    						all-natural foods available, backed by our <a href="https://@(PublicHost)/content/the-vital-choice-guarantee">100% Guarantee</a>.<br /><br />
                    						&#8226; Please send corrections or questions to <a href="mailto:info@{@}@vitalchoice.com">info@{@}@vitalchoice.com</a> or call 866-482-5887, Monday thru Friday, from 7am-4pm Pacific Time. Replies to this email will NOT be received or seen.<br />
                    						&#8226; We will send your tracking number(s) after your order has been shipped.
                    					</div>
                    				</td>
                    				<td width="230" valign="top">
                    					<div align="center">
                    						<a href="https://@(PublicHost)/content/request-catalog">
                    							<img src="https://@(PublicHost)/Assets/images/fall-catalog170.jpg" width="143" height="193" border="0">
                    						</a>
                    					</div>
                    				</td>
                    			</tr>
                    		</table>
                        </div>
                    </td>
                </tr>
                @if(AutoShipFrequency){{
    		    <tr> 
    		        <td valign="top" bgcolor="#FFFFFF">
                		<div align="center">
                			<table width="760" border="0" cellspacing="0" cellpadding="0">
                				<tr>
                                	<td colspan="2" valign="middle">
                                	    <strong>About Your Auto-Ship Order</strong><br /><br />
                                        You asked us to ship @(AutoShipFrequencyProductName) to you every @(AutoShipFrequency) month(s).<br />
                                        As a courtesy, we''ll send you a reminder email 7 days before your next shipment of this product.<br />
                                        That reminder email will show the last 4 digits of the credit card you used, and tell you how to:<br />
                                        &nbsp;&#8226; Change your Shipping Address <br />
                                        &nbsp;&#8226; Pause or Cancel your Auto-Ship Order<br />
                                        &nbsp;&#8226; Change your Payment Method (credit card)<br />
                                        &nbsp;&#8226; Change the Frequency of your Auto-Ship order<br /><br />
                                        It''s easy to make changes ... our reminder email will walk you through the steps.
                                    </td>
                    			</tr>
                    		</table>
                        </div>
                    </td>
                </tr>
    		    }}
                <tr>
                    <td valign="top">&nbsp;</td>
                    <td valign="top">&nbsp;</td>
                </tr>
                <tr>
                    <td valign="top" bgcolor="#FFFFFF">
                        <table width="800" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="20" valign="top">&nbsp;</td>
                                <td width="260" valign="top">&nbsp;</td>
                                <td width="20" valign="top">&nbsp;</td>
                                <td width="260" valign="top">&nbsp;</td>
                                <td width="18" valign="top">&nbsp;</td>
                                <td valign="top">&nbsp;</td>
                                <td width="20" valign="top">&nbsp;</td>
                            </tr>
                            <tr>
                                <td width="20" valign="top">
                                    <img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20">
                                </td>
                                <td width="260" valign="top">
                                    <table width="260" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td valign="top" style="font-size:15px;">
                                                <div align="left">
                                                    Ordered by:<br />
                                                    @(BillToAddress.FirstName) @(BillToAddress.LastName)<br />
                                                    @(BillToAddress.Company)<br />
                                                    @(BillToAddress.Address1)<br />
                                                    @(BillToAddress.Address2)<br />
                                                    @(BillToAddress.City), @(BillToAddress.StateCodeOrCounty) @(BillToAddress.Zip) @(BillToAddress.Country)
                                    			</div>
    			                            </td>
                                        </tr>
                                        <tr>
                                            <td><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <div align="left">
                                                    <span style="color:#666;">
                                                        &#8226; Frozen and dry goods ship separately.<br />
                                                        &#8226; If your order includes both frozen and dry goods, you will receive a separate Shipping Confirmation for each.
                                                    </span>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                                <td width="260" valign="top">
                                    <table width="260" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td valign="top" style="font-size:15px;">
                                                <div align="left">
                                                    Shipped to:<br />
                                                    @(ShipToAddress.FirstName) @(ShipToAddress.LastName)<br />
                                                    @(ShipToAddress.Company)<br />
                                                    @(ShipToAddress.Address1)<br />
                                                    @(ShipToAddress.Address2)<br />
                                                    @(ShipToAddress.City), @(ShipToAddress.StateCodeOrCounty) @(ShipToAddress.Zip) @(ShipToAddress.Country)
                                    			</div>
    			                            </td>
                                        </tr>
                                        <tr>
                                            <td><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <div align="left">
                                                    <span style="color:#666;">
                                                        &#8226; Frozen items ship by 2nd Day Air or 1-3 Day Express Ground service, unless you upgraded to Overnight service.<br />
                                                        &#8226; Non-perishable goods ship by Ground, unless you upgraded to 2nd Day Air or Overnight.
                                                    </span>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                                <td width="200" valign="top" style="font-size:15px;">
                                    @if(GiftMessage){{
                            		<table width="200" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td valign="top" style="font-size:15px;"><div align="left">Gift Message:</div></td>
                                        </tr>
                                        <tr>
                                            <td><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <div align="left">
                                                    <span style="color:#666;font-family: Tahoma, Geneva, sans-serif;font-size: 13px;">
                                                        @(GiftMessage)
                                                    </span>
                                                </div>
                                            </td>
                                          </tr>
                                    </table>
                                    }}
    		                    </td>
                                <td><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                            </tr>
                            <tr>
                                <td valign="top">&nbsp;</td>
                                <td valign="top">&nbsp;</td>
                                <td valign="top">&nbsp;</td>
                                <td valign="top">&nbsp;</td>
                                <td valign="top">&nbsp;</td>
                                <td valign="top">&nbsp;</td>
                                <td valign="top">&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td valign="top" bgcolor="#FFFFFF">
                        <table width="800" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                                <td width="780">
                                    <div align="left">
                                        These items were included in your Order Number(s): <span style="color:#c21722;">@(Id)</span><br />
                                        <i>Please note that changes made after submitting your order may not be reflected below.</i>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                        </table>
                        <table width="800" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="20"><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                                <td width="76"><div align="left"><u>SKU</u></div></td>
                                <td width="20"><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                                <td width="359"><div align="left"><u>Description</u></div></td>
                                <td width="20"><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                                <td width="40"><div align="left"><u>Qty</u></div></td>
                                <td width="20"><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                                <td width="96"><div align="right"><u>Price</u></div></td>
                                <td width="20"><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                                <td width="52"><div align="right"><u>Subtotal</u></div></td>
                                <td width="20"><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                            </tr>
                            @list(Skus){{
                            <tr>
                                <td>&nbsp;</td>
                                <td valign="top"><div align="left">@(Code)</div></td>
                                <td>&nbsp;</td>
                                <td><div align="left">@(DisplayName)</div></td>
                                <td>&nbsp;</td>
                                <td valign="top"><div align="left">@(Quantity)</div></td>
                                <td>&nbsp;</td>
                                <td valign="top"><div align="right">@money(Price)</div></td>
                                <td>&nbsp;</td>
                                <td valign="top"><div align="right">@money(SubTotal)</div></td>
                                <td>&nbsp;</td>
                            </tr>
                            }}
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td colspan="10">Promo items</td>
                            </tr>
                            @list(PromoSkus){{
                            <tr>
                                <td>&nbsp;</td>
                                <td valign="top"><div align="left">@(Code)</div></td>
                                <td>&nbsp;</td>
                                <td><div align="left">@(DisplayName)</div></td>
                                <td>&nbsp;</td>
                                <td valign="top"><div align="left">@(Quantity)</div></td>
                                <td>&nbsp;</td>
                                <td valign="top"><div align="right">@money(Price)</div></td>
                                <td>&nbsp;</td>
                                <td valign="top"><div align="right">@money(SubTotal)</div></td>
                                <td>&nbsp;</td>
                            </tr>
                            }}
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td><div align="left">Order Subtotal</div></td>
                                <td>&nbsp;</td>
                                <td><div align="right">@money(ProductsSubtotal)</div></td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td><div align="left">Discount</div></td>
                                <td>&nbsp;</td>
                                <td><div align="right">@money(DiscountTotal)</div></td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td><div align="left">Shipping*</div></td>
                                <td>&nbsp;</td>
                                <td><div align="right">@money(ShippingTotal)</div></td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td><div align="left">Tax</div></td>
                                <td>&nbsp;</td>
                                <td><div align="right">@money(TaxTotal)</div></td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td><div align="left">Order Total**</div></td>
                                <td>&nbsp;</td>
                                <td><div align="right">@money(Total)</div></td>
                                <td>&nbsp;</td>
                            </tr>
                        </table>
                        <br /><br />
                        <div align="right">
                            <table width="300" border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <div align="left">@(PaymentTypeMessage)</div>
                                    </td>
                                    <td width="20"><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td valign="top" bgcolor="#FFFFFF">
                        <div align="center">
                            <table width="760" border="0" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td width="485">
                                        <div align="left">
                                            <span style="font-size:11px;">
                                                <a href="#" style="font-size:11px;color:#39F;">Answers to Common Shipping Questions</a><br />
                                                <a href="#" style="font-size:11px;color:#39F;">Answers to Common Storage &amp; Cooking Questions</a><br />
                                                Still have questions? Send an email to <a href="https://@(PublicHost)/content/contact-customer-service" style="font-size:11px;color:#39F;">Customer Service</a><br />
                                                or call us toll-free at 866-482-5887
                                            </span>
                                        </div>
                                    </td>
                                    <td width="275" height="63"><div align="center">Thank you for your order!</div></td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <div align="left">
                                            <span style="font-size:11px;">
                                                <br />*ABOUT SHIPPING CHARGES:
                                                <ul style="margin:5px; padding-left:15px;">
                                                    <li>
                                                        IF this is an Auto-Ship or E-Gift Certificate order, you will NOT be charged for Standard Shipping (normally $4.95 or $9.95,<br />
                                                        depending on the dollar total of an order), so the Order Total $ figure will NOT include any Standard Shipping charge displayed above.
                                                    </li>
                                                    <li>
                                                        IF you chose to upgrade to Premium Shipping Service during checkout, you will be charged for shipping. If so, your<br />
                                                        Total $ figure will include that shipping upgrade charge. Premium Shipping Service charges are shown on our <a href="#" style="font-size:11px;">Shipping page</a>.
                                                    </li>
                                                </ul>
                                                **NOTE: Your total does not reflect the $1.00 "test" charge applied during checkout to validate your card, which is removed within 7 days.
                                            </span>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>    
                <tr>
                    <td valign="top" bgcolor="#FFFFFF">&nbsp;</td>
                </tr>
                <tr>
                    <td valign="top"><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                </tr>
                <tr>
                    <td valign="top">
                        <table width="800" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="20" valign="top" bgcolor="#FFFFFF">&nbsp;</td>
                                <td valign="top" bgcolor="#FFFFFF">&nbsp;</td>
                                <td valign="bottom" bgcolor="#FFFFFF">&nbsp;</td>
                            </tr>
                            <tr>
                                <td width="20"><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                                <td width="760" valign="top" bgcolor="#FFFFFF"><img src="https://@(PublicHost)/Assets/images/btm-ship-conf2.jpg" width="760" height="146" border="0" usemap="#Map2"></td>
                                <td width="20"><img src="https://@(PublicHost)/Assets/images/spacer.gif" width="20" height="20"></td>
                            </tr>
                            <tr>
                                <td valign="top" bgcolor="#FFFFFF">&nbsp;</td>
                                <td valign="top" bgcolor="#FFFFFF">&nbsp;</td>
                                <td valign="bottom" bgcolor="#FFFFFF">&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        
        <map name="Map">
            <area shape="rect" coords="16,13,217,66" href="http://www.vitalchoice.com">
        </map>
    
        <map name="Map2">
            <area shape="rect" coords="461,93,502,135" href="https://plus.google.com/+VitalchoiceWildSeafood/posts">
            <area shape="rect" coords="410,94,451,136" href="http://www.youtube.com/user/VitalChoiceSeafood">
            <area shape="rect" coords="360,93,401,135" href="http://www.pinterest.com/vitalchoice/">
            <area shape="rect" coords="310,93,351,135" href="https://twitter.com/vitalchoice">
            <area shape="rect" coords="0,1,759,85" href="https://@(PublicHost)/content/newsletter-sign-up">
            <area shape="rect" coords="258,93,299,135" href="https://www.facebook.com/vitalchoice">
        </map>
    </body>
    
    </html>
}}
%>', GETDATE(), GETDATE(),2, NULL)

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

}}
%>'
           ,''
           ,'Vital Choice - Order Confirmation Email'
           ,NULL
           ,NULL)

SET @contentItemId=@@identity

INSERT INTO [dbo].[EmailTemplates]
           ([Name]
           ,[ContentItemId]
           ,[MasterContentItemId]
           ,[StatusCode]
           ,[UserId]
           ,[ModelType]
           ,[EmailDescription])
     VALUES
           ('OrderConfirmationEmail'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='StoreFront Email Template')
           ,2
           ,NULL
           ,'VitalChoice.Ecommerce.Domain.Mail.OrderConfirmationEmail'
           ,'Order Confirmation Email')

END

GO