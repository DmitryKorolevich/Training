angular.module('app.modules.setting.controllers.objectLogReportController', [])
.controller('objectLogReportController', ['$scope', '$modalInstance', '$rootScope', '$timeout', '$window', 'data', 'toaster', 'promiseTracker',
    function ($scope, $modalInstance, $rootScope, $timeout, $window, data, toaster, promiseTracker)
    {

	function initialize() {

	    $scope.forms = {};
	    $scope.options = {};
	    $scope.options.isDiff = false;
	    $scope.options.showUnchanged = false;
	    $scope.$watch('options.showUnchanged', function (newValue, oldValue)
	    {
            jsondiffpatch.formatters.html.showUnchanged(newValue, null, 500);
	    });
	    $scope.$watch('options.isDiff', function (newValue, oldValue)
	    {
	        if (newValue)
	        {
	            jsondiffpatch.formatters.html.showUnchanged($scope.options.showUnchanged);
	        }
	    });
        	    
	    $scope.MainJsonData = angular.fromJson(data.Main.Data);
	    $scope.BeforeJsonData = angular.fromJson(data.Before.Data);
	    $scope.report = data;
	    createDiff();

	    $timeout(function ()
	    {
	        $('.wrapper > div > div.json-formatter-row > a').click()
	    }, 100);
	}

	function createDiff()
	{
	    var jsondiffpatch = $window.jsondiffpatch;
	    var jsonDiff = jsondiffpatch.create({
	        objectHash: function(obj) {
	            if (typeof obj._id !== 'undefined') {
	                return obj._id;
	            }
	            if (typeof obj.id !== 'undefined') {
	                return obj._id;
	            }
	            if (typeof obj.name !== 'undefined') {
	                return obj.name;
	            }
	            return JSON.stringify(obj);
	        },
	        arrays: {
	            detectMove: true,
	            includeValueOnMove: false
	        },
	        textDiff: {
	            minLength: 60
	        }
	    });

	    var delta = jsondiffpatch.diff($scope.BeforeJsonData, $scope.MainJsonData);
	    if (delta === undefined)
	    {
	        $scope.visualDiff = "No Diff";
	    }
	    else
	    {
	        $scope.visualDiff = jsondiffpatch.formatters.html.format(delta, $scope.BeforeJsonData);
	    }
	};

	$scope.cancel=function() {
	    $modalInstance.close();
	};
    
	initialize();
}]);