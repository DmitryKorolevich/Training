angular.module('app.modules.misc.controllers.addEditRedirectController', [])
.controller('addEditRedirectController', ['$scope', '$rootScope', '$uibModalInstance', 'data', 'redirectService', 'toaster', 'promiseTracker', '$rootScope',
    function ($scope, $rootScope, $uibModalInstance, data, redirectService, toaster, promiseTracker, $rootScope)
{
    $scope.saveTracker = promiseTracker("save");

    function successSaveHandler(result) {
        if (result.Success) {
            toaster.pop('success', "Success!", "Successfully saved.");

            data.thenCallback();
            $uibModalInstance.close();
        } else
        {
            $rootScope.fireServerValidation(result, $scope);
        }
    };

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize() {

        $scope.forms = {};
        $scope.options = {};

        $scope.redirect = {};
        $scope.redirect.FutureRedirects = [];
        if (data.id) {
            redirectService.getRedirect(data.id, $scope.saveTracker).success(function (result)
            {                
                if (result.Success)
                {
                    $scope.redirect = result.Data;
                    $.each($scope.redirect.FutureRedirects, function (index, item)
                    {
                        if (item.StartDate)
                        {
                            item.StartDate = Date.parseDateTime(item.StartDate);
                        }
                    });
                    if ($scope.redirect.FutureRedirects && $scope.redirect.FutureRedirects.length > 0)
                    {
                        $scope.options.FutureRedirectOpened = true;
                    }
                }
            }).
            error(function (result)
            {
                errorHandler(result);
            });
        }
    }

    $scope.save = function ()
    {
        $.each($scope.forms.form, function (index, element)
        {
            if (element && element.$name == index)
            {
                element.$setValidity("server", true);
            }
        });

        additionalValidatorClean();
        additionalValidatorFire();

        if ($scope.forms.form.$valid)
        {
            var redirect = angular.copy($scope.redirect);

            $.each(redirect.FutureRedirects, function (index, item)
            {
                if (item.StartDate)
                {
                    item.StartDate = item.StartDate.toServerDateTime();
                }
            });

            redirectService.updateRedirect(redirect, $scope.saveTracker).success(function (result)
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

    $scope.addFutureRedirect = function ()
    {
        additionalValidatorClean();
        additionalValidatorFire();

        if (!$scope.forms.form.$valid)
        {
            $scope.forms.submitted = true;
            return false;
        }

        var newRedirect = {
            Url: '',
            StartDate: null,
        };        
        $scope.redirect.FutureRedirects.push(newRedirect);

        $scope.forms.submitted = false;
    };

    $scope.deleteFutureRedirect = function (index)
    {
        $scope.redirect.FutureRedirects.splice(index, 1);
    };

    var additionalValidatorClean = function ()
    {
        $.each($scope.forms.form, function (index, form)
        {
            if (form && index.indexOf('i') == 0 && form.StartDate != undefined)
            {
                form.StartDate.$setValidity("exist", true);
                form.StartDate.$setValidity("future", true);
            }
        });
    };

    var additionalValidatorFire = function ()
    {
        $.each($scope.forms.form, function (index, form)
        {
            if (form && index.indexOf('i') == 0 && form.StartDate != undefined)
            {
                var itemIndex = parseInt(index.replace("i", ""));
                if ($scope.redirect.FutureRedirects[itemIndex] != undefined && $scope.redirect.FutureRedirects[itemIndex].StartDate &&
                    !$scope.redirect.FutureRedirects[itemIndex].Disabled)
                {
                    var startDate = $scope.redirect.FutureRedirects[itemIndex].StartDate;
                    $.each($scope.redirect.FutureRedirects, function (index, item)
                    {
                        if (itemIndex != index && item.StartDate && startDate.getTime() == item.StartDate.getTime())
                        {
                            form.StartDate.$setValidity("exist", false);
                        }
                    });

                    if (startDate < new Date())
                    {
                        form.StartDate.$setValidity("future", false);
                    }
                }
            }
        });
    };

    initialize();
}]);