'use strict';

angular.module('app.core.utils.textAngular.controllers.imageCustomizationController', [])
.controller('imageCustomizationController', ['$scope', '$uibModalInstance', 'data', 'toaster', function ($scope, $uibModalInstance, data, toaster) {

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

			if (data.Image) {
				$scope.image = data.Image;
				$scope.availableForRemoval = true;
			} else {
				$scope.image = { Alignment: 'baseline' };
			}
		};

		$scope.selected = function (file) {
		    $scope.image.Src = file.FullRelativeName;
			$scope.image.Width = file.Width;
			$scope.image.Height = file.Height;
		};

		$scope.ok = function () {
			if (!$scope.image || !$scope.image.FileUrl) {
				toaster.pop('error', "Error!", 'Please select an image first.', null, 'trustedHtml');
			} else {
				$uibModalInstance.close($scope.image);
			}
		};

		$scope.remove = function() {
			$uibModalInstance.dismiss();

			if (data.Remove) {
				data.Remove();
			}
		};

		$scope.cancel = function() {
			$uibModalInstance.dismiss();

			if (data.Cancel) {
				data.Cancel();
			}
		};

		intitialize();
	}]);