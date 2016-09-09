'use strict';

angular.module('app.modules.customer.controllers.searchCustomersController', [])
	.controller('searchCustomersController', [
		'$scope', 'customerService', 'toaster', 'promiseTracker', '$rootScope', 'gridSorterUtil', '$timeout',
        function ($scope, customerService, toaster, promiseTracker, $rootScope, gridSorterUtil, $timeout)
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
					        refresViewItems();
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
		        $scope.blockIds = [];
		        $scope.viewCustomers = [];

		        $scope.filter = {
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

		        $timeout(function ()
		        {
		            notifyLoaded();
		        }, 200);
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
		        if ((!$scope.address.Email) &&
                    (!$scope.address.LastName || $scope.address.LastName.length < 3) &&
                    (!$scope.address.FirstName || $scope.address.FirstName.length < 3) &&
                    (!$scope.defaultShippingAddress.Address1 || $scope.defaultShippingAddress.Address1.length < 3) &&
                    (!$scope.address.SearchText || $scope.address.SearchText.length < 3) &&
                    (!$scope.defaultShippingAddress.City || $scope.defaultShippingAddress.City.length < 3) &&
                    (!$scope.defaultShippingAddress.Zip || $scope.defaultShippingAddress.Zip.length < 3) &&
                    (!$scope.address.Phone || $scope.address.Phone.length < 3) &&
                    (!$scope.address.Company || $scope.address.Company.length < 3))
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

		    $scope.select = function (item)
		    {
		        notifyAboutAddItems([item]);
		    };

		    function notifyAboutAddItems(items)
		    {
		        var data = {};
		        data.name = $scope.name;
		        data.items = items;
		        $scope.$emit('searchCustomers#out#addItems', data);
		    };

		    function notifyLoaded()
		    {
		        var data = {};
		        data.name = $scope.name;
		        $scope.$emit('searchCustomers#out#loaded', data);
		    };

		    $scope.$on('searchCustomers#in#setFilter', function (event, args)
		    {
		        if (args.name == $scope.name)
		        {
		            $scope.address=args.filter.address;
		            $scope.defaultShippingAddress=args.filter.defaultShippingAddress;
		        };
		    });

		    $scope.$on('searchCustomers#in#search', function (event, args)
		    {
		        if (args.name == $scope.name)
		        {
		            $scope.filterCustomers();
		        }
		    });

		    $scope.$on('searchCustomers#in#setBlockIds', function (event, args)
		    {
		        if (args.name == $scope.name)
		        {
		            var blockIds = args.blockIds;
		            if (blockIds)
		            {
		                $scope.blockIds = blockIds;
		                refresViewItems();
		            }
		        };
		    });

		    function refresViewItems()
		    {
		        var items = [];
		        $.each($scope.customers, function (index, customer)
		        {
		            var add = true;
		            $.each($scope.blockIds, function (index, blockId)
		            {
		                if (customer.Id == blockId)
		                {
		                    add = false;
		                    return;
		                }
		            });
		            if (add)
		            {
		                items.push(customer);
		            };
		        });
		        $scope.viewCustomers = items;
		    };

		    initialize();
		}
	]);