'use strict';

angular.module('app.modules.setting', [
	'app.modules.setting.controllers.logsController',
	'app.modules.setting.controllers.logDetailsController',
	'app.modules.setting.controllers.countriesController',
	'app.modules.setting.controllers.addEditCountryController',
	'app.modules.setting.controllers.addEditStateController',
	'app.modules.setting.controllers.settingsController',
	'app.modules.setting.controllers.paymentMethodsController',
	'app.modules.setting.controllers.orderNotesManagementController',
	'app.modules.setting.controllers.addEditOrderNoteController',
	'app.modules.setting.controllers.objectLogReportController',
	'app.modules.setting.controllers.objectHistorySectionController',
	'app.modules.setting.controllers.errorDetailsController',
	'app.modules.setting.controllers.lookupsController',
	'app.modules.setting.controllers.lookupDetailController',
	'app.modules.setting.controllers.profileScopesController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

			$stateProvider
				.state('index.oneCol.manageLogs', {
					url: '/settings/logs',
					templateUrl: 'app/modules/setting/partials/logsList.html',
					controller: 'logsController'
				})
				.state('index.oneCol.manageProfileScopes', {
				    url: '/settings/profile-scopes',
				    templateUrl: 'app/modules/setting/partials/profileScopesList.html',
				    controller: 'profileScopesController'
				})
				.state('index.oneCol.manageCountries', {
					url: '/settings/countries',
					templateUrl: 'app/modules/setting/partials/countries.html',
					controller: 'countriesController'
				})
				.state('index.oneCol.manageSettings', {
					url: '/settings',
					templateUrl: 'app/modules/setting/partials/settingsDetail.html',
					controller: 'settingsController'
				})
				.state('index.oneCol.managePaymentMethods', {
					url: '/settings/approvedpaymentmethods',
					templateUrl: 'app/modules/setting/partials/paymentMethodsList.html',
					controller: 'paymentMethodsController'
				})
				.state('index.oneCol.manageOrderNotes', {
					url: '/settings/ordernotes',
					templateUrl: 'app/modules/setting/partials/orderNotesList.html',
					controller: 'orderNotesManagementController'
				})
		    	/*lookups*/
		        .state('index.oneCol.manageLookups', {
		            url: '/settings/lookups',
		            templateUrl: 'app/modules/setting/partials/lookupsList.html',
		            controller: 'lookupsController',
		        })
		        .state('index.oneCol.lookupDetail', {
		            url: '/settings/lookups/{id:int}',
		            templateUrl: 'app/modules/setting/partials/lookupDetail.html',
		            controller: 'lookupDetailController',
		        });
		}
]);