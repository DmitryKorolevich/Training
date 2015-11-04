﻿'use strict';

angular.module('app.modules.affiliate', [
	'app.modules.affiliate.controllers.affiliatesController',
	'app.modules.affiliate.controllers.affiliateManageController',
	'app.modules.affiliate.controllers.affiliateSendEmailController',
	'app.modules.affiliate.controllers.customersInAffiliatesListController',
	'app.modules.affiliate.controllers.customersInAffiliatesReportController',
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
		        })
		        //affiliate customer reports
		        .state('index.oneCol.customersInAffiliatesList', {
		            url: '/report/affiliates',
		            templateUrl: 'app/modules/affiliate/partials/customersInAffiliatesList.html',
		            controller: 'customersInAffiliatesListController'
		        })
		        .state('index.oneCol.customersInAffiliatesListWithId', {
		            url: '/report/affiliates/{id:int}',
		            templateUrl: 'app/modules/affiliate/partials/customersInAffiliatesList.html',
		            controller: 'customersInAffiliatesListController'
		        })
            	.state('index.oneCol.customersInAffiliatesReport', {
            	    url: '/report/affiliates-customers',
            	    templateUrl: 'app/modules/affiliate/partials/customersInAffiliatesReport.html',
            	    controller: 'customersInAffiliatesReportController'
            	});
		}
]);