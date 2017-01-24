IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE [Name]='Product page' AND Updated<'2017-01-24 00:00:00.000')
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
                        	    @if(@(model.Type==1 || model.Type==4)){{
                        	        <li>Free shipping on orders over $99.</li>
                            		<li>Ships via Ground Service for 3-7 day delivery.</li>
                            		<li>Vital Choice Guarantee: We''ll replace your product or refund your money if you''re not satisfied.</li>
                        	    }}
                        	    @if(@model.Type==3){{
                        	        <li>There are no shipping or handling charges for e-Gift Certificates.</li>
                                	<li>You can customize who your e-Gift Certificate will be emailed to by providing the Name and Email in the specified section after placing your order on the "Order Receipt" page.</li>
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
		@list(Skus) {{
    	    <div class="hide" itemscope itemtype="http://schema.org/Product">
                <span itemprop="name">@(@root.ProductPage.Name) @(@root.ProductPage.SubTitle)</span>
                <meta itemprop="sku" content="@(Code)" />
                <div itemprop="offers" itemscope itemtype="http://schema.org/Offer">
                    <meta itemprop="priceCurrency" content="USD" />$
                    <span itemprop="price">@(@model.Price.ToString("F"))</span>
                    <meta itemprop="itemCondition" itemtype="http://schema.org/OfferItemCondition" content="http://schema.org/NewCondition"/>New
                    @if(InStock) {{
                    <meta itemprop="availability" content="http://schema.org/InStock"/>in stock
                    }}
                    @ifnot(InStock){{
                    <meta itemprop="availability" content="http://schema.org/OutOfStock"/>out of stock
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



IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE [Name]='Article Individual - 2016 2 Column Layout' AND Updated<'2016-12-20 00:00:00.000')
BEGIN

	UPDATE [dbo].[MasterContentItems]
	SET Template = '@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles}}
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
    <style>
    .center-content-pane.article.printable div.body * {
    font-family: ''Myriad Pro'', Verdana, Tahoma, Helvetica, Arial, sans-serif !important;
    font-size:16px !important;
    line-height: 18px !important;
    }
    </style>
    
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
    <div class="bronto-subscribe-artiles-bottom-wrapper">
        <form class="bronto-form" onsubmit="return false;">
            <span>Get special offers, recipes, health news, PLUS our FREE seafood cooking guide!</span>
            <div class="email-input-container">
                <input placeholder="Email Address" class="txtEmail bronto-sub-email" type="text" />
                <div class="bubble">
                    Please enter a valid email address
                </div>
            </div>
            <a href="javascript:void(0);" class="btnPostEmail">I''m on Board</a>
            <a class="close-form" href="javascript:void(0);">Hide&nbsp;<img src="/Assets/images/cart/close-button.png" /></a>
        </form>
        <div class="bronto-form-success">
			<span>Got it, thanks! <a class="orange" href="/Assets/miscellaneous/Vital-Choice-In-the-Kitchen-2016.pdf" target="_blank">Click here</a> for your FREE seafood cooking guide & recipes e-booklet.<a class="close-form pull-right margin-right-small" href="javascript:void(0);">Hide&nbsp;<img src="/Assets/images/cart/close-button.png" /></a></span>
        </div>
	</div>
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
%>'
	WHERE [Name]='Article Individual - 2016 2 Column Layout'

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE [Name]='Article Sub Category' AND Updated<'2016-12-20 00:00:00.000')
BEGIN

	UPDATE [dbo].[MasterContentItems]
	SET Template = '@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles}}
@model() {{dynamic}}

<%
    
<left>
{{
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
}}

<center>
{{
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
}} :: TtlArticleCategoriesModel

<rightwrapper>
{{
    <div class="right-wrapper">
        @out()
    </div>
}}

<right>
{{
    @rightwrapper(){{
	    <a href="/content/newsletter-sign-up"><img src="/assets/images/news-baby-spot-8-29-13a-210x157px.png"></a>
	}}
	@if(ArticleBonusLink){{
	@rightwrapper(){{
	    <a href="@(ArticleBonusLink.Url)"><img src="/assets/images/bonus-tile-10-30-12A.jpg"></a>
	}}
	}}
    @rightwrapper(){{
	    <a href="/content/the-vital-choice-guarantee"><img src="/assets/images/guarantee-spot-8-29-13-210px.jpg"></a>
	}}
}}

<default> -> ()
{{
    
    <div class="bronto-subscribe-artiles-bottom-wrapper">
        <form class="bronto-form" onsubmit="return false;">
            <span>Get special offers, recipes, health news, PLUS our FREE seafood cooking guide!</span>
            <div class="email-input-container">
                <input placeholder="Email Address" class="txtEmail bronto-sub-email" type="text" />
                <div class="bubble">
                    Please enter a valid email address
                </div>
            </div>
            <a href="javascript:void(0);" class="btnPostEmail">I''m on Board</a>
            <a class="close-form" href="javascript:void(0);">Hide&nbsp;<img src="/Assets/images/cart/close-button.png" /></a>
        </form>
        <div class="bronto-form-success">
			<span>Got it, thanks! <a class="orange" href="/Assets/miscellaneous/Vital-Choice-In-the-Kitchen-2016.pdf" target="_blank">Click here</a> for your FREE seafood cooking guide & recipes e-booklet.<a class="close-form pull-right margin-right-small" href="javascript:void(0);">Hide&nbsp;<img src="/Assets/images/cart/close-button.png" /></a></span>
        </div>
	</div>
    <div class="working-area-holder article-categories-page">
        <div class="header-block">
            <img src="/assets/images/article-master-header-10-25-13A.png">
            <h2>Vital Choices Newsletter Article Archive: find articles by date or topic</h4>
        </div>
        <div class="left-content-pane">
            @left()
        </div>
        <div class="center-content-pane">
    	    @center(ArticleCategories)
    	</div>
    	<div class="right-content-pane">
        	@right()
    	</div>
	</div>
}}
%>'
	WHERE [Name]='Article Sub Category'

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE [Name]='Recipe Individual' AND Updated<'2016-12-20 00:00:00.000')
BEGIN

	UPDATE [dbo].[MasterContentItems]
	SET Template = '@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Recipes}}
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
        <a href="/content/newsletter-sign-up">
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
                    <img src="/assets/images/icons/tw.png">
                    <span>TWITTER</span>
                </a>
                <a target="_blank" href="https://plus.google.com/share?url=@(@model.ViewContext.AbsoluteUrl)" class="margin-right-medium small-window-open-link">
                    <img src="/assets/images/icons/g.png">
                    <span>GOOGLE+</span>
                </a>
                <a href="#" data-content-type="2" data-title="Email Recipe" data-content-name="@(@model.Model.Name)" data-absolute-url="@(@model.ViewContext.AbsoluteUrl)" class="margin-right-medium email-button">
                    <i class="fa fa-envelope"></i>
                    <span>E-MAIL</span>
                </a>
                <a target="_blank" href="http://www.addthis.com/bookmark.php?v=300&pubid=xa-509854151b0dec32" class="margin-right-medium small-window-open-link">
                    <img src="/assets/images/icons/ad.png">
                    <span>SHARE</span>
                </a>
                <a href="#" class="print-button">
                    <i class="fa fa-print"></i>
                    <span>PRINT</span>
                </a>
            </div>
        <div class="body">
            <div class="margin-bottom-medium">
                @if(@!string.IsNullOrEmpty(model.Model.YoutubeVideo))
                {{
                <div class="video margin-right-small not-printable">
                <iframe width="470" height="265" src="https://www.youtube.com/embed/@(@model.Model.YoutubeVideo)?rel=0&amp;enablejsapi=1" frameborder="0"></iframe>
                </div>
                }}
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
            	                <strong class="cross-sell-title">@(Title)</strong><br/>
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
    <div class="bronto-subscribe-artiles-bottom-wrapper">
        <form class="bronto-form" onsubmit="return false;">
            <span>Get special offers, recipes, health news, PLUS our FREE seafood cooking guide!</span>
            <div class="email-input-container">
                <input placeholder="Email Address" class="txtEmail bronto-sub-email" type="text" />
                <div class="bubble">
                    Please enter a valid email address
                </div>
            </div>
            <a href="javascript:void(0);" class="btnPostEmail">I''m on Board</a>
            <a class="close-form" href="javascript:void(0);">Hide&nbsp;<img src="/Assets/images/cart/close-button.png" /></a>
        </form>
        <div class="bronto-form-success">
			<span>Got it, thanks! <a class="orange" href="/Assets/miscellaneous/Vital-Choice-In-the-Kitchen-2016.pdf" target="_blank">Click here</a> for your FREE seafood cooking guide & recipes e-booklet.<a class="close-form pull-right margin-right-small" href="javascript:void(0);">Hide&nbsp;<img src="/Assets/images/cart/close-button.png" /></a></span>
        </div>
	</div>
    <div class="working-area-holder content-page recipe-page">
        <div class="header-block">
            <img src="/assets/images/itk-header-2015.jpg" />
        </div>
        @left(RecipeCategories)
    	@center()
	</div>
}}
%>'
	WHERE [Name]='Recipe Individual'

END

GO

IF EXISTS(SELECT [Id] FROM [dbo].[MasterContentItems] WHERE [Name]='Recipe Sub Category' AND Updated<'2016-12-20 00:00:00.000')
BEGIN

	UPDATE [dbo].[MasterContentItems]
	SET Template = '@using() {{VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Recipes}}
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
        <a href="/content/newsletter-sign-up">
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
            <a href="/content/in-the-kitchen">
                <img src="/assets/images/return-to-itk-banner-6-13-2014-blue.png">
            </a>
    	</div>
    	<div class="right-wrapper panel panel-border">
    	    <div class="sub-title">Seafood Basics</div><br/>
    	    <div class="seafood-basics">
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="/recipe/how-to-broil-salmon" class="tooltip-v tooltipstered" data-tooltip-title="How to Broil Salmon" data-tooltip-body="Many don&#8217;t realize how incredibly simple it is to broil wild Alaskan silver salmon perfectly, until seeing this short guide by Chef Becky Selengut.">
        	                <img src="/assets/images/broiling-salmon-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                How to Broil Silver Salmon
    	            </div>
    	        </div>
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="/recipe/how-to-saut-salmon" class="tooltip-v tooltipstered" data-tooltip-title="How to Saut&eacute; Salmon" data-tooltip-body="Using only seafood marinade and organic olive oil, Chef Becky Selengut shows just how simple it is to cook salmon beautifully in a frying pan.">
        	                <img src="/assets/images/saute-sockeye-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                How to Sauté Sockeye Salmon
    	            </div>
    	        </div>
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="/recipe/steamed-wild-halibut" class="tooltip-v tooltipstered" data-tooltip-title="Steamed Wild Halibut" data-tooltip-body="A remarkably simple yet highly sophisticated recipe for wild Alaskan halibut using caviar, spinach, carrots and sesame seeds, by Chef Becky Selengut.">
        	                <img src="/assets/images/steam-halibut-video-thumb-6-6-14a.jpg">
        	            </a>
    	            </div>
    	            <div class="right-part captions-block">
    	                How to Steam Halibut
    	            </div>
    	        </div>
    	        <div class="item-line">
    	            <div class="left-part">
        	            <a href="/recipe/how-to-clean-spot-prawns" class="tooltip-v tooltipstered" data-tooltip-title="How to Saut&eacute; Salmon" data-tooltip-body="Chef Becky Selengut demystifies the process of cleaning spot prawns, demonstrating how to de-vein and shell these delicacies for cooking.">
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
    <div class="bronto-subscribe-artiles-bottom-wrapper">
        <form class="bronto-form" onsubmit="return false;">
            <span>Get special offers, recipes, health news, PLUS our FREE seafood cooking guide!</span>
            <div class="email-input-container">
                <input placeholder="Email Address" class="txtEmail bronto-sub-email" type="text" />
                <div class="bubble">
                    Please enter a valid email address
                </div>
            </div>
            <a href="javascript:void(0);" class="btnPostEmail">I''m on Board</a>
            <a class="close-form" href="javascript:void(0);">Hide&nbsp;<img src="/Assets/images/cart/close-button.png" /></a>
        </form>
        <div class="bronto-form-success">
			<span>Got it, thanks! <a class="orange" href="/Assets/miscellaneous/Vital-Choice-In-the-Kitchen-2016.pdf" target="_blank">Click here</a> for your FREE seafood cooking guide & recipes e-booklet.<a class="close-form pull-right margin-right-small" href="javascript:void(0);">Hide&nbsp;<img src="/Assets/images/cart/close-button.png" /></a></span>
        </div>
	</div>
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
	WHERE [Name]='Recipe Sub Category'

END

GO
