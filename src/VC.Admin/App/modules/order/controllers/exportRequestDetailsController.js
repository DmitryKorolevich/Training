'use strict';

angular.module('app.modules.order.controllers.exportRequestDetailsController', [])
.controller('exportRequestDetailsController', ['$scope', '$uibModalInstance', 'data', '$rootScope', 'orderService', function ($scope, $uibModalInstance, data, $rootScope, orderService) {
    function initialize() {
        $scope.loadTimestamp = data.items.LoadTimestamp;
        $scope.items = data.items.ExportModels;
        $.each($scope.items, function (i, item) {
            item.ErrorExportedOrders = $.grep(item.ExportedOrders,
            function (elem) {
                return !elem.Success;
            });
            item.SuccessExportedOrders = $.grep(item.ExportedOrders,
            function (elem) {
                return elem.Success;
            });
        });
    }

    $scope.ok = function () {
        $uibModalInstance.close(false);
    };

    $scope.clear = function () {
        orderService.clearExportDetails($scope.loadTimestamp, $scope.refreshTracker)
                    .success(function (result) {
                        if (result.Success) {
                            $uibModalInstance.close(false);
                        } else {
                            var messages = "";
                            if (result.Messages) {
                                $.each(result.Messages, function (index, value) {
                                    messages += value.Message + "<br />";
                                });
                            } else {
                                messages = "Can't clear export details";
                            }
                            toaster.pop('error', 'Error!', messages, null, 'trustedHtml');
                        }
                    })
                    .error(function (result) {
                        errorHandler(result);
                    });
    }

    initialize();
}]);