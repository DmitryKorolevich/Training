'use strict';

angular.module('app.dataAccess', [
	'app.core.dataAccess.services.infrastructureService',
	'app.core.dataAccess.services.userService',
	'app.core.dataAccess.services.contentService',
	'app.core.dataAccess.services.settingService',
	'app.core.dataAccess.services.authenticationService',
	'app.core.dataAccess.services.productService',
	'app.core.dataAccess.services.fileService',
]);