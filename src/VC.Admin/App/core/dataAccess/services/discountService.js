'use strict';

angular.module('app.core.dataAccess.services.discountService', [])
.service('discountService', ['$http', function ($http) {
    var baseUrl = '/Api/Discount/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
	    //discounts  
	    getDiscounts: function (filter, tracker) {
	        return $http.post(baseUrl + 'GetDiscounts', filter, getConfig(tracker));
	    },
	    getDiscount: function (id, tracker) {
	        return $http.get(baseUrl + 'GetDiscount/' + id, getConfig(tracker));
	    },
	    updateDiscount: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateDiscount', model, getConfig(tracker));
	    },
	    deleteDiscount: function (id, tracker) {
	        return $http.post(baseUrl + 'DeleteDiscount/' + id, null, getConfig(tracker));
	    },
	    getHistoryReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetHistoryReport', filter, getConfig(tracker));
	    },
	};
}]);