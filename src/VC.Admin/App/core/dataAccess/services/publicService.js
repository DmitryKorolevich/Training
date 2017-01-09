'use strict';

angular.module('app.core.dataAccess.services.publicService', [])
.service('publicService', ['$http', function ($http)
{
    var baseUrl = '/Api/Public/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
	    //email orders
	    getEmailOrderSettings: function (tracker)
	    {
	        return $http.get(baseUrl + 'GetEmailOrderSettings', getConfig(tracker));
	    },
	    getEmailOrder: function (tracker)
	    {
	        return $http.get(baseUrl + 'GetEmailOrder', getConfig(tracker));
	    },
	    sendEmailOrder: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'SendEmailOrder', model, getConfig(tracker));
	    },
	};
}]);