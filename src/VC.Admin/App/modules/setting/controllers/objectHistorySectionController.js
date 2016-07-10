angular.module('app.modules.setting.controllers.objectHistorySectionController', [])
.controller('objectHistorySectionController', ['$scope', '$rootScope', '$state', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, settingService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil) {

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
            $scope.historyItems = null;
            $scope.historyItemsCount = 0;

            var loadFunction = $scope.historyFilter.IdObjectType == 2 ? settingService.getOrderObjectHistoryLogItems : settingService.getObjectHistoryLogItems;//order
            loadFunction($scope.historyFilter, $scope.tracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.historyItems = result.Data.Items;
                        $scope.historyItemsCount = result.Data.Count;
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


        function initialize() {
            $scope.service = null;
            $scope.tracker = null;

            $scope.historyFilter = {
                IdObject: null,
                IdObjectType: null,
                DataReferenceId: null,
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };
        }

        $scope.$on('objectHistorySection#in#refresh', function (event, args)
        {
            $scope.service = args.service;
            $scope.tracker = args.tracker;
            $scope.historyFilter.IdObject = args.idObject;
            $scope.historyFilter.IdObjectType = args.idObjectType;
            refreshItems();
        });

        $scope.openHistoryReport = function (id)
        {
            $scope.historyFilter.DataReferenceId = id;
            $scope.service.getHistoryReport($scope.historyFilter, $scope.tracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        modalUtil.open('app/modules/setting/partials/objectLogReportPopup.html', 'objectLogReportController', result.Data, { size: 'lg' });
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
        
        initialize();
    }]);