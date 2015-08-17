'use strict';

angular.module('app.core.dataAccess.services.orderService', [])
.service('orderService', ['$http', function ($http) {
	var baseUrl = '/Api/Order/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
	    //orders
	    getOrders: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetOrders', filter, getConfig(tracker));
	    },
	};
}]);