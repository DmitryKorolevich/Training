angular.module('app.modules.content.controllers.contentPagesController', [])
.controller('contentPagesController', ['$scope', '$state', 'contentService', 'toaster', 'modalUtil', 'confirmUtil','promiseTracker',
    function ($scope, $state, contentService, toaster, modalUtil, confirmUtil, promiseTracker) {
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

    function refreshContentPages() {
        contentService.getContentPages($scope.filter,$scope.refreshTracker)
			.success(function (result) {
			    if (result.Success) {
			        $scope.contentPages = result.Data.Items;
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
        contentService.getCategoriesTree({ Type : 7},$scope.refreshTracker)//contentPage categories
			.success(function (result) {
			    if (result.Success) {
			        initCategories(result.Data);
			        refreshContentPages();
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
            initCategory(category, categories, level + 1);
        });
    }

	function initialize() {
	    $scope.filter = {
            Name: '',
            CategoryId: null,
	        Paging: { PageIndex: 1, PageItemCount: 100 }
	    };
        $scope.loaded=false;

		loadCategories();
	}

	$scope.filterContentPages = function () {
	    $scope.filter.Paging.PageIndex = 1;
		refreshContentPages();
	};

	$scope.pageChanged = function () {
	    refreshContentPages();
	};

	$scope.open = function (id) {
        if(id)
        {
            $state.go('index.oneCol.contentPageDetail',{ id: id});
        }
        else
        {
            $state.go('index.oneCol.addNewContentPage');
        }
	};

	$scope.delete = function (id) {
		confirmUtil.confirm(function() {
		    contentService.deleteContentPage(id,$scope.deleteTracker)
			    .success(function (result) {
			        if (result.Success) {
			            toaster.pop('success', "Success!", "Successfully deleted.");
			            refreshContentPages();
			        } else {
			            errorHandler(result);
			        }
			    })
			    .error(function (result) {
			        errorHandler(result);
			    });
		}, 'Are you sure you want to delete this content page?');
	};

	initialize();
}]);