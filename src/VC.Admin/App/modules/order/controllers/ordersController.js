﻿angular.module('app.modules.order.controllers.ordersController', [])
.controller('ordersController', ['$scope', '$rootScope', '$state', '$stateParams', 'orderService', 'settingService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $stateParams, orderService, settingService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.deleteTracker = promiseTracker("delete");

        function errorHandler(result)
        {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function refreshOrders()
        {
            var filter = $scope.forms.IsActive ? $scope.filter : $scope.directOrdersfilter;
            filter.Sorting = $scope.filter.Sorting;

            var data = {};
            angular.copy(filter, data);
            if (data.From)
            {
                data.From = data.From.toServerDateTime();
            }
            if (data.To)
            {
                data.To = data.To.toServerDateTime();
            }

            orderService.getOrders(data, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {                        
                        $.each(result.Data.Items, function (index, item)
                        {
                            item.AllowExport = item.OrderStatus == 2;
                            item.IsSelected = item.OrderStatus == 3 || item.OrderStatus == 5;//Shipped
                            item.PAllowExport = item.POrderStatus == 2;
                            item.IsPSelected = item.POrderStatus == 3 || item.POrderStatus==5;//Shipped
                            item.NPAllowExport = item.NPOrderStatus == 2;
                            item.IsNPSelected = item.NPOrderStatus == 3 || item.NPOrderStatus == 5;//Shipped
                        });

                        $scope.items = result.Data.Items;
                        $scope.totalItems = result.Data.Count;
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

        function initialize()
        {
            $scope.customerTypes = angular.copy($rootScope.ReferenceData.CustomerTypes);
            $scope.customerTypes.splice(0, 0, { Key: null, Text: 'All Customer Types' });

            $scope.orderStatuses = angular.copy($rootScope.ReferenceData.OrderStatuses);
            $scope.orderStatuses.splice(0, 0, { Key: null, Text: 'All Order Statuses' });

            $scope.orderTypes = angular.copy($rootScope.ReferenceData.OrderTypes);
            $scope.orderTypes.splice(0, 0, { Key: null, Text: 'All Order Types' });

            $scope.orderSourceTypes = angular.copy($rootScope.ReferenceData.OrderSourceTypes);
            $scope.orderSourceTypes.splice(0, 0, { Key: null, Text: 'All Order Sources' });

            $scope.shippingMethods = [
                { Key: null, Text: 'All Shipping Methods' },
                { Key: 1, Text: 'Upgraded' },
                { Key: 2, Text: 'None' }
            ];
            $scope.settings = {};
            $scope.settings.allExport = false;

            $scope.forms = {};

            $scope.items = [];
            $scope.states = [];

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            var idSku = $stateParams.idsku ? $stateParams.idsku : null;
            var from = $stateParams.from ? Date.parseDateTime($stateParams.from) : currentDate.shiftDate('-1m');
            var to = $stateParams.to ? Date.parseDateTime($stateParams.to) : currentDate.shiftDate('+1d');
            $scope.filter = {
                To: to,
                From: from,
                ShipDate: false,
                OrderStatus: null,
                IdObjectType: null,
                IdOrderSource: null,
                POrderType: null,
                IdSku: idSku,
                IdCustomerType: null,
                IdShippingMethod: null,
                IdShipState: null,
                CustomerFirstName: null,
                CustomerLastName: null,
                CustomerCompany: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshOrders, "DateCreated", "Desc"),
                IsActive: true,
            };

            $scope.directOrdersfilter = {
                To: null,
                From: null,
                ShipDate: false,
                OrderStatus: null,
                IdObjectType: null,
                IdOrderSource: null,
                POrderType: null,
                IdCustomerType: null,
                IdShippingMethod: null,
                Paging: { PageIndex: 1, PageItemCount: 100 },
                Sorting: gridSorterUtil.resolve(refreshOrders, "DateCreated", "Desc"),
            };

            $scope.$watch("filter.ShipDate", function (newValue, oldValue)
            {
                if (newValue)
                {
                    $scope.filter.OrderStatus = 3;//Shipped
                }
                else
                {
                    $scope.filter.OrderStatus = null;
                }
            });

            $scope.forms.IsActive = true;

            settingService.getCountries({}, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.state = result.Data;
                        $.each(result.Data, function (index, country)
                        {
                            if (country.CountryCode == 'US')
                            {
                                $scope.states = country.States;
                                $scope.states.splice(0, 0, { Id: null, StateName: 'All'});
                            }
                        });
                        refreshOrders();
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

        $scope.filterOrders = function ()
        {
            if ($scope.filter.From > $scope.filter.To)
            {
                toaster.pop('error', "Error!", "'To' date can't be less than 'From' date.", null, 'trustedHtml');
                return;
            }
            $scope.forms.IsActive = true;
            $scope.forms.form.submitted = false;
            $scope.filter.Paging.PageIndex = 1;
            refreshOrders();
            //if ($scope.forms.filterForm.$valid)
            //{
            //    $scope.forms.filterForm.submitted = false;
            //    $scope.filter.Paging.PageIndex = 1;
            //    refreshOrders();
            //}
            //else
            //{
            //    $scope.forms.filterForm.submitted = true;
            //}
        };

        $scope.directFilterOrders = function ()
        {
            $scope.forms.IsActive = false;
            //$scope.forms.filterForm.submitted = false;
            if ($scope.forms.form.$valid)
            {
                $scope.forms.form.submitted = false;
                $scope.directOrdersfilter.Paging.PageIndex = 1;
                refreshOrders();
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.pageChanged = function ()
        {
            refreshOrders();
        };

        $scope.cancel = function (id)
        {
            confirmUtil.confirm(function ()
            {
                orderService.cancelOrder(id, $scope.deleteTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            toaster.pop('success', "Success!", "Successfully canceled.");
                            refreshOrders();
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to cancel this order?');
        };

        $scope.cancelRefund = function (id)
        {
            confirmUtil.confirm(function ()
            {
                orderService.cancelRefundOrder(id, $scope.deleteTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            toaster.pop('success', "Success!", "Successfully canceled.");
                            refreshOrders();
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
            }, 'Are you sure you want to cancel this refund?');
        };

        $scope.allExportCall = function ()
        {
            $.each($scope.items, function (index, item)
            {
                if (item.AllowExport)
                {
                    item.IsSelected = $scope.settings.allExport;
                }
                if (item.PAllowExport)
                {
                    item.IsPSelected = $scope.settings.allExport;
                }
                if (item.NPAllowExport)
                {
                    item.IsNPSelected = $scope.settings.allExport;
                }
            });
        };

        $scope.itemExportChanged = function (item, event)
        {
            if (!item.IsSelected && $scope.settings.allExport)
            {
                $scope.settings.allExport = false;
            }
        };

        $scope.PItemExportChanged = function (item, event)
        {
            if (!item.IsPSelected && $scope.settings.allExport)
            {
                $scope.settings.allExport = false;
            }
        };

        $scope.NPItemExportChanged = function (item, event)
        {
            if (!item.IsNPSelected && $scope.settings.allExport)
            {
                $scope.settings.allExport = false;
            }
        };

        $scope.exportOrders = function ()
        {

        };

        $scope.openOrder = function (id)
        {
            $state.go('index.oneCol.orderDetail', { id: id });
        };

        initialize();
    }]);