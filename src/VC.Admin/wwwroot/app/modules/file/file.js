'use strict';

angular.module('app.modules.file', [
	'app.modules.file.controllers.filesController',
	'app.modules.file.controllers.addFolderController',
    'app.modules.file.controllers.previewFileController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
				.state('index.oneCol.filesManagement', {
				    url: '/files',
				    templateUrl: 'app/modules/file/partials/filesManagement.html',
				    controller: 'filesController'
				});
		}
]);