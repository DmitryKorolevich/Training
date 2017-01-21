IF OBJECT_ID(N'[dbo].[OrderReviewRules]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[OrderReviewRules]
	(
		[Id] INT NOT NULL IDENTITY(1,1),
		[DateCreated] DATETIME2 NOT NULL,
		[DateEdited] DATETIME2 NOT NULL,
		[IdAddedBy] INT NOT NULL,
		[IdEditedBy] INT NOT NULL,
		[StatusCode] INT NOT NULL,
		[Name] NVARCHAR(250) NOT NULL,
		[ApplyType] INT NOT NULL,
		CONSTRAINT PK_OrderReviewRules PRIMARY KEY CLUSTERED (Id ASC)
	);

	ALTER TABLE [dbo].[OrderReviewRules]  WITH CHECK ADD  CONSTRAINT [FK_OrderReviewRulesToAddUser] FOREIGN KEY([IdAddedBy])
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[OrderReviewRules] CHECK CONSTRAINT [FK_OrderReviewRulesToAddUser]

	ALTER TABLE [dbo].[OrderReviewRules]  WITH CHECK ADD  CONSTRAINT [FK_OrderReviewRulesToEditUser] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[Users] ([Id])

	ALTER TABLE [dbo].[OrderReviewRules] CHECK CONSTRAINT [FK_OrderReviewRulesToEditUser]


	CREATE TABLE [dbo].[OrderReviewRuleOptionTypes](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](250) NOT NULL,
		[IdLookup] [int] NULL,
		[IdFieldType] [int] NOT NULL,
		[IdObjectType] [int] NULL,
		[DefaultValue] [nvarchar](250) NULL,
	 CONSTRAINT [PK_OrderReviewRuleOptionTypes] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[OrderReviewRuleOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_OrderReviewRuleOptionTypesToFieldType] FOREIGN KEY([IdFieldType])
	REFERENCES [dbo].[FieldTypes] ([Id])

	ALTER TABLE [dbo].[OrderReviewRuleOptionTypes] CHECK CONSTRAINT [FK_OrderReviewRuleOptionTypesToFieldType]

	ALTER TABLE [dbo].[OrderReviewRuleOptionTypes]  WITH CHECK ADD  CONSTRAINT [FK_OrderReviewRuleOptionTypesToLookup] FOREIGN KEY([IdLookup])
	REFERENCES [dbo].[Lookups] ([Id])

	ALTER TABLE [dbo].[OrderReviewRuleOptionTypes] CHECK CONSTRAINT [FK_OrderReviewRuleOptionTypesToLookup]

		
	CREATE TABLE [dbo].[OrderReviewRuleOptionValues](
		[IdOptionType] [int] NOT NULL,
		[IdOrderReviewRule] [int] NOT NULL,
		[Value] [nvarchar](250) NULL,
	 CONSTRAINT [PK_OrderReviewRuleOptionValues] PRIMARY KEY CLUSTERED 
	(
		[IdOrderReviewRule] ASC,
		[IdOptionType] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]


	ALTER TABLE [dbo].[OrderReviewRuleOptionValues] WITH CHECK ADD  CONSTRAINT [FK_OrderReviewRuleOptionValues] FOREIGN KEY([IdOrderReviewRule])
	REFERENCES [dbo].[OrderReviewRules] ([Id])

	ALTER TABLE [dbo].[OrderReviewRuleOptionValues] CHECK CONSTRAINT [FK_OrderReviewRuleOptionValues]

	ALTER TABLE [dbo].[OrderReviewRuleOptionValues]  WITH CHECK ADD  CONSTRAINT [FK_OrderReviewRuleOptionValuesToOrderReviewRuleOptionTypes] FOREIGN KEY([IdOptionType])
	REFERENCES [dbo].[OrderReviewRuleOptionTypes] ([Id])

	ALTER TABLE [dbo].[OrderReviewRuleOptionValues] CHECK CONSTRAINT [FK_OrderReviewRuleOptionValuesToOrderReviewRuleOptionTypes]

END

GO