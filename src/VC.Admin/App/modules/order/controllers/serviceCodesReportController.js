angular.module('app.modules.order.controllers.serviceCodesReportController', [])
.controller('serviceCodesReportController', ['$scope', '$rootScope', '$state', 'orderService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, orderService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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

            orderService.getServiceCodesReport(data, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.report = result.Data;
                        $.each($scope.report.Items, function (index, item)
                        {
                            item.AbsReshipsAmount = Math.abs(item.ReshipsAmount);
                            item.AbsRefundsAmount = Math.abs(item.RefundsAmount);
                            item.AbsReturnsAmount = Math.abs(item.ReturnsAmount);
                            item.AbsTotalAmount = Math.abs(item.TotalAmount);
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
            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-1m'),
            };

            refreshItems();
        }

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.forms.form.submitted = false;
                refreshItems();
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        initialize();
    }]);