angular.module('app.modules.product.controllers.addProductPopupController', [])
.controller('addProductPopupController', ['$scope', '$uibModalInstance', 'data', 'settingService', 'toaster', 'promiseTracker', '$rootScope', function ($scope, $uibModalInstance, data, settingService, toaster, promiseTracker, $rootScope) {

    function initialize() {
        $scope.types = $rootScope.ReferenceData.ProductTypes;
        $scope.type = 2;
        $scope.forms = {};

        $scope.save = function () {
            data.thenCallback($scope.type);
            $uibModalInstance.close();
        };

        $scope.cancel = function () {
            $uibModalInstance.close();
        };
    }

    initialize();
}]);