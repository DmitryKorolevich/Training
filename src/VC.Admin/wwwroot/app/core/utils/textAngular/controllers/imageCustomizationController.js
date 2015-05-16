'use strict';

angular.module('app.core.utils.textAngular.controllers.imageCustomizationController', [])
.controller('imageCustomizationController', ['$scope', '$modalInstance', function ($scope, $modalInstance) {

		function intitialize() {
			$scope.image = {};

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
		};

		$scope.selected = function (file) {
			$scope.image.Original = file;
			$scope.image.Src = file.PreviewUrl;
			$scope.image.Width = file.Width;
			$scope.image.Height = file.Height;
		};

		$scope.ok = function() {
			$modalInstance.close($scope.image);
		};

		$scope.cancel = function() {
			$modalInstance.dismiss();
		};

		intitialize();
	}]);