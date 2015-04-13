'use strict';

angular.module('app.modules.content', [
	'app.modules.content.controllers.recipesController',
	'app.modules.content.controllers.manageRecipeController',
	'app.modules.content.controllers.mastersController',
	'app.modules.content.controllers.manageMasterController',
	'app.modules.content.controllers.recipesCategoryController',
	'app.modules.content.controllers.manageRecipeCategoryController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

			$stateProvider
				.state('index.oneCol.manageRecipes', {
					url: '/content/recipes',
					templateUrl: 'app/modules/content/partials/recipesList.html',
					controller: 'recipesController'
				})
				.state('index.oneCol.addNewRecipe', {
					url: '/content/recipes/add',
					templateUrl: 'app/modules/content/partials/recipeDetail.html',
					controller: 'manageRecipeController'
				})
				.state('index.oneCol.recipeDetail', {
					url: '/content/recipes/{id:int}',
					templateUrl: 'app/modules/content/partials/recipeDetail.html',
					controller: 'manageRecipeController'
				})
				.state('index.oneCol.manageMasters', {
				    url: '/content/masters',
				    templateUrl: 'app/modules/content/partials/mastersList.html',
				    controller: 'mastersController'
				})
				.state('index.oneCol.addNewMaster', {
				    url: '/content/masters/add',
				    templateUrl: 'app/modules/content/partials/masterDetail.html',
				    controller: 'manageMasterController'
				})
				.state('index.oneCol.masterDetail', {
					url: '/content/masters/{id:int}',
					templateUrl: 'app/modules/content/partials/masterDetail.html',
					controller: 'manageMasterController'
				})
				.state('index.oneCol.manageRecipeCategories', {
				    url: '/content/recipes/categories',
				    templateUrl: 'app/modules/content/partials/recipeCategoriesTreeView.html',
				    controller: 'recipesCategoryController',
				    params: { mode: 'list', id: null },
				})
				.state('index.oneCol.addNewRecipeCategory', {
				    url: '/content/recipes/categories/add?{categoryid:int}',
				    templateUrl: 'app/modules/content/partials/recipeCategoryDetail.html',
				    controller: 'manageRecipeCategoryController'
				})
				.state('index.oneCol.recipeCategoryDetail', { 
				    url: '/content/recipes/categories/{id:int}',
				    templateUrl: 'app/modules/content/partials/recipeCategoryDetail.html',
				    controller: 'manageRecipeCategoryController'
				})
		}
]);