angular.module('app.modules.misc.controllers.addEditRedirectController', [])
.controller('addEditRedirectController', ['$scope', '$uibModalInstance', 'data', 'redirectService', 'toaster', 'promiseTracker', '$rootScope',
    function ($scope, $uibModalInstance, data, redirectService, toaster, promiseTracker, $rootScope)
{
    $scope.saveTracker = promiseTracker("save");

    function successSaveHandler(result) {
        if (result.Success) {
            toaster.pop('success', "Success!", "Successfully saved.");

            data.thenCallback();
            $uibModalInstance.close();
        } else {
            var messages = "";
            if (result.Messages) {
                $scope.forms.submitted = true;
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

        $scope.forms = {};

        $scope.redirect = {};
        if (data.id) {
            redirectService.getRedirect(data.id, $scope.saveTracker).success(function (result)
            {                
                if (result.Success)
                {
                    $scope.redirect = result.Data;
                }
            }).
            error(function (result)
            {
                errorHandler(result);
            });
        }
    }

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
            redirectService.updateRedirect($scope.redirect, $scope.saveTracker).success(function (result)
            {
                successSaveHandler(result);
            }).
            error(function (result)
            {
                errorHandler(result);
            });
        } else
        {
            $scope.forms.submitted = true;
        }
    };

    $scope.cancel = function ()
    {
        $uibModalInstance.close();
    };

    initialize();
}]);