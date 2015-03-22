'use strict';

angular.module('app.modules.demo.controllers.productDetailController', [])
	.controller('productDetailController', [
		'$scope', function($scope) {
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

			$scope.toogleEditorState = function(property) {
				$scope[property] = !$scope[property];
			};

			$scope.categoriesContext = {
				selectedNodes: []
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

			$scope.categoriesOptions = {
				onSelect: function ($event, node, context) {
					if ($event.ctrlKey) {
						var idx = context.selectedNodes.indexOf(node);
						if (context.selectedNodes.indexOf(node) === -1) {
							context.selectedNodes.push(node);
						} else {
							context.selectedNodes.splice(idx, 1);
						}
					} else {
						context.selectedNodes = [node];
					}
				}
			};

			$scope.subSkus = [
				{
					SKU: 'FRP006',
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
					SKU: 'FRP007',
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
					SKU: 'FRP008',
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
					SKU: 'FRP009',
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
					SKU: 'FRP010',
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

			$scope.sortableOptions = {
				handle: ' .sortable-move',
				 items: ' .panel:not(.panel-heading)',
				axis: 'y'
			}
	}
	]);