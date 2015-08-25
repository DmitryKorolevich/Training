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
	    getOrder: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetOrder/' + id, getConfig(tracker));
	    },
	    calculateOrder: function (model, canceller)
	    {
	        return $http.post(baseUrl + 'CalculateOrder', model, { timeout: canceller.promise });
	    },
	    updateOrder: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateOrder', model, getConfig(tracker));
	    },
	};
}]);