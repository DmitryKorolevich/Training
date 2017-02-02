angular.module('app.modules.setting.controllers.orderReviewRulesController', [])
.controller('orderReviewRulesController', ['$scope', '$rootScope', '$state', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil', 'Upload',
    function ($scope, $rootScope, $state, settingService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil, Upload)
    {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.deleteTracker = promiseTracker("delete");

        function errorHandler(result) {
            var messages = "";
            if (result.Messages) {
                $.each(result.Messages, function (index, value) {
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        };

        function refreshItems() {
            settingService.getOrderReviewRules($scope.filter, $scope.refreshTracker)
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
            $scope.options = {};

            $scope.filter = {
                SearchText: null,
                Description: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshItems, "DateCreated", "Desc")
            };

            refreshItems();
        }

        $scope.filterItems = function () {
            $scope.filter.Paging.PageIndex = 1;
            refreshItems();
        };

        $scope.pageChanged = function () {
            refreshItems();
        };

        $scope.delete = function (id) {
            confirmUtil.confirm(function () {
                settingService.deleteOrderReviewRule(id, $scope.deleteTracker)
                    .success(function (result) {
                        if (result.Success) {
                            toaster.pop('success', "Success!", "Successfully deleted.");
                            refreshItems();
                        } else {
                            errorHandler(result);
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to delete this order review rule?');
        };

        initialize();
    }]);