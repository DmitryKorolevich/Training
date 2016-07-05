'use strict';

angular.module('app.modules.order.controllers.exportResultController', [])
.controller('exportResultController', ['$scope', '$uibModalInstance', 'data', '$rootScope', function ($scope, $uibModalInstance, data, $rootScope)
{
    function initialize() {
        $scope.errors = $.grep(data.exportRes,
            function(elem) {
                return !elem.Success;
            });

        $scope.successCount = data.exportRes.length - $scope.errors.length;
        $scope.errorCount = $scope.errors.length;
        $scope.total = data.exportRes.length;
    }

    $scope.ok = function () {
        $uibModalInstance.close(false);
    };

    initialize();
}]);