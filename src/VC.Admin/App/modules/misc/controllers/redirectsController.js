angular.module('app.modules.misc.controllers.redirectsController', [])
.controller('redirectsController', ['$scope', '$rootScope', '$state', 'redirectService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, redirectService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
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

        function refresh()
        {
            redirectService.getRedirects($scope.filter, $scope.refreshTracker)
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
                Sorting: gridSorterUtil.resolve(refresh, "From", "Asc")
            };

            refresh();
        }

        $scope.pageChanged = function () {
            refresh();
        };

        $scope.open = function (id)
        {
            modalUtil.open('app/modules/misc/partials/addEditRedirectPopup.html', 'addEditRedirectController', {
                id: id, thenCallback: function ()
                {
                    $scope.filter.Paging.PageIndex = 1;
                    refresh();
                }
            }, { size: 'sm' });
        };

        $scope.delete = function (id) {
            confirmUtil.confirm(function () {
                redirectService.deleteRedirect(id, $scope.deleteTracker)
                    .success(function (result) {
                        if (result.Success) {
                            toaster.pop('success', "Success!", "Successfully deleted.");
                            refresh();
                        } else {
                            errorHandler(result);
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to delete this redirect?');
        };

        initialize();
    }]);