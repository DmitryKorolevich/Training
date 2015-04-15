'use strict';

angular.module('app.modules.content.controllers.faqManageController', [])
.controller('faqManageController', ['$scope','$state','$stateParams', 'contentService', 'toaster', 'confirmUtil', function ($scope,$state,$stateParams, contentService, toaster, confirmUtil) {

	function successSaveHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved.");
            $scope.id=result.Data.Id;
            $scope.faq.Id = result.Data.Id;
            $scope.faq.MasterContentItemId = result.Data.MasterContentItemId;
            $scope.previewUrl=$scope.baseUrl+$scope.faq.Url;
		} else {
            var messages=""; 
            if(result.Messages)
            {
                $scope.forms.faqForm.submitted = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $.each(result.Messages, function (index, value) {
                    if (value.Field) {
                        $scope.forms.faqForm[value.Field].$setValidity("server", false);
                    }
                    messages+=value.Message +"<br />";
                });
            }
    	    toaster.pop('error', "Error!", messages,null,'trustedHtml');
		}
	};

	function errorHandler(result) {
		toaster.pop('error', "Error!", "Server error occured");
	};

	function initialize() {
	    $scope.id = $stateParams.id;
	    $scope.descriptionExpanded = false;

	    $scope.toogleEditorState = function (property) {
	        $scope[property] = !$scope[property];
	    };

        $scope.baseUrl='http://dev2.vitalchoice.com:5010/faq/';
        $scope.previewUrl = null;

        $scope.faq=
        {
            Name:'',
            Url:'',
            Template: '@default()',
            Description:'',
            Title:null,
            MetaKeywords:null,
            MetaDescription: null,
            MasterContentItemId: 0,
        };
        $scope.loaded = false;
        $scope.forms={};

        $scope.save = function () {
            $.each($scope.forms.faqForm, function (index, element) {
                if (element.$name == index) {
                    element.$setValidity("server", true);
                }
            });

            if ($scope.forms.faqForm.$valid) {
                var categoryIds = [];
                getSelected($scope.rootCategory, categoryIds);
                $scope.faq.CategoryIds = categoryIds;

                contentService.updateFAQ($scope.faq).success(function (result) {
                    successSaveHandler(result);
                }).
                    error(function (result) {
                        errorHandler(result);
                    });
            } else {
                $scope.forms.faqForm.submitted = true;
            }
        };

	    contentService.getCategoriesTree({ Type: 5 })//faq categories
			.success(function (result) {
				if (result.Success) {
					$scope.rootCategory=result.Data;
                    if($scope.id)
                    {
                        contentService.getFAQ($scope.id)
			                .success(function (result) {
				                if (result.Success) {
					                $scope.faq=result.Data;
					                $scope.previewUrl = $scope.baseUrl + $scope.faq.Url;
                                    setSelected($scope.rootCategory, $scope.faq.CategoryIds);
                                    $scope.loaded=true;
				                } else {
					                errorHandler(result);
				                }
			                }).
			                error(function(result) {
				                errorHandler(result);
			                });
                    }
                    else
                    {
                        $scope.loaded=true;
                    }
				} else {
					errorHandler(result);
				}
			}).
			error(function(result) {
				errorHandler(result);
			});
	}

	function setSelected(category, ids) {
	    category.IsSelected = false;
        $.each(ids, function( index, id ) {
            if(category.Id==id)
            {
                category.IsSelected=true;
            }
        });
        $.each(category.SubItems, function( index, value ) {
            setSelected(value, ids);
        });
    }

    function getSelected(category , ids){
        if(category.IsSelected)
        {
            ids.push(category.Id);
        }
        $.each(category.SubItems, function( index, value ) {
            getSelected(value, ids);
        });
    }

	$scope.goToMaster = function (id) {
	    $state.go('index.oneCol.masterDetail', { id: id });
	};

	initialize();
}]);