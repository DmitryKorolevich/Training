﻿'use strict';

angular.module('app.modules.inventorysku', [
	'app.modules.inventorysku.controllers.inventorySkuCategoriesController',
	'app.modules.inventorysku.controllers.inventorySkuCategoryManageController',
	'app.modules.inventorysku.controllers.inventorySkusController',
	'app.modules.inventorysku.controllers.inventorySkuManageController',
	'app.modules.inventorysku.controllers.assignInventorySkusController',
	'app.modules.inventorysku.controllers.inventorySkusUsageReportController',
	'app.modules.inventorysku.controllers.inventoriesSummaryUsageReportController',
	'app.modules.inventorysku.controllers.skuInventoriesInfoController',
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
				})
                //reports
		        .state('index.oneCol.inventorySkusUsageReport', {
		            url: '/report/inventoryskus-usage',
		            templateUrl: 'app/modules/inventorysku/partials/inventorySkusUsageReport.html',
		            controller: 'inventorySkusUsageReportController'
		        })
		        .state('index.oneCol.inventoriesSummaryUsageReport', {
		            url: '/report/inventoryskus-usage-summary',
		            templateUrl: 'app/modules/inventorysku/partials/inventoriesSummaryUsageReport.html',
		            controller: 'inventoriesSummaryUsageReportController'
		        })
		        .state('index.oneCol.skuInventoriesInfoList', {
		            url: '/report/skuinventoriesinfo',
		            templateUrl: 'app/modules/inventorysku/partials/skuInventoriesInfoList.html',
		            controller: 'skuInventoriesInfoController'
		        });
		}
]);