'use strict';

angular.module('app.modules.setting.controllers.orderReviewRuleDetailController', [])
.controller('orderReviewRuleDetailController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'settingService', 'productService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, $timeout, settingService, productService, toaster, modalUtil, confirmUtil, promiseTracker)
{
    $scope.refreshTracker = promiseTracker("get");

    function successSaveHandler(result) {
        if (result.Success)
        {
            if (!$scope.rule.Id)
            {
                $state.go('index.oneCol.orderReviewRuleDetail', { id: result.Data.Id });
            }
            else
            {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.rule.Id = result.Data.Id;
            }
        } else
        {
            $rootScope.fireServerValidation(result, $scope);
        }
    };

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize() {
        $scope.state = 1;

        $scope.forms = {};
        $scope.options = {};
        $scope.options.ApplyTypes = [
            { Key: 1, Text: 'All conditions are met' },
            { Key: 2, Text: 'Any conditions are met' }
        ];
        $scope.options.CompareTypes = [
            { Key: 1, Text: 'Equal' },
            { Key: 2, Text: 'Not Equal' }
        ];
        $scope.options.OrderTypes = [
            { Key: 5, Text: 'Reships' },
            { Key: 6, Text: 'Refunds' }
        ];
        $scope.options.DatePeriods = [
            { Key: 1, Text: '1 month' },
            { Key: 3, Text: '3 months' },
            { Key: 6, Text: '6 months' },
            { Key: 12, Text: '1 year' },
            { Key: 24, Text: '2 years' },
        ];

        $scope.skusFilter = {
            Code: '',
            Paging: { PageIndex: 1, PageItemCount: 20 },
        };

        settingService.getOrderReviewRule($stateParams.id ? $stateParams.id : 0, $scope.refreshTracker).success(function (result)
        {
            if (result.Success)
            {
                $scope.rule = result.Data;
            } else
            {
                $rootScope.fireServerValidation(result, $scope);
            }
        }).error(function (result) {
            errorHandler(result);
        });
    };

    $scope.getSKUsBySKU = function (val)
    {
        if (val)
        {
            $scope.skusFilter.Code = val;
            return productService.getSkus($scope.skusFilter)
                .then(function (result)
                {
                    return result.data.Data.map(function (item)
                    {
                        return item;
                    });
                });
        }
    };

    $scope.save = function () {
        $.each($scope.forms.form, function (index, element) {
        	if (element && element.$name == index) {
                element.$setValidity("server", true);
            }
        });

        if ($scope.forms.form.$valid)
        {
            settingService.updateOrderReviewRule($scope.rule, $scope.refreshTracker).success(function (result)
            {
                successSaveHandler(result);
            }).
            error(function (result) {
                errorHandler(result);
            });
        } else
        {
            $scope.forms.submitted = true;
            toaster.pop('error', "Error!", $rootScope.baseValidationMessage, null, 'trustedHtml');
        }
    };

    initialize();
}]);