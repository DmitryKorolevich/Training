'use strict';

angular.module('app.modules.content.controllers.faqCategoryManageController', [])
.controller('faqCategoryManageController', ['$scope', '$state', '$stateParams', 'contentService', 'toaster', 'confirmUtil', function ($scope, $state, $stateParams, contentService, toaster, confirmUtil) {
   
	function successSaveHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved.");
            $scope.id=result.Data.Id;
            $scope.faqCategory.Id = result.Data.Id;
            $scope.faqCategory.MasterContentItemId = result.Data.MasterContentItemId;
            $scope.previewUrl = $scope.baseUrl + $scope.faqCategory.Url;
		} else {
            var messages=""; 
            if(result.Messages)
            {
                $scope.forms.form.submitted = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $.each(result.Messages, function (index, value) {
                    if (value.Field) {
                        $scope.forms.form[value.Field].$setValidity("server", false);
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

	    $scope.baseUrl = 'http://dev2.vitalchoice.com:5010/faqs/';
	    $scope.previewUrl = null;

	    $scope.faqCategory =
        {
            Name: '',
            Url: '',
            Type: 5,//faq category
            Template: '@default()',
            Title: null,
            MetaKeywords: null,
            MetaDescription: null,
            MasterContentItemId: 0,
        };
	    if ($stateParams.categoryid) {
	        $scope.faqCategory.ParentId = $stateParams.categoryid;
	    }

	    $scope.loaded = false;
	    $scope.forms = {};

	    if ($scope.id) {
	        contentService.getCategory($scope.id)
                .success(function (result) {
                    if (result.Success) {
                        $scope.faqCategory = result.Data;
                        $scope.previewUrl = $scope.baseUrl + $scope.faqCategory.Url;
                        $scope.loaded = true;
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function (result) {
                    errorHandler(result);
                });
	    }
	    else
	    {
	        $scope.loaded = true;
	    }
	}

	$scope.save = function () {
	    $.each($scope.forms.form, function (index, element) {
	        if (element.$name == index) {
	            element.$setValidity("server", true);
	        }
	    });

	    if ($scope.forms.form.$valid) {
	        contentService.updateCategory($scope.faqCategory).success(function (result) {
	            successSaveHandler(result);
	        }).
                error(function (result) {
                    errorHandler(result);
                });
	    } else {
	        $scope.forms.form.submitted = true;
	    }
	};

	$scope.goToMaster = function (id) {
	    $state.go('index.oneCol.masterDetail', { id: id });
	};

	initialize();
}]);