'use strict';

angular.module('app.modules.product.controllers.productManageController', [])
.controller('productManageController', ['$scope', '$rootScope', '$state', '$stateParams', '$modal', 'productService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, $modal, productService, toaster, confirmUtil, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.id = result.Data.Id;
            } else {
                var messages = "";
                if (result.Messages) {
                    $scope.forms.submitted = true;
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
            $scope.forms = {};
            $scope.baseUrl = $rootScope.ReferenceData.PublicHost + 'product/{0}?preview=true';
            $scope.previewUrl = null;
            $scope.types = $rootScope.ReferenceData.ProductTypes;
            $scope.googleCategories = [];
            $scope.specialIcons = [];

            $scope.id = $stateParams.id;

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
			        } else {
			            errorHandler(result);
			        }
			    }).
			    error(function (result) {
			        errorHandler(result);
			    });
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

            $scope.product.SKUs = [
            {
                Order: 1,
                Id: '43454',
                Name: 'FRP006',
                Active: true,
                RetailPrice: 55.00,
                WholesalePrice: 9.00,
                UnitsToMake: 1,
                Stock: 5,
                DisregardStock: true,
                AllowBackOrder: false,
                ShipsWithin: true,
                CrossSellingItem: false
            },
            {
                Order: 2,
                Id: '12312',
                Name: 'FRP007',
                Active: true,
                RetailPrice: 79.00,
                WholesalePrice: 79.00,
                UnitsToMake: 1,
                Stock: 2,
                DisregardStock: true,
                AllowBackOrder: false,
                ShipsWithin: false,
                ShipsWithinDuration: 1,
                CrossSellingItem: false,
                AutoShipProduct: true,
                AutoShipFrequencyAllowed: []
            },
            ];
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

        $scope.save = function () {
            $.each($scope.forms, function (index, form) {
                $.each(form, function (index, element) {
                    if (element.$name == index) {
                        element.$setValidity("server", true);
                    }
                });
            });

            var valid = true;
            $.each($scope.forms, function (index, form) {
                if (!form.$valid && index != 'submitted') {
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
            }
        };

        $scope.toggleOpen = function (item, event)
        {
            var itemElement = $(event.target).closest('fieldset').find('.panel-collapse').get(0);
            $.each($('.sortable-accordion .panel-collapse'), function (index, element) {
                if (itemElement != element && $(element).css('display') == 'block') {
                    $(element).slideToggle();
                }
            });
            $(itemElement).slideToggle();
        };

        initialize();
    }
]);