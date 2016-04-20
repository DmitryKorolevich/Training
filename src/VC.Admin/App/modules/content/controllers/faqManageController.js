'use strict';

angular.module('app.modules.content.controllers.faqManageController', [])
.controller('faqManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker',
function ($scope, $rootScope, $state, $stateParams, contentService, settingService, toaster, confirmUtil, promiseTracker)
{
    $scope.refreshTracker = promiseTracker("get");
    $scope.editTracker = promiseTracker("edit");

    function refreshHistory()
    {
        if ($scope.faq && $scope.faq.Id)
        {
            var data = {};
            data.service = settingService;
            data.tracker = $scope.refreshTracker;
            data.idObject = $scope.faq.Id;
            data.idObjectType = 9//faq
            $scope.$broadcast('objectHistorySection#in#refresh', data);
        }
    }

    function successSaveHandler(result) {
        if (result.Success) {
            toaster.pop('success', "Success!", "Successfully saved.");
            $scope.id = result.Data.Id;
            $scope.faq.Id = result.Data.Id;
            $scope.faq.MasterContentItemId = result.Data.MasterContentItemId;
            $scope.previewUrl = $scope.baseUrl.format($scope.faq.Url);
            refreshHistory();
        } else {
            var messages = "";
            if (result.Messages) {
                $scope.forms.faqForm.submitted = true;
                $scope.detailsTab.active = true;
                $scope.serverMessages = new ServerMessages(result.Messages);
                $.each(result.Messages, function (index, value) {
                    if (value.Field) {
                        $scope.forms.faqForm[value.Field].$setValidity("server", false);
                    }
                    messages += value.Message + "<br />";
                });
            }
            toaster.pop('error', "Error!", messages, null, 'trustedHtml');
        }
    };

    function errorHandler(result) {
        toaster.pop('error', "Error!", "Server error occured");
    };

    function initialize() {
        $scope.id = $stateParams.id ? $stateParams.id : 0;
        $scope.descriptionExpanded = false;

        $scope.toogleEditorState = function (property) {
            $scope[property] = !$scope[property];
        };

        $scope.baseUrl = 'http://' + $rootScope.ReferenceData.PublicHost + '/faq/{0}?preview=true';
        $scope.previewUrl = null;

        $scope.detailsTab = {
            active: true
        };
        $scope.loaded = false;
        $scope.forms = {};

        $scope.save = function () {
            $.each($scope.forms.faqForm, function (index, element) {
                if (element && element.$name == index) {
                    element.$setValidity("server", true);
                }
            });

            if ($scope.forms.faqForm.$valid) {
                var categoryIds = [];
                getSelected($scope.rootCategory, categoryIds);
                $scope.faq.CategoryIds = categoryIds;

                contentService.updateFAQ($scope.faq, $scope.editTracker).success(function (result) {
                    successSaveHandler(result);
                }).
                    error(function (result) {
                        errorHandler(result);
                    });
            } else {
                $scope.forms.faqForm.submitted = true;
                $scope.detailsTab.active = true;
            }
        };

        contentService.getCategoriesTree({ Type: 5 }, $scope.refreshTracker)//faq categories
			.success(function (result) {
			    if (result.Success) {
			        $scope.rootCategory = result.Data;
			        refreshMasters();
			    } else {
			        errorHandler(result);
			    }
			}).
			error(function (result) {
			    errorHandler(result);
			});
    };

    function refreshFAQ()
    {
        contentService.getFAQ($scope.id, $scope.refreshTracker)
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.faq = result.Data;
                    if ($scope.faq.Url)
                    {
                        $scope.previewUrl = $scope.baseUrl.format($scope.faq.Url);
                    };
                    if (!$scope.faq.MasterContentItemId)
                    {
                        $scope.faq.MasterContentItemId = $scope.MasterContentItemId;
                    };
                    setSelected($scope.rootCategory, $scope.faq.CategoryIds);
                    $scope.loaded = true;
                    refreshHistory();
                } else
                {
                    errorHandler(result);
                }
            }).
            error(function (result)
            {
                errorHandler(result);
            });
    };

    function refreshMasters()
    {
        contentService.getMasterContentItems({ Type: 6 })//faq
            .success(function (result)
            {
                if (result.Success)
                {
                    $scope.masters = result.Data;
                    $.each($scope.masters, function (index, master)
                    {
                        if (master.IsDefault)
                        {
                            $scope.MasterContentItemId = master.Id;
                        };
                    });
                    $scope.MastersLoaded = true;
                    refreshFAQ();
                } else
                {
                    errorHandler(result);
                }
            })
            .error(function (result)
            {
                errorHandler(result);
            });
    };

    $scope.updateCategoriesCollapsed = function (expand)
    {
        if (expand)
        {
            $scope.$broadcast('angular-ui-tree:expand-all');
        }
        else
        {
            $scope.$broadcast('angular-ui-tree:collapse-all');
        }
        $scope.categoriesExpanded = expand;
    };

    function setSelected(category, ids) {
        category.IsSelected = false;
        $.each(ids, function (index, id) {
            if (category.Id == id) {
                category.IsSelected = true;
            }
        });
        $.each(category.SubItems, function (index, value) {
            setSelected(value, ids);
        });
    }

    function getSelected(category, ids) {
        if (category.IsSelected) {
            ids.push(category.Id);
        }
        $.each(category.SubItems, function (index, value) {
            getSelected(value, ids);
        });
    }

    $scope.goToMaster = function (id) {
        $state.go('index.oneCol.masterDetail', { id: id });
    };

    initialize();
}]);