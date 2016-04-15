'use strict';

angular.module('app.modules.healthwise.controllers.markCustomerOrdersController', [])
.controller('markCustomerOrdersController', ['$scope', '$uibModalInstance', 'data', 'customerService', 'healthwiseService', 'toaster', 'promiseTracker', '$rootScope', 'confirmUtil',
    function ($scope, $uibModalInstance, data, customerService, healthwiseService, toaster, promiseTracker, $rootScope, confirmUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function successHandler(result)
        {
            if (result.Success)
            {
                if (result.Data)
                {
                    toaster.pop('success', "Success!", "Successfully flagged");
                    $uibModalInstance.close();
                    data.thenCallback();
                }
                else
                {
                    toaster.pop('error', "Error!", "Invalid Customer #");
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
                IdContains: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };
        };

        $scope.flag = function ()
        {
            if ($scope.forms.form.$valid)
            {
                var id = parseInt($scope.options.IdCustomer)
                if (id == NaN)
                {
                    id = 0;
                }
                confirmUtil.confirm(function ()
                {
                    healthwiseService.markCustomerOrders(id, $scope.refreshTracker)
                        .success(function (result)
                        {
                            successHandler(result);
                        })
                        .error(function (result)
                        {
                            errorHandler(result);
                        });
                }, 'Are you sure you want to flag orders on the given customer?');
            } else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.getCustomers = function (val)
        {
            if (val)
            {
                $scope.filter.IdContains = val;
                return customerService.getCustomers($scope.filter)
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
            $uibModalInstance.close();
        };

        initialize();
    }]);