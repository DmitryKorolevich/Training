'use strict';

angular.module('app.modules.content', [
	'app.modules.content.controllers.recipesController',
	'app.modules.content.controllers.manageRecipeController',
	'app.modules.content.controllers.manageMasterController',
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
		}
]);