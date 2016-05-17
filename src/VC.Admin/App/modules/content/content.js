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
	'app.modules.content.controllers.articlesController',
	'app.modules.content.controllers.articleManageController',
	'app.modules.content.controllers.articleCategoriesController',
	'app.modules.content.controllers.articleCategoryManageController',
	'app.modules.content.controllers.contentPagesController',
	'app.modules.content.controllers.contentPageManageController',
	'app.modules.content.controllers.contentPageCategoriesController',
	'app.modules.content.controllers.contentPageCategoryManageController',
	'app.modules.content.controllers.contentAreasController',
	'app.modules.content.controllers.contentAreaDetailController',
	'app.modules.content.controllers.manageStylesController',
	'app.modules.content.controllers.emailTemplatesController',
	'app.modules.content.controllers.emailTemplateManageController',
	'app.modules.content.controllers.contentCrossSellsManageController',
	'app.modules.content.controllers.articleBonusLinksController',
    'app.modules.content.controllers.aceErrorDetailsController'
])
.config([
		'$stateProvider', '$urlRouterProvider',
		function ($stateProvider, $urlRouterProvider) {

		    $stateProvider
                /*masters*/
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
                /*recipes*/
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
                /*faqs*/
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
		            url: '/content/faqs/categories/add?{categoryid:int}',
		            templateUrl: 'app/modules/content/partials/faqCategoryDetail.html',
		            controller: 'faqCategoryManageController'
		        })
		        .state('index.oneCol.faqCategoryDetail', {
		            url: '/content/faqs/categories/{id:int}',
		            templateUrl: 'app/modules/content/partials/faqCategoryDetail.html',
		            controller: 'faqCategoryManageController'
		        })
                /*articles*/
                .state('index.oneCol.manageArticles', {
                    url: '/content/articles',
                    templateUrl: 'app/modules/content/partials/articlesList.html',
                    controller: 'articlesController'
                })
		        .state('index.oneCol.addNewArticle', {
		            url: '/content/articles/add',
		            templateUrl: 'app/modules/content/partials/articleDetail.html',
		            controller: 'articleManageController'
		        })
		        .state('index.oneCol.articleDetail', {
		            url: '/content/articles/{id:int}',
		            templateUrl: 'app/modules/content/partials/articleDetail.html',
		            controller: 'articleManageController'
		        })
		        .state('index.oneCol.manageArticleCategories', {
		            url: '/content/articles/categories',
		            templateUrl: 'app/modules/content/partials/articleCategoriesTreeView.html',
		            controller: 'articleCategoriesController',
		            params: { mode: 'list', id: null },
		        })
		        .state('index.oneCol.addNewArticleCategory', {
		            url: '/content/articles/categories/add?{categoryid:int}',
		            templateUrl: 'app/modules/content/partials/articleCategoryDetail.html',
		            controller: 'articleCategoryManageController'
		        })
		        .state('index.oneCol.articleCategoryDetail', {
		            url: '/content/articles/categories/{id:int}',
		            templateUrl: 'app/modules/content/partials/articleCategoryDetail.html',
		            controller: 'articleCategoryManageController'
		        })
		        /*contentpages*/
                .state('index.oneCol.manageContentPages', {
                    url: '/content/contentpages',
                    templateUrl: 'app/modules/content/partials/contentPagesList.html',
                    controller: 'contentPagesController'
                })
		        .state('index.oneCol.addNewContentPage', {
		            url: '/content/contentpages/add',
		            templateUrl: 'app/modules/content/partials/contentPageDetail.html',
		            controller: 'contentPageManageController'
		        })
		        .state('index.oneCol.contentPageDetail', {
		            url: '/content/contentpages/{id:int}',
		            templateUrl: 'app/modules/content/partials/contentPageDetail.html',
		            controller: 'contentPageManageController'
		        })
		        .state('index.oneCol.manageContentPageCategories', {
		            url: '/content/contentpages/categories',
		            templateUrl: 'app/modules/content/partials/contentPageCategoriesTreeView.html',
		            controller: 'contentPageCategoriesController',
		            params: { mode: 'list', id: null },
		        })
		        .state('index.oneCol.addNewContentPageCategory', {
		            url: '/content/contentpages/categories/add?{categoryid:int}',
		            templateUrl: 'app/modules/content/partials/contentPageCategoryDetail.html',
		            controller: 'contentPageCategoryManageController'
		        })
		        .state('index.oneCol.contentPageCategoryDetail', {
		            url: '/content/contentpages/categories/{id:int}',
		            templateUrl: 'app/modules/content/partials/contentPageCategoryDetail.html',
		            controller: 'contentPageCategoryManageController'
		        })
				/*manage areas*/
				.state('index.oneCol.manageContentAreas', {
				    url: '/content/contentareas',
				    templateUrl: 'app/modules/content/partials/contentAreas.html',
				    controller: 'contentAreasController'
				})
				.state('index.oneCol.manageContentAreaDetail', {
				    url: '/content/contentareas/{id:int}',
				    templateUrl: 'app/modules/content/partials/contentAreaDetail.html',
				    controller: 'contentAreaDetailController'
				})
				/*manage css*/
				.state('index.oneCol.manageStyles', {
				    url: '/content/css',
				    templateUrl: 'app/modules/content/partials/stylesDetail.html',
				    controller: 'manageStylesController'
				})
            	/*email templates*/
                .state('index.oneCol.manageEmailTemplates', {
                    url: '/content/emailtemplates',
                    templateUrl: 'app/modules/content/partials/emailTemplatesList.html',
                    controller: 'emailTemplatesController'
                })
		        .state('index.oneCol.emailTemplateDetail', {
		            url: '/content/emailtemplates/{id:int}',
		            templateUrl: 'app/modules/content/partials/emailTemplateDetail.html',
		            controller: 'emailTemplateManageController'
		        })
				/*content cross sells*/
				.state('index.oneCol.manageAddToCartCs', {
		    		url: '/content/managecross/addtocart',
		    		templateUrl: 'app/modules/content/partials/contentCrossSells.html',
		    		controller: 'contentCrossSellsManageController'
				})
				.state('index.oneCol.manageViewCartCs', {
					url: '/content/managecross/viewcart',
					templateUrl: 'app/modules/content/partials/contentCrossSells.html',
					controller: 'contentCrossSellsManageController'
				})
		}
]);