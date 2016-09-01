GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'HelpTicketUpdateCustomerServiceNotification')
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
	<p>Help ticket was updated - <a href="@(Url)">@(Url)</a></p>
	<br/><br/><br/>
	Sincerely,<br/>
	Vital Choice Wild Seafood & Organics
	<br/><br/><br/><br/><br/>
	<i>
	*Please note. This is an automated message. Do not reply. This mailbox is not monitored.
	</i>
}}
%>'
           ,''
           ,'Help desk #@(Id) has been updated - Vital Choice Wild Seafood & Organics'
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
           ('HelpTicketUpdateCustomerServiceNotification'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'HelpTicketEmail'
           ,'Help ticket add/update customer service notification')

END

GO