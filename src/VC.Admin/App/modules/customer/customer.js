'use strict';

angular.module('app.modules.customer', [
	'app.modules.customer.controllers.addCustomerController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {
			$stateProvider
				.state('index.oneCol.addCustomer', {
					url: '/customer/add',
					templateUrl: 'app/modules/customer/partials/addCustomer.html',
					controller: 'addCustomerController'
				});
		}
]);