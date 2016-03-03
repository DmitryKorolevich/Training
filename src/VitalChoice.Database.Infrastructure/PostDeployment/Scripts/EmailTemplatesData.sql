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
N'@model() {{dynamic}}

<%    
<body>
{{
}}
    
<default> -> (Model)
{{
	@body()
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
<!DOCTYPE HTML PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Vital Choice: Wild Seafood & Organics</title>

</head>

<body style="padding:0; -webkit-text-size-adjust:none; -ms-text-size-adjust:none;" yahoo="fix">
    <div id="body_style" style="text-align:center; width:100%;">
        <!-- Outer Container -->
        <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse; ">
            <tr>
                <td bgcolor="#f0f0f0">

                    <!-- Message Body -->
                    <table class="message" align="center" border="0" cellpadding="0" cellspacing="0" style="border-collapse:collapse;">
                        <tr>
                            <td class="message" width="640" bgcolor="#ffffff">

                                <!-- Preheader Outer -->
                                <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                    <tr>
                                        <td bgcolor="#ffffff">

                                            <!-- Preheader Inner -->
                                            <table class="stack" align="center" border="0" cellpadding="0" cellspacing="0" style="border-collapse:collapse;">
                                                <tr align="center" style="display:block; font-size:0;">
                                                    <td width="320" align="center" valign="top" style="display:inline-block; max-width:320px;">

                                                        <!-- Content Left -->
                                                        <table class="content" align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                                            <tr>
                                                                <td align="left" class="center" style="padding:5px; color:#6d6e72; font-family:Arial, Helvetica, sans-serif; font-size:11px;"></td>
                                                            </tr>
                                                        </table>
                                                        <!-- End Content Left -->

                                                    </td>
                                                    <td width="320" align="center" valign="top" style="display:inline-block; max-width:320px;">

                                                        <!-- Content Right -->
                                                        <table class="content" align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                                            <tr>
                                                                <td align="right" class="center" style="padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:11px;"></td>
                                                            </tr>
                                                        </table>
                                                        <!-- End Content Right -->

                                                    </td>
                                                </tr>
                                            </table>
                                            <!-- End Preheader Inner-->

                                        </td>
                                    </tr>
                                </table>
                                <!-- End Preheader Outer -->
                                <!-- Full-Width Scalable Header Outer -->
                                <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                    <tr>
                                        <td bgcolor="#ffffff">

                                            <!--Full-Width Scalable Header Inner -->
                                            <table align="center" border="0" cellpadding="0" width="100%" cellspacing="0" style="border-collapse:collapse;">
                                                <tr>
                                                    <td align="center" valign="top" bgcolor="#ffffff" style="display:inline-block; max-width:640px;"></td>
                                                </tr>
                                            </table>
                                            <!-- End Full-Width Scalable Header Inner-->

                                        </td>
                                    </tr>
                                </table>
                                <!-- End Full-Width Scalable Header Outer -->
                                <!-- Full-Width Swappable Header Outer -->
                                <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                    <tr>
                                        <td bgcolor="#ffffff">

                                            <!--Full-Width Swappable Header Inner -->
                                            <table class="stack" align="center" border="0" cellpadding="0" cellspacing="0" style="border-collapse:collapse;">
                                                <tr>
                                                    <td align="center" valign="top" bgcolor="#ffffff" style="display:inline-block; max-width:640px;">

                                                        <!--Full-Width Swappable Content -->
                                                        <table align="center" border="0" cellpadding="10" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                                            <tr>
                                                                <td class="content">
                                                                    <a href="https://@(PublicHost)/">
                                                                        <img src="https://@(PublicHost)/assets/images/header-logo.png" width="100%" style="display:block; max-width:100%; height:auto;" border="0" />
                                                                    </a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <!-- EndFull-Width Swappable Content -->

                                                    </td>
                                                </tr>
                                            </table>
                                            <!-- End Full-Width Swappable Header Inner-->

                                        </td>
                                    </tr>
                                </table>
                                <!-- End Full-Width Swappable Header Outer -->
                                <!-- Mobile Hidden Navigation -->
                                <table border="0" cellpadding="0" cellspacing="0" align="center" width="100%" style="border-collapse:collapse;">
                                    <tr>
                                        <td bgcolor="#ffffff" class="mobhide" align="center">

                                            <!-- Mobile Hidden Navigation Cells -->
                                            <table class="stack" align="center" border="0" cellpadding="0" cellspacing="0" style="border-collapse:collapse;">
                                                <tr align="center" style="display:block; font-size:0;">
                                                    <td align="center" valign="top" style="display:inline-block; padding:5px 8px; font-family:Arial, Helvetica, sans-serif; font-size:11px; font-weight:bold; border-right:1px solid #6d6e71 !important;">
                                                        <a href="https://@(PublicHost)/products/wild-salmon" class="embiggen" style="text-decoration:none; color:#6d6e71;">Wild Salmon</a>
                                                    </td>
                                                    <td align="center" valign="top" style="display:inline-block; padding:5px 8px; font-family:Arial, Helvetica, sans-serif; font-size:11px; font-weight:bold; border-right:1px solid #6d6e71 !important;">
                                                        <a href="https://@(PublicHost)/products/wild-cod-tuna-halibut-more" class="embiggen" style="text-decoration:none; color:#6d6e71;">Wild Cod, Tuna & Halibut</a>
                                                    </td>
                                                    <td align="center" valign="top" style="display:inline-block; padding:5px 8px; font-family:Arial, Helvetica, sans-serif; font-size:11px; font-weight:bold; border-right:1px solid #6d6e71 !important;">
                                                        <a href="https://@(PublicHost)/products/wild-shrimp-shellfish" class="embiggen" style="text-decoration:none; color:#6d6e71;">Shellfish</a>
                                                    </td>
                                                    <td align="center" valign="top" style="display:inline-block; padding:5px 8px; font-family:Arial, Helvetica, sans-serif; font-size:11px; font-weight:bold; border-right:1px solid #6d6e71 !important;">
                                                        <a href="https://@(PublicHost)/products/canned-pouched-wild-seafood" class="embiggen" style="text-decoration:none; color:#6d6e71;">Canned Seafood</a>
                                                    </td>
                                                    <td align="center" valign="top" style="display:inline-block; padding:5px 8px; font-family:Arial, Helvetica, sans-serif; font-size:11px; font-weight:bold; border-right:1px solid #6d6e71 !important;">
                                                        <a href="#" class="embiggen" style="text-decoration:none; color:#6d6e71;">Omega-3''s & Supplements</a>
                                                    </td>
                                                    <td align="center" valign="top" style="display:inline-block; padding:5px 8px; font-family:Arial, Helvetica, sans-serif; font-size:11px; font-weight:bold;">
                                                        <a href="https://@(PublicHost)/products/specials-top-sellers" class="embiggen" style="text-decoration:none; color:#6d6e71;">Top Sellers</a>
                                                    </td>
                                                </tr>
                                            </table>
                                            <!--End Mobile Hidden Navigation Cells -->

                                        </td>
                                    </tr>
                                </table>
                                <!-- End Mobile Hidden Navigation -->
                                <!--Full-Width WYSIWYG Inner -->
                                <table align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                    <tr>
                                        <td align="center" valign="top" bgcolor="#ffffff" style="font-family: Arial, Helvetica, sans-serif; font-size:14px; color:#000000; padding:0 1px;">

                                            <div><img src="https://@(PublicHost)/Assets/images/order-confirmation-hero.jpg" alt="Thank you for your order!" width="638" style="display:block; max-width:100%; height:auto;" border="0" /></div>

                                        </td>
                                    </tr>
                                </table>
                                <!-- End Full-Width WYSIWYG Inner-->
                                <!-- Full-Width WYSIWYG Outer -->
                                <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                    <tr>
                                        <td bgcolor="#edf7f9" style="padding: 0;">

                                            <!--Full-Width WYSIWYG Inner -->
                                            <table align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                                <tr>
                                                    <td align="center" valign="top" bgcolor="#ffffff" style="font-family: Arial, Helvetica, sans-serif; font-size:14px; color:#000000; padding:0 1px;">

                                                        <div>
                                                            <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border-collapse:collapse;">



                                                                <tr>
                                                                    <td align="center" style="color: #6d6e72; font-family: Helvetica,Arial,sans-serif; font-size: 14px; line-height: 20px; font-weight: 300; padding:8px;">
                                                                        <p>
                                                                            Thank you for your purchase from Vital Choice Wild Seafood &amp; Organics! <br />
                                                                            We suggest you print out this Order Confirmation, or save it for future reference.
                                                                        </p>
                                                                    </td>
                                                                </tr>

                                                            </table>
                                                        </div>

                                                    </td>
                                                </tr>
                                            </table>
                                            <!-- End Full-Width WYSIWYG Inner-->

                                        </td>
                                    </tr>
                                </table>
                                <!-- End Full-Width WYSIWYG Outer -->
                                <!-- Stacking Thirds Outer -->
                                <div>
                                    <table width="639" style="border-collapse: collapse;">
                                        <tr>
                        					<td width="203" bgcolor="#f15c22" align="left" style="border-collapse:collapse;border:1px solid #ffffff;padding: 5px 5px 5px 5px; font-size: 16px; color: #FFFFFF; font-weight: bold; font-family: Arial, helvetica, sans-serif;">
                                                Bill To
                                            </td>
                        					<td width="203" bgcolor="#f15c22" align="left" style="border-collapse:collapse;border:1px solid #ffffff;padding: 5px 5px 5px 5px; font-size: 16px; color: #FFFFFF; font-weight: bold; font-family: Arial, helvetica, sans-serif;">
                                                Ship To
                                            </td>
                        					<td width="203" align="left" valign="top" bgcolor="#f15c22" style="border-collapse:collapse;border:1px solid #ffffff;padding: 5px 5px 5px 5px; font-size: 16px; color: #FFFFFF; font-weight: bold; font-family: Arial, helvetica, sans-serif;">
                        						Order Info
                        					</td>
                        				</tr>
                        				<tr>
                        					<td align="left" valign="top" style="padding: 5px 25px 5px 10px; font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif;">
                                                @(BillToAddress.FirstName) @(BillToAddress.LastName)<br />
                                                @(BillToAddress.Company)<br />
                                                @(BillToAddress.Address1)<br />
                                                @(BillToAddress.Address2)<br />
                                                @(BillToAddress.City), @(BillToAddress.StateCodeOrCounty) @(BillToAddress.Zip) @(BillToAddress.Country)
                                            </td>
                        					<td align="left" valign="top" style="padding: 5px 25px 5px 10px; font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif;">
                                                @(ShipToAddress.FirstName) @(ShipToAddress.LastName)<br />
                                                @(ShipToAddress.Company)<br />
                                                @(ShipToAddress.Address1)<br />
                                                @(ShipToAddress.Address2)<br />
                                                @(ShipToAddress.City), @(ShipToAddress.StateCodeOrCounty) @(ShipToAddress.Zip) @(ShipToAddress.Country)
                                            </td>
                        					<td align="left" valign="top" style="padding: 5px 0 5px 5px; font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif;">
                                                Order#: <br/>
                                                @(Id) <br/>
                                                See "Additional Information"<br />below for shipping guidance.<br/>
                                                Purchase Date: <br/>
                                                @date(DateCreated){{MM''/''dd''/''yyyy}}<br/>
                        					</td>
                        				</tr>
                                    <!-- End Stacking Thirds Inner-->
                        			@if(@(!string.IsNullOrEmpty(model.GiftMessage) || model.ShipDelayDate!=null || model.ShipDelayDateP!=null || model.ShipDelayDateNP!=null || !string.IsNullOrEmpty(model.DeliveryInstructions)))
                        			{{
                                        <tr>
                        					<td bgcolor="#f15c22" align="left" style="border-collapse:collapse;border:1px solid #ffffff;padding: 5px 5px 5px 5px; font-size: 15px; color: #FFFFFF; font-weight: bold; font-family: Arial, helvetica, sans-serif;">
                                                Gift Message
                                            </td>
                        					<td bgcolor="#f15c22" align="left" style="border-collapse:collapse;border:1px solid #ffffff;padding: 5px 5px 5px 5px; font-size: 15px; color: #FFFFFF; font-weight: bold; font-family: Arial, helvetica, sans-serif;">
                                                Requested Ship Date
                                            </td>
                        					<td bgcolor="#f15c22" align="left" style="border-collapse:collapse;border:1px solid #ffffff;padding: 5px 5px 5px 5px; font-size: 15px; color: #FFFFFF; font-weight: bold; font-family: Arial, helvetica, sans-serif;">
                                                Driver Delivery Instructions
                                            </td>
                        				</tr>
                        				<tr>
                        					<td align="left" valign="top" style="padding: 5px 25px 5px 10px; font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif;">
                        						@(GiftMessage)
                                            </td>
                                            <td align="left" valign="top" style="padding: 5px 25px 5px 10px; font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif; text-align: center;">
                                                Week of<br/>
            		                            @if(ShipDelayDate){{
            		                                @date(ShipDelayDate){{MM''/''dd''/''yyyy}}
            		                            }}
            		                            @ifnot(ShipDelayDate){{
            		                                @if(ShipDelayDateNP){{
            		                                    Perishable Foods <br />
            		                                    @date(ShipDelayDateNP){{MM''/''dd''/''yyyy}}<br />
            		                                }}
            		                                @if(ShipDelayDateP){{
            		                                    Non-Perishable Goods <br />
            		                                    @date(ShipDelayDateP){{MM''/''dd''/''yyyy}}<br />
            		                                }}
            		                            }}
                                            </td>
                        					<td align="left" valign="top" style="padding: 5px 0 5px 5px; font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif;">
                        						@(DeliveryInstructions)
                                            </td>
                                        </tr>
                                    }}
                        		    </table>
                                </div>
                            <!-- End Stacking Thirds Outer -->
                            <!-- Full Width Body -->
                            <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;" bgcolor="#f15c22">
                                <tr>
                                    <td>
                                        <!-- Full Width Body Content -->
                                        <table class="content" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                            <tr>
                                                <td align="center" style="padding:5px 10px 5px 20px; font-size:16px; color:#FFFFFF; font-weight:bold; font-family:Arial, helvetica, sans-serif; min-width:110px;">
                                                    Your Order Details
                                                </td>
                                            </tr>
                                        </table>
                                        <!-- End Full Width Body Content -->
                                    </td>
                                </tr>
                            </table>
                            <!-- End Full Width Body -->
                            <!-- Stacking Halves -->
                            <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;margin-top:5px;" bgcolor="#ffffff">
                                <tr>
                                    <td>
                                        <!-- Full Width Body Content -->
                                        <table class="content" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                            <tr>
                                                <td width="85" align="left" style="font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif; text-decoration: underline;"></td>
                                                <td width="90" align="left" style="font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif; text-decoration: underline;">
                                                    Product ID #
                                                </td>
                                                <td width="269" align="left" style="font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif; text-decoration: underline;">
                                                    Description
                                                </td>
                                                <td width="65" align="left" style="font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif; text-decoration: underline;">
                                                    Quantity
                                                </td>
                                                <td width="65" align="left" style="font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif; text-decoration: underline;">
                                                    Price
                                                </td>
                                                <td width="65" align="left" style="font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif; text-decoration: underline;">
                                                    Subtotal
                                                </td>
                                            </tr>
                                        @list(Skus){{
                                            <tr>
                                                <td align="left">
                                                    <a href="https://@(@root.Model.PublicHost)/@(ProductPageUrl)">
                                                        <img src="https://@(@root.Model.PublicHost)/@(IconUrl)" border="0" width="75" />
                                                    </a>
                                                </td>
                                                <td align="left" style="padding:0 5px 0 0; font-size:13px; color:#6d6e72; font-family:Arial, helvetica, sans-serif;">
                                                    @(Code)
                                                </td>
                                                <td align="left" style="padding:0 10px 0 0; font-size:13px; color:#6d6e72; font-family:Arial, helvetica, sans-serif; min-width:100px">
                                                    @(DisplayName)
                                                </td>
                                                <td align="left" style="padding:0 5px 0 5px; font-size:13px; color:#6d6e72; font-family:Arial, helvetica, sans-serif;">
                                                    @(Quantity)
                                                </td>
                                                <td align="left" style="padding:0 5px 0 0; font-size:13px; color:#6d6e72; font-family:Arial, helvetica, sans-serif;">
                                                    @money(Price)
                                                </td>
                                                <td align="left" style="padding:0 5px 0 0; font-size:13px; color:#6d6e72; font-family:Arial, helvetica, sans-serif;">
                                                    @money(SubTotal)
                                                </td>
                                            </tr>
                        				}}
                                        @if(@model.PromoSkus.Count>0)
                                        {{
                                            <tr>
                                                <td colspan="6">
                                                    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;" bgcolor="#f15c22">
                                                        <tr>
                                                            <td>
                                                                <!-- Full Width Body Content -->
                                                                <table class="content" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                                                    <tr>
                                                                        <td align="center" style="padding:5px 10px 5px 20px; font-size:16px; color:#FFFFFF; font-weight:bold; font-family:Arial, helvetica, sans-serif; min-width:110px;">
                                                                            Promo Products
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <!-- End Full Width Body Content -->
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            @list(PromoSkus){{
                                                <tr>
                                                    <td align="left">
                                                        <a href="https://@(@root.Model.PublicHost)/@(ProductPageUrl)">
                                                            <img src="https://@(@root.Model.PublicHost)/@(IconUrl)" border="0" width="75" />
                                                        </a>
                                                    </td>
                                                    <td align="left" style="padding:0 5px 0 0; font-size:13px; color:#6d6e72; font-family:Arial, helvetica, sans-serif;">
                                                        @(Code)
                                                    </td>
                                                    <td align="left" style="padding:0 10px 0 0; font-size:13px; color:#6d6e72; font-family:Arial, helvetica, sans-serif; min-width:100px">
                                                        @(DisplayName)
                                                    </td>
                                                    <td align="left" style="padding:0 5px 0 5px; font-size:13px; color:#6d6e72; font-family:Arial, helvetica, sans-serif;">
                                                        @(Quantity)
                                                    </td>
                                                    <td align="left" style="padding:0 5px 0 0; font-size:13px; color:#6d6e72; font-family:Arial, helvetica, sans-serif;">
                                                        @money(Price)
                                                    </td>
                                                    <td align="left" style="padding:0 5px 0 0; font-size:13px; color:#6d6e72; font-family:Arial, helvetica, sans-serif;">
                                                        @money(SubTotal)
                                                    </td>
                                                </tr>
                            				}}
                            			}}
                                        </table>
                                        <!-- End Full Width Body Content -->
                                    </td>
                                </tr>
                            </table>
                            <!-- End Stacking Halves -->
                            <!-- Total -->
                            <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                <tr>
                                    <td>
                                        <!-- Total Content -->
                                        <table class="content" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                            <tr>
                                                <td align="right" valign="top" style="font-family:Arial, helvetica, sans-serif; color:#6d6e72; font-size:13px; padding:10px;">
                                                    Subtotal: @money(ProductsSubtotal)<br />
                                                    Discount: @money(DiscountTotal)<br />
                                                    Shipping: @money(ShippingTotal)<br />
                                                    Tax: @money(TaxTotal)<br />
                                                    Gift Certificates: @money(GiftCertificatesTotal)<br />
                                                    <br />
                                                    <strong>Total: @money(Total)</strong>
                                                </td>
                                            </tr>
                                        </table>
                                        <!-- End Total Content -->
                                    </td>
                                </tr>
                            </table>
                            <!-- End Total -->
                            <!-- Order Info Header -->
                            <table bgcolor="#f15c22" width="100%" border="0" cellspacing="0" cellpadding="0" style="border-collapse:collapse;">
                                <tr>
                                    <td align="center" valign="top" style="font-family:Helvetica, Arial, sans-serif; color:#ffffff; padding:10px; font-weight:bold; text-align: center;font-size:16px;">Need Further Assistance?</td>
                                </tr>
                            </table>
                            <!-- End Order Info Header -->
                            <!-- Further Assistance Text -->
                            <br />
                            <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                <tr>
                                    <td bgcolor="#edf7f9" style="padding: 0;">
                        
                                        <!--Full-Width WYSIWYG Inner -->
                                        <table align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                            <tr>
                                                <td align="center" valign="top" bgcolor="#ffffff" style="font-family: Arial, Helvetica, sans-serif; font-size:13px; color:#000000; padding:0 1px;">
                                                    <div>
                                                        <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border-collapse:collapse;">
                        
                                                            <tr>
                                                                <td style="color: #6d6e72; font-family: Helvetica,Arial,sans-serif; font-size: 13px; line-height: 20px; font-weight: 300; padding:8px;">
                                                                    <li>Replies to this email WILL NOT be received or seen.</li>
                                                                    <li>Please send corrections or questions to info@{@}@vitalchoice.com or call 866-482-5887, Monday through Friday, from 7AM-4PM Pacific Time.</li>
                                                                    <li>We will send you your Tracking Number(s) after your order has been shipped.</li>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                        <!-- End Full-Width WYSIWYG Inner-->
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <!-- End Further Assistance Text -->
                            <!-- Additional Info Header -->
                            <table bgcolor="#f15c22" width="100%" border="0" cellspacing="0" cellpadding="0" style="border-collapse:collapse;">
                                <tr>
                                    <td align="center" valign="top" style="font-family:Helvetica, Arial, sans-serif; color:#ffffff; padding:10px; font-weight:bold; text-align: center;font-size:16px;">Additional Information</td>
                                </tr>
                            </table>
                            <!-- End Additional Info Header -->
                            <!-- Additional Info Text -->
                            <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                <tr>
                                    <td bgcolor="#edf7f9" style="padding: 0;">
                        
                                        <!--Full-Width WYSIWYG Inner -->
                                        <table align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                            <tr>
                                                <td align="center" valign="top" bgcolor="#ffffff" style="font-family: Arial, Helvetica, sans-serif; font-size:14px; color:#000000; padding:0 1px;">
                        
                                                    <div>
                                                        <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border-collapse:collapse;">
                        
                                                            <tr>
                                                                <td style="color: #6d6e72; font-family: Helvetica,Arial,sans-serif; font-size: 13px; line-height: 20px; font-weight: 300; padding:0 8px;">
                                                                    <span style="font-weight:bold; font-size:16px;">
                                                                        <br />
                                                                        General Shipping Info:
                                                                    </span><br />
                                                                    <li>There are NO Saturday or Sunday deliveries.</li>
                                                                    <li>Gift Certificates ship free; eGift Certificates are delivered free by email.</li>
                                                                    <li>Standard and Premium Shipping Service charges are shown on our Shipping Page.</li>
                                                                    <li>Website Orders placed on Saturday or Sunday will be sent on Tuesday (Monday if upgraded to 2nd Day Air or Overnight service).</li>
                                                                    <li>If you have other delivery date questions or concerns, please call our Customer Service team at 866-482-5887 Monday-Friday, 7AM-4PM Pacific Time.</li>
                                                                    <br />
                                                                    <u>Frozen Foods</u> (most of our non-canned foods) ship Monday through Wednesday by 1-3 Day Express Ground service (most orders) or 2nd Day Air. Frozen Items ordered on Wednesday after 10 AM Pacific time and upgraded to Overnight Service will ship on Thursday.
                                                                    <br /><br />
                                                                    <u>Non-Perishable Goods</u> (such as supplements and canned foods) ship by Ground service Monday through Friday. Non-Perishable Items typically arrive in 1-7 days, depending on destination, unless you upgraded to 2nd Day Air or Overnight service.
                                                                    <br /><br />
                                                                    <u>Chilled Foods</u> (such as live shellfish) ship Overnight (Tuesday through Thursday only), on cold gel packs.
                        
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="color: #6d6e72; font-family: Helvetica,Arial,sans-serif; font-size: 11px; line-height: 20px; font-weight: 300; padding:0 8px;">
                                                                    <br />
                                                                    <a href="#" target="_blank" style="font-size:13px; text-decoration:underline; color:#f15c22">Answers to Common Shipping Questions ></a><br />
                                                                    <br />
                                                                    <a href="#" target="_blank" style="font-size:13px; text-decoration:underline; color:#f15c22">Answers to Common Storage &amp; Cooking Questions ></a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                        
                                                </td>
                                            </tr>
                                        </table>
                                        <!-- End Full-Width WYSIWYG Inner-->
                        
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <!-- End Additional Info Text -->
                            <!--Full-Width WYSIWYG Inner -->
                            <table align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                                <tr>
                                    <td align="center" valign="top" bgcolor="#ffffff" style="font-family: Arial, Helvetica, sans-serif; font-size:14px; color:#000000; padding:0 1px;">
                                        <div>
                                            <a href="https://@(PublicHost)/content/newsletter-sign-up">
                                                <img src="https://@(PublicHost)/Assets/images/cart-recovery-newslettersignup_v2.jpg" width="638" style="display:block; max-width:100%; height:auto;" border="0" />
                                            </a>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <!-- End Full-Width WYSIWYG Inner-->
                        </td>
                    </tr>
                </table>
                <!-- End Message Body-->
                
                <!-- Begin Social Icons -->
                <div><br /></div>
                    <table align="center" border="0" cellpadding="5" cellspacing="0" style="border-collapse:collapse;">
                        <tr>
                            <td colspan="7" align="center"><span style="font-size:15px; font-weight:bold; color:#6d6e72; font-family:Arial, Helvetica, sans-serif;">FEELING SOCIAL?</span></td>
                        </tr>
                        <tr>
                            <td align="center" valign="middle"><a href="https://www.facebook.com/vitalchoice" target="blank"><img src="https://@(PublicHost)/Assets/images/VC-Facebook.png" height="40" width="40" style="border:none; display:block;" alt="Facebook" /></a></td>
                            <td align="center" valign="middle"><a href="#" target="blank"><img src="https://@(PublicHost)/Assets/images/VC-Blog.png" height="40" width="40" style="border:none; display:block;" alt="Blogt" /></a></td>
                            <td align="center" valign="middle"><a href="https://twitter.com/vitalchoice" target="blank"><img src="https://@(PublicHost)/Assets/images/VC-Twitter.png" height="40" width="40" style="border:none; display:block;" alt="Twitter" /></a></td>
                            <td align="center" valign="middle"><a href="https://plus.google.com/109701577970251794205/posts" target="blank"><img src="https://@(PublicHost)/Assets/images/VC-Google.png" height="40" width="40" style="border:none; display:block;" alt="Google Plus" /></a></td>
                            <td align="center" valign="middle"><a href="https://www.pinterest.com/vitalchoice/" target="blank"><img src="https://@(PublicHost)/Assets/images/VC-Pinterest.png" height="40" width="40" style="border:none; display:block;" alt="Pinterest" /></a></td>
                            <td align="center" valign="middle"><a href="https://instagram.com/vitalchoice/" target="blank"><img src="https://@(PublicHost)/Assets/images/social-icons-Insta.png" height="40" width="40" style="border:none; display:block;" alt="Instagram" /></a></td>
                            <td align="center" valign="middle"><a href="https://www.bcorporation.net/community/vital-choice" target="blank"><img src="https://@(PublicHost)/Assets/images/VC-BCorp.png" height="40" width="40" style="border:none; display:block;" alt="B Corp" /></a></td>
                        </tr>
                    </table>
                    <!-- End Social Icons -->
                    <!-- Footer Outer -->
                    <table border="0" align="center" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">
                        <tr>
                            <td align="center">
                                <!--Footer Inner -->
                                <table align="center" border="0" cellpadding="0" cellspacing="0" style="border-collapse:collapse;">
                                    <tr>
                                        <td align="center" valign="top" style="padding:10px; color:#000000; font-family:Arial, Helvetica, sans-serif; font-size:10px; line-height:18px;">
                                            <span style="font-size:12px; font-weight:bold; color:#6d6e72;">These items were included in your Order Number: @(Id)<br />Please note that changes made after submitting your order may not be reflected in this confirmation.</span><br /><br />
                                        </td>
                                    </tr>
                                </table>
                                <!-- End Footer Inner-->
                            </td>
                        </tr>
                    </table>
                    <!-- End Footer Outer -->
                    <img src="https://@(PublicHost)/Assets/images/spacer.gif" style="display:block;" height="1" width="1" />
                </td>
            </tr>
        </table>
    <!-- End Outer Container -->
    </div>
</body>
</html>
}} :: VitalChoice.Ecommerce.Domain.Mail.OrderConfirmationEmail
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

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'OrderShippingConfirmationEmail')
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
<!DOCTYPE HTML PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <title>Vital Choice: Wild Seafood & Organics</title>
</head>

<body style="padding: 0; -webkit-text-size-adjust: none; -ms-text-size-adjust: none;" yahoo="fix">
    <div id="body_style" style="text-align: center; width: 100%;">
        <!-- Outer Container -->
        <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
            <tr>
                <td bgcolor="#f0f0f0">
                    <!-- Message Body -->
                    <table class="message" align="center" border="0" cellpadding="0" cellspacing="0" style="border-collapse: collapse;">
                        <tr>
                            <td class="message" width="640" bgcolor="#ffffff">
                                <!-- Preheader Outer -->
                                <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                    <tr>
                                        <td bgcolor="#ffffff">
                                
                                            <!-- Preheader Inner -->
                                            <table class="stack" align="center" border="0" cellpadding="0" cellspacing="0" style="border-collapse: collapse;">
                                                <tr align="center" style="display: block; font-size: 0;">
                                                    <td width="320" align="center" valign="top" style="display: inline-block; max-width: 320px;">
                                
                                                        <!-- Content Left -->
                                                        <table class="content" align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                                            <tr>
                                                                <td align="left" class="center" style="padding: 5px; color: #6d6e72; font-family: Arial, Helvetica, sans-serif; font-size: 11px;"></td>
                                                            </tr>
                                                        </table>
                                                        <!-- End Content Left -->
                                
                                                    </td>
                                                    <td width="320" align="center" valign="top" style="display: inline-block; max-width: 320px;">
                                
                                                        <!-- Content Right -->
                                                        <table class="content" align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                                            <tr>
                                                                <td align="right" class="center" style="padding: 5px; font-family: Arial, Helvetica, sans-serif; font-size: 11px;"></td>
                                                            </tr>
                                                        </table>
                                                        <!-- End Content Right -->
                                
                                                    </td>
                                                </tr>
                                            </table>
                                            <!-- End Preheader Inner-->
                                
                                        </td>
                                    </tr>
                                </table>
                                <!-- End Preheader Outer -->
                                <!-- Full-Width Scalable Header Outer -->
                                <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                    <tr>
                                        <td bgcolor="#ffffff">
                                
                                            <!--Full-Width Scalable Header Inner -->
                                            <table align="center" border="0" cellpadding="0" width="100%" cellspacing="0" style="border-collapse: collapse;">
                                                <tr>
                                                    <td align="center" valign="top" bgcolor="#ffffff" style="display: inline-block; max-width: 640px;"></td>
                                                </tr>
                                            </table>
                                            <!-- End Full-Width Scalable Header Inner-->
                                
                                        </td>
                                    </tr>
                                </table>
                                <!-- End Full-Width Scalable Header Outer -->
                                <!-- Full-Width Swappable Header Outer -->
                                <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                    <tr>
                                        <td bgcolor="#ffffff">
                                
                                            <!--Full-Width Swappable Header Inner -->
                                            <table class="stack" align="center" border="0" cellpadding="0" cellspacing="0" style="border-collapse: collapse;">
                                                <tr>
                                                    <td align="center" valign="top" bgcolor="#ffffff" style="display: inline-block; max-width: 640px;">
                                
                                                        <!--Full-Width Swappable Content -->
                                                        <table align="center" border="0" cellpadding="10" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                                            <tr>
                                                                <td class="content">
                                                                    <a href="https://@(PublicHost)/">
                                                                        <img src="https://@(PublicHost)/assets/images/header-logo.png"
                                                                             width="100%" style="display: block; max-width: 100%; height: auto;"
                                                                             border="0"/>
                                                                    </a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <!-- EndFull-Width Swappable Content -->
                                
                                                    </td>
                                                </tr>
                                            </table>
                                            <!-- End Full-Width Swappable Header Inner-->
                                
                                        </td>
                                    </tr>
                                </table>
                                <!-- End Full-Width Swappable Header Outer -->
                                <!-- Mobile Hidden Navigation -->
                                <table border="0" cellpadding="0" cellspacing="0" align="center" width="100%" style="border-collapse: collapse;">
                                    <tr>
                                        <td bgcolor="#ffffff" class="mobhide" align="center">
                                
                                            <!-- Mobile Hidden Navigation Cells -->
                                            <table class="stack" align="center" border="0" cellpadding="0" cellspacing="0" style="border-collapse: collapse;">
                                                <tr align="center" style="display: block; font-size: 0;">
                                                    <td align="center" valign="top" style="display:inline-block; padding:5px 8px; font-family:Arial, Helvetica, sans-serif; font-size:11px; font-weight:bold; border-right:1px solid #6d6e71 !important;">
                                                        <a href="https://@(PublicHost)/products/wild-salmon" class="embiggen" style="text-decoration:none; color:#6d6e71;">Wild Salmon</a>
                                                    </td>
                                                    <td align="center" valign="top" style="display:inline-block; padding:5px 8px; font-family:Arial, Helvetica, sans-serif; font-size:11px; font-weight:bold; border-right:1px solid #6d6e71 !important;">
                                                        <a href="https://@(PublicHost)/products/wild-cod-tuna-halibut-more" class="embiggen" style="text-decoration:none; color:#6d6e71;">Wild Cod, Tuna & Halibut</a>
                                                    </td>
                                                    <td align="center" valign="top" style="display:inline-block; padding:5px 8px; font-family:Arial, Helvetica, sans-serif; font-size:11px; font-weight:bold; border-right:1px solid #6d6e71 !important;">
                                                        <a href="https://@(PublicHost)/products/wild-shrimp-shellfish" class="embiggen" style="text-decoration:none; color:#6d6e71;">Shellfish</a>
                                                    </td>
                                                    <td align="center" valign="top" style="display:inline-block; padding:5px 8px; font-family:Arial, Helvetica, sans-serif; font-size:11px; font-weight:bold; border-right:1px solid #6d6e71 !important;">
                                                        <a href="https://@(PublicHost)/products/canned-pouched-wild-seafood" class="embiggen" style="text-decoration:none; color:#6d6e71;">Canned Seafood</a>
                                                    </td>
                                                    <td align="center" valign="top" style="display:inline-block; padding:5px 8px; font-family:Arial, Helvetica, sans-serif; font-size:11px; font-weight:bold; border-right:1px solid #6d6e71 !important;">
                                                        <a href="#" class="embiggen" style="text-decoration:none; color:#6d6e71;">Omega-3''s & Supplements</a>
                                                    </td>
                                                    <td align="center" valign="top" style="display:inline-block; padding:5px 8px; font-family:Arial, Helvetica, sans-serif; font-size:11px; font-weight:bold;">
                                                        <a href="https://@(PublicHost)/products/specials-top-sellers" class="embiggen" style="text-decoration:none; color:#6d6e71;">Top Sellers</a>
                                                    </td>
                                                </tr>
                                            </table>
                                            <!--End Mobile Hidden Navigation Cells -->
                                
                                        </td>
                                    </tr>
                                </table>
                                <!-- End Mobile Hidden Navigation -->
                                <!--Full-Width WYSIWYG Inner -->
                                <table align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                    <tr>
                                        <td align="center" valign="top" bgcolor="#ffffff" style="font-family: Arial, Helvetica, sans-serif; font-size: 14px; color: #000000; padding: 0 1px;">
                                
                                            <div>
                                                <img src="https://@(PublicHost)/Assets/images/shipping-confirmation-hero.jpg"
                                                     alt="Thank you for your order!" width="638" style="display: block; max-width: 100%; height: auto;"
                                                     border="0"/>
                                            </div>
                                
                                        </td>
                                    </tr>
                                </table>
                                <!-- End Full-Width WYSIWYG Inner-->
                                <!-- Full-Width WYSIWYG Outer -->
                                <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                    <tr>
                                        <td bgcolor="#edf7f9" style="padding: 0;">
                                
                                            <!--Full-Width WYSIWYG Inner -->
                                            <table align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                                <tr>
                                                    <td align="center" valign="top" bgcolor="#ffffff" style="font-family: Arial, Helvetica, sans-serif; font-size: 14px; color: #000000; padding: 0 1px;">
                                
                                                        <div>
                                                            <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border-collapse: collapse;">
                                
                                
                                
                                                                <tr>
                                                                <td align="center" style="color: #6d6e72; font-family: Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; font-weight: 300; padding: 8px;">
                                                                <p>
                                                                <strong>GOOD NEWS!</strong> Your purchase from Vital
                                                                Choice Wild Seafood &amp; Organics has shipped!
                                                                <br/>
                                                                <br/>
                                                                <strong>Order Number:</strong> @(Id)
                                                                <br/>
                                                                <strong>Tracking Number(s):</strong>
                                                                @list(TrackingInfoItems){{
                                                                <br /><a href="@(ServiceUrl)" target="_blank">@(Number)</a>
                                                                }}
                                                                <br/>
                                                                <strong>Shipper:</strong> @(Carrier)
                                                                <br/>
                                                                <strong><a href="@(ServiceUrl)" target="_blank">Click here to Track Your Shipment Online</a></strong>
                                                                <br/>
                                                                <br/>See "Additional Information" below for shipping
                                                                guidance.
                                                                <br/>
                                                                <br/>
                                
                                
                                                            </table>
                                                        </div>
                                
                                                    </td>
                                                </tr>
                                            </table>
                                            <!-- End Full-Width WYSIWYG Inner-->
                                
                                        </td>
                                    </tr>
                                </table>
                                <!-- End Full-Width WYSIWYG Outer -->
                                <!-- Stacking Thirds Outer -->
                                
                                
                                <table width="639" style="border-collapse: collapse;">
                                    <tr>
                                		<td width="203" bgcolor="#f15c22" align="left" style="border-collapse:collapse;border:1px solid #ffffff;padding: 5px 5px 5px 5px; font-size: 16px; color: #FFFFFF; font-weight: bold; font-family: Arial, helvetica, sans-serif;">
                                            Bill To
                                        </td>
                                		<td width="203" bgcolor="#f15c22" align="left" style="border-collapse:collapse;border:1px solid #ffffff;padding: 5px 5px 5px 5px; font-size: 16px; color: #FFFFFF; font-weight: bold; font-family: Arial, helvetica, sans-serif;">
                                            Ship To
                                        </td>
                                		<td width="203" align="left" valign="top" bgcolor="#f15c22" style="border-collapse:collapse;border:1px solid #ffffff;padding: 5px 5px 5px 5px; font-size: 16px; color: #FFFFFF; font-weight: bold; font-family: Arial, helvetica, sans-serif;">
                                			Order Info
                                		</td>
                                	</tr>
                                	<tr>
                                		<td align="left" valign="top" style="padding: 5px 25px 5px 10px; font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif;">
                                            @(BillToAddress.FirstName) @(BillToAddress.LastName)<br />
                                            @(BillToAddress.Company)<br />
                                            @(BillToAddress.Address1)<br />
                                            @(BillToAddress.Address2)<br />
                                            @(BillToAddress.City), @(BillToAddress.StateCodeOrCounty) @(BillToAddress.Zip) @(BillToAddress.Country)
                                        </td>
                        				<td align="left" valign="top" style="padding: 5px 25px 5px 10px; font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif;">
                                            @(ShipToAddress.FirstName) @(ShipToAddress.LastName)<br />
                                            @(ShipToAddress.Company)<br />
                                            @(ShipToAddress.Address1)<br />
                                            @(ShipToAddress.Address2)<br />
                                            @(ShipToAddress.City), @(ShipToAddress.StateCodeOrCounty) @(ShipToAddress.Zip) @(ShipToAddress.Country)
                                        </td>
                        				<td align="left" valign="top" style="padding: 5px 0 5px 5px; font-size: 13px; color: #6d6e72; font-family: Arial, helvetica, sans-serif;">
                                            Order#:
                                            <br/> @(Id)
                                            <br/> Purchase Date:
                                            <br/> @date(DateCreated){{MM''/''dd''/''yyyy}}
                                            <br/>
                        				</td>
                        			</tr>
                                    <!-- End Stacking Thirds Inner-->
                                </table>
                                
                                <!-- End Stacking Thirds Outer -->
                                <!-- Full Width Body -->
                                <!-- End Full Width Body -->
                                <!-- Stacking Halves -->
                                <!-- End Stacking Halves -->
                                <!-- Order Info Header -->
                                <table bgcolor="#f15c22" width="100%" border="0" cellspacing="0" cellpadding="0" style="border-collapse: collapse;">
                                
                                    <tr>
                                        <td align="center" valign="top" style="font-family: Helvetica, Arial, sans-serif; color: #ffffff; padding: 10px; font-weight: bold; text-align: center;font-size:16px">Need Further Assistance?</td>
                                    </tr>
                                
                                </table>
                                <!-- End Order Info Header -->
                                <!-- Further Assistance Text -->
                                <br/>
                                <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                    <tr>
                                        <td bgcolor="#edf7f9" style="padding: 0;">
                                
                                            <!--Full-Width WYSIWYG Inner -->
                                            <table align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                                <tr>
                                                    <td align="center" valign="top" bgcolor="#ffffff" style="font-family: Arial, Helvetica, sans-serif; font-size: 13px; color: #000000; padding: 0 1px;">
                                
                                                        <div>
                                                            <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border-collapse: collapse;">
                                
                                                                <tr>
                                                                    <td style="color: #6d6e72; font-family: Helvetica, Arial, sans-serif; font-size: 13px; line-height: 20px; font-weight: 300; padding: 8px;">
                                                                        <li>Replies to this email WILL NOT be received or seen.</li>
                                                                        <li>
                                                                            Please send corrections or questions to info@{@}@vitalchoice.com
                                                                            or call 866-482-5887, Monday through Friday,
                                                                            from 7AM-4PM Pacific Time.
                                                                        </li>
                                
                                
                                                                    </td>
                                                                </tr>
                                
                                
                                
                                                            </table>
                                                        </div>
                                
                                                    </td>
                                                </tr>
                                            </table>
                                            <!-- End Full-Width WYSIWYG Inner-->
                                
                                        </td>
                                    </tr>
                                </table>
                                <br/>
                                <!-- End Further Assistance Text -->
                                <!-- Additional Info Header -->
                                <table bgcolor="#f15c22" width="100%" border="0" cellspacing="0" cellpadding="0" style="border-collapse: collapse;">
                                
                                    <tr>
                                        <td align="center" valign="top" style="font-family: Helvetica, Arial, sans-serif; color: #ffffff; padding: 10px; font-weight: bold; text-align: center;font-size:16px">Additional Information</td>
                                    </tr>
                                
                                </table>
                                <!-- End Additional Info Header -->
                                <!-- Additional Info Text -->
                                <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                    <tr>
                                        <td bgcolor="#edf7f9" style="padding: 0;">
                                
                                            <!--Full-Width WYSIWYG Inner -->
                                            <table align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                                <tr>
                                                    <td align="center" valign="top" bgcolor="#ffffff" style="font-family: Arial, Helvetica, sans-serif; font-size: 14px; color: #000000; padding: 0 1px;">
                                
                                                        <div>
                                                            <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" style="border-collapse: collapse;">
                                
                                                                <tr>
                                                                    <td style="color: #6d6e72; font-family: Helvetica, Arial, sans-serif; font-size: 13px; line-height: 20px; font-weight: 300; padding: 0 8px;">
                                                                        <span style="font-weight: bold; font-size: 16px;"><br />General Shipping Info:</span>
                                                                        <br/>
                                                                        <ul>
                                                                            <li>There are NO Saturday or Sunday deliveries.</li>
                                                                            <li>
                                                                                Gift Certificates ship free; eGift Certificates are
                                                                                delivered free by email.
                                                                            </li>
                                                                            <li>
                                                                                Standard and Premium Shipping Service charges are
                                                                                shown on our Shipping Page.
                                                                            </li>
                                                                            <li>
                                                                                Website Orders placed on Saturday or Sunday will
                                                                                be sent on Tuesday (Monday if upgraded to 2nd
                                                                                Day Air or Overnight service).
                                                                            </li>
                                                                            <li>
                                                                                If you have other delivery date questions or concerns,
                                                                                please call our Customer Service team at 866-482-5887
                                                                                Monday-Friday, 7AM-4PM Pacific Time.
                                                                            </li>
                                                                        </ul>
                                                                        <br/>
                                                                        <u>Frozen Foods</u> (most of our non-canned foods)
                                                                        ship Monday through Wednesday by 1-3 Day Express
                                                                        Ground service (most orders) or 2nd Day Air. Frozen
                                                                        Items ordered on Wednesday after 10 AM Pacific time
                                                                        and upgraded to Overnight Service will ship on Thursday.
                                                                        <br/>
                                                                        <br/>
                                                                        <u>Non-Perishable Goods</u> (such as supplements
                                                                        and canned foods) ship by Ground service Monday through
                                                                        Friday. Non-Perishable Items typically arrive in
                                                                        1-7 days, depending on destination, unless you upgraded
                                                                        to 2nd Day Air or Overnight service.
                                                                        <br/>
                                                                        <br/>
                                                                        <u>Chilled Foods</u> (such as live shellfish) ship
                                                                        Overnight (Tuesday through Thursday only), on cold
                                                                        gel packs.
                                
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="color: #6d6e72; font-family: Helvetica, Arial, sans-serif; font-size: 11px; line-height: 20px; font-weight: 300; padding: 0 8px;">
                                                                        <br/>
                                                                        <a href="#" target="_blank" style="font-size: 13px; text-decoration: underline; color: #f15c22">Answers to Common Shipping Questions ></a>
                                                                        <br/>
                                                                        <br/>
                                                                        <a href="#" target="_blank" style="font-size: 13px; text-decoration: underline; color: #f15c22">Answers to Common Storage &amp; Cooking Questions ></a>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                
                                                    </td>
                                                </tr>
                                            </table>
                                            <!-- End Full-Width WYSIWYG Inner-->
                                
                                        </td>
                                    </tr>
                                </table>
                                <br/>
                                <!-- End Additional Info Text -->
                                <!--Full-Width WYSIWYG Inner -->
                                <table align="center" border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                                    <tr>
                                        <td align="center" valign="top" bgcolor="#ffffff" style="font-family: Arial, Helvetica, sans-serif; font-size: 14px; color: #000000; padding: 0 1px;">
                                
                                            <div>
                                                <a href="https://@(PublicHost)/content/newsletter-sign-up">
                                                    <img src="https://@(PublicHost)/Assets/images/cart-recovery-newslettersignup_v2.jpg"
                                                         width="638" style="display: block; max-width: 100%; height: auto;" border="0"/>
                                                </a>
                                            </div>
                                
                                        </td>
                                    </tr>
                                </table>
                                <!-- End Full-Width WYSIWYG Inner-->
                            </td>
                        </tr>
                    </table>
                    <!-- End Message Body-->
                    <!-- Begin Social Icons -->
                    <div>
                        <br/>
                    </div>
                    <table align="center" border="0" cellpadding="5" cellspacing="0" style="border-collapse: collapse;">
                        <tr>
                            <td colspan="7" align="center"><span style="font-size: 15px; font-weight: bold; color: #6d6e72; font-family: Arial, Helvetica, sans-serif;">FEELING SOCIAL?</span></td>
                        </tr>
                        <tr>
                            <td align="center" valign="middle"><a href="https://www.facebook.com/vitalchoice" target="blank"><img src="https://@(PublicHost)/Assets/images/VC-Facebook.png" height="40" width="40" style="border:none; display:block;" alt="Facebook" /></a></td>
                            <td align="center" valign="middle"><a href="#" target="blank"><img src="https://@(PublicHost)/Assets/images/VC-Blog.png" height="40" width="40" style="border:none; display:block;" alt="Blogt" /></a></td>
                            <td align="center" valign="middle"><a href="https://twitter.com/vitalchoice" target="blank"><img src="https://@(PublicHost)/Assets/images/VC-Twitter.png" height="40" width="40" style="border:none; display:block;" alt="Twitter" /></a></td>
                            <td align="center" valign="middle"><a href="https://plus.google.com/109701577970251794205/posts" target="blank"><img src="https://@(PublicHost)/Assets/images/VC-Google.png" height="40" width="40" style="border:none; display:block;" alt="Google Plus" /></a></td>
                            <td align="center" valign="middle"><a href="https://www.pinterest.com/vitalchoice/" target="blank"><img src="https://@(PublicHost)/Assets/images/VC-Pinterest.png" height="40" width="40" style="border:none; display:block;" alt="Pinterest" /></a></td>
                            <td align="center" valign="middle"><a href="https://instagram.com/vitalchoice/" target="blank"><img src="https://@(PublicHost)/Assets/images/social-icons-Insta.png" height="40" width="40" style="border:none; display:block;" alt="Instagram" /></a></td>
                            <td align="center" valign="middle"><a href="https://www.bcorporation.net/community/vital-choice" target="blank"><img src="https://@(PublicHost)/Assets/images/VC-BCorp.png" height="40" width="40" style="border:none; display:block;" alt="B Corp" /></a></td>
                        </tr>
                    </table>
                    <!-- End Social Icons -->
                    <!-- Footer Outer -->
                    <table border="0" align="center" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse;">
                        <tr>
                            <td align="center">
                    
                                <!--Footer Inner -->
                                <table align="center" border="0" cellpadding="0" cellspacing="0" style="border-collapse: collapse;">
                                    <tr>
                                        <td align="center" valign="top" style="padding: 10px; color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 10px; line-height: 18px;">
                                            <span style="font-size: 12px; font-weight: bold; color: #6d6e72;">These items were included in your Order Number: @(Id)<br />Please note that changes made after submitting your order may not be reflected in this confirmation.</span>
                                            <br/>
                                            <br/>
                    
                                        </td>
                                    </tr>
                                </table>
                                <!-- End Footer Inner-->
                    
                            </td>
                        </tr>
                    </table>
                    <!-- End Footer Outer -->
                    <img src="https://@(PublicHost)/Assets/images/spacer.gif" style="display: block;" height="1" width="1"/>
                </td>
            </tr>
        </table>
        <!-- End Outer Container -->
    </div>
    @ifnot(IsPerishable){{
    <span style="line-height:0px;font-size:0px;height:0px;margin:0;padding:0;">PP_delay:8</span><br/>
    }}
    <span style="line-height:0px;font-size:0px;height:0px;margin:0;padding:0;">@(Email)</span>
</body>

</html>
}} :: VitalChoice.Ecommerce.Domain.Mail.OrderShippingConfirmationEmail
%>'
           ,''
           ,'Vital Choice - Order Shipping Confirmation Email'
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
           ('OrderShippingConfirmationEmail'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='StoreFront Email Template')
           ,2
           ,NULL
           ,'VitalChoice.Ecommerce.Domain.Mail.OrderShippingConfirmationEmail'
           ,'Order Shipping Confirmation Email')

END

GO