﻿'use strict';

angular.module('app.modules.product.controllers.productCategoryManageController', [])
.controller('productCategoryManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'productService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, productService, toaster, confirmUtil, promiseTracker) {
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

	function successSaveHandler(result) {
		if (result.Success) {
			toaster.pop('success', "Success!", "Successfully saved.");
            $scope.id=result.Data.Id;
            $scope.productCategory.Id = result.Data.Id;
            $scope.productCategory.MasterContentItemId = result.Data.MasterContentItemId;
            $scope.previewUrl = $scope.baseUrl.format($scope.productCategory.Url);
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

	    $scope.baseUrl = $rootScope.ReferenceData.PublicHost + 'products/{0}?preview=true';
	    $scope.previewUrl = null;

	    $scope.statuses = $rootScope.ReferenceData.ProductCategoryStatusNames;

	    $scope.toogleEditorState = function (property) {
	        $scope[property] = !$scope[property];
	    };

	    $scope.productCategory =
        {
            Name: '',
            Url: '',
            Template: '',
            Title: null,
            MetaKeywords: null,
            MetaDescription: null,
            MasterContentItemId: 0,
            Status: "2:1",
        };
	    if ($stateParams.categoryid) {
	        $scope.productCategory.ParentId = $stateParams.categoryid;
	    }

	    $scope.loaded = false;
	    $scope.forms = {};

	    if ($scope.id) {
	        productService.getCategory($scope.id, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.productCategory = result.Data;
                        $scope.previewUrl = $scope.baseUrl.format($scope.productCategory.Url);
                        setUIstatus($scope.productCategory);
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
	        getUIstatus($scope.productCategory);

	        productService.updateCategory($scope.productCategory, $scope.editTracker).success(function (result) {
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

	function setUIstatus(contentPage) {
	    contentPage.Status = contentPage.StatusCode + ":" + contentPage.Assigned;
	};

	function getUIstatus(contentPage) {
	    var items = contentPage.Status.split(':');
	    contentPage.StatusCode = parseInt(items[0]);
	    contentPage.Assigned = parseInt(items[1]);
	};

	initialize();
}]);