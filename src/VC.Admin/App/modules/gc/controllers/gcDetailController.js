'use strict';

angular.module('app.modules.gc.controllers.gcDetailController', [])
.controller('gcDetailController', ['$scope', '$rootScope', '$state', '$stateParams', 'gcService', 'orderService', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, gcService, orderService, settingService, toaster, modalUtil, confirmUtil, promiseTracker)
{
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function refreshHistory()
    {
        if ($scope.gc && $scope.gc.Id)
        {
            var data = {};
            data.service = settingService;
            data.tracker = $scope.refreshTracker;
            data.idObject = $scope.gc.Id;
            data.idObjectType = 20//gc
            $scope.$broadcast('objectHistorySection#in#refresh', data);
        }
    }

    function successSaveHandler(result)
    {
        if (result.Success)
        {
            toaster.pop('success', "Success!", "Successfully saved.");
            refreshHistory();
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
                    refreshHistory();
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
        var name = '';
        if ($scope.gc.FirstName)
        {
            name += $scope.gc.FirstName + ' ';
        }
        if ($scope.gc.LastName)
        {
            name += $scope.gc.LastName;
        }
        var data =
            {
                ToName: name,
                ToEmail: $scope.gc.Email,
                Gifts: [{ Code: $scope.gc.Code, Amount: $scope.gc.Balance }],
            };
        modalUtil.open('app/modules/gc/partials/sendEmail.html', 'sendEmailController', data);
    };

    initialize();
}]);