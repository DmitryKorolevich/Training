'use strict';

angular.module('app.core.utils.gridSorterUtil', [])
.service('gridSorterUtil', [function () {
	return {
	    resolve: function (fetchDataCallback, defaultPath, defaultOrder) {
		    var sorting = {
			    Path: defaultPath,
			    SortOrder: defaultOrder,

				applySort: function(sortPath) {
				    if (sortPath != '') {
					    if (this.Path == sortPath) {
					    	this.SortOrder = changeSortOrder(this.SortOrder);
					    }
					    this.Path = sortPath;
					    if (fetchDataCallback) {
						    fetchDataCallback();
					    }
				    }
			    }
		    };

		    function changeSortOrder(sortOrder) {
		    	if (sortOrder == 'Asc') {
		    		sortOrder = 'Desc';
		    	} else {
		    		sortOrder = 'Asc';
		    	}

		    	return sortOrder;
		    }

		    return sorting;
		}
	};
}]);