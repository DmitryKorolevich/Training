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
		updateRecipe: function(recipeManageModel) {
			return $http.post(baseUrl + 'UpdateRecipe', recipeManageModel);
		},
		deleteRecipe: function (id) {
			return $http.post(baseUrl + 'DeleteRecipe/'+ id);
		},

        //categories
		getCategoriesTree: function(filter) {
			return $http.post(baseUrl + 'GetCategoriesTree', filter);
		}
	};
}]);