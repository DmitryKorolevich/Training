angular.module('app.modules.help.controllers.bugTicketsController', [])
.controller('bugTicketsController', ['$scope', '$rootScope', '$state', 'helpService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, helpService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.deleteTracker = promiseTracker("delete");

        function errorHandler(result) {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function refreshItems() {
            helpService.getBugTickets($scope.filter, $scope.refreshTracker)
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
            $scope.priorities = angular.copy($rootScope.ReferenceData.Priorities);
            $scope.priorities.splice(0, 0, { Key: null, Text: 'All' });

            $scope.statuses = angular.copy($rootScope.ReferenceData.TicketStatuses);
            $.each($scope.statuses, function (index, item)
            {
                item.Text="Show " + item.Text;
            });
            $scope.statuses.splice(0, 0, { Key: null, Text: 'Show All' });

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-3m'),
                Priority: null,
                StatusCode: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshItems, "DateCreated", "Desc")
            };

            refreshItems();
        }

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.filter.Paging.PageIndex = 1;
                refreshItems();
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.pageChanged = function () {
            refreshItems();
        };

        $scope.delete = function (id)        {
            confirmUtil.confirm(function ()
            {
               helpService.deleteBugTicket(id, $scope.deleteTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            toaster.pop('success', "Success!", "Successfully deleted.");
                            refreshItems();
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to delete this bug ticket?');
        };

        initialize();
    }]);