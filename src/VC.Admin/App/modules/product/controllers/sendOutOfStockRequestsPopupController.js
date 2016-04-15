angular.module('app.modules.product.controllers.sendOutOfStockRequestsPopupController', [])
.controller('sendOutOfStockRequestsPopupController', ['$scope', '$uibModalInstance', 'data', 'productService', 'toaster', 'promiseTracker', '$rootScope',
    function ($scope, $uibModalInstance, data, productService, toaster, promiseTracker, $rootScope)
{
    $scope.refreshTracker = promiseTracker("refresh");

    function successSaveHandler(result) {
        if (result.Success)
        {
            toaster.pop('success', "Success!", "Successfully sent.");
            data.thenCallback($scope.state);
            $uibModalInstance.close();
        } else {
            var messages = "";
            if (result.Messages) {
                $scope.forms.form.submitted = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $.each(result.Messages, function (index, value) {
                    if (value.Field) {
                        $scope.forms.form[value.Field].$setValidity("server", false);
                    }
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        }
    };

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize() {

        $scope.forms = {};
        $scope.options = {};
        $scope.options.Ids = data.Ids;

        productService.getProductOutOfStockRequestsMessageFormat($scope.id, $scope.refreshTracker).success(function (result)
        {
            if (result.Success)
            {
                $scope.options.MessageFormat = result.Data;
            };
        }).error(function (result)
        {
            errorHandler(result);
        });
    }

    $scope.save = function ()
    {
        if ($scope.forms.form.$valid)
        {
            productService.sendProductOutOfStockRequests($scope.options, $scope.refreshTracker).success(function (result)
            {
                successSaveHandler(result);
            }).
            error(function (result)
            {
                errorHandler(result);
            });
        } else
        {
            $scope.forms.submitted = true;
        }
    };

    $scope.cancel = function ()
    {
        $uibModalInstance.close();
    };

    initialize();
}]);