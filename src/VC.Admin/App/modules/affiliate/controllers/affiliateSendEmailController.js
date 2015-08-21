'use strict';

angular.module('app.modules.affiliate.controllers.affiliateSendEmailController', [])
.controller('affiliateSendEmailController', ['$scope', '$modalInstance', 'data', 'affiliateService', 'toaster', 'promiseTracker', '$rootScope',
    function ($scope, $modalInstance, data, affiliateService , toaster, promiseTracker, $rootScope) {
	$scope.saveTracker = promiseTracker("save");

	function successSaveHandler(result) {
	    if (result.Success) {
	        toaster.pop('success', "Success!", "Successfully sent.");

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

	    $scope.data = data;

	    affiliateService.getAffiliateEmail($scope.data.Type, $scope.saveTracker).success(function (result)
	    {
	        if (result.Data)
	        {
	            $scope.email = result.Data;
	            $scope.email.ToName = $scope.data.ToName;
	            $scope.email.ToEmail = $scope.data.ToEmail;
	            $scope.email.Message = $scope.email.Message.format($scope.data.ToName, $scope.data.ToEmail);
	        }
	        else
	        {
	            errorHandler(result);
	        }
	    }).
        error(function (result)
        {
            errorHandler(result);
        });
	}

	$scope.save = function ()
	{
	    $.each($scope.forms.form, function (index, element)
	    {
	        if (element && element.$name == index)
	        {
	            element.$setValidity("server", true);
	        }
	    });

	    if ($scope.forms.form.$valid)
	    {
	        affiliateService.sendAffiliateEmail($scope.email, $scope.saveTracker).success(function (result)
	        {
	            successSaveHandler(result);
	        }).
            error(function (result)
            {
                errorHandler(result);
            });
	    } else
	    {
	        $scope.forms.form.submitted = true;
	    }
	};

	$scope.cancel = function ()
	{
	    $modalInstance.close();
	};
    
	initialize();
}]);