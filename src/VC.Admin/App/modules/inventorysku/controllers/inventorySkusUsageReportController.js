angular.module('app.modules.inventorysku.controllers.inventorySkusUsageReportController', [])
.controller('inventorySkusUsageReportController', ['$scope', '$rootScope', '$state', '$timeout', 'inventorySkuService', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $timeout, inventorySkuService, productService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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

        function refreshItems()
        {
            var data = {};
            angular.copy($scope.filter, data);
            if (data.From)
            {
                data.From = data.From.toServerDateTime();
            }
            if (data.To)
            {
                data.To = data.To.toServerDateTime();
            }
            if (data.Skus)
            {
                data.SkuIds = $.map(data.Skus, function (s, i)
                {
                    return s.Id;
                });
            }
            if (data.InvSkus)
            {
                data.InvSkuIds = $.map(data.InvSkus, function (s, i)
                {
                    return s.Id;
                });
            }

            inventorySkuService.getInventorySkuUsageReport(data, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.items = result.Data;
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
            $scope.options = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-1m'),
                Skus: [],
                InvSkus: [],
            };

            $scope.skusFilter = {
                Code: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };

            $scope.skuFilter = {
                ExactCode: '',
                Paging: { PageIndex: 1, PageItemCount: 1 },
            };

            $scope.invSkusFilter = {
                Code: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };

            $scope.invSkuFilter = {
                ExactCode: '',
                Paging: { PageIndex: 1, PageItemCount: 1 },
            };

            $scope.filterChanged();
            loadLookups();
        }

        function loadLookups()
        {
            inventorySkuService.getInventorySkuLookups($scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        var data = result.Data;
                        $scope.lookups = {};
                        $.each(data, function (index, item)
                        {
                            if (item.Name == 'InventorySkuChannels')
                            {
                                $scope.lookups.inventorySkuChannels = $.map(item.LookupVariants, function (l, i)
                                {
                                    return {
                                        Key: l.Id,
                                        Text: l.ValueVariant
                                    };
                                });
                            } else if (item.Name == 'InventorySkuProductSources')
                            {
                                $scope.lookups.inventorySkuProductSources = $.map(item.LookupVariants, function (l, i)
                                {
                                    return {
                                        Key: l.Id,
                                        Text: l.ValueVariant
                                    };
                                });
                            } else if (item.Name == 'InventorySkuUnitOfMeasures')
                            {
                                $scope.lookups.inventorySkuUnitOfMeasures = $.map(item.LookupVariants, function (l, i)
                                {
                                    return {
                                        Key: l.Id,
                                        Text: l.ValueVariant
                                    };
                                });
                            } else if (item.Name == 'InventorySkuPurchaseUnitOfMeasures')
                            {
                                $scope.lookups.inventorySkuPurchaseUnitOfMeasures = $.map(item.LookupVariants, function (l, i)
                                {
                                    return {
                                        Key: l.Id,
                                        Text: l.ValueVariant
                                    };
                                });
                            }
                        });
                        refreshItems();
                    } else
                    {
                        errorHandler(result);
                    }
                }).
                error(function (result)
                {
                    errorHandler(result);
                });
        };

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.forms.form.submitted = false;
                refreshItems();
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.filterChanged = function ()
        {
            var data = {};
            angular.copy($scope.filter, data);
            if (data.From)
            {
                data.From = data.From.toServerDateTime();
            }
            if (data.To)
            {
                data.To = data.To.toServerDateTime();
            }
            data.sSkuIds = '';
            if (data.Skus.length > 0)
            {
                for (i = 0; i < data.Skus.length; i++)
                {
                    data.sSkuIds += data.Skus[i].Id;
                    if (i != data.Skus.length - 1)
                    {
                        data.sSkuIds += ',';
                    }
                }
            }
            data.sInvSkuIds = '';
            if (data.InvSkus.length > 0)
            {
                for (i = 0; i < data.InvSkus.length; i++)
                {
                    data.sInvSkuIds += data.InvSkus[i].Id;
                    if (i != data.InvSkus.length - 1)
                    {
                        data.sInvSkuIds += ',';
                    }
                }
            }

            $scope.options.ExportUrl = inventorySkuService.getInventorySkuUsageReportFile(data, $rootScope.buildNumber);
        };

        $scope.getSKUsBySKU = function (val)
        {
            if (val)
            {
                $scope.skusFilter.Code = val;
                return productService.getSkus($scope.skusFilter)
                    .then(function (result)
                    {
                        return result.data.Data.map(function (item)
                        {
                            return item;
                        });
                    });
            }
        };

        $scope.skuChangedRequest = null;

        $scope.skuChanged = function (index)
        {
            //resolving issue with additional load after lost focus from the input in time of selecting a new element
            if ($scope.skuChangedRequest)
            {
                $timeout.cancel($scope.skuChangedRequest);
            }
            $scope.skuChangedRequest = $timeout(function ()
            {
                if ($scope.options.SkuCode)
                {
                    $scope.skuFilter.ExactCode = $scope.options.SkuCode;
                    $scope.options.SkuAddDisabled = true;
                    productService.getSku($scope.skuFilter)
                        .success(function (result)
                        {
                            if (result.Success)
                            {     
                                $scope.options.CurrentSku = result.Data;
                            } else
                            {
                                errorHandler(result);
                            }
                            $scope.options.SkuAddDisabled = false;
                        })
                        .error(function (result)
                        {
                            $scope.options.SkuAddDisabled = false;
                            errorHandler(result);
                        });
                    $scope.skuChangedRequest = null;
                }
            }, 20);
        };

        $scope.addSku = function ()
        {
            if ($scope.options.CurrentSku)
            {
                var allowAdd = true;
                $.each($scope.filter.Skus, function (index, item)
                {
                    if ($scope.options.CurrentSku.Id == item.Id)
                    {
                        allowAdd = false;
                        return false;
                    }
                });
                if (allowAdd)
                {
                    $scope.filter.Skus.push($scope.options.CurrentSku);
                    $scope.filterChanged();
                }
            }
        };

        $scope.removeSku = function (index)
        {
            $scope.filter.Skus.splice(index, 1);
            $scope.filterChanged();
        };

        $scope.getSKUsBySKU = function (val)
        {
            if (val)
            {
                $scope.skusFilter.Code = val;
                return productService.getSkus($scope.skusFilter)
                    .then(function (result)
                    {
                        return result.data.Data.map(function (item)
                        {
                            return item;
                        });
                    });
            }
        };


        $scope.getInvSKUsBySKU = function (val)
        {
            if (val)
            {
                $scope.invSkusFilter.Code = val;
                return inventorySkuService.getInventorySkus($scope.invSkusFilter)
                    .then(function (result)
                    {
                        return result.data.Data.Items.map(function (item)
                        {
                            return item;
                        });
                    });
            }
        };

        $scope.invSkuChangedRequest = null;

        $scope.invSkuChanged = function (index)
        {
            //resolving issue with additional load after lost focus from the input in time of selecting a new element
            if ($scope.invSkuChangedRequest)
            {
                $timeout.cancel($scope.invSkuChangedRequest);
            }
            $scope.invSkuChangedRequest = $timeout(function ()
            {
                if ($scope.options.InvSkuCode)
                {
                    $scope.invSkuFilter.ExactCode = $scope.options.InvSkuCode;
                    $scope.options.InvSkuAddDisabled = true;
                    inventorySkuService.getShortInventorySku($scope.invSkuFilter)
                        .success(function (result)
                        {
                            if (result.Success)
                            {
                                $scope.options.CurrentInvSku = result.Data;
                            } else
                            {
                                errorHandler(result);
                            }
                            $scope.options.InvSkuAddDisabled = false;
                        })
                        .error(function (result)
                        {
                            $scope.options.InvSkuAddDisabled = false;
                            errorHandler(result);
                        });
                    $scope.invSkuChangedRequest = null;
                }
            }, 20);
        };

        $scope.addInvSku = function ()
        {
            if ($scope.options.CurrentInvSku)
            {
                var allowAdd = true;
                $.each($scope.filter.InvSkus, function (index, item)
                {
                    if ($scope.options.CurrentInvSku.Id == item.Id)
                    {
                        allowAdd = false;
                        return false;
                    }
                });
                if (allowAdd)
                {
                    $scope.filter.InvSkus.push($scope.options.CurrentInvSku);
                    $scope.filterChanged();
                }
            }
        };

        $scope.removeInvSku = function (index)
        {
            $scope.filter.InvSkus.splice(index, 1);
            $scope.filterChanged();
        };

        initialize();
    }]);