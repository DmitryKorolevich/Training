angular.module('app.modules.setting.controllers.addEditStateController', [])
.controller('addEditStateController', ['$scope', '$uibModalInstance', 'data', 'settingService', 'toaster', 'promiseTracker', '$rootScope', function ($scope, $uibModalInstance, data, settingService, toaster, promiseTracker, $rootScope) {
    $scope.saveTracker = promiseTracker("save");

    function successSaveHandler(result) {
        if (result.Success) {
            toaster.pop('success', "Success!", "Successfully saved.");

            $scope.state = result.Data;
            data.thenCallback($scope.state);
            $uibModalInstance.close();
        } else
        {
            $rootScope.fireServerValidation(result, $scope);
        }
    };

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize() {

        $scope.forms = {};

        if (!data.state) {
            $scope.state =
            {
                Id: 0,
                CountryCode: data.countryCode,
                StateCode: '',
                StateName: '',
                StatusCode: 2,//Active
            };
        }
        else {
            $scope.state = Object.clone(data.state);
            $scope.state.StatusCode = $scope.state.StatusCode;
        }

        $scope.save = function () {
            $.each($scope.forms.form, function (index, element) {
            	if (element && element.$name == index) {
                    element.$setValidity("server", true);
                }
            });

            if ($scope.forms.form.$valid) {
                settingService.updateState($scope.state, $scope.saveTracker).success(function (result) {
                    successSaveHandler(result);
                }).
                error(function (result) {
                    errorHandler(result);
                });
            } else {
                $scope.forms.submitted = true;
            }
        };

        $scope.cancel = function () {
            $uibModalInstance.close();
        };
    }

    initialize();
}]);