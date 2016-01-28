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