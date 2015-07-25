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
		createAddressPrototype: function (tracker) {
			return $http.post(baseUrl + 'CreateAddressPrototype', null, getConfig(tracker));
		},
		createCustomerNotePrototype: function (tracker) {
			return $http.post(baseUrl + 'CreateCustomerNotePrototype', null, getConfig(tracker));
		},
		createUpdateCustomer: function (creatCustomerModel, tracker) {
			return $http.post(baseUrl + 'AddUpdateCustomer', creatCustomerModel, getConfig(tracker));
		},
		getCountries: function (tracker) {
			return $http.get(baseUrl + 'GetCountries', getConfig(tracker));
		},
		getExistingCustomer: function (id, tracker) {
			return $http.get(baseUrl + 'GetExistingCustomer/' + id, getConfig(tracker));
		},
		getPaymentMethods: function (customerType, tracker) {
			return $http.get(baseUrl + 'GetPaymentMethods/?customerType=' + customerType, getConfig(tracker));
		},
		getOrderNotes: function (customerType, tracker) {
			return $http.get(baseUrl + 'GetOrderNotes/?customerType=' + customerType, getConfig(tracker));
		},
		addAddress: function (address, id, tracker) {
		    return $http.post(baseUrl + 'AddAddress/?idCustomer=' + id, address, getConfig(tracker));
		},
		deleteAddress: function (id, tracker) {
		    return $http.post(baseUrl + 'DeleteAddress/?idAddress=' + id, getConfig(tracker));
		},
        addNote: function (note, id, tracker) {
            return $http.post(baseUrl + 'AddNote/?idCustomer=' + id, note, getConfig(tracker));
		},
		deleteNote: function (id, tracker) {
		    return $http.post(baseUrl + 'DeleteNote/?idNote=' + id, getConfig(tracker));
		}
	};
}]);