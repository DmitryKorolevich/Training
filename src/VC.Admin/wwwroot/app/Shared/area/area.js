'use strict';

angular.module('app.shared.area', [
	'app.shared.area.controllers.workingPanelController',
	'app.shared.area.controllers.leftPanelController'
]).config([
	'$stateProvider', '$urlRouterProvider',
	function($stateProvider, $urlRouterProvider) {

		$stateProvider
			.state('index.notFound', {
				url: '/404',
				templateUrl: 'app/shared/area/partials/404.html'
			});
	}
]);