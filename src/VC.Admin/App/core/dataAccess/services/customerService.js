﻿'use strict';

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
		createCreditCardPrototype: function (tracker) {
		    return $http.post(baseUrl + 'CreateCreditCardPrototype', getConfig(tracker));
		},
		createOacPrototype: function (tracker) {
		    return $http.post(baseUrl + 'CreateOacPrototype', getConfig(tracker));
		},
		createCheckPrototype: function (tracker) {
		    return $http.post(baseUrl + 'CreateCheckPrototype', getConfig(tracker));
		},
		createWireTransferPrototype: function (tracker)
		{
		    return $http.post(baseUrl + 'CreateWireTransferPrototype', getConfig(tracker));
		},
		createMarketingPrototype: function (tracker)
		{
		    return $http.post(baseUrl + 'CreateMarketingPrototype', getConfig(tracker));
		},
		createVCWellnessPrototype: function (tracker)
		{
		    return $http.post(baseUrl + 'CreateVCWellnessPrototype', getConfig(tracker));
		},
		resendActivation: function (publicId, tracker) {
			return $http.post(baseUrl + 'ResendActivation/' + publicId, null, getConfig(tracker));
		},
		resetPassword: function (publicId, tracker) {
			return $http.post(baseUrl + 'ResetPassword/' + publicId, null, getConfig(tracker));
		},
		loginAsCustomer: function (publicId, tracker) {
			return $http.post(baseUrl + 'LoginAsCustomer/' + publicId, null, getConfig(tracker));
		},
		getHistoryReport: function (filter, tracker)
		{
		    return $http.post(baseUrl + 'GetHistoryReport', filter, getConfig(tracker));
		},

		getCustomersForAffiliatesReportFileUrl: function (idAffiliate, buildNumber)
		{
		    return baseUrl + 'GetCustomersForAffiliates?idaffiliate={0}&buildNumber={1}'.format(idAffiliate, buildNumber);
		},
	};
}]);