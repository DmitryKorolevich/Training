angular.module('app.modules.setting.controllers.profileScopesController', [])
.controller('profileScopesController', ['$scope', '$state', '$stateParams', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
    function ($scope, $state, $stateParams, settingService, toaster, modalUtil, confirmUtil, promiseTracker)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function errorHandler(result)
        {
            var messages = "";
            if (result.Messages)
            {
                $.each(result.Messages, function (index, value)
                {
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        };

        function refreshItems()
        {
            settingService.getProfileScopeItems($scope.filter, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.options.Items = result.Data.Items;
                        $scope.totalItems = result.Data.Count;
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
            $scope.options = {};
            $scope.filter = {
                Paging: { PageIndex: 1, PageItemCount: 25 },
            };

            refreshItems();
        };

        $scope.pageChanged = function ()
        {
            refreshItems();
        };

        $scope.updateCategoriesCollapsed = function (expand)
        {
            if (expand)
            {
                $scope.$broadcast('angular-ui-tree:expand-all');
            }
            else
            {
                $scope.$broadcast('angular-ui-tree:collapse-all');
            }
            $scope.categoriesExpanded = expand;
        };

        $scope.toggle = function (item)
        {
            item.ShowInner = !item.ShowInner;
        };

        initialize();
    }]);