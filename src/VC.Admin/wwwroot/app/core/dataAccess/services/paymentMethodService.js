'use strict';

angular.module('app.core.dataAccess.services.paymentMethodService', [])
.service('paymentMethodService', ['$http', function ($http) {
	var baseUrl = '/Api/PaymentMethod/';

	function getConfig(tracker) {
		var config = {  };
		if (tracker) {
			config.tracker = tracker;
		}
		return config;
	};

	return {
		getPaymentMethods: function(tracker) {
			return $http.get(baseUrl + 'GetPaymentMethods', getConfig(tracker));
		},
		setState: function(state, tracker) {
			return $http.post(baseUrl + 'SetState', state, getConfig(tracker));
		}
	};
}]);