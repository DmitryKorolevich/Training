'use strict';

angular.module('app.modules.demo.controllers.manageRecipesController', [])
.controller('manageRecipesController', ['$scope', function ($scope) {
/*Ace Editor*/
	$scope.editorModel = '@*----Output top misc----*@\n' +
		'<div style="width:1000px; margin:auto auto;">\n' +
		'<img src="http://www.vitalchoice.com/shop/pc/catalog/kitchen_portal/in-the-kitchen-header-4-24-14a.png" width="955" height="90">\n' +
		'</div>\n' +
		'<%\n' +
		'<body:body>\n' +
		'{{\n' +
		'<strong>This is individual body</strong>\n' +
		'}}\n' +
		'<left:left>\n' +
		'{{\n' +
		'@left()\n' +
		'{{\n' +
		'<strong>This is individual left</strong>\n' +
		'}}\n' +
		'}}\n' +
		'<right:right>\n' +
		'{{\n' +
		'@right()\n' +
		'{{\n' +
		'<strong>This is individual right</strong>\n' +
		'}}\n' +
		'}}\n' +
		'%>\n' +
		'@default()';


/*Categories*/
	$scope.toggle = function(scope) {
		scope.toggle();
	};

	$scope.hasChildren = function(scope) {
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
					checked: false,
					children: []
				},
				{
					label: 'Canned Wild Albacore Tuna (Troll-Caught)',
					status: 1,
					checked: false,
					children: []
				},
				{
					label: 'Canned Wild Portuguese Sardines',
					status: 1,
					checked: true,
					children: []
				},
				{
					label: 'Canned Wild Pacific Dungeness Crab',
					status: 2,
					checked: false,
					children: []
				},
				{
					label: 'Smoked Mussels (Cultured)',
					status: 1,
					checked: false,
					children: []
				},
				{
					label: 'Pouched Wild Tuna &amp; Salmon',
					status: 1,
					checked: false,
					children: []
				},
				{
					label: 'Canned Wild Portuguese Mackerel',
					status: 1,
					checked: true,
					children: []
				},
				{
					label: 'Canned Wild Seafood Samplers',
					status: 1,
					checked: false,
					children: []
				},
				{
					label: 'Canned Wild Fish - No Salt Added',
					status: 1,
					checked: false,
					children: []
				},
				{
					label: 'Canned Wild Portuguese Sardine Fillets',
					status: 1,
					checked: false,
					children: []
				},
				{
					label: 'Canned Wild Fish - Kosher',
					status: 1,
					checked: false,
					children: []
				},
				{
					label: 'Canned Wild Salmon Meal Kit',
					status: 1,
					checked: false,
					children: []
				},
				{
					label: 'Pouched Wild Albacore Tuna (Troll-Caught)',
					status: 1,
					checked: false,
					children: []
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
					checked: false,
					children: []
				},
				{
					label: 'Grilling Planks',
					status: 1,
					checked: true,
					children: []
				}, {
					label: 'Canned Fish Accessories',
					status: 1,
					checked: false,
					children: []
				}, {
					label: 'Signature Water Bottle',
					status: 1,
					checked: false,
					children: []
				},
				{
					label: 'Vital Choice Apron',
					status: 1,
					checked: true,
					children: []
				},
				{
					label: 'Tea Accessories',
					status: 1,
					checked: false,
					children: []
				}
			]
		}, {
			label: 'Frozen Seafood Samplers',
			status: 1,
			checked: true,
			children: []
		},
		{
			label: 'Organic Foods & Grass-Fed Beef',
			status: 1,
			checked: false,
			children: [
				{
					label: 'Farmhouse Culture Kraut & Kimchi',
					status: 1,
					checked: false,
					children: []
				},
				{
					label: 'Organic Grass-Fed Beef by Skagit River Ranch',
					status: 1,
					checked: false,
					children: [
						{
							label: 'Organic Grass-Fed Wagyu Beef Chuck Roast',
							status: 1,
							checked: false,
							children: []
						},
						{
							label: 'Organic Grass-Fed Wagyu Beef Burger Patties - 5.3 oz',
							status: 1,
							checked: true,
							children: []
						},
						{
							label: 'Organic Grass-Fed Wagyu Beef Tenderloin Fillets',
							status: 1,
							checked: false,
							children: []
						},
						{
							label: 'Organic Grass-Fed Wagyu Beef Sampler',
							status: 2,
							checked: false,
							children: []
						},
						{
							label: 'Organic Grass-Fed Wagyu Beef - Ground',
							status: 1,
							checked: false,
							children: []
						},
						{
							label: 'Organic Grass-Fed Wagyu Beef Stir-Fry Strips',
							status: 1,
							checked: false,
							children: []
						},
						{
							label: 'Organic Grass-Fed Wagyu Beef Ribeye Steaks',
							status: 1,
							checked: false,
							children: []
						}
					]
				}
			]
		}
	];
}]);