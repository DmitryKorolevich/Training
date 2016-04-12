'use strict';

angular.module('app.core.dataAccess.services.userService', [])
.service('userService', ['$http', function ($http) {
	var baseUrl = '/Api/UserManagement/';

	function getConfig(tracker) {
		var config = {  };
		if (tracker) {
			config.tracker = tracker;
		}
		return config;
	};

	return {
	    getAdminTeams: function (tracker)
	    {
	        return $http.get(baseUrl + 'GetAdminTeams', getConfig(tracker));
	    },
		getUsers: function(filter, tracker) {
			return $http.post(baseUrl + 'GetUsers', filter, getConfig(tracker));
		},
		createUserPrototype: function(tracker) {
			return $http.post(baseUrl + 'CreateUserPrototype', null, getConfig(tracker));
		},
		createUser: function(creatUserModel, tracker) {
			return $http.post(baseUrl + 'CreateUser', creatUserModel, getConfig(tracker));
		},
		updateUser: function(editUserModel, tracker) {
			return $http.post(baseUrl + 'UpdateUser', editUserModel, getConfig(tracker));
		},
		getUser: function (publicId, tracker) {
			return $http.get(baseUrl + 'GetUser/' + publicId, getConfig(tracker));
		},
		deleteUser: function (publicId, tracker) {
			return $http.post(baseUrl + 'DeleteUser', { PublicId: publicId }, getConfig(tracker));
		},
		resendActivation: function (publicId, tracker) {
			return $http.post(baseUrl + 'ResendActivation/' + publicId, null, getConfig(tracker));
		},
        resetPassword: function (publicId, tracker) {
			return $http.post(baseUrl + 'ResetPassword/' + publicId, null, getConfig(tracker));
		}
	};
}]);