﻿'use strict';

angular.module('app.utils', [
	'textAngular',
	'ui.tree',
	'nvd3ChartDirectives',
	'toaster',
	'ui.sortable',
	'ui.ace',
	'ngProgress',
	'angular-ladda',
	'ajoslin.promise-tracker',
    'ngFileUpload',
	'app.core.utils.appBootstrap',
	'app.core.utils.modalUtil',
	'app.core.utils.commonActionsUtil',
	'app.core.utils.confirmation.confirmController',
	'app.core.utils.confirmation.confirmUtil',
	'app.core.utils.loading.overlayDirective',
	'app.core.utils.parts.basePaginationDirective',
	'app.core.utils.parts.imgLoadDirective',
	'app.core.utils.parts.fileFieldDirective',
	])
.config(function($provide) {
	$provide.decorator('taOptions', ['$delegate', function (taOptions) {
		taOptions.toolbar = [
			['h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'p', 'pre', 'justifyLeft', 'justifyCenter', 'justifyRight', 'indent', 'outdent'],
			['quote', 'bold', 'italics', 'underline', 'strikeThrough', 'ul', 'ol', 'redo', 'undo', 'clear', 'html', 'insertImage', 'insertLink', 'insertVideo']
		];
		return taOptions;
	}]);
});