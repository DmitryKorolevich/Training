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
                    (!$scope.filter.LastName || $scope.filter.LastName.length<3) &&
                    (!$scope.filter.FirstName || $scope.filter.FirstName.length<3) &&
                    (!$scope.filter.Address1 || $scope.filter.Address1.length<3) &&
                    (!$scope.filter.SearchText || $scope.filter.SearchText.length<3) &&
                    (!$scope.filter.City || $scope.filter.City.length < 3) &&
                    (!$scope.filter.Zip || $scope.filter.Zip.length<3) &&
                    (!$scope.filter.Phone || $scope.filter.Phone.length<3))
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