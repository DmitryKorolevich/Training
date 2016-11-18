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
           ,'VitalChoice.Ecommerce.Domain.Mail.HelpTicketEmail'
           ,'Help ticket add/update customer service notification')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'GiftAdminNotificationEmail')
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
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>Vital Choice E-Gift Certificate</title>
</head>
<body>
    <div style="font-size:14px;">
        <div align="left">
            <div style="margin:20px 0px 20px 0px;">
                Hi <strong>@(Recipient)</strong>, you''ve been given an e-gift certificate from Vital Choice! <br /><br />
                Lucky you! You can use it toward any of the healthy and delicious products found at <a href="https://@(PublicHost)/">@(PublicHost)</a> or in the Vital Choice catalog.<br /><br />
                <em>@(Message)</em>
            </div>
        </div>
        @list(Gifts){{
        <div align="center">
            <table width="850" border="0" cellspacing="0" cellpadding="0" style="font-family: ''Times New Roman'';">
              <tr>
                <td height="379" valign="top" style="background:url(https://@(@root.Model.PublicHost)/assets/images/egift/ecertificate.jpg);background-repeat:no-repeat;min-height:379px;">
                    <table width="850" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                            <td width="504" valign="top" style="min-height:379px;">
                                <div style="margin-top:220px;margin-left:100px;">
                                    <table width="400" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td height="30" style="font-size:13px;font-style:italic;font-weight:bold;">
                                                <div align="left">From: Vital Choice</div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="font-size:13px;font-style:italic;font-weight:bold;"><div align="left">To: @(@root.Model.Recipient)</div></td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                            <td width="346" valign="top">
                                <div style="margin-top:115px;margin-left:30px;">
                                    <table width="200" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td width="150" style="font-size:48px;font-weight:bold;font-style:italic;">
                                                <div align="left">@money(Amount)
                                                    <div style="vertical-align:text-top;">
                                                        <div style="font-size:20px;font-weight:bold;font-style:italic;">@(Code)</div>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
              </tr>
            </table>
            @if(ShowDots){{
            <div align="center">
                <div style="margin:30px 0px 30px 0px;">
                    <img src="https://@(@root.Model.PublicHost)/assets/images/egift/dots.jpg" />
                </div>
            </div>
            }}
        </div>
        }}
        </div>
        <div align="center">
            <div style="margin:20px 0px 20px 0px;font-size:13px;">
                Vital Choice is a trusted source for fast home delivery of the world''s finest wild Alaskan seafood and organic fare. 
                Our foods are among the purest available, and are sustainably harvested from healthy, well-managed wild fisheries and organic farms. 
                They are recognized for their superior taste and health benefits, and are endorsed by leading health and wellness experts.
                <br />
                <br /> 
                Redeem your e-gift certificate online at <a href="https://@(PublicHost)/">@(PublicHost)</a> or by phone at 1-800-608-4825.
            </div>
        </div>
    </div>
</body>
</html>
}} :: VitalChoice.Ecommerce.Domain.Mail.GiftAdminNotificationEmail
%>'
           ,''
           ,'A Gift Certificate for you!'
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
           ('GiftAdminNotificationEmail'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='StoreFront Email Template')
           ,2
           ,NULL
           ,'VitalChoice.Ecommerce.Domain.Mail.GiftAdminNotificationEmail'
           ,'Gift Admin Notification Email')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'HelpTicketAddCustomerNotification')
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
		Thanks for contacting Vital Choice.<br/><br/>
		This is just a quick note to let you know we received your message, and Customer Service will be responding to you soon.
	</p><br/>
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
           ,'Support Ticket Created - Vital Choice Wild Seafood & Organics'
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
           ('HelpTicketAddCustomerNotification'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='Admin Email Template')
           ,2
           ,NULL
           ,'VitalChoice.Ecommerce.Domain.Mail.HelpTicketEmail'
           ,'Help ticket add customer notification')

END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[EmailTemplates] WHERE [Name] = 'GiftExpirationDateAdminNotificationEmail')
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
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>Vital Choice E-Gift Certificate</title>
</head>
<body>
    <div style="font-size:14px;">
        <div align="left">
            <div style="margin:20px 0px 20px 0px;">
                Hi <strong>@(Recipient)</strong>, you''ve been given an e-gift certificate from Vital Choice! <br /><br />
                Lucky you! You can use it toward any of the healthy and delicious products found at <a href="https://@(PublicHost)/">@(PublicHost)</a> or in the Vital Choice catalog.<br /><br />
                @if(Message)
                {{
                <em>@(Message)</em>
                }}
            </div>
        </div>
        @list(Gifts){{
        <div align="center">
            <table width="1000" border="0" cellspacing="0" cellpadding="0" style="font-family: ''Times New Roman'';">
              <tr>
                <td height="442" valign="top" style="background:url(https://@(@root.Model.PublicHost)/assets/images/egift/vital-dollar-blank.jpeg);background-repeat:no-repeat;min-height:379px;">
                    <table width="1000" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                            <td width="670" valign="top" style="min-height:442px">
                                <div style="margin-top:315px;margin-left:115px;color: #eb451b;">
                                    <table width="400" border="0" cellspacing="0" cellpadding="0">
                                        <tbody>
                                            <tr>
                                                <td height="30" style="font-size: 30px;font-style:italic;font-family: Lucida Grande;">
                                                    <div align="center">@(Code)</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-size: 16px;font-style: italic;font-family: Lucida Grande;">
                                                    <div align="center">Exp: @date(ExpirationDate) {{MM''/''dd''/''yy}}</div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </td>
                            <td width="330" valign="top">
                                <div style="margin-top:155px;margin-left:30px">
                                    <table width="200" border="0" cellspacing="0" cellpadding="0" style="color: white;">
                                        <tbody><tr>
                                            <td width="60" style="font-size: 120px;vertical-align: top;padding-top: 15px;font-family: Lucida Grande;">
                                                <div align="left">$
                                                </div>
                                            </td>
                                            <td width="180" style="font-size: 180px;vertical-align: top;font-family: Lucida Grande;">
                                                <div align="left">@(@model.Amount.ToString("##.##"))
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody></table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
              </tr>
            </table>
            @if(ShowDots){{
            <div align="center">
                <div style="margin:30px 0px 30px 0px;">
                    <img src="https://@(@root.Model.PublicHost)/assets/images/egift/dots.jpg" />
                </div>
            </div>
            }}
        </div>
        }}
        </div>
        <div align="center">
            <div style="margin:20px 0px 20px 0px;font-size:13px;">
                Vital Choice is a trusted source for fast home delivery of the world''s finest wild Alaskan seafood and organic fare. 
                Our foods are among the purest available, and are sustainably harvested from healthy, well-managed wild fisheries and organic farms. 
                They are recognized for their superior taste and health benefits, and are endorsed by leading health and wellness experts.
                <br />
                <br /> 
                Redeem your e-gift certificate online at <a href="https://@(PublicHost)/">@(PublicHost)</a> or by phone at 1-800-608-4825.
            </div>
        </div>
    </div>
</body>
</html>
}} :: VitalChoice.Ecommerce.Domain.Mail.GiftAdminNotificationEmail
%>'
           ,''
           ,'A Gift Certificate for you!'
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
           ('GiftExpirationDateAdminNotificationEmail'
           ,@contentItemId
           ,(SELECT Id FROM MasterContentItems WHERE Name='StoreFront Email Template')
           ,2
           ,NULL
           ,'VitalChoice.Ecommerce.Domain.Mail.GiftAdminNotificationEmail'
           ,'Gift With Expiration Date Admin Notification Email')

END

GO