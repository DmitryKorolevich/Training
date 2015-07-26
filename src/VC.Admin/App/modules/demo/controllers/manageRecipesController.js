'use strict';

angular.module('app.modules.demo.controllers.manageRecipesController', [])
.controller('manageRecipesController', ['$scope', function ($scope) {

/*Categories*/
	$scope.toggle = function(scope) {
		scope.toggle();
	};

	$scope.hasChildren = function(scope) {
		return scope.childNodesCount() > 0;
	};


	$scope.treeOptions = {
	    accept: function (sourceNode, destNodes, destIndex) {
	        var data = sourceNode.$modelValue;
	        var destType = destNodes.$element.attr('data-type');
	        return (data.type == destType); // only accept the same type
	    }
	};

	$scope.countries = [
    {
        Name: 'United',
        Code: 'US',
        type: 'country',
        states: [
            {
                Name: 'Alabama',
                Code: 'AL',
                type: 'US',
            },
            {
                Name: 'Alabama23',
                Code: 'AL2',
                type: 'US',
            },
            {
                Name: 'Alabama24',
                Code: 'AL2',
                type: 'US',
            },
            {
                Name: 'Alabama25',
                Code: 'AL2',
                type: 'US',
            }, ]
    },
 {
     Name: 'Candada',
     Code: 'CA',
     type: 'country',
     states: [
         {
             Name: 'Alberta',
             Code: 'AB',
             type: 'CA',
         },
         {
             Name: 'Alberta2',
             Code: 'AB2',
             type: 'CA',
         }]
 },
  {
      Name: 'USSR',
      Code: 'USSR',
      type: 'country',
      states: [
          {
              Name: 'USSRAlberta',
              Code: 'AB',
              type: 'USSR',
          },
          {
              Name: 'USSRAlberta2',
              Code: 'AB2',
              type: 'USSR',
          }]
  },
	];

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

	$scope.testEdit = function() {
		alert("Test Edit handler");
	};

	$scope.testDelete = function () {
		alert("Test Delete handler");
	};
}]);