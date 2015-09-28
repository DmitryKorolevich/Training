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