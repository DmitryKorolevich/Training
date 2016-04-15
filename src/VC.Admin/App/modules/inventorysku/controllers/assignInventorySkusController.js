angular.module('app.modules.inventorysku.controllers.assignInventorySkusController', [])
.controller('assignInventorySkusController', ['$scope', '$rootScope', '$state', '$uibModalInstance', 'inventorySkuService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil', 'data',
    function ($scope, $rootScope, $state, $uibModalInstance, inventorySkuService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil, data)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function errorHandler(result) {
            var messages = "";
            if (result.Messages) {
                $.each(result.Messages, function (index, value) {
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        };

        function refreshItems() {
            inventorySkuService.getInventorySkus($scope.filter, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.items = result.Data.Items;
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

            $scope.forms = {};

            $scope.assignedItems = angular.copy(data.assignedItems);

            $scope.filter = {
                StatusCode: 2,
                Code: null,
            };
        }

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                refreshItems();
            }
            else
            {
                $scope.forms.submitted = true;
            }
        };

        $scope.add = function(newItem){            
            var add=true;
            $.each($scope.assignedItems, function (index, item) {
                if(item.Id==newItem.Id)
                {
                    add=false;
                    return;
                }
            });
            if(add)
            {
                var item = angular.copy(newItem);
                item.Quantity = 1;
                $scope.assignedItems.push(item);
            }
        };

        $scope.delete = function (index) {
            $scope.assignedItems.splice(index, 1);
        };

        $scope.save = function()
        {
            clearServerValidation();

            if ($scope.forms.InventorySkus.$valid)
            {
                data.thenCallback($scope.assignedItems);
                $uibModalInstance.close();
            }
            else
            {
                $scope.forms.InventorySkus.submitted = true;
            }
        };

        var clearServerValidation = function ()
        {
            $.each($scope.forms, function (index, form)
            {
                if (form && !(typeof form === 'boolean'))
                {
                    if (index == "InventorySkus")
                    {
                        $.each(form, function (index, subForm)
                        {
                            if (index.indexOf('i') == 0)
                            {
                                $.each(subForm, function (index, element)
                                {
                                    if (element && element.$name == index)
                                    {
                                        element.$setValidity("server", true);
                                    }
                                });
                            }
                        });
                    }
                    else
                    {
                        $.each(form, function (index, element)
                        {
                            if (element && element.$name == index)
                            {
                                element.$setValidity("server", true);
                            }
                        });
                    }
                }
            });
        };

        $scope.cancel = function ()
        {
            $uibModalInstance.close();
        };

        initialize();
    }]);