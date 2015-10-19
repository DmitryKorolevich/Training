angular.module('app.modules.setting.controllers.objectLogReportController', [])
.controller('objectLogReportController', ['$scope', '$modalInstance', 'data', 'toaster', 'promiseTracker', '$rootScope',
    function($scope,$modalInstance,data,toaster,promiseTracker,$rootScope) {
    $scope.refreshTracker=promiseTracker("get");

	function initialize() {

	    $scope.forms = {};
        	    
	    data.Main.JsonData = angular.fromJson(data.Main.Data);
	    data.Before.JsonData = angular.fromJson(data.Before.Data);
	    $scope.report = data;
	}

	$scope.cancel=function() {
	    $modalInstance.close();
	};
    
	initialize();
}]);