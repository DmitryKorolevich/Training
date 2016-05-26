angular.module('app.modules.customer.controllers.wholesalesReportController', [])
.controller('wholesalesReportController', ['$scope', '$rootScope', '$state', 'customerService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, customerService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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
            customerService.getWholesales($scope.filter, $scope.refreshTracker)
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
            $scope.tradeClasses = angular.copy($rootScope.ReferenceData.TradeClasses);
            $scope.tradeClasses.splice(0, 0, { Key: null, Text: 'All Trade Classes' });

            $scope.tiers = angular.copy($rootScope.ReferenceData.Tiers);
            $scope.tiers.splice(0, 0, { Key: null, Text: 'All Tiers' });

            $scope.forms = {};
            $scope.options = {};

            $scope.filter = {
                IdTradeClass: null,
                IdTier: null,
                OnlyActive: true,
            };

            $scope.options.exportUrl = customerService.getWholesalesReportFile($scope.filter, $rootScope.buildNumber);
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

        $scope.pageChanged = function ()
        {
            refreshItems();
        };

        $scope.filterChanged = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.options.exportUrl = customerService.getWholesalesReportFile($scope.filter, $rootScope.buildNumber);
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        initialize();
    }]);