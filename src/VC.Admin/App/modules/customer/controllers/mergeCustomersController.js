'use strict';

angular.module('app.modules.customer.controllers.mergeCustomersController', [])
	.controller('mergeCustomersController', [
		'$scope', 'customerService', 'toaster', 'promiseTracker', '$rootScope', 'gridSorterUtil', function ($scope, customerService, toaster, promiseTracker, $rootScope, gridSorterUtil)
		{
		    $scope.refreshTracker = promiseTracker("refresh");

		    function initialize()
		    {
		        $scope.forms = {};

		        $scope.primary = null;
		        $scope.selectedCustomers = [];
		    };

		    $scope.$on('searchCustomers#out#addItems', function (event, args)
		    {
		        if (args.name == 'Step1SearchCustomers' && args.items.length==1)
		        {
		            $scope.primary = args.items[0];
		        }
		        if (args.name == 'Step2SearchCustomers')
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

		    $scope.unselectCustomer =  function (index) {
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
		            var ids =$.map($scope.selectedCustomers, function(item){
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

		    initialize();
		}
	]);