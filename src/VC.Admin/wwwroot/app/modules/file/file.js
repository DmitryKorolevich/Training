'use strict';

angular.module('app.modules.file', [
	'app.modules.file.controllers.filesController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
				.state('index.oneCol.filesManagement', {
				    url: '/files',
				    templateUrl: 'app/modules/file/partials/filesManagement.html',
				    controller: 'filesController'
				});
		}
]);