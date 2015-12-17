'use strict';

angular.module('app.modules.healthwise.controllers.healthwiseDetailController', [])
.controller('healthwiseDetailController', ['$scope', '$rootScope', '$state', '$stateParams', 'healthwiseService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, healthwiseService, toaster, modalUtil, confirmUtil, promiseTracker)
{
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function successSaveHandler(result)
    {
        if (result.Success)
        {
            toaster.pop('success', "Success!", "Successfully paid.");
            refresh();
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

    var refresh = function ()
    {
        $scope.payment = {
            AllowPayment: false,
        };
        loadOrders();
        loadHealthwise();
    };

    function initialize()
    {
        $scope.id = $stateParams.id;

        $scope.forms = {};
        $scope.options = {};
        $scope.payment = {
            AllowPayment:false,
        };

        loadOrders();
        loadHealthwise();
    };

    function loadOrders()
    {
        healthwiseService.getHealthwiseOrders($scope.id, $scope.refreshTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.items = result.Data;
                } else
                {
                    errorHandler(result);
                }
            }).
            error(function (result)
            {
                errorHandler(result);
            });
    };

    function loadHealthwise()
    {
        healthwiseService.getHealthwisePeriod($scope.id, $scope.refreshTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    var data = result.Data;
                    if(data)
                    {
                        $scope.options.FirstName=data.FirstName;
                        $scope.options.LastName = data.LastName;
                        $scope.options.IdCustomer = data.Id;
                        if(data.Periods && data.Periods.length==1)
                        {
                            if(data.Periods[0].AllowPayment)
                            {
                                $scope.payment.Id = data.Periods[0].Id;
                                $scope.payment.AllowPayment=true;
                                $scope.payment.Amount=(data.Periods[0].OrderSubtotals / data.Periods[0].OrdersCount).toFixed(2);
                                $scope.payment.Date = Date.parseDateTime(data.Periods[0].LastOrderDate);
                                $scope.payment.PayAsGC=true;
                            }
                            $scope.options.PaidAmount = data.Periods[0].PaidAmount;
                            $scope.options.PaidDate = data.Periods[0].PaidDate;
                            $scope.options.AllowMove = data.Periods[0].PaidDate==null;
                        }
                    }
                } else
                {
                    errorHandler(result);
                }
            }).
            error(function (result)
            {
                errorHandler(result);
            });
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
            var data = $scope.payment;
            if ($scope.payment.Date)
            {
                $scope.payment.Date = Date.parseDateTime($scope.payment.Date);
            }
            healthwiseService.makeHealthwisePeriodPayment($scope.payment, $scope.editTracker).
                success(function (result)
                {
                    successSaveHandler(result);
                }).error(function (result)
                {
                    errorHandler(result);
                });
        } else
        {
            $scope.forms.form.submitted = true;
        }
    };

    $scope.openMovePopup = function()
    {
        var ids = [];
        $.each($scope.items, function (index, item)
        {
            if (item.IsSelected)
            {
                ids.push(item.Id);
            }
        });

        if (ids.length == 0)
        {
            toaster.pop('error', "Error!", "At least one order should be selected");
            return;
        }

        modalUtil.open('app/modules/healthwise/partials/moveToPeriodPopup.html', 'moveToPeriodController', {
            idCustomer: $scope.options.IdCustomer,
            idPeriodFrom: $scope.id,
            ids: ids,
            thenCallback: function (data)
            {
                refresh();
            }
        });
    };

    $scope.allSelectCall = function()
    {
        $.each($scope.items, function(index, item)
        {
            item.IsSelected = $scope.options.allSelected;
        });
    };

    $scope.openOrder = function (id) {
        $state.go('index.oneCol.orderDetail', { id: id });
    };

    $scope.itemSelectChanged = function (item)
    {
        if (!item.IsSelected && $scope.options.allSelected)
        {
            $scope.options.allSelected = false;
        }
    };

    initialize();
}]);