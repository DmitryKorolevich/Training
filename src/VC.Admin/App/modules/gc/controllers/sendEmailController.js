'use strict';

angular.module('app.modules.gc.controllers.sendEmailController', [])
.controller('sendEmailController', ['$scope', '$modalInstance', 'data', 'gcService', 'toaster', 'promiseTracker', '$rootScope',
    function ($scope, $modalInstance, data, gcService, toaster, promiseTracker, $rootScope) {
	$scope.saveTracker = promiseTracker("save");

	function successSaveHandler(result) {
	    if (result.Success) {
	        toaster.pop('success', "Success!", "Successfully saved.");

	        if (data.thenCallback) {
	            data.thenCallback(result.Data);
	        }
	        $modalInstance.close();
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

	    $scope.save = function () {
	        $.each($scope.forms.form, function (index, element) {
	            if (element.$name == index) {
	                element.$setValidity("server", true);
	            }
	        });

		    if ($scope.forms.form.$valid) {
		        gcService.senfGiftCertificateEmail($scope.email, $scope.saveTracker).success(function (result) {
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
			$modalInstance.close();
		};
	}
    
	initialize();
}]);