angular.module('app.modules.affiliate.controllers.affiliatesController', [])
.controller('affiliatesController', ['$scope', '$rootScope', '$state', 'affiliateService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, affiliateService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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

        function refreshAffiliates()
        {
            affiliateService.getAffiliates($scope.filter, $scope.refreshTracker)
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
            $scope.affiliateTiers = angular.copy($rootScope.ReferenceData.AffiliateTiers);
            $scope.affiliateTiers.splice(0, 0, { Key: null, Text: 'All Tiers' });

            $scope.filter = {
                Id: null,
                Tier: null,
                Name: null,
                Company: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshAffiliates, "Name", "Asc")
            };

            refreshAffiliates();
        }

        $scope.filterAffiliates = function ()
        {
            $scope.filter.Paging.PageIndex = 1;
            refreshAffiliates();
        };

        $scope.pageChanged = function () {
            refreshAffiliates();
        };

        $scope.open = function (id) {
            if (id) {
                $state.go('index.oneCol.affiliateDetail', { id: id });
            }
            else {
                $state.go('index.oneCol.affiliateAdd');
            }
        };

        $scope.delete = function (id) {
            confirmUtil.confirm(function () {
                affiliateService.deleteAffiliate(id, $scope.deleteTracker)
                    .success(function (result) {
                        if (result.Success) {
                            toaster.pop('success', "Success!", "Successfully deleted.");
                            refreshAffiliates();
                        } else {
                            errorHandler(result);
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to delete this affiliate?');
        };

        $scope.pay = function (id)
        {
            modalUtil.open('app/modules/affiliate/partials/affiliatePayCommissions.html', 'affiliatePayCommissionsController', {
                id: id, thenCallback: function ()
                {
                    refreshAffiliates();
                }
            }, { size: 'sm' });
        };

        initialize();
    }]);