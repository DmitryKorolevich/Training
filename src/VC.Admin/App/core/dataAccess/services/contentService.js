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

	    //master templates
	    getMasterContentItems: function (filter, tracker) {
	        return $http.post(baseUrl + 'GetMasterContentItems', filter, getConfig(tracker));
	    },
	    getMasterContentItem: function (id, tracker) {
	        return $http.get(baseUrl + 'GetMasterContentItem/' + id, getConfig(tracker));
	    },
	    updateMasterContentItem: function (model, tracker) {
	        return $http.post(baseUrl + 'UpdateMasterContentItem', model, getConfig(tracker));
	    },
	    deleteMasterContentItem: function (id, tracker) {
	        return $http.post(baseUrl + 'DeleteMasterContentItem/' + id, null, getConfig(tracker));
	    },

	    //email templates
	    getEmailTemplates: function (filter, tracker)
	    {
	        return $http.post(baseUrl + 'GetEmailTemplates', filter, getConfig(tracker));
	    },
	    getEmailTemplate: function (id, tracker)
	    {
	        return $http.get(baseUrl + 'GetEmailTemplate/' + id, getConfig(tracker));
	    },
	    updateEmailTemplate: function (model, tracker)
	    {
	        return $http.post(baseUrl + 'UpdateEmailTemplate', model, getConfig(tracker));
	    },
	    deleteEmailTemplate: function (id, tracker)
	    {
	        return $http.post(baseUrl + 'DeleteEmailTemplate/' + id, getConfig(tracker));
	    },

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
		getRecipeSettings: function(tracker) {
		    return $http.get(baseUrl + 'GetRecipeSettings', null, getConfig(tracker));
		},

	    //faq
		getFAQs: function (filter, tracker) {
		    return $http.post(baseUrl + 'GetFAQs', filter, getConfig(tracker));
		},
		getFAQ: function (id, tracker) {
		    return $http.get(baseUrl + 'GetFAQ/' + id, getConfig(tracker));
		},
		updateFAQ: function (model, tracker) {
		    return $http.post(baseUrl + 'UpdateFAQ', model, getConfig(tracker));
		},
		deleteFAQ: function (id, tracker) {
		    return $http.post(baseUrl + 'DeleteFAQ/' + id, null, getConfig(tracker));
		},

	    //articles
		getArticles: function (filter, tracker) {
		    return $http.post(baseUrl + 'GetArticles', filter, getConfig(tracker));
		},
		getArticle: function (id, tracker) {
		    return $http.get(baseUrl + 'GetArticle/' + id, getConfig(tracker));
		},
		updateArticle: function (model, tracker) {
		    return $http.post(baseUrl + 'UpdateArticle', model, getConfig(tracker));
		},
		deleteArticle: function (id, tracker) {
		    return $http.post(baseUrl + 'DeleteArticle/' + id, null, getConfig(tracker));
		},

	    //recipes
		getContentPages: function(filter,tracker) {
		    return $http.post(baseUrl + 'GetContentPages', filter, getConfig(tracker));
		},
		getContentPage: function (id,tracker) {
		    return $http.get(baseUrl + 'GetContentPage/' + id, getConfig(tracker));
		},
		updateContentPage: function(model,tracker) {
		    return $http.post(baseUrl + 'UpdateContentPage', model, getConfig(tracker));
		},
		deleteContentPage: function (id, tracker) {
		    return $http.post(baseUrl + 'DeleteContentPage/' + id, null, getConfig(tracker));
		},

		//content cross sells
		getContentCrossSells: function (type, tracker) {
			return $http.get(baseUrl + 'GetContentCrossSells/' + type, getConfig(tracker));
		},
		updateContentCrossSells: function (model, tracker) {
			return $http.post(baseUrl + 'UpdateContentCrossSells', model, getConfig(tracker));
		}
	};
}]);