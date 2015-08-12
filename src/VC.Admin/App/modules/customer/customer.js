'use strict';

angular.module('app.modules.customer', [
	'app.modules.customer.services.customerEditService',
	'app.modules.customer.controllers.addEditCustomerController',
	'app.modules.customer.controllers.customerManagementController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function($stateProvider, $urlRouterProvider) {
			$stateProvider
				.state('index.oneCol.addCustomer', {
					url: '/customer/add',
					templateUrl: 'app/modules/customer/partials/manageCustomer.html',
					controller: 'addEditCustomerController'
				})
				.state('index.oneCol.manageCustomers', {
					url: '/customers',
					templateUrl: 'app/modules/customer/partials/customersList.html',
					controller: 'customerManagementController'
				})
				.state('index.oneCol.customerDetail', {
					url: '/customers/{id:int}',
					templateUrl: 'app/modules/customer/partials/manageCustomer.html',
					controller: 'addEditCustomerController'
				});
		}
	]);