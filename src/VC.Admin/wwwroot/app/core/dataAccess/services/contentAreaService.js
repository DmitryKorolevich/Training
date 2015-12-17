'use strict';

angular.module('app.core.dataAccess.services.contentAreaService', [])
.service('contentAreaService', ['$http', function ($http) {
	var baseUrl = '/Api/ContentArea/';

	function getConfig(tracker) {
		var config = {};
		if (tracker) {
			config.tracker = tracker;
		}
		return config;
	};

	return {
		getContentAreas: function (tracker) {
			return $http.get(baseUrl + 'GetContentAreas', getConfig(tracker));
		},
		saveContentArea: function (model, tracker) {
			return $http.post(baseUrl + 'UpdateContentArea', model, getConfig(tracker));
		},
		getContentArea: function (id, tracker) {
			return $http.get(baseUrl + 'GetContentArea/' + id, getConfig(tracker));
		}
	};
}]);