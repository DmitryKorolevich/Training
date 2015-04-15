'use strict';

angular.module('app.modules.content', [
	'app.modules.content.controllers.mastersController',
	'app.modules.content.controllers.masterManageController',
	'app.modules.content.controllers.recipesController',
	'app.modules.content.controllers.recipeManageController',
	'app.modules.content.controllers.recipeCategoriesController',
	'app.modules.content.controllers.recipeCategoryManageController',
	'app.modules.content.controllers.faqsController',
	'app.modules.content.controllers.faqManageController',
	'app.modules.content.controllers.faqCategoriesController',
	'app.modules.content.controllers.faqCategoryManageController',
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
		        .state('index.oneCol.manageMasters', {
		            url: '/content/masters',
		            templateUrl: 'app/modules/content/partials/mastersList.html',
		            controller: 'mastersController'
		        })
		        .state('index.oneCol.addNewMaster', {
		            url: '/content/masters/add',
		            templateUrl: 'app/modules/content/partials/masterDetail.html',
		            controller: 'masterManageController'
		        })
		        .state('index.oneCol.masterDetail', {
		            url: '/content/masters/{id:int}',
		            templateUrl: 'app/modules/content/partials/masterDetail.html',
		            controller: 'masterManageController'
		        })
		        .state('index.oneCol.manageRecipes', {
		            url: '/content/recipes',
		            templateUrl: 'app/modules/content/partials/recipesList.html',
		            controller: 'recipesController'
		        })
		        .state('index.oneCol.addNewRecipe', {
		            url: '/content/recipes/add',
		            templateUrl: 'app/modules/content/partials/recipeDetail.html',
		            controller: 'recipeManageController'
		        })
		        .state('index.oneCol.recipeDetail', {
		            url: '/content/recipes/{id:int}',
		            templateUrl: 'app/modules/content/partials/recipeDetail.html',
		            controller: 'recipeManageController'
		        })
		        .state('index.oneCol.manageRecipeCategories', {
		            url: '/content/recipes/categories',
		            templateUrl: 'app/modules/content/partials/recipeCategoriesTreeView.html',
		            controller: 'recipeCategoriesController',
		            params: { mode: 'list', id: null },
		        })
		        .state('index.oneCol.addNewRecipeCategory', {
		            url: '/content/recipes/categories/add?{categoryid:int}',
		            templateUrl: 'app/modules/content/partials/recipeCategoryDetail.html',
		            controller: 'recipeCategoryManageController'
		        })
		        .state('index.oneCol.recipeCategoryDetail', {
		            url: '/content/recipes/categories/{id:int}',
		            templateUrl: 'app/modules/content/partials/recipeCategoryDetail.html',
		            controller: 'recipeCategoryManageController'
		        })
		        .state('index.oneCol.manageFaqs', {
		            url: '/content/faqs',
		            templateUrl: 'app/modules/content/partials/faqsList.html',
		            controller: 'faqsController'
		        })
		        .state('index.oneCol.addNewFaq', {
		            url: '/content/faqs/add',
		            templateUrl: 'app/modules/content/partials/faqDetail.html',
		            controller: 'faqManageController'
		        })
		        .state('index.oneCol.faqDetail', {
		            url: '/content/faqs/{id:int}',
		            templateUrl: 'app/modules/content/partials/faqDetail.html',
		            controller: 'faqManageController'
		        })
		        .state('index.oneCol.manageFaqCategories', {
		            url: '/content/faqs/categories',
		            templateUrl: 'app/modules/content/partials/faqCategoriesTreeView.html',
		            controller: 'faqCategoriesController',
		            params: { mode: 'list', id: null },
		        })
		        .state('index.oneCol.addNewFaqCategory', {
		            url: '/content/recipes/categories/add?{categoryid:int}',
		            templateUrl: 'app/modules/content/partials/faqCategoryDetail.html',
		            controller: 'faqCategoryManageController'
		        })
		        .state('index.oneCol.faqCategoryDetail', {
		            url: '/content/faqs/categories/{id:int}',
		            templateUrl: 'app/modules/content/partials/faqCategoryDetail.html',
		            controller: 'faqCategoryManageController'
		        });
		}
]);