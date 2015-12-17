angular.module('app.modules.product.controllers.productCategoriesStatisticController', [])
.controller('productCategoriesStatisticController', ['$scope', '$rootScope', '$state', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, productService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshCategoriesTracker = promiseTracker("refreshCategories");
        $scope.refreshSkusTracker = promiseTracker("refreshSkus");

        function errorHandler(result)
        {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function refreshSkus()
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

            productService.getSkusInProductCategoryStatistic(data, $scope.refreshSkusTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.items = result.Data;
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

        function refreshCategories()
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

            productService.getProductCategoriesStatistic(data, $scope.refreshCategoriesTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.rootCategory = result.Data;
                        $scope.categoriesExpanded = false;
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

        var getCategoriesTreeViewScope = function ()
        {
            return angular.element($('.categories .ya-treeview').get(0)).scope();
        };

        $scope.updateCategoriesCollapsed = function (expand)
        {
            var scope = getCategoriesTreeViewScope();
            if (expand)
            {
                scope.expandAll();
            }
            else
            {
                scope.collapseAll();
            }
            $scope.categoriesExpanded = expand;
        };

        function initialize()
        {
            $scope.forms = {};
            $scope.options = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1y'),
                From: currentDate.shiftDate('-1m'),
                IdCategory: null,
            };

            refreshCategories();
            $scope.filterChanged();
        }

        $scope.filterCategories = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.items = null;
                $scope.categoriesExpanded = false;
                refreshCategories();
            }
            else
            {
                $scope.forms.submitted = true;
            }
        };

        $scope.selectCategory = function (id)
        {
            $scope.filter.IdCategory = id;
            refreshSkus();
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
            $scope.options.exportUrl = productService.getProductCategoriesStatisticReportFile(data, $rootScope.buildNumber);
        };

        initialize();
    }]);