'use strict';

angular.module('app.modules.inventorysku', [
	'app.modules.inventorysku.controllers.inventorySkuCategoriesController',
	'app.modules.inventorysku.controllers.inventorySkuCategoryManageController',
	'app.modules.inventorysku.controllers.lookupsController',
	'app.modules.inventorysku.controllers.lookupDetailController',
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
		        /*lookups*/
		        .state('index.oneCol.manageInventorySkuLookups', {
		            url: '/inventoryskus/settings/lookups',
		            templateUrl: 'app/modules/inventorysku/partials/lookupsList.html',
		            controller: 'lookupsController',
		        })
		        .state('index.oneCol.lookupDetail', {
		            url: '/inventoryskus/settings/lookups/{id:int}',
		            templateUrl: 'app/modules/inventorysku/partials/lookupDetail.html',
		            controller: 'lookupDetailController',
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