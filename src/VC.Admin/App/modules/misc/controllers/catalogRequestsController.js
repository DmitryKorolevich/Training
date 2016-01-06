angular.module('app.modules.misc.controllers.catalogRequestsController', [])
.controller('catalogRequestsController', ['$scope', '$rootScope', '$state', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, settingService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.deleteTracker = promiseTracker("delete");

        function errorHandler(result)
        {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function refresh()
        {
            settingService.getCatalogRequests($scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.items = result.Data;
                    } else
                    {
                        errorHandler(result);
                    }
                })
                .error(function (result)
                {
                    errorHandler(result);
                });
        };

        function initialize()
        {
            $scope.forms = {};

            $scope.options = {};
            $scope.options.exportUrl = settingService.getCatalogRequestsReportFileUrl($rootScope.buildNumber);

            refresh();
        }

        $scope.clear = function ()
        {
            confirmUtil.confirm(function ()
            {
                settingService.deleteCatalogRequests($scope.refreshTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            refresh();
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to delete catelog requests?');
        };

        initialize();
    }]);