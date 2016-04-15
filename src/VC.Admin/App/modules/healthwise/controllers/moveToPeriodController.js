'use strict';

angular.module('app.modules.healthwise.controllers.moveToPeriodController', [])
.controller('moveToPeriodController', ['$scope', '$uibModalInstance', 'data', 'healthwiseService', 'toaster', 'promiseTracker', '$rootScope', 'confirmUtil',
    function ($scope, $uibModalInstance, data, healthwiseService, toaster, promiseTracker, $rootScope, confirmUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function successHandler(result)
        {
            if (result.Success)
            {
                toaster.pop('success', "Success!", "Successfully moved");
                $uibModalInstance.close();
                data.thenCallback();
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
            $scope.idCustomer = data.idCustomer;

            $scope.options = {};
            $scope.forms = {};

            $scope.filter = {
                IdPeriod: data.idPeriodFrom,
                Ids: data.ids,
            };

            $scope.model = {
                IdPeriod: null,
                Ids: data.ids,
            };

            refreshPeriods();
        };


        var refreshPeriods = function ()
        {
            healthwiseService.getHealthwisePeriodsForMovement($scope.filter, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.items = result.Data;
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

        $scope.move = function (id)
        {
            confirmUtil.confirm(function ()
            {
                $scope.model.IdPeriod = id;
                healthwiseService.moveHealthwiseOrders($scope.model, $scope.refreshTracker)
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
            }, 'Are you sure you want to move orders to the given period?');
        };

        $scope.addPeriod = function ()
        {
            healthwiseService.addPeriod($scope.idCustomer, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        refreshPeriods();
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

        $scope.cancel = function ()
        {
            $uibModalInstance.close();
        };

        initialize();
    }]);