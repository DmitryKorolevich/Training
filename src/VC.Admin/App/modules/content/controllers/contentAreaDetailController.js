'use strict';

angular.module('app.modules.content.controllers.contentAreaDetailController', [])
.controller('contentAreaDetailController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentAreaService', 'toaster', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, contentAreaService, toaster, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved");
                $scope.contentArea = result.Data;
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
            $scope.previewUrl = 'http://' + $rootScope.ReferenceData.PublicHost + '/?preview=true';

            var id = $stateParams.id ? $stateParams.id : 0;

	        contentAreaService.getContentArea(id, $scope.refreshTracker)
		        .success(function(result) {
		        	if (result.Success) {
		        		$scope.contentArea = result.Data;
			        } else {
		        		var messages = "";
		        		if (result.Messages) {
		        			$.each(result.Messages, function (index, value) {
		        				messages += value.Message + "<br />";
		        			});
		        		} else {
		        			messages = "Can't get content area";
		        		}

		        		toaster.pop('error', 'Error!', messages, null, 'trustedHtml');
			        }
		        })
		        .error(function(result) {
			        errorHandler(result);
		        });
        };

	    $scope.save = function() {
	    	contentAreaService.saveContentArea($scope.contentArea, $scope.editTracker)
		    .success(function(result) {
			    successSaveHandler(result);
		    }).error(function(result) {
		    	errorHandler(result);
		    });
	    };

	    initialize();
    }]);