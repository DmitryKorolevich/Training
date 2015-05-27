'use strict';

angular.module('app.modules.product.controllers.productManageController', [])
.controller('productManageController', ['$scope', '$rootScope', '$state', '$stateParams', '$modal', 'productService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, $modal, productService, toaster, confirmUtil, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

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
                    $scope.serverMessages = new ServerMessages(result.Messages);
                    var formForShowing=null;
                    $.each(result.Messages, function (index, value) {
                        if (value.Field) {
                            if (value.Field.indexOf('.') > -1) {
                                var items = value.Field.split(".");
                                $scope.forms[items[0]][items[1]].$setValidity("server", false);
                            }
                            else {
                                $.each($scope.forms, function (index, form) {
                                    if(form[value.Field]!=undefined)
                                    {                                        
                                        form[value.Field].$setValidity("server", false);
                                        if(formForShowing==null)
                                        {
                                            formForShowing = index;
                                        }
                                        return false;
                                    }
                                });
                            }
                        }
                    });

                    if(formForShowing)
                    {
                        activateTab(formForShowing.$name);
                    }
                }
                toaster.pop('error', "Error!", messages, null, 'trustedHtml');
            }
        };

        function errorHandler(result) {
            toaster.pop('error', "Error!", "Server error occured");
        };

        $scope.deleteSKU = function (index)
        {
            $scope.product.SKUs.splice(index, 1);
        };

        $scope.addSKU = function () {

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
                IsOpen: true,
            };
            var skus = [];
            $.each($scope.product.SKUs, function (index, item)
            {
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

        $scope.sortableOptions = {
            handle: ' .sortable-move',
            items: ' .panel:not(.panel-heading)',
            axis: 'y',
            start: function (e, ui) { $scope.dragging = true; },
            stop: function (e, ui) { $scope.dragging = false; }
        }

        $scope.toogleEditorState = function (property) {
            $scope[property] = !$scope[property];
        };

        function initialize() {
            $scope.id = $stateParams.id;

            $scope.forms = {};
            $scope.baseUrl = $rootScope.ReferenceData.PublicHost + 'product/{0}?preview=true';
            $scope.previewUrl = null;
            $scope.allTypes = $rootScope.ReferenceData.ProductTypes;
            $scope.googleCategories = [];
            $scope.specialIcons = [];
            $scope.sellers = [];
            $scope.orphanTypes = [];
            
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

            loadCategories();            
        };

        function loadCategories()
        {
            productService.getCategoriesTree({ },$scope.refreshTracker)
                .success(function (result) {
                    if (result.Success) {
                        $scope.rootCategory=result.Data;
                        if($scope.id)
                        {
                            loadProduct();
                        }
                        else
                        {
                            $scope.types = $scope.allTypes;
                            createNewProduct();
                        }
                    } else {
                        errorHandler(result);
                    }
                }).
                error(function(result) {
                    errorHandler(result);
                });
        };

        function loadProduct()
        {
            productService.getProduct($scope.id, $scope.refreshTracker)
			    .success(function (result) {
			        if (result.Success) {
			            $scope.product = result.Data;                        
			            $scope.product.StatusCode = "" + $scope.product.StatusCode;
			            setSelected($scope.rootCategory, $scope.product.CategoryIds);
			            refreshPossiableProductTypes();
			        } else {
			            errorHandler(result);
			        }
			    }).
			    error(function (result) {
			        errorHandler(result);
			    });
        };

        var refreshPossiableProductTypes = function()
        {
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

        function setSelected(category, ids) {
            category.IsSelected = false;
            $.each(ids, function( index, id ) {
                if(category.Id==id)
                {
                    category.IsSelected=true;
                }
            });
            $.each(category.SubItems, function( index, value ) {
                setSelected(value, ids);
            });
        };

        function getSelected(category , ids){
            if(category.IsSelected)
            {
                ids.push(category.Id);
            }
            $.each(category.SubItems, function( index, value ) {
                getSelected(value, ids);
            });
        };

        function createNewProduct()
        {            
            $scope.product = {};
            $scope.product.StatusCode='2';//Active
            $scope.product.Hidden = false;
            $scope.product.Type = 1;//Perishable

            $scope.product.SKUs = [];
        };

        function activateTab(formName)
        {            
            $.each($scope.tabs, function (index, item)
            {
                if (formName.indexOf('SKUs') == 0)
                {
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
        }

        var additionalSKUsValidatorFire = function ()
        {
            $.each($scope.forms, function (index, form) {
                if (index.indexOf('SKUs') == 0 && form.Name != undefined) {
                    var itemIndex = parseInt(index.replace("SKUs", ""));
                    if ($scope.product.SKUs[itemIndex]!=undefined && $scope.product.SKUs[itemIndex].Name) {
                        var name = $scope.product.SKUs[itemIndex].Name;
                        $.each($scope.product.SKUs, function (index, item)
                        {
                            if (itemIndex != index && name.toLowerCase() == item.Name.toLowerCase())
                            {
                                form.Name.$setValidity("exist", false);
                            }
                        });
                    }
                }
            });
        };

        var additionalSKUsValidatorClean = function () {
            $.each($scope.forms, function (index, form) {
                if (index.indexOf('SKUs') == 0 && form.Name!=undefined) {
                    form.Name.$setValidity("exist", true);
                }
            });
        };

        var openSKUs = function ()
        {
            var valid = true;
            $.each($scope.forms, function (index, form) {
                if (index.indexOf('SKUs') == 0 && !form.$valid) {
                    var itemIndex = parseInt(index.replace("SKUs", ""));
                    if ($scope.product.SKUs[itemIndex] != undefined) {
                        $scope.product.SKUs[itemIndex].IsOpen = true;
                    }
                    valid = false;
                }
            });
            return valid;
        };

        $scope.save = function () {
            $.each($scope.forms, function (index, form) {
                $.each(form, function (index, element) {
                    if (element.$name == index) {
                        element.$setValidity("server", true);
                    }
                });
            });
            additionalSKUsValidatorClean();
            additionalSKUsValidatorFire();
            openSKUs();

            var valid = true;
            $.each($scope.forms, function (index, form) {
                if (!form.$valid && index != 'submitted' && index != 'skussubmitted') {
                    valid = false;                          
                    activateTab(index);
                    return false;
                }
            });

            if (valid) {                
                var categoryIds = [];
                getSelected($scope.rootCategory, categoryIds);
                $scope.product.CategoryIds = categoryIds;

                productService.updateProduct($scope.product, $scope.editTracker).success(function (result) {
                    successSaveHandler(result);
                }).error(function (result) {
                    errorHandler(result);
                });
            } else {
                $scope.forms.submitted = true;
                $scope.forms.skussubmitted = true;
            }
        };

        $scope.toggleOpen = function (item, event)
        {
            item.IsOpen = !item.IsOpen;
        };

        initialize();
    }
]);