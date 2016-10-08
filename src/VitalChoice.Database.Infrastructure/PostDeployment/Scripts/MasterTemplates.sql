﻿IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE [Name]='Product page' AND Updated<'2016-10-01 00:00:00.000')
BEGIN

	UPDATE [dbo].[MasterContentItems]
	SET 
		Template = '@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage}}
@using() {{System.Linq}}
@model() {{dynamic}}

<%
<review_rating>{{
    @if(@model == 0){{
         <img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 0.5){{
        <img src="/assets/images/halfstar.gif"/><img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 1){{
        <img src="/assets/images/fullstar.gif"/><img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 1.5){{
        <img src="/assets/images/fullstar.gif"/><img src="/assets/images/halfstar.gif"/><img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 2){{
        <img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/emptystar.gif"/><img src="/assets/images/emptystar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 2.5){{
        <img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/halfstar.gif"/><img src="/assets/images/emptystar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 3){{
        <img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/emptystar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 3.5){{
        <img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/halfstar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 4){{
        <img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif" /><img class="rating-last-child" src="/assets/images/emptystar.gif" />
    }}
    @if(@model == 4.5){{
        <img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif" /><img class="rating-last-child" src="/assets/images/halfstar.gif" />
    }}
    @if(@model == 5){{
         <img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif"/><img src="/assets/images/fullstar.gif" /><img class="rating-last-child" src="/assets/images/fullstar.gif" />
    }}
}}    
    
<product_breadcrumb>
{{
    <div class="category-breadcrumb">
        @(BreadcrumbOrderedItems) {{
    	    @list(@model.Take(model.Count - 1)) {{
                <a href="@(Url)" title="@(Label)">@(Label)</a>
                <img src="/assets/images/breadarrow2.jpg">
            }}
            @if(@model.Any())
            {{
                <span>@(@model.Last().Label)</span>
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
    		@(ReviewsTab) {{
    		    <div class="product-stars-container">
    			    @review_rating(AverageRatings)
    			</div>
    			@if(@model.ReviewsCount > 0){{
    			    <a href="#tabs-reviews" id="lnkReviewsTab">
    				    Read <strong>@(ReviewsCount)</strong> reviews
    			    </a>
    			}}
    			<a class="write-review-link" href="#">
    				Write a Review
    			</a>
    		}}
		</div>
		<div class="product-intro-description">
			@(ShortDescription)
		</div>
		@(DescriptionTab) {{
		    @if(){{
		        @ifnot(Hidden){{
		            <a class="product-intro-more" href="#product-detais-tabs" id="lnkDescriptionTab">Read more &gt;</a>
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
					    <input name="sku" type="radio" value="@(Code)" data-in-stock="@(@model.InStock.ToString().ToLower())" data-price="@money(Price)" data-autoship="@(AutoShip)"/>
					    @(PortionsCount) - @money(Price)
					    @if(SalesText) {{
					        <span class="product-best-value">@(SalesText)</span>
					    }}
				    </label>
				}}
				<div class="mybuys" style="display:none">
    				@list(Skus) {{
    				    <div class="MyBuys_ProductPrice">@(@model.Price.ToString("F"))</div>
    				}}
				</div>
				<div id="hidden_mybuys" style="display:none">
                    <span class="inStock_mb">@(@model.AtLeastOneSkuInStock.ToString())</span>
                </div>
			</div>
            <div class="product-action-right">
			    @if(@model.Skus.Any()){{
			        <div style="display: none;" class="in-stock">
    				    <span id="spSelectedPrice" class="product-selected-price">Selected Price @money(@model.Skus.First().Price)</span>
    				    @if(@model.ShowDiscountMessage==true){{
    				    <span class="wholesale-message-line">15% discount on largest size</span>
    				    <a href="javascript:void()" id="lnkAddToCart" class="ladda-button" data-style="zoom-in">
    				    }}
    				    @ifnot(@model.ShowDiscountMessage==true){{
    				    <a href="javascript:void()" id="lnkAddToCart" class="ladda-button no-message" data-style="zoom-in">
    				    }}
    					    <span class="ladda-label"></span>
    					    <img src="/assets/images/addtocartorange-2015.jpg"/>
    				    </a>
    				        <div class="product-autoship-container" style="display: none;">
    				            <a href="#" class="product-autoship-link"></a>
    				            <i class="tooltip-v glyphicon glyphicon-question-sign" data-tooltip-title="Learn About Auto-Shipping" data-tooltip-body="<strong>Why choose Auto-Shipping?</strong><br>
• You get <u>FREE Shipping &amp; Discount*</u> every shipment!<br>
• Easily Pause or Cancel your our Auto-Shipments at any time.<br>
• We only charge your credit card when an item ships … we''ll alert you by email.<br><br>
 
<strong>Important Notes:</strong><br>
• Your Cart will be emptied of any existing items if you choose Auto-Ship for this item.<br>
• You <em>cannot</em> add other items to your Cart until you finish your Auto-Ship order.<br>
• After clicking Auto-Ship, you must log in to your account, or create one.<br>
• Choose your desired delivery schedule on the page that appears after you log in.
<br><br>
*Cannot be combined with other offers."></i>
                            </div>
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
                                    <span class="reviews-item-author">@(CustomerName) on @date(DateCreated){{g}}</span>
						            <span class="reviews-item-text"><b>Review:</b> @(Review)</span>
					            </div>
				            </div>
				            <hr />
                        }}
                        @if(@model.ReviewsCount > 3){{
    				        <a class="read-more-reviews" href="/reviews/@(@chained)">
    					        Read more reviews >
    				        </a>
				        }}
    			    </div>
                }}
            }}
        }}
		@(IngredientsTab) {{
		    @if(){{
		        @ifnot(Hidden){{
		            <div id="tabs-nutrition">
    		            @if(NutritionalTitle){{
		                    <span class="ingredients-section-begin">Ingredients:</span>
				            @(Content)
				            <hr/>
				            <div class="ingredients-nutrition-facts">
					            <div class="nutrition-facts-line">
						            <span class="facts-static-title">Nutrition Facts</span>
					            </div>
					            <div class="nutrition-facts-line">
						            <span class="facts-nutrition-title">@(NutritionalTitle)</span>
					            </div>
					            @if(ServingSize){{
					                <div class="nutrition-facts-line">
					        	        <span class="facts-nutrition-line">Serving Size @(ServingSize)</span>
					                </div>
					            }}
					            @if(Servings){{
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
                <a class="product-related-link" href="javascript:function(){return false;}" data-video-id="@(VideoId)" data-video-modal="true" data-video-autoplay="true" data-video-autoclose="true">
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
    @if(Image){{
        @socialmeta(){{
            <meta property="og:image" content="https://@(::AppOptions.PublicHost)@(Image)">
            <meta itemprop="image" content="https://@(::AppOptions.PublicHost)@(Image)" />
            <link rel="image_src" href="https://@(::AppOptions.PublicHost)@(Image)" />
        }}
    }}
    <div class="product-main relative">
        <div class="overlay hide">
				<div class="loading">Loading…</div>
		</div>
        @product_breadcrumb()
        <section class="product-intro-container">
	        @product_introduction()    
	    </section>
	    <section class="product-detais" id="product-detais-tabs">
	        @product_details()
	    </section>
	    <section class="product-accessories">
	        @product_accessories()
	    </section>
    </div>
    @scripts()
}}:: TtlProductPageModel 
%>'
	WHERE Name = 'Product page'

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE [Name]='Article Individual' AND Updated<'2016-10-07 00:00:00.000')
BEGIN

	UPDATE [dbo].[MasterContentItems]
	SET 
		Updated = GETDATE(),
		Template = '@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles}}
@using() {{System.Collections.Generic}}
@model() {{dynamic}}

<%

<center>
{{
	<div class="center-content-pane article printable">
	    @if(@!string.IsNullOrEmpty(model.Model.FileUrl))
        {{
            <div>
                <img class="main-img" src="@(@model.Model.FileUrl)"/>
            </div>
        }}
        <div class="top-section">
            <span class="title">@(@model.Model.Name)</span>
            <br/>
            <span class="sub-title">@(@model.Model.SubTitle)</span>
            <br/>
            <br/>
            <span class="date">@date(@model.Model.PublishedDate) {{MM''/''dd''/''yyyy}}</span>
            <span class="author">@(@model.Model.Author)</span>
            <div class="icons-bar not-printable">
        	    <a target="_blank" href="http://www.facebook.com/sharer.php?u=@(@model.ViewContext.AbsoluteUrl)&t=@(@model.Model.Name)" class="margin-right-medium small-window-open-link">
                    <img src="/assets/images/articles/facebook.jpg">
                </a>
                <a target="_blank" href="http://twitter.com/share?text=@(@model.Model.Name)&url=@(@model.ViewContext.AbsoluteUrl)" class="margin-right-medium small-window-open-link">
                    <img src="/assets/images/articles/twitter.jpg">
                </a>
                <a target="_blank" href="https://plus.google.com/share?url=@(@model.ViewContext.AbsoluteUrl)" class="margin-right-medium small-window-open-link">
                    <img src="/assets/images/articles/google.jpg">
                </a>
                <a href="#" data-content-type="1" data-title="Email Article" data-content-name="@(@model.Model.Name)" data-absolute-url="@(@model.ViewContext.AbsoluteUrl)" class="margin-right-medium email-button">
                    <img src="/assets/images/articles/email.jpg">
                </a>
                <a target="_blank" href="#" class="print-button margin-right-medium">
                    <img src="/assets/images/articles/print.jpg">
                </a>
                <a href="http://www.addthis.com/bookmark.php?v=300&pubid=xa-509854151b0dec32" class="small-window-open-link">
                    <img src="/assets/images/articles/more.jpg">
                </a>
            </div>
        </div>
        <div class="body">   
            @(@model.Model.ContentItem.Description)
        </div>
	</div>
}}

<right_top>
{{
	@if(ArticleBonusLink){{
    <div class="right-wrapper">
        <a href="@(ArticleBonusLink.Url)"><img src="/assets/images/articles/bonuses.jpg"></a>
    </div>
    }}
    <div class="right-wrapper">
        <a href="/products/top-sellers"><img src="/assets/images/articles/top-sellers.jpg"></a>
    </div>
    <div class="right-wrapper newsletter-block">
        <div class="header">Vital Choices Newsletter</div>
        <div class="body">
            <span>
                Sign-up for special offers, recipes, nutrition and eco news
            </span>
            <br>
            <div class="input-wrapper">
                <form method="post" name="search" onsubmit="return brontoSignupValidateEmail(event)" action="https://app.bronto.com/public/webform/process/">
                		<input type="hidden" value="98fbmg41k7lrzlevfi29oph85r7l8" name="fid"/>
    					<input type="hidden" value="09dcaffa5c9971f4be87813780496171" name="sid"/>
    					<input type="hidden" value="" name="delid"/>
    					<input type="hidden" value="" name="subid"/>
                        <input type="hidden" name="td" value="">
                        <input type="hidden" name="formtype" value="addcontact">
    					<input type="hidden" value="true" name="74699[291714]"/>
                        <input type="text" name="74686" autocomplete="off" class="bronto-email" placeholder="Enter email here">
                        <input class="yellow" type="submit" value="Go">
                        <div class="sugnup-bubble" style="left:-82px;">
                            Please enter a valid email address
                        </div>
                    </form>
            </div>
            <a href="http://hosting-source.bm23.com/26468/public/PAGES/VC-NEWS-2-4-13-A.html" target="_blank">Recent Issues</a>
        </div>
    </div>
    <a class="block-link" href="/articles">
        <div class="right-wrapper more-articles">
            More Articles >
        </div>
    </a>
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

<right>
{{
	<div class="right-content-pane">
    	@right_top()
	</div>
}}

<default> -> ()
{{
    @script(){{
        <script src="https://www.google.com/recaptcha/api.js" async defer></script>
        <script src="/app/modules/help/sendContentUrlNotification.js"></script>
    }}
    @if(@model.Model.FileUrl!=null){{
        @socialmeta(){{
            <meta property="og:image" content="https://@(@root.AppOptions.PublicHost)@(@model.Model.FileUrl)">
            <meta itemprop="image" content="https://@(@root.AppOptions.PublicHost)@(@model.Model.FileUrl)" />
            <link rel="image_src" href="https://@(@root.AppOptions.PublicHost)@(@model.Model.FileUrl)" />
        }}
    }}
    <div class="working-area-holder content-page article-page">
        <div class="header-block">
            <img usemap="#ArticleHeader" src="/assets/images/articles/article-page-header.jpg">
            <map name="ArticleHeader" id="ArticleHeader">
			    <area shape="rect" coords="780,22,808,50" href="https://www.youtube.com/user/VitalChoiceSeafood" alt="Youtube" target="_blank">
				<area shape="rect" coords="814,22,842,50" href="https://pinterest.com/vitalchoice/" alt="Pintrest" target="_blank">
				<area shape="rect" coords="848,22,876,50" href="https://www.facebook.com/vitalchoice" alt="Facebook" target="_blank">
				<area shape="rect" coords="882,22,910,50" href="https://twitter.com/vitalchoice" alt="Twitter" target="_blank">
			</map>
        </div>
    	@center()
    	@right()
	</div>
}}
%>'
	WHERE Name = 'Article Individual'

END

GO