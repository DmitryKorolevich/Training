'use strict';

angular.module('app.modules.setting', [
	'app.modules.setting.controllers.logsController',
	'app.modules.setting.controllers.logDetailsController',
	'app.modules.setting.controllers.countriesController',
	'app.modules.setting.controllers.addEditCountryController',
	'app.modules.setting.controllers.addEditStateController',
	'app.modules.setting.controllers.settingsController',
	'app.modules.setting.controllers.paymentMethodsController'
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
				});
		}
]);