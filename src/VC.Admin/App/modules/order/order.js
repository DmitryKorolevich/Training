'use strict';

angular.module('app.modules.order',[
	'app.modules.order.controllers.ordersController',
	'app.modules.order.controllers.orderManageController',
	'app.modules.order.controllers.orderStatusUpdateController',
	'app.modules.order.controllers.moveOrderController',
	'app.modules.order.controllers.customerOrdersController',
	'app.modules.order.controllers.ordersRegionStatisticController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
				.state('index.oneCol.manageOrders',{
				    url: '/orders',
				    templateUrl: 'app/modules/order/partials/ordersList.html',
				    controller: 'ordersController'
				})
		        .state('index.oneCol.orderDetail',{
		            url: '/orders/{id:int}',
		            templateUrl: 'app/modules/order/partials/orderDetail.html',
		            controller: 'orderManageController'
		        })
		        .state('index.oneCol.orderAdd',{
		            url: '/orders/add?{idcustomer:int}{idsource:int}',
		            templateUrl: 'app/modules/order/partials/orderDetail.html',
		            controller: 'orderManageController'
		        })
		        .state('index.oneCol.changeOrderStatus', {
		            url: '/tools/change-order-status',
		            templateUrl: 'app/modules/order/partials/orderStatusUpdateForm.html',
		            controller: 'orderStatusUpdateController'
		        })
		        .state('index.oneCol.moveOrder', {
		            url: '/tools/move-order',
		            templateUrl: 'app/modules/order/partials/moveOrderForm.html',
		            controller: 'moveOrderController'
		        })
		        .state('index.oneCol.ordersRegionStatistic', {
		            url: '/report/regional-sales',
		            templateUrl: 'app/modules/order/partials/ordersRegionStatistic.html',
		            controller: 'ordersRegionStatisticController'
		        })
		        .state('index.oneCol.ordersRegionStatisticDetailByRegion', {
		            url: '/report/regional-sales-details?{region:string}',
		            templateUrl: 'app/modules/order/partials/ordersRegionStatisticDetail.html',
		            controller: 'ordersRegionStatisticDetailController'
		        })
		        .state('index.oneCol.ordersRegionStatisticDetailByZip', {
		            url: '/report/regional-sales-details?{zip:string}',
		            templateUrl: 'app/modules/order/partials/ordersRegionStatisticDetail.html',
		            controller: 'ordersRegionStatisticDetailController'
		        });
		}
]);