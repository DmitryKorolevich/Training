﻿'use strict';

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

				imageCustomizationService.customizeImage(null, function(markup) {
					textAngular.$editor().wrapSelection('insertHTML', markup, true);
					promise.resolve();
					return false;
				});

				return false;
			},
			onElementSelect: {
				element: 'img',
				action: function (event, $element, editorScope) {
					var finishEdit = function () {
						editorScope.updateTaBindtaTextElement();
					};
					event.preventDefault();

					var textAngular = this;

					var initialData = angular.fromJson($element.data('restore'));

					imageCustomizationService.customizeImage(initialData, function(markup) {
						$element.replaceWith(markup);
						finishEdit();
						return false;
					});

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