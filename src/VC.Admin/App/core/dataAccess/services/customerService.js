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
		createNCPrototype: function (tracker)
		{
		    return $http.post(baseUrl + 'CreateNCPrototype', getConfig(tracker));
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
		getProfileAddressFieldValuesByValueAsync: function (filter, tracker)
		{
		    return $http.post(baseUrl + 'GetProfileAddressFieldValuesByValueAsync', filter, getConfig(tracker));
		},
		getDefaultShippingAddressFieldValuesByValueAsync: function (filter, tracker)
		{
		    return $http.post(baseUrl + 'GetDefaultShippingAddressFieldValuesByValueAsync', filter, getConfig(tracker));
		},
		getCustomerStaticFieldValuesByValue: function (filter, tracker)
		{
		    return $http.post(baseUrl + 'GetCustomerStaticFieldValuesByValue', filter, getConfig(tracker));
		},
		getCustomerCardExist: function (idPaymentMethod, idCustomer, tracker)
		{
		    return $http.get(baseUrl + 'GetCustomerCardExist/{0}?idcustomer={1}'.format(idPaymentMethod, idCustomer), getConfig(tracker));
		},
		mergeCustomers: function (id, model, tracker)
		{
		    return $http.post(baseUrl + 'MergeCustomers/{0}'.format(id), model, getConfig(tracker));
		},

	    //reports
		getWholesaleSummaryReport: function (tracker)
		{
		    return $http.get(baseUrl + 'GetWholesaleSummaryReport', getConfig(tracker));
		},
		getWholesaleSummaryReportMonthStatistic: function (count, include, tracker)
		{
		    return $http.get(baseUrl + 'GetWholesaleSummaryReportMonthStatistic/?count={0}&include={1}'.format(count, include), getConfig(tracker));
		},
		getWholesales: function (filter, tracker)
		{
		    return $http.post(baseUrl + 'GetWholesales', filter, getConfig(tracker));
		},
		getWholesalesReportFile: function (filter, buildNumber)
		{
		    return baseUrl + ('GetWholesalesReportFile?idtradeclass={0}&idtier={1}&onlyactive={2}&buildNumber={3}')
                .format(filter.IdTradeClass, filter.IdTier, filter.OnlyActive, buildNumber);
		},
	};
}]);