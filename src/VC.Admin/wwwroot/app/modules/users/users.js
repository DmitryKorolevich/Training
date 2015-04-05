'use strict';

angular.module('app.modules.users', [
	'app.modules.users.controllers.addEditUserController',
	'app.modules.users.controllers.userManagementController'
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

			$stateProvider
				.state('index.oneCol.manageUsers', {
					url: '/users/manage',
					templateUrl: 'app/modules/users/partials/usersList.html',
					controller: 'userManagementController'
				});
		}
]);