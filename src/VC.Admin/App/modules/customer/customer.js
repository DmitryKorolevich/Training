'use strict';

angular.module('app.modules.customer', [
	'app.modules.customer.services.customerEditService',
	'app.modules.customer.controllers.addEditCustomerController',
	'app.modules.customer.controllers.customerManagementController',
	'app.modules.customer.controllers.manageCustomerFilesController',
	'app.modules.customer.controllers.manageCustomerNotesController',
	'app.modules.customer.controllers.wholesaleSummaryReportController',
	'app.modules.customer.controllers.wholesalesReportController',
	'app.modules.customer.controllers.mailingReportController',
	'app.modules.customer.controllers.searchCustomersController',
	'app.modules.customer.controllers.mergeCustomersController',
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
				})
            	.state('index.oneCol.wholesaleSummaryReport', {
            	    url: '/report/wholesale-summary',
            	    templateUrl: 'app/modules/customer/partials/wholesaleSummaryReport.html',
					controller: 'wholesaleSummaryReportController'
            	})
                .state('index.oneCol.wholesalesListReport', {
                    url: '/report/wholesale-list',
                    templateUrl: 'app/modules/customer/partials/wholesalesReport.html',
			        controller: 'wholesalesReportController'
                })
                .state('index.oneCol.mailingReport', {
                    url: '/report/mailing-list',
                    templateUrl: 'app/modules/customer/partials/mailingReport.html',
                    controller: 'mailingReportController'
                })
		        .state('index.oneCol.mergeCustomers', {
		            url: '/tools/merge-customers',
		            templateUrl: 'app/modules/customer/partials/mergeCustomers.html',
		            controller: 'mergeCustomersController'
		        });
		}
	]);