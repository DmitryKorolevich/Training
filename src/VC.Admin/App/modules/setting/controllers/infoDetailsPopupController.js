'use strict';

angular.module('app.modules.setting.controllers.infoDetailsPopupController', [])
.controller('infoDetailsPopupController', ['$scope', '$uibModalInstance', 'data', 'toaster', function ($scope, $uibModalInstance, data, toaster)
{
    function initialize()
    {
        $scope.Header = data.Header;
        $scope.Messages = data.Messages;

        $scope.cancel = function ()
        {
            $uibModalInstance.close();
        };
    }

    initialize();
}]);