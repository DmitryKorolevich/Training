'use strict';

angular.module('app.modules.affiliate.controllers.affiliateManageController', [])
.controller('affiliateManageController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', '$modal',
    'affiliateService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, $timeout, $modal, affiliateService, toaster, confirmUtil, promiseTracker)
    {
        $scope.refreshTracker = promiseTracker("get");

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.affiliate.Id = result.Data.Id;
            } else {
                var messages = "";
                if (result.Messages) {
                    $scope.forms.submitted = true;
                    $scope.detailsTab.active = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);

                    $.each(result.Messages, function (index, value) {
                        if (value.Field) {
                            $.each($scope.forms, function (index, form)
                            {
                                if (form && !(typeof form === 'boolean'))
                                {
                                    if (form[value.Field] != undefined)
                                    {
                                        form[value.Field].$setValidity("server", false);
                                        return false;
                                    }
                                }
                            });
                        }
                    });
                }
                toaster.pop('error', "Error!", messages, null, 'trustedHtml');
            }
        };

        function errorHandler(result) {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function initialize() {
            $scope.id = $stateParams.id ? $stateParams.id : 0;

            $scope.forms = {};
            $scope.detailsTab = {
                active: true
            };
        };

        $scope.save = function () {
            clearServerValidation();

            if ($scope.forms.mainForm.$valid)
            {

            } else {
                $scope.forms.submitted = true;
                $scope.detailsTab.active = true;
            }
        };

        var clearServerValidation = function () {
            $.each($scope.forms, function (index, form) {
                if (form && !(typeof form === 'boolean')) {
                    $.each(form, function (index, element)
                    {
                        if (element && element.$name == index)
                        {
                            element.$setValidity("server", true);
                        }
                    });
                }
            });
        };

        initialize();
    }
]);