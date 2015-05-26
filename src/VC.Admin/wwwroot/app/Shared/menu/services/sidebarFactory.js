'use strict';

angular.module('app.shared.menu.services.sidebarFactory', [])
.factory('sidebarFactory', [function () {
	var sidebars = [
		{
			name: 'salesOrders',
			moduleName: 'salesOrders',
			menu: [
				{ name: 'sidebar-healthWise', stateName: 'index.twoCols.salesOrders.healthWise', stateLabel: 'HealthWise' },
				{ name: 'sidebar-orderStatusHistory', stateName: 'index.twoCols.salesOrders.orderStatusHistory', stateLabel: 'Order Status History' },
				{ name: 'sidebar-ooStockRequests', stateName: 'index.twoCols.salesOrders.ooStockRequests', stateLabel: 'Out of Stock Requests' }
			]
		},
		{
			name: 'wholesale',
			moduleName: 'wholesale',
			menu: [
			  { name: 'sidebar-wholesaleSummary', stateName: 'index.twoCols.wholesale.wholesaleSummary', stateLabel: 'Wholesale Summary Report' },
			  { name: 'sidebar-wholesaleDropShipOrders', stateName: 'index.twoCols.wholesale.wholesaleDropShipOrders', stateLabel: 'Wholesale Drop Ship Orders Report' }
			]
		},
		{
			name: 'operations',
			moduleName: 'operations',
			menu: [
			  { name: 'sidebar-vitalGreen', stateName: 'index.twoCols.operations.vitalGreen', stateLabel: 'VitalGreen' }
			]
		}
	];

	return {
		sidebars: sidebars
	};

}]);