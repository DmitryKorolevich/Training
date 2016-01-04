'use strict';

angular.module('app.core.dataAccess.services.redirectService', [])
.service('redirectService', ['$http', function ($http)
{
    var baseUrl = '/Api/Redirect/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
	    getRedirects: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetRedirects', filter, getConfig(tracker));
	    },
	    getRedirect: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetRedirect/' + id, getConfig(tracker));
	    },
	    updateRedirect: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateRedirect', model, getConfig(tracker));
	    },
	    deleteRedirect: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'DeleteRedirect/' + id, null, getConfig(tracker));
	    },
	};
}]);