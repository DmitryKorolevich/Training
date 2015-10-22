﻿'use strict';

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
	    getTopPurchasedSkus: function(tracker) {
	        return $http.get(baseUrl+'GetTopPurchasedSkus',getConfig(tracker));
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
	    sendProductOutOfStockRequests: function (ids, tracker)
	    {
	        return $http.post(baseUrl + 'SendProductOutOfStockRequests', ids, getConfig(tracker));
	    },
	};
}]);