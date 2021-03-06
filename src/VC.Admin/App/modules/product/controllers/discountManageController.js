﻿'use strict';

angular.module('app.modules.product.controllers.discountManageController', [])
.controller('discountManageController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', '$uibModal', 'productService', 'discountService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, $timeout, $uibModal, productService, discountService, toaster, confirmUtil, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");
        
        function refreshHistory()
        {
            if ($scope.discount && $scope.discount.Id)
            {
                var data = {};
                data.service = discountService;
                data.tracker = $scope.refreshTracker;
                data.idObject = $scope.discount.Id;
                data.idObjectType = 4//discount
                $scope.$broadcast('objectHistorySection#in#refresh', data);
            }
        }

        function successSaveHandler(result) {
            if (result.Success)
            {
                if (!$scope.discount.Id)
                {
                    $state.go('index.oneCol.discountDetail', { id: result.Data.Id });
                }

                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.discount.Id = result.Data.Id;
                $scope.discount.DiscountTiers = result.Data.DiscountTiers;
                refreshHistory();
            } else
            {
                $rootScope.fireServerValidation(result, $scope);
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
            $scope.maxTimesUseModes = [
                { Key: 1, Text: 'One Time Only' },
                { Key: 2, Text: 'Unlimited' },
                { Key: 3, Text: 'Number of Uses' },
            ];

            $scope.skuFilter = {
                Code: '',
                Paging: { PageIndex: 1, PageItemCount: 20 },
            };

            $scope.forms = {};
            $scope.forms.discountTiersSubmitted = false;
            $scope.detailsTab = {
                active: true
            };
            $scope.options = {};

            loadCategories();
        };

        function loadCategories() {
            productService.getCategoriesTree({}, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.rootCategory = result.Data;
                        $scope.rootAppliedCategory = angular.copy($scope.rootCategory);
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
			                $scope.discount.SelectedMode = 2;
			            } else if ($scope.discount.CategoryIdsAppliedOnlyTo.length > 0)
			            {
			                $scope.discount.SelectedMode = 3;
			            } else
			            {
			                $scope.discount.SelectedMode = 1;
			            }

			            if ($scope.discount.ExpirationDate)
			            {
			                $scope.discount.ExpirationDate = Date.parseDateTime($scope.discount.ExpirationDate);
			            }
			            if ($scope.discount.StartDate)
			            {
			                $scope.discount.StartDate = Date.parseDateTime($scope.discount.StartDate);
			            }

			            if ($scope.discount.MaxTimesUse)
			            {
			                if ($scope.discount.MaxTimesUse == 1)
			                {
			                    $scope.options.maxTimesUseMode = 1;
			                }
			                else
			                {
			                    $scope.options.maxTimesUseMode = 3;
			                    $scope.options.maxTimesUse = $scope.discount.MaxTimesUse;
			                }
			            }
			            else
			            {
			                $scope.options.maxTimesUseMode = 2;
			            }

			            refreshHistory();

			            setSelected($scope.rootCategory, $scope.discount.CategoryIds);
			            setSelected($scope.rootAppliedCategory, $scope.discount.CategoryIdsAppliedOnlyTo);
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
            $scope.lastTierToAddingNewRequired = false;
            if ($scope.discount.DiscountTiers.length != 0 && $scope.discount.DiscountType==5)
            {
                $scope.forms.DiscountTiers["i" + ($scope.discount.DiscountTiers.length - 1)].To.$setValidity("required", true);
            }

            if (isMainFormValid() && $scope.forms.DiscountTiers.$valid) {
                var categoryIds = [];
                getSelected($scope.rootCategory, categoryIds);
                $scope.discount.CategoryIds = categoryIds;

                var categoryIdsAppliedOnlyTo = [];
                getSelected($scope.rootAppliedCategory, categoryIdsAppliedOnlyTo);
                $scope.discount.CategoryIdsAppliedOnlyTo = categoryIdsAppliedOnlyTo;

                var data = {};
                angular.copy($scope.discount, data);
                //Remove all selected skus if this option doesn't have sense
                if (data.DiscountType == 3 || data.SelectedMode==1)
                {
                    data.DiscountsToSelectedSkus = [];
                    data.CategoryIdsAppliedOnlyTo = [];
                } else
                {
                    if (data.SelectedMode == 2)
                    {
                        data.CategoryIdsAppliedOnlyTo = [];
                    }
                    if (data.SelectedMode == 3)
                    {
                        data.DiscountsToSelectedSkus = [];
                    }
                }
                if (data.DiscountType != 5) {
                    data.DiscountTiers = [];
                };

                if (data.ExpirationDate)
                {
                    data.ExpirationDate = data.ExpirationDate.toServerDateTime();
                }
                if (data.StartDate)
                {
                    data.StartDate = data.StartDate.toServerDateTime();
                }

                if ($scope.options.maxTimesUseMode == 1)
                {
                    data.MaxTimesUse = 1;
                } else if ($scope.options.maxTimesUseMode == 2)
                {
                    data.MaxTimesUse = null;
                } else
                {
                    data.MaxTimesUse = $scope.options.maxTimesUse;
                }

                discountService.updateDiscount(data, $scope.refreshTracker).success(function (result) {
                    successSaveHandler(result);
                }).error(function (result) {
                    errorHandler(result);
                });
            } else {
                $scope.forms.submitted = true;
                $scope.forms.discountTiersSubmitted = true;
                $scope.detailsTab.active = true;
                toaster.pop('error', "Error!", "Validation errors, please correct field values.", null, 'trustedHtml');
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

        $scope.updateCategoriesCollapsed = function (expand)
        {
            if (expand)
            {
                $scope.$broadcast('angular-ui-tree:expand-all', { name: 'filter-categories' });
            }
            else
            {
                $scope.$broadcast('angular-ui-tree:collapse-all', { name: 'filter-categories' });
            }
            $scope.categoriesExpanded = expand;
        };

        $scope.updateAppliedCategoriesCollapsed = function (expand)
        {
            if (expand)
            {
                $scope.$broadcast('angular-ui-tree:expand-all', { name: 'applied-categories' });
            }
            else
            {
                $scope.$broadcast('angular-ui-tree:collapse-all', { name: 'applied-categories' });
            }
            $scope.appliedCategoriesExpanded = expand;
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

        $scope.addTier = function ()
        {
            $scope.lastTierToAddingNewRequired = false;

            if ($scope.discount.DiscountTiers.length != 0)
            {
                $scope.forms.DiscountTiers["i" + ($scope.discount.DiscountTiers.length - 1)].To.$setValidity("required", true);
                if (!$scope.discount.DiscountTiers[$scope.discount.DiscountTiers.length - 1].To)
                {
                    $scope.forms.DiscountTiers["i" + ($scope.discount.DiscountTiers.length - 1)].To.$setValidity("required", false);
                    $scope.lastTierToAddingNewRequired = true;
                }
            }
            if ($scope.forms.DiscountTiers.$valid)
            {
                if ($scope.lastTierToAddingNewRequired)
                {
                    return;
                }

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
                    if (index != ($scope.discount.DiscountTiers.length - 1))
                    {
                        currentTier.To = Math.round((currentTier.From + 0.01) * 100) / 100;
                    }
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