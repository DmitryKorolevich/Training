'use strict';

angular.module('app.modules.content.controllers.aceErrorDetailsController', [])
.controller('aceErrorDetailsController', ['$scope', '$uibModalInstance', 'data', function ($scope, $uibModalInstance, data)
{
    function initialize()
    {
        $scope.cancel = function () {
            $uibModalInstance.close();
        };

        $scope.error = data.error;
    }

    initialize();
}]);
