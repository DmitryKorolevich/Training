'use strict';

angular.module('app.modules.affiliate', [
	'app.modules.affiliate.controllers.affiliatesController',
	'app.modules.affiliate.controllers.affiliateManageController',
	'app.modules.affiliate.controllers.affiliateSendEmailController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
				.state('index.oneCol.manageAffiliates', {
				    url: '/affiliates',
				    templateUrl: 'app/modules/affiliate/partials/affiliatesList.html',
				    controller: 'affiliatesController'
				})
		        .state('index.oneCol.affiliateDetail', {
		            url: '/affiliates/{id:int}',
		            templateUrl: 'app/modules/affiliate/partials/affiliateDetail.html',
		            controller: 'affiliateManageController'
		        })
		        .state('index.oneCol.affiliateAdd', {
		            url: '/affiliates/add',
		            templateUrl: 'app/modules/affiliate/partials/affiliateDetail.html',
		            controller: 'affiliateManageController'
		        }); 
		}
]);