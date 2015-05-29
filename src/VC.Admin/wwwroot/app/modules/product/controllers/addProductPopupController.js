﻿angular.module('app.modules.product.controllers.addProductPopupController', [])
.controller('addProductPopupController', ['$scope', '$modalInstance', 'data', 'settingService', 'toaster', 'promiseTracker', '$rootScope', function ($scope, $modalInstance, data, settingService, toaster, promiseTracker, $rootScope) {

    function initialize() {
        $scope.types = $rootScope.ReferenceData.ProductTypes;
        $scope.type = 2;
        $scope.forms = {};

        $scope.save = function () {
            data.thenCallback($scope.type);
            $modalInstance.close();
        };

        $scope.cancel = function () {
            $modalInstance.close();
        };
    }

    initialize();
}]);