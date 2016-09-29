'use strict';

angular.module('app.modules.order.controllers.exportRequestDetailsController', [])
.controller('exportRequestDetailsController', ['$scope', '$uibModalInstance', 'data', '$rootScope', function ($scope, $uibModalInstance, data, $rootScope)
{
    function initialize()
    {
        $scope.items = data.items;
        $.each($scope.items, function (i, item)
        {
            item.ErrorExportedOrders = $.grep(item.ExportedOrders,
            function (elem)
            {
                return !elem.Success;
            });
            item.SuccessExportedOrders = $.grep(item.ExportedOrders,
            function (elem)
            {
                return elem.Success;
            });
        });
    }

    $scope.ok = function () {
        $uibModalInstance.close(false);
    };

    initialize();
}]);