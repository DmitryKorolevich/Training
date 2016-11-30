angular.module('app.modules.product.controllers.averageDailySalesReportController', [])
.controller('averageDailySalesReportController', ['$scope', '$rootScope', '$state', '$q', '$timeout', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $q, $timeout, productService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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

        function getFilterData()
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

            return data;
        };

        function refreshItems()
        {
            var data = getFilterData();

            if (data.Mode == 1)
            {
                productService.getSkuAverageDailySalesBySkuReportItems(data, $scope.refreshTracker)
                    .success(function (result) {
                        if (result.Success)
                        {
                            $scope.items = result.Data.Items;
                            $scope.totalItems = result.Data.Count;
                            $scope.options.UsedMode = data.Mode;
                        } else {
                            errorHandler(result);
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
            }
            if ($scope.filter.Mode == 2)
            {
                productService.getSkuAverageDailySalesByProductReportItems(data, $scope.refreshTracker)
                    .success(function (result) {
                        if (result.Success)
                        {
                            $scope.items = result.Data.Items;
                            $scope.totalItems = result.Data.Count;
                            $scope.options.UsedMode = data.Mode;
                        } else {
                            errorHandler(result);
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
            }
        };

        function initialize()
        {
            $scope.options = {};

            $scope.customerTypes = angular.copy($rootScope.ReferenceData.CustomerTypes);
            $scope.customerTypes.splice(0, 0, { Key: null, Text: 'All Customer Types' });
            $scope.modes = [
                { Key: 1, Text: 'Display by SKU' },
                { Key: 2, Text: 'Display by Product' },
            ];

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate,
                From: currentDate.shiftDate('-3m'),
                Mode: 1,
                IdCustomerType: null,
                OnlyActiveSku: true,
                OnlyOOS: false,
                ProductName: null,
                Skus: [],
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshItems, "TotalOOSImpactAmount", "Desc")
            };

            $scope.skusFilter = {
                Code: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };

            $scope.skuFilter = {
                ExactCode: '',
                Paging: { PageIndex: 1, PageItemCount: 1 },
            };

            $scope.productsFilter = {
                Code: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };

            refreshItems();
        }

        $scope.pageChanged = function ()
        {
            refreshItems();
        };

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                var msg = getValidFrequencyMessage();
                if (msg == null)
                {
                    refreshItems();
                }
                else
                {
                    $scope.forms.form.submitted = true;
                    toaster.pop('error', "Error!", msg, null, 'trustedHtml');
                }
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        var getValidFrequencyMessage = function ()
        {
            var msg = null;
            if ($scope.filter.From > $scope.filter.To)
            {
                msg = "'To' date can't be less than 'From' date.";
            }
            return msg;
        };
        
        $scope.exportClick = function (event)
        {
            if ($scope.forms.form.$valid)
            {
                var msg = getValidFrequencyMessage();
                if (msg == null)
                {
                    if ($scope.filter.Mode == 1)
                    {
                        productService.requestSkuAverageDailySalesBySkuReportFile(getFilterData(), $scope.refreshTracker)
                           .success(function (result)
                           {
                               if (result.Success)
                               {
                                   var id = result.Data;
                                   var url = productService.getSkuAverageDailySalesBySkuReportFile(id, $rootScope.buildNumber);
                                   $rootScope.downloadFileIframe(url);
                               } else
                               {
                                   errorHandler(result);
                               }
                           })
                           .error(function (result)
                           {
                               errorHandler(result);
                           });
                    }
                    if ($scope.filter.Mode == 2)
                    {
                        productService.requestSkuAverageDailySalesByProductReportFile(getFilterData(), $scope.refreshTracker)
                           .success(function (result)
                           {
                               if (result.Success)
                               {
                                   var id = result.Data;
                                   var url = productService.getSkuAverageDailySalesByProductReportFile(id, $rootScope.buildNumber);
                                   $rootScope.downloadFileIframe(url);
                               } else
                               {
                                   errorHandler(result);
                               }
                           })
                           .error(function (result)
                           {
                               errorHandler(result);
                           });
                    }
                }
                else
                {
                    $scope.forms.form.submitted = true;
                    toaster.pop('error', "Error!", msg, null, 'trustedHtml');
                }                
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.getProductsByProduct = function (val)
        {
            if (val)
            {
                $scope.productsFilter.SearchText = val;
                return productService.getShortProducts($scope.productsFilter)
                    .then(function (result)
                    {
                        return result.data.Data.Items.map(function (item)
                        {
                            return item;
                        });
                    });
            }
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
                            $scope.skuChangedRequest = null;
                        })
                        .error(function (result)
                        {
                            $scope.options.SkuAddDisabled = false;
                            errorHandler(result);
                            $scope.skuChangedRequest = null;
                        });
                }
            }, 100);
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
                }
            }
        };

        $scope.removeSku = function (index)
        {
            $scope.filter.Skus.splice(index, 1);
        };

        initialize();
    }]);