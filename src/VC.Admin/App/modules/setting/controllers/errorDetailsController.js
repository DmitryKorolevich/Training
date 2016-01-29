'use strict';

angular.module('app.modules.setting.controllers.errorDetailsController', [])
.controller('errorDetailsController', ['$scope', '$modalInstance', 'data', 'toaster', function ($scope, $modalInstance, data, toaster)
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
            $modalInstance.close();
        };
    }

    initialize();
}]);