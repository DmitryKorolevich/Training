'use strict';

angular.module('app.core.utils.textAngular', [
	'textAngular',
	'app.core.utils.textAngular.controllers.imageCustomizationController',
	'app.core.utils.textAngular.services.imageCustomizationService'
])
.config(function ($provide) {
	$provide.decorator('taOptions', ['taRegisterTool', '$delegate', 'imageCustomizationService', function (taRegisterTool, taOptions, imageCustomizationService) {
		taRegisterTool('image', {
			iconclass: 'fa fa-picture-o',
			action: function (promise, restoreSelection) {
				var textAngular = this;

				imageCustomizationService.customizeImage(textAngular, restoreSelection, promise, null, null);

				return false;
			},
			onElementSelect: {
				element: 'img',
				action: function (event, $element, editorScope) {
					var finishEdit = function () {
						editorScope.updateTaBindtaTextElement();
						editorScope.hidePopover();
					};
					event.preventDefault();

					var textAngular = this;

					var initialData = angular.fromJson($element.data('restore'));

					imageCustomizationService.customizeImage(textAngular, null, null, initialData, $element);

					finishEdit();

					return false;
				}
			}
		});

		taOptions.toolbar = [
			['h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'p', 'pre', 'justifyLeft', 'justifyCenter', 'justifyRight', 'indent', 'outdent'],
			['quote', 'bold', 'italics', 'underline', 'strikeThrough', 'ul', 'ol', 'redo', 'undo', 'clear', 'html', 'insertLink', 'insertVideo', 'image']
		];

		return taOptions;
	}]);
});