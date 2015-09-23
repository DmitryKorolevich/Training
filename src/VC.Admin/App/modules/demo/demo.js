'use strict';

angular.module('app.modules.demo',
	[
		'app.modules.demo.controllers.customerListController',
		'app.modules.demo.controllers.customerDetailController',
		'app.modules.demo.controllers.dashboardController',
		'app.modules.demo.controllers.orderDetailController',
		'app.modules.demo.controllers.modalAddSubProductController',
		'app.modules.demo.controllers.manageRecipesController',
		'app.modules.demo.controllers.uploadController',
	])
	.config([
		'$stateProvider', '$urlRouterProvider',
		function($stateProvider, $urlRouterProvider) {

			$stateProvider
				/*dashboard*/
				.state('index.oneCol.dashboard', {
					url: '',
					templateUrl: 'app/modules/demo/partials/dashboard.html',
					controller: 'dashboardController'
				})
				/*orders*/
				.state('index.oneCol.locateOrder', {
					url: '/orders/locate',
					template: '<h2>Locate Order</h2>'
				})
				.state('index.oneCol.placeNewOrder', {
					url: '/orders/place',
					templateUrl: 'app/modules/demo/partials/addNewOrder.html',
					controller: 'orderDetailController'
				})
				/*reports*/
				/*reports main*/
				.state('index.twoCols.salesOrders', {
					abstract: true,
					url: '/reports/:name',
					views:
					{
						'left': {
							templateUrl: 'app/shared/menu/partials/sidebar.html',
							controller: 'sidebarController'
						}
					}
				})
				.state('index.twoCols.wholesale', {
					abstract: true,
					url: '/reports/:name',
					views:
					{
						'left': {
							templateUrl: 'app/shared/menu/partials/sidebar.html',
							controller: 'sidebarController'
						}
					}
				})
				.state('index.oneCol.affiliates', {
					url: '/report/:name',
					template: '<h2>Affiliates</h2>'
				}).state('index.twoCols.operations', {
					abstract: true,
					url: '/report/:name',
					views:
					{
						'left': {
							templateUrl: 'app/shared/menu/partials/sidebar.html',
							controller: 'sidebarController'
						}
					}
				})
				.state('index.oneCol.listProcessingAnalysis', {
					url: '/report/:name',
					template: '<h2>List Processing, Analysis</h2>'
				})

				/*reports sidebar*/
				/*1*/
				.state('index.twoCols.salesOrders.healthWise', {
					url: '/healthWise',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Health Wise</h2>'
						}
					}
				}).state('index.twoCols.salesOrders.orderStatusHistory', {
					url: '/orderStatusHistory',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Order Status History</h2>'
						}
					}
				}).state('index.twoCols.salesOrders.ooStockRequests', {
					url: '/ooStockRequests',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Out of Stock Requests</h2>'
						}
					}
				})
				/*2*/
				.state('index.twoCols.wholesale.wholesaleSummary', {
					url: '/wholesaleSummary',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Wholesale Summary Report</h2>'
						}
					}
				})
				.state('index.twoCols.wholesale.wholesaleDropShipOrders', {
					url: '/wholesaleDropShipOrders',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Wholesale Drop Ship Orders Report</h2>'
						}
					}
				})
				/*4*/
				.state('index.twoCols.operations.vitalGreen', {
					url: '/vitalGreen',
					views:
					{
						'@index.twoCols': {
							template: '<h2>VitalGreen</h2>'
						}
					}
				})
				/*content*/
				.state('index.oneCol.manageRecipesTemp', { 
					url: '/content/draggablecategories',
					templateUrl: 'app/modules/demo/partials/manageRecipes.html',
					controller: 'manageRecipesController'
				})
				/*tools*/
				.state('index.oneCol.productTaxCodes', {
					url: '/tools/productTaxCodes',
					template: '<h2>Product Tax Codes</h2>'
				})
				.state('index.oneCol.emailAddressProfiles', {
					url: '/tools/emailAddressProfiles',
					template: '<h2>Multiple Email Address Profiles</h2>'
				})
				.state('index.oneCol.healthWise', {
					url: '/tools/healthWise',
					template: '<h2>HealthWise</h2>'
				})
				/*settings*/
				.state('index.oneCol.perishableCartThreshold', {
					url: '/settings/perishableCartThreshold',
					template: '<h2>Perishable Cart Threshold</h2>'
				})
				/*help*/
				.state('index.oneCol.upload', {
					url: '/help/upload',
					templateUrl: 'app/modules/demo/partials/upload.html',
					controller: 'uploadController'
				})
				.state('index.oneCol.viewWiki', {
					url: '/help/viewWiki',
					template: '<h2>View Wiki</h2>'
				});
			/*---------------------------------*/
		}
	]);