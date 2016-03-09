angular.module('app.modules.inventorysku.controllers.inventorySkusController', [])
.controller('inventorySkusController', ['$scope', '$rootScope', '$state', 'inventorySkuService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, inventorySkuService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.deleteTracker = promiseTracker("delete");

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

            $scope.filter = {
                Code: null,
                Description: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshItems, "Code", "Asc")
            };

            refreshItems();
        }

        $scope.filterItems = function () {
            $scope.filter.Paging.PageIndex = 1;
            refreshItems();
        };

        $scope.pageChanged = function () {
            refreshItems();
        };

        $scope.open = function (id) {
            if (id) {
                $state.go('index.oneCol.inventorySkuDetail', { id: id });
            }
            else {
                $state.go('index.oneCol.addNewInventorySku', { });
            }
        };

        $scope.delete = function (id) {
            confirmUtil.confirm(function () {
                inventorySkuService.deleteInventorySku(id, $scope.deleteTracker)
                    .success(function (result) {
                        if (result.Success) {
                            toaster.pop('success', "Success!", "Successfully deleted.");
                            refreshItems();
                        } else {
                            errorHandler(result);
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to delete this inventory SKU?');
        };

        initialize();
    }]);