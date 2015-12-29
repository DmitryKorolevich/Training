IF((SELECT count(*) FROM ContentProcessors
WHERE Id IN (1,2,3))!=3)
BEGIN

INSERT INTO ContentProcessors
(Id, Type, Name, Description)
SELECT 1, 'RecipeRootCategoryProcessor', 'Recipe root category processor', NULL

INSERT INTO ContentProcessors
(Id, Type, Name, Description)
SELECT 2, 'RecipeSubCategoriesProcessor', 'Recipe sub-categories processor', NULL

INSERT INTO ContentProcessors
(Id, Type, Name, Description)
SELECT 3, 'RecipesProcessor', 'Recipes processor', NULL

END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentProcessors] WHERE [Type] = N'ProductCategoryProcessor')
BEGIN
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(4, N'ProductCategoryProcessor', N'Product category processor', N'Processor to manage product categories')

	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 4 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Product sub categories'
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentProcessors] WHERE [Type] = N'ArticleCategoriesProcessor')
BEGIN
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(7, N'ArticleCategoriesProcessor', N'Article categories processor', N'Tree view of article categories')
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(8, N'ArticlesProcessor', N'Articles processor', N'Articles with paging by category id')

	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 7 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Article Sub Category'
	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 8 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Article Sub Category'
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentProcessors] WHERE [Type] = N'RecentArticlesProcessor')
BEGIN
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(9, N'RecentArticlesProcessor', N'Recent articles processor', N'5 recent articles by date')
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(10, N'RecentRecipesProcessor', N'Recent recipes processor', N'1 recent recips by date')
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(11, N'ArticleCategoriesForArticleProcessor', N'Article categories processor for article', N'Tree view of article categories')

	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 9 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Article Individual'
	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 10 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Article Individual'
	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 11 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Article Individual'
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentProcessors] WHERE [Type] = N'RecipeCategoriesProcessor')
BEGIN

	DELETE [dbo].[MasterContentItemsToContentProcessors]
	WHERE ContentProcessorId IN (1,2,3)
	
	DELETE [dbo].[ContentProcessors]
	WHERE Id IN (1,2,3)

	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(12, N'RecipeCategoriesProcessor', N'Recipe categories processor', N'Tree view of recipe categories, chef recipe categories with recipes')
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(14, N'RecipesProcessor', N'Recipes processor', N'Recipes list by the given category id')

	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 12 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Recipe Sub Category'
	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 14 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Recipe Sub Category'
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentProcessors] WHERE [Type] = N'RecipeCategoriesForRecipeProcessor')
BEGIN

	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(15, N'RecipeCategoriesForRecipeProcessor', N'Recipe categories processor for recipe', N'Tree view of recipe categories, chef recipe categories with recipes')

	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 15 FROM [dbo].[MasterContentItems] WHERE [Name] = N'Recipe Individual'
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentProcessors] WHERE [Type] = N'FAQCategoriesProcessor')
BEGIN

	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(17, N'FAQCategoriesProcessor', N'FAQ categories processor', N'Tree view of FAQ categories')
	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(18, N'FAQsProcessor', N'FAQs processor', N'FAQs list by the given category id')

	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 17 FROM [dbo].[MasterContentItems] WHERE [Name] = N'FAQ Sub Category'
	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	SELECT [Id], 18 FROM [dbo].[MasterContentItems] WHERE [Name] = N'FAQ Sub Category'
END

GO

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentProcessors] WHERE [Type] = N'RecipeCategoriesForContentPageProcessor')
BEGIN

	DECLARE @id int

	INSERT INTO [dbo].[ContentProcessors]
	(Id, [Type], Name, Description)
	VALUES
	(19, N'RecipeCategoriesForContentPageProcessor', N'Recipe categories processor for content page', N'Tree view of recipe categories, chef recipe categories with recipes')

	INSERT MasterContentItems
	       ([Name]
           ,[TypeId]
           ,[Template]
           ,[Created]
           ,[Updated]
           ,[StatusCode]
           ,[UserId])
     VALUES
	 ('Content Individual with Recipe Categories',
	 8,
	 N'@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Recipes}}
@model() {{dynamic}}

<%
    
<category>
{{
    @if(@model.SubCategories.Count>0)
    {{
        <ul class="drop-menu sub-menu collapsed">
        @list(@model.SubCategories)
        {{
            <li>
                @if(@model.SubCategories.Count>0)
                {{
                <a class="trigger" href="#">@(Name)</a>
                }}
                @if(@model.SubCategories.Count==0)
                {{
                <a href="@(Url)">@(Name)</a>
                }}
                @category()
            </li>
        }}
        </ul>
    }}
}}

<left>
{{
	<div class="left-content-pane">
		    <div class="panel left-wrapper">
        	<div class="sub-title">Chef Recipe Videos</div>
            <ul class="drop-menu">
                @list(ChefCategories)
                {{
                    <li>
                        <a class="trigger" href="#">@(Name)</a>
                        <ul class="drop-menu sub-menu collapsed">
                        @list(Recipes)
                        {{
                            <li>
                                <a href="@(Url)">@(Name)</a>
                            </li>
                        }}
                        </ul>
                    </li>
                }}
            </ul>
	    </div>
	    <div class="panel left-wrapper">
        	<div class="sub-title">Recipes by Category</div>
            <ul class="drop-menu">
                @list(AllCategories)
                {{
                    <li>
                        <a class="trigger" href="#">@(Name)</a>
                        @category()
                    </li>
                }}
            </ul>
	    </div>
	</div>
}} :: TtlRecipeCategoriesModel

<center>
{{
	<div class="center-content-pane">
    
	</div>
}}

<right>
{{
	<div class="right-content-pane">
    
	</div>
}}

<default> -> ()
{{
    <div class="working-area-holder content-page content-with-recipe-categories-page">
        <div class="header-block">
            <img src="/assets/images/in-the-kitchen-header-4-24-14a.png" />
        </div>
        @left(RecipeCategories)
    	@center()
    	@right()
	</div>
}}
%>',
	GETDATE(),
	GETDATE(),
	2,
	NULL)

	SET @id=@@IDENTITY

	INSERT INTO [dbo].[MasterContentItemsToContentProcessors]
	([MasterContentItemId],[ContentProcessorId])
	VALUES
	(@id,19)

END

GO
