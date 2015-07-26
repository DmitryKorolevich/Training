 angular.module('app.core.utils.loading.overlayDirective', [])
   .directive('spinnerOverlay', function() {
		 return {
			 restrict: 'E',
			 transclude: true,
			 scope: { show: '=loading' },
			 //link: function(scope, element, attributes) {
			 //   scope.directive
			 //},
			 template: '<div class="relative">' +
					'<div data-ng-show="show" class="overlay">' +
						'<div class="loading">Loading&#8230;</div>' +
					'</div>' +
					'<div ng-transclude></div>' +
				 '</div>'
		 };
	 })