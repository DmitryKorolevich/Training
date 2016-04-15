'use strict';

angular.module('app.modules.order.controllers.sendOrderConfirmationController', [])
.controller('sendOrderConfirmationController', ['$scope', '$uibModalInstance', 'data', 'orderService', 'toaster', 'promiseTracker', '$rootScope',
    function ($scope, $uibModalInstance, data, orderService, toaster, promiseTracker, $rootScope)
{
        $scope.refreshTracker = promiseTracker("save");

	function successSaveHandler(result) {
	    if (result.Success) {
	        toaster.pop('success', "Success!", "Successfully sent.");
	        data.thenCallback();
	        $uibModalInstance.close();
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
	    $scope.options.Id = data.Id;

	    $scope.save = function () {
	        $.each($scope.forms.form, function (index, element) {
	        	if (element && element.$name == index) {
	                element.$setValidity("server", true);
	            }
	        });

		    if ($scope.forms.form.$valid) {
		        orderService.sendOrderConfirmationEmail($scope.options.Id, $scope.options, $scope.refreshTracker).success(function (result)
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
			$uibModalInstance.close();
		};
	}
    
	initialize();
}]);