angular.module('app.modules.order.controllers.customerAutoShipsController', [])
.controller('customerAutoShipsController', ['$scope', '$rootScope', '$state', 'orderService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil','customerService',
    function ($scope, $rootScope, $state, orderService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil, customerService)
    {
    	$scope.$on('customerAutoShips#in#init', function (event, args)
    	{
    		$scope.baseUrl = 'https://' + $rootScope.ReferenceData.PublicHost;

            $scope.idCustomer = args.idCustomer;
            $scope.autoShipsFilter = {
                idCustomer: $scope.idCustomer,
                Paging: { PageIndex: 1, PageItemCount: 20 },
                Sorting: gridSorterUtil.resolve(refreshAutoShips, "DateCreated", "Desc")
            };
            if ($scope.idCustomer)
            {
            	refreshAutoShips();
            }
        });

        $scope.$on('customerAutoShips#in#refresh', function (event, args)
        {
        	$scope.autoShipsFilter.Paging.PageIndex = 1;
        	refreshAutoShips();
        });

        $scope.ordersPageChanged = function ()
        {
        	refreshAutoShips();
        };

		$scope.activatePause = function(id, activate) {
		    orderService.activatePauseAutoShip(id, $scope.idCustomer, activate, $scope.addEditTracker)
			.success(function (result) {
				if (result.Success) {
					refreshAutoShips();
				} else {
					if (result.Messages) {
						toaster.pop('error', 'Error!', result.Messages[0].Message);
					} else {
						toaster.pop('error', 'Error!', "Can't start / pause Auto-Ship");
					}
				}
			}).error(function (result) {
			    toaster.pop('error', 'Error!', "Can't start / pause Auto-Ship");
			});
		}

		$scope.editBilling = function(id) {
		    customerService.getCountries($scope.addEditTracker).success(function(res) {
			    orderService.getAutoShipCreditCards(id, $scope.idCustomer, $scope.addEditTracker)
				    .success(function(result) {
					    if (result.Success) {
						    openModal(result, id, res.Data);
					    } else {
						    if (result.Messages) {
							    toaster.pop('error', 'Error!', result.Messages[0].Message);
						    } else {
							    toaster.pop('error', 'Error!', "Can't get auto-ship billing details");
						    }
					    }
				    }).error(function(result) {
					    toaster.pop('error', 'Error!', "Can't get auto-ship billing details");
				    });
		    }).error(function(result) {
			    toaster.pop('error', 'Error!', "Can't load countries");
		    });
	    }

		$scope.emptyOrNull = function (item) {
			return !(item.Value === null || item.Value.trim().length === 0)
		}

		$scope.delete = function(id) {
		    confirmUtil.confirm(function() {
			    orderService.deleteAutoShip(id, $scope.idCustomer, $scope.addEditTracker)
				    .success(function(result) {
					    if (result.Success) {
						    refreshAutoShips();
					    } else {
						    if (result.Messages) {
							    toaster.pop('error', 'Error!', result.Messages[0].Message);
						    } else {
							    toaster.pop('error', 'Error!', "Can't cancel auto-ship");
						    }
					    }
				    }).error(function(result) {
					    toaster.pop('error', 'Error!', "Can't cancel auto-ship");
				    });
		    }, 'Are you sure you want to delete this auto-ship?');
	    }

	    function openModal(result, id, countries) {
			modalUtil.open('app/modules/order/partials/manageAutoShipBilling.html', 'manageAutoShipBillingController', {
				creditCards: result.Data, orderId: id, countries: countries, thenCallback: function () {
					refreshAutoShips();
				}
			});
		}

	    function refreshAutoShips()
        {
        	orderService.getAutoShips($scope.autoShipsFilter, $scope.addEditTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.autoShips = result.Data.Items;
                        $scope.autoShipsTotalItems = result.Data.Count;
                    } else
                    {
                        toaster.pop('error', 'Error!', "Can't process auto-ships");
                    }
                })
                .error(function (result)
                {
                    toaster.pop('error', 'Error!', "Can't process auto-ships");
                });
        };
    }]);