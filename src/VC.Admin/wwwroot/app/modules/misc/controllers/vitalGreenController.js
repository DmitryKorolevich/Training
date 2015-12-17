angular.module('app.modules.misc.controllers.vitalGreenController', [])
.controller('vitalGreenController', ['$scope', '$rootScope', '$state', 'vitalGreenService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, vitalGreenService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.deleteTracker = promiseTracker("delete");

        function errorHandler(result)
        {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function refreshReport()
        {
            vitalGreenService.getVitalGreenReport($scope.filter, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.report = result.Data;
                        $scope.filterChanged();
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
            var currentDate = new Date();
            $scope.years = [];
            var year = currentDate.getFullYear() - 15;
            while (year <= currentDate.getFullYear())
            {
                $scope.years.push({ Key: year, Text: year});
                year++;
            }

            $scope.forms = {};
            $scope.report = {};

            var currentDate=new Date();
            $scope.filter = {
                Year: currentDate.getFullYear(),
                Month: currentDate.getMonth()+1
            };
            $scope.exportUrl = null;

            refreshReport();
        }

        $scope.filterReport = function ()
        {
            refreshReport();
        };

        $scope.filterChanged = function ()
        {
            $scope.report.exportUrl = vitalGreenService.getVitalGreenReportFileUrl($scope.filter, $rootScope.buildNumber);
        };

        initialize();
    }]);