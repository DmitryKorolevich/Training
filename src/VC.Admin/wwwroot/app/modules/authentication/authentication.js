'use strict';

angular.module('app.modules.authentication', [
	'app.modules.authentication.controllers.loginController',
	'app.modules.authentication.controllers.activationController'
])
.config([
	'$stateProvider', '$urlRouterProvider',
	function ($stateProvider, $urlRouterProvider) {

		$stateProvider
			.state('index.oneCol.activation', {
				url: '/authentication/activate/{token}',
				templateUrl: 'app/modules/authentication/partials/activation.html',
				controller: 'activationController'
			})
			.state('index.oneCol.login', {
				url: '/authentication/login',
				templateUrl: 'app/modules/authentication/partials/login.html',
				controller: 'loginController'
			});
	}
]);