'use strict';

angular.module('app.modules.gc.controllers.gcsAddController', [])
.controller('gcsAddController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'gcService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, $timeout, gcService, toaster, modalUtil, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function successSaveHandler(result) {
        if (result.Success) {
            $scope.codes = result.Data;
            $.each($scope.codes, function (index, code) {
                code.Balance = code.Balance.toFixed(2);
            });
            $scope.state = 2;
            toaster.pop('success', "Success!", "Successfully saved.");
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

    function initialize() {
        $scope.state = 1;

        $scope.forms = {};

        $scope.codes = [];

        gcService.getGiftCertificatesAdding($scope.refreshTracker).success(function (result) {
            if (result.Success) {
                $scope.gc = result.Data;
            };
        }).error(function (result) {
            errorHandler(result);
        });
    };

    $scope.save = function () {
        $.each($scope.forms.form, function (index, element) {
        	if (element && element.$name == index) {
                element.$setValidity("server", true);
            }
        });

        if ($scope.forms.form.$valid) {
            gcService.addGiftCertificates($scope.gc.Quantity, $scope.gc, $scope.editTracker).success(function (result) {
                successSaveHandler(result);
            }).
                error(function (result) {
                    errorHandler(result);
                });
        } else {
            $scope.forms.form.submitted = true;
        }
    };

    $scope.send = function ()
    {
        var items = [];
        $.each($scope.codes, function (index, gc)
        {
            var item = {
                Code: gc.Code,
                Amount: gc.Balance
            };
            items.push(item);
        });
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
                Gifts: items,
            };
        modalUtil.open('app/modules/gc/partials/sendEmail.html', 'sendEmailController', data);
    };

    initialize();
}]);