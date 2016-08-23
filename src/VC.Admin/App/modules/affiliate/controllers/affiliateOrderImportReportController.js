angular.module('app.modules.affiliate.controllers.affiliateOrderImportReportController', [])
.controller('affiliateOrderImportReportController', ['$scope', '$rootScope', '$state', '$q', 'Upload', 'orderService', 'productService', 'discountService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $q, Upload, orderService, productService, discountService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function errorHandler(result)
        {
            var messages = "";
            if (result.Messages)
            {
                $.each(result.Messages, function (index, value)
                {
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        };

        function initialize()
        {
            $scope.options = {};

            $scope.forms = {};
        }

        $scope.upload = function (files)
        {
            $scope.options.selectedOrderImportFile = files && files.length > 0 ? files[0] : null;
            if ($scope.options.selectedOrderImportFile)
            {
                $scope.options.uploadingOrdersImport = true;
                var deferred = $scope.refreshTracker.createPromise();
                Upload.upload({
                    url: '/api/order/GetAffiliateOrdersInfo',
                    data: {},
                    file: $scope.options.selectedOrderImportFile
                }).progress(function (evt)
                {

                }).success(function (result, status, headers, config)
                {
                    deferred.resolve();
                    if (result.Success)
                    {
                        $scope.report = result.Data;
                        $scope.filterChanged();
                    } else
                    {
                        errorHandler(result.Messages);
                    }
                    $scope.options.selectedOrderImportFile = null;
                }).error(function (data, status, headers, config)
                {
                    deferred.resolve();
                    $scope.options.selectedOrderImportFile = null;

                    toaster.pop('error', "Error!", "Server error ocurred");
                });
            }
        };

        $scope.filterChanged = function ()
        {
            $scope.options.exportUrl = orderService.getAffiliateOrdersInfoReportFile($scope.report.Id, $rootScope.buildNumber);
        };

        initialize();
    }]);