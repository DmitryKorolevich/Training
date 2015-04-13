'use strict';

angular.module('app.core.dataAccess.services.contentService', [])
.service('contentService', ['$http', function ($http) {
	var baseUrl = '/Api/Content/';

	return {
        //recipes
		getRecipes: function(filter) {
			return $http.post(baseUrl + 'GetRecipes', filter);
		},
		getRecipe: function (id) {
			return $http.get(baseUrl + 'GetRecipe/' + id);
		},
		updateRecipe: function(model) {
			return $http.post(baseUrl + 'UpdateRecipe', model);
		},
		deleteRecipe: function (id) {
			return $http.post(baseUrl + 'DeleteRecipe/'+ id);
		},

        //categories
		getCategoriesTree: function(filter) {
			return $http.post(baseUrl + 'GetCategoriesTree', filter);
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