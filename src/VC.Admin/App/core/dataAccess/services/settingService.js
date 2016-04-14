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
		getLogItems: function (filter, tracker) {
			return $http.post(baseUrl + 'GetLogItems', filter, getConfig(tracker));
		},
		getProfileScopeItems: function (filter, tracker)
		{
		    return $http.post(baseUrl + 'GetProfileScopeItems', filter, getConfig(tracker));
		},

	    //object logs
		getObjectHistoryLogItems: function (filter, tracker)
		{
		    return $http.post(baseUrl + 'GetObjectHistoryLogItems', filter, getConfig(tracker));
		},
		getOrderObjectHistoryLogItems: function (filter, tracker)
		{
		    return $http.post(baseUrl + 'GetOrderObjectHistoryLogItems', filter, getConfig(tracker));
		},
		getHistoryReport: function (filter, tracker)
		{
		    return $http.post(baseUrl + 'GetHistoryReport', filter, getConfig(tracker));
		},

	    //lookups
		getLookups: function (tracker)
		{
		    return $http.get(baseUrl + 'GetLookups', getConfig(tracker));
		},
		getLookup: function (id, tracker)
		{
		    return $http.get(baseUrl + 'GetLookup/{0}'.format(id), getConfig(tracker));
		},
		updateLookupVariants: function (id, model, tracker)
		{
		    return $http.post(baseUrl + 'UpdateLookupVariants/{0}'.format(id), model, getConfig(tracker));
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
	        return $http.post(baseUrl + 'DeleteCountry/' + id, null, getConfig(tracker));
	    },        
	    updateState: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateState', model, getConfig(tracker));
	    },
	    deleteState: function (id, tracker) {
	        return $http.post(baseUrl + 'DeleteState/' + id, null, getConfig(tracker));
	    },

	    //settings
	    getGlobalSettings: function (tracker) {
	        return $http.get(baseUrl + 'GetGlobalSettings', getConfig(tracker));
	    },
	    updateGlobalSettings: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateGlobalSettings', model, getConfig(tracker));
	    },

	    //catalog requests
	    getCatalogRequests: function (tracker)
	    {
	        return $http.get(baseUrl + 'GetCatalogRequests', getConfig(tracker));
	    },
	    getCatalogRequestsReportFileUrl: function (buildNumber)
	    {
	        return baseUrl + 'GetCatalogRequestsReportFile?buildNumber={0}'.format(buildNumber);
	    },
	    deleteCatalogRequests: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'DeleteCatalogRequests', null, getConfig(tracker));
	    },
	};
}]);