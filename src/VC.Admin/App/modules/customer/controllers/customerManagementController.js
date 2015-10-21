'use strict';

angular.module('app.modules.customer.controllers.customerManagementController', [])
	.controller('customerManagementController', [
		'$scope', 'customerService', 'toaster', 'promiseTracker', '$rootScope', 'gridSorterUtil', function ($scope, customerService, toaster, promiseTracker, $rootScope, gridSorterUtil) {
			$scope.refreshTracker = promiseTracker("refresh");
			$scope.editTracker = promiseTracker("edit");           

			function refreshCustomers() {
				customerService.getCustomers($scope.filter, $scope.refreshTracker)
					.success(function(result) {
						if (result.Success) {
							$scope.customers = result.Data.Items;
							$scope.totalItems = result.Data.Count;
						} else {
							toaster.pop('error', 'Error!', "Can't get access to the customers");
						}
					})
					.error(function(result) {
						toaster.pop('error', "Error!", "Server error ocurred");
					});
			};

			function initialize() {
				$scope.filter = {
					SearchText: "",
					Paging: { PageIndex: 1, PageItemCount: 100 },
					Sorting: gridSorterUtil.resolve(refreshCustomers, 'Updated', 'Desc')
				};

				refreshCustomers();
			}

			$scope.pageChanged = function () {
				refreshCustomers();
			};

			$scope.filterCustomers = function () {
				$scope.filter.Paging.PageIndex = 1;

				refreshCustomers();
			};

			initialize();
		}
	]);