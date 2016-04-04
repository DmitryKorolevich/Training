﻿'use strict';

angular.module('app.modules.order', [
	'app.modules.order.services.orderEditService',
	'app.modules.order.controllers.ordersController',
	'app.modules.order.controllers.orderManageController',
	'app.modules.order.controllers.orderReshipManageController',
	'app.modules.order.controllers.orderRefundManageController',
	'app.modules.order.controllers.orderStatusUpdateController',
	'app.modules.order.controllers.moveOrderController',
	'app.modules.order.controllers.customerOrdersController',
	'app.modules.order.controllers.ordersRegionStatisticController',
	'app.modules.order.controllers.ordersRegionStatisticDetailController',
	'app.modules.order.controllers.sendOrderConfirmationController',
	'app.modules.order.controllers.sendOrderShippingConfirmationController',
	'app.modules.order.controllers.serviceCodesReportController',
	'app.modules.order.controllers.serviceCodeDetailController',
	'app.modules.order.controllers.customerAutoShipsController',
	'app.modules.order.controllers.manageAutoShipBillingController'
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
				.state('index.oneCol.manageOrders', {
				    url: '/orders',
				    templateUrl: 'app/modules/order/partials/ordersList.html',
				    controller: 'ordersController'
				})
		        .state('index.oneCol.orderDetail', {
		            url: '/orders/{id:int}',
		            templateUrl: 'app/modules/order/partials/orderDetail.html',
		            controller: 'orderManageController'
		        })
		        .state('index.oneCol.orderAdd', {
		            url: '/orders/add?{idcustomer:int}{idsource:int}',
		            templateUrl: 'app/modules/order/partials/orderDetail.html',
		            controller: 'orderManageController'
		        })
		        .state('index.oneCol.orderReshipDetail', {
		            url: '/orderreships/{id:int}',
		            templateUrl: 'app/modules/order/partials/orderReshipDetail.html',
		            controller: 'orderReshipManageController'
		        })
		        .state('index.oneCol.orderReshipAdd', {
		            url: '/orderreships/add?{idcustomer:int}{idsource:int}',
		            templateUrl: 'app/modules/order/partials/orderReshipDetail.html',
		            controller: 'orderReshipManageController'
		        })
		        .state('index.oneCol.orderRefundDetail', {
		            url: '/orderrefunds/{id:int}',
		            templateUrl: 'app/modules/order/partials/orderRefundDetail.html',
		            controller: 'orderRefundManageController'
		        })
		        .state('index.oneCol.orderRefundAdd', {
		            url: '/orderrefunds/add?{idcustomer:int}{idsource:int}',
		            templateUrl: 'app/modules/order/partials/orderRefundDetail.html',
		            controller: 'orderRefundManageController'
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
		        .state('index.oneCol.ordersRegionStatisticDetail', {
		            url: '/report/regional-sales-details?{from:string}{to:string}{idcustomertype:int}{idordertype:int}{region:string}{zip:string}',
		            templateUrl: 'app/modules/order/partials/ordersRegionStatisticDetail.html',
		            controller: 'ordersRegionStatisticDetailController'
		        })
				.state('index.oneCol.serviceCodesStatistic', {
				    url: '/report/service-codes',
				    templateUrl: 'app/modules/order/partials/serviceCodesReport.html',
				    controller: 'serviceCodesReportController'
				})
		        .state('index.oneCol.serviceCodeDetail', {
		            url: '/report/service-codes/{id:int}?{from:string}{to:string}',
		            templateUrl: 'app/modules/order/partials/serviceCodeDetail.html',
		            controller: 'serviceCodeDetailController'
		        });
		}
]);