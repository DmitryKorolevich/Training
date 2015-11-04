angular.module('app.modules.affiliate.controllers.customersInAffiliatesListController', [])
.controller('customersInAffiliatesListController', ['$scope', '$rootScope', '$state', '$stateParams', 'customerService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $stateParams, customerService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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
            customerService.getCustomers($scope.filter, $scope.refreshTracker)
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
            $scope.id = $stateParams.id ? $stateParams.id : null;

            $scope.items = [];
            $scope.filter = {
                IdAffiliate: $scope.id,
                IdAffiliateRequired: true,
                Paging: { PageIndex: 1, PageItemCount: 100 },
            };

            $scope.report = {
                exportUrl: null,
            };
            $scope.filterChanged();

            refreshItems();
        }

        $scope.filterItems = function ()
        {
            $scope.filter.Paging.PageIndex = 1;
            refreshItems();
        };

        $scope.filterChanged = function ()
        {
            $scope.report.exportUrl = customerService.getCustomersForAffiliatesReportFileUrl($scope.filter, $rootScope.buildNumber);
        };

        $scope.pageChanged = function () {
            refreshItems();
        };

        initialize();
    }]);