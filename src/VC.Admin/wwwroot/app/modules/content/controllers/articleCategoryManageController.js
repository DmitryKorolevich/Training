﻿'use strict';

angular.module('app.modules.content.controllers.articleCategoryManageController', [])
.controller('articleCategoryManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'toaster', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, contentService, toaster, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");
   
	function successSaveHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved.");
            $scope.id=result.Data.Id;
            $scope.articleCategory.Id = result.Data.Id;
            $scope.articleCategory.MasterContentItemId = result.Data.MasterContentItemId;
            $scope.previewUrl = $scope.baseUrl.format($scope.articleCategory.Url);
		} else {
            var messages=""; 
            if(result.Messages)
            {
                $scope.forms.form.submitted = true;                
	            $scope.detailsTab.active = true;
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

	    $scope.baseUrl = $rootScope.ReferenceData.PublicHost + 'articles/{0}?preview=true';
	    $scope.previewUrl = null;

	    $scope.articleCategory =
        {
            Name: '',
            Url: '',
            Type: 3,//article category
            Template: '',
            Title: null,
            MetaKeywords: null,
            MetaDescription: null,
            MasterContentItemId: 0,
        };
	    if ($stateParams.categoryid) {
	        $scope.articleCategory.ParentId = $stateParams.categoryid;
	    }
	    $scope.detailsTab = {
	        active: true
	    };

	    $scope.loaded = false;
	    $scope.forms = {};

	    if ($scope.id) {
	        contentService.getCategory($scope.id,$scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.articleCategory = result.Data;
                        $scope.previewUrl = $scope.baseUrl.format($scope.articleCategory.Url);
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
	        contentService.updateCategory($scope.articleCategory,$scope.editTracker).success(function (result) {
	            successSaveHandler(result);
	        }).
                error(function (result) {
                    errorHandler(result);
                });
	    } else {
	        $scope.forms.form.submitted = true;
	        $scope.detailsTab.active = true;
	    }
	};

	$scope.goToMaster = function (id) {
	    $state.go('index.oneCol.masterDetail', { id: id });
	};

	initialize();
}]);