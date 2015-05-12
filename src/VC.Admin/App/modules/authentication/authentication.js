'use strict';

angular.module('app.modules.authentication', [
	'app.modules.authentication.controllers.loginController',
	'app.modules.authentication.controllers.activationController',
	'app.modules.authentication.controllers.resetPasswordController'
])
.config([
	'$stateProvider', '$urlRouterProvider',
	function ($stateProvider, $urlRouterProvider) {

	    $stateProvider
            .state('index.oneCol.resetPassword', {
            	url: '/authentication/passwordreset/{token}',
                templateUrl: 'app/modules/authentication/partials/resetPassword.html',
                controller: 'resetPasswordController'
            })
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