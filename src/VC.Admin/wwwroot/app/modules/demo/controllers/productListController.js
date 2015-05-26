'use strict';

angular.module('app.modules.demo.controllers.productListController', [])
.controller('productListController', ['$scope', function ($scope) {
	$scope.products = [
		{ SKU: 'FGS', Name: 'Good Health Gift', Description: 'Good Health Gift' },
		{ SKU: 'FGS', Name: 'Dill Weed - 0.7 oz.', Description: 'Good Health Gift' },
		{ SKU: 'FGS', Name: 'Dr. Amen Healthy Brain Gift Box - $129 Delivered', Description: 'Good Health Gift' },
		{ SKU: 'FGS', Name: 'Cumin - Ground 2.0 oz.', Description: 'Good Health Gift' },
		{ SKU: 'FGS', Name: 'Darjeeling 2nd Flush Muscatel - 2.5oz', Description: 'Good Health Gift' },
		{ SKU: 'FGS', Name: 'Salmon Sausage Sampler', Description: 'Good Health Gift' },
		{ SKU: 'FGS', Name: 'Salmon Patties Meal Kit', Description: 'Good Health Gift' },
		{ SKU: 'FGS', Name: 'Savory Baked Cherry Tomatoes - 6.7 oz', Description: 'Raw denim you probably havent heard of them jean shorts Austin' },
		{ SKU: 'FGS', Name: 'Soups Sampler Trio', Description: 'Good Health Gift' },
		{ SKU: 'FGS', Name: 'Good Health Gift', Description: 'Aliquip placeat salvia cillum iphone. Seitan aliquip quis cardigatcher voluptate nisi qui' },
		{ SKU: 'FGS', Name: 'Sockeye Salmon Candy 4 oz - $49', Description: 'Good Health Gift' },
		{ SKU: 'FGS', Name: 'Sockeye Salmon Candy - 6 ozt', Description: 'Good Health Gift' },
		{ SKU: 'FGS', Name: 'Candy - 6 oz', Description: 'heard of them jean shorts Austin Health Gift' },
		{ SKU: 'FGS', Name: 'Candy - 6 oz', Description: 'Aliquip placeat salvia cillum iphone. Seitan aliquip quis cardigann shorts Austin Gift' },
		{ SKU: 'FGS', Name: 'Salmon Candy - 6 oz', Description: 'Good heard of them jean shorts Austin Gift' },
		{ SKU: 'FGS', Name: 'Sockeye  Candy - 6 oz', Description: 'Good heard of them jean shorts Austin Gift' },
		{ SKU: 'FGS', Name: 'Good Health Gift', Description: 'Good Health Gift' },
		{ SKU: 'FGS', Name: 'Soups Sampler Trio', Description: 'Good Health Gift' },
		{ SKU: 'FGS', Name: 'Special Instructions - Non-Perishables', Description: 'Good Health Gift' },
		{ SKU: 'FGS', Name: 'Good Health Gift', Description: 'GoAliquip placeat salvia cillum iphone. Seitan aliquip quisigarel, butcher voluptate nisi quift' },
		{ SKU: 'FGS', Name: 'Spicy Chorizo Style Salmon Sausage', Description: 'Good Health Aliquip placeat salvia cilloluptate nisi qui' },
	]
}]);