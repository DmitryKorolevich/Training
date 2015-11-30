'use strict';

angular.module('app.modules.healthwise', [
	'app.modules.healthwise.controllers.healthwisePeriodsController',
	'app.modules.healthwise.controllers.markOrderController',
	'app.modules.healthwise.controllers.markCustomerOrdersController',
	'app.modules.healthwise.controllers.healthwiseDetailController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider		        
				.state('index.oneCol.healthwisePeriods', {
				    url: '/report/healthwise-customers',
				    templateUrl: 'app/modules/healthwise/partials/healthwisePeriodsList.html',
				    controller: 'healthwisePeriodsController'
				})
            	.state('index.oneCol.healthwiseDetail', {
            	    url: '/report/healthwise/{id:int}',
            	    templateUrl: 'app/modules/healthwise/partials/healthwiseDetail.html',
            	    controller: 'healthwiseDetailController'
            	});
		}
]);