﻿'use strict';

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
    'app.core.utils.unsavedChanges.unsavedChangesFacilities',
	'app.core.utils.httpInterceptor',
	'app.core.utils.appBootstrap',
	'app.core.utils.modalUtil',
	'app.core.utils.gridSorterUtil',
	'app.core.utils.commonActionsUtil',
	'app.core.utils.confirmation.confirmController',
	'app.core.utils.confirmation.confirmUtil',
	'app.core.utils.infoPopup.infoPopupController',
	'app.core.utils.infoPopup.infoPopupUtil',
	'app.core.utils.loading.overlayDirective',
	'app.core.utils.parts.basePaginationDirective',
	'app.core.utils.parts.imgLoadDirective',
	'app.core.utils.parts.fileFieldDirective',
	'app.core.utils.parts.numbersOnlyDirective',
	'app.core.utils.parts.maxCharactersDirective',
	'app.core.utils.parts.maskDirective',
	'app.core.utils.parts.starsDirective',
	'app.core.utils.parts.cancelDirective',
	'app.core.utils.textAngular',
	'app.core.utils.webStorageUtil',
	'app.core.utils.dataStateRemediator'
	])