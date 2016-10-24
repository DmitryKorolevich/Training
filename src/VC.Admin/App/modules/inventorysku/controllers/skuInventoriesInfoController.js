angular.module('app.modules.inventorysku.controllers.skuInventoriesInfoController', [])
.controller('skuInventoriesInfoController', ['$scope', '$rootScope', '$state', 'inventorySkuService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil', 'Upload',
    function ($scope, $rootScope, $state, inventorySkuService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil, Upload)
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
            inventorySkuService.getSkuInventoriesInfo($scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        var items = result.Data;
                        $scope.noPartsActiveItems = $.grep(items, function (item, index)
                        {
                            return (!item.InventoriesLine || item.InventoriesLine.length == 0) && item.StatusCode == 2 && item.ProductStatusCode == 2;
                        });
                        $scope.withPartsItems = $.grep(items, function (item, index)
                        {
                            return item.InventoriesLine && item.InventoriesLine.length > 0;
                        });
                    } else {
                        errorHandler(result);
                    }
                })
                .error(function (result) {
                    errorHandler(result);
                });
        };

        function initialize()
        {
            $scope.options = {};
            $scope.options.activeExportUrl = inventorySkuService.getSkuInventoriesInfoReportFile({ withInventories: true, activeOnly: false }, $rootScope.buildNumber);

            $scope.filter = {
            };

            refreshItems();
        }

        initialize();
    }]);