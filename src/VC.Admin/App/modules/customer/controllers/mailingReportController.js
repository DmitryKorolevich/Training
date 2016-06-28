angular.module('app.modules.customer.controllers.mailingReportController', [])
.controller('mailingReportController', ['$scope', '$rootScope', '$state', '$q', 'orderService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
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
            if (data.FromFirst)
            {
                data.FromFirst = data.FromFirst.toServerDateTime();
            }
            if (data.ToFirst)
            {
                data.ToFirst = data.ToFirst.toServerDateTime();
            }
            if (data.FromLast)
            {
                data.FromLast = data.FromLast.toServerDateTime();
            }
            if (data.ToLast)
            {
                data.ToLast = data.ToLast.toServerDateTime();
            }

            return data;
        };

        function refreshItems()
        {
            orderService.getMailingReportItems(getFilterData(), $scope.refreshTracker)
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
            $scope.orderSources = angular.copy($rootScope.ReferenceData.OrderSources);
            $scope.orderSources.splice(0, 0, { Key: null, Text: 'All First Order Source' });

            $scope.customerTypes = angular.copy($rootScope.ReferenceData.CustomerTypes);
            $scope.customerTypes.splice(0, 0, { Key: null, Text: 'All Customer Types' });

            $scope.checkboxOptions = [
                { Key: null, Text: 'All' },
                { Key: true, Text: 'Yes' },
                { Key: false, Text: 'No' },
            ];

            $scope.options = {};

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-7d'),
                CustomerIdObjectType: null,
                FromOrderCount: null,
                ToOrderCount: null,
                FromFirst: null,
                ToFirst: null,
                FromLast: null,
                ToLast: null,
                LastFromTotal: null,
                LastToTotal: null,
                DNM: null,
                DNR: null,
                IdCustomerOrderSource: null,
                KeyCodeFirst: null,
                DiscountCodeFirst: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
            };

            $scope.options.exportUrl = orderService.getMailingReportItemsReportFile(getFilterData(), $rootScope.buildNumber);
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
                $scope.options.exportUrl = orderService.getMailingReportItemsReportFile(getFilterData(), $rootScope.buildNumber);
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        initialize();
    }]);