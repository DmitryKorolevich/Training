﻿'use strict';

angular.module('app.modules.product.controllers.inventoryCategoryManageController', [])
.controller('inventoryCategoryManageController', ['$scope', '$rootScope', '$uibModalInstance', 'data', 'productService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $uibModalInstance, data, productService, toaster, confirmUtil, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.id = result.Data.Id;
                $scope.category.Id = result.Data.Id;
                data.thenCallback($scope.category);
                $uibModalInstance.close();
            } else
            {
                $rootScope.fireServerValidation(result, $scope);
            }
        };

        function errorHandler(result) {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function initialize() {
            $scope.id = data.id ? data.id : 0;

            $scope.forms = {};

            productService.getInventoryCategory($scope.id, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.category = result.Data;
                        if (data.categoryid) {
                            $scope.category.ParentId = data.categoryid;
                        };
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function (result) {
                    errorHandler(result);
                });
        }

        $scope.save = function () {
            $.each($scope.forms.form, function (index, element) {
                if (element && element.$name == index) {
                    element.$setValidity("server", true);
                }
            });

            if ($scope.forms.form.$valid) {

                productService.updateInventoryCategory($scope.category, $scope.editTracker).success(function (result) {
                    successSaveHandler(result);
                }).
                    error(function (result) {
                        errorHandler(result);
                    });
            } else {
                $scope.forms.submitted = true;
            }
        };

        $scope.cancel = function () {
            $uibModalInstance.close();
        };

        initialize();
    }]);