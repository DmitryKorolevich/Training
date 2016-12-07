'use strict';

angular.module('app.modules.setting.controllers.addEditOrderNoteController', [])
.controller('addEditOrderNoteController', ['$scope', '$uibModalInstance', 'data', 'orderNoteService', 'toaster', 'promiseTracker', '$rootScope', function ($scope, $uibModalInstance, data, orderNoteService, toaster, promiseTracker, $rootScope) {
	$scope.saveTracker = promiseTracker("save");

	function successHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved");
			$uibModalInstance.close();
		} else
		{
		    $rootScope.fireServerValidation(result, $scope);
		}
		data.thenCallback();
	};

	function errorHandler(result) {
		toaster.pop('error', "Error!", "Server error occured");
		data.thenCallback();
	};

	function initialize()
	{
	    $scope.forms = {};

		$scope.orderNote = data.orderNote;

	    $scope.editMode = data.editMode;

		$scope.save = function () {
			$.each($scope.orderNoteForm, function(index, element) {
				if (element && element.$name == index) {
					element.$setValidity("server", true);
				}
			});

			if ($scope.orderNoteForm.$valid) {

				$scope.saving = true;
				if ($scope.editMode) {
					orderNoteService.updateOrderNote($scope.orderNote, $scope.saveTracker).success(function (result) {
							successHandler(result);
						}).
						error(function(result) {
							errorHandler(result);
						});
				} else {
					orderNoteService.createOrderNote($scope.orderNote, $scope.saveTracker).success(function (result) {
							successHandler(result);
						}).
						error(function(result) {
							errorHandler(result);
						});
				}

			} else {
				$scope.forms.submitted = true;
			}
		};

		$scope.cancel = function () {
			$uibModalInstance.close();
		};
	}

	$scope.toggleCustomerTypeSelection = function (customerType) {
		if (!$scope.orderNote.CustomerTypes) {
			$scope.orderNote.CustomerTypes = [];
		}

		var idx = $scope.orderNote.CustomerTypes.indexOf(customerType);

		if (idx > -1) {
			$scope.orderNote.CustomerTypes.splice(idx, 1);
		}
		else {
			$scope.orderNote.CustomerTypes.push(customerType);
		}
	};

	initialize();
}]);