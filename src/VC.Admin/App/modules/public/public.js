'use strict';

angular.module('app.modules.public', [
	'app.modules.public.controllers.emailOrderManageController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
		        .state('index.oneCol.emailOrderDetail', {
		            url: '/public/email-order',
		            templateUrl: 'app/modules/public/partials/emailOrderDetail.html',
		            controller: 'emailOrderManageController'
		        });
		}
]);