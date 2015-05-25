'use strict';

angular.module('app.modules.product.controllers.productManageController', [])
.controller('productManageController', ['$scope', '$rootScope', '$state', '$stateParams', '$modal', 'productService', 'toaster', 'confirmUtil', 'promiseTracker',
    function ($scope, $rootScope, $state, $stateParams, $modal, productService, toaster, confirmUtil, promiseTracker) {
        $scope.refreshTracker = promiseTracker("get");
        $scope.editTracker = promiseTracker("edit");

        $scope.description = '<table width="100%" border="0" cellspacing="0" cellpadding="0">   <tbody><tr>     <td valign="top"><br>Our Electronic Gift Certificates make great last-minute gifts! <br><br>Simply add the Gift Certificate of your choice to your cart, then enter the recipient\'s name and email address (not yours) into the appropriate "SHIP TO" fields during checkout. <br><br>The Gift Certificate will be delivered to your chosen recipient by email shortly after the completion of your order.<br><br><span style="font-weight: bold; ">IMPORTANT</span><br>There are no shipping or handling charges for e-Gift Certificates.<br>The e-Gift Certificate will be sent to the Name and Email address you provide in the "SHIP TO" section during checkout.<br><br>Do not select the "Use my billing address" auto-fill option unless you wish to receive the certificate yourself.<br><br>If your order contains no other items, you may simply enter "N/A" in the mandatory address fields.<br><br>If you\'re uncertain of the recipient\'s email address, you may send the e-Gift to yourself and forward when convenient.<br><br>Questions? Call us anytime at (800) 608-4825<br><br></td>        </tr> </tbody></table> ';
        $scope.descriptionExpanded = false;

        $scope.servicingStorage = '<table width="100%" border="0" cellspacing="0" cellpadding="0">   <tbody><tr>     <td valign="top"><br>Our Electronic Gift Certificates make great last-minute gifts! <br><br>Simply add the Gift Certificate of your choice to your cart, then enter the recipient\'s name and email address (not yours) into the appropriate "SHIP TO" fields during checkout. <br><br>The Gift Certificate will be delivered to your chosen recipient by email shortly after the completion of your order.<br><br><span style="font-weight: bold; ">IMPORTANT</span><br>There are no shipping or handling charges for e-Gift Certificates.<br>The e-Gift Certificate will be sent to the Name and Email address you provide in the "SHIP TO" section during checkout.<br><br>Do not select the "Use my billing address" auto-fill option unless you wish to receive the certificate yourself.<br><br>If your order contains no other items, you may simply enter "N/A" in the mandatory address fields.<br><br>If you\'re uncertain of the recipient\'s email address, you may send the e-Gift to yourself and forward when convenient.<br><br>Questions? Call us anytime at (800) 608-4825<br><br></td>        </tr> </tbody></table> ';
        $scope.servicingStorageExpanded = false;

        $scope.short = '<table width="100%" border="0" cellspacing="0" cellpadding="0">   <tbody><tr>     <td valign="top"><br>Our Electronic Gift Certificates make great last-minute gifts! <br><br>Simply add the Gift Certificate of your choice to your cart, then enter the recipient\'s name and email address (not yours) into the appropriate "SHIP TO" fields during checkout. <br><br>The Gift Certificate will be delivered to your chosen recipient by email shortly after the completion of your order.<br><br><span style="font-weight: bold; ">IMPORTANT</span><br>There are no shipping or handling charges for e-Gift Certificates.<br>The e-Gift Certificate will be sent to the Name and Email address you provide in the "SHIP TO" section during checkout.<br><br>Do not select the "Use my billing address" auto-fill option unless you wish to receive the certificate yourself.<br><br>If your order contains no other items, you may simply enter "N/A" in the mandatory address fields.<br><br>If you\'re uncertain of the recipient\'s email address, you may send the e-Gift to yourself and forward when convenient.<br><br>Questions? Call us anytime at (800) 608-4825<br><br></td>        </tr> </tbody></table> ';
        $scope.shortExpanded = false;

        $scope.recepies = '<table width="100%" border="0" cellspacing="0" cellpadding="0">   <tbody><tr>     <td valign="top"><br>Our Electronic Gift Certificates make great last-minute gifts! <br><br>Simply add the Gift Certificate of your choice to your cart, then enter the recipient\'s name and email address (not yours) into the appropriate "SHIP TO" fields during checkout. <br><br>The Gift Certificate will be delivered to your chosen recipient by email shortly after the completion of your order.<br><br><span style="font-weight: bold; ">IMPORTANT</span><br>There are no shipping or handling charges for e-Gift Certificates.<br>The e-Gift Certificate will be sent to the Name and Email address you provide in the "SHIP TO" section during checkout.<br><br>Do not select the "Use my billing address" auto-fill option unless you wish to receive the certificate yourself.<br><br>If your order contains no other items, you may simply enter "N/A" in the mandatory address fields.<br><br>If you\'re uncertain of the recipient\'s email address, you may send the e-Gift to yourself and forward when convenient.<br><br>Questions? Call us anytime at (800) 608-4825<br><br></td>        </tr> </tbody></table> ';
        $scope.recepiesExpanded = false;

        $scope.ingredients = '<table width="100%" border="0" cellspacing="0" cellpadding="0">   <tbody><tr>     <td valign="top"><br>Our Electronic Gift Certificates make great last-minute gifts! <br><br>Simply add the Gift Certificate of your choice to your cart, then enter the recipient\'s name and email address (not yours) into the appropriate "SHIP TO" fields during checkout. <br><br>The Gift Certificate will be delivered to your chosen recipient by email shortly after the completion of your order.<br><br><span style="font-weight: bold; ">IMPORTANT</span><br>There are no shipping or handling charges for e-Gift Certificates.<br>The e-Gift Certificate will be sent to the Name and Email address you provide in the "SHIP TO" section during checkout.<br><br>Do not select the "Use my billing address" auto-fill option unless you wish to receive the certificate yourself.<br><br>If your order contains no other items, you may simply enter "N/A" in the mandatory address fields.<br><br>If you\'re uncertain of the recipient\'s email address, you may send the e-Gift to yourself and forward when convenient.<br><br>Questions? Call us anytime at (800) 608-4825<br><br></td>        </tr> </tbody></table> ';
        $scope.ingredientsExpanded = false;

        $scope.notes = '<table width="100%" border="0" cellspacing="0" cellpadding="0">   <tbody><tr>     <td valign="top"><br>Our Electronic Gift Certificates make great last-minute gifts! <br><br>Simply add the Gift Certificate of your choice to your cart, then enter the recipient\'s name and email address (not yours) into the appropriate "SHIP TO" fields during checkout. <br><br>The Gift Certificate will be delivered to your chosen recipient by email shortly after the completion of your order.<br><br><span style="font-weight: bold; ">IMPORTANT</span><br>There are no shipping or handling charges for e-Gift Certificates.<br>The e-Gift Certificate will be sent to the Name and Email address you provide in the "SHIP TO" section during checkout.<br><br>Do not select the "Use my billing address" auto-fill option unless you wish to receive the certificate yourself.<br><br>If your order contains no other items, you may simply enter "N/A" in the mandatory address fields.<br><br>If you\'re uncertain of the recipient\'s email address, you may send the e-Gift to yourself and forward when convenient.<br><br>Questions? Call us anytime at (800) 608-4825<br><br></td>        </tr> </tbody></table> ';
        $scope.notesExpanded = false;

        $scope.toogleEditorState = function (property) {
            $scope[property] = !$scope[property];
        };

        $scope.toggle = function (scope) {
            scope.toggle();
        };

        $scope.hasChildren = function (scope) {
            return scope.childNodesCount() > 0;
        };

        $scope.categories = [
			{
			    label: 'Canned & Pouched Wild Seafood',
			    status: 1,
			    checked: false,
			    children: [
					{
					    label: 'Wild Canned Sockeye Salmon',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Canned Wild Albacore Tuna (Troll-Caught)',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Canned Wild Portuguese Sardines',
					    status: 1,
					    checked: true
					},
					{
					    label: 'Canned Wild Pacific Dungeness Crab',
					    status: 2,
					    checked: false
					},
					{
					    label: 'Smoked Mussels (Cultured)',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Pouched Wild Tuna &amp; Salmon',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Canned Wild Portuguese Mackerel',
					    status: 1,
					    checked: true
					},
					{
					    label: 'Canned Wild Seafood Samplers',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Canned Wild Fish - No Salt Added',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Canned Wild Portuguese Sardine Fillets',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Canned Wild Fish - Kosher',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Canned Wild Salmon Meal Kit',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Pouched Wild Albacore Tuna (Troll-Caught)',
					    status: 1,
					    checked: false
					}
			    ]
			}, {
			    label: 'Cookbooks & Cooking Accessories',
			    status: 1,
			    checked: false,
			    children: [
					{
					    label: 'Favorite Books & Cookbooks',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Grilling Planks',
					    status: 1,
					    checked: true
					}, {
					    label: 'Canned Fish Accessories',
					    status: 1,
					    checked: false
					}, {
					    label: 'Signature Water Bottle',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Vital Choice Apron',
					    status: 1,
					    checked: true
					},
					{
					    label: 'Tea Accessories',
					    status: 1,
					    checked: false
					}
			    ]
			}, {
			    label: 'Frozen Seafood Samplers',
			    status: 1,
			    checked: true
			},
			{
			    label: 'Organic Foods & Grass-Fed Beef',
			    status: 1,
			    checked: false,
			    children: [
					{
					    label: 'Farmhouse Culture Kraut & Kimchi',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Organic Grass-Fed Beef by Skagit River Ranch',
					    status: 1,
					    checked: false,
					    children: [
							{
							    label: 'Organic Grass-Fed Wagyu Beef Chuck Roast',
							    status: 1,
							    checked: false
							},
							{
							    label: 'Organic Grass-Fed Wagyu Beef Burger Patties - 5.3 oz',
							    status: 1,
							    checked: true
							},
							{
							    label: 'Organic Grass-Fed Wagyu Beef Tenderloin Fillets',
							    status: 1,
							    checked: false
							},
							{
							    label: 'Organic Grass-Fed Wagyu Beef Sampler',
							    status: 2,
							    checked: false
							},
							{
							    label: 'Organic Grass-Fed Wagyu Beef - Ground',
							    status: 1,
							    checked: false
							},
							{
							    label: 'Organic Grass-Fed Wagyu Beef Stir-Fry Strips',
							    status: 1,
							    checked: false
							},
							{
							    label: 'Organic Grass-Fed Wagyu Beef Ribeye Steaks',
							    status: 1,
							    checked: false
							}
					    ]
					},
					{
					    label: 'Paleo Choices',
					    status: 2,
					    checked: false
					},
					{
					    label: 'Seaweed Salad',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Organic Nuts - Raw and Roasted',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Organic Extra Dark Chocolate',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Organic Berries - Frozen',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Organic Oils & Vinegar',
					    status: 1,
					    checked: false,
					    children: [
							{
							    label: 'Organic Cooking Oils',
							    status: 1,
							    checked: false
							},
							{
							    label: 'Organic Balsamic Vinegar',
							    status: 1,
							    checked: false
							}
					    ]
					},
					{
					    label: 'Soups & Cioppino',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Salmon Meal Kits',
					    status: 1,
					    checked: false
					}
			    ]
			},
			{
			    label: 'Organic Nuts & Dried Fruit',
			    status: 2,
			    checked: false
			},
			{
			    label: 'Pet Products',
			    status: 1,
			    checked: false
			},
			{
			    label: 'Product Sampler Packs',
			    status: 1,
			    checked: false
			},
			{
			    label: 'Red + White Fish Combo Packs',
			    status: 1,
			    checked: false
			},
			{
			    label: 'Soups & Meal Kits',
			    status: 1,
			    checked: false
			},
			{
			    label: 'System Items',
			    status: 1,
			    checked: false
			},
			{
			    label: 'VC Missing Import Items',
			    status: 2,
			    checked: false
			},
			{
			    label: 'Wholesale',
			    status: 1,
			    checked: false,
			    children: [
					{
					    label: 'Canned Wild Seafood',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Dietary Supplements',
					    status: 1,
					    checked: false
					},
					{
					    label: 'Organic Foods',
					    status: 1,
					    checked: false
					}
			    ]
			},
			{
			    label: 'Salmon Heads for Stock & Soup',
			    status: 1,
			    checked: true
			}
        ];

        $scope.sortableOptions = {
            handle: ' .sortable-move',
            items: ' .panel:not(.panel-heading)',
            axis: 'y',
            start: function (e, ui) { $scope.dragging = true; },
            stop: function (e, ui) { $scope.dragging = false; }
        }

        $scope.open = function () {

            var modalInstance = $modal.open({
                templateUrl: 'app/modules/demo/partials/addSubProduct.html',
                controller: 'modalAddSubProductController',
                size: 'lg',
                resolve: {
                    items: function () {
                        return $scope.items;
                    }
                }
            });

            modalInstance.result.then(function (selectedItem) {
                $scope.selected = selectedItem;
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        function initialize() {
            $scope.forms = {};

            $scope.product = {};
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
            {
                Order: 3,
                Id: '44444',
                Name: 'FRP008',
                Active: true,
                RetailPrice: 56.00,
                WholesalePrice: 71.00,
                UnitsToMake: 1,
                Stock: 51,
                DisregardStock: true,
                AllowBackOrder: false,
                ShipsWithin: false,
                ShipsWithinDuration: 8,
                CrossSellingItem: false,
                AutoShipProduct: true,
                AutoShipFrequencyAllowed: []
            },
            {
                Order: 4,
                Id: '5555',
                Name: 'FRP009',
                Active: true,
                RetailPrice: 6.00,
                WholesalePrice: 7.00,
                UnitsToMake: 9,
                Stock: 1,
                DisregardStock: true,
                AllowBackOrder: true,
                ShipsWithin: true,
                ShipsWithinDuration: 23,
                CrossSellingItem: true,
                AutoShipProduct: true,
                AutoShipFrequencyAllowed: []
            },
            {
                Order: 5,
                Id: '12323',
                Name: 'FRP010',
                Active: true,
                RetailPrice: 6.35,
                WholesalePrice: 71.00,
                UnitsToMake: 9,
                Stock: 10,
                DisregardStock: true,
                AllowBackOrder: true,
                ShipsWithin: true,
                ShipsWithinDuration: 23,
                CrossSellingItem: false,
                AutoShipProduct: true,
                AutoShipFrequencyAllowed: []
            }
            ];
        };

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
                    return false;
                }
            });

            if (valid) {
                productService.updateProduct($scope.product, $scope.editTracker).success(function (result) {
                    successSaveHandler(result);
                }).error(function (result) {
                    errorHandler(result);
                });
            } else {
                $scope.forms.submitted = true;
            }
        };

        $scope.toggleOpen = function (item)
        {
            if (item.isOpen!=undefined) {
                item.isOpen = !item.isOpen;
            }
            else
            {
                item.isOpen = true;
            }
        };

        function successSaveHandler(result) {
            if (result.Success) {
                toaster.pop('success', "Success!", "Successfully saved.");
                $scope.id = result.Data.Id;
            } else {
                var messages = "";
                if (result.Messages) {
                    $scope.forms.submitted = true;
                    $scope.serverMessages = new ServerMessages(result.Messages);
                    $.each(result.Messages, function (index, value) {
                        if (value.Field) {
                            if (value.Field.indexOf('.') > -1) {
                                var items = value.Field.split(".");
                                $scope.forms[items[0]][items[1]].$setValidity("server", false);
                            }
                            else {
                                $scope.forms.mainForm[value.Field].$setValidity("server", false);
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

        initialize();
    }
]);