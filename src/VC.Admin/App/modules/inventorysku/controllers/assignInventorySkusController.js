angular.module('app.modules.inventorysku.controllers.assignInventorySkusController', [])
.controller('assignInventorySkusController', ['$scope', '$rootScope', '$state', '$modalInstance', 'inventorySkuService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil', 'data',
    function ($scope, $rootScope, $state, $modalInstance, inventorySkuService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil, data)
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
                $scope.assignedItems.push(newItem);
            }
        };

        $scope.delete = function (index) {
            $scope.assignedItems.splice(index, 1);
        };

        $scope.save = function()
        {
            data.thenCallback($scope.assignedItems);
            $modalInstance.close();
        };

        $scope.cancel = function ()
        {
            $modalInstance.close();
        };

        initialize();
    }]);