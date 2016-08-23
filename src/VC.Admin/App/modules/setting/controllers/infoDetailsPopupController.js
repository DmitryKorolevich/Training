'use strict';

angular.module('app.modules.setting.controllers.infoDetailsPopupController', [])
.controller('infoDetailsPopupController', ['$scope', '$uibModalInstance', 'data', 'toaster', function ($scope, $uibModalInstance, data, toaster)
{
    function initialize()
    {
        $scope.Header = data.Header;
        $scope.Messages = data.Messages;
        $scope.CancelButton = {
            Label:'Cancel'
        };
        if (data.CancelButton)
        {
            $scope.CancelButton = data.CancelButton;
        }
        if (data.OkButton)
        {
            $scope.OkButton = data.OkButton;
        }

        $scope.cancel = function ()
        {
            if($scope.CancelButton && $scope.CancelButton.Handler)
            {
                $scope.CancelButton.Handler();
            }
            $uibModalInstance.close();
        };

        $scope.ok = function ()
        {
            if($scope.OkButton && $scope.OkButton.Handler)
            {
                $scope.OkButton.Handler();
            }
            $uibModalInstance.close();
        };
    }

    initialize();
}]);