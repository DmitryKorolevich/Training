angular.module('app.modules.content.controllers.contentAreasController', [])
.controller('contentAreasController', ['$scope', 'promiseTracker', 'contentAreaService', 'toaster', '$rootScope', '$state', function ($scope, promiseTracker, contentAreaService, toaster, $rootScope, $state) {
	$scope.refreshTracker = promiseTracker("refresh");

	function initialize() {
		refreshContentAreas();

		$scope.open = function(id) {
			$state.go('index.oneCol.manageContentAreaDetail', { id: id });
		};
	};

	function refreshContentAreas() {
		contentAreaService.getContentAreas($scope.refreshTracker)
			.success(function (result) {
				if (result.Success) {
					$scope.contentAreas = result.Data;

				} else {
					toaster.pop('error', 'Error!', "Can't get access to the content areas");
				}
			})
			.error(function (result) {
				toaster.pop('error', "Error!", "Server error ocurred");
			});
	};

	initialize();
}]);