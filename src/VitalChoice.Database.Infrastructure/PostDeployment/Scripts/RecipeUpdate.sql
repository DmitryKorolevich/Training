IF OBJECT_ID(N'[dbo].[RecipeCrossSells]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[RecipeCrossSells](
		[Id] [int] IDENTITY (1, 1) NOT NULL,
		[IdRecipe] [int] NOT NULL,
		[Image] [nvarchar](250) NOT NULL,
		[Title] [nvarchar](250) NOT NULL,
		[Subtitle] [nvarchar](250) NOT NULL,
		[Url] [nvarchar](250) NOT NULL,
	 CONSTRAINT [PK_RecipeCrossSells] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	)

	ALTER TABLE [dbo].[RecipeCrossSells]  WITH CHECK ADD  CONSTRAINT [FK_RecipeCrossSells_Recipes] FOREIGN KEY([IdRecipe])
	REFERENCES [dbo].[Recipes] ([Id])
END

IF OBJECT_ID(N'[dbo].[RelatedRecipes]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[RelatedRecipes](
		[Id] [int] IDENTITY (1, 1) NOT NULL,
		[IdRecipe] [int] NOT NULL,
		[Image] [nvarchar](250) NOT NULL,
		[Title] [nvarchar](250) NOT NULL,
		[Url] [nvarchar](250) NOT NULL,
	 CONSTRAINT [PK_RelatedRecipes] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	)

	ALTER TABLE [dbo].[RelatedRecipes]  WITH CHECK ADD  CONSTRAINT [FK_RelatedRecipes_Recipes] FOREIGN KEY([IdRecipe])
	REFERENCES [dbo].[Recipes] ([Id])
END

IF OBJECT_ID(N'[dbo].[RecipeVideos]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[RecipeVideos](
		[Id] [int] IDENTITY (1, 1) NOT NULL,
		[IdRecipe] [int] NOT NULL,
		[Image] [nvarchar](250) NOT NULL,
		[Video] [nvarchar](250) NOT NULL,
		[Text] [nvarchar](250) NOT NULL,
	 CONSTRAINT [PK_RecipeVideos] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	)

	ALTER TABLE [dbo].[RecipeVideos]  WITH CHECK ADD  CONSTRAINT [FK_RecipeVideos_Recipes] FOREIGN KEY([IdRecipe])
	REFERENCES [dbo].[Recipes] ([Id])
END

IF COL_LENGTH('[dbo].[Recipes]','AboutChef') IS NULL
BEGIN
	ALTER TABLE [dbo].[Recipes]
	ADD [AboutChef] NVARCHAR(MAX) NULL

	ALTER TABLE [dbo].[Recipes]
	ADD [Ingredients] NVARCHAR(MAX) NULL

	ALTER TABLE [dbo].[Recipes]
	ADD [Directions] NVARCHAR(MAX) NULL
END

IF OBJECT_ID(N'[dbo].[RecipeDefaultSettings]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[RecipeDefaultSettings](
		[Id] [int] IDENTITY (1, 1) NOT NULL,
		[Key] [nvarchar](250) NOT NULL,
		[Value] [nvarchar](250) NOT NULL,
	 CONSTRAINT [PK_RecipeDefaultSettings] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	)
END

IF NOT EXISTS (SELECT [Id] FROM [dbo].[RecipeDefaultSettings])
BEGIN
	INSERT INTO [dbo].[RecipeDefaultSettings]
	([Value],[Key])
	SELECT '/some1.png', N'CrossSellImage1'
	UNION
	SELECT '/some2.png', N'CrossSellImage2'
	UNION
	SELECT '/some3.png', N'CrossSellImage3'
	UNION
	SELECT '/some4.png', N'CrossSellImage4'
	UNION
	SELECT 'Default Title 1', N'CrossSellTitle1'
	UNION
	SELECT 'Default Title 2', N'CrossSellTitle2'
	UNION
	SELECT 'Default Title 3', N'CrossSellTitle3'
	UNION
	SELECT 'Default Title 4', N'CrossSellTitle4'
	UNION
	SELECT 'Default Subtitle 1', N'CrossSellSubtitle1'
	UNION
	SELECT 'Default Subtitle 2', N'CrossSellSubtitle2'
	UNION
	SELECT 'Default Subtitle 3', N'CrossSellSubtitle3'
	UNION
	SELECT 'Default Subtitle 4', N'CrossSellSubtitle4'
	UNION
	SELECT 'http://someurl.com/1', N'CrossSellUrl1'
	UNION
	SELECT 'http://someurl.com/2', N'CrossSellUrl2'
	UNION
	SELECT 'http://someurl.com/3', N'CrossSellUrl3'
	UNION
	SELECT 'http://someurl.com/4', N'CrossSellUrl4'
	UNION
	SELECT '/some1.png', N'RelatedRecipeImage1'
	UNION
	SELECT '/some2.png', N'RelatedRecipeImage2'
	UNION
	SELECT '/some3.png', N'RelatedRecipeImage3'
	UNION
	SELECT '/some4.png', N'RelatedRecipeImage4'
	UNION
	SELECT 'Default Title 1', N'RelatedRecipeTitle1'
	UNION
	SELECT 'Default Title 2', N'RelatedRecipeTitle2'
	UNION
	SELECT 'Default Title 3', N'RelatedRecipeTitle3'
	UNION
	SELECT 'Default Title 4', N'RelatedRecipeTitle4'
	UNION
	SELECT 'http://someurl.com/1', N'RelatedRecipeUrl1'
	UNION
	SELECT 'http://someurl.com/2', N'RelatedRecipeUrl2'
	UNION
	SELECT 'http://someurl.com/3', N'RelatedRecipeUrl3'
	UNION
	SELECT 'http://someurl.com/4', N'RelatedRecipeUrl4'
	UNION
	SELECT '/some1.png', N'YouTubeImage1'
	UNION	
	SELECT '/some2.png', N'YouTubeImage2'
	UNION
	SELECT '/some3.png', N'YouTubeImage3'
	UNION
	SELECT '/some1.png', N'YouTubeText1'
	UNION	
	SELECT '/some2.png', N'YouTubeText2'
	UNION
	SELECT '/some3.png', N'YouTubeText3'
	UNION
	SELECT 'jGwOsFo8TTg', N'YouTubeVideo1'
	UNION	
	SELECT 'btlfoO75kfI', N'YouTubeVideo2'
	UNION
	SELECT 'vCsRTamxWuw', N'YouTubeVideo3'
END

IF COL_LENGTH('[dbo].[RecipeCrossSells]','Number') IS NULL
BEGIN
	DELETE FROM [dbo].[RecipeCrossSells]
	DELETE FROM [dbo].[RecipeVideos]
	DELETE FROM [dbo].[RelatedRecipes]

	ALTER TABLE [dbo].[RecipeCrossSells]
	ADD [Number] [TINYINT] NOT NULL

	ALTER TABLE [dbo].[RelatedRecipes]
	ADD [Number] [TINYINT] NOT NULL

	ALTER TABLE [dbo].[RecipeVideos]
	ADD [Number] [TINYINT] NOT NULL

	ALTER TABLE [dbo].[RecipeCrossSells]
	ADD CONSTRAINT
	UQ_RecipeCrossSells_Number UNIQUE NONCLUSTERED
	(
		IdRecipe,
		Number
	)

	ALTER TABLE [dbo].[RelatedRecipes]
	ADD CONSTRAINT
	UQ_RelatedRecipes_Number UNIQUE NONCLUSTERED
	(
		IdRecipe,
		Number
	)

	ALTER TABLE [dbo].[RecipeVideos]
	ADD CONSTRAINT
	UQ_RecipeVideos_Number UNIQUE NONCLUSTERED
	(
		IdRecipe,
		Number
	)

END