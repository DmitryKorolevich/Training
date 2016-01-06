'use strict';

angular.module('app.modules.gc.controllers.gcDetailController', [])
.controller('gcDetailController', ['$scope', '$rootScope', '$state', '$stateParams', 'gcService', 'orderService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, gcService, orderService, toaster, modalUtil, confirmUtil, promiseTracker)
{
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function successSaveHandler(result)
    {
        if (result.Success)
        {
            toaster.pop('success', "Success!", "Successfully saved.");
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
        $scope.id = $stateParams.id;

        $scope.forms = {};
        $scope.gc = {};

        refresh();
        refreshOrders();
    };

    function refresh()
    {
        gcService.getGiftCertificate($scope.id, $scope.refreshTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.gc = result.Data;
                    $scope.gc.StatusCode = $scope.gc.StatusCode;
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

    function refreshOrders()
    {
        if ($scope.id)
        {
            orderService.getGCOrders($scope.id, $scope.refreshTracker)
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
        }
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
            gcService.updateGiftCertificate($scope.gc, $scope.editTracker).success(function (result)
            {
                successSaveHandler(result);
            }).
                error(function (result)
                {
                    errorHandler(result);
                });
        } else
        {
            $scope.forms.form.submitted = true;
        }
    };

    $scope.send = function ()
    {
        var data =
            {
                ToName: $scope.gc.FirstName || $scope.gc.LastName ? $scope.gc.FirstName + ' ' + $scope.gc.LastName : null,
                ToEmail: $scope.gc.Email,
                FromName: 'Vital Choice',
                Codes: [$scope.gc],
            };
        modalUtil.open('app/modules/gc/partials/sendEmail.html', 'sendEmailController', data);
    };

    initialize();
}]);