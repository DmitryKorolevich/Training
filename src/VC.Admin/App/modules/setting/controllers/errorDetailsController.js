'use strict';

angular.module('app.modules.setting.controllers.errorDetailsController', [])
.controller('errorDetailsController', ['$scope', '$uibModalInstance', 'data', 'toaster', function ($scope, $uibModalInstance, data, toaster)
{
    function initialize()
    {
        $scope.Header = data.Header;
        if (!$scope.Header)
        {
            $scope.Header = "Error details";
        }
        $scope.Messages = data.Messages;

        $scope.cancel = function ()
        {
            $uibModalInstance.close();
        };
    }

    initialize();
}]);