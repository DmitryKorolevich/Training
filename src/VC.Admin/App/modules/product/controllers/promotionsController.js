angular.module('app.modules.product.controllers.promotionsController', [])
.controller('promotionsController', ['$scope', '$rootScope', '$state', 'promotionService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, promotionService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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

        function refreshPromotions()
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

            promotionService.getPromotions(data, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.promotions = result.Data.Items;
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
                ValidFrom: null,
                ValidTo: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshPromotions, "DateCreated", "Desc")
            };

            refreshPromotions();
        }

        $scope.filterPromotions = function () {
            $scope.filter.Paging.PageIndex = 1;
            refreshPromotions();
        };

        $scope.pageChanged = function () {
            refreshPromotions();
        };

        $scope.open = function (id) {
            if (id) {
                $state.go('index.oneCol.promotionDetail', { id: id });
            }
            else {
                $state.go('index.oneCol.addNewPromotion');
            }
        };

        $scope.delete = function (id) {
            confirmUtil.confirm(function () {
                promotionService.deletePromotion(id, $scope.deleteTracker)
                    .success(function (result) {
                        if (result.Success) {
                            toaster.pop('success', "Success!", "Successfully deleted.");
                            refreshPromotions();
                        } else {
                            errorHandler(result);
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to delete this promotion?');
        };

        initialize();
    }]);