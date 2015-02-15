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
		}
	];

	return {
		sidebars: sidebars
	};

}]);