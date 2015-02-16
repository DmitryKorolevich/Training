'use strict';

angular.module('app.shared.menu.services.navigationFactory', [])
.factory('navigationFactory', [function () {
	var menu = [
		{
			name: 'product1', stateLabel: 'Products',
			subMenu: [
				{ name: 'demo', stateName: 'index.oneCol.demo1', stateLabel: 'Add New Product' },
				{ name: 'product1.locate', stateName: 'index.oneCol.locate', stateLabel: 'Locate a Product' },
				{ name: 'demo', stateName: 'index.oneCol.demo2', stateLabel: 'Import Products' },
				{ name: 'demo', stateName: 'index.oneCol.demo3', stateLabel: 'Manage Apparel Products' },
				{ name: 'demo', stateName: 'index.oneCol.demo4', stateLabel: 'Manage Categories' },
				{ name: 'demo', stateName: 'index.oneCol.demo5', stateLabel: 'Manage Brands' },
				{ name: 'demo', stateName: 'index.oneCol.demo6', stateLabel: 'Manage Custom Fields' },
				{ name: 'demo', stateName: 'index.oneCol.demo7', stateLabel: 'Manage Gift Certificates' },
				{ name: 'demo', stateName: 'index.oneCol.demo8', stateLabel: 'Manage Product Options' },
				{ name: 'demo', stateName: 'index.oneCol.demo9', stateLabel: 'Manage Product Reviews' },
				{ name: 'demo', stateName: 'index.oneCol.demo10', stateLabel: 'Manage Suppliers' },
				{ name: 'demo', stateName: 'index.oneCol.demo11', stateLabel: 'Manage Drop-Shippers' },
				{ name: 'demo', stateName: 'index.oneCol.demo12', stateLabel: 'Update Multiple Products' },
				{ name: 'demo', stateName: 'index.oneCol.demo13', stateLabel: 'Cart Cross Selling Items' },
				{ name: 'demo', stateName: 'index.oneCol.demo14', stateLabel: 'Out of Stock Requests' },
				{ name: 'demo', stateName: 'index.oneCol.demo15', stateLabel: 'Perishable Cart Threshold Amount' }
			]
		},
		{
			name: 'reports', stateLabel: 'Reports',
			subMenu: [
				{ name: 'reportDemo1', stateName: 'index.twoCols.demo1', stateLabel: 'Sales, Orders' },
				{ name: 'reportDemo2', stateName: 'index.twoCols.demo2', stateLabel: 'Wholesale' },
				{ name: 'reportDemo3', stateName: 'index.twoCols.demo3', stateLabel: 'Affiliates' },
				{ name: 'reportDemo4', stateName: 'index.twoCols.demo4', stateLabel: 'Operations' },
				{ name: 'reportDemo5', stateName: 'index.twoCols.demo5', stateLabel: 'List Processing, Analysis' }
			]
		},
		{ name: 'state1', stateName: 'index.oneCol.state1', stateLabel: 'Module' },
		{
			name: 'state2', stateName: 'index.oneCol.state2', stateLabel: 'Complex Module',
			subMenu: [
				{ name: 'child1', stateName: 'index.oneCol.state2.childState1', stateLabel: 'Option 1' },
				{ name: 'child2', stateName: 'index.oneCol.state2.childState2', stateLabel: 'Option 2' },
				{ name: 'child3', stateName: 'index.oneCol.state2.childState3', stateLabel: 'Option 3' },
				{ name: 'child4', stateName: 'index.oneCol.state2.childState4', stateLabel: 'Option 4' },
				{ name: 'child5', stateName: 'index.oneCol.state2.childState5', stateLabel: 'Option 5' },
				{ name: 'child6', stateName: 'index.oneCol.state2.childState6', stateLabel: 'Option 6' },
				{ name: 'child7', stateName: 'index.oneCol.state2.childState7', stateLabel: 'Option 7' },
				{ name: 'child8', stateName: 'index.oneCol.state2.childState8', stateLabel: 'Option 8' },
				{ name: 'child9', stateName: 'index.oneCol.state2.childState9', stateLabel: 'Option 9' }
			]
		},
		{ name: 'state3', stateName: 'index.twoCols.state3', stateLabel: 'Module and Sidebar'/*, sidebar: 'sidebar1'*/ },
		{
			name: 'state4', stateName: 'index.oneCol.state4', stateLabel: 'Complex Module and Sidebar',
			subMenu: [
				{ name: 'child10', stateName: 'index.oneCol.state4.childState10', stateLabel: 'Option 10' },
				{ name: 'child11', stateName: 'index.oneCol.state4.childState11', stateLabel: 'Option 11' }
			]
			/*sidebar: 'sidebar2'*/
		},
		{ name: 'state5', stateName: 'index.oneCol.state5', stateLabel: 'Placeholder 1' },
		{ name: 'state6', stateName: 'index.oneCol.state6', stateLabel: 'Placeholder 2' }
	];

	var currentModule = '';
	var currentSubModule = '';
	var currentSidebarModule = '';

	function setCurrentModule(module) {
		if (module) {
			currentModule = menu[0];
		}
	}

	function seCurrentSubModule(subModule) {
		if (subModule) {
			currentSubModule = menu[0];
		}
	}

	function setCurrentSidebarNodule(sidebarModule) {
		if (sidebarModule) {
			currentSidebarModule = menu[0];
		}
	}

	return {
		menu : menu
	};

}]);