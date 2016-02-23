'use strict';

angular.module('app.modules.content.controllers.contentCrossSellsManageController', [])
	.controller('contentCrossSellsManageController', [
		'$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'toaster', 'promiseTracker',
		function($scope, $rootScope, $state, $stateParams, contentService, toaster, promiseTracker) {
			$scope.refreshTracker = promiseTracker("get");
			$scope.editTracker = promiseTracker("edit");

			function initialize() {
				$scope.id = $state.name == "index.oneCol.manageAddToCartCs" ? 2 : 3;

				$scope.title = $scope.id == 2 ? "Manage Add To Cart Cross Selling" : "Manage View Cart Cross Selling";

				refreshCrossSells();
			};

			function refreshCrossSells() {
				contentService.getContentCrossSells($scope.id, $scope.refreshTracker)
                .success(function (result) {
                	if (result.Success) {
                		$scope.model = result.Data;
                	} else {
                		errorHandler(result);
                	}
                })
                .error(function (result) {
                	errorHandler(result);
                });
			}

			function errorHandler(result) {
				toaster.pop('error', "Error!", "Server error occured");
			};

			initialize();
		}
	]);