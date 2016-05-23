'use strict';

angular.module('app.modules.product', [
	'app.modules.product.controllers.productCategoriesController',
	'app.modules.product.controllers.productCategoryManageController',
	'app.modules.product.controllers.inventoryCategoriesController',
	'app.modules.product.controllers.inventoryCategoryManageController',
	'app.modules.product.controllers.productsController',
	'app.modules.product.controllers.productManageController',
	'app.modules.product.controllers.addProductPopupController',
	'app.modules.product.controllers.discountsController',
	'app.modules.product.controllers.discountManageController',
	'app.modules.product.controllers.skusSearchController',
	'app.modules.product.controllers.productsSearchController',
	'app.modules.product.controllers.productsWithReviewsController',
	'app.modules.product.controllers.pendingProductReviewsController',
	'app.modules.product.controllers.productReviewManageController',
	'app.modules.product.controllers.activeProductReviewsController',
	'app.modules.product.controllers.topPurchasedProductsController',
	'app.modules.product.controllers.productTaxCodesController',
	'app.modules.product.controllers.promotionsController',
	'app.modules.product.controllers.promotionManageController',
	'app.modules.product.controllers.outOfStocksController',
	'app.modules.product.controllers.productCategoriesStatisticController',
	'app.modules.product.controllers.sendOutOfStockRequestsPopupController',
	'app.modules.product.controllers.manageProductsOrderController'
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
		        /*product categories*/
		        .state('index.oneCol.manageProductCategories', {
		            url: '/products/categories',
		            templateUrl: 'app/modules/product/partials/productCategoriesTreeView.html',
		            controller: 'productCategoriesController',
		        })
		        .state('index.oneCol.addNewProductCategory', {
		            url: '/products/categories/add?{categoryid:int}',
		            templateUrl: 'app/modules/product/partials/productCategoryDetail.html',
		            controller: 'productCategoryManageController'
		        })
		        .state('index.oneCol.productCategoryDetail', {
		            url: '/products/categories/{id:int}',
		            templateUrl: 'app/modules/product/partials/productCategoryDetail.html',
		            controller: 'productCategoryManageController'
		        })
                .state('index.oneCol.productCategoriesStatistic', {
                    url: '/report/category-sales-report',
                    templateUrl: 'app/modules/product/partials/productCategoriesStatistic.html',
                    controller: 'productCategoriesStatisticController'
                })
                /*inventory categories*/
		        .state('index.oneCol.manageInventoryCategories', {
		            url: '/products/inventorycategories',
		            templateUrl: 'app/modules/product/partials/inventoryCategoriesTreeView.html',
		            controller: 'inventoryCategoriesController',
		        })
		        /*products*/
		        .state('index.oneCol.manageProducts', {
		            url: '/products',
		            templateUrl: 'app/modules/product/partials/productsList.html',
		            controller: 'productsController'
		        })
		        .state('index.oneCol.addNewProduct', {
		            url: '/products/add?{type:int}',
		            templateUrl: 'app/modules/product/partials/productDetail.html',
		            controller: 'productManageController'
		        })
				.state('index.oneCol.productDetail', {
				    url: '/products/{id:int}',
				    templateUrl: 'app/modules/product/partials/productDetail.html',
				    controller: 'productManageController'
				})
		        .state('index.oneCol.manageProductTaxCodes', {
		            url: '/tools/producttaxcodes',
		            templateUrl: 'app/modules/product/partials/productTaxCodesList.html',
		            controller: 'productTaxCodesController'
		        })
		        /*discounts*/
		        .state('index.oneCol.manageDiscounts', {
		            url: '/discounts',
		            templateUrl: 'app/modules/product/partials/discountsList.html',
		            controller: 'discountsController'
		        })
		        .state('index.oneCol.addNewDiscount', {
		            url: '/discounts/add',
		            templateUrl: 'app/modules/product/partials/discountDetail.html',
		            controller: 'discountManageController'
		        })
				.state('index.oneCol.discountDetail', {
				    url: '/discounts/{id:int}',
				    templateUrl: 'app/modules/product/partials/discountDetail.html',
				    controller: 'discountManageController'
				})
            	/*product reviews*/
		        .state('index.oneCol.manageProductReviews', {
		            url: '/products/reviews',
		            templateUrl: 'app/modules/product/partials/productsWithReviewsList.html',
		            controller: 'productsWithReviewsController'
		        })
		        .state('index.oneCol.managePendingProductReviews', {
		            url: '/products/pendingreviews',
		            templateUrl: 'app/modules/product/partials/pendingProductReviewsList.html',
		            controller: 'pendingProductReviewsController'
		        })
		        .state('index.oneCol.productDetailManageReviews', {
		            url: '/products/{idproduct:int}/reviews',
		            templateUrl: 'app/modules/product/partials/activeProductReviewsList.html',
		            controller: 'activeProductReviewsController'
		        })
				.state('index.oneCol.productReviewDetail', {
				    url: '/products/reviews/{id:int}?{:idproduct:int}',
				    templateUrl: 'app/modules/product/partials/productReviewDetail.html',
				    controller: 'productReviewManageController'
				})
            	/*promotions*/
		        .state('index.oneCol.managePromotions', {
		            url: '/promotions',
		            templateUrl: 'app/modules/product/partials/promotionsList.html',
		            controller: 'promotionsController'
		        })
		        .state('index.oneCol.addNewPromotion', {
		            url: '/promotions/add',
		            templateUrl: 'app/modules/product/partials/promotionDetail.html',
		            controller: 'promotionManageController'
		        })
				.state('index.oneCol.promotionDetail', {
				    url: '/promotions/{id:int}',
				    templateUrl: 'app/modules/product/partials/promotionDetail.html',
				    controller: 'promotionManageController'
				})
                /*product out of stock requests*/
				.state('index.oneCol.outOfStocks', {
				    url: '/products/out-of-stock-request',
				    templateUrl: 'app/modules/product/partials/outOfStocksList.html',
				    controller: 'outOfStocksController'
				});
		}
]);