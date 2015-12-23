IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Template like '%@(BodyHtml)%' AND [Name] = 'Product sub categories')
BEGIN
	UPDATE [dbo].[MasterContentItems]
	SET [Template] = N'@using(){{VitalChoice.Domain.Transfer.TemplateModels}}
@model() {{TtlCategoryModel}}

<%
<layout> -> ()
{{
<aside id="menuSidebar" class="category-aside">
	<ul class="category-sidebar">
		<li>
			<a href="#">
				Wild Salmon
			</a>
			<ul>
				<li><a href="#">Sockeye Salmon - Wild Alaskan</a></li>

				<li><a href="#">King Salmon - Wild Pacific</a></li>
				<li><a href="#">Reefnet Pink Salmon</a></li>

				<li><a href="#">Silver Salmon - Wild Alaskan</a></li>
				<li><a href="#">Smoked Salmon - Wild Alaskan</a></li>
				<li><a href="#">Wild Salmon Samplers</a></li>
				<li><a href="#">Seared Sockeye (Tataki)</a></li>
				<li><a href="#">Ikura Wild Salmon Caviar</a></li>
				<li><a href="#">Salmon Jerky - Pacific NW</a></li>
			</ul>
		</li>
		<li>
			<a href="#">Wild Cod, Halibut &amp; More</a>
			<ul>
				<li><a href="#">Cod - Wild Alaskan</a></li>
				<li><a href="#">Halibut - Wild Alaskan</a></li>
				<li><a href="#">Tuna - Wild NW Pacific</a></li>
				<li><a href="#">Sablefish - Wild Alaskan</a></li>
				<li><a href="#">Petrale Sole - Wild Pacific NW</a></li>
			</ul>
		</li>
		<li>
			<a href="#">Wild Shrimp and Shellfish</a>
			<ul>
				<li><a href="#">Sea Scallops - Wild</a></li>
				<li><a href="#">Lobster - Wild Maine</a></li>
				<li><a href="#">Calamari - Wild Pacific</a></li>
				<li><a href="#">Spot Prawns - Wild Pacific</a></li>
				<li><a href="#">Tanner Crab - Wild Alaskan</a></li>
				<li><a href="#">Alaskan King Crab</a></li>
				<li><a href="#">Dungeness Crab - Wild Pacific</a></li>
				<li><a href="#">Cultured Clams- Live &amp; Frozen</a></li>
				<li><a href="#">Cultured Oysters- Live &amp; Frozen</a></li>
				<li><a href="#">Cultured Mussels- Live &amp; Frozen</a></li>
				<li><a href="#">Wild Oregon Pink Shrimp - Cooked</a></li>
			</ul>
		</li>
		<li>
			<a href="#">Canned Wild Seafood</a>
			<ul>
				<li><a href="#">Sockeye Salmon</a></li>
				<li><a href="#">Pink Salmon</a></li>
				<li><a href="#">Pink Shrimp</a></li>
				<li><a href="#">Anchovies</a></li>
				<li><a href="#">Sardines</a></li>
				<li><a href="#">Sardine Fillets</a></li>
				<li><a href="#">Albacore Tuna</a></li>
				<li><a href="#">Pouched Tuna &amp; Salmon</a></li>
				<li><a href="#">Smoked Mussels</a></li>
				<li><a href="#">Mackerel</a></li>
				<li><a href="#">Dungeness Crab</a></li>
				<li><a href="#">No Salt Added Options</a></li>
				<li><a href="#">Kosher Certified Options</a></li>
				<li><a href="#">Salmon Meal Kits</a></li>
				<li><a href="#">Canned Seafood Samplers</a></li>
			</ul>
		</li>
		<li>
			<a href="#">
				Lox &amp; Smoked Fish
			</a>
			<ul>
				<li><a href="#">Sockeye Salmon:<br>Lox, Hot-Smoked, Candy, More</a></li>
				<li><a href="#">Smoked King Salmon</a></li>
				<li><a href="#">King Salmon Candy</a></li>
				<li><a href="#">Salmon Jerky Strips</a></li>
				<li><a href="#">Smoked Albacore Tuna</a></li>
				<li><a href="#">Sablefish ("black cod")</a></li>
			</ul>
		</li>
		<li>
			<a href="#">Wild Salmon<br>Burgers, Bacon, and Sausage</a>
			<ul>
				<li><a href="#">Wild Salmon Bacon</a></li>
				<li><a href="#">Wild Sockeye Salmon Burgers</a></li>
				<li><a href="#">Wild Sockeye Salmon Sausage</a></li>
				<li><a href="#">Salmon Bacon &amp; Sausage Combo</a></li>
			</ul>
		</li>
		<li>
			<a href="#">Omega-3s &amp; Supplements</a>
			<ul>
				<li><a href="#">Omega-3 Krill Oil</a></li>
				<li><a href="#">Omega-3 Salmon Oil</a></li>
				<li><a href="#">High Potency Omega-3 Therapy</a></li>
				<li><a href="#">Daily Packs: Omega-3s + D3</a></li>
				<li><a href="#">Vital Omega-3 &amp; 6 Test™ Kit</a></li>
				<li><a href="#">Rhodiola in Wild Salmon Oil</a></li>
				<li><a href="#">Curcumin in Wild Salmon Oil</a></li>
				<li><a href="#">Vitamin D + Omega-3 Combos</a></li>
				<li><a href="#">Astaxanthin Marine Antioxidant</a></li>
				<li><a href="#">Sport-Certified™ Omega-3s+D3</a></li>
			</ul>
		</li>
		<li>
			<a href="#">Soups, Cioppino &amp; Meal Kits</a>
			<ul>
				<li><a href="#">Thai Coconut Soup</a></li>
				<li><a href="#">Cioppino Soup/Stew</a></li>
				<li><a href="#">Moroccan Chickpea Soup</a></li>
				<li><a href="#">Soups + Cioppino Sampler</a></li>

				<li><a href="#">Salmon Patty Mix</a></li>
			</ul>
		</li>
		<li>
			<a href="#">Chicken, Beef, &amp; Organic Foods</a>
			<ul>
				<li><a href="#">Heritage Chicken</a></li>
				<li><a href="#">Grass-Fed Beef</a></li>
				<li><a href="#">Meals, Broths &amp; Soups - Organic &amp; Natural</a></li>

				<li><a href="#">Organic Nuts</a></li>

				<li><a href="#">Organic Trail Mix</a></li>
				<li><a href="#">Organic Dried Fruits</a></li>
				<li><a href="#">Organic Dark Chocolate</a></li>
				<li><a href="#">Organic Oils &amp; Vinegars</a></li>

				<li><a href="#">Organic Berries - Frozen</a></li>



				<li><a href="#">Organic Seafood Seasonings</a></li>
				<li><a href="#">Seaweed Salad &amp; Kelp Cubes</a></li>
				<li><a href="#">Cultured Organic Garlic Flower</a></li>
			</ul>
		</li>
		<li>
			<a href="#">Gifts, Samplers &amp; Pet Treats</a>
			<ul>
				<li><a href="#">All Gifts</a></li>
				<li><a href="#">Gift Packs</a></li>
				<li><a href="#">Gift Certificates</a></li>
				<li><a href="#">E-Gift Certificates</a></li>
				<li><a href="#">Salmon Pet Treats</a></li>
				<li><a href="#">Doctors'' Favorites</a></li>
				<li><a href="#">Parties &amp; Entertaining</a></li>
				<li><a href="#">Health Advisors'' Packs</a></li>
				<li><a href="#">Frozen Seafood Samplers</a></li>
				<li><a href="#">Red + White Fish Combos</a></li>
				<li><a href="#">Canned Seafood Samplers</a></li>
			</ul>
		</li>
		<li>
			<a href="#">Cook''s Tools &amp; Books</a>
			<ul>
				<li><a href="#">Grilling Planks</a></li>
				<li><a href="#">Snap-On Can Lids</a></li>
				<li><a href="#">Vital Choice Aprons</a></li>
				<li><a href="#">Books &amp; Cookbooks</a></li>
				<li><a href="#">Ring Pull Can Opener</a></li>
				<li><a href="#">Stainless Tea Strainer</a></li>
				<li><a href="#">Vital Choice Water Bottles</a></li>
			</ul>
		</li>
		<li><a href="#">Top Sellers</a></li>
		<li><a href="#">Special Offers</a></li>
		<li><a href="#">New Products</a></li>
	</ul>
</aside>
<section class="category-main">
	<div class="category-breadcrumb">
		<a href="#">Wild Salmon</a>
	</div>
	<div class="category-top">
		@if(@!string.IsNullOrEmpty(model.FileImageSmallUrl)):param() {{
		    <img src="@(FileImageSmallUrl)">
		    <br>
		}}
		@(LongDescription)
	</div>
	<div class="categories-selection-container">
	    @list(SubCategories)
        {{
			<a href="@(Url)" title="@(Name)">
				<img src="@(FileImageSmallUrl)" alt="@(Name)">@(Name)
			</a>
        }}
        @list(Products)
        {{
			<a href="@(Url)" title="@(Name)">
				<img src="@(Thumbnail)" alt="@(Name)">@(Name)
			</a>
        }}
	</div>
	<article class="category-article">
	    @(LongDescriptionBottom)
	</article>
</section>
}}:: TtlCategoryModel 
%>'
WHERE [Name] = 'Product sub categories'

END

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Template like '%<li><a href="#">Organic Nuts</a></li>%' AND [Name] = 'Product sub categories')
BEGIN
	UPDATE [dbo].[MasterContentItems]
	SET [Template] = N'@using() {{VitalChoice.Domain.Transfer.TemplateModels}}
@using() {{System.Linq}}
@model() {{TtlCategoryModel}}

<%
<layout> -> ()
{{
<aside id="menuSidebar" class="category-aside">
	<ul class="category-sidebar">
	    @list(SideMenuItems)
        {{
            <li>
			     @if(@model.SubItems.Count > 0):param() {{
			        <a href="#" title="@(Label)">
				        @(Label)
			        </a>
		            <ul>
		                    @list(SubItems)
                            {{
                                <li>
                                    <a href="@(Url)" title="@(Label)">
                                        @(Label)
                                    </a>
                                </li>
                            }}
		            </ul>
		        }}
		        @if(@model.SubItems.Count == 0):param(){{
		            <a href="@(Url)" title="@(Label)">
				        @(Label)
			        </a>
		        }}
			</li>
        }}
		<li><a href="#">Top Sellers</a></li>
		<li><a href="#">Special Offers</a></li>
		<li><a href="#">New Products</a></li>
	</ul>
</aside>
<section class="category-main">
	<div class="category-breadcrumb">
	    @list(BreadcrumbOrderedItems):param(BreadcrumbOrderedItems)
        {{
            <a href="@(Url)" title="@(Label)">@(Label)</a>
            @if(@model == chained.Last()){{
                <img src="/assets/images/breadarrow2.jpg">
            }}
        }}
	</div>
	<div class="category-top">
		@if(@!string.IsNullOrEmpty(model.FileImageSmallUrl)):param() {{
		    <img src="@(FileImageSmallUrl)">
		    <br>
		}}
		@(LongDescription)
	</div>
	<div class="categories-selection-container">
	    @list(SubCategories)
        {{
			<a href="@(Url)" title="@(Name)">
				<img src="@(FileImageSmallUrl)" alt="@(Name)">@(Name)
			</a>
        }}
        @list(Products)
        {{
			<a href="@(Url)" title="@(Name)">
				<img src="@(Thumbnail)" alt="@(Name)">@(Name)
			</a>
        }}
	</div>
	<article class="category-article">
	    @(LongDescriptionBottom)
	</article>
</section>
}}:: TtlCategoryModel 
%>'
WHERE [Name] = 'Product sub categories'

END

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Template like '@if(@model == chained.Last()){{' AND [Name] = 'Product sub categories')
BEGIN
	UPDATE [dbo].[MasterContentItems]
	SET [Template] = N'@using() {{VitalChoice.Domain.Transfer.TemplateModels}}
@using() {{System.Linq}}
@model() {{TtlCategoryModel}}

<%
<layout> -> ()
{{
<aside id="menuSidebar" class="category-aside">
	<ul class="category-sidebar">
	    @list(SideMenuItems)
        {{
            <li>
			     @if(@model.SubItems.Count > 0):param() {{
			        <a href="#" title="@(Label)">
				        @(Label)
			        </a>
		            <ul>
		                    @list(SubItems)
                            {{
                                <li>
                                    <a href="@(Url)" title="@(Label)">
                                        @(Label)
                                    </a>
                                </li>
                            }}
		            </ul>
		        }}
		        @if(@model.SubItems.Count == 0):param(){{
		            <a href="@(Url)" title="@(Label)">
				        @(Label)
			        </a>
		        }}
			</li>
        }}
		<li><a href="#">Top Sellers</a></li>
		<li><a href="#">Special Offers</a></li>
		<li><a href="#">New Products</a></li>
	</ul>
</aside>
<section class="category-main">
	<div class="category-breadcrumb">
	    @list(BreadcrumbOrderedItems):param(BreadcrumbOrderedItems)
        {{
            <a href="@(Url)" title="@(Label)">@(Label)</a>
            @if(@model != chained.Last()){{
                <img src="/assets/images/breadarrow2.jpg">
            }}
        }}
	</div>
	<div class="category-top">
		@if(@!string.IsNullOrEmpty(model.FileImageSmallUrl)):param() {{
		    <img src="@(FileImageSmallUrl)">
		    <br>
		}}
		@(LongDescription)
	</div>
	<div class="categories-selection-container">
	    @list(SubCategories)
        {{
			<a href="@(Url)" title="@(Name)">
				<img src="@(FileImageSmallUrl)" alt="@(Name)">@(Name)
			</a>
        }}
        @list(Products)
        {{
			<a href="@(Url)" title="@(Name)">
				<img src="@(Thumbnail)" alt="@(Name)">@(Name)
			</a>
        }}
	</div>
	<article class="category-article">
	    @(LongDescriptionBottom)
	</article>
</section>
}}:: TtlCategoryModel 
%>'
WHERE [Name] = 'Product sub categories'

END

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Template like '%@if(@!string.IsNullOrEmpty(model.FileImageSmallUrl)):param() {{%' AND [Name] = 'Product sub categories')
BEGIN
	UPDATE [dbo].[MasterContentItems]
	SET [Template] = N'@using() {{VitalChoice.Domain.Transfer.TemplateModels}}
@using() {{System.Linq}}
@model() {{TtlCategoryModel}}

<%
<layout> -> ()
{{
<aside id="menuSidebar" class="category-aside">
	<ul class="category-sidebar">
	    @list(SideMenuItems)
        {{
            <li>
			     @if(@model.SubItems.Count > 0):param() {{
			        <a href="#" title="@(Label)">
				        @(Label)
			        </a>
		            <ul>
		                    @list(SubItems)
                            {{
                                <li>
                                    <a href="@(Url)" title="@(Label)">
                                        @(Label)
                                    </a>
                                </li>
                            }}
		            </ul>
		        }}
		        @if(@model.SubItems.Count == 0):param(){{
		            <a href="@(Url)" title="@(Label)">
				        @(Label)
			        </a>
		        }}
			</li>
        }}
		<li><a href="#">Top Sellers</a></li>
		<li><a href="#">Special Offers</a></li>
		<li><a href="#">New Products</a></li>
	</ul>
</aside>
<section class="category-main">
	<div class="category-breadcrumb">
	    @list(BreadcrumbOrderedItems):param(BreadcrumbOrderedItems)
        {{
            <a href="@(Url)" title="@(Label)">@(Label)</a>
            @if(@model != chained.Last()){{
                <img src="/assets/images/breadarrow2.jpg">
            }}
        }}
	</div>
	<div class="category-top">
		@if(@!string.IsNullOrEmpty(model.FileImageLargeUrl)):param() {{
		    <img src="@(FileImageLargeUrl)">
		    <br>
		}}
		@(LongDescription)
	</div>
	<div class="categories-selection-container">
	    @list(SubCategories)
        {{
			<a href="@(Url)" title="@(Name)">
				<img src="@(FileImageSmallUrl)" alt="@(Name)">@(Name)
			</a>
        }}
        @list(Products)
        {{
			<a href="@(Url)" title="@(Name)">
				<img src="@(Thumbnail)" alt="@(Name)">@(Name)
			</a>
        }}
	</div>
	<article class="category-article">
	    @(LongDescriptionBottom)
	</article>
</section>
}}:: TtlCategoryModel 
%>'
WHERE [Name] = 'Product sub categories'

END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Template like '%<layout> -> (ProductCategory)%' AND [Name] = 'Product sub categories')
BEGIN
	IF NOT EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE [Name] = 'Product sub categories')
	BEGIN
		SET IDENTITY_INSERT [dbo].[MasterContentItems] ON

		INSERT [dbo].[MasterContentItems] ([Id], [Name], [TypeId], [Template], [Created], [Updated], [StatusCode], [UserId])
		VALUES
	(14, 'Product sub categories', 9, N'@using() {{VitalChoice.Domain.Transfer.TemplateModels}}
@using() {{System.Linq}}
@model() {{dynamic}}

<%
<menu_sidebar>
{{
	<ul class="category-sidebar">
	    @list(SideMenuItems)
        {{
            <li>
			     @if(@model.SubItems.Count > 0):param() {{
			        <a href="#" title="@(Label)">
				        @(Label)
			        </a>
		            <ul>
		                    @list(SubItems)
                            {{
                                <li>
                                    <a href="@(Url)" title="@(Label)">
                                        @(Label)
                                    </a>
                                </li>
                            }}
		            </ul>
		        }}
		        @if(@model.SubItems.Count == 0):param(){{
		            <a href="@(Url)" title="@(Label)">
				        @(Label)
			        </a>
		        }}
			</li>
        }}
		<li><a href="#">Top Sellers</a></li>
		<li><a href="#">Special Offers</a></li>
		<li><a href="#">New Products</a></li>
	</ul>
}}

<category_breadcrumb>
{{
    <div class="category-breadcrumb">
	    @list(BreadcrumbOrderedItems):param(BreadcrumbOrderedItems)
        {{
            <a href="@(Url)" title="@(Label)">@(Label)</a>
            @if(@model != chained.Last()){{
                <img src="/assets/images/breadarrow2.jpg">
            }}
        }}
	</div>
}}

<category_top>
{{
	@if(@!string.IsNullOrEmpty(model.FileImageLargeUrl)):param() {{
	    <img src="@(FileImageLargeUrl)">
	    <br>
	}}
	@(LongDescription)
}}

<category_article>
{{
    @(LongDescriptionBottom)
}}

<layout> -> (ProductCategory)
{{
<aside id="menuSidebar" class="category-aside">
    @menu_sidebar()
</aside>
<section class="category-main">
	@category_breadcrumb()
	<div class="category-top">
	    @category_top()
	</div>
	<div class="categories-selection-container">
	    @list(SubCategories)
        {{
			<a href="@(Url)" title="@(Name)">
				<img src="@(FileImageSmallUrl)" alt="@(Name)">@(Name)
			</a>
        }}
        @list(Products)
        {{
			<a href="/product/@(Url)" title="@(Name)">
				<img src="@(Thumbnail)" alt="@(Name)">@(Name)
			</a>
        }}
	</div>
	<article class="category-article">
	    @category_article()
	</article>
</section>
}}:: TtlCategoryModel 
%>', GETDATE(), GETDATE(),2, NULL)

		SET IDENTITY_INSERT [dbo].[MasterContentItems] OFF
	END
	ELSE
	BEGIN
		UPDATE [dbo].[MasterContentItems]
	SET [Template] = N'@using() {{VitalChoice.Domain.Transfer.TemplateModels}}
@using() {{System.Linq}}
@model() {{dynamic}}

<%
<menu_sidebar>
{{
	<ul class="category-sidebar">
	    @list(SideMenuItems)
        {{
            <li>
			     @if(@model.SubItems.Count > 0):param() {{
			        <a href="#" title="@(Label)">
				        @(Label)
			        </a>
		            <ul>
		                    @list(SubItems)
                            {{
                                <li>
                                    <a href="@(Url)" title="@(Label)">
                                        @(Label)
                                    </a>
                                </li>
                            }}
		            </ul>
		        }}
		        @if(@model.SubItems.Count == 0):param(){{
		            <a href="@(Url)" title="@(Label)">
				        @(Label)
			        </a>
		        }}
			</li>
        }}
		<li><a href="#">Top Sellers</a></li>
		<li><a href="#">Special Offers</a></li>
		<li><a href="#">New Products</a></li>
	</ul>
}}

<category_breadcrumb>
{{
    <div class="category-breadcrumb">
	    @list(BreadcrumbOrderedItems):param(BreadcrumbOrderedItems)
        {{
            <a href="@(Url)" title="@(Label)">@(Label)</a>
            @if(@model != chained.Last()){{
                <img src="/assets/images/breadarrow2.jpg">
            }}
        }}
	</div>
}}

<category_top>
{{
	@if(@!string.IsNullOrEmpty(model.FileImageLargeUrl)):param() {{
	    <img src="@(FileImageLargeUrl)">
	    <br>
	}}
	@(LongDescription)
}}

<category_article>
{{
    @(LongDescriptionBottom)
}}

<layout> -> (ProductCategory)
{{
<aside id="menuSidebar" class="category-aside">
    @menu_sidebar()
</aside>
<section class="category-main">
	@category_breadcrumb()
	<div class="category-top">
	    @category_top()
	</div>
	<div class="categories-selection-container">
	    @list(SubCategories)
        {{
			<a href="@(Url)" title="@(Name)">
				<img src="@(FileImageSmallUrl)" alt="@(Name)">@(Name)
			</a>
        }}
        @list(Products)
        {{
			<a href="/product/@(Url)" title="@(Name)">
				<img src="@(Thumbnail)" alt="@(Name)">@(Name)
			</a>
        }}
	</div>
	<article class="category-article">
	    @category_article()
	</article>
</section>
}}:: TtlCategoryModel 
%>'
WHERE [Name] = 'Product sub categories'
	END
END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Template like '%@using() {{VitalChoice.Domain.Transfer.TemplateModels}}%' AND [Name] = 'Product sub categories')
BEGIN
	UPDATE [dbo].[MasterContentItems]
	SET [Template] = N'@using() {{VitalChoice.Domain.Transfer.TemplateModels.Category}}
@using() {{System.Linq}}
@model() {{dynamic}}

<%
<menu_sidebar>
{{
	<ul class="category-sidebar">
	    @list(SideMenuItems)
        {{
            <li>
			     @if(@model.SubItems.Count > 0):param() {{
			        <a href="#" title="@(Label)">
				        @(Label)
			        </a>
		            <ul>
		                    @list(SubItems)
                            {{
                                <li>
                                    <a href="@(Url)" title="@(Label)">
                                        @(Label)
                                    </a>
                                </li>
                            }}
		            </ul>
		        }}
		        @if(@model.SubItems.Count == 0):param(){{
		            <a href="@(Url)" title="@(Label)">
				        @(Label)
			        </a>
		        }}
			</li>
        }}
		<li><a href="#">Top Sellers</a></li>
		<li><a href="#">Special Offers</a></li>
		<li><a href="#">New Products</a></li>
	</ul>
}}

<category_breadcrumb>
{{
    <div class="category-breadcrumb">
	    @list(BreadcrumbOrderedItems):param(BreadcrumbOrderedItems)
        {{
            <a href="@(Url)" title="@(Label)">@(Label)</a>
            @if(@model != chained.Last()){{
                <img src="/assets/images/breadarrow2.jpg">
            }}
        }}
	</div>
}}

<category_top>
{{
	@if(@!string.IsNullOrEmpty(model.FileImageLargeUrl)):param() {{
	    <img src="@(FileImageLargeUrl)">
	    <br>
	}}
	@(LongDescription)
}}

<category_article>
{{
    @(LongDescriptionBottom)
}}

<layout> -> (ProductCategory)
{{
<aside id="menuSidebar" class="category-aside">
    @menu_sidebar()
</aside>
<section class="category-main">
	@category_breadcrumb()
	<div class="category-top">
	    @category_top()
	</div>
	<div class="categories-selection-container">
	    @list(SubCategories)
        {{
			<a href="@(Url)" title="@(Name)">
				<img src="@(FileImageSmallUrl)" alt="@(Name)">@(Name)
			</a>
        }}
        @list(Products)
        {{
			<a href="/product/@(Url)" title="@(Name)">
				<img src="@(Thumbnail)" alt="@(Name)">@(Name)
			</a>
        }}
	</div>
	<article class="category-article">
	    @category_article()
	</article>
</section>
}}:: TtlCategoryModel 
%>'
WHERE [Name] = 'Product sub categories'

END

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE (Template = N'' OR Template is NULL) AND [Name] = 'Product page')
BEGIN
	UPDATE [dbo].[MasterContentItems]
	SET [Template] = N'@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage}}
@using() {{System.Linq}}
@model() {{dynamic}}

<%
<review_rating>{{
    @if(@model == 0){{
         <img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 1){{
         <img src="/assets/images/fullstar.gif"/><img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 2){{
         <img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 3){{
         <img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/emptystar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 4){{
         <img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 5){{
         <img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif" /><img class="rating-last-child" src="/assets/images/fullstar.gif" />
    }}
}}    
    
<product_breadcrumb>
{{
    <div class="category-breadcrumb">
	    @list(@model.BreadcrumbOrderedItems.Take(model.BreadcrumbOrderedItems.Count - 1)) {{
            <a href="@(Url)" title="@(Label)">@(Label)</a>
            <img src="/assets/images/breadarrow2.jpg">
        }}
        @if(@model.BreadcrumbOrderedItems.Any())
        {{
            <span>@(@model.BreadcrumbOrderedItems.Last().Label)</span>
        }}
	</div>
}}

<product_introduction>
{{
    <img class="product-intro-image" alt="@(Name)" src="@(Image)"/>
	<div class="product-intro-info">
		<div class="product-intro-main">
			<div class="product-intro-headers">
				<h1>@(Name)</h1>
				@if(SubTitle) {{
				    <h2>@(SubTitle)</h2>
				}}
				@if(@model.Skus.Any()){{
				    <h3 id="hSelectedCode">Product #@(@model.Skus.First().Code)</h3>
				}}
			</div>
			@if(@model.SpecialIcon == 1){{
			    <img title="MSC" src="/assets/images/specialIcons/msc-atc.jpg"/>
			}}
			@if(@model.SpecialIcon == 2){{
			    <img title="USDA" src="/assets/images/specialIcons/usda-atc.jpg"/>
			}}
			@if(@model.SpecialIcon == 3){{
			   <img title="ASMI" src="/assets/images/specialIcons/alaskaseafoodicon.jpg"/>
			}}
			@if(@model.SpecialIcon == 4){{
			   <img title="USDA + Fair Trade" src="/assets/images/specialIcons/usda-fairtrade-atc.jpg"/>
			}}
			@if(@model.SpecialIcon == 5){{
			    <img title="Certified Humane" src="/assets/images/specialIcons/humane-atc.jpg"/>
			}}
			@if(@model.SpecialIcon == 6){{
			    <img title="ASMI-W" src="/assets/images/specialIcons/ASMI-W.jpg"/>
			}}
		</div>
		<div class="product-intro-sub">
		    <div class="product-stars-container">
			    @review_rating(@model.ReviewsTab.AverageRatings)
			</div>
			@if(@model.ReviewsTab.ReviewsCount > 0){{
			    <span class="product-reviews-count">[@(@model.ReviewsTab.ReviewsCount)]</span>
			    <a href="#tabs-reviews" id="lnkReviewsTab">
				    Read <strong>@(@model.ReviewsTab.ReviewsCount)</strong> reviews
			    </a>
			}}
			<a class="write-review-link" href="#">
				Write a Review
			</a>
		</div>
		<div class="product-intro-description">
			@(ShortDescription)
		</div>
		@(DescriptionTab) {{
		    @if(){{
		        @ifnot(Hidden){{
		            <a class="product-intro-more" href="#tabs-details" id="lnkDescriptionTab">Read more ></a>
		        }}
		    }}
		}}
		<div class="product-action-bar">
			<div class="product-action-left">
				@if(SubProductGroupName){{
					<span class="action-left-header">@(SubProductGroupName)</span>
				}}
				@ifnot(SubProductGroupName){{
					<span class="action-left-header">Number of Portions:</span>
				}}
				@list(Skus) {{
				    <label class="product-portion-line">
					    <input name="sku" type="radio" value="@(Code)" data-in-stock="@(@model.InStock.ToString().ToLower())" data-price="@money(Price)"/>
					    @(PortionsCount) - @money(Price)
					    @if(SalesText) {{
					        <span class="product-best-value">@(SalesText)</span>
					    }}
				    </label>
				}}
			</div>
            <div class="product-action-right">
			    @if(@model.Skus.Any()){{
			        <div style="display: none;" class="in-stock">
    				    <span id="spSelectedPrice" class="product-selected-price">Selected Price @money(@model.Skus.First().Price)</span>
    				    <a href="#">
    					    <img src="/assets/images/addtocartorange-2015.jpg"/>
    				    </a>
				    </div>
			        <div style="display: none;" class="out-of-stock">
			            <a href="#">
    					    <img src="/assets/images/OOS-graphic.png"/>
    				    </a>
			        </div>
				}}
			</div>
		</div>
	</div>
}}

<product_details>
{{
    <div class="tabs-control">
		<ul>
		    @(DescriptionTab) {{
		        @if(){{
		            @ifnot(Hidden){{
		                @if(TitleOverride){{
		                   <li><a href="#tabs-details">@(TitleOverride)</a></li>
	                    }}
	                    @ifnot(TitleOverride){{
	                    <li><a href="#tabs-details">Details</a></li>
                        }}
	                }}
	            }}
            }}
            @(ReviewsTab){{
                @if(){{
                    @if(@model.ReviewsCount > 0){{
                        <li><a href="#tabs-reviews">Reviews</a></li>
                    }}
                }}
            }}
		    @(IngredientsTab) {{
		        @if(){{
		            @ifnot(Hidden){{
		                @if(TitleOverride){{
		                   <li><a href="#tabs-nutrition">@(TitleOverride)</a></li>
	                    }}
	                    @ifnot(TitleOverride){{
	                       <li><a href="#tabs-nutrition">Nutrition & Ingredients</a></li>
                        }}
	                }}
	            }}
            }}
            @(RecipesTab) {{
                @if(){{
		            @ifnot(Hidden){{
		                @if(TitleOverride){{
		                    <li><a href="#tabs-recipes">@(TitleOverride)</a></li>
	                    }}
	                    @ifnot(TitleOverride){{
	                        <li><a href="#tabs-recipes">Recipes</a></li>
                        }}
	                }}
	            }}
            }}
		    @(ServingTab) {{
		        @if(){{
		            @ifnot(Hidden){{
		                @if(TitleOverride){{
		                    <li><a href="#tabs-serving">@(TitleOverride)</a></li>
	                    }}
	                    @ifnot(TitleOverride){{
	                        <li><a href="#tabs-serving">Serving/Care</a></li>
                        }}
	                }}
	            }}
            }}
		    @(ShippingTab) {{
		        @if(){{
		            @ifnot(Hidden){{
		                @if(TitleOverride){{
		                    <li><a href="#tabs-shipping">@(TitleOverride)</a></li>
	                    }}
	                    @ifnot(TitleOverride){{
	                        <li><a href="#tabs-shipping">Shipping</a></li>
                        }}
	                }}
	            }}
            }}
		</ul>
		@(DescriptionTab) {{
		    @if(){{
		        @ifnot(Hidden){{
		            <div id="tabs-details">
    		            @(Content)
    			    </div>
	            }}  
	        }}
	    }}
	    @(ReviewsTab):param(Url){{
            @if(){{
                @if(@model.ReviewsCount > 0){{
                    <div id="tabs-reviews">
    		            <p class="product-reviews-overall">
    		                Average Ratings:
    		                @review_rating(AverageRatings)  
    		                @(AverageRatings)
    		            </p>
				        <a class="write-review-link" href="#">
					        Write a Review
				        </a>
				        <hr/>
				        @list(Reviews) {{
                            <div class="product-reviews-item">
					            <div class="reviews-item-rating">
						            @review_rating(Rating)  
						        </div>
					            <div class="reviews-item-info">
						            <span class="reviews-item-title">"@(Title)"</span>
						            <span class="reviews-item-author">@(CustomerName) on @time(DateCreated){{g}}</span>
						            <span class="reviews-item-text"><b>Review:</b> @(Review)</span>
					            </div>
				            </div>
				            <hr />
                        }}
				        <a class="read-more-reviews" href="/reviews/@(@chained)">
					        Read more reviews >
				        </a>
    			    </div>
                }}
            }}
        }}
		@(IngredientsTab) {{
		    @if(){{
		        @ifnot(Hidden){{
		            <div id="tabs-nutrition">
    		            @(Content)
    		            
    		            @if(@model.NutritionalTitle != null){{
    		                @if(@model.Content != null){{
    		                    <span class="ingredients-section-begin margin-top-medium">Ingredients:</span>
    		                }}
    		                @if(@model.Content == null){{
    		                    <span class="ingredients-section-begin">Ingredients:</span>
    		                }}
				            <span class="ingredients-product-title">@(IngredientsTitle)</span>
				            <hr/>
				            <div class="ingredients-nutrition-facts">
					            <div class="nutrition-facts-line">
						            <span class="facts-static-title">Nutrition Facts</span>
					            </div>
					            <div class="nutrition-facts-line">
						            <span class="facts-nutrition-title">@(NutritionalTitle)</span>
					            </div>
					            @if(@model.ServingSize != null){{
					                <div class="nutrition-facts-line">
					        	        <span class="facts-nutrition-line">Serving Size @(ServingSize)</span>
					                </div>
					            }}
					            @if(@model.Servings != null){{
					                <div class="nutrition-facts-line">
						                <span class="facts-nutrition-line">Number of servings: @(Servings)</span>
					                </div>
					            }}
					            <hr/>
					            <div class="nutrition-facts-line">
						            <span class="facts-hint-line">Amount Per Serving</span>
					            </div>
					            <div class="nutrition-facts-line">
						            <span class="facts-info-subtitle">Calories</span>
						            <span class="facts-info-line">@(Calories)</span>
						            <span class="facts-info-value">Calories from Fat @(CaloriesFromFat)</span>
					            </div>
					            <hr/>
					            <div class="nutrition-facts-line">
						            <span class="facts-hint-value">% Daily Value*</span>
				        	    </div>
					            <div class="nutrition-facts-line">
						            <span class="facts-info-subtitle">Total Fat</span>
						            <span class="facts-info-line">@(TotalFat)</span>
						            <span class="facts-info-value">@(TotalFatPercent)</span>
					            </div>
					            <div class="nutrition-facts-line">
						            <span class="facts-info-indent facts-info-line">Saturated Fat @(SaturatedFat)</span>
					        	    <span class="facts-info-value">@(SaturatedFatPercent)</span>
				        	    </div>
					            <div class="nutrition-facts-line">
						            <span class="facts-info-indent facts-info-line">Trans Fat @(TransFat)</span>
						             <span class="facts-info-value">@(TransFatPercent)</span>
					            </div>
					            <div class="nutrition-facts-line">
						            <span class="facts-info-subtitle">Cholesterol</span>
						            <span class="facts-info-line">@(Cholesterol)</span>
						            <span class="facts-info-value">@(CholesterolPercent)</span>
					            </div>
					            <div class="nutrition-facts-line">
					        	    <span class="facts-info-subtitle">Sodium</span>
						            <span class="facts-info-line">@(Sodium)</span>
						            <span class="facts-info-value">@(SodiumPercent)</span>
					            </div>
					            <div class="nutrition-facts-line">
						            <span class="facts-info-subtitle">Total Carbohydrate</span>
						            <span class="facts-info-line">@(TotalCarbohydrate)</span>
						            <span class="facts-info-value">@(TotalCarbohydratePercent)</span>
					            </div>
					            <div class="nutrition-facts-line">
						            <span class="facts-info-indent facts-info-line">Dietary Fiber @(DietaryFiber)</span>
					                <span class="facts-info-value">@(DietaryFiberPercent)</span>
					            </div>
					            <div class="nutrition-facts-line">
						            <span class="facts-info-indent facts-info-line">Sugars @(Sugars)</span>
						            <span class="facts-info-value">@(SugarsPercent)</span>
					            </div>
					            <div class="nutrition-facts-line">
					        	    <span class="facts-info-subtitle">Protein</span>
					        	    <span class="facts-info-line">@(Protein)</span>
					        	    <span class="facts-info-value">@(ProteinPercent)</span>
					            </div>
				        	    <hr/>
					            <div class="nutrition-facts-line">
						            <span class="facts-info-line">@(AdditionalNotes)</span>
				        	    </div>
				        	    <div class="nutrition-facts-line">
						            <span class="facts-bottom-hint">* Percent Daily Values are based on a 2,000 calorie diet. Your daily values may be higher or lower depending on your calorie needs.</span>
				        	    </div>
			        	    </div>
			        	}}
    			    </div>
	            }}
	        }}
	    }}
		@(RecipesTab) {{
		    @if(){{
		        @ifnot(Hidden){{
		            <div id="tabs-recipes">
    		            @(Content)
    		            @if(@model.Recipes.Count > 0){{
    		                <div class="margin-top-medium">
    		                    @list(Recipes){{
    		                        <a class="product-recipe-link" title="@(Name)" href="@(Url)">@(Name)</a>
    		                    }}
    		                </div>
    		            }}
    			    </div>
    			}}
	        }}
	    }}
		@(ServingTab) {{
		    @if(){{
		        @ifnot(Hidden){{
		            <div id="tabs-serving">
    		            @(Content)
    			    </div>
    			}}
	        }}
	    }}
		@(ShippingTab) {{
		    @if(){{
		        @ifnot(Hidden){{
		            <div id="tabs-shipping">
    		            @(Content)
    			    </div>
    			}}
	        }}
	    }}
	</div>
}}
	
<product_accessories>
{{
    <div class="product-related-accessories">
		<span class="product-accessories-title">Try one of these delicious recipes</span>
		<div class="accessories-container">
		    @list(YoutubeVideos) {{
                <a class="product-related-link" href="javascript:function(){return false;}" data-video-id="@(VideoId)">
				    <img src="@(Image)">
			    	@(Text)
			    </a>
            }}
		</div>
	</div>
	<div class="product-related-accessories accessories-top-margin">
		<span class="product-accessories-title">Discover these customer favorites ... satisfaction 100% Guaranteed!</span>
		<div class="accessories-container">
		    @list(CrossSells) {{
                <a class="product-related-link" href="@(Url)">
				    <img src="@(Image)">
			    </a>
            }}
		</div>
	</div>
}}	

<scripts>
{{
    <script>
		var productPublicId = "@(ProductPublicId)";
    </script>
}}
	
<layout> -> (ProductPage)
{{
    <div class="product-main">
        @product_breadcrumb()
        <section class="product-intro-container">
	        @product_introduction()    
	    </section>
	    <section class="product-detais">
	        @product_details()
	    </section>
	    <section class="product-accessories">
	        @product_accessories()
	    </section>
    </div>
    @scripts()
}}:: TtlProductPageModel 
%>'
WHERE [Name] = 'Product page'

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Template like '%@using() {{VitalChoice.Domain%' AND [Name] = 'Product sub categories')
BEGIN
	UPDATE MasterContentItems
	SET [Template] = N'@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels}}
	@using() {{System.Linq}}
	@model() {{dynamic}}

	<%
	<menu_sidebar>
	{{
		<ul class="category-sidebar">
			@list(SideMenuItems)
			{{
				<li>
					 @if(@model.SubItems.Count > 0) {{
						<a href="#" title="@(Label)">
							@(Label)
						</a>
						<ul>
								@list(SubItems)
								{{
									<li>
										<a href="@(Url)" title="@(Label)">
											@(Label)
										</a>
									</li>
								}}
						</ul>
					}}
					@if(@model.SubItems.Count == 0){{
						<a href="@(Url)" title="@(Label)">
							@(Label)
						</a>
					}}
				</li>
			}}
			<li><a href="#">Top Sellers</a></li>
			<li><a href="#">Special Offers</a></li>
			<li><a href="#">New Products</a></li>
		</ul>
	}}

	<category_breadcrumb>
	{{
		<div class="category-breadcrumb">
			@list(@model.BreadcrumbOrderedItems.Take(model.BreadcrumbOrderedItems.Count - 1))
			{{
				<a href="@(Url)" title="@(Label)">@(Label)</a>
				<img src="/assets/images/breadarrow2.jpg">
			}}
			@(@model.BreadcrumbOrderedItems.Last())
			{{
				<a href="@(Url)" title="@(Label)">@(Label)</a>
			}}
		</div>
	}}

	<category_top>
	{{
		@if(@!string.IsNullOrEmpty(model.FileImageLargeUrl)) {{
			<img src="@(FileImageLargeUrl)">
			<br>
		}}
		@(LongDescription)
	}}

	<category_article>
	{{
		@(LongDescriptionBottom)
	}}

	<layout> -> (ProductCategory)
	{{
	<aside id="menuSidebar" class="category-aside">
		@menu_sidebar()
	</aside>
	<section class="category-main">
		@category_breadcrumb()
		<div class="category-top">
			@category_top()
		</div>
		<div class="categories-selection-container">
			@list(SubCategories)
			{{
				<a href="@(Url)" title="@(Name)">
					<img src="@(FileImageSmallUrl)" alt="@(Name)">@(Name)
				</a>
			}}
			@list(Products)
			{{
				<a href="/product/@(Url)" title="@(Name)">
					<img src="@(Thumbnail)" alt="@(Name)">@(Name)
				</a>
			}}
		</div>
		<article class="category-article">
			@category_article()
		</article>
	</section>
	}}:: TtlCategoryModel 
	%>'
	WHERE [Name] = 'Product sub categories'
END

GO

IF ((SELECT TOP 1 MasterContentItemId FROM ContentCategories WHERE Type=3 AND ParentID IS NULL)
=(SELECT TOP 1 Id FROM MasterContentItems WHERE Name='Article Root Category'))
BEGIN

UPDATE ContentCategories
SET  MasterContentItemId=(SELECT TOP 1 Id FROM MasterContentItems WHERE Name='Article Sub Category')
WHERE Type=3 AND ParentID IS NULL

UPDATE ContentItems
SET Template=''
WHERE Id=(SELECT TOP 1 ContentItemId FROM ContentCategories WHERE Type=3 AND ParentID IS NULL)

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Name='Article Sub Category' AND Updated<'2015-12-10 00:00:00.000')
BEGIN
	UPDATE [dbo].[MasterContentItems]
	SET 
	[Updated]=GETDATE(),
	[Template] = N'@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles}}
@model() {{dynamic}}

<%
    
<left>
{{
	<div class="left-content-pane">
	    @if(@(model.Model.ParentId))
	    {{
	        <strong>@(@model.Model.Name)</strong>
	    }}
	    @ifnot(@(model.Model.ParentId)) {{
	        <strong>Articles by Date</strong>
	    }}
		<br/>
		<br/>
		@list(@(model.Articles.Items))
        {{
            <div class="article-line">
                @if(PublishedDate)
	            {{
                <div class="date">
                @date(PublishedDate) {{MM''/''dd''/''yyyy}}
                </div>
                }}
                <a href="@(Url)">@(Name)</a>
            </div>
    		<br/>
        }}
        @if(@!string.IsNullOrEmpty(model.Articles.PreviousLink))
	    {{
	        <a href="@(@model.Articles.PreviousLink)"><< View the previous 50 articles</a>
	    }}
	    @if(@!string.IsNullOrEmpty(model.Articles.NextLink))
	    {{
	        <a class="pull-right" href="@(@model.Articles.NextLink)">View the next 50 articles >></a>
	    }}
	</div>
}}

<center>
{{
	<div class="center-content-pane">
		<span>
		    <strong>ARTICLES BY TOPIC</strong>&nbsp;
	        @if(@!string.IsNullOrEmpty(model.ShowAllLink)) {{
		    <a href="@(ShowAllLink)">Show all</a>
		    }}
		</span>
		<br/>
		<br/>
		@list(Categories)
        {{
            <strong>@(Name)</strong><br/>
            @if(SubCategories)
            {{
                @list(SubCategories)
                {{
                    <a href="@(Url)">@(Name)</a><br/>
                }}
            }}
            <br/>
        }}
	</div>
}} :: TtlArticleCategoriesModel

<rightwrapper>
{{
    <div class="right-wrapper">
        @out()
    </div>
}}

<right>
{{
	<div class="right-content-pane">
	    @rightwrapper(){{
		    <a href="#"><img src="/assets/images/news-baby-spot-8-29-13a-210x157px.png"></a>
		}}
	    @rightwrapper(){{
		    <a href="#"><img src="/assets/images/bonus-tile-10-30-12A.jpg"></a>
		}}
	    @rightwrapper(){{
		    <a href="#"><img src="/assets/images/guarantee-spot-8-29-13-210px.jpg"></a>
		}}
	</div>
}}

<default> -> ()
{{
    <div class="working-area-holder article-categories-page">
        <div class="header-block">
            <img src="/assets/images/article-master-header-10-25-13A.png">
            <h2>Vital Choices Newsletter Article Archive: find articles by date or topic</h4>
        </div>
        @left()
    	@center(ArticleCategories)
    	@right()
	</div>
}}
%>'
WHERE Name='Article Sub Category'

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Name='Article Individual' AND Updated<'2015-12-10 00:00:00.000')
BEGIN
	UPDATE [dbo].[MasterContentItems]
	SET 
	[Updated]=GETDATE(),
	[Template] = N'@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles}}
@using() {{System.Collections.Generic}}
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
	    <strong>ARTICLES BY TOPIC</strong>
        <br/>
        <br/>
        <ul class="drop-menu">
            @list()
            {{
                <li>
                    <a class="trigger" href="#">@(Name)</a>
                    @category()
                </li>
            }}
        </ul>
	</div>
}}

<center>
{{
	<div class="center-content-pane article printable">
            <strong class="title">@(@model.Model.Name)</strong>
            <br/>
            <strong>@(@model.Model.SubTitle)</strong>
            <br/>
            <br/>
            <span class="date">@date(@model.Model.PublishedDate) {{MM''/''dd''/''yyyy}}</span>
            <span class="author">@(@model.Model.Author)</span>
            <div class="icons-bar not-printable">
    	        <a target="_blank" href="http://www.facebook.com/sharer.php?u=@(@model.Request.AbsoluteUrl)&t=@(@model.Model.Name)" class="margin-right-medium small-window-open-link">
                    <img src="/assets/images/icons/fb.png">
                    <span>FACEBOOK</span>
                </a>
                <a target="_blank" href="http://twitter.com/share?text=@(@model.Model.Name)&url=@(@model.Request.AbsoluteUrl)" class="margin-right-medium small-window-open-link">
                    <img src="/assets/images/icons/fb.png">
                    <span>TWITTER</span>
                </a>
                <a target="_blank" href="https://plus.google.com/share?url=@(@model.Request.AbsoluteUrl)" class="margin-right-medium small-window-open-link">
                    <img src="/assets/images/icons/fb.png">
                    <span>GOOGLE+</span>
                </a>
                <a href="#" data-content-name="@(@model.Model.Name)" data-absolute-url="@(@model.Request.AbsoluteUrl)" class="margin-right-medium email-button">
                    <img src="/assets/images/icons/fb.png">
                    <span>E-MAIL</span>
                </a>
                <a target="_blank" href="http://www.addthis.com/bookmark.php?v=300&pubid=xa-509854151b0dec32" class="margin-right-medium small-window-open-link">
                    <img src="/assets/images/icons/fb.png">
                    <span>SHARE</span>
                </a>
                <a href="#" class="print-button">
                    <img src="/assets/images/icons/fb.png">
                    <span>PRINT</span>
                </a>
            </div>
            <div class="body">
                <img class="main-img" src="@(@model.Model.FileUrl)"/>
                @(@model.Model.ContentItem.Description)
            </div>
	</div>
}}

<right_top>
{{
    <div class="right-wrapper newsletter-block">
        <div class="header">Vital Choices Newsletter</div>
        <div class="body">
            <strong>Special Offers • Recipes<br>
                Nutrition &amp; Eco News
            </strong>
            <br>
            <div class="input-wrapper">
                <input type="text" autocomplete="off" placeholder="Enter email here">
                <input class="yellow" type="button" value="Go">
            </div>
            <a href="#">View a recent issue</a>
        </div>
    </div>
    <div class="right-wrapper">
	    <a href="#"><img src="/assets/images//bonus-tile-10-30-12A.jpg"></a>
    </div>
}}

<right_recent_articles>
{{
    <strong>RECENT ARTICLES</strong>
    <br/>
    <br/>
    @list(@model)
    {{
        <a href="@(Url)">@(Name)</a>
        <br/>
        <br/>
    }}
}}

<right_recent_recipes>
{{
    @list(@model)
    {{
        <a href="@(Url)">@(Name)</a>
        <br/>
        <br/>
    }}
}}

<right_bottom>
{{
    <div class="right-wrapper">
	    <a href="#"><img src="/assets/images//Top-sellers-sidebar-tile-12-27-12-296px-A.jpg"></a>
    </div>
}}

<right>
{{
	<div class="right-content-pane">
    	@right_top()
    	@right_recent_articles(RecentArticles)
    	@right_recent_recipes(RecentRecipes)
    	@right_bottom()
	</div>
}}

<default> -> ()
{{
    <script>
        window.addEventListener("load", function() {
            var element = document.createElement("script");
            element.src = "/app/modules/help/sendContentUrlNotification.js";
            document.body.appendChild(element);
        }, false);
    </script>
    <div class="working-area-holder content-page article-page">
        <div class="header-block">
            <img src="/assets/images/article-master-header-10-25-13A.png">
        </div>
        @left(ArticleCategories)
    	@center()
    	@right()
	</div>
}}
%>'
WHERE Name='Article Individual'

END

GO

IF ((SELECT TOP 1 MasterContentItemId FROM ContentCategories WHERE Type=1 AND ParentID IS NULL)
=(SELECT TOP 1 Id FROM MasterContentItems WHERE Name='Recipe Root Category'))
BEGIN

UPDATE ContentCategories
SET  MasterContentItemId=(SELECT TOP 1 Id FROM MasterContentItems WHERE Name='Recipe Sub Category')
WHERE Type=1 AND ParentID IS NULL

UPDATE ContentItems
SET Template=''
WHERE Id=(SELECT TOP 1 ContentItemId FROM ContentCategories WHERE Type=1 AND ParentID IS NULL)

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Name='Recipe Sub Category' AND Updated<'2015-12-16 00:00:00.000')
BEGIN
	UPDATE [dbo].[MasterContentItems]
	SET 
	[Updated]=GETDATE(),
	[Template] = N'@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Recipes}}
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
    
<left_bottom>
{{
    <div>
        <a href="#">
            <img src="/assets/images/newsletter-recipes-spot-4-2-14-214px-b.png">
        </a>
	</div>
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
        @left_bottom()
	</div>
}} :: TtlRecipeCategoriesModel

<center>
{{
	<div class="center-content-pane">
        <strong class="title">
        @if(@model.Model.ParentId) {{
		    @(Model.Name)
		}}
		@ifnot(@model.Model.ParentId) {{
		    Recipes by Category
		}}
        </strong>
        <br/>
        <div class="clear margin-bottom-small"></div>
        @ifnot(@model.Model.ParentId) {{
            @list(RecipeCategories.AllCategories){{
                <div class="categories-group margin-bottom-small">
    	            <div class="sub-title">@(Name)</div>
                    @list(SubCategories){{
                        <a href="@(Url)">@(Name)</a><br/>
                    }}
                </div>
            }}
        }}
		@if(@model.Model.ParentId) {{
		    @if(@model.RecipeCategories.SubCategories.Count>0){{
		        @list(RecipeCategories.SubCategories){{
                    <div class="categories-group margin-bottom-small">
                        <a href="@(Url)">@(Name)</a><br/>
                    </div>
                }}
		    }}
		    @if(@model.RecipeCategories.SubCategories.Count==0){{
		        @list(Recipes){{
		            <div class="margin-bottom-small">
                        <a href="@(Url)">@(Name)</a><br/>
                    </div>
		        }}
		    }}
		}}
	</div>
}}


<right>
{{
	<div class="right-content-pane">
    	<div class="right-wrapper">
            <a href="#">
                <img src="/assets/images/return-to-itk-banner-6-13-2014-blue.png">
            </a>
    	</div>
    	<div class="right-wrapper panel panel-border">
    	    <div class="sub-title">Seafood Basics</div><br/>
    	    <div class="seafood-basics">
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered" data-tooltip-title="How to Broil Salmon" data-tooltip-body="Many don&#8217;t realize how incredibly simple it is to broil wild Alaskan silver salmon perfectly, until seeing this short guide by Chef Becky Selengut.">
        	                <img src="/assets/images/broiling-salmon-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                How to Broil Silver Salmon
    	            </div>
    	        </div>
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered" data-tooltip-title="How to Saut&eacute; Salmon" data-tooltip-body="Using only seafood marinade and organic olive oil, Chef Becky Selengut shows just how simple it is to cook salmon beautifully in a frying pan.">
        	                <img src="/assets/images/saute-sockeye-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                How to Sauté Sockeye Salmon
    	            </div>
    	        </div>
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered" data-tooltip-title="Steamed Wild Halibut" data-tooltip-body="A remarkably simple yet highly sophisticated recipe for wild Alaskan halibut using caviar, spinach, carrots and sesame seeds, by Chef Becky Selengut.">
        	                <img src="/assets/images/steam-halibut-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                How to Steam Halibut
    	            </div>
    	        </div>
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="#" class="tooltip-v tooltipstered" data-tooltip-title="How to Saut&eacute; Salmon" data-tooltip-body="Chef Becky Selengut demystifies the process of cleaning spot prawns, demonstrating how to de-vein and shell these delicacies for cooking.">
        	                <img src="/assets/images/clean-prawns-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                How to Clean Spot Prawns
    	            </div>
    	        </div>
    	    </div>
    	    <div class="clear"></div>
        </div>
    	<div class="right-wrapper">
	        <a href="#"><img src="/assets/images/new-king-salmon-banner-288px-a.jpg"></a>
        </div>
	</div>
}}

<default> -> ()
{{
    <div class="working-area-holder content-page recipe-categories-page">
        <div class="header-block">
            <img src="/assets/images/itk-header-2015.jpg">
        </div>
        @left(RecipeCategories)
    	@center()
    	@right()
	</div>
}}
%>'
WHERE Name='Recipe Sub Category'

END

GO

IF EXISTS (SELECT
	[Id]
FROM [dbo].[MasterContentItems]
WHERE Name = 'Recipe Individual'
AND Updated < '2015-12-16 00:00:00.000') BEGIN
UPDATE [dbo].[MasterContentItems]
SET	[Updated] = GETDATE(),
	[Template] = N'@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Recipes}}
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
    
<left_bottom>
{{
    <div>
        <a href="#">
            <img src="/assets/images/newsletter-recipes-spot-4-2-14-214px-b.png">
        </a>
	</div>
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
        @left_bottom()
	</div>
}} :: TtlRecipeCategoriesModel

<center>
{{
	<div class="center-content-pane printable">
        <strong class="title">@(@model.Model.Name)</strong>
        <br/>
        <span class="sub-title-italic">@(@model.Model.Subtitle)</span>
        <br/>
        <br/>
        <div class="icons-bar not-printable">
    	    <a target="_blank" href="http://www.facebook.com/sharer.php?u=@(@model.ViewContext.AbsoluteUrl)&t=@(@model.Model.Name)" class="margin-right-medium small-window-open-link">
                <img src="/assets/images/icons/fb.png">
                <span>FACEBOOK</span>
            </a>
            <a target="_blank" href="http://twitter.com/share?text=@(@model.Model.Name)&url=@(@model.ViewContext.AbsoluteUrl)" class="margin-right-medium small-window-open-link">
                <img src="/assets/images/icons/fb.png">
                <span>TWITTER</span>
            </a>
            <a target="_blank" href="https://plus.google.com/share?url=@(@model.ViewContext.AbsoluteUrl)" class="margin-right-medium small-window-open-link">
                <img src="/assets/images/icons/fb.png">
                <span>GOOGLE+</span>
            </a>
            <a href="#" data-content-name="@(@model.Model.Name)" data-absolute-url="@(@model.ViewContext.AbsoluteUrl)" class="margin-right-medium email-button">
                <img src="/assets/images/icons/fb.png">
                <span>E-MAIL</span>
            </a>
            <a target="_blank" href="http://www.addthis.com/bookmark.php?v=300&pubid=xa-509854151b0dec32" class="margin-right-medium small-window-open-link">
                <img src="/assets/images/icons/fb.png">
                <span>SHARE</span>
            </a>
            <a href="#" class="print-button">
                <img src="/assets/images/icons/fb.png">
                <span>PRINT</span>
            </a>
        </div>
        <div class="body">
            <div class="margin-bottom-medium">
                <div class="video margin-right-small not-printable">
                @if(@model.Model.YoutubeVideo)
                {{
                <iframe width="470" height="265" src="https://www.youtube.com/embed/@(@model.Model.YoutubeVideo)?rel=0&amp;enablejsapi=1" frameborder="0"></iframe>
                }}
                </div>
                @if(@!string.IsNullOrEmpty(model.Model.AboutChef))
                {{
                <div class="panel panel-border chef">
                    @(Model.AboutChef)
                </div>
                }}
            </div>
            <div class="description"> 
                @(@model.Model.ContentItem.Description)
            </div>
            @if(@!string.IsNullOrEmpty(model.Model.Ingredients))
            {{
            <div class="part-sub-title margin-bottom-medium margin-top-medium">
                <div class="text">
                    Ingredients
                </div>
                <div class="hr-wrapper">
                    <hr/>
                </div>
            </div>
            <div class="clear"></div>
            @if(@model.Model.CrossSells.Count>0)
            {{
            <div class="cross-sell-products-wrapper">
                <div class="cross-sell-products">
                    <div class="header">Shop for Key Ingredients</div>
                    <hr/>
                    @list(Model.CrossSells){{
                    <div class="item-line">
        	            <div class="left-part">
            	            <a href="@(Url)">
            	                <img src="@(Image)">
            	            </a>
        	            </div>
        	            <div class="right-part">
							<a href="@(Url)">
        						<span class="cross-sell-title">@(Title)</span><br/>
        						<span class="cross-sell-sub-title">@(Subtitle)</span>
							</a>
        	            </div>
    	            </div>
    	            }}
                </div>
            </div>
            <div class="ingredients cross-sell-products-left-part">
            }}
            @ifnot(@model.Model.CrossSells.Count>0)
            {{
            <div class="ingredients">
            }}
                @(@model.Model.Ingredients)
            </div>
            }}
            @if(@model.Model.CrossSells.Count>0)
            {{
            <div class="clear"></div>
            }}
            @if(@!string.IsNullOrEmpty(model.Model.Directions))
            {{
            <div class="part-sub-title margin-bottom-medium margin-top-medium">
                <div class="text">
                    Directions
                </div>
                <div class="hr-wrapper">
                    <hr/>
                </div>
            </div>
            <div class="clear"></div>
            <div class="directions">
                @(@model.Model.Directions)
            </div>
            }}
            <div class="part-sub-title margin-bottom-medium margin-top-medium">
                <div class="text">
                    Check out these customer favorites
                </div>
                <div class="hr-wrapper">
                    <hr/>
                </div>
            </div>
            <div class="clear"></div>
            <div class="customer-favorites-section">
                @list(Model.RelatedRecipes){{
                    <div class="item">
                        <a class="content-link" href="@(Url)">
                            <img src="@(Image)"/>
                            <div class="item-title">@(Title)</div>
                        </a>
                    </div>
                }}
            </div>
        </div>
	</div>
}}

<default> -> ()
{{
    <div class="working-area-holder content-page recipe-page">
        <div class="header-block">
            <img src="/assets/images/itk-header-2015.jpg" />
        </div>
        @left(RecipeCategories)
    	@center()
	</div>
}}
%>'
WHERE Name = 'Recipe Individual'

END

GO

IF NOT EXISTS (SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Name = 'Content Individual Empty')
BEGIN

	INSERT [dbo].[MasterContentItems] ([Name], [TypeId], [Template], [Created], [Updated], [StatusCode], [UserId])
	VALUES 
	('Content Individual Empty', 8, N'<%
<default> -> (Model)
{{
    <div class="working-area-holder content-page">
    	@(@model.ContentItem.Description)
    </div>
}} :: dynamic
%>', GETDATE(), GETDATE(),2, NULL)

END

GO

IF ((SELECT TOP 1 MasterContentItemId FROM ContentCategories WHERE Type=5 AND ParentID IS NULL)
=(SELECT TOP 1 Id FROM MasterContentItems WHERE Name='FAQ Root Category'))
BEGIN

UPDATE ContentCategories
SET  MasterContentItemId=(SELECT TOP 1 Id FROM MasterContentItems WHERE Name='FAQ Sub Category')
WHERE Type=5 AND ParentID IS NULL

UPDATE ContentItems
SET Template=''
WHERE Id=(SELECT TOP 1 ContentItemId FROM ContentCategories WHERE Type=5 AND ParentID IS NULL)

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Name='FAQ Sub Category' AND Updated<'2015-12-22 00:00:00.000')
BEGIN
	UPDATE [dbo].[MasterContentItems]
	SET 
	[Updated]=GETDATE(),
	[Template] = N'@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.FAQs}}
@model() {{dynamic}}

<%
    
<left_menu>
{{
    <div class="margin-bottom-small">
	    <a href="#"><strong>Customer Care</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="/content/contact-customer-service"><strong>Contact Customer Service</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="#"><strong>Shipping Information</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="#"><strong>Privacy Policy</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="#"><strong>Returns & Exchanges</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="#"><strong>Comments</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="/content/request-catalog"><strong>Request Catalog</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="#"><strong>FAQ</strong></a>
    </div>
}}
    
<left>
{{
	<div class="left-content-pane">
	    <div class="left-wrapper">
	        @left_menu()
		</div>
		<div class="left-wrapper">
		    <a href="#"><img src="/assets/images/news-baby-spot-8-29-13a-210x157px.png"></a>
	    </div>
	</div>
}}

<center>
{{
	<div class="center-content-pane">
        <h2>Frequently Asked Questions (FAQs)</h2>
        @ifnot(@model.Model.ParentId) {{
            <ol>
                <li class="margin-bottom-small">Click on a subject category below to view all of the FAQs related to that subject.</li>
                <li class="margin-bottom-small">If you don''t find your answer among those FAQs, please use the search box below.</li>
                <li class="margin-bottom-small">If Search does not yield an answer, please send a query from our 
                    <a href="/content/contact-customer-service">Customer Service page</a>.
                </li>
            </ol>
            <div class="input-wrapper margin-bottom-medium">
                <input type="text" autocomplete="off" placeholder="Enter your search term here">
                <input class="yellow" type="button" value="Search FAQs">
            </div>
            @list(FAQCategories.AllCategories){{
                <strong>@(Name)</strong>
                <ul>
                    @list(SubCategories){{
                        <li><a href="@(Url)">@(Name)</a></li>
                    }}
                </ul>
            }}
        }}
        @if(@model.Model.ParentId) {{
            <a href="/faqs">&lt;&lt; Back to Main FAQ Page</a>
            <br/>
            <br/>
            <div class="margin-bottom-medium"><strong>@(Model.Name)</strong></div>
            @if(@model.FAQCategories.SubCategories.Count>0){{
		        @list(FAQCategories.SubCategories){{
                    <div class="margin-bottom-small">
	                    <a href="@(Url)">@(Name)</a>
                    </div>
                }}
		    }}
		    @if(@model.FAQCategories.SubCategories.Count==0){{
		        @list(FAQs){{
                    <div class="margin-bottom-small">
	                    <a href="@(Url)">@(Name)</a>
                    </div>
                }}
		    }}
        }}
	</div>
}}

<right>
{{
	<div class="right-content-pane">
	    <div class="right-wrapper">
		    <a href="#"><img src="/assets/images/guarantee-spot-8-29-13-210px.jpg"></a>
        </div>
	    <div class="right-wrapper">
	        <strong class="centered-horizontal">
	            <a href="#">Click here to read a letter from Randy, describing the genesis and goals of Vital Choice.</a>
	        </strong>
	    </div>
	</div>
}}

<default> -> ()
{{
    <div class="working-area-holder content-page faq-part faq-categories-page">
        @left()
    	@center()
    	@right()
	</div>
}}
%>'
WHERE Name='FAQ Sub Category'

END

GO

IF EXISTS (SELECT
	[Id]
FROM [dbo].[MasterContentItems]
WHERE Name = 'FAQ Individual'
AND Updated < '2015-12-22 00:00:00.000') BEGIN
UPDATE [dbo].[MasterContentItems]
SET	[Updated] = GETDATE(),
	[Template] = N'@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.FAQs}}
@model() {{dynamic}}

<%
    
<left_menu>
{{
    <div class="margin-bottom-small">
	    <a href="#"><strong>Customer Care</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="/content/contact-customer-service"><strong>Contact Customer Service</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="#"><strong>Shipping Information</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="#"><strong>Privacy Policy</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="#"><strong>Returns & Exchanges</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="#"><strong>Comments</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="/content/request-catalog"><strong>Request Catalog</strong></a>
    </div>
    <div class="margin-bottom-small">
	    <a href="#"><strong>FAQ</strong></a>
    </div>
}}
    
<left>
{{
	<div class="left-content-pane">
	    <div class="left-wrapper">
	        @left_menu()
		</div>
		<div class="left-wrapper">
		    <a href="#"><img src="/assets/images/news-baby-spot-8-29-13a-210x157px.png"></a>
	    </div>
	</div>
}}

<center>
{{
	<div class="center-content-pane">
        <h2>Frequently Asked Questions (FAQs)</h2>
        <a href="/faqs">&lt;&lt; Back to Main FAQ Page</a>
        <br/>
        <br/>
        <div class="faq-title margin-bottom-small"><strong>@(Model.Name)</strong></div>
        <div class="faq-body">
            @(Model.ContentItem.Description)
        </div>
	</div>
}}

<right>
{{
	<div class="right-content-pane">
	    <div class="right-wrapper">
		    <a href="#"><img src="/assets/images/guarantee-spot-8-29-13-210px.jpg"></a>
        </div>
	    <div class="right-wrapper">
	        <strong class="centered-horizontal">
	            <a href="#">Click here to read a letter from Randy, describing the genesis and goals of Vital Choice.</a>
	        </strong>
	    </div>
	</div>
}}

<default> -> ()
{{
    <div class="working-area-holder content-page faq-part faq-page">
        @left()
    	@center()
    	@right()
	</div>
}}
%>'
WHERE Name = 'FAQ Individual'

END

GO