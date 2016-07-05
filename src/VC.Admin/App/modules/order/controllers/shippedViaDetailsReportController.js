angular.module('app.modules.order.controllers.shippedViaDetailsReportController', [])
.controller('shippedViaDetailsReportController', ['$scope', '$rootScope', '$state', '$stateParams', '$q', 'orderService', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $stateParams, $q, orderService, settingService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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
            orderService.getShippedViaItemsReportOrderItems(getFilterData(), $scope.refreshTracker)
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
            $scope.serviceCodes = angular.copy($rootScope.ReferenceData.ServiceCodes);
            $scope.serviceCodes.splice(0, 0, { Key: 0, Text: 'All Codes' });
            $scope.serviceCodes.splice(0, 0, { Key: -1, Text: 'None' });
            $scope.serviceCodes.splice(0, 0, { Key: null, Text: 'All Orders' });
            $scope.states = [];

            $scope.warehouses = [
                { Key: null, Text: 'All Warehouses' },
                { Key: 1, Text: 'WA' },
                { Key: 2, Text: 'VA' }
            ];

            $scope.shipMethodTypes = angular.copy($rootScope.ReferenceData.ShipMethodTypes);
            $scope.shipMethodTypes.splice(0, 0, { Key: null, Text: 'All Ship Service' });

            $scope.carriers = angular.copy($rootScope.ReferenceData.Carriers);
            $scope.carriers.splice(0, 0, { Key: null, Text: 'All Carriers' });

            $scope.options = {};

            $scope.forms = {};

            var from = $stateParams.from;
            var to = $stateParams.to;
            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            if (to)
            {
                to = Date.parseDateTime(to);
            }
            else
            {
                to = currentDate.shiftDate('+1d');
            }
            if (from)
            {
                from = Date.parseDateTime(from);
            }
            else
            {
                from = currentDate.shiftDate('-1m');
            }
            $scope.filter = {
                To: to,
                From: from,
                IdState: $stateParams.idstate ? $stateParams.idstate : null,
                IdServiceCode: $stateParams.idservicecode ? $stateParams.idservicecode : null,
                IdWarehouse: $stateParams.warehouse ? $stateParams.warehouse : null,
                IdShipService: $stateParams.shipmethodtype ? $stateParams.shipmethodtype : null,
                Carrier: $stateParams.carrier ? $stateParams.carrier : null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
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
                        $scope.options.exportUrl = orderService.getShippedViaItemsReportOrderItemsReportFile(getFilterData(), $rootScope.buildNumber);
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
                $scope.options.exportUrl = orderService.getShippedViaItemsReportOrderItemsReportFile(getFilterData(), $rootScope.buildNumber);
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        initialize();
    }]);