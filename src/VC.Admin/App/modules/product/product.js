'use strict';

angular.module('app.modules.product', [
	'app.modules.product.controllers.productCategoriesController',
	'app.modules.product.controllers.productCategoryManageController',
	'app.modules.content.controllers.productsController',
	'app.modules.product.controllers.productManageController',
	'app.modules.product.controllers.addProductPopupController',
	'app.modules.content.controllers.discountsController',
	'app.modules.product.controllers.discountManageController',
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
				});
		}
]);