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
	    deleteAffiliate: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'DeleteAffiliate/' + id, getConfig(tracker));
	    },
	};
}]);