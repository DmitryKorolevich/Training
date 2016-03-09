'use strict';

angular.module('app.core.dataAccess.services.inventorySkuService', [])
.service('inventorySkuService', ['$http', function ($http)
{
    var baseUrl = '/Api/InventorySku/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
        //inventorySkus
	    getInventorySkus: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetInventorySkus', filter, getConfig(tracker));
	    },
	    getInventorySku: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetInventorySku/' + id, getConfig(tracker));
	    },
	    updateInventorySku: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateInventorySku', model, getConfig(tracker));
	    },
	    deleteInventorySku: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'DeleteInventorySku/' + id, null, getConfig(tracker));
	    },

	    //inventoryLookups
	    getInventorySkuLookups: function (tracker)
	    {
	        return $http.get(baseUrl + 'GetInventorySkuLookups', getConfig(tracker));
	    },
	    getInventorySkuLookup: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetInventorySkuLookup/{0}'.format(id), getConfig(tracker));
	    },
	    updateInventorySkuLookupVariants: function (id, model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateInventorySkuLookupVariants/{0}'.format(id), model, getConfig(tracker));
	    },

	    //inventoryCategories
	    getInventorySkuCategoriesTree: function (filter, tracker) {
	        return $http.post(baseUrl + 'GetInventorySkuCategoriesTree', filter, getConfig(tracker));
	    },
	    updateInventorySkuCategoriesTree: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateInventorySkuCategoriesTree', model, getConfig(tracker));
	    },
	    getInventorySkuCategory: function (id, tracker) {
	        return $http.get(baseUrl + 'GetInventorySkuCategory/' + id, getConfig(tracker));
	    },
	    updateInventorySkuCategory: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateInventorySkuCategory', model, getConfig(tracker));
	    },
	    deleteInventorySkuCategory: function (id, tracker) {
	        return $http.post(baseUrl + 'DeleteInventorySkuCategory/' + id, null, getConfig(tracker));
	    },
	};
}]);