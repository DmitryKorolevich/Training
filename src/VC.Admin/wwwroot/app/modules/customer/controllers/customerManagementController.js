'use strict';

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

		        $scope.filter = {
                    Address: {},
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

		    $scope.filterCustomers = function ()
		    {
		        if ((!$scope.filter.Email || $scope.filter.Email.length<3) &&
                    (!$scope.filter.Address.LastName || $scope.filter.Address.LastName.length < 3) &&
                    (!$scope.filter.Address.FirstName || $scope.filter.Address.FirstName.length < 3) &&
                    (!$scope.filter.Address.Address1 || $scope.filter.Address.Address1.length < 3) &&
                    (!$scope.filter.SearchText || $scope.filter.SearchText.length<3) &&
                    (!$scope.filter.Address.City || $scope.filter.Address.City.length < 3) &&
                    (!$scope.filter.Address.Zip || $scope.filter.Address.Zip.length < 3) &&
                    (!$scope.filter.Address.Phone || $scope.filter.Address.Phone.length < 3))
		        {                    
					toaster.pop('error', "Info", "At least one field should be filled with at least 3 characters.");
                    return;
		        }

		        $scope.filter.Paging.PageIndex = 1;

		        refreshCustomers();
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

		    initialize();
		}
	]);