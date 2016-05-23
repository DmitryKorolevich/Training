'use strict';

angular.module('app.modules.content.controllers.contentAreaDetailController', [])
.controller('contentAreaDetailController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentAreaService', 'settingService', 'toaster', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, contentAreaService, settingService, toaster, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        function refreshHistory()
        {
            if ($scope.contentArea && $scope.contentArea.Id)
            {
                var data = {};
                data.service = settingService;
                data.tracker = $scope.refreshTracker;
                data.idObject = $scope.contentArea.Id;
                data.idObjectType = 15//content area
                $scope.$broadcast('objectHistorySection#in#refresh', data);
            }
        }

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved");
                $scope.contentArea = result.Data;
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
            $scope.previewUrl = 'http://' + $rootScope.ReferenceData.PublicHost + '/?preview=true';

            var id = $stateParams.id ? $stateParams.id : 0;

	        contentAreaService.getContentArea(id, $scope.refreshTracker)
		        .success(function(result) {
		        	if (result.Success) {
		        	    $scope.contentArea = result.Data;
		        	    refreshHistory();
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