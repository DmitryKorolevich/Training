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
            inventorySkuService.getSkuInventoriesInfo($scope.filter, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        var items = result.Data;
                        $scope.noPartsItems = $.grep(items, function (index, item)
                        {
                            return !item.InventoriesLine;
                        });
                        $scope.activeItems = $.grep(items, function (index, item)
                        {
                            return item.StatusCode==2;
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
            $scope.options.activeExportUrl = inventorySkuService.getSkuInventoriesInfoReportFile({ activeOnly: true}, $rootScope.buildNumber);

            $scope.filter = {
            };

            refreshItems();
        }

        initialize();
    }]);