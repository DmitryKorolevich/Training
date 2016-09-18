angular.module('app.modules.inventorysku.controllers.inventorySkusController', [])
.controller('inventorySkusController', ['$scope', '$rootScope', '$state', 'inventorySkuService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil', 'Upload',
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

        function initialize()
        {
            $scope.options = {};

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
            }, 'Are you sure you want to delete this inventory part?');
        };

        $scope.upload = function (files)
        {
            $scope.options.selectedOrderImportFile = files && files.length > 0 ? files[0] : null;
            if ($scope.options.selectedOrderImportFile)
            {
                $scope.options.uploadingOrdersImport = true;
                var deferred = $scope.refreshTracker.createPromise();
                Upload.upload({
                    url: '/api/inventorysku/ImportInventorySkus',
                    data: {},
                    file: $scope.options.selectedOrderImportFile
                }).progress(function (evt)
                {

                }).success(function (result, status, headers, config)
                {
                    deferred.resolve();
                    if (result.Success)
                    {
                        modalUtil.open('app/modules/setting/partials/infoDetailsPopup.html', 'infoDetailsPopupController', {
                            Header: "Success!",
                            Messages: [{ Message: "Successfully imported" }],
                            OkButton: {
                                Label: 'Ok',
                                Handler: function ()
                                {
                                }
                            },
                        }, { size: 'xs' });
                        $scope.filterItems();
                    } else
                    {
                        if (result.Messages)
                        {
                            modalUtil.open('app/modules/setting/partials/infoDetailsPopup.html', 'infoDetailsPopupController', {
                                Header: "Error details",
                                Messages: result.Messages
                            });
                        }
                    }
                    $scope.options.selectedOrderImportFile = null;
                }).error(function (data, status, headers, config)
                {
                    deferred.resolve();
                    $scope.options.selectedOrderImportFile = null;

                    toaster.pop('error', "Error!", "Server error ocurred");
                });
            }
        };

        initialize();
    }]);