﻿'use strict';

angular.module('app.core.utils.confirmation.confirmUtil', [])
.service('confirmUtil', ['$uibModal', '$log', function ($uibModal, $log) {
	return {
	    confirm: function (okHandler, optionalConfirmText, optionalCancelHandler, optionalOkLabel, optionalCancelLabel)
	    {
			var modalInstance = $uibModal.open({
				templateUrl: "app/core/utils/confirmation/confirmation.html",
				controller: "confirmController",
				animation: true,
				resolve: {
					data: function () {
						var data = {
							okHandler: okHandler,
							cancelHandler: optionalCancelHandler,
							confirmText: optionalConfirmText,
							okLabel: optionalOkLabel,
							cancelLabel: optionalCancelLabel,
						};
						return data;
					}
				}
			});

			modalInstance.result.then(function (data) {
				$log.info('Confirm successfully worked out: ' + new Date());
			}, function () {
				$log.info('Confirm dismissed at: ' + new Date());
			});
		}
	};
}]);