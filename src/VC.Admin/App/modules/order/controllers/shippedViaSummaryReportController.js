angular.module('app.modules.order.controllers.shippedViaSummaryReportController', [])
.controller('shippedViaSummaryReportController', ['$scope', '$rootScope', '$state', '$q', 'orderService', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $q, orderService, settingService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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
            orderService.getShippedViaSummaryReport(getFilterData(), $scope.refreshTracker)
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
        };

        function initialize()
        {
            $scope.serviceCodes = angular.copy($rootScope.ReferenceData.ServiceCodes);
            $scope.serviceCodes.splice(0, 0, { Key: 0, Text: 'All Codes' });
            $scope.serviceCodes.splice(0, 0, { Key: -1, Text: 'None' });
            $scope.serviceCodes.splice(0, 0, { Key: null, Text: 'All Orders' });
            $scope.states = [];

            $scope.options = {};

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-1m'),
                IdState: null,
                IdServiceCode: null,
            };

            settingService.getCountries({}, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.state = result.Data;
                        $.each(result.Data, function (index, country)
                        {
                            if (country.CountryCode == 'US')
                            {
                                $scope.states = country.States;
                                $scope.states.splice(0, 0, { Id: null, StateName: 'All' });
                            }
                        });
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
        }

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                refreshItems();
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.openDetails = function (warehouse, shipMethodType, carrier)
        {
            var data = {
                from: $scope.filter.From,
                to: $scope.filter.To,
                warehouse: warehouse,
                shipmethodtype: shipMethodType,
                carrier: carrier,
            };
            if (data.from)
            {
                data.from = data.from.toQueryParamDateTime();
            }
            if (data.to)
            {
                data.to = data.to.toQueryParamDateTime();
            }
            if ($scope.filter.IdState)
            {
                data.idstate = $scope.filter.IdState;
            }
            if ($scope.filter.IdOrdIdServiceCodeerType)
            {
                data.idservicecode = $scope.filter.IdServiceCode;
            }

            $state.go('index.oneCol.shippedViaDetailsReport', data);
        };

        initialize();
    }]);