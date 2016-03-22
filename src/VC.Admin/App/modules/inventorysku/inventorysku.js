'use strict';

angular.module('app.modules.inventorysku', [
	'app.modules.inventorysku.controllers.inventorySkuCategoriesController',
	'app.modules.inventorysku.controllers.inventorySkuCategoryManageController',
	'app.modules.inventorysku.controllers.inventorySkusController',
	'app.modules.inventorysku.controllers.inventorySkuManageController',
	'app.modules.inventorysku.controllers.assignInventorySkusController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
                /*inventory categories*/
		        .state('index.oneCol.manageInventorySkuCategories', {
		            url: '/inventoryskus/settings/categories',
		            templateUrl: 'app/modules/inventorysku/partials/inventorySkuCategoriesTreeView.html',
		            controller: 'inventorySkuCategoriesController',
		        })
		        /*inventory skus*/
		        .state('index.oneCol.manageInventorySkus', {
		            url: '/inventoryskus',
		            templateUrl: 'app/modules/inventorysku/partials/inventorySkusList.html',
		            controller: 'inventorySkusController'
		        })
		        .state('index.oneCol.addNewInventorySku', {
		            url: '/inventoryskus/add',
		            templateUrl: 'app/modules/inventorysku/partials/inventorySkuDetail.html',
		            controller: 'inventorySkuManageController'
		        })
				.state('index.oneCol.inventorySkuDetail', {
				    url: '/inventoryskus/{id:int}',
				    templateUrl: 'app/modules/inventorysku/partials/inventorySkuDetail.html',
				    controller: 'inventorySkuManageController'
				});

		}
]);