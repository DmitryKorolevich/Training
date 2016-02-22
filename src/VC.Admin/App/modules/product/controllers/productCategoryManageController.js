'use strict';

angular.module('app.modules.product.controllers.productCategoryManageController', [])
.controller('productCategoryManageController', ['$scope', '$rootScope', '$state', '$stateParams', 'productService', 'contentService', 'settingService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, productService, contentService, settingService, toaster, confirmUtil, promiseTracker)
    {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        function refreshHistory()
        {
            if ($scope.productCategory && $scope.productCategory.Id)
            {
                var data = {};
                data.service = settingService;
                data.tracker = $scope.refreshTracker;
                data.idObject = $scope.productCategory.Id;
                data.idObjectType = 14//product category
                $scope.$broadcast('objectHistorySection#in#refresh', data);
            }
        }

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.id = result.Data.Id;
                $scope.productCategory.Id = result.Data.Id;
                $scope.productCategory.MasterContentItemId = result.Data.MasterContentItemId;
                $scope.previewUrl = $scope.baseUrl.format($scope.productCategory.Url);
                refreshHistory();
            } else {
                var messages = "";
                if (result.Messages) {
                    $scope.forms.form.submitted = true;
                    $scope.detailsTab.active = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);
                    $.each(result.Messages, function (index, value) {
                        if (value.Field) {
                            $scope.forms.form[value.Field].$setValidity("server", false);
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

            $scope.baseUrl = 'http://' + $rootScope.ReferenceData.PublicHost + '/products/{0}?preview=true';
            $scope.previewUrl = null;

            $scope.statuses = $rootScope.ReferenceData.ProductCategoryStatusNames;
            $scope.visibleOptions = $rootScope.ReferenceData.VisibleOptions;

            $scope.toogleEditorState = function (property) {
                $scope[property] = !$scope[property];
            };

            $scope.detailsTab = {
                active: true
            };

            $scope.loaded = false;
            $scope.forms = {};

            refreshMasters();
        }

        function refreshCategory()
        {
            productService.getCategory($scope.id, $scope.refreshTracker)
                .success(function (result)
                {
                    if (result.Success)
                    {
                        $scope.productCategory = result.Data;
                        if ($scope.productCategory.Url)
                        {
                            $scope.previewUrl = $scope.baseUrl.format($scope.productCategory.Url);
                        };
                        if (!$scope.productCategory.MasterContentItemId)
                        {
                            $scope.productCategory.MasterContentItemId = $scope.MasterContentItemId;
                        };
                        if ($stateParams.categoryid)
                        {
                            $scope.productCategory.ParentId = $stateParams.categoryid;
                        };
                        setUIstatus($scope.productCategory);
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
        }

        function refreshMasters()
        {
            contentService.getMasterContentItems({ Type: 9 })//productCategory
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
                        refreshCategory();
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

        $scope.save = function () {
            $.each($scope.forms.form, function (index, element) {
                if (element && element.$name == index) {
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
                $scope.detailsTab.active = true;
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