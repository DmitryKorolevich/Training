'use strict';

angular.module('app.core.dataAccess.services.settingService', [])
.service('settingService', ['$http', function ($http) {
	var baseUrl = '/Api/Setting/';

	return {
        //logs
		getLogItems: function(filter) {
		    return $http.post(baseUrl + 'GetLogItems', filter);
		},
	};
}]);