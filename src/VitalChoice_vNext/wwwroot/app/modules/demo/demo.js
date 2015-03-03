'use strict';

angular.module('app.modules.demo',
	[
		'app.modules.demo.controllers.productListController',
		'app.modules.demo.controllers.productDetailController',
		'app.modules.demo.controllers.customerListController',
		'app.modules.demo.controllers.customerDetailController'
	])
	.config([
		'$stateProvider', '$urlRouterProvider',
		function($stateProvider, $urlRouterProvider) {

			$stateProvider
				/*customer*/
				.state('index.oneCol.locateCustomer', {
					url: '/customers/locate',
					templateUrl: 'app/modules/demo/partials/searchCustomer.html'
				})
				.state('index.oneCol.addNewCustomer', {
					url: '/customers/add',
					templateUrl: 'app/modules/demo/partials/addNewCustomer.html'
				})
				.state('index.oneCol.customersList', {
					url: '/customers',
					templateUrl: 'app/modules/demo/partials/customers.html',
					controller: 'customerListController'
				})
				.state('index.oneCol.customerDetail', {
					url: '/customers/detail/1',
					templateUrl: 'app/modules/demo/partials/customerDetail.html',
					controller: 'customerDetailController'
				})
				/*orders*/
				.state('index.oneCol.viewAllOrders', {
					url: '/orders',
					template: '<h2>View Add Orders</h2>'
				})
				.state('index.oneCol.locateOrder', {
					url: '/orders/locate',
					template: '<h2>Locate Order</h2>'
				})
				.state('index.oneCol.placeNewOrder', {
					url: '/orders/place',
					template: '<h2>Place New Order</h2>'
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
				/*products*/
				.state('index.oneCol.locateProduct', {
					url: '/products/locate',
					templateUrl: 'app/modules/demo/partials/searchProducts.html'
				})
				.state('index.oneCol.productsList', {
					url: '/products',
					templateUrl: 'app/modules/demo/partials/products.html',
					controller: 'productListController'
				})
				.state('index.oneCol.productDetail', {
					url: '/products/detail/1',
					templateUrl: 'app/modules/demo/partials/productDetailAngular.html',
					controller: 'productDetailController'
				})
				.state('index.oneCol.addNewProduct', {
					url: '/products/add',
					template: '<h2>Add New Product</h2>'
				})
				.state('index.oneCol.manageCategories', {
					url: '/products/manageCategories',
					template: '<h2>Manage Categories</h2>'
				})
				.state('index.oneCol.manageProductReviews', {
					url: '/products/manageProductReviews',
					template: '<h2>Manage Product Reviews</h2>'
				})
				/*affiliates*/
				.state('index.oneCol.locateAffiliate', {
					url: '/affiliates/locate',
					template: '<h2>Locate Affiliate</h2>'
				})
				.state('index.oneCol.addNewAffiliate', {
					url: '/affiliates/add',
					template: '<h2>Add New Affiliate</h2>'
				})
				/*content*/
				.state('index.oneCol.managePages', {
					url: '/content/managePages',
					template: '<h2>Manage Pages</h2>'
				})
				.state('index.oneCol.manageArticles', {
					url: '/content/manageArticles',
					template: '<h2>Manage Articles</h2>'
				})
				.state('index.oneCol.manageRecipes', {
					url: '/content/manageRecipes',
					template: '<h2>Manage Recipes</h2>'
				})
				.state('index.oneCol.manageFaqs', {
					url: '/content/manageFaqs',
					template: '<h2>Manage FAQs</h2>'
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
				.state('index.oneCol.reassignTransaction', {
					url: '/tools/reassignTransaction',
					template: '<h2>Reassign Transaction</h2>'
				})
				.state('index.oneCol.changeOrderStatus', {
					url: '/tools/changeOrderStatus',
					template: '<h2>Change Order Status</h2>'
				})
				.state('index.oneCol.healthWise', {
					url: '/tools/healthWise',
					template: '<h2>HealthWise</h2>'
				})
				/*users*/
				.state('index.oneCol.manageUsers', {
					url: '/users/manage',
					template: '<h2>Manage Users</h2>'
				})
				/*settings*/
				.state('index.oneCol.manageCountries', {
					url: '/settings/manageCountries',
					template: '<h2>Manage Countries</h2>'
				})
				.state('index.oneCol.manageStates', {
					url: '/settings/manageStates',
					template: '<h2>Manage States</h2>'
				})
				.state('index.oneCol.perishableCartThreshold', {
					url: '/settings/perishableCartThreshold',
					template: '<h2>Perishable Cart Threshold</h2>'
				})
				/*help*/
				.state('index.oneCol.submitBug', {
					url: '/help/submitBug',
					template: '<h2>Submit Bug</h2>'
				})
				.state('index.oneCol.viewWiki', {
					url: '/help/viewWiki',
					template: '<h2>View Wiki</h2>'
				});
			/*---------------------------------*/
		}
	]);