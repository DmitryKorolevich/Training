'use strict';

angular.module('app.modules.gc', [
	'app.modules.gc.controllers.gcsController',
	'app.modules.gc.controllers.sendEmailController',
	'app.modules.gc.controllers.gcDetailController',
	'app.modules.gc.controllers.gcsAddController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
				.state('index.oneCol.manageGCs', {
				    url: '/gcs',
				    templateUrl: 'app/modules/gc/partials/gcsList.html',
				    controller: 'gcsController'
				})
		        .state('index.oneCol.gcDetail', {
		            url: '/gcs/{id:int}',
		            templateUrl: 'app/modules/gc/partials/gcDetail.html',
		            controller: 'gcDetailController'
		        })
		        .state('index.oneCol.gcsAdd', {
		            url: '/gcs/add',
		            templateUrl: 'app/modules/gc/partials/gcsAdd.html',
		            controller: 'gcsAddController'
		        }); 
		}
]);