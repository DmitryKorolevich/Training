'use strict';

angular.module('app.modules.demo', ['app.modules.demo.controllers.productListController'])
	.config([
		'$stateProvider', '$urlRouterProvider',
		function($stateProvider, $urlRouterProvider) {

			$stateProvider
				.state('index.oneCol.demo1', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				})
				.state('index.oneCol.demo2', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				})
				.state('index.oneCol.demo3', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				})
				.state('index.oneCol.demo4', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				}).state('index.oneCol.demo5', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				}).state('index.oneCol.demo6', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				}).state('index.oneCol.demo7', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				})
				.state('index.oneCol.demo8', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				}).state('index.oneCol.demo9', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				}).state('index.oneCol.demo10', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				}).state('index.oneCol.demo11', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				}).state('index.oneCol.demo12', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				}).state('index.oneCol.demo13', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				}).state('index.oneCol.demo14', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				}).state('index.oneCol.demo15', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPage.html',
				})
				.state('index.twoCols.demo', {
					url: '/demo',
					templateUrl: 'app/modules/demo/partials/demoPageWithSidebar.html',
				})
				.state('index.oneCol.locate', {
					url: '/products/locate',
					templateUrl: 'app/modules/demo/partials/searchProducts.html',
				})
				.state('index.oneCol.list', {
					url: '/products/list',
					templateUrl: 'app/modules/demo/partials/products.html',
					controller: 'productListController'
				})
				.state('index.oneCol.detail', {
					url: '/products/detail/1',
					templateUrl: 'app/modules/demo/partials/productDetailAngular.html',
				})
				.state('index.oneCol.state1', {
					url: '/state1',
					templateUrl: 'app/modules/demo/partials/buttons.html'
				})
				.state('index.oneCol.state2', {
					abstract: true,
					url: '/state2',
					template: '<ui-view/>'
				})
				.state('index.oneCol.state2.childState1', {
					url: '/childState1',
					templateUrl: 'app/modules/demo/partials/containers.html'
				})
				.state('index.oneCol.state2.childState2', {
					url: '/childState2',
					templateUrl: 'app/modules/demo/partials/dialogs.html'
				})
				.state('index.oneCol.state2.childState3', {
					url: '/childState3',
					templateUrl: 'app/modules/demo/partials/forms.html'
				})
				.state('index.oneCol.state2.childState4', {
					url: '/childState4',
					templateUrl: 'app/modules/demo/partials/heading.html'
				})
				.state('index.oneCol.state2.childState5', {
					url: '/childState5',
					templateUrl: 'app/modules/demo/partials/indicators.html'
				})
				.state('index.oneCol.state2.childState6', {
					url: '/childState6',
					templateUrl: 'app/modules/demo/partials/navs.html'
				})
				.state('index.oneCol.state2.childState7', {
					url: '/childState7',
					templateUrl: 'app/modules/demo/partials/buttons.html'
				})
				.state('index.oneCol.state2.childState8', {
					url: '/childState8',
					templateUrl: 'app/modules/demo/partials/indicators.html'
				})
				.state('index.oneCol.state2.childState9', {
					url: '/childState9',
					templateUrl: 'app/modules/demo/partials/heading.html'
				})
				.state('index.twoCols.state3', {
					url: '/state3/:name',
					views:
					{
						'': {
							templateUrl: 'app/modules/demo/partials/indicators.html'
						},
						'left': {
							templateUrl: 'app/shared/menu/partials/sidebar.html',
							controller: 'sidebarController'
						}
					}

				})
				.state('index.twoCols.state3.sidebar1', {
					url: '/childStateSidebar1',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/dialogs.html'
						}
					}
				})
				.state('index.twoCols.state3.sidebar2', {
					url: '/childStateSidebar2',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/navs.html'
						}
					}
				})
				.state('index.twoCols.state3.sidebar3', {
					url: '/childStateSidebar3',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/buttons.html'
						}
					}
				})
				.state('index.oneCol.state4', {
					abstract: true,
					url: '/state4',
					template: '<ui-view/>'
				})
				.state('index.oneCol.state4.childState10', {
					url: '/childState10',
					templateUrl: 'app/modules/demo/partials/buttons.html'
				})
				.state('index.oneCol.state4.childState11', {
					url: '/childState11',
					templateUrl: 'app/modules/demo/partials/containers.html'
				})
				.state('index.oneCol.state5', {
					url: '/state5',
					templateUrl: 'app/modules/demo/partials/buttons.html'
				})
				.state('index.oneCol.state6', {
					url: '/state6',
					templateUrl: 'app/modules/demo/partials/dialogs.html'
				})

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
				});
		}
	]);