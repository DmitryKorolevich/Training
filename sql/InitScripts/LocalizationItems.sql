USE [VitalChoice]
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 1, N'ValidationMessages', N'FieldRequired', N'Field is required.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 2, N'ValidationMessages', N'FieldLength', N'Field max length.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 3, N'ValidationMessages', N'FieldMin', N'Field characteristic less then expected.')
GO
INSERT [dbo].[LocalizationItem] ([GroupId], [ItemId], [GroupName], [ItemName], [Comment]) VALUES (1, 4, N'ValidationMessages', N'FieldMax', N'Field characteristic more then expected.')
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
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 1, N'en', N'{0} is required.')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 2, N'en', N'{0} exceeds length of {1} literals.')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 3, N'en', N'{0} less than {1}.')
GO
INSERT [dbo].[LocalizationItemData] ([GroupId], [ItemId], [CultureId], [Value]) VALUES (1, 4, N'en', N'{0} more than {1}')
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
