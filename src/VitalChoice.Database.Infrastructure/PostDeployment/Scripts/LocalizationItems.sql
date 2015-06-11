GO
TRUNCATE TABLE LocalizationItemData 

GO

DELETE LocalizationItem 

GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 1, N'ValidationMessages', N'FieldRequired', N'Field is required.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 2, N'ValidationMessages', N'FieldLength', N'Field max length.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 3, N'ValidationMessages', N'FieldMin', N'Field characteristic less then expected.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 4, N'ValidationMessages', N'FieldMax', N'Field characteristic more then expected.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 5, N'ValidationMessages', N'FieldContentUrlInvalidFormat', N'Field invalid url format in the content area.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 6, N'ValidationMessages', N'FieldNumber', N'Field must be number.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 7, N'ValidationMessages', N'EmailFormat', N'Incorrect email format.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 8, N'ValidationMessages', N'AtLeastOneRole', N'At least one role should be assigned.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 9, N'ValidationMessages', N'UserStatusRestriction', N'User status can be updated to Active or Disabled only.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 10, N'ValidationMessages', N'PasswordMustMatch', N'Password and password confirmation should match.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 11, N'ValidationMessages', N'FieldNameInvalidFormat', N'Password and password confirmation should match.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 12, N'ValidationMessages', N'FieldMinOrEqual', N'Field characteristic less or equal then expected.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 14, N'ValidationMessages', N'FieldMaxOrEqual', N'Field characteristic more or equal then expected.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 15, N'ValidationMessages', N'Exist', N'Exist in colelction.')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 1, N'en', N'{0} is required.')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 2, N'en', N'{0} exceeds length of {1} literals.')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 3, N'en', N'{0} less than {1}.')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 4, N'en', N'{0} more than {1}')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 5, N'en', N'{0} has invalid format(e.g. some-page-name).')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 6, N'en', N'{0} should be a whole number.')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 7, N'en', N'{0} has incorrect email format.')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 8, N'en', N'Should have at least one role assigned.')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 9, N'en', N'{0} can be changed to Active or Disabled only.')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 10, N'en', N'{0} must match password confirmation')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 11, N'en', N'{0} has invalid format(e.g. some-folder-name).')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 12, N'en', N'{0} less or equal than {1}.')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 14, N'en', N'{0} more or equal than {1}')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 15, N'en', N'Item with this {0} already exists')
GO

INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (2, 1, N'BaseButtonLabels', N'Save', N'')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (2, 2, N'BaseButtonLabels', N'Cancel', N'')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (2, 3, N'BaseButtonLabels', N'Yes', N'')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (2, 4, N'BaseButtonLabels', N'No', N'')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (2, 5, N'BaseButtonLabels', N'Go', N'')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (2, 6, N'BaseButtonLabels', N'Search', N'')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (2, 1, N'en', N'Save')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (2, 2, N'en', N'Cancel')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (2, 3, N'en', N'Yes')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (2, 4, N'en', N'No')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (2, 5, N'en', N'Go')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (2, 6, N'en', N'Search')
GO



INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 1, N'GeneralFieldNames', N'Name', N'Name field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 2, N'GeneralFieldNames', N'Template', N'Template field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 3, N'GeneralFieldNames', N'Url', N'Url field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 4, N'GeneralFieldNames', N'Title', N'Title field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 5, N'GeneralFieldNames', N'Description', N'Description field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 6, N'GeneralFieldNames', N'Date', N'Date field name')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 1, N'en', N'Name')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 2, N'en', N'Template')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 3, N'en', N'URL')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 4, N'en', N'Title')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 5, N'en', N'Description')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 6, N'en', N'Date')
GO


INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 7, N'GeneralFieldNames', N'CountryName', N'Country Name field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 8, N'GeneralFieldNames', N'CountryCode', N'Country Code field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 9, N'GeneralFieldNames', N'StateName', N'State / Province Name field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 10, N'GeneralFieldNames', N'StateCode', N'State / Province Code field name')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 7, N'en', N'Country Name')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 8, N'en', N'Country Code')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 9, N'en', N'State / Province Name')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 10, N'en', N'State / Province Code')
GO

INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 11, N'GeneralFieldNames', N'Email', N'Email field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 12, N'GeneralFieldNames', N'Password', N'Password field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 13, N'GeneralFieldNames', N'FirstName', N'FirstName field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 14, N'GeneralFieldNames', N'LastName', N'LastName field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 15, N'GeneralFieldNames', N'ConfirmPassword', N'ConfirmPassword field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 16, N'GeneralFieldNames', N'Roles', N'Roles field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 17, N'GeneralFieldNames', N'UserStatus', N'Status field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 18, N'GeneralFieldNames', N'AgentId', N'AgentId field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 19, N'GeneralFieldNames', N'OldPassword', N'OldPassword field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 20, N'GeneralFieldNames', N'NewPassword', N'NewPassword field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 21, N'GeneralFieldNames', N'ConfirmNewPassword', N'ConfirmNewPassword field name')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 11, N'en', N'Email')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 12, N'en', N'Password')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 13, N'en', N'First Name')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 14, N'en', N'Last Name')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 15, N'en', N'Password Confirmation')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 16, N'en', N'Roles')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 17, N'en', N'User Status')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 18, N'en', N'Agent ID')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 19, N'en', N'Old Password')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 20, N'en', N'New Password')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 21, N'en', N'Confirm New Password')
GO

INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 50, N'GeneralFieldNames', N'Quantity', N'Quantity field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 51, N'GeneralFieldNames', N'Amount', N'Amount field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 52, N'GeneralFieldNames', N'Message', N'Message field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 54, N'GeneralFieldNames', N'Message', N'SKU field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 55, N'GeneralFieldNames', N'Message', N'Retail Price field name')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (3, 56, N'GeneralFieldNames', N'Message', N'Wholesale Price field name')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 50, N'en', N'Quantity')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 51, N'en', N'Amount')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 52, N'en', N'Message')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 54, N'en', N'SKU')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 55, N'en', N'Retail Price')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (3, 56, N'en', N'Wholesale Price')
GO