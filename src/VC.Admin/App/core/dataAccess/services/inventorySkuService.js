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
	    getShortInventorySku: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetShortInventorySku', filter, getConfig(tracker));
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

	    //reports
	    getInventorySkuUsageReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetInventorySkuUsageReport', filter, getConfig(tracker));
	    },
	    getInventorySkuUsageReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + 'GetInventorySkuUsageReportFile?from={0}&to={1}&skuids={2}&invskuids={3}&buildNumber={4}'
                .format(filter.From, filter.To, filter.sSkuIds, filter.sInvSkuIds, buildNumber);
	    },
	    getInventoriesSummaryUsageReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetInventoriesSummaryUsageReport', filter, getConfig(tracker));
	    },
	    getInventoriesSummaryUsageReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + 'GetInventoriesSummaryUsageReportFile?from={0}&to={1}&sku={2}&invsku={3}&assemble={4}&idsinvcat={5}&frequency={6}&infotype={7}&buildNumber={8}'
                .format(filter.From, filter.To, filter.Sku, filter.InvSku, filter.Assemble, filter.IdsInvCat, filter.FrequencyType, filter.InfoType, buildNumber);
	    },
	};
}]);