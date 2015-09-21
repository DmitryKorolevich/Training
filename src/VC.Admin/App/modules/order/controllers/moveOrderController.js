'use strict';

angular.module('app.modules.order.controllers.moveOrderController', [])
.controller('moveOrderController', ['$scope', '$rootScope', '$state', '$stateParams', 'orderService', 'customerService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, orderService, customerService, toaster, modalUtil, confirmUtil, promiseTracker)
{
    $scope.refreshTracker = promiseTracker("get");

    function successSaveHandler(result) {
        if (result.Success)
        {
            toaster.pop('success', "Success!", "Successfully updated.");
        } else {
            var messages = "";
            if (result.Messages) {
                $scope.forms.form.submitted = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $.each(result.Messages, function (index, value) {
                    if (value.Field) {
                        $scope.forms.form[value.Field].$setValidity("server", false);
                    }
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        }
    };

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize()
    {
        $scope.orderStatuses = angular.copy($rootScope.ReferenceData.OrderStatuses);

        $scope.forms = {};
        $scope.order = {
        };
        $scope.orderFilter = {
            Id: '',
            Paging: { PageIndex: 1, PageItemCount: 20 },
        };
        $scope.customerFilter = {
            IdContains: '',
            Paging: { PageIndex: 1, PageItemCount: 20 },
        };
    };

    $scope.save = function () {
        $.each($scope.forms.form, function (index, element) {
        	if (element && element.$name == index) {
                element.$setValidity("server", true);
            }
        });

        if ($scope.forms.form.$valid)
        {
            var id = parseInt($scope.order.Id);
            if (id == NaN)
            {
                id = 0;
            }
            var IdCustomer = parseInt($scope.order.IdCustomer);
            if (IdCustomer == NaN)
            {
                IdCustomer = 0;
            }
            orderService.moveOrder(id, IdCustomer, $scope.refreshTracker).success(function (result)
            {
                successSaveHandler(result);
            }).
                error(function (result) {
                    errorHandler(result);
                });
        } else {
            $scope.forms.form.submitted = true;
        }
    };

    $scope.getOrders = function (val)
    {
        if (val)
        {
            $scope.orderFilter.Id = val;
            return orderService.getShortOrders($scope.orderFilter)
                .then(function (result)
                {
                    return result.data.Data.Items.map(function (item)
                    {
                        return item;
                    });
                });
        }
    };

    $scope.getCustomers = function (val)
    {
        if (val)
        {
            $scope.customerFilter.IdContains = val;
            return customerService.getCustomers($scope.customerFilter)
                .then(function (result)
                {
                    return result.data.Data.Items.map(function (item)
                    {
                        return item;
                    });
                });
        }
    };

    initialize();
}]);