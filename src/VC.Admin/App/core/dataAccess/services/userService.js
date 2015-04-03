'use strict';

angular.module('app.core.dataAccess.services.userService', [])
.service('userService', ['$http', function ($http) {
	var baseUrl = '/Api/UserManagement/';

	return {
		getUsers: function(filter) {
			return $http.post(baseUrl + 'GetUsers', filter);
		},
		createUserPrototype: function() {
			return $http.post(baseUrl + 'CreateUserPrototype');
		},
		createUser: function(creatUserModel) {
			return $http.post(baseUrl + 'CreateUser', creatUserModel);
		},
		updateUser: function(editUserModel) {
			return $http.post(baseUrl + 'UpdateUser', editUserModel);
		},
		getUser: function (publicId) {
			return $http.get(baseUrl + 'GetUser/' + publicId);
		},
		deleteUser: function (publicId) {
			return $http.post(baseUrl + 'DeleteUser', { PublicId: publicId });
		}
	};
}]);