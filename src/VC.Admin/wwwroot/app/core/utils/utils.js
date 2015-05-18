'use strict';

angular.module('app.utils', [
	'ui.tree',
	'nvd3ChartDirectives',
	'toaster',
	'ui.sortable',
	'ui.ace',
	'ngProgress',
	'angular-ladda',
	'ajoslin.promise-tracker',
    'ngFileUpload',
    'fiestah.money',
	'app.core.utils.appBootstrap',
	'app.core.utils.modalUtil',
	'app.core.utils.commonActionsUtil',
	'app.core.utils.confirmation.confirmController',
	'app.core.utils.confirmation.confirmUtil',
	'app.core.utils.loading.overlayDirective',
	'app.core.utils.parts.basePaginationDirective',
	'app.core.utils.parts.imgLoadDirective',
	'app.core.utils.parts.fileFieldDirective',
	'app.core.utils.textAngular'
	])