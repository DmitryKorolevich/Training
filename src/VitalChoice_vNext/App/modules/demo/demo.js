'use strict';

angular.module('app.modules.demo', [])
	.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

			$stateProvider
				.state('index.oneCol.state1', {
					url: '/state1',
					templateUrl: 'app/modules/demo/partials/buttons.html'
				})
				.state('index.oneCol.state2', {
					abstract: true,
					url: '/state2',
					template: '<ui-view/>'
				})
				.state('index.oneCol.state2.childState1', {
					url: '/childState1',
					templateUrl: 'app/modules/demo/partials/containers.html'
				})
				.state('index.oneCol.state2.childState2', {
					url: '/childState2',
					templateUrl: 'app/modules/demo/partials/dialogs.html'
				})
				.state('index.oneCol.state2.childState3', {
					url: '/childState3',
					templateUrl: 'app/modules/demo/partials/forms.html'
				})
				.state('index.oneCol.state2.childState4', {
					url: '/childState4',
					templateUrl: 'app/modules/demo/partials/heading.html'
				})
				.state('index.oneCol.state2.childState5', {
					url: '/childState5',
					templateUrl: 'app/modules/demo/partials/indicators.html'
				})
				.state('index.oneCol.state2.childState6', {
					url: '/childState6',
					templateUrl: 'app/modules/demo/partials/navs.html'
				})
				.state('index.oneCol.state2.childState7', {
					url: '/childState7',
					templateUrl: 'app/modules/demo/partials/buttons.html'
				})
				.state('index.oneCol.state2.childState8', {
					url: '/childState8',
					templateUrl: 'app/modules/demo/partials/indicators.html'
				})
				.state('index.oneCol.state2.childState9', {
					url: '/childState9',
					templateUrl: 'app/modules/demo/partials/heading.html'
				})
				.state('index.twoCols.state3', {
					url: '/state3/:name',
					views:
					{
						'': {
							templateUrl: 'app/modules/demo/partials/indicators.html'
						},
						'left': {
							templateUrl: 'app/shared/menu/partials/sidebar.html',
							controller: 'sidebarController'
						}
					}

				})
				.state('index.twoCols.state3.sidebar1', {
					url: '/childStateSidebar1',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/dialogs.html'
						}
					}
				})
				.state('index.twoCols.state3.sidebar2', {
					url: '/childStateSidebar2',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/navs.html'
						}
					}
				})
				.state('index.twoCols.state3.sidebar3', {
					url: '/childStateSidebar3',
					views:
					{
						'@index.twoCols': {
							templateUrl: 'app/modules/demo/partials/buttons.html'
						}
					}
				})
				.state('index.oneCol.state4', {
					abstract: true,
					url: '/state4',
					template: '<ui-view/>'
				})
				.state('index.oneCol.state4.childState10', {
					url: '/childState10',
					templateUrl: 'app/modules/demo/partials/buttons.html'
				})
				.state('index.oneCol.state4.childState11', {
					url: '/childState11',
					templateUrl: 'app/modules/demo/partials/containers.html'
				})
				.state('index.oneCol.state5', {
					url: '/state5',
					templateUrl: 'app/modules/demo/partials/buttons.html'
				})
				.state('index.oneCol.state6', {
					url: '/state6',
					templateUrl: 'app/modules/demo/partials/dialogs.html'
				});
		}
]);