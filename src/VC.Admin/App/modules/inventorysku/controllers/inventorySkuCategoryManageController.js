'use strict';

angular.module('app.modules.inventorysku.controllers.inventorySkuCategoryManageController', [])
.controller('inventorySkuCategoryManageController', ['$scope', '$uibModalInstance', 'data', 'inventorySkuService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $uibModalInstance, data, inventorySkuService, toaster, confirmUtil, promiseTracker)
    {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.id = result.Data.Id;
                $scope.category.Id = result.Data.Id;
                data.thenCallback($scope.category);
                $uibModalInstance.close();
            } else {
                var messages = "";
                if (result.Messages) {
                    $scope.forms.submitted = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);
                    $.each(result.Messages, function (index, value) {
                        if (value.Field) {
                            $scope.forms.form[value.Field].$setValidity("server", false);
                        }
                        messages += value.Message + "<br />";
                    });
                }
                toaster.pop('error', "Error!", messages, null, 'trustedHtml');
            }
        };

        function errorHandler(result) {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function initialize() {
            $scope.id = data.id ? data.id : 0;

            $scope.forms = {};

            inventorySkuService.getInventorySkuCategory($scope.id, $scope.refreshTracker)
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

                inventorySkuService.updateInventorySkuCategory($scope.category, $scope.editTracker).success(function (result)
                {
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