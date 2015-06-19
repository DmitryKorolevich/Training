'use strict';

angular.module('app.modules.product.controllers.discountManageController', [])
.controller('discountManageController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', '$modal', 'productService', 'discountService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, $timeout, $modal, productService, discountService, toaster, confirmUtil, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.discount.Id = result.Data.Id;
            } else {
                var messages = "";
                if (result.Messages) {
                    $scope.forms.submitted = true;
                    $scope.detailsTab.active = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);
                    $.each(result.Messages, function (index, value) {
                        if (value.Field) {
                            $scope.forms.mainForm[value.Field].$setValidity("server", false);
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

            $scope.assignedCustomerTypes = $rootScope.ReferenceData.AssignedCustomerTypes;
            $scope.discountTypes = $rootScope.ReferenceData.DiscountTypes;

            $scope.forms = {};
            $scope.detailsTab = {
                active: true
            };

            loadCategories();
        };

        function loadCategories() {
            productService.getCategoriesTree({}, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.rootCategory = result.Data;
                        loadDiscount();
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function (result) {
                    errorHandler(result);
                });
        };

        function loadDiscount() {
            discountService.getDiscount($scope.id, $scope.refreshTracker)
			    .success(function (result) {
			        if (result.Success) {
			            $scope.discount = result.Data;
			            if ($scope.discount.DiscountsToSelectedSkus.length > 0) {
			                $scope.discount.SelectedProductsOnly = true;
			            }
			            setSelected($scope.rootCategory, $scope.discount.CategoryIds);
			            addProductsListWatchers();

			        } else {
			            errorHandler(result);
			        }
			    }).
			    error(function (result) {
			        errorHandler(result);
			    });
        };

        $scope.save = function () {
            $.each($scope.forms.mainForm, function (index, element) {
                if (element && element.$name == index) {
                    element.$setValidity("server", true);
                }
            });

            if (isValid()) {
                var categoryIds = [];
                getSelected($scope.rootCategory, categoryIds);
                $scope.discount.CategoryIds = categoryIds;

                var data = {};
                angular.copy($scope.discount, data);
                //Remove all selected skus if this option doesn't have sense
                if (data.DiscountType == 3 || !data.SelectedProductsOnly) {
                    data.DiscountsToSelectedSkus = [];
                }

                discountService.updateDiscount(data, $scope.refreshTracker).success(function (result) {
                    successSaveHandler(result);
                }).error(function (result) {
                    errorHandler(result);
                });
            } else {
                $scope.forms.submitted = true;
                $scope.detailsTab.active = true;
            }
        };

        var isValid = function () {
            if ($scope.forms.mainForm.$valid) {
                return true;
            }
            else {
                //This hard code is realted with bug with setting an error to a form after closing a date picker popup with empty value
                //https://github.com/angular-ui/bootstrap/issues/3701
                if (Object.keys($scope.forms.mainForm.$error).length == 1 && $scope.forms.mainForm.$error.date) {
                    var valid = true;
                    $.each($scope.forms.mainForm.$error.date, function (index, item) {
                        if (item.$formatters.length != 0) {
                            valid = false;
                            return;
                        }
                    });
                    return valid;
                }
            }
            return false;
        };

        function notifyAboutAddBlockIds(name) {
            var blockIds = [];
            var list = $scope.discount[name];
            $.each(list, function (index, item) {
                blockIds.push(item.IdSku);
            });
            var data = {};
            data.name = name;
            data.blockIds = blockIds;
            $scope.$broadcast('skusSearch#in#setBlockIds', data);
        };

        $scope.$on('skusSearch#out#addItems', function (event, args) {
            var list = $scope.discount[args.name];
            if (list) {
                if (args.items) {
                    var newSelectedSkus = [];
                    $.each(args.items, function (index, item) {
                        var add = true;
                        $.each(list, function (index, selectedSku) {
                            if (item.Id == selectedSku.IdSku) {
                                add = false;
                                return;
                            }
                        });
                        if (add) {
                            var newSelectedSku = {};
                            newSelectedSku.IdSku = item.Id;
                            newSelectedSku.ShortSkuInfo = {};
                            newSelectedSku.ShortSkuInfo.Code = item.Code;
                            newSelectedSku.ShortSkuInfo.ProductName = item.ProductName;
                            newSelectedSkus.push(newSelectedSku);
                        }
                    });
                    $.each(newSelectedSkus, function (index, newSelectedSku) {
                        list.push(newSelectedSku);
                    });
                }
            }
        });

        function addProductsListWatchers() {
            $scope.$watchCollection('discount.DiscountsToSkus', function () {
                notifyAboutAddBlockIds('DiscountsToSkus');
            });
            $scope.$watchCollection('discount.DiscountsToSelectedSkus', function () {
                notifyAboutAddBlockIds('DiscountsToSelectedSkus');
            });
            notifyAboutAddBlockIds('DiscountsToSkus');
            notifyAboutAddBlockIds('DiscountsToSelectedSkus');
        };

        $scope.deleteDiscountToSku = function (index) {
            $scope.discount.DiscountsToSkus.splice(index, 1);
        };

        $scope.deleteDiscountToSelectedSku = function (index) {
            $scope.discount.DiscountsToSelectedSkus.splice(index, 1);
        };

        var getCategoriesTreeViewScope = function () {
            return angular.element($('.categories .ya-treeview').get(0)).scope();
        };

        $scope.updateCategoriesCollapsed = function (expand) {
            var scope = getCategoriesTreeViewScope();
            if (expand) {
                scope.expandAll();
            }
            else {
                scope.collapseAll();
            }
            $scope.categoriesExpanded = expand;
        };

        $scope.toogleEditorState = function (property) {
            $scope[property] = !$scope[property];
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
        };

        function getSelected(category, ids) {
            if (category.IsSelected) {
                ids.push(category.Id);
            }
            $.each(category.SubItems, function (index, value) {
                getSelected(value, ids);
            });
        };

        initialize();
    }
]);