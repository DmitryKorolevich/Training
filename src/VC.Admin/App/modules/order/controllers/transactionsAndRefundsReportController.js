angular.module('app.modules.order.controllers.transactionsAndRefundsReportController', [])
.controller('transactionsAndRefundsReportController', ['$scope', '$rootScope', '$state', '$q', 'orderService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $q, orderService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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

            return data;
        };

        function refreshItems()
        {
            orderService.getTransactionAndRefundReport(getFilterData(), $scope.refreshTracker)
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
            $scope.customerTypes = angular.copy($rootScope.ReferenceData.CustomerTypes);
            $scope.customerTypes.splice(0, 0, { Key: null, Text: 'All Customer Types' });

            $scope.orderStatuses = angular.copy($rootScope.ReferenceData.OrderStatuses);
            $scope.orderStatuses.splice(0, 0, { Key: null, Text: 'All Order Statuses' });

            $scope.orderTypes = angular.copy($rootScope.ReferenceData.OrderTypes);
            $scope.orderTypes.splice(0, 0, { Key: null, Text: 'All Order Types' });

            $scope.serviceCodes = angular.copy($rootScope.ReferenceData.ServiceCodes);
            $scope.serviceCodes.splice(0, 0, { Key: null, Text: 'All Service Codes' });

            $scope.options = {};

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-1m'),
                IdCustomerType: null,
                IdServiceCode: null,
                CustomerFirstName: null,
                CustomerLastName: null,
                CustomerLastName: null,
                IdCustomer: null,
                IdOrder: null,
                IdOrderStatus: null,
                IdOrderType: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
            };

            $scope.options.exportUrl = orderService.getTransactionAndRefundReportFile(getFilterData(), $rootScope.buildNumber);
            refreshItems();
        }

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.filter.Paging.PageIndex = 1;
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

        $scope.filterChanged = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.options.exportUrl = orderService.getTransactionAndRefundReportFile(getFilterData(), $rootScope.buildNumber);
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        initialize();
    }]);