'use strict';

angular.module('app.core.dataAccess.services.customerService', [])
.service('customerService', ['$http', function ($http) {
	var baseUrl = '/Api/Customer/';

	function getConfig(tracker) {
		var config = {};
		if (tracker) {
			config.tracker = tracker;
		}
		return config;
	};

	return {
		getCustomers: function (filter, tracker) {
			return $http.post(baseUrl + 'GetCustomers', filter, getConfig(tracker));
		},
		createCustomerPrototype: function (tracker) {
			return $http.post(baseUrl + 'CreateCustomerPrototype', null, getConfig(tracker));
		},
		createUpdateCustomer: function (creatCustomerModel, tracker) {
			return $http.post(baseUrl + 'AddUpdateCustomer', creatCustomerModel, getConfig(tracker));
		},
		getCountries: function (tracker) {
			return $http.get(baseUrl + 'GetCountries', getConfig(tracker));
		},
		getPaymentMethods: function (customerType, tracker) {
			return $http.get(baseUrl + 'GetPaymentMethods/?customerType=' + customerType, getConfig(tracker));
		},
		getOrderNotes: function (customerType, tracker) {
			return $http.get(baseUrl + 'GetOrderNotes/?customerType=' + customerType, getConfig(tracker));
		}
	};
}]);