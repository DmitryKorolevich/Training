﻿'use strict';

angular.module('app.modules.order.controllers.orderStatusUpdateController', [])
.controller('orderStatusUpdateController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'orderService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, $timeout, orderService, toaster, modalUtil, confirmUtil, promiseTracker)
{
    $scope.refreshTracker = promiseTracker("get");

    function successSaveHandler(result)
    {
        if (result.Success)
        {
            if (!$scope.order.OrderPart)
            {
                $scope.order.CurrentIdStatus = $scope.order.Status;
            } else if ($scope.order.OrderPart == 2)
            {
                $scope.order.CurrentIdPStatus = $scope.order.Status;
            } if ($scope.order.OrderPart == 1)
            {
                $scope.order.CurrentIdNPStatus = $scope.order.Status;
            }
            toaster.pop('success', "Success!", "Successfully updated.");
        } else
        {
            var messages = "";
            if (result.Messages)
            {
                $scope.forms.form.submitted = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $.each(result.Messages, function (index, value)
                {
                    if (value.Field)
                    {
                        $scope.forms.form[value.Field].$setValidity("server", false);
                    }
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

        $scope.orderStatuses = angular.copy($rootScope.ReferenceData.OrderStatuses);
        $scope.orderParts = [
            { Key: 2, Text: 'P' },
            { Key: 1, Text: 'NP' }
        ];

        $scope.forms = {};
        $scope.order = {
            Status: 2,
            OrderPart: null,
        };
        $scope.filter = {
            Id: '',
            Paging: { PageIndex: 1, PageItemCount: 20 },
        };
        $scope.directFilter = {
            Id: '',
            Paging: { PageIndex: 1, PageItemCount: 20 },
        };
    };

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
            if ($scope.options.loadedId)
            {
                var id = parseInt($scope.options.loadedId);
                orderService.updateOrderStatus(id, $scope.order.Status, $scope.order.OrderPart, $scope.refreshTracker).success(function (result)
                {
                    successSaveHandler(result);
                }).
                error(function (result)
                {
                    errorHandler(result);
                });
            }
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

    $scope.idLoaded = function (item, model, label)
    {
        $scope.options.loadedId = $scope.order.Id;
        $scope.order.CurrentIdStatus = item.OrderStatus;
        $scope.order.CurrentIdPStatus = item.POrderStatus;
        $scope.order.CurrentIdNPStatus = item.NPOrderStatus;
        if ($scope.order.CurrentIdStatus)
        {
            $scope.order.OrderPart = null;
        }
        else
        {
            $scope.order.OrderPart = 2;//P
        }
    };

    $scope.lostFocusRequest = null;

    $scope.lostFocus = function (index)
    {
        //resolving issue with additional load after lost focus from the input in time of selecting a new element
        if ($scope.lostFocusRequest)
        {
            $timeout.cancel($scope.lostFocusRequest);
        }
        $scope.lostFocusRequest = $timeout(function ()
        {
            if ($scope.options.loadedId != $scope.order.Id)
            {
                loadOrder();
            }
        }, 100);
    };

    $scope.idChanged = function ()
    {
        if ($scope.options.loadedId != $scope.order.Id)
        {
            $.each($scope.forms.form, function (index, element)
            {
                if (element && element.$name == index)
                {
                    element.$setValidity("server", true);
                }
            });
            $scope.order.CurrentIdStatus = null;
            $scope.order.CurrentIdPStatus = null;
            $scope.order.CurrentIdNPStatus = null;
            $scope.options.loadedId = null;
        }
    };

    $scope.getOrderSettings = function (event)
    {
        if ($scope.options.loadedId != $scope.order.Id)
        {
            loadOrder();
            event.preventDefault();
        }
    };

    var loadOrder = function ()
    {
        $scope.directFilter.Id = $scope.order.Id;
        orderService.getShortOrders($scope.directFilter, $scope.refreshTracker).success(function (result)
        {
            if (result.Data.Items.length == 1)
            {
                $scope.idLoaded(result.Data.Items[0], null, null);
            }
        }).
        error(function (result)
        {
            errorHandler(result);
        });
    };

    initialize();
}]);