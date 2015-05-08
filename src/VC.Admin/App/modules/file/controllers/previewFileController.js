'use strict';

angular.module('app.modules.file.controllers.previewFileController', [])
.controller('previewFileController', ['$scope', '$rootScope', '$modalInstance', 'data', 'filesConfig',
    function ($scope, $rootScope, $modalInstance, data, filesConfig) {
	
    function initialize() {
        $scope.baseUrl = $rootScope.ReferenceData.PublicHost + filesConfig.urlPrefix + '{0}';
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