'use strict';

angular.module('app.shared', [
	'app.shared.menu',
	'app.shared.area',
	'app.shared.layout'
]);
//.config([
//	'$stateProvider', '$urlRouterProvider',
//	function($stateProvider, $urlRouterProvider) {

//		$urlRouterProvider.otherwise('app/shared/partials/404.html');

//		$stateProvider
//			.state('index', {
//				url: '/',
//				views: {
//					'@': {
//						templateUrl: 'index.html',
//						controller: 'indexController'
//					},
//					'topMenu': {
//						templateUrl: 'app/shared/menu/partials/mainMenu.html',
//						controller: 'mainNavigationController'
//					},
//					'sidebar': {
//						templateUrl: 'app/shared/menu/partials/sidebar.html',
//						controller: 'sidebarController'
//					},
//					'workingPanel': {
//						templateUrl: 'app/shared/area/workingPanel.html',
//						controller: 'workingPanelController'
//					}
//				}
//			});
//	}
//]);