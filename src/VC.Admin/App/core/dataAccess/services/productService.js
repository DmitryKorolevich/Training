'use strict';

angular.module('app.core.dataAccess.services.productService', [])
.service('productService', ['$http', function ($http) {
	var baseUrl = '/Api/Product/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
        //categories
	    getCategoriesTree: function (filter, tracker) {
	        return $http.post(baseUrl + 'GetCategoriesTree', filter, getConfig(tracker));
	    },
	    updateCategoriesTree: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateCategoriesTree', model, getConfig(tracker));
	    },
	    getCategory: function (id, tracker) {
	        return $http.get(baseUrl + 'GetCategory/' + id, getConfig(tracker));
	    },
	    updateCategory: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateCategory', model, getConfig(tracker));
	    },
	    deleteCategory: function (id, tracker) {
	        return $http.post(baseUrl + 'DeleteCategory/' + id, null, getConfig(tracker));
	    },
	    getProductCategoriesStatistic: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetProductCategoriesStatistic', filter, getConfig(tracker));
	    },
	    getProductCategoriesStatisticReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + 'GetProductCategoriesStatisticReportFile?from={0}&to={1}&buildNumber={2}'.format(filter.From, filter.To, buildNumber);
	    },
	    getSkusInProductCategoryStatistic: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetSkusInProductCategoryStatistic', filter, getConfig(tracker));
	    },
	    getProductsOnCategoryOrder: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetProductsOnCategoryOrder/' + id, getConfig(tracker));
	    },
	    updateProductsOnCategoryOrder: function (id, model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateProductsOnCategoryOrder/' + id, model, getConfig(tracker));
	    },

	    //inventoryCategories
	    getInventoryCategoriesTree: function (filter, tracker) {
	        return $http.post(baseUrl + 'GetInventoryCategoriesTree', filter, getConfig(tracker));
	    },
	    updateInventoryCategoriesTree: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateInventoryCategoriesTree', model, getConfig(tracker));
	    },
	    getInventoryCategory: function (id, tracker) {
	        return $http.get(baseUrl + 'GetInventoryCategory/' + id, getConfig(tracker));
	    },
	    updateInventoryCategory: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateInventoryCategory', model, getConfig(tracker));
	    },
	    deleteInventoryCategory: function (id, tracker) {
	        return $http.post(baseUrl + 'DeleteInventoryCategory/' + id, null, getConfig(tracker));
	    },

	    //products  
	    getSkus: function (filter, tracker) {
	        return $http.post(baseUrl + 'GetSkus', filter, getConfig(tracker));
	    },
	    getTopPurchasedSkus: function(id, tracker) {
	        return $http.get(baseUrl+'GetTopPurchasedSkus/' + id, getConfig(tracker));
	    },
	    getSku: function(filter,tracker) {
	        return $http.post(baseUrl+'GetSku',filter,getConfig(tracker));
	    },
	    getProducts: function (filter, tracker) {
	        return $http.post(baseUrl + 'GetProducts', filter, getConfig(tracker));
	    },
	    getProductEditSettings: function (tracker) {
	        return $http.get(baseUrl + 'GetProductEditSettings', getConfig(tracker));
	    },
	    getProduct: function (id, tracker) {
	        return $http.get(baseUrl + 'GetProduct/' + id, getConfig(tracker));
	    },
	    updateProduct: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateProduct', model, getConfig(tracker));
	    },
	    updateProductTaxCodes: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateProductTaxCodes', model, getConfig(tracker));
	    },
	    deleteProduct: function (id, tracker) {
	        return $http.post(baseUrl + 'DeleteProduct/' + id, null, getConfig(tracker));
	    },
	    getHistoryReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetHistoryReport', filter, getConfig(tracker));
	    },
	    getSkusPrices: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetSkusPrices', filter, getConfig(tracker));
	    },
	    updateSkusPrices: function (items, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateSkusPrices', items, getConfig(tracker));
	    },

	    //product reviews
	    getProductsWithReviews: function (filter, tracker) {
	        return $http.post(baseUrl + 'GetProductsWithReviews', filter, getConfig(tracker));
	    },
	    getProductReviews: function (filter, tracker) {
	        return $http.post(baseUrl + 'GetProductReviews', filter, getConfig(tracker));
	    },
	    getProductReview: function (id, tracker) {
	        return $http.get(baseUrl + 'GetProductReview/' + id, getConfig(tracker));
	    },
	    updateProductReview: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateProductReview', model, getConfig(tracker));
	    },
	    deleteProductReview: function (id, tracker) {
	        return $http.post(baseUrl + 'DeleteProductReview/' + id, null, getConfig(tracker));
	    },

	    //product out of stock requests
	    getProductOutOfStockContainers: function (tracker)
	    {
	        return $http.post(baseUrl + 'GetProductOutOfStockContainers', null, getConfig(tracker));
	    },
	    getProductOutOfStockRequestsMessageFormat: function (tracker)
	    {
	        return $http.get(baseUrl + 'GetProductOutOfStockRequestsMessageFormat', getConfig(tracker));
	    },
	    sendProductOutOfStockRequests: function (ids, tracker)
	    {
	        return $http.post(baseUrl + 'SendProductOutOfStockRequests', ids, getConfig(tracker));
	    },
	    deleteProductOutOfStockRequests: function (ids, tracker)
	    {
	        return $http.post(baseUrl + 'DeleteProductOutOfStockRequests', ids, getConfig(tracker));
	    },

	    //reports
	    getSkuBreakDownReportItems: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetSkuBreakDownReportItems', filter, getConfig(tracker));
	    },
	    getSkuBreakDownReportItemsReportFile: function (filter, buildNumber)
	    {
	        return baseUrl + ('GetSkuBreakDownReportItemsReportFile?from={0}&to={1}&buildNumber={2}')
                .format(filter.From, filter.To, buildNumber);
	    },
	    getSkuPOrderTypeBreakDownReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetSkuPOrderTypeBreakDownReport', filter, getConfig(tracker));
	    },
	    getSkuPOrderTypeFutureBreakDownReport: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetSkuPOrderTypeFutureBreakDownReport', filter, getConfig(tracker));
	    },
	};
}]);