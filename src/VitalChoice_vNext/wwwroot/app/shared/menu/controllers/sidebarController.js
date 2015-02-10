'use strict';

angular.module('app.shared.menu.controllers.sidebarController', [
    'app.shared.menu.services.navigationFactory'
])
.controller('sidebarController', ['$scope', 'navigationFactory', function ($scope, navigationFactory) {
	alert("sidebar");

}]);