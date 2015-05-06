'use strict';

angular.module('app.modules.profile', [
	'app.modules.profile.controllers.profileController'
])
.config([
	'$stateProvider', '$urlRouterProvider',
	function ($stateProvider, $urlRouterProvider) {

	    $stateProvider
			.state('index.oneCol.profile', {
			    url: '/profile/manage',
			    templateUrl: 'app/modules/profile/partials/profile.html',
			    controller: 'profileController'
			});
	}
]);