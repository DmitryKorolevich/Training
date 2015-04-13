'use strict';

angular.module('app.modules.setting', [
	'app.modules.setting.controllers.logsController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

			$stateProvider
				.state('index.oneCol.manageLogs', {
					url: '/setting/logs',
					templateUrl: 'app/modules/setting/partials/logsList.html',
					controller: 'logsController'
				})
		}
]);