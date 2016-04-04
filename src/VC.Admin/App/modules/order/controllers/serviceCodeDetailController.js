angular.module('app.modules.order.controllers.serviceCodeDetailController', [])
.controller('serviceCodeDetailController', ['$scope', '$rootScope', '$state', '$stateParams', 'serviceCodeService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil',
    function ($scope, $rootScope, $state, $stateParams, serviceCodeService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function errorHandler(result)
        {
            var messages = "";
            if (result.Messages)
            {
                $.each(result.Messages, function (index, value)
                {
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        };

        function refreshRefunds()
        {
            var data = {};
            angular.copy($scope.filter, data);
            if (data.From)
            {
                data.From = data.From.toServerDateTime();
            }
            if (data.To)
            {
                data.To = data.To.toServerDateTime();
            }
            data.Paging = angular.copy($scope.RefundPaging);

            serviceCodeService.getServiceCodeRefundItems(data, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.totalRefundItems = result.Data.Count;
                        $scope.refunds = result.Data.Items;
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

        function refreshReships()
        {
            var data = {};
            angular.copy($scope.filter, data);
            if (data.From)
            {
                data.From = data.From.toServerDateTime();
            }
            if (data.To)
            {
                data.To = data.To.toServerDateTime();
            }
            data.Paging = angular.copy($scope.ReshipPaging);

            serviceCodeService.getServiceCodeReshipItems(data, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.totalReshipItems = result.Data.Count;
                        $scope.reships = result.Data.Items;
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
            $scope.forms = {};
            $scope.options = {};
            $scope.options.ServiceCode = $stateParams.id;

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            $scope.filter = {
                To: $stateParams.to ? Date.parseDateTime($stateParams.to) : currentDate.shiftDate('+1d'),
                From: $stateParams.from ? Date.parseDateTime($stateParams.from) : currentDate.shiftDate('-1m'),
                ServiceCode: $stateParams.id,
            };

            $scope.RefundPaging = { PageIndex: 1, PageItemCount: 100 };
            $scope.ReshipPaging = { PageIndex: 1, PageItemCount: 100 };

            $scope.filterChanged();
            refreshRefunds();
            refreshReships();
        }

        $scope.filterItems = function ()
        {
            if ($scope.forms.form.$valid)
            {
                $scope.RefundPaging.PageIndex = 1;
                $scope.ReshipPaging.PageIndex = 1;
                $scope.forms.form.submitted = false;
                refreshRefunds();
                refreshReships();
            }
            else
            {
                $scope.forms.form.submitted = true;
            }
        };

        $scope.filterChanged = function ()
        {
            var data = {};
            angular.copy($scope.filter, data);
            if (data.From)
            {
                data.From = data.From.toServerDateTime();
            }
            if (data.To)
            {
                data.To = data.To.toServerDateTime();
            }
            $scope.options.refundExportUrl = serviceCodeService.getServiceCodeRefundItemsReportFile(data, $rootScope.buildNumber);
            $scope.options.reshipExportUrl = serviceCodeService.getServiceCodeReshipItemsReportFile(data, $rootScope.buildNumber);
        };

        $scope.reassignItems = function ()
        {
            if (!$scope.forms.newCode.$valid)
            {
                $scope.forms.newCode.submitted = true;
                return;
            }
            else
            {
                $scope.forms.newCode.submitted = false;
                var refundIds = [];
                $.each($scope.refunds, function (index, item)
                {
                    if (item.Assign)
                    {
                        refundIds.push(item.Id);
                    }
                });

                var reshipIds = [];
                $.each($scope.reships, function (index, item)
                {
                    if (item.Assign)
                    {
                        reshipIds.push(item.Id);
                    }
                });

                if (refundIds.length > 0)
                {
                    serviceCodeService.assignServiceCodeForRefunds($scope.options.NewServiceCode, refundIds, $scope.refreshTracker)
                        .success(function (result)
                        {
                            if (result.Success)
                            {
                                $scope.RefundPaging.PageIndex = 1;
                                if ($scope.forms.form.$valid)
                                {
                                    refreshRefunds();
                                }
                                else
                                {
                                    $scope.forms.form.submitted = true;
                                }
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

                if (reshipIds.length > 0)
                {
                    serviceCodeService.assignServiceCodeForReships($scope.options.NewServiceCode, reshipIds, $scope.refreshTracker)
                        .success(function (result)
                        {
                            if (result.Success)
                            {
                                $scope.ReshipPaging.PageIndex = 1;
                                if ($scope.forms.form.$valid)
                                {
                                    refreshReships();
                                }
                                else
                                {
                                    $scope.forms.form.submitted = true;
                                }
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
            }
        };

        initialize();
    }]);