'use strict';

angular.module('app.modules.customer.controllers.mergeCustomersController', [])
	.controller('mergeCustomersController', [
		'$scope', 'customerService', 'toaster', 'promiseTracker',
        '$timeout', '$rootScope', 'gridSorterUtil', function ($scope, customerService, toaster, promiseTracker,
         $timeout, $rootScope, gridSorterUtil)
		{
		    const step1Name = 'Step1SearchCustomers';
		    const step2Name = 'Step2SearchCustomers';

		    $scope.refreshTracker = promiseTracker("refresh");
		    $scope.dublicateEmailsRefreshTracker = promiseTracker("dublicateEmailsRefresh");

		    function initialize()
		    {
		        $scope.forms = {};

		        $scope.primary = null;
		        $scope.selectedCustomers = [];

		        $scope.filter = {
		            Paging: { PageIndex: 1, PageItemCount: 100 }
		        };

		        refreshDublicateEmails();
		    };

		    function refreshDublicateEmails()
		    {
		        customerService.getCustomersWithDublicateEmails($scope.filter, $scope.dublicateEmailsRefreshTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            $scope.dublicateEmails = result.Data.Items;
                            $scope.dublicateTotalItems = result.Data.Count;
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
		    };

		    $scope.pageChanged = function ()
		    {
		        refreshDublicateEmails();
		    };

		    $scope.selectDublicateEmail = function (item)
		    {
		        $scope.primary = null;
		        $scope.selectedCustomers = [];

		        $timeout(function ()
		        {
		            var data = {};
		            data.name = step1Name;
		            data.filter = {
		                address: {
		                    Email: item.Email
		                },
		                defaultShippingAddress: {}
		            };
		            $scope.$broadcast('searchCustomers#in#setFilter', data);
		            $scope.$broadcast('searchCustomers#in#search', data);
		            $rootScope.scrollTo(step1Name);
		        }, 200);
		    };

		    $scope.$on('searchCustomers#out#addItems', function (event, args)
		    {
		        if (args.name == step1Name && args.items.length == 1)
		        {
		            $scope.primary = args.items[0];
		        }
		        if (args.name == step2Name)
		        {
		            $.each(args.items, function (index, item)
		            {
		                if ($scope.primary == null || ($scope.primary != null && item.Id != $scope.primary.Id))
		                {
		                    var add = true;
		                    $.each($scope.selectedCustomers, function (index, selectedCustomer)
		                    {
		                        if (item.Id == selectedCustomer.Id)
		                        {
		                            add = false;
		                            return;
		                        }
		                    });
		                    if (add)
		                    {
		                        var newSelectedCustomer = angular.copy(item);
		                        $scope.selectedCustomers.push(newSelectedCustomer);
		                    }
		                }
		            });
		        }
		    });

		    $scope.unselectCustomer = function (index)
		    {
		        $scope.selectedCustomers.splice(index, 1);
		    };

		    $scope.selectAnotherPrimary = function ()
		    {
		        $scope.primary = null;
		        $scope.selectedCustomers = [];
		    };

		    $scope.mergeCustomers = function ()
		    {
		        if ($scope.primary && $scope.selectedCustomers && $scope.selectedCustomers.length > 0)
		        {
		            var ids = $.map($scope.selectedCustomers, function (item)
		            {
		                return item.Id;
		            });
		            customerService.mergeCustomers($scope.primary.Id, ids, $scope.refreshTracker)
                        .success(function (result)
                        {
                            if (result.Success)
                            {
                                $scope.primary = null;
                                $scope.selectedCustomers = [];
                                toaster.pop('success', "Success!", "Successfully merged");
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
		        else
		        {
		            toaster.pop('error', 'Error!', 'Please select at least one customer for merge.', null, 'trustedHtml');
		        }
		    };

		    $scope.userTheSameEmailStep2 = function ()
		    {
		        var data = {};
		        data.name = step2Name;
		        data.filter = {
		            address: {
		                Email: $scope.primary.Email
		            },
		            defaultShippingAddress: {}
		        };
		        $scope.$broadcast('searchCustomers#in#setFilter', data);
		        $scope.$broadcast('searchCustomers#in#search', data);
		        $rootScope.scrollTo(step1Name);
		    };

		    initialize();
		}
	]);