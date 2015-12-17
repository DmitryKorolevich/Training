angular.module('app.modules.product.controllers.topPurchasedProductsController',[])
.controller('topPurchasedProductsController',['$scope','$modalInstance','data','productService','toaster','promiseTracker','$rootScope',
    function($scope,$modalInstance,data,productService,toaster,promiseTracker,$rootScope) {
    $scope.refreshTracker=promiseTracker("get");

	function errorHandler(result) {
		toaster.pop('error', "Error!", "Server error occured");
	};

	function initialize() {

	    $scope.forms = {};

	    $scope.addedProducts=data.products;

	    productService.getTopPurchasedSkus($scope.filter,$scope.refreshTracker)
            .success(function(result) {
                if(result.Success) {
                    $scope.products=result.Data;
                    $.each($scope.products,function(index,product) {
                        $.each($scope.addedProducts,function(index,addedProduct) {
                            if(addedProduct.Code==product.Code)
                            {
                                product.IsSelected=true;
                            }
                        });
                    });
                } else {
                    errorHandler(result);
                }
            })
            .error(function(result) {
                errorHandler(result);
            });
	}

	$scope.save=function() {
	    var addedProducts=[];
	    $.each($scope.products,function(index,product) {
	        if(product.IsSelected)
	        {
	            addedProducts.push(product);
	        }
	    });

	    data.thenCallback(addedProducts);
	    $modalInstance.close();
	};

	$scope.cancel=function() {
	    $modalInstance.close();
	};
    
	initialize();
}]);