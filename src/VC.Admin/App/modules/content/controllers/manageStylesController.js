'use strict';

angular.module('app.modules.content.controllers.manageStylesController', [])
.controller('manageStylesController', ['$scope', '$rootScope', '$state', '$stateParams', 'manageStylesService', 'settingService', 'toaster', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, manageStylesService, settingService, toaster, promiseTracker) {
    	$scope.refreshTracker = promiseTracker("get");
    	$scope.editTracker = promiseTracker("edit");

    	function refreshHistory()
    	{
    	    if ($scope.model && $scope.model.Id)
    	    {
    	        var data = {};
    	        data.service = settingService;
    	        data.tracker = $scope.refreshTracker;
    	        data.idObject = $scope.model.Id;
    	        data.idObjectType = 19//styles
    	        $scope.$broadcast('objectHistorySection#in#refresh', data);
    	    }
    	}

    	function successSaveHandler(result) {
    		if (result.Success) {
    			toaster.pop('success', "Success!", "Successfully saved");
    			$scope.model.css = result.Data.CSS;
    			refreshHistory();
    		} else {
    			var messages = "";
    			if (result.Messages) {
    				$.each(result.Messages, function (index, value) {
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
		    $scope.model = { css: ""};
		    $scope.previewUrl = 'http://' + $rootScope.ReferenceData.PublicHost + '/?preview=true';

    		manageStylesService.getStyles($scope.refreshTracker)
		        .success(function (result) {
		        	if (result.Success) {
		        	    $scope.model.css = result.Data.CSS;
		        	    $scope.model.Id = result.Data.Id;
		        	    refreshHistory();
		        	} else {
		        		var messages = "";
		        		if (result.Messages) {
		        			$.each(result.Messages, function (index, value) {
		        				messages += value.Message + "<br />";
		        			});
		        		} else {
		        			messages = "Can't get custom CSS";
		        		}

		        		toaster.pop('error', 'Error!', messages, null, 'trustedHtml');
		        	}
		        })
		        .error(function (result) {
		        	errorHandler(result);
		        });
    	};

    	$scope.save = function () {
    		manageStylesService.saveStyles($scope.model.css, $scope.editTracker)
		    .success(function (result) {
		    	successSaveHandler(result);
		    }).error(function (result) {
		    	errorHandler(result);
		    });
    	};

    	initialize();
    }]);