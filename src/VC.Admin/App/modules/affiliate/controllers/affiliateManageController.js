'use strict';

angular.module('app.modules.affiliate.controllers.affiliateManageController', [])
.controller('affiliateManageController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', '$uibModal',
    'affiliateService', 'customerService', 'modalUtil', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, $timeout, $uibModal, affiliateService, customerService, modalUtil, toaster, confirmUtil, promiseTracker)
    {
        $scope.refreshTracker = promiseTracker("get");
        $scope.resetTracker = promiseTracker("reset");
        $scope.resendTracker = promiseTracker("resend");
        $scope.refreshCustomersTracker = promiseTracker("refreshCustomersTracker");
        $scope.refreshOrdersTracker = promiseTracker("refreshOrdersTracker");
        $scope.refreshPaymentHistoryTracker = promiseTracker("refreshPaymentHistoryTracker");

        function refreshHistory()
        {
            if ($scope.affiliate && $scope.affiliate.Id)
            {
                var data = {};
                data.service = affiliateService;
                data.tracker = $scope.refreshTracker;
                data.idObject = $scope.affiliate.Id;
                data.idObjectType = 7//customer
                $scope.$broadcast('objectHistorySection#in#refresh', data);
            }
        }

        function successSaveHandler(result)
        {
            if (result.Success)
            {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.affiliate.Id = result.Data.Id;
                $scope.orderFilter.IdAffiliate = result.Data.Id;
                $scope.customerFilter.IdAffiliate = result.Data.Id;
                $scope.affiliate.DateEdited = result.Data.DateEdited;
                $scope.affiliate.Email = result.Data.Email;
                $scope.affiliate.StatusCode = result.Data.StatusCode;
                $scope.affiliate.ActivatePending = false;
                $scope.options.LoginAsAffiliateUrl = affiliateService.getLoginAsAffiliateUrl($scope.affiliate.Id, $rootScope.buildNumber);
                refreshHistory();
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

        function errorHandlerEmail(result)
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
            $scope.id = $stateParams.id ? $stateParams.id : 0;

            $scope.affiliateTiers = $rootScope.ReferenceData.AffiliateTiers;

            $scope.forms = {};
            $scope.detailsTab = {
                active: true
            };

            $scope.options = {};
            if ($scope.id)
            {
                $scope.options.customersExportUrl = customerService.getCustomersForAffiliatesReportFileUrl($scope.id, $rootScope.buildNumber);
            }

            $scope.customerFilter = {
                IdAffiliate: $scope.id,
                IdAffiliateRequired: true,
                Paging: { PageIndex: 1, PageItemCount: 100 },
            };

            $scope.orderFilter = {
                From: null,
                To: null,
                IdAffiliate: $scope.id,
                Paging: { PageIndex: 1, PageItemCount: 100 },
            };

            loadCountries();
            refreshCustomers();
            refreshOrders();
            refreshPaymentHistory();
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
                        $scope.affiliate.ActivatePending = false;
                        $scope.options.LoginAsAffiliateUrl = affiliateService.getLoginAsAffiliateUrl($scope.affiliate.Id, $rootScope.buildNumber);
                        refreshHistory();
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
                var data = angular.copy($scope.affiliate);
                if (data.newEmail || data.emailConfirm)
                {
                    data.Email = data.newEmail;
                    data.EmailConfirm = data.emailConfirm;
                } else
                {
                    data.EmailConfirm = data.Email;
                }

                if (data.StatusCode != 4)
                {
                    data.SuspendMessage = null;
                }
                if (data.ActivatePending)
                {
                    data.StatusCode = 2;//active
                }

                affiliateService.updateAffiliate(data, $scope.refreshTracker).success(function (result)
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
                toaster.pop('error', "Error!", "Validation errors, please correct field values.", null, 'trustedHtml');
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

        $scope.toggleSuspended = function ()
        {
            $scope.affiliate.StatusCode = $scope.affiliate.StatusCode == 4 ? ($scope.affiliate.IsConfirmed ? 2 : 1) : 4;
        };

        $scope.resend = function ()
        {
            affiliateService.resendActivation($scope.affiliate.PublicUserId, $scope.resendTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        toaster.pop('success', "Success!", "Successfully sent");

                    } else
                    {
                        errorHandlerEmail(result);
                    }
                }).error(function ()
                {
                    toaster.pop('error', "Error!", "Server error occured");
                });
        };

        $scope.resetPassword = function ()
        {
            affiliateService.resetPassword($scope.affiliate.PublicUserId, $scope.resetTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        toaster.pop('success', "Success!", "Successfully reset");
                    } else
                    {
                        errorHandlerEmail(result);
                    }
                }).error(function ()
                {
                    toaster.pop('error', "Error!", "Server error occured");
                });
        };

        function refreshCustomers()
        {
            if ($scope.customerFilter.IdAffiliate)
            {
                customerService.getCustomers($scope.customerFilter, $scope.refreshCustomersTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            $scope.customers = result.Data.Items;
                            $scope.customersTotalItems = result.Data.Count;
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
            }
        };

        $scope.customersPageChanged = function ()
        {
            refreshCustomers();
        };

        function refreshOrders()
        {
            if ($scope.orderFilter.IdAffiliate)
            {
                var data = {};
                angular.copy($scope.orderFilter, data);
                if (data.From)
                {
                    data.From = data.From.toServerDateTime();
                }
                if (data.To)
                {
                    data.To = data.To.toServerDateTime();
                }
                affiliateService.getAffiliateOrderPaymentsWithCustomerInfo(data, $scope.refreshOrdersTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            $scope.orders = result.Data.Items;
                            $scope.ordersTotalItems = result.Data.Count;
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
            }
        };

        $scope.filterOrders = function ()
        {
            $scope.orderFilter.Paging.PageIndex = 1;
            refreshOrders();
        };

        $scope.ordersPageChanged = function ()
        {
            refreshOrders();
        };

        function refreshPaymentHistory()
        {
            if ($scope.orderFilter.IdAffiliate)
            {
                affiliateService.getAffiliatePaymentHistory($scope.orderFilter.IdAffiliate, $scope.refreshPaymentHistoryTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            $scope.paymentHistoryItems = result.Data;
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
            }
        };

        initialize();
    }
]);