﻿'use strict';

angular.module('app.modules.file.controllers.addFolderController', [])
.controller('addFolderController', ['$scope', '$uibModalInstance', 'data', 'fileService', 'toaster', 'promiseTracker', '$rootScope',
    function ($scope, $uibModalInstance, data, fileService, toaster, promiseTracker, $rootScope) {
	$scope.saveTracker = promiseTracker("save");

	function successSaveHandler(result) {
	    if (result.Success) {
	        toaster.pop('success', "Success!", "Successfully saved.");

	        data.thenCallback(result.Data);
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

	    $scope.directory =
                    {
                        FullRelativeName: data.fullRelativeName,
                        Name: '',
                    };

	    $scope.save = function () {
	        $.each($scope.forms.form, function (index, element) {
	        	if (element && element.$name == index) {
	                element.$setValidity("server", true);
	            }
	        });

		    if ($scope.forms.form.$valid) {
		        fileService.addDirectory($scope.directory, $scope.saveTracker).success(function (result) {
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