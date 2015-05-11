IF(NOT EXISTS (SELECT [RoleId] FROM [dbo].[AspNetRoleClaims]))
BEGIN
	INSERT INTO [dbo].[AspNetRoleClaims]
	([ClaimType], [ClaimValue], [RoleId])
	VALUES
	(N'Permission', 1, 1),
	(N'Permission', 2, 1),
	(N'Permission', 3, 1),
	(N'Permission', 4, 1),
	(N'Permission', 5, 1),
	(N'Permission', 6, 1),
	(N'Permission', 8, 1),
	(N'Permission', 9, 1),
	(N'Permission', 10, 1),
	(N'Permission', 11, 1),

	(N'Permission', 6, 2),
	(N'Permission', 11, 2),

	(N'Permission', 1, 3),
	(N'Permission', 2, 3),
	(N'Permission', 11, 3),

	(N'Permission', 4, 4),
	(N'Permission', 11, 4)
END

UPDATE [dbo].[AspNetRoles]
SET [NormalizedName] = N'Admin User'
WHERE [NormalizedName] = N'AdminUser'

GO

UPDATE [dbo].[AspNetRoles]
SET [NormalizedName] = N'Content User'
WHERE [NormalizedName] = N'ContentUser'

GO

UPDATE [dbo].[AspNetRoles]
SET [NormalizedName] = N'Order User'
WHERE [NormalizedName] = N'OrderUser'

GO

UPDATE [dbo].[AspNetRoles]
SET [NormalizedName] = N'Product User'
WHERE [NormalizedName] = N'ProductUser'

GO

UPDATE [dbo].[AspNetRoles]
SET [NormalizedName] = N'Super Admin User'
WHERE [NormalizedName] = N'SuperAdminUser'

GO

IF(NOT EXISTS (SELECT [Email] FROM [dbo].[AspNetUsers] WHERE [Email] = N'noreplyvitalchoice@gmail.com'))
BEGIN
	SET IDENTITY_INSERT [dbo].[AspNetUsers] ON

	INSERT [dbo].[AspNetUsers] ([Id], [PublicId], [AccessFailedCount], [ConcurrencyStamp], [Email], [EmailConfirmed], [UserName], [FirstName], [LastName], [Status], [LockoutEnabled], [LockoutEnd], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [LastLoginDate], [CreateDate], [UpdatedDate], [DeletedDate], [SecurityStamp], [TwoFactorEnabled]) VALUES (1012, N'67af9366-76b7-4128-a6a6-5e41bff57ade', 0, N'755be2cd-b77b-4500-9bc0-6cc2e6e55c88', N'noreplyvitalchoice@gmail.com', 0, N'noreplyvitalchoice@gmail.com', N'VC', N'Admin', 1, 1, NULL, N'NOREPLYVITALCHOICE@GMAIL.COM', N'NOREPLYVITALCHOICE@GMAIL.COM', N'AQAAAAEAACcQAAAAEEnZPPKH+gddd5qbtLM8XMRzjqXKVKnobOHoEb0YnONajINxkKmjzhOErfAajwnGQw==', NULL, 0, NULL, CAST(0x07BE3BC97707E4390B AS DateTime2), CAST(0x07F1881DE307E4390B AS DateTime2), NULL, N'339b920f-8ddc-4fd5-976a-aa33e098ae43', 0)

	SET IDENTITY_INSERT [dbo].[AspNetUsers] OFF

	INSERT [dbo].[AdminProfiles] ([Id], [AgentId], [ConfirmationToken], [TokenExpirationDate], [IsConfirmed]) VALUES (1012, N'vc', N'c6f09915-d95d-4513-beb1-f6202a1358cf', CAST(0x0781C6C87707E7390B AS DateTime2), 1)

	INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (1012, 5)
END

GO

UPDATE [dbo].[AspNetUsers] 
SET [EmailConfirmed] = 1
WHERE [Email] = N'noreplyvitalchoice@gmail.com'

GO