'use strict';

angular.module('app.core.dataAccess.services.manageStylesService', [])
.service('manageStylesService', ['$http', function ($http) {
	var baseUrl = '/Api/Styles/';

	function getConfig(tracker) {
		var config = {};
		if (tracker) {
			config.tracker = tracker;
		}
		return config;
	};

	return {
		saveStyles: function (model, tracker) {
			return $http.post(baseUrl + 'UpdateStyles', model, getConfig(tracker));
		},
		getStyles: function (tracker) {
			return $http.get(baseUrl + 'GetStyles', getConfig(tracker));
		}
	};
}]);