'use strict';

angular.module('app.shared', [
		'app.shared.menu',
		'app.shared.area',
		'app.shared.layout'
	])
	.config([
		'$stateProvider', '$urlRouterProvider',
		function($stateProvider, $urlRouterProvider) {

			$stateProvider
				.state('index', {
					url: '',
					views: {
						'': {
							templateUrl: 'app/shared/layout/partials/index.html',
							controller: 'indexController'
						},
						'topMenu@index': {
							templateUrl: 'app/shared/menu/partials/mainMenu.html',
							controller: 'mainNavigationController'
						},
						'leftPanel@index': {
							templateUrl: 'app/shared/area/partials/leftPanel.html',
							controller: 'leftPanelController'
						},
						'workingPanel@index': {
							templateUrl: 'app/shared/area/partials/workingPanel.html',
							controller: 'workingPanelController'
						}
					}
				});
		}
	]);