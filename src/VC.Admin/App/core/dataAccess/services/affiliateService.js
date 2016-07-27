'use strict';

angular.module('app.core.dataAccess.services.affiliateService', [])
.service('affiliateService', ['$http', function ($http)
{
    var baseUrl = '/Api/Affiliate/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
	    //affiliates
	    getAffiliates: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetAffiliates', filter, getConfig(tracker));
	    },
	    getAffiliate: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetAffiliate/' + id, getConfig(tracker));
	    },
	    updateAffiliate: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateAffiliate', model, getConfig(tracker));
	    },
	    getAffiliateEmail: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetAffiliateEmail/' + id, getConfig(tracker));
	    },
	    sendAffiliateEmail: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'SendAffiliateEmail', model, getConfig(tracker));
	    },
	    deleteAffiliate: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'DeleteAffiliate/' + id, null, getConfig(tracker));
	    },
	    resendActivation: function (publicId, tracker)
	    {
	        return $http.post(baseUrl + 'ResendActivation/' + publicId, null, getConfig(tracker));
	    },
	    resetPassword: function (publicId, tracker)
	    {
	        return $http.post(baseUrl + 'ResetPassword/' + publicId, null, getConfig(tracker));
	    },
	    getHistoryReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetHistoryReport', filter, getConfig(tracker));
	    },
	    getCustomerInAffiliateReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetCustomerInAffiliateReport', filter, getConfig(tracker));
	    },
	    getAffiliateOrderPaymentsWithCustomerInfo: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetAffiliateOrderPaymentsWithCustomerInfo', filter, getConfig(tracker));
	    },
	    getAffiliatesSummary: function (tracker)
	    {
	        return $http.get(baseUrl + 'GetAffiliatesSummary', getConfig(tracker));
	    },
	    getAffiliatesSummaryReportItemsForMonths: function (count, include, tracker)
	    {
	        return $http.get(baseUrl + 'GetAffiliatesSummaryReportItemsForMonths/?count={0}&include={1}'.format(count,include), getConfig(tracker));
	    },
	    getAffiliatePaymentHistory: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetAffiliatePaymentHistory/' + id, getConfig(tracker));
	    },
	    payForAffiliateOrders: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'PayForAffiliateOrders/' + id, null, getConfig(tracker));
	    },
	    getUnpaidOrdersForLastPeriod: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetUnpaidOrdersForLastPeriod/' + id, getConfig(tracker));
	    },
	    getLoginAsAffiliateUrl: function (id, buildNumber)
	    {
	        return baseUrl + 'LoginAsAffiliate/{0}?&buildNumber={1}'.format(id, buildNumber);
	    },
	};
}]);