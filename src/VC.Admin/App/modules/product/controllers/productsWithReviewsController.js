angular.module('app.modules.product.controllers.productsWithReviewsController', [])
.controller('productsWithReviewsController', ['$scope', '$state', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $state, productService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil) {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.deleteTracker = promiseTracker("delete");

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function refreshProducts() {
        productService.getProductsWithReviews($scope.filter, $scope.refreshTracker)
			.success(function (result) {
			    if (result.Success) {
			        $scope.products = result.Data.Items;
                    $scope.totalItems = result.Data.Count;
			    } else {
			        errorHandler(result);
			    }
			})
			.error(function (result) {
			    errorHandler(result);
			});
	};

	function initialize() {
	    $scope.filter = {
	        SearchText: '',
            StatusCode: 2,//active
            Paging: { PageIndex: 1, PageItemCount: 100 },
            Sorting: gridSorterUtil.resolve(refreshProducts,"ProductName","Asc")
	    };

	    refreshProducts();
	}

	$scope.filterProducts = function () {
	    $scope.filter.Paging.PageIndex = 1;
	    refreshProducts();
	};

	$scope.pageChanged = function () {
	    refreshProducts();
	};

	$scope.open = function (idProduct) {
	    $state.go('index.oneCol.productDetailManageReviews',{ idproduct: idProduct });
	};

	initialize();
}]);