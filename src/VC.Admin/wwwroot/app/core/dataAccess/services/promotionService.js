'use strict';

angular.module('app.core.dataAccess.services.promotionService', [])
.service('promotionService', ['$http', function ($http)
{
    var baseUrl = '/Api/Promotion/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
	    //promotions  
	    getPromotions: function (filter, tracker) {
	        return $http.post(baseUrl + 'GetPromotions', filter, getConfig(tracker));
	    },
	    getPromotion: function (id, tracker) {
	        return $http.get(baseUrl + 'GetPromotion/' + id, getConfig(tracker));
	    },
	    updatePromotion: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdatePromotion', model, getConfig(tracker));
	    },
	    deletePromotion: function (id, tracker) {
	        return $http.post(baseUrl + 'DeletePromotion/' + id, null, getConfig(tracker));
	    },
	    getHistoryReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetHistoryReport', filter, getConfig(tracker));
	    },
	};
}]);