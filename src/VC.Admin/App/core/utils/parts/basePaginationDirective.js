angular.module('app.core.utils.parts.basePaginationDirective', [])
   .directive('basePagination', function () {
		 return {
			 restrict: 'E',
			 scope: {
			     totalItems: '=',
			     itemsPerPage: '=',
			     pageChanged: '=',
			     ngModel: '=',
			 },
			 template: '<uib-pagination boundary-links="true" total-items="totalItems" items-per-page="itemsPerPage" data-ng-model="ngModel" ' +
                       'previous-text="&lsaquo;" next-text="&rsaquo;" first-text="&laquo;" last-text="&raquo;" data-ng-change="pageChanged"></uib-pagination>'
		 };
	 })