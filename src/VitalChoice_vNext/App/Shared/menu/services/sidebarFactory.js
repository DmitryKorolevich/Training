'use strict';

angular.module('app.shared.menu.services.sidebarFactory', [])
.factory('sidebarFactory', [function () {
	var sidebars = [
		{
			name: 'sidebar1', moduleName: 'state3', menu: [
			  { name: 'sidebar1.child1', stateName: 'index.twoCols.state3.sidebar1', stateLabel: 'Sidebar Option 1' },
			  {
			  	name: 'sidebar1.child2', stateName: 'index.twoCols.state3.sidebar2', stateLabel: 'Sidebar Option 2', subMenu: [
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
			  { name: 'sidebar1.child3', stateName: 'index.twoCols.state3.sidebar3', stateLabel: 'Sidebar Option 3' }
			]
		},
		{
			name: 'sidebar2', moduleName: 'state4', menu: [
			  {	name: 'sidebar1.child1', stateName: 'index.twoCols.state3.sidebar1', stateLabel: 'Sidebar Option 11' },
			  { name: 'sidebar1.child2', stateName: 'index.twoCols.state3.sidebar2', stateLabel: 'Sidebar Option 222' },
			  { name: 'sidebar1.child3', stateName: 'index.twoCols.state3.sidebar3', stateLabel: 'Sidebar Option 3222' }
			]
		},
		{
			name: 'reportSidebar1', moduleName: 'reportDemo1', menu: [
			  { name: 'reportSidebar1.child1', stateName: 'index.twoCols.demo1.child1', stateLabel: 'Summary Sales Report' },
			  { name: 'reportSidebar1.child2', stateName: 'index.twoCols.demo1.child2', stateLabel: 'Breakdown Report' },
			  { name: 'reportSidebar1.child3', stateName: 'index.twoCols.demo1.child3', stateLabel: 'SKU Breakdown Report' },
			  { name: 'reportSidebar1.child4', stateName: 'index.twoCols.demo1.child4', stateLabel: 'Futures Breakdown Report' },
			  { name: 'reportSidebar1.child5', stateName: 'index.twoCols.demo1.child5', stateLabel: 'Order SKU and Address Report' },
			  { name: 'reportSidebar1.child6', stateName: 'index.twoCols.demo1.child6', stateLabel: 'Regional Sales Summary' },
			  { name: 'reportSidebar1.child7', stateName: 'index.twoCols.demo1.child7', stateLabel: 'Deleted Orders Report' },
			  { name: 'reportSidebar1.child8', stateName: 'index.twoCols.demo1.child8', stateLabel: 'Order SKU Counts' },
			  { name: 'reportSidebar1.child9', stateName: 'index.twoCols.demo1.child9', stateLabel: 'Shipped Via Report' },
			  { name: 'reportSidebar1.child10', stateName: 'index.twoCols.demo1.child10', stateLabel: 'Category Sales Report' },
			  { name: 'reportSidebar1.child11', stateName: 'index.twoCols.demo1.child11', stateLabel: 'Transaction & Refund Report' }
			]
		}, {
			name: 'reportSidebar2', moduleName: 'reportDemo2', menu: [
			  { name: 'reportSidebar1.child1', stateName: 'index.twoCols.demo2.child1', stateLabel: 'Wholesale Summary Report' }
			]
		}, {
			name: 'reportSidebar3', moduleName: 'reportDemo3', menu: [
			  { name: 'reportSidebar1.child1', stateName: 'index.twoCols.demo3.child1', stateLabel: 'Affiliate Customers Report' },
			  { name: 'reportSidebar1.child2', stateName: 'index.twoCols.demo3.child2', stateLabel: 'Affiliate Pay Comissions' },
			  { name: 'reportSidebar1.child3', stateName: 'index.twoCols.demo3.child3', stateLabel: 'Affiliate Summary Information' }
			]
		}, {
			name: 'reportSidebar4', moduleName: 'reportDemo4', menu: [
			  { name: 'reportSidebar1.child1', stateName: 'index.twoCols.demo4.child1', stateLabel: 'Weekly Agent Report' },
			  { name: 'reportSidebar1.child2', stateName: 'index.twoCols.demo4.child2', stateLabel: 'Service Code Report' },
			  { name: 'reportSidebar1.child3', stateName: 'index.twoCols.demo4.child3', stateLabel: 'Product Quality Issues Report' },
			  { name: 'reportSidebar1.child4', stateName: 'index.twoCols.demo4.child4', stateLabel: 'KPI Report (APIs)' }
			]
		}, {
			name: 'reportSidebar5', moduleName: 'reportDemo5', menu: [
			  { name: 'reportSidebar1.child1', stateName: 'index.twoCols.demo5.child1', stateLabel: 'Matchback and Post-Season Analysis Report' },
			  { name: 'reportSidebar1.child2', stateName: 'index.twoCols.demo5.child2', stateLabel: 'Gift Certificate Report' },
			  { name: 'reportSidebar1.child3', stateName: 'index.twoCols.demo5.child3', stateLabel: 'Gift Certificate Usage Report' },
			  { name: 'reportSidebar1.child4', stateName: 'index.twoCols.demo5.child4', stateLabel: 'Life Time Calculation Report' },
			  { name: 'reportSidebar1.child5', stateName: 'index.twoCols.demo5.child5', stateLabel: 'Mailing List Report' },
			  { name: 'reportSidebar1.child6', stateName: 'index.twoCols.demo5.child6', stateLabel: 'Weekly Sales Report' },
			  { name: 'reportSidebar1.child7', stateName: 'index.twoCols.demo5.child7', stateLabel: 'New Customers Report' }
			]
		}
	];

	return {
		sidebars: sidebars
	};

}]);