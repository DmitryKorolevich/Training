﻿'use strict';

angular.module('app.modules.authentication', [
	'app.modules.authentication.controllers.loginController',
	'app.modules.authentication.controllers.activationController'
])
.config([
	'$stateProvider', '$urlRouterProvider',
	function ($stateProvider, $urlRouterProvider) {

		$stateProvider
			.state('index.oneCol.activation', {
				url: '/authentication/activate/demotoken',
				templateUrl: 'app/modules/authentication/partials/activation.html',
				controller: 'activationController'
			});
	}
]);