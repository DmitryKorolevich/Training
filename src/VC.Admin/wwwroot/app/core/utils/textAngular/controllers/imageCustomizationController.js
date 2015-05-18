﻿'use strict';

angular.module('app.core.utils.textAngular.controllers.imageCustomizationController', [])
.controller('imageCustomizationController', ['$scope', '$modalInstance', 'data', 'toaster', function ($scope, $modalInstance, data, toaster) {

		function intitialize() {
			$scope.alignmentLookup = [
				{ Text: 'Baseline', Value: 'baseline' },
				{ Text: 'Top', Value: 'top' },
				{ Text: 'Middle', Value: 'middle' },
				{ Text: 'Bottom', Value: 'bottom' },
				{ Text: 'Text-top', Value: 'text-top' },
				{ Text: 'Text-bottom', Value: 'text-bottom' },
				{ Text: 'Left', Value: 'left'},
				{ Text: 'Right', Value: 'right'}
			];

			if (data) {
				$scope.image = data;
			} else {
				$scope.image = { Alignment: 'baseline' };
			}
		};

		$scope.selected = function (file) {
			$scope.image.Src = file.PreviewUrl;
			$scope.image.Width = file.Width;
			$scope.image.Height = file.Height;
		};

		$scope.ok = function () {
			if (!$scope.image || !$scope.image.FileUrl) {
				toaster.pop('error', "Error!", 'Please select an image first.', null, 'trustedHtml');
			}
			$modalInstance.close($scope.image);
		};

		$scope.cancel = function() {
			$modalInstance.dismiss();
		};

		intitialize();
	}]);