'use strict';

angular.module('app.modules.file.controllers.previewFileController', [])
.controller('previewFileController', ['$scope', '$rootScope', '$modalInstance', 'data',
    function ($scope, $rootScope, $modalInstance, data) {
	
    function initialize() {
        if ($rootScope.ReferenceData.PublicHost) {
            $scope.baseUrl = $rootScope.ReferenceData.PublicHost.substring(0, $rootScope.ReferenceData.PublicHost.length - 1) + '{0}';
        }
        var fileUrl = data.fileUrl;

        if (fileUrl) {
            $scope.previewUrl = $scope.baseUrl.format(fileUrl);
        };
	}

	$scope.cancel = function () {
	    $modalInstance.close();
	};

	initialize();
}]);