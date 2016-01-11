'use strict';

angular.module('app.core.utils.textAngular', [
	'ngSanitize',
	'textAngular',
	'app.core.utils.textAngular.controllers.imageCustomizationController',
	'app.core.utils.textAngular.services.imageCustomizationService'
])
.config(['$provide', function ($provide) {
	$provide.decorator('taOptions', ['taRegisterTool', '$delegate', 'imageCustomizationService', '$timeout', function (taRegisterTool, taOptions, imageCustomizationService, $timeout) {
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

					var detectActiveElement = function () {
						var toReplace = null;

						if ($element.parent().hasClass('custom-image-link')) {
							toReplace = $element.parent();
						} else {
							toReplace = $element;
						}
						return toReplace;
					};

					var initialData = imageCustomizationService.restoreImage($element);

					imageCustomizationService.customizeImage(initialData, function (markup) {
						var toReplace = detectActiveElement();
						toReplace.replaceWith(markup);

						finishEdit();
						return false;
					}, function () {
						var toReplace = detectActiveElement();
						toReplace.remove();

						finishEdit();
						return false;
					});

					return false;
				}
			}
		});

		taRegisterTool("toggleHtml", {
			iconclass: 'fa fa-code',
			tooltiptext: 'Toggle html / Rich Text',
			action: function () {
				var editor = this.$editor();

				editor.switchView();

				if (editor.showHtml && !editor.displayElements.html[0].innerHTML && !editor.displayElements.html[0].innerText && editor.displayElements.text[0].innerHTML) {
					//$timeout(function () { $(editor.displayElements.html[0]).val(editor.displayElements.text[0].innerHTML) }, 500); // stupid hack
					$(editor.displayElements.html[0]).val(editor.displayElements.text[0].innerHTML);
				}
			},
			activeState: function () {
				return this.$editor().showHtml;
			}
		});

		taOptions.toolbar = [
			['h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'p', 'pre', 'justifyLeft', 'justifyCenter', 'justifyRight', 'indent', 'outdent'],
			['quote', 'bold', 'italics', 'underline', 'strikeThrough', 'ul', 'ol', 'redo', 'undo', 'clear', 'html' /*'toggleHtml'*/, 'insertLink', 'insertVideo', 'image']
		];
		taOptions.keyMappings = [];
		taOptions.disableSanitizer = true;

		return taOptions;
	}]);
}]);