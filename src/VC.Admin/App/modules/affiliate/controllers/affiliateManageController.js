'use strict';

angular.module('app.modules.affiliate.controllers.affiliateManageController', [])
.controller('affiliateManageController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', '$modal',
    'affiliateService', 'customerService', 'modalUtil', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, $timeout, $modal, affiliateService, customerService, modalUtil, toaster, confirmUtil, promiseTracker)
    {
        $scope.refreshTracker = promiseTracker("get");

        function successSaveHandler(result)
        {
            if (result.Success)
            {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.affiliate.Id = result.Data.Id;
                $scope.affiliate.DateEdited = result.Data.DateEdited;
            } else
            {
                var messages = "";
                if (result.Messages)
                {
                    $scope.forms.submitted = true;
                    $scope.detailsTab.active = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);

                    $.each(result.Messages, function (index, value)
                    {
                        if (value.Field)
                        {
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

        function errorHandler(result)
        {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function initialize()
        {
            $scope.id = $stateParams.id ? $stateParams.id : 0;

            $scope.affiliateTiers = $rootScope.ReferenceData.AffiliateTiers;

            $scope.forms = {};
            $scope.detailsTab = {
                active: true
            };

            loadCountries();
        };

        function loadCountries()
        {
            customerService.getCountries($scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.countries = result.Data;
                        loadAffiliate();
                    } else
                    {
                        errorHandler(result);
                    }
                }).
                error(function (result)
                {
                    errorHandler(result);
                });
        }

        function loadAffiliate()
        {
            affiliateService.getAffiliate($scope.id, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.affiliate = result.Data;
                    } else
                    {
                        errorHandler(result);
                    }
                }).
                error(function (result)
                {
                    errorHandler(result);
                });
        }

        $scope.save = function ()
        {
            clearServerValidation();

            if ($scope.forms.mainForm.$valid)
            {

                affiliateService.updateAffiliate($scope.affiliate, $scope.refreshTracker).success(function (result)
                {
                    successSaveHandler(result);
                }).error(function (result)
                {
                    errorHandler(result);
                });
            } else
            {
                $scope.forms.submitted = true;
                $scope.detailsTab.active = true;
            }
        };

        var clearServerValidation = function ()
        {
            $.each($scope.forms, function (index, form)
            {
                if (form && !(typeof form === 'boolean'))
                {
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

        $scope.statesByCountryId = function (countryId)
        {
            var states = null;
            if (countryId)
            {
                var country = null;
                $.each($scope.countries, function(index,item)
                {
                    if(item.Id==countryId)
                    {
                        country = item;
                        return;
                    }
                });
                if (country)
                {
                    states = country.States;
                }
            }
            return states;
        };

        $scope.notify = function ()
        {
            var data =
                {
                    Type: 1,//notify
                    ToName: $scope.affiliate.Name,
                    ToEmail: $scope.affiliate.Email,
                };
            modalUtil.open('app/modules/affiliate/partials/affiliateSendEmail.html', 'affiliateSendEmailController', data);
        };

        $scope.email = function ()
        {
            var data =
                {
                    Type: 2,//email
                    ToName: $scope.affiliate.Name,
                    ToEmail: $scope.affiliate.Email,
                };
            modalUtil.open('app/modules/affiliate/partials/affiliateSendEmail.html', 'affiliateSendEmailController', data);
        };

        initialize();
    }
]);