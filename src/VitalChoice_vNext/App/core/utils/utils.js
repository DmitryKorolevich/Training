'use strict';

angular.module('app.utils', ['textAngular', 'ya.treeview', 'ya.treeview.tpls' ])
.config(function($provide) {
	$provide.decorator('taOptions', ['$delegate', function (taOptions) {
		taOptions.toolbar = [
			['h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'p', 'pre', 'justifyLeft', 'justifyCenter', 'justifyRight', 'indent', 'outdent'],
			['quote', 'bold', 'italics', 'underline', 'strikeThrough', 'ul', 'ol', 'redo', 'undo', 'clear', 'html', 'insertImage', 'insertLink', 'insertVideo']
		];
		return taOptions;
	}]);
});