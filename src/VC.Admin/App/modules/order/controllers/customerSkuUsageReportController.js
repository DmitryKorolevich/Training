angular.module('app.modules.order.controllers.customerSkuUsageReportController', [])
.controller('customerSkuUsageReportController', ['$scope', '$rootScope', '$state', '$timeout', '$q', 'orderService', 'productService', 'discountService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $timeout, $q, orderService, productService, discountService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function Category(data, level)
        {
            var self = this;
            var namePrefix = '';
            for (var i = 0; i < level; i++)
            {
                namePrefix += '----';
            }
            self.Text = namePrefix + data.Name;
            self.Key = data.Id;
        };

        function errorHandler(result) {
            var messages = "";
            if (result.Messages) {
                $.each(result.Messages, function (index, value) {
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        };

        function getFilterData(){
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
            var filter = getFilterData();
            filter.Exclude = [];
            orderService.getCustomerSkuUsageReportItems(filter, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success)
                    {
                        $scope.items = result.Data.Items;
                        $scope.totalItems = result.Data.Count;

                        $scope.options.allExlude = true;
                        $.each($scope.items, function (index, item)
                        {
                            item.IsSelected = true;
                            $.each($scope.filter.Exclude, function (index, excludeItem)
                            {
                                if (item.IdCustomer == excludeItem.Key && item.IdSku == excludeItem.Value)
                                {
                                    item.IsSelected = false;
                                    $scope.options.allExlude = false;
                                }
                            });
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

            $scope.forms = {};

            $scope.customerTypes = angular.copy($rootScope.ReferenceData.CustomerTypes);
            $scope.customerTypes.splice(0, 0, { Key: null, Text: 'All Customer Types' });

            $scope.skusFilter = {
                Code: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };

            $scope.skuFilter = {
                ExactCode: '',
                Paging: { PageIndex: 1, PageItemCount: 1 },
            };

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-1m'),
                IdCustomerType: null,
                IdCategory: null,
                Skus: [],
                Exclude: [],
                Paging: { PageIndex: 1, PageItemCount: 100 },
            };
            $scope.options.allExlude = true;

            loadCategories();
        }

        function loadCategories()
        {
            productService.getCategoriesTree({}, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.rootCategory = {};
                        $scope.rootCategory.SubItems = result.Data.SubItems;
                        $scope.filterCategories = [{ Key: null, Text: 'All Categories' }];
                        initCategory($scope.rootCategory, 0);

                        refreshItems();
                    } else
                    {
                        errorHandler(result);
                    }
                })
                .error(function (result)
                {
                    errorHandler(result);
                });
        };

        function initCategory(serviceCategory, level)
        {
            if (serviceCategory.Name)
            {
                $scope.filterCategories.push(new Category(serviceCategory, level));
            }
            $.each(serviceCategory.SubItems, function (index, category)
            {
                initCategory(category, level + 1)
            });
        }

        function compareId(a, b)
        {
            return b-a;
        }

        $scope.itemExludeChanged = function (item, event)
        {
            if (item.IsSelected)
            {
                $scope.filter.Exclude = jQuery.grep($scope.filter.Exclude, function (value)
                {
                    var remove=false;
                    if(value.Key == item.IdCustomer && value.Value == item.IdSku)
                    {
                        remove=true;
                    }
                    return !remove;
                });
                
                $scope.options.allExlude = true;
                $.each($scope.items, function (index, innerItem)
                {
                    if (!innerItem.IsSelected)
                    {
                        $scope.options.allExlude = false;
                        return false;
                    }
                });
            }
            else
            {
                $scope.filter.Exclude.push({
                    Key: item.IdCustomer,
                    Value: item.IdSku
                });
                $scope.options.allExlude = false;
            }
        };

        $scope.allItemExludeCall = function ()
        {
            if ($scope.options.allExlude)
            {
                $scope.filter.Exclude = jQuery.grep($scope.filter.Exclude, function (value)
                {
                    var remove=false;
                    $.each($scope.items, function (index, item)
                    {
                        if(value.Key == item.IdCustomer && value.Value == item.IdSku)
                        {
                            remove=true;
                        }
                    });
                    return !remove;
                });
            }
            else
            {
                $.each($scope.items, function (index, item)
                {
                    $scope.filter.Exclude.push({
                        Key: item.IdCustomer,
                        Value: item.IdSku
                    });
                });
            }
            $.each($scope.items, function (index, item)
            {
                item.IsSelected = $scope.options.allExlude;
            });
        };

        $scope.exportItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                orderService.requestCustomerSkuUsageReportFile(getFilterData(), $scope.refreshTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            var id = result.Data;
                            var url = orderService.getCustomerSkuUsageReportFile(id, $rootScope.buildNumber);
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
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.filter.Exclude = [];
                $scope.options.allExlude = true;
                refreshItems();
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.pageChanged = function ()
        {
            refreshItems();
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

        initialize();
    }]);