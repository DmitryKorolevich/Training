IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE [Name]='Product page' AND Updated<'2016-11-10 00:00:00.000')
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
		@if(@model.IdCustomerType==1) {{
		    <div class="product-action-bar retail">
		        <div class="wf-orange-stripe"></div>
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
    			    @if(@model.Skus.Any()){{
    			        <div style="display: none;" class="in-stock">
        				    <span id="spSelectedPrice" class="product-selected-price">Selected Price @money(@model.Skus.First().Price)</span>
                            <a href="javascript:void()" id="lnkAddToCart" class="ladda-button no-message" data-style="zoom-in">
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
    				}}
    			</div>
                <div class="product-action-right">
    			    @if(@model.Skus.Any()){{
    			        <div style="display: none;" class="out-of-stock">
    			            <a href="#">
    			                <div class="out-of-stock-image"></div>
    			                <div class="out-of-stock-right-block">
        			                <div class="out-of-stock-text">Would you like us to send you an email when this product returns?</div>
        			                <div class="out-of-stock-button"></div>
    			                </div>
        				    </a>
    			        </div>
    			        <div style="display: none;" class="in-stock cart-list">
                        	<ul>
                        	    @if(@model.Type==2){{
                        	        @if(@model.Shellfish==false){{
                                		<li>Free shipping on orders over $99.</li>
                                		<li>Ships frozen with dry ice for 1-3 day Express Ground or Air delivery.</li>
                                		<li>Vital Choice Guarantee: We''ll replace your product or refund your money if you''re not satisfied.</li>
                                	}}
                        	        @if(@model.Shellfish==true){{
                        	            <li>Free shipping on orders over $99.</li>
                                		<li>All live shellfish are shipped overnight with ice-cold gel packs.</li>
                                		<li>Vital Choice Guarantee: We''ll replace your product or refund your money if you''re not satisfied.</li>
                        	        }}
                        	    }}
                        	    @if(@model.Type==1){{
                        	        <li>Free shipping on orders over $99.</li>
                            		<li>Ships via Ground Service for 3-7 day delivery.</li>
                            		<li>Vital Choice Guarantee: We''ll replace your product or refund your money if you''re not satisfied.</li>
                        	    }}
                        	</ul>
                        </div>
    				}}
    			</div>
    		</div>  
		}}
		@ifnot(@model.IdCustomerType==1) {{
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
		}}
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
    @if(@model.Criterio!=null){{
        <script type="text/javascript" src="//static.criteo.net/js/ld/ld.js" async="true"></script>
        <script type="text/javascript">
        window.criteo_q = window.criteo_q || [];
        window.criteo_q.push(
        { event: "setAccount", account: 27307 },
        { event: "setEmail", email: "@(CustomerEmail)" },
        { event: "setSiteType", type: "d" },
        { event: "viewItem", item:[ "@(Criterio)" ]}
        );
        </script>
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

IF NOT EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE [Name]='Article Individual - 2016 2 Column Layout')
BEGIN

	INSERT [dbo].[MasterContentItems]
	(Name,TypeId,Created,Updated,StatusCode,IdEditedBy, Template)
	VALUES
	('Article Individual - 2016 2 Column Layout',4, getdate(),getdate(), 2, NULL,
	'@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles}}
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
    <div class="working-area-holder content-page article-page-2-column">
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
%>')

	INSERT MasterContentItemsToContentProcessors
	(MasterContentItemId,ContentProcessorId)
	VALUES
	((SELECT TOP 1 Id FROM MasterContentItems WHERE Name = 'Article Individual - 2016 2 Column Layout')
	,(SELECT TOP 1 Id FROM ContentProcessors WHERE Type = 'ArticleBonusLinkProcessor'))


END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE [Name]='Product sub categories' AND Updated<'2016-10-29 00:00:00.000')
BEGIN

	UPDATE [dbo].[MasterContentItems]
	SET 
		Template = '@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels}}
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
		<li><a href="/products/top-sellers">Top Sellers</a></li>
		<li><a href="/products/special-offers">Special Offers</a></li>
		<li><a href="/products/new-at-vital-choice">New Products</a></li>
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
        @if(@model.BreadcrumbOrderedItems.Any())
        {{
            @(@model.BreadcrumbOrderedItems.Last())
            {{
                <a href="@(Url)" title="@(Label)">@(Label)</a>
            }}
        }}
	</div>
}}

<category_top>
{{
	@if(@!string.IsNullOrEmpty(model.FileImageLargeUrl)) {{
	    <img src="@(FileImageLargeUrl)">
	    <br>
	}}
    @ifnot(HideLongDescription){{
	    @(LongDescription)
	}}
}}

<category_article>
{{
    @ifnot(HideLongDescriptionBottom){{
        @(LongDescriptionBottom)
    }}
}}

<layout> -> (ProductCategory)
{{
    @script(){{
        <script src="/app/modules/product/addtocart.js"></script>
        <script src="/app/modules/product/product-category.js"></script>
    }}
    @if(@model.FileImageLargeUrl!=null){{
        @socialmeta(){{
            <meta property="og:image" content="https://@(@root.AppOptions.PublicHost)@(FileImageLargeUrl)">
            <meta itemprop="image" content="https://@(@root.AppOptions.PublicHost)@(FileImageLargeUrl)" />
            <link rel="image_src" href="https://@(@root.AppOptions.PublicHost)@(FileImageLargeUrl)" />
        }}
    }}
    @if(@model.Criterio!=null){{
        <script type="text/javascript" src="//static.criteo.net/js/ld/ld.js" async="true"></script>
        <script type="text/javascript">
        window.criteo_q = window.criteo_q || [];
        window.criteo_q.push(
        { event: "setAccount", account: 27307 },
        { event: "setEmail", email: "@(CustomerEmail)" },
        { event: "setSiteType", type: "d" },
        { event: "viewList", item:[ @(Criterio) ]}
        );
        </script>
    }}
<aside id="menuSidebar" class="category-aside">
    @menu_sidebar()
</aside>
<section class="category-main">
	@category_breadcrumb()
	@if(@model.ViewType==1)
	{{
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
				<img src="@(Thumbnail)" alt="@(Name)">
				@(Name)<br/>
				@(SubTitle)
			</a>
        }}
	</div>
	<article class="category-article">
	    @category_article()
	</article>
	}}
	@if(@model.ViewType==2)
	{{
    <div class="category-skus relative">
        <div class="overlay hide">
				<div class="loading">Loading…</div>
		</div>
        <div class="margin-bottom-small">
            Specify a quantity for any of the products listed on this page, then click ''Add to Cart'' to add them to your shopping cart.
        </div>
        <table class="standard-table margin-bottom-small">
	        <thead>
        		<tr>
        			<th>Qty</th>
        			<th></th>
        			<th>SKU</th>
        			<th>Description</th>
        			<th>Case Price</th>
        		</tr>
        	</thead>
	        <tbody>
	            @list(Skus)
	            {{
	            <tr>
	                <td class="qty">
	                    @if(InStock)
	                    {{
	                    <input data-sku-code="@(Code)" class="input-control width-small">
	                    <span class="field-validation-error hide-imp">
	                        <span>Quantity must be whole number. 3 digits max.</span>
	                    </span>
	                    }}
	                </td>
	                </td>
                    <td>
                      	<div class="thumb">
                      	    <img src="@(Thumbnail)">
                        </div>
                    </td>
	                <td>
	                    <span>@(Code)</span>
	                </td>
                    <td>
                        <span class="title">
                            @(Name) @(SubTitle)
                        </span>
                        <br/>
                        @(ShortDescription)
	                    @ifnot(InStock)
	                    {{
	                    <span class="field-validation-error">
	                        <span>Currently out of stock.</span>
	                    </span>
	                    }}
                    </td>
                    <td class="price">
	                    <span>@money(Price)</span>
                    </td>
                </tr>
                }}
            </tbody>
        </table>
        <a href="javascript:void()" id="lnkAddToCart" class="ladda-button" data-style="zoom-in">
    	    <span class="ladda-label"></span>
    		<img src="/assets/images/addtocartorange-2015.jpg">
    	</a>
	</div>
	}}
</section>
}}:: TtlCategoryModel 
%>'
	WHERE Name = 'Product sub categories'

END

GO