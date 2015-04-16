'use strict';

angular.module('app.core.dataAccess.services.contentService', [])
.service('contentService', ['$http', function ($http) {
	var baseUrl = '/Api/Content/';

	function getConfig(tracker) {
	    var config = {};
	    if (tracker) {
	        config.tracker = tracker;
	    }
	    return config;
	};

	return {
        //recipes
		getRecipes: function(filter,tracker) {
		    return $http.post(baseUrl + 'GetRecipes', filter, getConfig(tracker));
		},
		getRecipe: function (id,tracker) {
			return $http.get(baseUrl + 'GetRecipe/' + id, getConfig(tracker));
		},
		updateRecipe: function(model,tracker) {
			return $http.post(baseUrl + 'UpdateRecipe', model, getConfig(tracker));
		},
		deleteRecipe: function (id,tracker) {
			return $http.post(baseUrl + 'DeleteRecipe/'+ id, getConfig(tracker));
		},

	    //faq
		getFAQs: function(filter) {
		    return $http.post(baseUrl + 'GetFAQs', filter);
		},
		getFAQ: function (id) {
		    return $http.get(baseUrl + 'GetFAQ/' + id);
		},
		updateFAQ: function(model) {
		    return $http.post(baseUrl + 'UpdateFAQ', model);
		},
		deleteFAQ: function (id) {
		    return $http.post(baseUrl + 'DeleteFAQ/'+ id);
		},

	    //articles
		getArticles: function(filter) {
		    return $http.post(baseUrl + 'GetArticles', filter);
		},
		getArticle: function (id) {
		    return $http.get(baseUrl + 'GetArticle/' + id);
		},
		updateArticle: function(model) {
		    return $http.post(baseUrl + 'UpdateArticle', model);
		},
		deleteArticle: function (id) {
		    return $http.post(baseUrl + 'DeleteArticle/'+ id);
		},

        //categories
		getCategoriesTree: function(filter,tracker) {
			return $http.post(baseUrl + 'GetCategoriesTree', filter, getConfig(tracker));
		},
		updateCategoriesTree: function(model) {
		    return $http.post(baseUrl + 'UpdateCategoriesTree', model);
		},
		getCategory: function (id) {
		    return $http.get(baseUrl + 'GetCategory/' + id);
		},
		updateCategory: function (model) {
		    return $http.post(baseUrl + 'UpdateCategory', model);
		},
		deleteCategory: function (id) {
		    return $http.post(baseUrl + 'DeleteCategory/'+ id);
		},

        //master templates
        getMasterContentItems: function(filter) {
			return $http.post(baseUrl + 'GetMasterContentItems', filter);
		},
        getMasterContentItem: function (id) {
			return $http.get(baseUrl + 'GetMasterContentItem/' + id);
		},
		updateMasterContentItem: function(model) {
			return $http.post(baseUrl + 'UpdateMasterContentItem', model);
		},
		deleteMasterContentItem: function (id) {
			return $http.post(baseUrl + 'DeleteMasterContentItem/'+ id);
		},
	};
}]);