'use strict';

angular.module('app.modules.healthwise.controllers.markOrderController', [])
.controller('markOrderController', ['$scope', '$modalInstance', 'data', 'orderService', 'healthwiseService', 'toaster', 'promiseTracker', '$rootScope', 'confirmUtil',
    function ($scope, $modalInstance, data, orderService, healthwiseService, toaster, promiseTracker, $rootScope, confirmUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function successHandler(result)
        {
            if (result.Success)
            {
                if (result.Data)
                {
                    toaster.pop('success', "Success!", "Successfully flagged");
                    $modalInstance.close();
                    data.thenCallback();
                }
                else
                {
                    toaster.pop('error', "Error!", "Invalid Order #");
                }
            } else
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
            }
        };

        function errorHandler(result)
        {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function initialize()
        {
            $scope.options = {};
            $scope.forms = {};

            $scope.filter = {
                Id: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };
        };

        $scope.flag = function ()
        {
            if ($scope.forms.form.$valid)
            {
                var id = parseInt($scope.options.IdOrder)
                if (id == NaN)
                {
                    id = 0;
                }
                confirmUtil.confirm(function ()
                {
                    healthwiseService.markOrder(id, $scope.refreshTracker)
                        .success(function (result)
                        {
                            if (result.Success)
                            {
                                successHandler(result);
                            } else
                            {
                                errorHandler(result);
                            }
                        })
                        .error(function (result)
                        {
                            errorHandler(result);
                        });
                }, 'Are you sure you want to flag the given order?');
            } else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.getOrders = function (val)
        {
            if (val)
            {
                $scope.filter.Id = val;
                return orderService.getShortOrders($scope.filter)
                    .then(function (result)
                    {
                        return result.data.Data.Items.map(function (item)
                        {
                            return item;
                        });
                    });
            }
        };

        $scope.cancel = function ()
        {
            $modalInstance.close();
        };

        initialize();
    }]);