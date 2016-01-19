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