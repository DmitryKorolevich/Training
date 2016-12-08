'use strict';

angular.module('app.core.dataAccess.services.discountService', [])
.service('discountService', ['$http', function ($http) {
    var baseUrl = '/Api/Discount/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	function generateQueryParamsBasedOnFilter(filter)
	{
	    var url = '';
	    $.each(filter, function (index, item)
	    {
	        if (index != 'Paging' && index != 'Sorting')
	        {
	            url += '{0}={1}&'.format(index.toLowerCase(), item);
	        }
	        if (index == 'Sorting')
	        {
	            $.each(item, function (sortingIndex, sortingItem)
	            {
	                url += '{0}={1}&'.format(sortingIndex.toLowerCase(), sortingItem);
	            });
	        }
	    });
	    return url;
	}

	return {
	    //discounts  
	    getDiscounts: function (filter, tracker) {
	        return $http.post(baseUrl + 'GetDiscounts', filter, getConfig(tracker));
	    },
	    getDiscountsReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + ('GetDiscountsReportFile?{0}buildNumber={1}')
                .format(generateQueryParamsBasedOnFilter(filter), buildNumber);
	    },
	    getDiscount: function (id, tracker) {
	        return $http.get(baseUrl + 'GetDiscount/' + id, getConfig(tracker));
	    },
	    updateDiscount: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateDiscount', model, getConfig(tracker));
	    },
	    deleteDiscount: function (id, tracker) {
	        return $http.post(baseUrl + 'DeleteDiscount/' + id, null, getConfig(tracker));
	    },
	    getHistoryReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetHistoryReport', filter, getConfig(tracker));
	    },
	};
}]);