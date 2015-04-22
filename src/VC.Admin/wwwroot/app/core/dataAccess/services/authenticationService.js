'use strict';

angular.module('app.core.dataAccess.services.authenticationService', [])
.service('authenticationService', ['$http', function ($http) {
	var baseUrl = '/Api/Account/';

	function getConfig(tracker) {
		var config = {  };
		if (tracker) {
			config.tracker = tracker;
		}
		return config;
	};

	return {
		activate: function(user, tracker) {
			return $http.post(baseUrl + 'Activate', user, getConfig(tracker));
		},
		getUser: function (token, tracker) {
			return $http.get(baseUrl + 'GetUser/' + token, getConfig(tracker));
		}
	};
}]);