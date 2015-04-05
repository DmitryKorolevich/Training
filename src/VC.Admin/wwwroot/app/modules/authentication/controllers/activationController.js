'use strict';

angular.module('app.modules.authentication.controllers.activationController', [])
.controller('activationController', ['$scope', '$state', '$rootScope', function ($scope, $state, $rootScope) {
	$rootScope.authenticated = false;

	$scope.user = {
		FirstName: "Gary",
		LastName: "Gould",
		Email: "gary.gould@gmail.com",
		AgentId: "GG"
	};

	$scope.activate = function () {
		$state.go("index.oneCol.dashboard");
		$rootScope.authenticated = true;
	};

}]);