angular.module('app.modules.product.controllers.pendingProductReviewsController', [])
.controller('pendingProductReviewsController', ['$scope', '$state', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $state, productService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil) {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.deleteTracker = promiseTracker("delete");

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function refreshProductReviews() {
        productService.getProductReviews($scope.filter, $scope.refreshTracker)
			.success(function (result) {
			    if (result.Success) {
			        $scope.productReviews = result.Data.Items;
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
            StatusCode: 1,//pending
            Paging: { PageIndex: 1, PageItemCount: 100 },
            Sorting: gridSorterUtil.resolve(refreshProductReviews,"DateCreated","Desc")
	    };

	    refreshProductReviews();
	}

	$scope.filterProductReviews = function () {
	    $scope.filter.Paging.PageIndex = 1;
	    refreshProductReviews();
	};

	$scope.pageChanged = function () {
	    refreshProductReviews();
	};

	$scope.edit = function (id) {
	    $state.go('index.oneCol.productReviewDetail',{ id: id });
	};

	$scope.delete=function(id) {
	    confirmUtil.confirm(function() {
	        productService.deleteProductReview(id,$scope.deleteTracker)
                .success(function(result) {
                    if(result.Success) {
                        toaster.pop('success',"Success!","Successfully deleted.");
                        refreshProductReviews();
                    } else {
                        errorHandler(result);
                    }
                })
                .error(function(result) {
                    errorHandler(result);
                });
	    },'Are you sure you want to delete this product review?');
	};

	initialize();
}]);