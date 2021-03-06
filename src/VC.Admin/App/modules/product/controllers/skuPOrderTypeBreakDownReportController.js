﻿angular.module('app.modules.product.controllers.skuPOrderTypeBreakDownReportController', [])
.controller('skuPOrderTypeBreakDownReportController', ['$scope', '$rootScope', '$state', '$q', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $q, productService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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

            if ($scope.filter.Future)
            {
                productService.getSkuPOrderTypeFutureBreakDownReport(data, $scope.refreshTracker)
                    .success(function (result) {
                        if (result.Success) {
                            $scope.report = result.Data;
                        } else {
                            errorHandler(result);
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
            }
            else
            {
                productService.getSkuPOrderTypeBreakDownReport(data, $scope.refreshTracker)
                    .success(function (result) {
                        if (result.Success) {
                            $scope.report = result.Data;
                            updateSkuUrls();
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

            $scope.frequencyTypes = [
                {Key: 1, Text: 'Daily'},
                {Key: 2, Text: 'Weekly'},
                {Key: 3, Text: 'Monthly'},
            ];

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-3d'),
                Future: false,
                FrequencyType: 1,//daily
            };

            $scope.skusFilter = {
                Code: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };

            $scope.filterChanged();
            refreshItems();
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
            if ($scope.filter.FrequencyType == 3)
            {
                var months = $scope.filter.To.getMonth() - $scope.filter.From.getMonth()
                    + (12 * ($scope.filter.To.getFullYear() - $scope.filter.From.getFullYear()));
                if (months > 12)
                {
                    msg = "Date range can't be more than 12 months.";
                }
            }
            if ($scope.filter.FrequencyType == 2)
            {
                var days = Math.round(Math.abs($scope.filter.To.getTime() - $scope.filter.From.getTime()) / 8.64e7);
                var weeks = Math.round(days / 7);
                if (weeks > 12)
                {
                    msg = "Date range can't be more than 12 weeks.";
                }
            }
            if ($scope.filter.FrequencyType == 1)
            {
                var days = Math.round(Math.abs($scope.filter.To.getTime() - $scope.filter.From.getTime()) / 8.64e7);
                if (days > 12)
                {
                    msg = "Date range can't be more than 12 days.";
                }
            }
            return msg;
        };

        $scope.filterChanged = function ()
        {
            return;

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

                //$scope.options.exportUrl = productService.getOrdersAgentReportFile(data, $rootScope.buildNumber);
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

        function updateSkuUrls()
        {
            $.each($scope.report.Skus, function (index, item)
            {
                item.Url = '/orders?idsku={0}'.format(item.IdSku);
                if ($scope.filter.From)
                {
                    item.Url += '&from=' + $scope.filter.From.toQueryParamDateTime();
                }
                if ($scope.filter.To)
                {
                    item.Url += '&to=' + $scope.filter.To.toQueryParamDateTime();
                }
            });
        };


        $scope.futureChanged = function ()
        {
            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            if ($scope.filter.Future)
            {
                $scope.filter.To = currentDate.shiftDate('+42d');
                $scope.filter.From = currentDate;
                $scope.filter.FrequencyType = 2;//weekly
            }
            else
            {
                $scope.filter.To = currentDate.shiftDate('+1d');
                $scope.filter.From = currentDate.shiftDate('-3d');
                $scope.filter.FrequencyType = 1;//daily
            }
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

        initialize();
    }]);