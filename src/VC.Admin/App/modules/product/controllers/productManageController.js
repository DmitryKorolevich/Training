'use strict';

angular.module('app.modules.product.controllers.productManageController', [])
.controller('productManageController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', '$modal', 'productService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, $timeout, $modal, productService, toaster, confirmUtil, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        var sellerFieldName = 'Seller';
        var googleCategoryFieldName = 'GoogleCategory';
        var specialIconFieldName = 'SpecialIcon';

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.product.Id = result.Data.Id;
                refreshPossiableProductTypes();
            } else {
                var messages = "";
                if (result.Messages) {
                    $scope.forms.submitted = true;
                    $scope.forms.skussubmitted = true;
                    $scope.forms.crossessubmitted = true;
                    $scope.forms.videossubmitted = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);
                    var formForShowing = null;
                    $.each(result.Messages, function (index, value) {
                        if (value.Field) {
                            if (value.Field.indexOf('.') > -1) {
                                var items = value.Field.split(".");
                                $scope.forms[items[0]][items[1]][items[2]].$setValidity("server", false);
                                formForShowing = items[0];
                                openSKUs();
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

                    if (formForShowing) {
                        activateTab(formForShowing);
                    }
                }
                toaster.pop('error', "Error!", messages, null, 'trustedHtml');
            }
        };

        function errorHandler(result) {
            toaster.pop('error', "Error!", "Server error occured");
        };

        function initialize() {
            $scope.id = $stateParams.id;

            $scope.forms = {};
            $scope.youTubeBaseUrl = 'https://www.youtube.com/watch?v={0}'
            $scope.basePreviewUrl = $rootScope.ReferenceData.PublicHost + 'product/{0}?preview=true';
            $scope.baseUrl = $rootScope.ReferenceData.PublicHost.substring(0, $rootScope.ReferenceData.PublicHost.length - 1) + '{0}';
            $scope.previewUrl = null;
            $scope.allTypes = $rootScope.ReferenceData.ProductTypes;
            $scope.googleCategories = [];
            $scope.specialIcons = [];
            $scope.sellers = [];
            $scope.defaultSeller = null;

            $scope.defaults = {};
            $scope.defaults.CrossSells = [
            {
                Image: '/some1.png',
                Url: 'http://someurl.com/1',
            },
            {
                Image: '/some2.png',
                Url: 'http://someurl.com/2',
            },
            {
                Image: '/some3.png',
                Url: 'http://someurl.com/3',
            },
            {
                Image: '/some4.png',
                Url: 'http://someurl.com/4',
            }];
            $scope.defaults.Videos = [
            {
                Video: 'jGwOsFo8TTg',
                Image: '/some1.png',
                Text: 'Some text1',
            },
            {
                Video: 'btlfoO75kfI',
                Image: '/some2.png',
                Text: 'Some text2',
            },
            {
                Video: 'vCsRTamxWuw',
                Image: '/some3.png',
                Text: 'Some text3',
            }];

            $scope.parentDetailsTab = {
                active: true,
                formName: 'parentDetails',
            };
            $scope.imagesTab = {
                active: false,
                formName: 'images',
            };
            $scope.subProductsTab = {
                active: false,
                formName: 'SKUs',
            };
            $scope.nutritionalTab = {
                active: false,
                formName: 'nutritional',
            };
            $scope.categoriesTab = {
                active: false,
                formName: 'categories',
            };
            $scope.crossSellProductsAndVideosTab = {
                active: false,
                formName: 'crossSellProductsAndVideos',
            };
            $scope.inventoryAndShippingTab = {
                active: false,
                formName: 'inventoryAndShipping',
            };
            var tabs = [];
            tabs.push($scope.parentDetailsTab);
            tabs.push($scope.imagesTab);
            tabs.push($scope.subProductsTab);
            tabs.push($scope.nutritionalTab);
            tabs.push($scope.categoriesTab);
            tabs.push($scope.crossSellProductsAndVideosTab);
            tabs.push($scope.inventoryAndShippingTab);
            $scope.tabs = tabs;

            loadLookups();
        };

        function loadLookups() {
            productService.getProductLookups($scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.lookups = result.Data;
                        loadCategories();
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
                        if ($scope.id) {
                            loadProduct();
                        }
                        else {
                            createNewProduct();
                            refreshPossiableProductTypes();
                            setProductTypeWatch();
                        }
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function (result) {
                    errorHandler(result);
                });
        };

        function loadProduct() {
            productService.getProduct($scope.id, $scope.refreshTracker)
			    .success(function (result) {
			        if (result.Success) {
			            $scope.product = result.Data;
			            $scope.product.StatusCode = $scope.product.StatusCode;
			            setSelected($scope.rootCategory, $scope.product.CategoryIds);
			            refreshPossiableProductTypes();
			            setProductTypeWatch();
			            initCrossses();
			            initVideos();
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
                        activateTab(index);
                        return false;
                    }
                }
            });

            if (valid) {
                var categoryIds = [];
                getSelected($scope.rootCategory, categoryIds);
                $scope.product.CategoryIds = categoryIds;
                updateCrossses();
                updateVideos();

                productService.updateProduct($scope.product, $scope.editTracker).success(function (result) {
                    successSaveHandler(result);
                }).error(function (result) {
                    errorHandler(result);
                });
            } else {
                $scope.forms.submitted = true;
                $scope.forms.skussubmitted = true;
                $scope.forms.crossessubmitted = true;
                $scope.forms.videossubmitted = true;
            }
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

        function createNewProduct() {
            $scope.product = {};
            $scope.product.StatusCode = 2;//Active
            $scope.product.Hidden = false;
            if ($stateParams.type) {
                $scope.product.Type = $stateParams.type;
            }
            else {
                $scope.product.Type = 2;//Perishable
            }

            $scope.product.SKUs = [];
            $scope.product.CrossSellProducts = [
            {
                Image: null, ImageUse: false,
                Url: null, UrlUse: false,
            },
            {
                Image: null, ImageUse: false,
                Url: null, UrlUse: false,
            },
            {
                Image: null, ImageUse: false,
                Url: null, UrlUse: false,
            },
            {
                Image: null, ImageUse: false,
                Url: null, UrlUse: false,
            }];
            $scope.product.Videos = [
            {
                Video: null, VideoUse: false,
                Image: null, ImageUse: false,
                Text: null, TextUse: false,
            },
            {
                Video: null, VideoUse: false,
                Image: null, ImageUse: false,
                Text: null, TextUse: false,
            },
            {
                Video: null, VideoUse: false,
                Image: null, ImageUse: false,
                Text: null, TextUse: false,
            }];
        };

        function activateTab(formName) {
            $.each($scope.tabs, function (index, item) {
                if (formName.indexOf('SKUs') == 0) {
                    formName = 'SKUs';
                }
                if (formName.indexOf('CrossSellProducts') == 0) {
                    formName = 'crossSellProductsAndVideos';
                }
                if (formName.indexOf('Videos') == 0) {
                    formName = 'crossSellProductsAndVideos';
                }
                if (item.formName == formName) {
                    item.active = true;
                    return false;
                }
            });
        };

        $scope.setCustomVideo = function (item) {
            item.VideoUse = true;
            item.ImageUse = true;
            item.TextUse = true;
        };

        $scope.removeCustomVideo = function (item) {
            item.VideoUse = false;
            item.ImageUse = false;
            item.TextUse = false;
        };

        $scope.setCustomCross = function (item) {
            item.ImageUse = true;
            item.UrlUse = true;
        };

        $scope.removeCustomCross = function (item) {
            item.ImageUse = false;
            item.UrlUse = false;
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
                Id: null,
                Name: '',
                Active: true,
                RetailPrice: 0.00,
                WholesalePrice: 0.00,
                Stock: null,
                DisregardStock: true,
                NonDiscountable: true,
                HideFromDataFeed: true,
                Seller: $scope.defaultSeller,
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
                if (item.Image) {
                    item.ImageUse = true;
                }
                if (item.Url) {
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
                if (item.Video) {
                    item.VideoUse = true;
                }
                if (item.Image) {
                    item.ImageUse = true;
                }
                if (item.Text) {
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

        $scope.toggleOpen = function (item, event) {
            item.IsOpen = !item.IsOpen;
        };

        initialize();
    }
]);