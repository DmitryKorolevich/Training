'use strict';

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
	'app.core.utils.textAngular.controllers.imageCustomizationController'
	])
.config(function($provide) {
	$provide.decorator('taOptions', ['taRegisterTool', 'modalUtil', '$delegate', function (taRegisterTool, modalUtil, taOptions) {
		taRegisterTool('image1', {
			iconclass: 'fa fa-picture-o',
			action: function (promise, restoreSelection) {
				var textAngular = this;
				var modalInstance = modalUtil.open('app/core/utils/textAngular/partials/imageCustomization.html', 'imageCustomizationController', null, null, function(file) {
					restoreSelection();
						textAngular.$editor().wrapSelection('insertImage', file.Original.PreviewUrl);
						promise.resolve();
					 });

				//modalInstance.result.then(function (imgUrl) {
				//	rangy.restoreSelection(savedSelection);
				//	textAngular.$editor().wrapSelection('insertImage', imgUrl);
				//});
				return false;
			}
		});

		taOptions.toolbar = [
			['h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'p', 'pre', 'justifyLeft', 'justifyCenter', 'justifyRight', 'indent', 'outdent'],
			['quote', 'bold', 'italics', 'underline', 'strikeThrough', 'ul', 'ol', 'redo', 'undo', 'clear', 'html', 'insertLink', 'insertVideo', 'image1']
		];

		return taOptions;
	}]);
});