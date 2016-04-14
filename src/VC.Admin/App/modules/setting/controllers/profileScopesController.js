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
                Paging: { PageIndex: 1, PageItemCount: 10 },
            };

            refreshItems();
        };

        $scope.pageChanged = function ()
        {
            refreshItems();
        };

        var getCategoriesTreeViewScope = function ()
        {
            return angular.element($('.categories .ya-treeview').get(0)).scope();
        };

        $scope.updateCategoriesCollapsed = function (expand)
        {
            var scope = getCategoriesTreeViewScope();
            if (expand)
            {
                scope.expandAll();
            }
            else
            {
                scope.collapseAll();
            }
            $scope.categoriesExpanded = expand;
        };

        initialize();
    }]);