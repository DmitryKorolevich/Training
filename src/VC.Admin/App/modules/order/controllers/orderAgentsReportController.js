angular.module('app.modules.order.controllers.orderAgentsReportController', [])
.controller('orderAgentsReportController', ['$scope', '$rootScope', '$state', '$q', 'orderService', 'userService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $q, orderService, userService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");

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
            var data = {};
            angular.copy($scope.filter, data);
            if (data.From)
            {
                data.From = data.From.toServerDateTime();
            }
            if (data.To)
            {
                data.To = data.To.toServerDateTime();
            }

            orderService.getOrdersAgentReport(data, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.report = result.Data;
                        initAgentsSorting();
                    } else {
                        errorHandler(result);
                    }
                })
                .error(function (result) {
                    errorHandler(result);
                });
        };

        function initAgentsSorting()
        {
            $.each($scope.report.Periods, function (i, period)
            {
                $.each(period.Teams, function (index, team)
                {
                    team.Sorting = {
                        Path: 'AgentId',
                        SortOrder: 'Asc',

                        applySort: function (sortPath)
                        {
                            if (sortPath != '')
                            {
                                if (this.Path == sortPath)
                                {
                                    this.SortOrder = this.SortOrder == 'Asc' ? 'Desc' : 'Asc';
                                }
                                this.Path = sortPath;

                                var sortOrder = this.SortOrder;
                                var path = this.Path;
                                team.Agents.sort(function (a, b)
                                {
                                    if (sortOrder == 'Asc')
                                    {
                                        if (a[path] < b[path]) return -1;
                                        if (a[path] > b[path]) return 1;
                                        return 0
                                    }
                                    else
                                    {
                                        if (a[path] < b[path]) return 1;
                                        if (a[path] > b[path]) return -1;
                                        return 0
                                    }
                                });
                            }
                        }
                    };
                });
            });
        };

        function initialize()
        {
            $scope.options = {};
            $scope.options.allowSeeFullReport = $rootScope.validatePermission(3);

            $scope.frequencyTypes = [
                {Key: 1, Text: 'Daily'},
                {Key: 2, Text: 'Weekly'},
                {Key: 3, Text: 'Monthly'},
            ];

            $scope.forms = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: currentDate.shiftDate('+1d'),
                From: currentDate.shiftDate('-1m'),
                FrequencyType: 2,//weekly
                IdAdminTeam: null,
                IdAdmin: null,
            };

            $q.all({
                teamsCall: userService.getAdminTeams($scope.refreshTracker),
                adminsCall: userService.getUsers({} ,$scope.refreshTracker),
            }).then(function (result)
            {
                if (result.teamsCall.data.Success && result.adminsCall.data.Success)
                {
                    $scope.teams = result.teamsCall.data.Data;
                    $scope.teams.splice(0, 0, { Id: null, Name: 'All' });
                    if (!$scope.options.allowSeeFullReport)
                    {
                        $scope.filter.IdAdmin = $rootScope.currentUser.Id;
                    }
                    else
                    {
                        $scope.admins = result.adminsCall.data.Data.Items;
                        $scope.admins.splice(0, 0, { Id: null, AgentId: 'All' });
                    }

                    refreshItems();
                }
                else
                {
                    errorHandler(result);
                }
            });
        }

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                if ($scope.filter.From > $scope.filter.To)
                {
                    toaster.pop('error', "Error!", "'To' date can't be less than 'From' date.", null, 'trustedHtml');
                    return;
                }
                $scope.forms.form.submitted = false;
                refreshItems();
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.idAdminTeamChanged = function ()
        {
            if ($scope.filter.IdAdminTeam != null)
            {
                $scope.filter.IdAdmin = null;
            }
        };

        $scope.idAdminChanged = function ()
        {
            if ($scope.filter.IdAdmin != null)
            {
                $scope.filter.IdAdminTeam = null;
            }
        };

        initialize();
    }]);