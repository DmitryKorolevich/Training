'use strict';

angular.module('app.modules.product', [
	'app.modules.product.controllers.productCategoriesController',
	'app.modules.product.controllers.productCategoryManageController',
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
		        });
		}
]);