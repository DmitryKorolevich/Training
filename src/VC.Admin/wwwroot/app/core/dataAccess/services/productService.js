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
	        return $http.post(baseUrl + 'DeleteCategory/' + id, getConfig(tracker));
	    },

	    //products  
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
	    deleteProduct: function (id, tracker) {
	        return $http.post(baseUrl + 'DeleteProduct/' + id, getConfig(tracker));
	    },
	};
}]);