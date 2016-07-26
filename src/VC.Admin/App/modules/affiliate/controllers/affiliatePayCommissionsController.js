'use strict';

angular.module('app.modules.affiliate.controllers.affiliatePayCommissionsController', [])
.controller('affiliatePayCommissionsController', ['$scope', '$uibModalInstance', 'data', 'affiliateService', 'toaster', 'promiseTracker', '$rootScope', 'confirmUtil',
    function ($scope, $uibModalInstance, data, affiliateService, toaster, promiseTracker, $rootScope, confirmUtil)
{
    $scope.refreshTracker = promiseTracker("refresh");

	function successHandler(result) {
		if (result.Success) {
		    toaster.pop('success', "Success!", "Successfully paid");
		    $uibModalInstance.close();
		    data.thenCallback();
		} else {
			var messages = "";
			if (result.Messages) {
				$.each(result.Messages, function(index, value) {
					messages += value.Message + "<br />";
				});
			}
			toaster.pop('error', "Error!", messages, null, 'trustedHtml');
		}
	};

	function errorHandler(result) {
		toaster.pop('error', "Error!", "Server error occured");
	};

	function initialize() {
	    $scope.options={};
	    $scope.options.Id = data.id;

	    loadAffiliate();
	    loadUnPaidCommissions();
	};

	var loadAffiliate = function()
	{
	    affiliateService.getAffiliate($scope.options.Id, $scope.refreshTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.affiliate = result.Data;
                } else
                {
                    errorHandler(result);
                }
            }).
            error(function (result)
            {
                errorHandler(result);
            });
	};

	var loadUnPaidCommissions = function ()
	{
	    affiliateService.getUnpaidOrdersForLastPeriod($scope.options.Id, $scope.refreshTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.commissions = result.Data;
                    $scope.options.Amount=0;
                    $.each($scope.commissions, function (index, value)
                    {
                        $scope.options.Amount+=value.Commission;
                    });
                } else
                {
                    errorHandler(result);
                }
            }).
            error(function (result)
            {
                errorHandler(result);
            });
	};

	$scope.pay = function()
	{
	    confirmUtil.confirm(function () {
	        affiliateService.payForAffiliateOrders($scope.options.Id, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        successHandler(result);
                    } else {
                        errorHandler(result);
                    }
                })
                .error(function (result) {
                    errorHandler(result);
                });
	    }, 'Are you sure you want to pay to this affiliate({0})?'.format($scope.options.Id));
	};

	$scope.cancel = function ()
	{
	    $uibModalInstance.close();
	};

	initialize();
}]);