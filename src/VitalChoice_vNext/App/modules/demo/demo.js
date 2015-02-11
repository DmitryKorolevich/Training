'use strict';

angular.module('app.modules.demo', [])
	.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

			$stateProvider
				.state('index.state1', {
					url: '/state1',
					templateUrl: 'app/modules/demo/partials/buttons.html'
				})
				.state('index.state2', {
					abstract: true,
					url: '/state2',
					template: '<ui-view/>'
				})
				.state('index.state2.childState1', {
					url: '/childState1',
					templateUrl: 'app/modules/demo/partials/containers.html'
				})
				.state('index.state2.childState2', {
					url: '/childState2',
					templateUrl: 'app/modules/demo/partials/dialogs.html'
				})
				.state('index.state2.childState3', {
					url: '/childState3',
					templateUrl: 'app/modules/demo/partials/forms.html'
				})
				.state('index.state2.childState4', {
					url: '/childState4',
					templateUrl: 'app/modules/demo/partials/heading.html'
				})
				.state('index.state2.childState5', {
					url: '/childState5',
					templateUrl: 'app/modules/demo/partials/indicators.html'
				})
				.state('index.state2.childState6', {
					url: '/childState6',
					templateUrl: 'app/modules/demo/partials/navs.html'
				})
				.state('index.state2.childState7', {
					url: '/childState7',
					templateUrl: 'app/modules/demo/partials/buttons.html'
				})
				.state('index.state2.childState8', {
					url: '/childState8',
					templateUrl: 'app/modules/demo/partials/indicators.html'
				})
				.state('index.state2.childState9', {
					url: '/childState9',
					templateUrl: 'app/modules/demo/partials/heading.html'
				})
				.state('index.state3', {
					url: '/state3',
					templateUrl: 'app/modules/demo/partials/indicators.html'
				})
				.state('index.state4', {
					abstract: true,
					url: '/state4',
					template: '<ui-view/>'
				})
				.state('index.state4.childState10', {
					url: '/childState10',
					templateUrl: 'app/modules/demo/partials/buttons.html'
				})
				.state('index.state4.childState11', {
					url: '/childState11',
					templateUrl: 'app/modules/demo/partials/containers.html'
				})
				.state('index.state5', {
					url: '/state5',
					templateUrl: 'app/modules/demo/partials/buttons.html'
				})
				.state('index.state6', {
					url: '/state6',
					templateUrl: 'app/modules/demo/partials/dialogs.html'
				});
		}
]);