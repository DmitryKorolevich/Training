'use strict';

angular.module('app.core.utils.confirmation.confirmController', [])
.controller('confirmController', ['$scope', '$uibModalInstance', 'data', function ($scope, $uibModalInstance, data) {
	function initialize() {
		var confirmText = "Are you sure?";
		if (data.confirmText != null) {
			confirmText = data.confirmText;
		}
		$scope.confirmText = confirmText;
		var okLabel = "OK";
		if (data.okLabel != null)
		{
		    okLabel = data.okLabel;
		}
		$scope.okLabel = okLabel;
		var cancelLabel = "Cancel";
		if (data.cancelLabel != null)
		{
		    cancelLabel = data.cancelLabel;
		}
		$scope.cancelLabel = cancelLabel;
		$scope.okHandler = data.okHandler;
		$scope.cancelHandler = data.cancelHandler;
	};

	$scope.continue = function ()
	{
	    if ($scope.okHandler)
	    {
	        $scope.okHandler();
	    }
	    $uibModalInstance.close(false);
	};

	$scope.close = function () {
		if ($scope.cancelHandler) {
			$scope.cancelHandler();
		}
		$uibModalInstance.dismiss(false);
	};

	initialize();
}]);