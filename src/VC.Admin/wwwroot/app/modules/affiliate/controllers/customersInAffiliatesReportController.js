angular.module('app.modules.affiliate.controllers.customersInAffiliatesReportController', [])
.controller('customersInAffiliatesReportController', ['$scope', '$rootScope', '$state', 'affiliateService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, affiliateService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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
            affiliateService.getCustomerInAffiliateReport($scope.filter, $scope.refreshTracker)
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
            $scope.filter = {
                Paging: { PageIndex: 1, PageItemCount: 100 },
            };

            refreshItems();
        }

        $scope.filterItems = function ()
        {
            $scope.filter.Paging.PageIndex = 1;
            refreshItems();
        };

        $scope.pageChanged = function () {
            refreshItems();
        };

        initialize();
    }]);