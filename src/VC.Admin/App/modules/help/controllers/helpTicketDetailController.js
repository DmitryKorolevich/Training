'use strict';

angular.module('app.modules.help.controllers.helpTicketDetailController', [])
.controller('helpTicketDetailController', ['$scope', '$rootScope', '$state', '$stateParams', 'helpService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, helpService, toaster, modalUtil, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function successSaveHandler(result) {
        if (result.Success) {
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

        $scope.forms = {};
        $scope.helpTicket = {};

        refresh();
    };

    function refresh() {
        helpService.getHelpTicket($scope.id, $scope.refreshTracker)
            .success(function (result) {
                if (result.Success) {
                    $scope.helpTicket = result.Data;
                } else {
                    errorHandler(result);
                }
            }).
            error(function (result) {
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
            helpService.updateHelpTicket($scope.helpTicket, $scope.editTracker).success(function (result)
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

    initialize();
}]);