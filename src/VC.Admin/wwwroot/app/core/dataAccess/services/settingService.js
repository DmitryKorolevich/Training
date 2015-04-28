'use strict';

angular.module('app.core.dataAccess.services.settingService', [])
.service('settingService', ['$http', function ($http) {
	var baseUrl = '/Api/Setting/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
        //logs
		getLogItems: function(filter) {
		    return $http.post(baseUrl + 'GetLogItems', filter);
		},

        //countries/states
	    getCountries: function (filter, tracker) {
	        return $http.post(baseUrl + 'GetCountries', filter, getConfig(tracker));
	    },
	    updateCountriesOrder: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateCountriesOrder', model, getConfig(tracker));
	    },
	    updateCountry: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateCountry', model, getConfig(tracker));
	    },
	    deleteCountry: function (id, tracker) {
	        return $http.post(baseUrl + 'DeleteCountry/' + id, getConfig(tracker));
	    },        
	    updateState: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateState', model, getConfig(tracker));
	    },
	    deleteState: function (id, tracker) {
	        return $http.post(baseUrl + 'DeleteState/' + id, getConfig(tracker));
	    },

	    //settings
	    getGlobalPerishableThreshold: function (tracker) {
	        return $http.get(baseUrl + 'GetGlobalPerishableThreshold', getConfig(tracker));
	    },
	    updateGlobalPerishableThreshold: function (value, tracker) {
	        return $http.post(baseUrl + 'UpdateGlobalPerishableThreshold/' + value, getConfig(tracker));
	    },
	};
}]);