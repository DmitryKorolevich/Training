'use strict';

angular.module('app.modules.demo', ['app.modules.demo.controllers.productListController', 'app.modules.demo.controllers.productDetailController'])
	.config([
		'$stateProvider', '$urlRouterProvider',
		function($stateProvider, $urlRouterProvider) {

			$stateProvider
				/*customer*/
				.state('index.oneCol.locateCustomer', {
					url: '/locateCustomer',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.addNewCustomer', {
					url: '/addNewCustomer',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				/*orders*/
				.state('index.oneCol.viewAllOrders', {
					url: '/locateCustomer',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.locateOrder', {
					url: '/addNewCustomer',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.placeNewOrder', {
					url: '/placeNewOrder',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				/*reports*/
				/*reports main*/
				.state('index.twoCols.demo1', {
					url: '/report/:name',
					views:
					{
						'': {
							templateUrl: 'app/modules/demo/partials/demoPage.html'
						},
						'left': {
							templateUrl: 'app/shared/menu/partials/sidebar.html',
							controller: 'sidebarController'
						}
					}
				}).state('index.twoCols.demo2', {
					url: '/report/:name',
					views:
					{
						'': {
							templateUrl: 'app/modules/demo/partials/demoPage.html'
						},
						'left': {
							templateUrl: 'app/shared/menu/partials/sidebar.html',
							controller: 'sidebarController'
						}
					}
				}).state('index.twoCols.demo3', {
					url: '/report/:name',
					views:
					{
						'': {
							templateUrl: 'app/modules/demo/partials/demoPage.html'
						},
						'left': {
							templateUrl: 'app/shared/menu/partials/sidebar.html',
							controller: 'sidebarController'
						}
					}
				}).state('index.twoCols.demo4', {
					url: '/report/:name',
					views:
					{
						'': {
							templateUrl: 'app/modules/demo/partials/demoPage.html'
						},
						'left': {
							templateUrl: 'app/shared/menu/partials/sidebar.html',
							controller: 'sidebarController'
						}
					}
				})
				.state('index.twoCols.demo5', {
					url: '/report/:name',
					views:
					{
						'': {
							templateUrl: 'app/modules/demo/partials/demoPage.html'
						},
						'left': {
							templateUrl: 'app/shared/menu/partials/sidebar.html',
							controller: 'sidebarController'
						}
					}
				})

				/*reports sidebar*/
				/*1*/
				.state('index.twoCols.demo1.child1', {
					url: '/child1',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Summary Sales Report</h2>'
						}
					}
				}).state('index.twoCols.demo1.child2', {
					url: '/child2',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Breakdown Report</h2>'
						}
					}
				}).state('index.twoCols.demo1.child3', {
					url: '/child3',
					views:
					{
						'@index.twoCols': {
							template: '<h2>SKU Breakdown Report</h2>'
						}
					}
				}).state('index.twoCols.demo1.child4', {
					url: '/child4',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Futures Breakdown Report</h2>'
						}
					}
				}).state('index.twoCols.demo1.child5', {
					url: '/child5',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Order SKU and Address Report</h2>'
						}
					}
				}).state('index.twoCols.demo1.child6', {
					url: '/child6',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Regional Sales Summary</h2>'
						}
					}
				}).state('index.twoCols.demo1.child7', {
					url: '/child7',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Deleted Orders Report</h2>'
						}
					}
				}).state('index.twoCols.demo1.child8', {
					url: '/child8',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Order SKU Counts</h2>'
						}
					}
				}).state('index.twoCols.demo1.child9', {
					url: '/child9',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Shipped Via Report</h2>'
						}
					}
				}).state('index.twoCols.demo1.child10', {
					url: '/child10',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Category Sales Report</h2>'
						}
					}
				}).state('index.twoCols.demo1.child11', {
					url: '/child11',
					views:
					{
						'@index.twoCols': {
							template: '<h2>Transaction & Refund Report</h2>'
						}
					}
				})
				/*2*/
				.state('index.twoCols.demo2.child1', {
					url: '/child1',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				/*3*/
				.state('index.twoCols.demo3.child1', {
					url: '/child1',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				.state('index.twoCols.demo3.child2', {
					url: '/child2',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				.state('index.twoCols.demo3.child3', {
					url: '/child3',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				/*4*/
				.state('index.twoCols.demo4.child1', {
					url: '/child1',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				.state('index.twoCols.demo4.child2', {
					url: '/child2',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				.state('index.twoCols.demo4.child3', {
					url: '/child3',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				.state('index.twoCols.demo4.child4', {
					url: '/child4',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				/*5*/
				.state('index.twoCols.demo5.child1', {
					url: '/child1',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				.state('index.twoCols.demo5.child2', {
					url: '/child2',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				.state('index.twoCols.demo5.child3', {
					url: '/child3',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				.state('index.twoCols.demo5.child4', {
					url: '/child4',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				.state('index.twoCols.demo5.child5', {
					url: '/child5',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				.state('index.twoCols.demo5.child6', {
					url: '/child6',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				.state('index.twoCols.demo5.child7', {
					url: '/child7',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html'
						}
					}
				})
				/*products*/
				.state('index.oneCol.locateProduct', {
					url: '/locateCustomer',
					templateUrl: 'app/modules/demo/partials/searchProducts.html'
				})
				.state('index.oneCol.productsList', {
					url: '/productslist',
					templateUrl: 'app/modules/demo/partials/products.html',
					controller: 'productListController'
				})
				.state('index.oneCol.productDetail', {
					url: '/productDetail/1',
					templateUrl: 'app/modules/demo/partials/productDetailAngular.html',
					controller: 'productDetailController'
				})
				.state('index.oneCol.addNewProduct', {
					url: '/addNewProduct',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.manageCategories', {
					url: '/manageCategories',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.manageProductReviews', {
					url: '/manageProductReviews',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				/*affiliates*/
				.state('index.oneCol.locateAffiliate', {
					url: '/locateAffiliate',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.addNewAffiliate', {
					url: '/addNewAffiliate',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				/*content*/
				.state('index.oneCol.managePages', {
					url: '/managePages',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.manageArticles', {
					url: '/manageArticles',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.manageRecipes', {
					url: '/manageRecipes',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.manageFaqs', {
					url: '/manageFaqs',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				/*tools*/
				.state('index.oneCol.productTaxCodes', {
					url: '/productTaxCodes',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.emailAddressProfiles', {
					url: '/emailAddressProfiles',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.reassignTransaction', {
					url: '/reassignTransaction',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.changeOrderStatus', {
					url: '/changeOrderStatus',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.healthWise', {
					url: '/healthWise',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				/*users*/
				.state('index.oneCol.manageUsers', {
					url: '/manageUsers',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				/*settings*/
				.state('index.oneCol.manageCountries', {
					url: '/manageCountries',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.manageStates', {
					url: '/manageStates',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.perishableCartThreshold', {
					url: '/perishableCartThreshold',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				/*help*/
				.state('index.oneCol.submitBug', {
					url: '/submitBug',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				})
				.state('index.oneCol.viewWiki', {
					url: '/viewWiki',
					templateUrl: 'app/modules/demo/partials/demoPage.html'
				});
			/*---------------------------------*/
		}
	]);