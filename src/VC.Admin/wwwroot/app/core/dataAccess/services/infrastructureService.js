'use strict';

angular.module('app.core.dataAccess.services.infrastructureService', [])
.service('infrastructureService', ['$http', function ($http) {
	var baseUrl = '/Api/Infrastructure/';
		var config = {};

		return {
		getReferenceData: function() {
			return $http.get(baseUrl + 'GetReferenceData', config);
		}
	};
}]);