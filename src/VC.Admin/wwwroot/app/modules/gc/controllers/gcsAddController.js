'use strict';

angular.module('app.modules.gc.controllers.gcsAddController', [])
.controller('gcsAddController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'gcService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, $timeout, gcService, toaster, modalUtil, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function successSaveHandler(result) {
        if (result.Success) {
            $scope.codes = result.Data;
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
        $scope.id = $stateParams.id;
        $scope.state = 1;

        $scope.forms = {};
        $scope.gc =
        {
            Quantity: 1,
            Balance: 0,
        };

        $timeout(function ()
        {
            $scope.gc.Balance = 0;
        }, 50);

        $scope.codes = [];
    };

    $scope.save = function () {
        $.each($scope.forms.form, function (index, element) {
            if (element.$name == index) {
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

    $scope.send = function () {
        var data =
            {
                ToName: $scope.gc.FirstName || $scope.gc.LastName ? $scope.gc.FirstName + ' ' + $scope.gc.LastName: null,
                ToEmail: $scope.gc.Email,
                FromName: 'Vital Choice',
                Message: '',
            };
        $.each($scope.codes, function (index, code) {
            data.Message += code.Code + '\r\n';
        });
        modalUtil.open('app/modules/gc/partials/sendEmail.html', 'sendEmailController', data);
    };

    initialize();
}]);