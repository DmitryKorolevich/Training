'use strict';

angular.module('app.modules.setting.controllers.lookupDetailController', [])
.controller('lookupDetailController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, $timeout, settingService, toaster, modalUtil, confirmUtil, promiseTracker)
{
    $scope.refreshTracker = promiseTracker("get");

    function successSaveHandler(result) {
        if (result.Success) {
            toaster.pop('success', "Success!", "Successfully saved.");
        } else {
            var messages = "";
            if (result.Messages) {
                $scope.forms.submitted = true;
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
        $scope.state = 1;

        $scope.forms = {};

        settingService.getLookup($stateParams.id, $scope.refreshTracker).success(function (result)
        {
            if (result.Success)
            {
                $scope.lookup = result.Data;
            };
        }).error(function (result) {
            errorHandler(result);
        });
    };

    $scope.save = function () {
        $.each($scope.forms.form, function (index, element) {
        	if (element && element.$name == index) {
                element.$setValidity("server", true);
            }
        });

        duplicatesValidatorClean();
        duplicatesValidatorFire();

        if ($scope.forms.form.$valid)
        {
            settingService.updateLookupVariants($scope.lookup.Id, $scope.lookup.LookupVariants, $scope.refreshTracker).success(function (result)
            {
                successSaveHandler(result);
            }).
            error(function (result) {
                errorHandler(result);
            });
        } else {
            $scope.forms.submitted = true;
        }
    };

    $scope.sortableOptions = {
        handle: ' .sortable-move',
        items: ' .variant-panel',
        axis: 'y',
        start: function (e, ui)
        {
            $scope.dragging = true;
        },
        stop: function (e, ui)
        {
            $scope.dragging = false;
            var variants = [];
            $.each($scope.lookup.LookupVariants, function (index, item)
            {
                var newItem = {};
                angular.copy(item, newItem);
                variants.push(newItem);
            });
            $scope.lookup.LookupVariants = [];
            $scope.lookup.LookupVariants = variants;
        }
    };

    $scope.addVariant = function ()
    {
        duplicatesValidatorClean();
        duplicatesValidatorFire();

        if (!$scope.forms.form.$valid)
        {
            $scope.forms.submitted = true;
            return false;
        }

        var variant = {
            ValueVariant: '',
        };

        var variants = [];
        $.each($scope.lookup.LookupVariants, function (index, item)
        {
            var newItem = {};
            angular.copy(item, newItem);
            variants.push(newItem);
        });
        variants.push(variant);
        $scope.lookup.LookupVariants = [];
        $scope.lookup.LookupVariants = variants;

        $scope.forms.submitted = false;
    };

    $scope.deleteVariant = function (index)
    {
        var variants = [];
        $.each($scope.lookup.LookupVariants, function (index, item)
        {
            var newItem = {};
            angular.copy(item, newItem);
            variants.push(newItem);
        });
        variants.splice(index, 1);
        $scope.lookup.LookupVariants = [];
        $scope.lookup.LookupVariants = variants;
    };

    var duplicatesValidatorClean = function ()
    {
        $.each($scope.forms.form, function (index, form)
        {
            if (form && index.indexOf('i') == 0 && form.ValueVariant != undefined)
            {
                form.ValueVariant.$setValidity("exist", true);
            }
        });
    };

    var duplicatesValidatorFire = function ()
    {
        $.each($scope.forms.form, function (index, form)
        {
            if (form && index.indexOf('i') == 0 && form.ValueVariant != undefined)
            {
                var itemIndex = parseInt(index.replace("i", ""));
                if ($scope.lookup.LookupVariants[itemIndex] != undefined && $scope.lookup.LookupVariants[itemIndex].ValueVariant)
                {
                    var name = $scope.lookup.LookupVariants[itemIndex].ValueVariant;
                    $.each($scope.lookup.LookupVariants, function (index, item)
                    {
                        if (itemIndex != index && item.ValueVariant && name.toLowerCase() == item.ValueVariant.toLowerCase())
                        {
                            form.ValueVariant.$setValidity("exist", false);
                        }
                    });
                }
            }
        });
    };

    initialize();
}]);