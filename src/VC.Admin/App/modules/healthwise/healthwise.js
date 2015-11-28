'use strict';

angular.module('app.modules.healthwise', [
	'app.modules.healthwise.controllers.healthwisePeriodsController',
	'app.modules.healthwise.controllers.markOrderController',
	'app.modules.healthwise.controllers.markCustomerOrdersController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider		        
				.state('index.oneCol.healthwisePeriods', {
				    url: '/report/healthwise-customers',
				    templateUrl: 'app/modules/healthwise/partials/healthwisePeriodsList.html',
				    controller: 'healthwisePeriodsController'
				});
		}
]);