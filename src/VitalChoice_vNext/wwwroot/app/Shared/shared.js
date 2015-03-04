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
					abstract: true,
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
						'workingPanel@index': {
							templateUrl: 'app/shared/area/partials/workingPanel.html',
							controller: 'workingPanelController'
						}
					}
				})
				.state('index.oneCol', {
					abstract: true,
					url: '',
					templateUrl: 'app/shared/area/partials/oneColumn.html'
				}).state('index.twoCols', {
					abstract: true,
					url: '',
					templateUrl: 'app/shared/area/partials/twoColumn.html'
				});;
		}
	]);