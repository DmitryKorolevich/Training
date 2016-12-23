angular.module('app.modules.order.controllers.orderAbuseReportController', [])
.controller('orderAbuseReportController', ['$scope', '$rootScope', '$state', '$q', '$timeout', 'orderService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $q, $timeout, orderService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");

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
            if (data.CountMode == 1)
            {
                data.Refunds = null;
                data.ReshipsOrRefunds = null;
            }
            if (data.CountMode == 2)
            {
                data.Reships = null;
                data.ReshipsOrRefunds = null;
            }
            if (data.CountMode == 3)
            {
                data.Reships = null;
                data.Refunds = null;
            }

            return data;
        };

        function refreshItems()
        {
            var data = getFilterData();

            if (data.Mode == 1)
            {
                orderService.getOrderAbuseReportItems(data, $scope.refreshTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            $scope.items = result.Data.Items;
                            $scope.totalItems = result.Data.Count;
                            $scope.options.UsedMode = data.Mode;
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
                orderService.getCustomerAbuseItems(data, $scope.refreshTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            $scope.items = result.Data.Items;
                            $scope.totalItems = result.Data.Count;
                            $scope.options.UsedMode = data.Mode;
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
        };

        function initialize()
        {
            $scope.options = {};
            $scope.options.ServiceCodes = angular.copy($rootScope.ReferenceData.ServiceCodes);
            $scope.options.ServiceCodes.splice(0, 0, { Key: null, Text: 'All Service Codes' });
            $scope.options.Modes = [
                { Key: 1, Text: 'Display by Order' },
                { Key: 2, Text: 'Display by Customer' },
            ];
            $scope.options.CountModes = [
                { Key: 1, Text: 'Search by Reships' },
                { Key: 2, Text: 'Search by Refunds' },
                { Key: 3, Text: 'Search by Reships or Refunds' },
            ];

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate,
                From: currentDate.shiftDate('-3m'),
                Mode: 1,
                CountMode: 1,
                Reships: null,
                Refunds: null,
                ReshipsOrRefunds: null,
                IdServiceCode: null,
                IdCustomer: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshItems, "DateCreated", "Desc")
            };

            $scope.filterChanged();
            refreshItems();
        }

        $scope.filterChanged = function ()
        {
            var data = getFilterData();

            if (data.Mode == 1)
            {
                $scope.options.exportUrl = orderService.getOrderAbuseReportItemsReportFile(data, $rootScope.buildNumber);
            }
            if (data.Mode == 2)
            {
                $scope.options.exportUrl = orderService.getCustomerAbuseItemsReportFile(data, $rootScope.buildNumber);
            }
        };

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

        initialize();
    }]);