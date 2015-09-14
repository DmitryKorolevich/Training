angular.module('app.modules.product.controllers.discountsController', [])
.controller('discountsController', ['$scope', '$rootScope', '$state', 'discountService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, discountService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil) {
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

        function refreshDiscounts() {
            discountService.getDiscounts($scope.filter, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.discounts = result.Data.Items;
                        $scope.totalItems = result.Data.Count;
                    } else {
                        errorHandler(result);
                    }
                })
                .error(function (result) {
                    errorHandler(result);
                });
        };

        function initialize() {
            $scope.activeFilterOptions = $rootScope.ReferenceData.ActiveFilterOptions;

            $scope.customerTypes = angular.copy($rootScope.ReferenceData.CustomerTypes);
            $scope.customerTypes.splice(0, 0, { Key: null, Text: 'All' });

            $scope.filter = {
                SearchText: '',
                Status: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshDiscounts, "DateCreated", "Desc")
            };

            refreshDiscounts();
        }

        $scope.filterDiscounts = function () {
            $scope.filter.Paging.PageIndex = 1;
            refreshDiscounts();
        };

        $scope.pageChanged = function () {
            refreshDiscounts();
        };

        $scope.open = function (id) {
            if (id) {
                $state.go('index.oneCol.discountDetail', { id: id });
            }
            else {
                $state.go('index.oneCol.addNewDiscount');
            }
        };

        $scope.delete = function (id) {
            confirmUtil.confirm(function () {
                discountService.deleteDiscount(id, $scope.deleteTracker)
                    .success(function (result) {
                        if (result.Success) {
                            toaster.pop('success', "Success!", "Successfully deleted.");
                            refreshDiscounts();
                        } else {
                            errorHandler(result);
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to delete this discount?');
        };

        initialize();
    }]);