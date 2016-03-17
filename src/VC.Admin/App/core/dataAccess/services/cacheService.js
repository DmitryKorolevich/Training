'use strict';

angular.module('app.core.dataAccess.services.cacheService', [])
.service('cacheService', ['$http', function ($http) {
	var baseUrl = '/Api/Cache/';

	function getConfig(tracker) {
		var config = {  };
		if (tracker) {
			config.tracker = tracker;
		}
		return config;
	};

	return {
		getCacheStatus: function(tracker) {
			return $http.get(baseUrl + 'CacheStatus', getConfig(tracker));
		}
	};
}]);