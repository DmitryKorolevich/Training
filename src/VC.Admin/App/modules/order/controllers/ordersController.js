angular.module('app.modules.order.controllers.ordersController', [])
.controller('ordersController', ['$scope', '$rootScope', '$state', '$stateParams', '$location', '$q', 'orderService', 'settingService', 'userService',
    'gcService', 'monitorService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $stateParams, $location, $q, orderService, settingService, userService,
        gcService, monitorService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.deleteTracker = promiseTracker("delete");
        $scope.gcRefreshTracker = promiseTracker("gcRefresh");

        function errorHandler(result)
        {
            if (result.Messages)
            {
                var messages = "";
                $.each(result.Messages, function (index, value)
                {
                    messages += value.Message + "<br />";
                });
                toaster.pop('error', "Error!", messages, null, 'trustedHtml');
            }
            else
            {
                toaster.pop('error', "Error!", "Server error occured");
            }
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

            $scope.filter.Paging.PageItemCountUsed = $scope.filter.Paging.PageItemCount;
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

            $scope.orderTypes = angular.copy($rootScope.ReferenceData.PublicOrderTypes);
            $scope.orderTypes.splice(0, 0, { Key: null, Text: 'All Order Types' });

            $scope.orderSourceTypes = angular.copy($rootScope.ReferenceData.OrderSourceTypes);
            $scope.orderSourceTypes.splice(0, 0, { Key: null, Text: 'All Order Sources' });

            $scope.pageSizeOptions = [
                { Key: 50 },
                { Key: 75 },
                { Key: 100 },
                { Key: 200 }
            ];

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
            $scope.options = {};

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            var idSku = $stateParams.idsku ? $stateParams.idsku : null;
            var from = $stateParams.from ? Date.parseDateTime($stateParams.from) : currentDate.shiftDate('-1m');
            var to = $stateParams.to ? Date.parseDateTime($stateParams.to) : currentDate.shiftDate('+1d');
            var statuses = $stateParams.statuses ? $stateParams.statuses.split(',') : null;
            $scope.filter = {
                To: to,
                From: from,
                ShipDate: false,
                OrderStatus: null,
                IdObjectType: null,
                IdOrderSource: null,
                POrderType: null,
                IdSku: idSku,
                IncludeOrderStatuses: statuses,
                IdCustomerType: null,
                IdShippingMethod: null,
                IdShipState: null,
                CustomerFirstName: null,
                CustomerLastName: null,
                CustomerCompany: null,
                IdAddedBy: null,
                Paging: { PageIndex: 1, PageItemCount: 200 },
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
                Paging: { PageIndex: 1, PageItemCount: 200 },
                Sorting: gridSorterUtil.resolve(refreshOrders, "DateCreated", "Desc"),
            };

            $scope.gcfilter = {
                ExactCode: null,
                Paging: { PageIndex: 1, PageItemCount: 1 },
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


            $q.all({
                countriesCall: settingService.getCountries({}, $scope.refreshTracker),
                adminsCall: userService.getUsers({
                    Sorting: gridSorterUtil.resolve(refreshOrders, "AgentId", "Asc")
                }, $scope.refreshTracker),
            }).then(function (result)
            {
                if (result.countriesCall.data.Success && result.adminsCall.data.Success)
                {
                    $scope.state = result.countriesCall.data.Data;
                    $.each(result.countriesCall.data.Data, function (index, country)
                    {
                        if (country.CountryCode == 'US')
                        {
                            $scope.states = country.States;
                            $scope.states.splice(0, 0, { Id: null, StateName: 'All' });
                        }
                    });
                    $scope.admins = result.adminsCall.data.Data.Items;
                    $scope.admins.splice(0, 0, { Id: null, AgentId: 'All Agents' });

                    refreshOrders();
                }
                else
                {
                    errorHandler(result);
                }
            });
            if ($rootScope.exportStatusRefreshTimer != null)
            {
                clearTimeout($rootScope.exportStatusRefreshTimer);
            }
            refreshExportStatus();
        }

        var refreshExportStatus = function ()
        {
            if ($state.is('index.oneCol.manageOrders') || $state.is('index.oneCol.dashboard'))
            {
                monitorService.getExportGeneralStatus()
                    .success(function (result)
                    {
                        $rootScope.exportStatusRefreshTimer = setTimeout(refreshExportStatus, 5000);
                        if (result.Success)
                        {
                            $scope.options.exportStatus = result.Data;
                        } else
                        {
                            errorHandler(result);
                        }
                    })
                    .error(function (result)
                    {
                        $rootScope.exportStatusRefreshTimer = setTimeout(refreshExportStatus, 5000);
                        errorHandler(result);
                    });
            }
        };

        $scope.filterGCs = function ()
        {
            if ($scope.gcfilter.ExactCode)
            {
                $scope.gc = null;
                $scope.options.gcSearchUsed = false;
                gcService.getGiftCertificates($scope.gcfilter, $scope.gcRefreshTracker)
                    .success(function (result)
                    {
                        if (result.Success)
                        {
                            if (result.Data.Items && result.Data.Items.length > 0)
                            {
                                $scope.gc = result.Data.Items[0];
                            }
                            else
                            {
                                $scope.gc = null;
                            }
                            $scope.options.gcSearchUsed = true;
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
            $scope.filter.IdSku = null;
            $scope.filter.IncludeOrderStatuses = null;
            $scope.forms.IsActive = true;
            if ($scope.forms.filterForm.$valid)
            {
                if ($scope.filter.From > $scope.filter.To)
                {
                    toaster.pop('error', "Error!", "'To' date can't be less than 'From' date.", null, 'trustedHtml');
                    return;
                }
                $scope.forms.filterForm.submitted = false;
                $scope.filter.Paging.PageIndex = 1;
                refreshOrders();
            }
            else
            {
                $scope.forms.filterForm.submitted = true;
            }
        };

        $scope.directFilterOrders = function ()
        {
            $scope.forms.IsActive = false;
            $scope.forms.filterForm.submitted = false;
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

        $scope.exportOrders = function () {
            var exportItems = [];

            $.each($scope.items, function (index, item) {
                if (item.IsSelected && item.OrderStatus == 2 || item.IsPSelected && item.POrderStatus == 2 || item.IsNPSelected && item.NPOrderStatus == 2) {
                    var exportItem = {};
                    exportItem.Id = item.Id;
                    exportItem.IsRefund = item.IdObjectType == 6;
                    exportItem.OrderType = (
                            item.IsSelected && item.OrderStatus == 2
                            || (
                                item.IsNPSelected && item.NPOrderStatus == 2
                                && item.IsPSelected && item.POrderStatus == 2
                            )
                        )
                        ? 0
                        : (item.IsPSelected && item.POrderStatus == 2 ? 1 : 2);

                    exportItems.push(exportItem);
                }
            });

            orderService.exportOrders(exportItems, $scope.refreshTracker)
                    .success(function (result)
                    {
                        if (result.Success) {
                            toaster.pop('success', "Success!", "Export Successfully Started");
                        } else
                        {
                            var messages = "";
                            if (result.Messages) {
                                $.each(result.Messages, function (index, value) {
                                    messages += value.Message + "<br />";
                                });
                            } else {
                                messages = "Can't export orders";
                            }
                            toaster.pop('error', 'Error!', messages, null, 'trustedHtml');
                        }
                    })
                    .error(function (result)
                    {
                        errorHandler(result);
                    });
        };

        $scope.openOrder = function (id)
        {
            $state.go('index.oneCol.orderDetail', { id: id });
        };

        $scope.showExportDetails = function ()
        {
            orderService.getExportDetails()
                .success(function (result)
                {
                    if (result.Success)
                    {
                        modalUtil.open('app/modules/order/partials/exportRequestDetails.html', 'exportRequestDetailsController', {
                            items: result.Data
                        });
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

        initialize();
    }]);