'use strict';

angular.module('app.core.utils.textAngular.services.imageCustomizationService', [])
.service('imageCustomizationService', ['modalUtil', function (modalUtil) {
	
	return {
		customizeImage: function (initialData, finishSelection, removeCallback) {
			var savedSelection = rangy.saveSelection();

			var modalInstance = modalUtil.open('app/core/utils/textAngular/partials/imageCustomization.html', 'imageCustomizationController', { Image: initialData, Remove: removeCallback }, null, function (image) {

				image.Title = image.Title ? image.Title : "";
				image.Src = image.Src ? image.Src : "";
				image.Alignment = image.Alignment ? image.Alignment : "baseline";

				var markup = "<img data-restore='" + angular.toJson(image) + "' alt='" + image.Title
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
		}
	};
}]);