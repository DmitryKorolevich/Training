'use strict';

angular.module('app.core.utils.modalUtil', [])
.service('modalUtil', ['$uibModal', '$log', function ($uibModal, $log) {
	return {
	    open: function (templateUrl, controller, data, options, successCallback, errorCallback) {
	        var options = $.extend(
                {
                    size: 'sm',
                    backdrop: true
                }, options);

			var modalInstance = $uibModal.open({
				templateUrl: templateUrl,
				controller: controller,
				size: options.size,
				backdrop: options.backdrop,
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