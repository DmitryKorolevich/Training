angular.module('app.modules.order.controllers.ordersController',[])
.controller('ordersController',['$scope','$state','orderService','toaster','modalUtil','confirmUtil','promiseTracker','gridSorterUtil',
    function($scope,$state,orderService,toaster,modalUtil,confirmUtil,promiseTracker,gridSorterUtil) {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.deleteTracker = promiseTracker("delete");

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function refreshOrders() {

	};

	function initialize() {
	    $scope.filter = {
            Paging: { PageIndex: 1, PageItemCount: 100 },
            Sorting: gridSorterUtil.resolve(refreshOrders,"DateCreated","Desc")
	    };

	    refreshOrders();
	}

	$scope.filterOrders = function () {
	    $scope.filter.Paging.PageIndex = 1;
	    refreshOrders();
	};

	$scope.pageChanged = function () {
	    refreshOrders();
	};

	initialize();
}]);