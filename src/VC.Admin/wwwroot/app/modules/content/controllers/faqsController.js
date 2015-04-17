angular.module('app.modules.content.controllers.faqsController', [])
.controller('faqsController', ['$scope', '$state', 'contentService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker',
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

    function refreshFaqs() {
        contentService.getFAQs($scope.filter,$scope.refreshTracker)
			.success(function (result) {
			    if (result.Success) {
			        $scope.faqs = result.Data.Items;
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
        contentService.getCategoriesTree({ Type : 5},$scope.refreshTracker)//faq categories
			.success(function (result) {
			    if (result.Success) {
			        initCategories(result.Data);
			        refreshFaqs();
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
	        Paging: { PageIndex: 1, PageItemCount: 100 }
	    };
        $scope.loaded=false;

		loadCategories();
	}

	$scope.filterFaqs = function () {
	    $scope.filter.Paging.PageIndex = 1;
		refreshFaqs();
	};

	$scope.pageChanged = function () {
	    refreshFaqs();
	};

	$scope.open = function (id) {
        if(id)
        {
            $state.go('index.oneCol.faqDetail',{ id: id});
        }
        else
        {
            $state.go('index.oneCol.addNewFaq');
        }
	};

	$scope.delete = function (id) {
		confirmUtil.confirm(function() {
		    contentService.deleteFAQ(id,$scope.deleteTracker)
			    .success(function (result) {
			        if (result.Success) {
			            toaster.pop('success', "Success!", "Successfully deleted");
			            refreshFaqs();
			        } else {
			            errorHandler(result);
			        }
			    })
			    .error(function (result) {
			        errorHandler(result);
			    });
		}, 'Are you sure you want to delete this faq item?');
	};

	initialize();
}]);