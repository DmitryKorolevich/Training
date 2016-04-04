'use strict';

angular.module('app.core.dataAccess.services.serviceCodeService', [])
.service('serviceCodeService', ['$http', function ($http)
{
    var baseUrl = '/Api/ServiceCode/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
	    getServiceCodesReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetServiceCodesReport', filter, getConfig(tracker));
	    },
	    getServiceCodeRefundItems: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetServiceCodeRefundItems', filter, getConfig(tracker));
	    },
	    getServiceCodeReshipItems: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetServiceCodeReshipItems', filter, getConfig(tracker));
	    },
	    getServiceCodeRefundItemsReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + 'GetServiceCodeRefundItemsReportFile?from={0}&to={1}&servicecode={2}&buildNumber={3}'
                .format(filter.From, filter.To, filter.ServiceCode, buildNumber);
	    },
	    getServiceCodeReshipItemsReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + 'GetServiceCodeReshipItemsReportFile?from={0}&to={1}&servicecode={2}&buildNumber={3}'
                .format(filter.From, filter.To, filter.ServiceCode, buildNumber);
	    },
	    assignServiceCodeForRefunds: function (serviceCode, ids, tracker)
	    {
	        return $http.post(baseUrl + 'AssignServiceCodeForRefunds/?servicecode={0}'.format(serviceCode), ids, getConfig(tracker));
	    },
	    assignServiceCodeForReships: function (serviceCode, ids, tracker)
	    {
	        return $http.post(baseUrl + 'AssignServiceCodeForReships/?servicecode={0}'.format(serviceCode), ids, getConfig(tracker));
	    },
	};
}]);