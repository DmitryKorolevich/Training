'use strict';

angular.module('app.modules.order.controllers.sendOrderShippingConfirmationController', [])
.controller('sendOrderShippingConfirmationController', ['$scope', '$modalInstance', 'data', 'orderService', 'toaster', 'promiseTracker', '$rootScope',
    function ($scope, $modalInstance, data, orderService, toaster, promiseTracker, $rootScope)
{
        $scope.refreshTracker = promiseTracker("save");

	function successSaveHandler(result) {
	    if (result.Success) {
	        toaster.pop('success', "Success!", "Successfully sent.");
	        data.thenCallback();
	        $modalInstance.close();
	    } else {
	        var messages = "";
	        if (result.Messages) {
	            $scope.forms.submitted = true;
	            $scope.serverMessages = new ServerMessages(result.Messages);
	            $.each(result.Messages, function (index, value) {
	                if (value.Field) {
	                    $scope.forms.form[value.Field].$setValidity("server", false);
	                }
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

	    $scope.forms = {};

	    $scope.options = {};
	    $scope.options.Email = data.Email;
	    $scope.options.OrderStatus = data.OrderStatus;
	    $scope.options.POrderStatus = data.POrderStatus;
	    $scope.options.NPOrderStatus = data.NPOrderStatus;

	    $scope.options.SendAll = $scope.options.OrderStatus != null;
	    $scope.options.SendP = $scope.options.OrderStatus == null && $scope.options.POrderStatus==3;//shipped
	    $scope.options.SendNP = $scope.options.OrderStatus == null && $scope.options.NPOrderStatus == 3;//shipped

	    $scope.options.Id = data.Id;

	    $scope.save = function () {
	        $.each($scope.forms.form, function (index, element) {
	        	if (element && element.$name == index) {
	                element.$setValidity("server", true);
	            }
	        });

		    if ($scope.forms.form.$valid) {
		        orderService.sendOrderShippingConfirmationEmail($scope.options.Id, $scope.options, $scope.refreshTracker).success(function (result)
		        {
		            successSaveHandler(result);
		        }).
                error(function (result) {
                    errorHandler(result);
                });
		    } else {
		        $scope.forms.submitted = true;
			}
		};

		$scope.cancel = function () {
			$modalInstance.close();
		};
	}
    
	initialize();
}]);