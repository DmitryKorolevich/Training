'use strict';

angular.module('app.core.utils.textAngular.services.imageCustomizationService', [])
	.service('imageCustomizationService', [
		'modalUtil', function(modalUtil) {

			return {
				customizeImage: function(initialData, finishSelection, removeCallback) {
					var savedSelection = rangy.saveSelection();

					var modalInstance = modalUtil.open('app/core/utils/textAngular/partials/imageCustomization.html', 'imageCustomizationController', {
						Image: initialData,
						Remove: function() {
							rangy.restoreSelection(savedSelection);

							removeCallback();
						},
						Cancel: function() {
							rangy.restoreSelection(savedSelection);
						}
					}, null, function(image) {

						image.Title = image.Title ? image.Title : "";
						image.Src = image.Src ? image.Src : "";
						image.Alignment = image.Alignment ? image.Alignment : "baseline";

						var markup = "<img data-restore='" + image.FileUrl + "' alt='" + image.Title
							+ "' src='" + image.Src
							+ "' style='border:none; margin: " + (image.Top ? (image.Top + "px") : "auto")
							+ " " + (image.Right ? (image.Right + "px") : "auto")
							+ " " + (image.Bottom ? (image.Bottom + "px") : "auto")
							+ " " + (image.Left ? (image.Left + "px") : "auto")
							+ (image.KeepRatio ? "; max-height: " : "; height: ") + (image.Height ? (image.Height + "px") : "auto")
							+ (image.KeepRatio ? "; max-width: " : "; width: ") + (image.Width ? (image.Width + "px") : "auto")
							+ "; vertical-align: " + (!(image.Alignment == 'left' || image.Alignment == 'right') ? image.Alignment : 'baseline')
							+ "; float:" + ((image.Alignment == 'left' || image.Alignment == 'right') ? image.Alignment : 'none') + ";' />";

						if (image.Hyperlink) {
							markup = "<a class='custom-image-link' target='" + (image.NewWindow ? "_blank" : "_self") + "'  href='" + image.Hyperlink + "'>" + markup + "</a>"
						}

						rangy.restoreSelection(savedSelection);

						if (finishSelection) {
							finishSelection(markup);
						}
					});
				},
				restoreImage(element) {
					var image = {};

					var width = parseInt(element.css('width'));
					var height = parseInt(element.css('height'));

					var maxWidth = parseInt(element.css('max-width'));
					var maxHeight = parseInt(element.css('max-height'));

					var right = parseInt(element.css('margin-right'));
					var left = parseInt(element.css('margin-left'));
					var top = parseInt(element.css('margin-top'));
					var bottom = parseInt(element.css('margin-bottom'));

					var verticalAlign = element.css('vertical-align');
					var float = element.css('float');

					var alt = element.attr('alt');
					var src = element.attr('src');
					var fileUrl = element.data("restore");

					var target = "";
					var href = "";
					var a = element.parent();
					if (a.hasClass('custom-image-link')) {
						href = a.attr('href');
						target = a.attr('target');

						image.Hyperlink = href;
						image.NewWindow = target == "_blank" ? true : false;
					}

					if (width) {
						image.Width = width;
					}
					if (height) {
						image.Height = height;
					}

					if (maxWidth && maxHeight) {
						image.KeepRatio = true;
						if (maxWidth) {
							image.Width = maxWidth;
						}
						if (maxHeight) {
							image.Height = maxHeight;
						}
					} else {
						image.KeepRatio = false;
					}

					if (right) {
						image.Right = right;
					}
					if (left) {
						image.Left = left;
					}
					if (top) {
						image.Top = top;
					}
					if (bottom) {
						image.Bottom = bottom;
					}

					if (float == 'none') {
						image.Alignment = verticalAlign;
					} else {
						image.Alignment = float;
					}

					image.Title = alt;
					image.Src = src;
					image.FileUrl = fileUrl;

					return image;
				}
			};
		}
	]);