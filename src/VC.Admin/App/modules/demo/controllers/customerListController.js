'use strict';

angular.module('app.modules.demo.controllers.customerListController', [])
.controller('customerListController', ['$scope', function ($scope) {
	$scope.customers = [
		{ Name: 'Kopp, Kristin', City: 'Mountlake Terrace', State: 'WA', LastOrderPlaced: '3/2/2015', TotalOrders: '100', LastUpdated: '9/9/2015' },
		{ Name: 'Britt, Charles', City: 'Aurora', State: 'CA', LastOrderPlaced: '3/1/2015', TotalOrders: '12', LastUpdated: '1/12/2015' },
		{ Name: 'saunby, colleen', City: 'Anacortes', State: 'WA', LastOrderPlaced: '3/1/2015', TotalOrders: '34', LastUpdated: '3/3/2015' },
		{ Name: 'Calvert, Jason', City: 'Ferndale', State: 'WA', LastOrderPlaced: '3/12/2014', TotalOrders: '34', LastUpdated: '3/3/2015' },
		{ Name: 'ORRIS, JB', City: 'Menlo Park', State: 'MS', LastOrderPlaced: '10/3/2015', TotalOrders: '12', LastUpdated: '6/8/2015' },
		{ Name: 'Simmons, Mark', City: 'Neenah', State: 'CA', LastOrderPlaced: '3/3/2015', TotalOrders: '12', LastUpdated: '3/3/2015' },
		{ Name: 'Leo, Kristin', City: 'Malibu', State: 'WA', LastOrderPlaced: '1/7/2015', TotalOrders: '76', LastUpdated: '10/7/2015' },
		{ Name: 'Nodine, Tony', City: 'Madison', State: 'MS', LastOrderPlaced: '1/9/2015', TotalOrders: '76', LastUpdated: '12/3/2015' },
		{ Name: 'Green, Michael', City: 'Orlando', State: 'WA', LastOrderPlaced: '1/8/2015', TotalOrders: '12', LastUpdated: '9/3/2015' },
		{ Name: 'Kristin, Bob', City: 'Mountlake Terrace', State: 'WA', LastOrderPlaced: '3/3/2015', TotalOrders: '76', LastUpdated: '3/3/2015' },
		{ Name: 'Swistock, Amy E', City: 'Santa Clarita', State: 'MS', LastOrderPlaced: '3/7/2015', TotalOrders: '123', LastUpdated: '3/9/2015' },
		{ Name: 'Simmons, Mark', City: 'San Jose', State: 'WA', LastOrderPlaced: '3/3/2015', TotalOrders: '34', LastUpdated: '3/4/2015' },
		{ Name: 'Lopp, In', City: 'Mountlake Terrace', State: 'OH', LastOrderPlaced: '3/3/2015', TotalOrders: '23', LastUpdated: '3/3/2015' },
		{ Name: 'Hall, Thia', City: 'Wayne', State: 'WA', LastOrderPlaced: '3/3/2015', TotalOrders: '76', LastUpdated: '3/3/2015' },
		{ Name: 'Gary, Gould', City: 'Mooreland', State: 'CA', LastOrderPlaced: '9/10/2014', TotalOrders: '76', LastUpdated: '3/7/2015' },
		{ Name: 'massey, terry', City: 'Mountlake Terrace', State: 'WA', LastOrderPlaced: '3/3/2015', TotalOrders: '76', LastUpdated: '3/9/2015' },
		{ Name: 'K, Kristin', City: 'MMooreland', State: 'MS', LastOrderPlaced: '2/3/2013', TotalOrders: '23', LastUpdated: '3/9/2015' },
		{ Name: 'Dart, Kristin', City: 'Wayne', State: 'OH', LastOrderPlaced: '3/10/2012', TotalOrders: '76', LastUpdated: '3/3/2015' },
		{ Name: 'Cameron, James', City: 'Mountlake Terrace', State: 'WA', LastOrderPlaced: '3/3/2015', TotalOrders: '23', LastUpdated: '3/9/2015' },
		{ Name: 'Lee, Bruce', City: 'Orlando', State: 'OH', LastOrderPlaced: '1/3/2011', TotalOrders: '121', LastUpdated: '3/11/2015' },
		{ Name: 'kalemkiarian, Alice', City: 'Wayne', State: 'WA', LastOrderPlaced: '3/10/2010', TotalOrders: '56', LastUpdated: '3/11/2015' },
	]
}]);