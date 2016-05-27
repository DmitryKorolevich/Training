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

        function refreshDiscounts()
        {
            var data = {};
            angular.copy($scope.filter, data);
            if (data.ValidFrom)
            {
                data.ValidFrom = data.ValidFrom.toServerDateTime();
            }
            if (data.ValidTo)
            {
                data.ValidTo = data.ValidTo.toServerDateTime();
            }

            discountService.getDiscounts(data, $scope.refreshTracker)
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
            $scope.customerTypes.splice(0, 0, { Key: null, Text: 'All Customer Types' });

            $scope.filter = {
                SearchText: '',
                Status: null,
                ValidFrom: null,
                ValidTo: null,
                DateStatus: 2,//live
                SearchByAssigned: true,
                Assigned: 1,//retail
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshDiscounts, "Code", "Asc")
            };

            refreshDiscounts();
        }

        $scope.filterDiscounts = function ()
        {
            if ($scope.filter.ValidFrom > $scope.filter.ValidTo)
            {
                toaster.pop('error', "Error!", "'Valid To' date can't be less than 'Valid From' date.", null, 'trustedHtml');
                return;
            }
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