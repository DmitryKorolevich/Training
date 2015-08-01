'use strict';

angular.module('app.core.utils.infoPopup.infoPopupUtil', [])
.service('infoPopupUtil', ['$modal', '$log', function ($modal, $log) {
	return {
		info: function (title, text, optionalOkHandler) {
			var modalInstance = $modal.open({
				templateUrl: "app/core/utils/infoPopup/info.html",
				controller: "infoPopupController",
				resolve: {
					data: function () {
						var data = {
							okHandler: optionalOkHandler,
							text: text,
							title: title,
						};
						return data;
					}
				}
			});

			modalInstance.result.then(function (data) {
				$log.info('Info successfully worked out: ' + new Date());
			}, function () {
				$log.info('Info dismissed at: ' + new Date());
			});
		}
	};
}]);