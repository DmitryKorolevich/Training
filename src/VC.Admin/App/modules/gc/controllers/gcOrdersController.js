angular.module('app.modules.gc.controllers.gcOrdersController', [])
.controller('gcOrdersController', ['$scope', '$rootScope', '$state', 'gcService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, gcService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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

        function refresh()
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

            gcService.getGiftCertificatesWithOrderInfo(data, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.items = result.Data.Items;
                        $scope.options.Count = result.Data.Count;
                        $scope.options.Total = result.Data.Total;
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
            $scope.types = Object.clone($rootScope.ReferenceData.GCTypes);
            $scope.types.splice(0, 0, { Key: null, Text: 'All' });

            $scope.forms = {};
            $scope.options = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-1y'),
                ShippingAddress: { LastName: null },
                BillingAddress: { LastName: null },
                Type: null,
                StatusCode: null,
                NotZeroBalance: false,
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refresh, "Created", "Desc")
            };

            refresh();
            $scope.filterChanged();
        }

        $scope.filterData = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.filter.Paging.PageIndex = 1;
                refresh();
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.pageChanged = function () {
            refresh();
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
            $scope.options.exportUrl = gcService.getGiftCertificatesWithOrderInfoReportFile(data, $rootScope.buildNumber);
        };

        initialize();
    }]);