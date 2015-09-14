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
                    $scope.forms.discountTiersSubmitted = true;
                    $scope.detailsTab.active = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);

                    $.each(result.Messages, function (index, value) {
                        if (value.Field) {
                            if (value.Field.indexOf('.') > -1) {
                                var items = value.Field.split(".");
                                $scope.forms[items[0]][items[1]][items[2]].$setValidity("server", false);
                            }
                            else {
                                $.each($scope.forms, function (index, form) {
                                    if (form && !(typeof form === 'boolean')) {
                                        if (form[value.Field] != undefined) {
                                            form[value.Field].$setValidity("server", false);
                                            if (formForShowing == null) {
                                                formForShowing = index;
                                            }
                                            return false;
                                        }
                                    }
                                });
                            }
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

            $scope.assignedCustomerTypes = angular.copy($rootScope.ReferenceData.CustomerTypes);
            $scope.assignedCustomerTypes.splice(0, 0, { Key: null, Text: 'All' });
            $scope.discountTypes = $rootScope.ReferenceData.DiscountTypes;
            $scope.tierDiscountTypes = [
                { Key: 1, Text: "$" },
                { Key: 2, Text: "%" }
            ];//price or percent

            $scope.skuFilter = {
                Code: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };

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
			            if ($scope.discount.DiscountTiers.length == 0) {
			                $scope.addTier();
			            }
			        } else {
			            errorHandler(result);
			        }
			    }).
			    error(function (result) {
			        errorHandler(result);
			    });
        };

        $scope.save = function () {
            clearServerValidation();

            if (isMainFormValid() && $scope.forms.DiscountTiers.$valid) {
                var categoryIds = [];
                getSelected($scope.rootCategory, categoryIds);
                $scope.discount.CategoryIds = categoryIds;

                var data = {};
                angular.copy($scope.discount, data);
                //Remove all selected skus if this option doesn't have sense
                if (data.DiscountType == 3 || !data.SelectedProductsOnly) {
                    data.DiscountsToSelectedSkus = [];
                }
                if (data.DiscountType != 5) {
                    data.DiscountTiers = [];
                };

                discountService.updateDiscount(data, $scope.refreshTracker).success(function (result) {
                    successSaveHandler(result);
                }).error(function (result) {
                    errorHandler(result);
                });
            } else {
                $scope.forms.submitted = true;
                $scope.forms.discountTiersSubmitted = true;
                $scope.detailsTab.active = true;
            }
        };

        var clearServerValidation = function () {
            $.each($scope.forms, function (index, form) {
                if (form && !(typeof form === 'boolean')) {
                    if (index == "DiscountTiers") {
                        $.each(form, function (index, subForm) {
                            if (index.indexOf('i') == 0) {
                                $.each(subForm, function (index, element) {
                                    if (element && element.$name == index) {
                                        element.$setValidity("server", true);
                                    }
                                });
                            }
                        });
                    }
                    else {
                        $.each(form, function (index, element) {
                            if (element && element.$name == index) {
                                element.$setValidity("server", true);
                            }
                        });
                    }
                }
            });
        };

        var isMainFormValid = function () {
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

        $scope.addTier = function () {
            if ($scope.forms.DiscountTiers.$valid) {
                var tier = {
                    From: null,
                    To: null,
                    IdDiscountType: 2,//percent
                    Percent: null,
                    Amount: null,
                }
                if ($scope.discount.DiscountTiers.length != 0) {
                    var lastTier = $scope.discount.DiscountTiers[$scope.discount.DiscountTiers.length - 1];
                    if (lastTier.To) {
                        tier.From = lastTier.To + 0.01;
                    }
                }
                $scope.discount.DiscountTiers.push(tier);
                $scope.forms.discountTiersSubmitted = false;
            }
            else
            {
                $scope.forms.discountTiersSubmitted = true;
            }
        };

        $scope.tierToBlur = function (index)
        {
            var currentTier = $scope.discount.DiscountTiers[index];
            if (index == 0 && currentTier.From && currentTier.To)
            {
                if (currentTier.From >= currentTier.To)
                {
                    currentTier.To = Math.round((currentTier.From + 0.01) * 100) / 100;
                    recalculateTiers(index);
                }
            }
        };

        $scope.tierFromBlur = function (index) {
            recalculateTiers(index);
        };

        var recalculateTiers = function (index) {
            var currentTier = $scope.discount.DiscountTiers[index];
            if (index != 0)
            {
                var tierBefore = $scope.discount.DiscountTiers[index - 1];
                if(tierBefore && tierBefore.To)
                {
                    if (tierBefore.To >= currentTier.From)
                    {
                        currentTier.From = Math.round((tierBefore.To + 0.01) * 100) / 100;
                    }
                }
                else
                {
                    currentTier.From = null;
                }                    
            }
            if (currentTier.From) {
                if(currentTier.From >= currentTier.To)
                {
                    currentTier.To = Math.round((currentTier.From + 0.01) * 100) / 100;
                }
                if (index+1 < $scope.discount.DiscountTiers.length)
                {
                    index++;
                    recalculateTiers(index);
                }
            }
            else
            {
                if (index != 0) {
                    currentTier.To = null;
                }
            }
        };

        $scope.deleteTier = function (index) {
            $scope.discount.DiscountTiers.splice(index, 1);
        };

        $scope.getSkus = function (val) {
            $scope.skuFilter.Code = val;
            return productService.getSkus($scope.skuFilter)
                .then(function (result) {
                    return result.data.Data.map(function (item) {
                        return item.Code;
                    });
                });
        };

        initialize();
    }
]);