﻿'use strict';

angular.module('app.modules.product.controllers.productManageController', [])
.controller('productManageController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', '$uibModal', 'productService', 'inventorySkuService', 'toaster', 'confirmUtil', 'promiseTracker', 'contentService', 'modalUtil',
    function ($scope, $rootScope, $state, $stateParams, $timeout, $uibModal, productService, inventorySkuService, toaster, confirmUtil, promiseTracker, contentService, modalUtil)
    {
        $scope.refreshTracker = promiseTracker("get");
        $scope.refreshInventoriesTracker = promiseTracker("refreshInventories");

        var sellerFieldName = 'Seller';
        var googleCategoryFieldName = 'GoogleCategory';
        var specialIconFieldName = 'SpecialIcon';
        
        var disregardStockFieldName = 'DisregardStock';
        var stockFieldName = 'Stock';
        var nonDiscountableFieldName = 'NonDiscountable';
        var hideFromDataFeedFieldName = 'HideFromDataFeed';
        var qTYThresholdFieldName = 'QTYThreshold';

        function refreshHistory()
        {
            if ($scope.product && $scope.product.Id)
            {
                var data = {};
                data.service = productService;
                data.tracker = $scope.refreshTracker;
                data.idObject = $scope.product.Id;
                data.idObjectType = 3//product
                $scope.$broadcast('objectHistorySection#in#refresh', data);
            }
        }

        function successSaveHandler(result) {
            if (result.Success)
            {
                if (!$scope.product.Id)
                {
                    $state.go('index.oneCol.productDetail', { id: result.Data.Id });
                }

                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.product.Id = result.Data.Id;
                $scope.previewUrl = $scope.basePreviewUrl.format($scope.product.Url);
                $scope.product.MasterContentItemId = result.Data.MasterContentItemId;
                refreshPossiableProductTypes();
                refreshHistory();
            } else
            {
                $rootScope.fireServerValidation(result, $scope, serverValidationFormNameHandler, openSKUs);
            }
        };

        var serverValidationFormNameHandler = function (formName)
        {
            if (formName.indexOf('SKUs') == 0 || formName == 'skusDetails')
            {
                formName = 'SKUs';
            }
            if (formName.indexOf('CrossSellProducts') == 0)
            {
                formName = 'crossSellProductsAndVideos';
            }
            if (formName.indexOf('Videos') == 0)
            {
                formName = 'crossSellProductsAndVideos';
            }

            return formName;
        };

        function errorHandler(result) {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function initialize() {
            $scope.id = $stateParams.id ? $stateParams.id : 0;

            $scope.forms = {};
            $scope.forms.submitted = false;
            $scope.forms.skussubmitted = false;
            $scope.forms.crossessubmitted = false;
            $scope.forms.videossubmitted = false;

            $scope.youTubeBaseUrl = 'https://www.youtube.com/watch?v={0}'
            $scope.basePreviewUrl = 'http://' + $rootScope.ReferenceData.PublicHost + '/product/{0}?preview=true';
            $scope.previewUrl = null;
            $scope.allTypes = $rootScope.ReferenceData.ProductTypes;
            $scope.googleCategories = [];
            $scope.specialIcons = [];
            $scope.sellers = [];
            $scope.defaultSeller = null;

            $scope.productTypeDefaults = {};
            $scope.options = {};
            $scope.parentDetailsTab = {
                index: 1,
                formName: 'parentDetails',
            };
            $scope.imagesTab = {
                index: 2,
                formName: 'images',
            };
            $scope.subProductsTab = {
                index: 3,
                formName: 'SKUs',
            };
            $scope.nutritionalTab = {
                index: 4,
                formName: 'nutritional',
            };
            $scope.categoriesTab = {
                index: 5,
                formName: 'categories',
            };
            $scope.salesCategoriesTab = {
                index: 6,
                formName: 'salesCategories',
            };
            $scope.crossSellProductsAndVideosTab = {
                index: 7,
                formName: 'crossSellProductsAndVideos',
            };
            $scope.options.activeTabIndex = $scope.parentDetailsTab.index;
            var tabs = [];
            tabs.push($scope.parentDetailsTab);
            tabs.push($scope.imagesTab);
            tabs.push($scope.subProductsTab);
            tabs.push($scope.nutritionalTab);
            tabs.push($scope.categoriesTab);
            tabs.push($scope.salesCategoriesTab);
            tabs.push($scope.crossSellProductsAndVideosTab);
            $scope.tabs = tabs;

            loadLookups();
            loadCategories();
            loadInventoryCategories();
			refreshMasters();
        };

        function refreshMasters() {
        	contentService.getMasterContentItems({ Type: 10 })//product
                .success(function (result) {
                	if (result.Success) {
                		$scope.masters = result.Data;
                		var hasDefailt = false;
                		$.each($scope.masters, function (index, master) {
                		    if (master.IsDefault || $scope.masters.length == 1) {
                		        hasDefailt = true;
                		        $scope.MasterContentItemId = master.Id;
                		    };
                		});
                		if (!hasDefailt) {
                		    $scope.MasterContentItemId = $scope.masters[0].Id;
                		}
                		allowLoadProduct();
                	} else {
                		errorHandler(result);
                	}
                })
                .error(function (result) {
                	errorHandler(result);
                });
        };

        function allowLoadProduct()
        {
        	if ($scope.lookups && $scope.defaults && $scope.rootCategory && $scope.rootInventoryCategory && $scope.masters)
            {
                loadProduct();
            };
        };

        function loadLookups() {
            productService.getProductEditSettings($scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.lookups = result.Data.Lookups;
                        $scope.defaults = result.Data.DefaultValues;
                        $.each($scope.lookups, function (index, lookup)
                        {
                            if (lookup.Name == 'InventorySkuChannels')
                            {
                                $scope.inventorySkuChannels = lookup.Items;
                            }
                        });
                        allowLoadProduct();
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function (result) {
                    errorHandler(result);
                });
        };

        function loadCategories() {
            productService.getCategoriesTree({}, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.rootCategory = result.Data;
                        allowLoadProduct();
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function (result) {
                    errorHandler(result);
                });
        };

        function loadInventoryCategories() {
            productService.getInventoryCategoriesTree({}, $scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.rootInventoryCategory = {};
                        $scope.rootInventoryCategory.SubItems = result.Data;
                        allowLoadProduct();
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function (result) {
                    errorHandler(result);
                });
        };

        function loadProduct()
        {
            if (!$scope.id && $stateParams.source)
            {
                $scope.id = $stateParams.source;
            }

            productService.getProduct($scope.id, $scope.refreshTracker)
			    .success(function (result) {
			        if (result.Success) {
			            $scope.product = result.Data;
			            $scope.product.StatusCode = $scope.product.StatusCode;
			            if ($scope.id == 0) {
			                if ($stateParams.type) {
			                    $scope.product.Type = $stateParams.type;
			                }
			                else {
			                    $scope.product.Type = 2;//Perishable
			                }
			            }
			            if ($scope.product.Url)
			            {
			                $scope.previewUrl = $scope.basePreviewUrl.format($scope.product.Url);
			            }
			            if (!$scope.product.MasterContentItemId) {
			            	$scope.product.MasterContentItemId = $scope.MasterContentItemId;
			            };

			            if ($scope.product.SKUs)
			            {
			                $.each($scope.product.SKUs, function (index, sku)
			                {
			                    if (sku.BornDate)
			                    {
			                        sku.BornDate = Date.parseDateTime(sku.BornDate);
			                        sku.OriginBornDate = sku.BornDate;
			                    }
			                });
			            }

			            setSelected($scope.rootCategory, $scope.product.CategoryIds);
			            //setInventorySelected($scope.rootInventoryCategory, $scope.product.InventoryCategoryId);
			            $scope.updateSalesCategoriesCollapsed(true);
			            refreshPossiableProductTypes();
			            setProductTypeWatch();
			            initCrossses();
			            initVideos();

			            if ($stateParams.source)
			            {
			                $scope.id = 0;
			                $scope.product.Id = 0;
			                $scope.product.SourceId = $stateParams.source;
			            }
			            refreshHistory();
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
                    if (index == "SKUs" || index == "CrossSellProducts" || index == "Videos") {                        
                        $.each(form, function (index, subForm) {
                            if(index.indexOf('i')==0)
                            {
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

        $scope.save = function () {
            clearServerValidation();
            additionalSKUsValidatorClean();
            additionalSKUsValidatorFire();
            openSKUs();

            var valid = true;
            $.each($scope.forms, function (index, form) {
                if (form && !(typeof form === 'boolean')) {
                    if (!form.$valid && index != 'submitted' && index != 'skussubmitted' && index != 'crossessubmitted' && index != 'videossubmitted') {
                        valid = false;
                        $rootScope.activateTab($scope, index, serverValidationFormNameHandler);
                        return false;
                    }
                }
            });

            valid = valid && validateAutoShiplFrequency();

            if (valid)
            {
                var categoryIds = [];
                getSelected($scope.rootCategory, categoryIds);
                $scope.product.CategoryIds = categoryIds;
                //$scope.product.InventoryCategoryId = getInventorySelected($scope.rootInventoryCategory);
                updateCrossses();
                updateVideos();

                var data = angular.copy($scope.product);
                if (data.SKUs)
                {
                    $.each(data.SKUs, function (index, sku)
                    {
                        if (sku.BornDate)
                        {
                            if (sku.BornDate != sku.OriginBornDate)
                            {
                                sku.BornDate.setHours(0, 0, 0);
                            }
                            sku.BornDate = sku.BornDate.toServerDateTime();
                        }
                    });
                }

                productService.updateProduct(data, $scope.refreshTracker).success(function (result) {
                    successSaveHandler(result);
                }).error(function (result) {
                    errorHandler(result);
                });
            } else {
                $scope.forms.submitted = true;
                $scope.forms.skussubmitted = true;
                $scope.forms.crossessubmitted = true;
                $scope.forms.videossubmitted = true;
                toaster.pop('error', "Error!", $rootScope.baseValidationMessage, null, 'trustedHtml');
            }
        };

        $scope.updateCategoriesCollapsed = function (expand) {
            if (expand)
            {
                $scope.$broadcast('angular-ui-tree:expand-all', {name: 'public'});
                //scope.expandAll();
            }
            else
            {
                $scope.$broadcast('angular-ui-tree:collapse-all', {name: 'public'});
            }
            $scope.categoriesExpanded = expand;
        };

        $scope.updateSalesCategoriesCollapsed = function (expand) {
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

        $scope.sortableOptions = {
            handle: ' .sortable-move',
            items: ' .panel:not(.panel-heading)',
            axis: 'y',
            start: function (e, ui) {
                $scope.dragging = true;
            },
            stop: function (e, ui) {
                $scope.dragging = false;
            }
        }

        $scope.sortableOptionsSKU = {
            handle: ' .sortable-move',
            items: ' .panel:not(.panel-heading)',
            axis: 'y',
            start: function (e, ui) {
                $scope.dragging = true;
            },
            stop: function (e, ui) {
                $scope.dragging = false;
                var skus = [];
                $.each($scope.product.SKUs, function (index, item) {
                    var newItem = {};
                    angular.copy(item, newItem);
                    skus.push(newItem);
                });
                $scope.product.SKUs = [];
                $scope.product.SKUs = skus;
            }
        }

        $scope.toogleEditorState = function (property) {
            $scope[property] = !$scope[property];
        };

        var refreshPossiableProductTypes = function () {
            var types = [];
            if ($scope.product.Type == 1 || $scope.product.Type == 2) {
                $.each($scope.allTypes, function (index, item) {
                    if (item.Key == 1 || item.Key == 2) {
                        types.push(item);
                    }
                });
            }
            if ($scope.product.Type == 3 || $scope.product.Type == 4) {
                $.each($scope.allTypes, function (index, item) {
                    if (item.Key == 3 || item.Key == 4) {
                        types.push(item);
                    }
                });
            }
            $scope.types = types;
        };

        var setProductTypeWatch = function () {
            $scope.$watch('product.Type', function (newValue, oldValue) {
                if (newValue) {
                    $scope.sellers = getLookupValues(sellerFieldName, newValue);
                    $scope.defaultSeller = getLookupDefaultValue(sellerFieldName, newValue);
                    if (!$scope.defaultSeller) {
                        $scope.defaultSeller = 1;
                    }
                    $scope.googleCategories = getLookupValues(googleCategoryFieldName, newValue, true);
                    if (!$scope.product.GoogleCategory) {
                        $scope.product.GoogleCategory = getLookupDefaultValue(googleCategoryFieldName, newValue);
                    }
                    $scope.specialIcons = getLookupValues(specialIconFieldName, newValue, true);
                    if (!$scope.product.SpecialIcon) {
                        $scope.product.SpecialIcon = getLookupDefaultValue(specialIconFieldName, newValue);
                    }                    
                    $.each($scope.defaults, function (index, defaultItem) {                        
                        if (index == newValue) {
                            $scope.productTypeDefaults = defaultItem;
                            return false;
                        };
                    });
                }
            });
        };

        var getLookupValues = function (name, type, addNone) {
            var items = [];
            $.each($scope.lookups, function (index, lookup) {
                if (lookup.Name == name && lookup.Type == type) {
                    var baseItems = lookup.Items;
                    angular.copy(baseItems, items);
                    if (addNone) {
                        items.splice(0, 0, {
                            Key: null,
                            Text: 'None'
                        });
                    }
                }
            });
            return items;
        }

        var getLookupDefaultValue = function (name, type) {
            var defaultValue = null;
            $.each($scope.lookups, function (index, lookup) {
                if (lookup.Name == name && lookup.Type == type) {
                    defaultValue = lookup.DefaultValue;
                }
            });
            return defaultValue;
        }

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

        function expandInventoryCategory(id)
        {
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

        $scope.setCustomVideo = function (item) {
            item.VideoUse = true;
            item.ImageUse = true;
            item.TextUse = true;
            item.IsDefault = false;
        };

        $scope.removeCustomVideo = function (item) {
            item.VideoUse = false;
            item.ImageUse = false;
            item.TextUse = false;
            item.IsDefault = true;
        };

        $scope.setCustomCross = function (item) {
            item.ImageUse = true;
            item.UrlUse = true;
            item.IsDefault = false;
        };

        $scope.removeCustomCross = function (item) {
            item.ImageUse = false;
            item.UrlUse = false;
            item.IsDefault = true;
        };

        $scope.deleteSKU = function (index) {
            var skus = [];
            $.each($scope.product.SKUs, function (index, item) {
                var newItem = {};
                angular.copy(item, newItem);
                skus.push(newItem);
            });
            skus.splice(index, 1);
            $scope.product.SKUs = [];
            $scope.product.SKUs = skus;
        };

        $scope.goToMaster = function (id) {
        	$state.go('index.oneCol.masterDetail', { id: id });
        };

        $scope.addSKU = function () {
            clearServerValidation();
            additionalSKUsValidatorClean();
            additionalSKUsValidatorFire();
            var valid = openSKUs();

            if (!valid) {
                $scope.forms.skussubmitted = true;
                return false;
            }

            var sku = {
                Id: 0,
                Name: '',
                Active: true,
                RetailPrice: 0.00,
                WholesalePrice: 0.00,
                Stock: parseInt($scope.productTypeDefaults[stockFieldName]),
                DisregardStock: Boolean.parse($scope.productTypeDefaults[disregardStockFieldName]),
                NonDiscountable: Boolean.parse($scope.productTypeDefaults[nonDiscountableFieldName]),
                HideFromDataFeed: Boolean.parse($scope.productTypeDefaults[hideFromDataFeedFieldName]),
                QTYThreshold: $scope.productTypeDefaults[qTYThresholdFieldName] ? parseInt($scope.productTypeDefaults[qTYThresholdFieldName]) : null,
                Seller: $scope.defaultSeller,
                InventorySkus: [],
                IsOpen: true,
            };
            var skus = [];
            $.each($scope.product.SKUs, function (index, item) {
                var newItem = {};
                angular.copy(item, newItem);
                newItem.IsOpen = false;
                skus.push(newItem);
            });
            skus.splice(0, 0, sku);
            $scope.product.SKUs = [];
            $scope.product.SKUs = skus;

            $scope.forms.skussubmitted = false;
        };

        var validateAutoShiplFrequency = function ()
        {
            var isValid = true;
            $.each($scope.product.SKUs, function (index, subSku)
            {
                isValid = isValid && !(subSku.AutoShipProduct && !subSku.AutoShipFrequency1 && !subSku.AutoShipFrequency2 && !subSku.AutoShipFrequency3 && !subSku.AutoShipFrequency6);
            });
            return isValid;
        };

        var additionalSKUsValidatorFire = function () {
            $.each($scope.forms.SKUs, function (index, form) {
                if (form && index.indexOf('i') == 0 && form.Name != undefined) {
                    var itemIndex = parseInt(index.replace("i", ""));
                    if ($scope.product.SKUs[itemIndex] != undefined && $scope.product.SKUs[itemIndex].Name) {
                        var name = $scope.product.SKUs[itemIndex].Name;
                        $.each($scope.product.SKUs, function (index, item) {
                            if (itemIndex != index && item.Name && name.toLowerCase() == item.Name.toLowerCase()) {
                                form.Name.$setValidity("exist", false);
                            }
                        });
                    }
                }
            });
        };

        var additionalSKUsValidatorClean = function () {
            $.each($scope.forms.SKUs, function (index, form) {
                if (form && index.indexOf('i') == 0 && form.Name != undefined) {
                    form.Name.$setValidity("exist", true);
                }
            });
        };

        var openSKUs = function () {
            var valid = true;
            $.each($scope.forms.SKUs, function (index, form) {
                if (form && index.indexOf('i')==0 && !form.$valid) {
                    var itemIndex = parseInt(index.replace("i", ""));
                    if ($scope.product.SKUs[itemIndex] != undefined) {
                        $scope.product.SKUs[itemIndex].IsOpen = true;
                    }
                    valid = false;
                }
            });
            return valid;
        };

        var initCrossses = function () {
            $.each($scope.product.CrossSellProducts, function (index, item) {
                if (item.Image || item.Url)
                {
                    item.ImageUse = true;
                    item.UrlUse = true;
                }
            });
        };

        var updateCrossses = function () {
            $.each($scope.product.CrossSellProducts, function (index, item) {
                if (!item.ImageUse) {
                    item.Image = null;
                }
                if (!item.UrlUse) {
                    item.Url = null;
                }
            });
        };

        var initVideos = function () {
            $.each($scope.product.Videos, function (index, item) {
                if (item.Video || item.Image || item.Text)
                {
                    item.VideoUse = true;
                    item.ImageUse = true;
                    item.TextUse = true;
                }
            });
        };

        var updateVideos = function () {
            $.each($scope.product.Videos, function (index, item) {
                if (!item.VideoUse) {
                    item.Video = null;
                }
                if (!item.ImageUse) {
                    item.Image = null;
                }
                if (!item.TextUse) {
                    item.Text = null;
                }
            });
        };

        $scope.autoShipProductClick = function (item)
        {
            if (item.AutoShipProduct)
            {
                item.AutoShipFrequency1 = true;
                item.AutoShipFrequency2 = true;
                item.AutoShipFrequency3 = true;
                item.AutoShipFrequency6 = true;
            }
        };

        $scope.toggleOpen = function (item, event) {
            item.IsOpen = !item.IsOpen;
        };

        $scope.assignInventorySkus = function(sku)
        {
            modalUtil.open('app/modules/inventorysku/partials/assignInventorySkusPopup.html', 'assignInventorySkusController', {
                assignedItems: sku.InventorySkus, thenCallback: function (data)
                {
                    sku.InventorySkus = data;
                }
            }, { size: 'sm' });
            event.stopPropagation();
            return false;
        };

        initialize();
    }
]);