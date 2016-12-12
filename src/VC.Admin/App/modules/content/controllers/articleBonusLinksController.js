angular.module('app.modules.content.controllers.articleBonusLinksController', [])
.controller('articleBonusLinksController', ['$scope', '$rootScope', '$state', '$uibModalInstance', 'contentService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil', 'data',
    function ($scope, $rootScope, $state, $uibModalInstance, contentService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil, data)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function errorHandler(result)
        {
            $rootScope.fireServerValidation(result, $scope);
        };

        function refreshItems()
        {
            contentService.getArticleBonusLinks($scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.items = result.Data;
                        $.each($scope.items, function (index, item)
                        {
                            if (item.StartDate)
                            {
                                item.StartDate = Date.parseDateTime(item.StartDate);
                            }
                        });
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

            refreshItems();
        }

        $scope.save = function ()
        {
            $.each($scope.forms.form, function (index, element)
            {
                if (element && element.$name == index)
                {
                    element.$setValidity("server", true);
                }
            });

            if ($scope.forms.form.$valid)
            {
                var items = angular.copy($scope.items);
                
                $.each(items, function (index, item)
                {
                    if (item.StartDate)
                    {
                        item.StartDate = item.StartDate.toServerDateTime();
                    }
                });

                contentService.updateArticleBonusLinks(items, $scope.refreshTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            data.thenCallback();
                            $uibModalInstance.close();
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
            }
            else
            {
                $scope.forms.submitted = true;
            }
        };

        $scope.cancel = function ()
        {
            $uibModalInstance.close();
        };

        $scope.addItem = function ()
        {
            if (!$scope.forms.form.$valid)
            {
                $scope.forms.submitted = true;
                return false;
            }
            var item = {
                Url: null,
                FromDate: null,
            };
            $scope.items.push(item);

            $scope.forms.submitted = false;
        };

        $scope.deleteItem = function (index)
        {
            $scope.items.splice(index, 1);
        };

        initialize();
    }]);