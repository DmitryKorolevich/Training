'use strict';

angular.module('app.core.utils.infoPopup.infoPopupUtil', [])
.service('infoPopupUtil', ['$uibModal', '$log', function ($uibModal, $log) {
	return {
	    info: function (title, text, optionalOkHandler, preventClose) {
	        if (preventClose === undefined || preventClose === null)
	        {
	            preventClose = false;
	        }
			var modalInstance = $uibModal.open({
				templateUrl: "app/core/utils/infoPopup/info.html",
				controller: "infoPopupController",
				backdrop: preventClose ? 'static' : true,
				keyboard: preventClose ? false : true,
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