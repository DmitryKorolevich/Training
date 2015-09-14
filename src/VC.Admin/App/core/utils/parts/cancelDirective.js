'use strict';

angular.module('app.core.utils.parts.cancelDirective', [])
.directive('cancelButton', [
	'commonActionsUtil',
	function (commonActionsUtil) {
		return {
			scope: true,
			priority: 4000,
			link: function (scope, element, attrs) {
				element.bind('click', function (event) {
					commonActionsUtil.cancel(attrs.cancelButtonDefaultState);
				});
			}
		};
	}
]);