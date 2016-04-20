'use strict';

angular.module('app.modules.inventorysku.controllers.inventorySkuManageController', [])
.controller('inventorySkuManageController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', '$uibModal', 'inventorySkuService', 'toaster', 'confirmUtil', 'promiseTracker', 'contentService',
    function ($scope, $rootScope, $state, $stateParams, $timeout, $uibModal, inventorySkuService, toaster, confirmUtil, promiseTracker, contentService)
    {
        $scope.refreshTracker = promiseTracker("refresh");

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.inventorySku.Id = result.Data.Id;
            } else {
                var messages = "";
                if (result.Messages)
                {
                    $scope.forms.submitted = true;
                    $scope.detailsTab.active = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);

                    $.each(result.Messages, function (index, value)
                    {
                        if (value.Field)
                        {
                            $.each($scope.forms, function (index, form)
                            {
                                if (form && !(typeof form === 'boolean'))
                                {
                                    if (form[value.Field] != undefined)
                                    {
                                        form[value.Field].$setValidity("server", false);
                                        return false;
                                    }
                                }
                            });
                        }
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

            $scope.forms = {};

            $scope.detailsTab = {
                active: true,
                formName: 'details',
            };
            $scope.categoriesTab = {
                active: false,
                formName: 'categories',
            };
            var tabs = [];
            tabs.push($scope.detailsTab);
            tabs.push($scope.categoriesTab);
            $scope.tabs = tabs;

            loadLookups();
            loadInventoryCategories();
        };

        function allowLoadInventorySku()
        {
        	if ($scope.lookups && $scope.rootInventoryCategory)
            {
                loadInventorySku();
            };
        };

        function loadLookups() {
            inventorySkuService.getInventorySkuLookups($scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        var data = result.Data;
                        $scope.lookups = {};
                        $.each(data, function (index, item)
                        {
                            if (item.Name == 'InventorySkuChannels')
                            {
                                $scope.lookups.inventorySkuChannels = item.LookupVariants;
                            } else if (item.Name == 'InventorySkuProductSources')
                            {
                                $scope.lookups.inventorySkuProductSources = item.LookupVariants;
                            } else if (item.Name == 'InventorySkuUnitOfMeasures')
                            {
                                $scope.lookups.inventorySkuUnitOfMeasures = item.LookupVariants;
                            } else if (item.Name == 'InventorySkuPurchaseUnitOfMeasures')
                            {
                                $scope.lookups.inventorySkuPurchaseUnitOfMeasures = item.LookupVariants;
                            }
                        });
                        allowLoadInventorySku();
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function (result) {
                    errorHandler(result);
                });
        };

        function loadInventoryCategories() {
            inventorySkuService.getInventorySkuCategoriesTree({}, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.rootInventoryCategory = {};
                        $scope.rootInventoryCategory.SubItems = result.Data;
                        allowLoadInventorySku();
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function (result) {
                    errorHandler(result);
                });
        };

        function loadInventorySku()
        {
            inventorySkuService.getInventorySku($scope.id, $scope.refreshTracker)
			    .success(function (result) {
			        if (result.Success) {
			            $scope.inventorySku = result.Data;

			            $scope.updateSalesCategoriesCollapsed(true);
			            //setInventorySelected($scope.rootInventoryCategory, $scope.inventorySku.IdInventorySkuCategory);
			        } else {
			            errorHandler(result);
			        }
			    }).
			    error(function (result) {
			        errorHandler(result);
			    });
        };

        var clearServerValidation = function ()
        {
            $.each($scope.forms, function (index, form) {
                if (form && !(typeof form === 'boolean')) {
                    $.each(form, function (index, element)
                    {
                        if (element && element.$name == index)
                        {
                            element.$setValidity("server", true);
                        }
                    });
                }
            });
        };

        $scope.save = function () {
            clearServerValidation();

            if ($scope.forms.details.$valid)
            {
                inventorySkuService.updateInventorySku($scope.inventorySku, $scope.refreshTracker).success(function (result)
                {
                    successSaveHandler(result);
                }).error(function (result) {
                    errorHandler(result);
                });
            } else {
                $scope.forms.submitted = true;
                $scope.detailsTab.active = true;
                toaster.pop('error', "Error!", "Validation errors, please correct field values.", null, 'trustedHtml');
            }
        };


        $scope.updateSalesCategoriesCollapsed = function (expand)
        {
            if (expand)
            {
                $scope.$broadcast('angular-ui-tree:expand-all');
            }
            else
            {
                $scope.$broadcast('angular-ui-tree:collapse-all');
            }
            $scope.salesCategoriesExpanded = expand;
        };

        function setInventorySelected(category, id) {
            if (id!=null && category.Id == id) {
                expandInventoryCategory(category.Id);
            };
            $.each(category.SubItems, function (index, value) {
                setInventorySelected(value, id);
            });
        };

        function getInventoryRootNodesScopes() {
            return angular.element($('.categories.sales .ya-treeview').get(0)).scope().$nodesScope.childNodes();
        }

        function expandInventoryCategory(id) {
            var parentScopes = getInventoryScopePath(id);

            for (var i = 0; i < parentScopes.length; i++) {
                parentScopes[i].expand();
            }
        }

        function getInventoryScopePath(id) {
            var toReturn = null;
            var rootScopes = getInventoryRootNodesScopes();
            $.each(rootScopes, function (index, scope) {
                var result = getScopePath(id, scope, []);
                if (result)
                {
                    toReturn = result;
                    return false;
                }
            });
            return toReturn;
        }

        function getScopePath(id, scope, parentScopeList) {

            if (!scope) return null;

            var newParentScopeList = parentScopeList.slice();
            newParentScopeList.push(scope);

            if (scope.$modelValue && scope.$modelValue.Id === id) {
                return newParentScopeList;
            }

            var foundScopesPath = null;
            var childNodes = scope.childNodes();

            for (var i = 0; foundScopesPath === null && i < childNodes.length; i++) {
                foundScopesPath = getScopePath(id, childNodes[i], newParentScopeList);
            }

            return foundScopesPath;
        }

        function activateTab(formName) {
            $.each($scope.tabs, function (index, item) {
                if (item.formName == formName) {
                    item.active = true;
                    return false;
                }
            });
        };

        $scope.toggleOpen = function (item, event) {
            item.IsOpen = !item.IsOpen;
        };

        initialize();
    }
]);