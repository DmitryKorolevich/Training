angular.module('app.modules.setting.controllers.lookupsController', [])
.controller('lookupsController', ['$scope', '$state', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
function ($scope, $state, settingService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
	$scope.refreshTracker = promiseTracker("refresh");

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function refresh() {
        settingService.getLookups($scope.refreshTracker)
			.success(function (result) {
			    if (result.Success) {
			        $scope.items = result.Data;
			    } else {
			        errorHandler(result);
			    }
			})
			.error(function (result) {
			    errorHandler(result);
			});
	};


	function initialize() {
        refresh();
	}

	initialize();
}]);