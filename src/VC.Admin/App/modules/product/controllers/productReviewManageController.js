'use strict';

angular.module('app.modules.product.controllers.productReviewManageController',[])
.controller('productReviewManageController',['$scope','$rootScope','$state','$stateParams','$timeout','productService','toaster','modalUtil','confirmUtil','promiseTracker',
function ($scope, $rootScope, $state, $stateParams, $timeout, productService, toaster, modalUtil, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function successSaveHandler(result) {
        if (result.Success) {
            toaster.pop('success',"Success!","Successfully saved.");
            $scope.goBack();
        } else
        {
            $rootScope.fireServerValidation(result, $scope);
        }
    };

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize() {
        $scope.id=$stateParams.id?$stateParams.id:0;
        $scope.idProduct=$stateParams.idproduct?$stateParams.idproduct:0;

        $scope.forms = {};

        $scope.productReview={};

        productService.getProductReview($scope.id, $scope.refreshTracker).success(function(result) {
            if (result.Success) {
                $scope.productReview=result.Data;
            };
        }).error(function (result) {
            errorHandler(result);
        });
    };

    $scope.save = function () {
        $.each($scope.forms.form, function (index, element) {
        	if (element && element.$name == index) {
                element.$setValidity("server", true);
            }
        });

        if ($scope.forms.form.$valid) {
            productService.updateProductReview($scope.productReview,$scope.editTracker).success(function(result) {
                successSaveHandler(result);
            }).
                error(function (result) {
                    errorHandler(result);
                });
        } else {
            $scope.forms.submitted = true;
        }
    };

    $scope.goBack=function() {
        if($scope.idProduct) {
            $state.go('index.oneCol.productDetailManageReviews',{ idproduct: $scope.idProduct });
        }
        else
        {
            $state.go('index.oneCol.managePendingProductReviews',{});
        }
    };

    $scope.delete=function() {
        confirmUtil.confirm(function() {
            productService.deleteProductReview($scope.id,$scope.deleteTracker)
                .success(function(result) {
                    if(result.Success) {
                        toaster.pop('success',"Success!","Successfully deleted.");
                        $scope.goBack();
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