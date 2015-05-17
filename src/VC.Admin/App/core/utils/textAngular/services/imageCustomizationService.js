'use strict';

angular.module('app.core.utils.textAngular.services.imageCustomizationService', [])
.service('imageCustomizationService', ['modalUtil', function (modalUtil) {
	
	return {
		customizeImage: function (textAngular, restoreSelection, promise, initialData, element) {
			var modalInstance = modalUtil.open('app/core/utils/textAngular/partials/imageCustomization.html', 'imageCustomizationController', initialData, null, function (image) {

				image.Title = image.Title ? image.Title : "";
				image.Src = image.Src ? image.Src : "";
				image.Alignment = image.Alignment ? image.Alignment : "baseline";

				var markup = "<img data-restore=" + JSON.stringify(image) + " alt='" + image.Title
							+ "' src='" + image.Src
							+ "' style='margin-left:" + (image.Left ? (image.Left + "px") : "auto")
							+ "; margin-right: " + (image.Right ? (image.Right + "px") : "auto")
							+ "; margin-top: " + (image.Top ? (image.Top + "px") : "auto")
							+ "; margin-bottom: " + (image.Bottom ? (image.Bottom + "px") : "auto")
							+ "; height: " + (image.Height ? (image.Height + "px") : "auto")
							+ "; width: " + (image.Width ? (image.Width + "px") : "auto")
							+ "; vertical-align: " + (!(image.Alignment == 'left' || image.Alignment == 'right') ? image.Alignment : 'baseline')
							+ "; float:" + ((image.Alignment == 'left' || image.Alignment == 'right') ? image.Alignment : 'none') + ";' />";

				if (image.Hyperlink) {
					markup = "<a href='" + image.Hyperlink + "'>" + markup + "</a>"
				}

				if (restoreSelection) {
					restoreSelection();
				}

				if (element) {
					element.replaceWith(markup);
				} else {
					textAngular.$editor().wrapSelection('insertHTML', markup, true);
				}

				if (promise) {
					promise.resolve();
				}
			});
		}
	};
}]);