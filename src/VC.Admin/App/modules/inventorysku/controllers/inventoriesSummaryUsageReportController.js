angular.module('app.modules.inventorysku.controllers.inventoriesSummaryUsageReportController', [])
.controller('inventoriesSummaryUsageReportController', ['$scope', '$rootScope', '$state', '$q', 'inventorySkuService', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $q, inventorySkuService, productService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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
            var ids = [];
            getTreeCategoryIds(data, ids);
            self.Key = ids;
        };

        function errorHandler(result)
        {
            var messages = "";
            if (result.Messages)
            {
                $.each(result.Messages, function (index, value)
                {
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

            inventorySkuService.getInventoriesSummaryUsageReport(data, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.report = result.Data;
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

        function initialize()
        {
            $scope.options = {};
            $scope.assembles = [
                { Key: null, Text: 'All Assemble Types' },
                { Key: true, Text: 'Assembled' },
                { Key: false, Text: 'Not Assembled' }
            ];

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-4m'),
                Sku: null,
                InvSku: null,
                Assemble: null,
                IdsInvCat: null,
            };

            $scope.skusFilter = {
                Code: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };

            $scope.invSkusFilter = {
                Code: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };

            loadCategories();
        }

        function loadCategories()
        {
            inventorySkuService.getInventorySkuCategoriesTree({}, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.rootCategory = {};
                        $scope.rootCategory.SubItems = result.Data;
                        $scope.filterCategories = [{ Key: null, Text: 'All' }];
                        initCategory($scope.rootCategory, 0);

                        $scope.filterChanged();
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

        function getTreeCategoryIds(serviceCategory, ids)
        {
            ids.push(serviceCategory.Id)
            $.each(serviceCategory.SubItems, function (index, category)
            {
                getTreeCategoryIds(category, ids)
            });
        }

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
            var months = $scope.filter.To.getMonth() - $scope.filter.From.getMonth()
                + (12 * ($scope.filter.To.getFullYear() - $scope.filter.From.getFullYear()));
            if (months > 12)
            {
                msg = "Date range can't be more than 12 months.";
            }
            return msg;
        };

        $scope.filterChanged = function ()
        {
            if ($scope.forms.form.$valid)
            {
                var msg = getValidFrequencyMessage();
                if (msg)
                {
                    $scope.options.exportMsg = msg;
                    $scope.options.exportUrl = null;
                    $scope.forms.form.submitted = true;
                    return;
                }
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
                $scope.options.exportUrl = inventorySkuService.getInventoriesSummaryUsageReportFile(data, $rootScope.buildNumber);
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.exportClick = function (event)
        {
            if (!$scope.options.exportUrl)
            {
                $scope.forms.form.submitted = true;
                if ($scope.options.exportMsg)
                {
                    toaster.pop('error', "Error!", $scope.options.exportMsg, null, 'trustedHtml');
                }
                event.preventDefault();
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

        initialize();
    }]);