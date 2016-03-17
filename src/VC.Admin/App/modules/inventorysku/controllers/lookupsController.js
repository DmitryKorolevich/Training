angular.module('app.modules.inventorysku.controllers.lookupsController', [])
.controller('lookupsController', ['$scope', '$state', 'inventorySkuService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
function ($scope, $state, inventorySkuService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
	$scope.refreshTracker = promiseTracker("refresh");

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function refresh() {
        inventorySkuService.getInventorySkuLookups($scope.refreshTracker)
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