﻿'use strict';

angular.module('app.modules.order',[
	'app.modules.order.controllers.ordersController',
	'app.modules.order.controllers.orderManageController',
	'app.modules.order.controllers.orderStatusUpdateController',
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
		            url: '/orders/add?{idcustomer:int}',
		            templateUrl: 'app/modules/order/partials/orderDetail.html',
		            controller: 'orderManageController'
		        })
		        .state('index.oneCol.changeOrderStatus', {
		            url: '/tools/change-order-status',
		            templateUrl: 'app/modules/order/partials/orderStatusUpdateForm.html',
		            controller: 'orderStatusUpdateController'
		        });
		}
]);