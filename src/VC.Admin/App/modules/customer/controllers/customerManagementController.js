﻿'use strict';

angular.module('app.modules.customer.controllers.customerManagementController', [])
	.controller('customerManagementController', [
		'$scope', 'customerService', 'toaster', 'promiseTracker', '$rootScope', 'gridSorterUtil', function ($scope, customerService, toaster, promiseTracker, $rootScope, gridSorterUtil)
		{
		    $scope.refreshTracker = promiseTracker("refresh");
		    $scope.editTracker = promiseTracker("edit");

		    function refreshCustomers()
		    {
		        customerService.getCustomers($scope.filter, $scope.refreshTracker)
					.success(function (result)
					{
					    if (result.Success)
					    {
					        $scope.customers = result.Data.Items;
					        $scope.totalItems = result.Data.Count;
					    } else
					    {
					        toaster.pop('error', 'Error!', "Can't get access to the customers");
					    }
					})
					.error(function (result)
					{
					    toaster.pop('error', "Error!", "Server error ocurred");
					});
		    };

		    function initialize()
		    {
		        $scope.forms = {};

		        $scope.address = {};
		        $scope.defaultShippingAddress = {};

		        $scope.filter = {
		            IdOrder: null,
		            Address: null,
		            DefaultShippingAddress: null,
		            SearchText: "",
		            Paging: { PageIndex: 1, PageItemCount: 100 },
		            Sorting: gridSorterUtil.resolve(refreshCustomers, 'Updated', 'Desc')
		        };

		        $scope.autoCompleteFilter = {
                    FieldName: null,
                    FieldValue: null,
                };
		    }

		    $scope.pageChanged = function ()
		    {
		        refreshCustomers();
		    };

		    $scope.filterCustomers = function (event)
		    {
		        if (!isCustomerFilterAllowSearch())
		        {                    
                    return;
		        }

		        $scope.filter.Address = angular.copy($scope.address);
		        $scope.filter.DefaultShippingAddress = angular.copy($scope.defaultShippingAddress);
		        $scope.filter.Email = $scope.address.Email;
		        $scope.filter.SearchText = $scope.address.SearchText;
                $scope.filter.Paging.PageIndex = 1;

		        refreshCustomers();
		    };

		    var isCustomerFilterAllowSearch = function ()
		    {
		        if ((!$scope.address.Email || $scope.address.Email.length < 3) &&
                    (!$scope.address.LastName || $scope.address.LastName.length < 3) &&
                    (!$scope.address.FirstName || $scope.address.FirstName.length < 3) &&
                    (!$scope.defaultShippingAddress.Address1 || $scope.defaultShippingAddress.Address1.length < 3) &&
                    (!$scope.address.SearchText || $scope.address.SearchText.length < 3) &&
                    (!$scope.defaultShippingAddress.City || $scope.defaultShippingAddress.City.length < 3) &&
                    (!$scope.defaultShippingAddress.Zip || $scope.defaultShippingAddress.Zip.length < 3) &&
                    (!$scope.address.Phone || $scope.address.Phone.length < 3) &&
                    (!$scope.address.Company || $scope.address.Company.length < 3) &&
                    (!$scope.filter.IdOrder || $scope.filter.IdOrder.length < 3))
		        {
		            toaster.pop('error', "Info", "At least one field should be filled with at least 3 characters.");
		            return false;
		        }
		        return true;
		    };
            
		    $scope.getByStaticAutoComplete = function (val, field)
		    {
		        if (val)
		        {
		            $scope.autoCompleteFilter.FieldName = field;
		            $scope.autoCompleteFilter.FieldValue = val;
		            return customerService.getCustomerStaticFieldValuesByValue($scope.autoCompleteFilter)
                        .then(function (result)
                        {
                            return result.data.Data.map(function (item)
                            {
                                return item;
                            });
                        });
		        }
		    };

		    $scope.getByProfileAddressAutoComplete = function (val, field)
		    {
		        if (val)
		        {
		            $scope.autoCompleteFilter.FieldName = field;
		            $scope.autoCompleteFilter.FieldValue = val;
		            return customerService.getProfileAddressFieldValuesByValueAsync($scope.autoCompleteFilter)
                        .then(function (result)
                        {
                            return result.data.Data.map(function (item)
                            {
                                return item;
                            });
                        });
		        }
		    };

		    $scope.getByDefaultShippingAddressAutoComplete = function (val, field)
		    {
		        if (val)
		        {
		            $scope.autoCompleteFilter.FieldName = field;
		            $scope.autoCompleteFilter.FieldValue = val;
		            return customerService.getDefaultShippingAddressFieldValuesByValueAsync($scope.autoCompleteFilter)
                        .then(function (result)
                        {
                            return result.data.Data.map(function (item)
                            {
                                return item;
                            });
                        });
		        }
		    };		    

		    $scope.applySort = function (columnName)
		    {
		        if ($scope.filter.Address != null)
		        {
		            $scope.filter.Sorting.applySort(columnName);
		        }
		    };

		    initialize();
		}
	]);