'use strict';

angular.module('app.core.dataAccess.services.healthwiseService', [])
.service('healthwiseService', ['$http', function ($http)
{
    var baseUrl = '/Api/Healthwise/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
	    getHealthwiseCustomersWithPeriods: function (filter, tracker)
	    {
		    return $http.post(baseUrl + 'GetHealthwiseCustomersWithPeriods', filter, getConfig(tracker));
		},
	    getHealthwisePeriod: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetHealthwisePeriod/'+id, getConfig(tracker));
	    },
	    getHealthwiseOrders: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetHealthwiseOrders/' + id, getConfig(tracker));
	    },
	    makeHealthwisePeriodPayment: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'MakeHealthwisePeriodPayment', model, getConfig(tracker));
	    },
	    markOrder: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'MarkOrder/' + id, null, getConfig(tracker));
	    },
	    markCustomerOrders: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'MarkCustomerOrders/' + id, null, getConfig(tracker));
	    },
	    getHealthwisePeriodsForMovement: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetHealthwisePeriodsForMovement', filter, getConfig(tracker));
	    },
	    moveHealthwiseOrders: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'MoveHealthwiseOrders', model, getConfig(tracker));
	    },
	};
}]);