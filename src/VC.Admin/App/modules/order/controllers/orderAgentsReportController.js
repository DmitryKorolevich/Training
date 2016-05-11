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
            var ids = [];
            if (data.IdAdminTeams.length > 0)
            {
                for (i = 0; i < data.IdAdminTeams.length; i++)
                {
                    if (data.IdAdminTeams[i] != null)
                    {
                        ids.push(data.IdAdminTeams[i]);
                    }
                }
            }
            data.IdAdminTeams = ids;

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
                IdAdminTeams: [null],
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

                    $scope.filterChanged();
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
                var msg = getValidFrequencyMessage();
                if (msg == null)
                {
                    refreshItems();
                }
                else
                {
                    $scope.forms.form.submitted = true;
                    toaster.pop('error', "Error!", msg, null, 'trustedHtml');
                }
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        var getValidFrequencyMessage = function ()
        {
            var msg = null;
            if ($scope.filter.From > $scope.filter.To)
            {
                msg = "'To' date can't be less than 'From' date.";
            }
            if ($scope.filter.FrequencyType == 3)
            {
                var months = $scope.filter.To.getMonth() - $scope.filter.From.getMonth()
                    + (12 * ($scope.filter.To.getFullYear() - $scope.filter.From.getFullYear()));
                if (months > 12)
                {
                    msg = "Date range can't be more than 12 months.";
                }
            }
            if ($scope.filter.FrequencyType == 2)
            {
                var days = Math.round(Math.abs($scope.filter.To.getTime() - $scope.filter.From.getTime()) / 8.64e7);
                var weeks = Math.round(days / 7);
                if (weeks > 16)
                {
                    msg = "Date range can't be more than 16 weeks.";
                }
            }
            if ($scope.filter.FrequencyType == 1)
            {
                var days = Math.round(Math.abs($scope.filter.To.getTime() - $scope.filter.From.getTime()) / 8.64e7);
                if (days > 14)
                {
                    msg = "Date range can't be more than 14 days.";
                }
            }
            return msg;
        };

        $scope.filterChanged = function ()
        {
            if ($scope.forms.form.$valid)
            {
                var msg = getValidFrequencyMessage();
                if (msg)
                {
                    $scope.options.exportMsg = msg;
                    $scope.options.exportUrl = null;
                    $scope.forms.form.submitted = true;
                    return;
                }
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
                data.sIdAdminTeams = '';
                if (data.IdAdminTeams.length > 0)
                {
                    for (i = 0; i < data.IdAdminTeams.length; i++)
                    {
                        if (data.IdAdminTeams[i] != null)
                        {
                            data.sIdAdminTeams += data.IdAdminTeams[i];
                            if (i != data.IdAdminTeams.length - 1)
                            {
                                data.sIdAdminTeams += ',';
                            }
                        }
                    }
                }
                $scope.options.exportUrl = orderService.getOrdersAgentReportFile(data, $rootScope.buildNumber);
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.exportClick = function (event)
        {
            if (!$scope.options.exportUrl)
            {
                $scope.forms.form.submitted = true;
                if ($scope.options.exportMsg)
                {
                    toaster.pop('error', "Error!", $scope.options.exportMsg, null, 'trustedHtml');
                }
                event.preventDefault();
            }
        };

        $scope.idAdminTeamChanged = function ()
        {
            if ($scope.filter.IdAdminTeams.length == 1 || $scope.filter.IdAdminTeams[0]==null)
            {
                $scope.filter.IdAdmin = null;
            }
            filterChanged();
        };

        $scope.idAdminChanged = function ()
        {
            if ($scope.filter.IdAdmin != null)
            {
                $scope.filter.IdAdminTeams = [null];
            }
            filterChanged();
        };

        initialize();
    }]);