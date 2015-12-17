angular.module('app.modules.content.controllers.articlesController', [])
.controller('articlesController', ['$scope', '$state', 'contentService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker','gridSorterUtil',
    function ($scope, $state, contentService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil) {
        $scope.refreshTracker = promiseTracker("refresh");
        $scope.deleteTracker = promiseTracker("delete");

    function Category(data,level, root)
    {
        var self = this;
        if (!root) {
            self.Id = data.Id;
            var namePrefix = '';
            for (var i = 2; i <= level; i++) {
                namePrefix += '----';
            }
            self.Name = namePrefix + data.Name;
        }
        else
        {
            self.Id = null;
            self.Name = 'All';
        }

    }

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function refreshArticles() {
        contentService.getArticles($scope.filter,$scope.refreshTracker)
			.success(function (result) {
			    if (result.Success) {
			        $scope.articles = result.Data.Items;
                    $scope.totalItems = result.Data.Count;
                    $scope.loaded=true;
			    } else {
			        errorHandler(result);
			    }
			})
			.error(function (result) {
			    errorHandler(result);
			});
	};

    function loadCategories(){
        contentService.getCategoriesTree({ Type : 3},$scope.refreshTracker)//article categories
			.success(function (result) {
			    if (result.Success) {
			        initCategories(result.Data);
			        refreshArticles();
			    } else {
			        errorHandler(result);
			    }
			})
			.error(function (result) {
			    errorHandler(result);
			});
    };

    function initCategories(rootCategory){
        var categories = [];
        initCategory(rootCategory, categories, 0, true);
        categories.splice(1,0,new Category({Id: -1, Name: 'Uncategorized'}));
        $scope.categories = categories;
    }

    function initCategory(serviceCategory, categories, level, root) {
        categories.push(new Category(serviceCategory, level, root));
        $.each(serviceCategory.SubItems, function (index, category) {
            initCategory(category, categories,level+1)
        });
    }

	function initialize() {
	    $scope.filter = {
            Name: '',
            CategoryId: null,
            Paging: { PageIndex: 1, PageItemCount: 100 },
            Sorting: gridSorterUtil.resolve(refreshArticles, "Updated", "Desc")
	    };
        $scope.loaded=false;

		loadCategories();
	}

	$scope.filterArticles = function () {
	    $scope.filter.Paging.PageIndex = 1;
	    refreshArticles();
	};

	$scope.pageChanged = function () {
	    refreshArticles();
	};

	$scope.open = function (id) {
        if(id)
        {
            $state.go('index.oneCol.articleDetail',{ id: id});
        }
        else
        {
            $state.go('index.oneCol.addNewArticle');
        }
	};

	$scope.delete = function (id) {
		confirmUtil.confirm(function() {
		    contentService.deleteArticle(id,$scope.deleteTracker)
			    .success(function (result) {
			        if (result.Success) {
			            toaster.pop('success', "Success!", "Successfully deleted.");
			            refreshArticles();
			        } else {
			            errorHandler(result);
			        }
			    })
			    .error(function (result) {
			        errorHandler(result);
			    });
		}, 'Are you sure you want to delete this article?');
	};

	initialize();
}]);