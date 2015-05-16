'use strict';

angular.module('app.core.utils.modalUtil', [])
.service('modalUtil', ['$modal', '$log', function ($modal, $log) {
	return {
	    open: function (templateUrl, controller, data, options, successCallback, errorCallback) {
	        var options = $.extend(
                {
                    size: 'sm'
                }, options);

			var modalInstance = $modal.open({
				templateUrl: templateUrl,
				controller: controller,
				size: options.size,
				resolve: {
					data: function () {
						return data;
					}
				},
                controllerAs: 'modal'
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

            return modalInstance;
		}
	};
}]);