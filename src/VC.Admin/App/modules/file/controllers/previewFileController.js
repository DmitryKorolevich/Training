'use strict';

angular.module('app.modules.file.controllers.previewFileController', [])
.controller('previewFileController', ['$scope', '$rootScope', '$uibModalInstance', 'data',
    function ($scope, $rootScope, $uibModalInstance, data) {
	
    function initialize() {
        if ($rootScope.ReferenceData.PublicHost) {
            $scope.baseUrl = 'https://' + $rootScope.ReferenceData.PublicHost + '{0}';
        }
        var fileUrl = data.fileUrl;

        if (fileUrl) {
            $scope.previewUrl = $scope.baseUrl.format(fileUrl);
        };
	}

	$scope.cancel = function () {
	    $uibModalInstance.close();
	};

	initialize();
}]);