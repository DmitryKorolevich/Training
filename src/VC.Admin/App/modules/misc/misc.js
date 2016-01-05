'use strict';

angular.module('app.modules.misc',[
	'app.modules.misc.controllers.vitalGreenController',
	'app.modules.misc.controllers.catalogRequestsController',
	'app.modules.misc.controllers.redirectsController',
	'app.modules.misc.controllers.addEditRedirectController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
				.state('index.oneCol.vitalGreen', {
				    url: '/tools/vital-green',
				    templateUrl: 'app/modules/misc/partials/vitalGreenReport.html',
				    controller: 'vitalGreenController'
				})
		        .state('index.oneCol.catalogRequests', {
		            url: '/report/catalog-requests',
		            templateUrl: 'app/modules/misc/partials/catalogRequests.html',
		            controller: 'catalogRequestsController'
		        })
		        .state('index.oneCol.manageRedirect', {
		            url: '/tools/redirect',
		            templateUrl: 'app/modules/misc/partials/redirectsList.html',
		            controller: 'redirectsController'
		        });
		}
]);