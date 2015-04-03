'use strict';

angular.module('app.core.utils.modalUtil', [])
.service('modalUtil', ['$modal', '$log', function ($modal, $log) {
	return {
		open: function(templateUrl, controller,data, successCallback, errorCallback) {
			var modalInstance = $modal.open({
				templateUrl: templateUrl,
				controller: controller,
				size: 'lg',
				resolve: {
					data: function () {
						return data;
					}
				}
			});

			modalInstance.result.then(function (data) {
				$log.info('Modal successfully worked out: ' + new Date());
				if (successCallback) {
					successCallback(data);
				}
			}, function () {
				$log.info('Modal dismissed at: ' + new Date());
				if (errorCallback) {
					errorCallback();
				}
			});
		}
	};
}]);