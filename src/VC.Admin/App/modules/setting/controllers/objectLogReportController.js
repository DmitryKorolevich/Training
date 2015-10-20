angular.module('app.modules.setting.controllers.objectLogReportController', [])
.controller('objectLogReportController', ['$scope', '$modalInstance', 'data', 'toaster', 'promiseTracker', '$rootScope', '$timeout',
    function ($scope, $modalInstance, data, toaster, promiseTracker, $rootScope, $timeout)
    {
    $scope.refreshTracker=promiseTracker("get");

	function initialize() {

	    $scope.forms = {};
        	    
	    $scope.MainJsonData = angular.fromJson(data.Main.Data);
	    $scope.BeforeJsonData = angular.fromJson(data.Before.Data);
	    $scope.report = data;

	    $timeout(function ()
	    {
	        $('.wrapper > div > div.json-formatter-row > a').click()
	    }, 100);
	}

	$scope.cancel=function() {
	    $modalInstance.close();
	};
    
	initialize();
}]);