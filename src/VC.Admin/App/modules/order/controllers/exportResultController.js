'use strict';

angular.module('app.modules.order.controllers.exportResultController', [])
.controller('exportResultController', ['$scope', '$uibModalInstance', 'data', '$rootScope', function ($scope, $uibModalInstance, data, $rootScope) {
    function initialize() {
        $scope.errors = $.grep(data.exportRes.ExportModels,
            function (elem) {
                return !elem.Success;
            });

        $scope.successCount = data.exportRes.ExportModels.length - $scope.errors.length;
        $scope.errorCount = $scope.errors.length;
        $scope.total = data.exportRes.ExportModels.length;
        $scope.loadTimestamp = data.exportRes.LoadTimestamp;
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