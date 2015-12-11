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
	SET [Template] = N'@using() {{VitalChoice.Domain.Transfer.TemplateModels.ProductPage}}
@using() {{System.Linq}}
@model() {{dynamic}}

<%
<product_breadcrumb>
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

<product_introduction>
{{
    <img class="product-intro-image" alt="@(Name)" src="@(Image)"/>
	<div class="product-intro-info">
		<div class="product-intro-main">
			<div class="product-intro-headers">
				<h1>@(Name)</h1>
				<h2>Skinless/Boneless 6-oz. Portions</h2>
				<h3>Product #@(@model.Skus.First().Code)</h3>
			</div>
			<img title="Alaska Seafood" src="/assets/images/products/ASMI-W.jpg"/>
		</div>
		<div class="product-intro-sub">
			<div class="product-stars-container">
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
			</div>
			<span class="product-reviews-count">[185]</span>
			<a href="#">
				Read <strong>185</strong> reviews
			</a>
			<a href="#">
				Write a Review
			</a>
		</div>
		<p class="product-intro-description">
			Rich in flavor, omega-3s, and vitamin D, our most popular salmon is abundant with the antioxidant astaxanthin, the source of its vibrant red hue. Its pure, fresh flavor is what some call the truest salmon taste.
		</p>
		<a class="product-intro-more" href="#tabs-details">Read more ></a>
		<div class="product-action-bar">
			<div class="product-action-left">
				<span class="action-left-header">Number of Portions:</span>
				<label class="product-portion-line">
					<input type="radio"/>
					6 - $79.00
				</label>
				<label class="product-portion-line">
					<input type="radio"/>
					12 - $138.00
				</label>
				<label class="product-portion-line">
					<input type="radio"/>
					24 - $239.00 <span class="product-best-value">Best Value!</span>
				</label>
			</div>
			<div class="product-action-right">
				<span class="product-selected-price">Selected Price $79.00</span>
				<a href="#">
					<img src="/assets/images/addtocartorange-2015.jpg"/>
				</a>
			</div>
		</div>
	</div>
}}

<product_details>
{{
    <div class="tabs-control">
			<ul>
				<li><a href="#tabs-details">Details</a></li>
				<li><a href="#tabs-reviews">Reviews</a></li>
				<li><a href="#tabs-nutrition">Nutrition & Ingredients</a></li>
				<li><a href="#tabs-recipes">Recipes</a></li>
				<li><a href="#tabs-serving">Serving/Care</a></li>
				<li><a href="#tabs-shipping">Shipping</a></li>
			</ul>
			<div class="tab-container" id="tabs-details">
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
				<p>Proin elit arcu, rutrum commodo, vehicula tempus, commodo a, risus. Curabitur nec arcu. Donec sollicitudin mi sit amet mauris. Nam elementum quam ullamcorper ante. Etiam aliquet massa et lorem. Mauris dapibus lacus auctor risus. Aenean tempor ullamcorper leo. Vivamus sed magna quis ligula eleifend adipiscing. Duis orci. Aliquam sodales tortor vitae ipsum. Aliquam nulla. Duis aliquam molestie erat. Ut et mauris vel pede varius sollicitudin. Sed ut dolor nec orci tincidunt interdum. Phasellus ipsum. Nunc tristique tempus lectus.</p>
			</div>
			<div id="tabs-reviews">
				<p>Morbi tincidunt, dui sit amet facilisis feugiat, odio metus gravida ante, ut pharetra massa metus id nunc. Duis scelerisque molestie turpis. Sed fringilla, massa eget luctus malesuada, metus eros molestie lectus, ut tempus eros massa ut dolor. Aenean aliquet fringilla sem. Suspendisse sed ligula in ligula suscipit aliquam. Praesent in eros vestibulum mi adipiscing adipiscing. Morbi facilisis. Curabitur ornare consequat nunc. Aenean vel metus. Ut posuere viverra nulla. Aliquam erat volutpat. Pellentesque convallis. Maecenas feugiat, tellus pellentesque pretium posuere, felis lorem euismod felis, eu ornare leo nisi vel felis. Mauris consectetur tortor et purus.</p>
			</div>
			<div id="tabs-nutrition">
				<p>Mauris eleifend est et turpis. Duis id erat. Suspendisse potenti. Aliquam vulputate, pede vel vehicula accumsan, mi neque rutrum erat, eu congue orci lorem eget lorem. Vestibulum non ante. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Fusce sodales. Quisque eu urna vel enim commodo pellentesque. Praesent eu risus hendrerit ligula tempus pretium. Curabitur lorem enim, pretium nec, feugiat nec, luctus a, lacus.</p>
				<p>Duis cursus. Maecenas ligula eros, blandit nec, pharetra at, semper at, magna. Nullam ac lacus. Nulla facilisi. Praesent viverra justo vitae neque. Praesent blandit adipiscing velit. Suspendisse potenti. Donec mattis, pede vel pharetra blandit, magna ligula faucibus eros, id euismod lacus dolor eget odio. Nam scelerisque. Donec non libero sed nulla mattis commodo. Ut sagittis. Donec nisi lectus, feugiat porttitor, tempor ac, tempor vitae, pede. Aenean vehicula velit eu tellus interdum rutrum. Maecenas commodo. Pellentesque nec elit. Fusce in lacus. Vivamus a libero vitae lectus hendrerit hendrerit.</p>
			</div>
			<div id="tabs-recipes">
				<p>Mauris eleifend est et turpis. Duis id erat. Suspendisse potenti. Aliquam vulputate, pede vel vehicula accumsan, mi neque rutrum erat, eu congue orci lorem eget lorem. Vestibulum non ante. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Fusce sodales. Quisque eu urna vel enim commodo pellentesque. Praesent eu risus hendrerit ligula tempus pretium. Curabitur lorem enim, pretium nec, feugiat nec, luctus a, lacus.</p>
				<p>Duis cursus. Maecenas ligula eros, blandit nec, pharetra at, semper at, magna. Nullam ac lacus. Nulla facilisi. Praesent viverra justo vitae neque. Praesent blandit adipiscing velit. Suspendisse potenti. Donec mattis, pede vel pharetra blandit, magna ligula faucibus eros, id euismod lacus dolor eget odio. Nam scelerisque. Donec non libero sed nulla mattis commodo. Ut sagittis. Donec nisi lectus, feugiat porttitor, tempor ac, tempor vitae, pede. Aenean vehicula velit eu tellus interdum rutrum. Maecenas commodo. Pellentesque nec elit. Fusce in lacus. Vivamus a libero vitae lectus hendrerit hendrerit.</p>
			</div>
			<div id="tabs-serving">
				<p>Mauris eleifend est et turpis. Duis id erat. Suspendisse potenti. Aliquam vulputate, pede vel vehicula accumsan, mi neque rutrum erat, eu congue orci lorem eget lorem. Vestibulum non ante. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Fusce sodales. Quisque eu urna vel enim commodo pellentesque. Praesent eu risus hendrerit ligula tempus pretium. Curabitur lorem enim, pretium nec, feugiat nec, luctus a, lacus.</p>
				<p>Duis cursus. Maecenas ligula eros, blandit nec, pharetra at, semper at, magna. Nullam ac lacus. Nulla facilisi. Praesent viverra justo vitae neque. Praesent blandit adipiscing velit. Suspendisse potenti. Donec mattis, pede vel pharetra blandit, magna ligula faucibus eros, id euismod lacus dolor eget odio. Nam scelerisque. Donec non libero sed nulla mattis commodo. Ut sagittis. Donec nisi lectus, feugiat porttitor, tempor ac, tempor vitae, pede. Aenean vehicula velit eu tellus interdum rutrum. Maecenas commodo. Pellentesque nec elit. Fusce in lacus. Vivamus a libero vitae lectus hendrerit hendrerit.</p>
			</div>
			<div id="tabs-shipping">
				<p>Mauris eleifend est et turpis. Duis id erat. Suspendisse potenti. Aliquam vulputate, pede vel vehicula accumsan, mi neque rutrum erat, eu congue orci lorem eget lorem. Vestibulum non ante. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Fusce sodales. Quisque eu urna vel enim commodo pellentesque. Praesent eu risus hendrerit ligula tempus pretium. Curabitur lorem enim, pretium nec, feugiat nec, luctus a, lacus.</p>
				<p>Duis cursus. Maecenas ligula eros, blandit nec, pharetra at, semper at, magna. Nullam ac lacus. Nulla facilisi. Praesent viverra justo vitae neque. Praesent blandit adipiscing velit. Suspendisse potenti. Donec mattis, pede vel pharetra blandit, magna ligula faucibus eros, id euismod lacus dolor eget odio. Nam scelerisque. Donec non libero sed nulla mattis commodo. Ut sagittis. Donec nisi lectus, feugiat porttitor, tempor ac, tempor vitae, pede. Aenean vehicula velit eu tellus interdum rutrum. Maecenas commodo. Pellentesque nec elit. Fusce in lacus. Vivamus a libero vitae lectus hendrerit hendrerit.</p>
			</div>
		</div>
}}
	
<product_accessories>
{{
    <div class="product-related-accessories">
		<span class="product-accessories-title">Try one of these delicious recipes</span>
		<div class="accessories-container">
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/video-thumb-wild-salmon.jpg">
				Vital Choice Wild Salmon
			</a>
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/video-thumb-salmon-sauteed.jpg">
				Sauteing Sockeye Salmon
			</a>
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/video-thumb-becky.jpg">
				Chef Becky Selengut
			</a>
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/video-thumb-salmon-broiled.jpg">
				How to Broil Salmon
			</a>
		</div>
	</div>
	<div class="product-related-accessories accessories-top-margin">
		<span class="product-accessories-title">Discover these customer favorites ... satisfaction 100% Guaranteed!</span>
		<div class="accessories-container">
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/Sockeye6oz_218.jpg">
			</a>
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/FTM606_tuna_med_218.jpg">
			</a>
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/cwrp_casn_salmon_218.jpg">
			</a>
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/NCB106_goodfishbook_218.jpg">
			</a>
		</div>
	</div>
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
}}:: TtlProductPageModel 
%>'
WHERE [Name] = 'Product page'

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Template like '%<div class="tab-container" id="tabs-details">%' AND [Name] = 'Product page')
BEGIN
	UPDATE [dbo].[MasterContentItems]
	SET [Template] = N'@using() {{VitalChoice.Domain.Transfer.TemplateModels.ProductPage}}
@using() {{System.Linq}}
@model() {{dynamic}}

<%
<product_breadcrumb>
{{
    <div class="category-breadcrumb">
	    @list(@model.BreadcrumbOrderedItems.Take(model.BreadcrumbOrderedItems.Count - 1)) {{
            <a href="@(Url)" title="@(Label)">@(Label)</a>
            <img src="/assets/images/breadarrow2.jpg">
        }}
        <span>@(@model.BreadcrumbOrderedItems.Last().Label)</span>
	</div>
}}

<product_introduction>
{{
    <img class="product-intro-image" alt="@(Name)" src="@(Image)"/>
	<div class="product-intro-info">
		<div class="product-intro-main">
			<div class="product-intro-headers">
				<h1>@(Name)</h1>
				@if(@model.SubTitle != null):param() {{
				    <h2>@(SubTitle)</h2>
				}}
				<h3>Product #@(@model.Skus.First().Code)</h3>
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
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
			</div>
			<span class="product-reviews-count">[185]</span>
			<a href="#">
				Read <strong>185</strong> reviews
			</a>
			<a href="#">
				Write a Review
			</a>
		</div>
		<div class="product-intro-description">
			@(ShortDescription)
		</div>
		<a class="product-intro-more" href="#tabs-details">Read more ></a>
		<div class="product-action-bar">
			<div class="product-action-left">
				<span class="action-left-header">Number of Portions:</span>
				<label class="product-portion-line">
					<input type="radio"/>
					6 - $79.00
				</label>
				<label class="product-portion-line">
					<input type="radio"/>
					12 - $138.00
				</label>
				<label class="product-portion-line">
					<input type="radio"/>
					24 - $239.00 <span class="product-best-value">Best Value!</span>
				</label>
			</div>
			<div class="product-action-right">
				<span class="product-selected-price">Selected Price $79.00</span>
				<a href="#">
					<img src="/assets/images/addtocartorange-2015.jpg"/>
				</a>
			</div>
		</div>
	</div>
}}

<product_details>
{{
    <div class="tabs-control">
			<ul>
			    @if(@model.DescriptionTab.Hidden == false):param() {{
			        @if(@model.DescriptionTab.TitleOverride != null):param() {{
			             <li><a href="#tabs-details">@(@model.DescriptionTab.TitleOverride)</a></li>
			        }}
			        @if(@model.DescriptionTab.TitleOverride == null) {{
			             <li><a href="#tabs-details">Details</a></li>
			        }}
			    }}
			    <!--<li><a href="#tabs-reviews">Reviews</a></li>-->
			    @if(@model.IngredientsTab.Hidden == false):param() {{
			        @if(@model.IngredientsTab.TitleOverride != null):param() {{
			             <li><a href="#tabs-nutrition">@(@model.IngredientsTab.TitleOverride)</a></li>
			        }}
			        @if(@model.IngredientsTab.TitleOverride == null) {{
			             <li><a href="#tabs-nutrition">Nutrition & Ingredients</a></li>
			        }}
			    }}
			    @if(@model.RecipesTab.Hidden == false):param() {{
			        @if(@model.RecipesTab.TitleOverride != null):param() {{
			             <li><a href="#tabs-recipes">@(@model.RecipesTab.TitleOverride)</a></li>
			        }}
			        @if(@model.RecipesTab.TitleOverride == null) {{
			             <li><a href="#tabs-recipes">Recipes</a></li>
			        }}
			    }}
			    @if(@model.ServingTab.Hidden == false):param() {{
			        @if(@model.ServingTab.TitleOverride != null):param() {{
			             <li><a href="#tabs-serving">@(@model.ServingTab.TitleOverride)</a></li>
			        }}
			        @if(@model.ServingTab.TitleOverride == null) {{
			             <li><a href="#tabs-serving">Serving/Care</a></li>
			        }}
			    }}
			    @if(@model.ShippingTab.Hidden == false):param() {{
			        @if(@model.ShippingTab.TitleOverride != null):param() {{
			             <li><a href="#tabs-shipping">@(@model.ShippingTab.TitleOverride)</a></li>
			        }}
			        @if(@model.ShippingTab.TitleOverride == null) {{
			             <li><a href="#tabs-shipping">Shipping</a></li>
			        }}
			    }}
			</ul>
			@if(@model.DescriptionTab.Hidden == false):param() {{
			    <div id="tabs-details">
			        @(@model.DescriptionTab.Content)
				</div>
			}}
			<!--Reviews -->
			@if(@model.IngredientsTab.Hidden == false):param() {{
			    <div id="tabs-nutrition">
			        @(@model.IngredientsTab.Content)
				</div>
			}}
			@if(@model.RecipesTab.Hidden == false):param() {{
			    <div id="tabs-recipes">
			        @(@model.RecipesTab.Content)
				</div>
			}}
			@if(@model.ServingTab.Hidden == false):param() {{
			    <div id="tabs-serving">
			        @(@model.ServingTab.Content)
				</div>
			}}
			@if(@model.ShippingTab.Hidden == false):param() {{
			    <div id="tabs-shipping">
			        @(@model.ShippingTab.Content)
				</div>
			}}
		</div>
}}
	
<product_accessories>
{{
    <div class="product-related-accessories">
		<span class="product-accessories-title">Try one of these delicious recipes</span>
		<div class="accessories-container">
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/video-thumb-wild-salmon.jpg">
				Vital Choice Wild Salmon
			</a>
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/video-thumb-salmon-sauteed.jpg">
				Sauteing Sockeye Salmon
			</a>
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/video-thumb-becky.jpg">
				Chef Becky Selengut
			</a>
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/video-thumb-salmon-broiled.jpg">
				How to Broil Salmon
			</a>
		</div>
	</div>
	<div class="product-related-accessories accessories-top-margin">
		<span class="product-accessories-title">Discover these customer favorites ... satisfaction 100% Guaranteed!</span>
		<div class="accessories-container">
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/Sockeye6oz_218.jpg">
			</a>
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/FTM606_tuna_med_218.jpg">
			</a>
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/cwrp_casn_salmon_218.jpg">
			</a>
			<a class="product-related-link" href="#">
				<img src="/assets/images/products/NCB106_goodfishbook_218.jpg">
			</a>
		</div>
	</div>
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
}}:: TtlProductPageModel 
%>'
WHERE [Name] = 'Product page'

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Template like '%<span class="product-selected-price">Selected Price $79.00</span>%' AND [Name] = 'Product page')
BEGIN
	UPDATE [dbo].[MasterContentItems]
	SET [Template] = N'@using() {{VitalChoice.Domain.Transfer.TemplateModels.ProductPage}}
@using() {{System.Linq}}
@model() {{dynamic}}

<%
<product_breadcrumb>
{{
    <div class="category-breadcrumb">
	    @list(@model.BreadcrumbOrderedItems.Take(model.BreadcrumbOrderedItems.Count - 1)) {{
            <a href="@(Url)" title="@(Label)">@(Label)</a>
            <img src="/assets/images/breadarrow2.jpg">
        }}
        <span>@(@model.BreadcrumbOrderedItems.Last().Label)</span>
	</div>
}}

<product_introduction>
{{
    <img class="product-intro-image" alt="@(Name)" src="@(Image)"/>
	<div class="product-intro-info">
		<div class="product-intro-main">
			<div class="product-intro-headers">
				<h1>@(Name)</h1>
				@if(@model.SubTitle != null):param() {{
				    <h2>@(SubTitle)</h2>
				}}
				<h3 id="hSelectedCode">Product #@(@model.Skus.First().Code)</h3>
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
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
			</div>
			<span class="product-reviews-count">[185]</span>
			<a href="#">
				Read <strong>185</strong> reviews
			</a>
			<a href="#">
				Write a Review
			</a>
		</div>
		<div class="product-intro-description">
			@(ShortDescription)
		</div>
		<a class="product-intro-more" href="#tabs-details">Read more ></a>
		<div class="product-action-bar">
			<div class="product-action-left">
				<span class="action-left-header">Number of Portions:</span>
				@list(@model.Skus) {{
				    <label class="product-portion-line">
					    <input name="sku" type="radio" value="@(Code)" data-price="@money(Price)"/>
					    @(PortionsCount) - @money(Price)
					    @if(@model.BestValue) {{
					        <span class="product-best-value">Best Value!</span>
					    }}
				    </label>
				}}
			</div>
			<div class="product-action-right">
				<span id="spSelectedPrice" class="product-selected-price">Selected Price @money(@model.Skus.First().Price)</span>
				<a href="#">
					<img src="/assets/images/addtocartorange-2015.jpg"/>
				</a>
			</div>
		</div>
	</div>
}}

<product_details>
{{
    <div class="tabs-control">
			<ul>
			    @if(@model.DescriptionTab != null && model.DescriptionTab.Hidden == false):param() {{
			        @if(@model.DescriptionTab.TitleOverride != null):param() {{
			             <li><a href="#tabs-details">@(@model.DescriptionTab.TitleOverride)</a></li>
			        }}
			        @if(@model.DescriptionTab.TitleOverride == null) {{
			             <li><a href="#tabs-details">Details</a></li>
			        }}
			    }}
			    <!--<li><a href="#tabs-reviews">Reviews</a></li>-->
			    @if(@model.IngredientsTab != null && model.IngredientsTab.Hidden == false):param() {{
			        @if(@model.IngredientsTab.TitleOverride != null):param() {{
			             <li><a href="#tabs-nutrition">@(@model.IngredientsTab.TitleOverride)</a></li>
			        }}
			        @if(@model.IngredientsTab.TitleOverride == null) {{
			             <li><a href="#tabs-nutrition">Nutrition & Ingredients</a></li>
			        }}
			    }}
			    @if(@model.RecipesTab != null && model.RecipesTab.Hidden == false):param() {{
			        @if(@model.RecipesTab.TitleOverride != null):param() {{
			             <li><a href="#tabs-recipes">@(@model.RecipesTab.TitleOverride)</a></li>
			        }}
			        @if(@model.RecipesTab.TitleOverride == null) {{
			             <li><a href="#tabs-recipes">Recipes</a></li>
			        }}
			    }}
			    @if(@model.ServingTab != null && model.ServingTab.Hidden == false):param() {{
			        @if(@model.ServingTab.TitleOverride != null):param() {{
			             <li><a href="#tabs-serving">@(@model.ServingTab.TitleOverride)</a></li>
			        }}
			        @if(@model.ServingTab.TitleOverride == null) {{
			             <li><a href="#tabs-serving">Serving/Care</a></li>
			        }}
			    }}
			    @if(@model.ShippingTab != null && model.ShippingTab.Hidden == false):param() {{
			        @if(@model.ShippingTab.TitleOverride != null):param() {{
			             <li><a href="#tabs-shipping">@(@model.ShippingTab.TitleOverride)</a></li>
			        }}
			        @if(@model.ShippingTab.TitleOverride == null) {{
			             <li><a href="#tabs-shipping">Shipping</a></li>
			        }}
			    }}
			</ul>
			@if(@model.DescriptionTab != null && model.DescriptionTab.Hidden == false):param() {{
			    <div id="tabs-details">
			        @(@model.DescriptionTab.Content)
				</div>
			}}
			<!--Reviews -->
			@if(@model.IngredientsTab != null && model.IngredientsTab.Hidden == false):param() {{
			    <div id="tabs-nutrition">
			        @(@model.IngredientsTab.Content)
				</div>
			}}
			@if(@model.RecipesTab != null && model.RecipesTab.Hidden == false):param() {{
			    <div id="tabs-recipes">
			        @(@model.RecipesTab.Content)
				</div>
			}}
			@if(@model.ServingTab != null && model.ServingTab.Hidden == false):param() {{
			    <div id="tabs-serving">
			        @(@model.ServingTab.Content)
				</div>
			}}
			@if(@model.ShippingTab != null && model.ShippingTab.Hidden == false):param() {{
			    <div id="tabs-shipping">
			        @(@model.ShippingTab.Content)
				</div>
			}}
		</div>
}}
	
<product_accessories>
{{
    <div class="product-related-accessories">
		<span class="product-accessories-title">Try one of these delicious recipes</span>
		<div class="accessories-container">
		    @list(@model.YoutubeVideos) {{
                <a class="product-related-link" target="_blank" href="@(Video)">
				    <img src="@(Image)">
			    	@(Text)
			    </a>
            }}
		</div>
	</div>
	<div class="product-related-accessories accessories-top-margin">
		<span class="product-accessories-title">Discover these customer favorites ... satisfaction 100% Guaranteed!</span>
		<div class="accessories-container">
		    @list(@model.CrossSells) {{
                <a class="product-related-link" target="_blank" href="@(Url)">
				    <img src="@(Image)">
			    </a>
            }}
		</div>
	</div>
}}	

<scripts>
{{
    <script>
   window.addEventListener("load", function(){
         '+ CHAR(36) + '("input[name=sku]:first").attr("checked", true);
   
         '+ CHAR(36) + '("body").on("change", "input[name=sku]", function(){
            var jChecked =  '+ CHAR(36) + '("input[name=sku]:checked");
            
             '+ CHAR(36) + '("#spSelectedPrice").text("Selected Price " + jChecked.attr("data-price"));
             '+ CHAR(36) + '("#hSelectedCode").text("Product #" + jChecked.val());
        });
    }, false);
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


IF NOT EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Template like '%@if(@model.Skus.Any()){{%' AND [Name] = 'Product page')
BEGIN
	UPDATE [dbo].[MasterContentItems]
	SET [Template] = N'@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage}}
@using() {{System.Linq}}
@model() {{dynamic}}

<%
<product_breadcrumb>
{{
    <div class="category-breadcrumb">
	    @list(@model.BreadcrumbOrderedItems.Take(model.BreadcrumbOrderedItems.Count - 1)) {{
            <a href="@(Url)" title="@(Label)">@(Label)</a>
            <img src="/assets/images/breadarrow2.jpg">
        }}
        <span>@(@model.BreadcrumbOrderedItems.Last().Label)</span>
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
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
				<img src="/assets/images/products/fullstar.gif">
			</div>
			<span class="product-reviews-count">[185]</span>
			<a href="#">
				Read <strong>185</strong> reviews
			</a>
			<a href="#">
				Write a Review
			</a>
		</div>
		<div class="product-intro-description">
			@(ShortDescription)
		</div>
		<a class="product-intro-more" href="#tabs-details">Read more ></a>
		<div class="product-action-bar">
			<div class="product-action-left">
				<span class="action-left-header">Number of Portions:</span>
				@list(Skus) {{
				    <label class="product-portion-line">
					    <input name="sku" type="radio" value="@(Code)" data-price="@money(Price)"/>
					    @(PortionsCount) - @money(Price)
					    @if(BestValue) {{
					        <span class="product-best-value">Best Value!</span>
					    }}
				    </label>
				}}
			</div>
			<div class="product-action-right">
			    @if(@model.Skus.Any()){{
				    <span id="spSelectedPrice" class="product-selected-price">Selected Price @money(@model.Skus.First().Price)</span>
				}}
				<a href="#">
					<img src="/assets/images/addtocartorange-2015.jpg"/>
				</a>
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
		    <!--<li><a href="#tabs-reviews">Reviews</a></li>-->
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
		<!--Reviews -->
		@(IngredientsTab) {{
		    @if(){{
		        @ifnot(Hidden){{
		            <div id="tabs-nutrition">
    		            @(Content)
    			    </div>
	            }}
	        }}
	    }}
		@(RecipesTab) {{
		    @if(){{
		        @ifnot(Hidden){{
		            <div id="tabs-recipes">
    		            @(Content)
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
                <a class="product-related-link" target="_blank" href="@(Video)">
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
                <a class="product-related-link" target="_blank" href="@(Url)">
				    <img src="@(Image)">
			    </a>
            }}
		</div>
	</div>
}}	

<scripts>
{{
    <script>
   window.addEventListener("load", function(){
          '+ CHAR(36) + '("input[name=sku]:first").attr("checked", true);
   
         '+ CHAR(36) + '("body").on("change", "input[name=sku]", function(){
            var jChecked =  '+ CHAR(36) + '("input[name=sku]:checked");
            
             '+ CHAR(36) + '("#spSelectedPrice").text("Selected Price " + jChecked.attr("data-price"));
             '+ CHAR(36) + '("#hSelectedCode").text("Product #" + jChecked.val());
        });
    }, false);
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

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Template like '%@time(DateCreated)</span>%' AND [Name] = 'Product page')
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
        <span>@(@model.BreadcrumbOrderedItems.Last().Label)</span>
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
					    <input name="sku" type="radio" value="@(Code)" data-price="@money(Price)"/>
					    @(PortionsCount) - @money(Price)
					    @if(SalesText) {{
					        <span class="product-best-value">@(SalesText)</span>
					    }}
				    </label>
				}}
			</div>
			<div class="product-action-right">
			    @if(@model.Skus.Any()){{
				    <span id="spSelectedPrice" class="product-selected-price">Selected Price @money(@model.Skus.First().Price)</span>
				}}
				<a href="#">
					<img src="/assets/images/addtocartorange-2015.jpg"/>
				</a>
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
	    @(ReviewsTab){{
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
				        <a class="read-more-reviews" href="#">
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
    			    </div>
	            }}
	        }}
	    }}
		@(RecipesTab) {{
		    @if(){{
		        @ifnot(Hidden){{
		            <div id="tabs-recipes">
    		            @(Content)
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
                <a class="product-related-link" target="_blank" href="@(Video)" data-video-id="@(VideoId)">
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
                <a class="product-related-link" target="_blank" href="@(Url)">
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

IF NOT EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE Template like '%<a class="read-more-reviews" href="/reviews/@(@chained)">%' AND [Name] = 'Product page')
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
        <span>@(@model.BreadcrumbOrderedItems.Last().Label)</span>
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
					    <input name="sku" type="radio" value="@(Code)" data-price="@money(Price)"/>
					    @(PortionsCount) - @money(Price)
					    @if(SalesText) {{
					        <span class="product-best-value">@(SalesText)</span>
					    }}
				    </label>
				}}
			</div>
			<div class="product-action-right">
			    @if(@model.Skus.Any()){{
				    <span id="spSelectedPrice" class="product-selected-price">Selected Price @money(@model.Skus.First().Price)</span>
				}}
				<a href="#">
					<img src="/assets/images/addtocartorange-2015.jpg"/>
				</a>
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
                <a class="product-related-link" target="_blank" href="@(Url)">
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
    	        <a target="_blank" href="http://www.facebook.com/sharer.php?u=@(@model.Request.AbsoluteUrl)&t=@(@model.Model.Name)" class="margin-right-medium">
                    <img src="/assets/images/icons/fb.png">
                    <span>FACEBOOK</span>
                </a>
                <a target="_blank" href="http://twitter.com/share?text=@(@model.Model.Name)&url=@(@model.Request.AbsoluteUrl)" class="margin-right-medium">
                    <img src="/assets/images/icons/fb.png">
                    <span>TWITTER</span>
                </a>
                <a target="_blank" href="https://plus.google.com/share?url=@(@model.Request.AbsoluteUrl)" class="margin-right-medium">
                    <img src="/assets/images/icons/fb.png">
                    <span>GOOGLE+</span>
                </a>
                <a href="#" data-content-name="@(@model.Model.Name)" data-absolute-url="@(@model.Request.AbsoluteUrl)" class="margin-right-medium email-button">
                    <img src="/assets/images/icons/fb.png">
                    <span>E-MAIL</span>
                </a>
                <a target="_blank" href="http://www.addthis.com/bookmark.php?v=300&pubid=xa-509854151b0dec32" class="margin-right-medium">
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