'use strict';

angular.module('app.modules.gc.controllers.sendEmailController', [])
.controller('sendEmailController', ['$scope', '$uibModalInstance', 'data', 'gcService', 'toaster', 'promiseTracker', '$rootScope',
    function ($scope, $uibModalInstance, data, gcService, toaster, promiseTracker, $rootScope) {
	$scope.saveTracker = promiseTracker("save");

	function successSaveHandler(result) {
	    if (result.Success) {
	        toaster.pop('success', "Success!", "Successfully sent.");

	        if (data.thenCallback) {
	            data.thenCallback(result.Data);
	        }
	        $uibModalInstance.close();
	    } else {
	        var messages = "";
	        if (result.Messages) {
	            $scope.forms.form.submitted = true;
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

	    $scope.email = data;

	    $.each($scope.email.Gifts, function (index, code)
	    {
	        var balanceData = code.Amount;
	        if (code.Amount.toFixed)
	        {
	            balanceData = balanceData.toFixed(2);
	        }
	    });
        
	    $scope.save = function () {
	        $.each($scope.forms.form, function (index, element) {
	        	if (element && element.$name == index) {
	                element.$setValidity("server", true);
	            }
	        });

		    if ($scope.forms.form.$valid) {
		        gcService.sendGiftCertificateEmail($scope.email, $scope.saveTracker).success(function (result)
		        {
		            successSaveHandler(result);
		        }).
                error(function (result) {
                    errorHandler(result);
                });
		    } else {
		        $scope.forms.form.submitted = true;
			}
		};

		$scope.cancel = function () {
			$uibModalInstance.close();
		};
	}
    
	initialize();
}]);