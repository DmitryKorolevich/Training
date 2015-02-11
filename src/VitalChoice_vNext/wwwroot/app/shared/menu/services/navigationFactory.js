'use strict';

angular.module('app.shared.menu.services.navigationFactory', [])
.factory('navigationFactory', [function () {
	var menu = [
		{ name: 'state1', stateName: 'index.state1', stateLabel: 'Module' },
		{
			name: 'state2', stateName: 'index.state2', stateLabel: 'Complex Module',
			subMenu: [
				{ name: 'child1', stateName: 'index.state2.childState1', stateLabel: 'Option 1' },
				{ name: 'child2', stateName: 'index.state2.childState2', stateLabel: 'Option 2' },
				{ name: 'child3', stateName: 'index.state2.childState3', stateLabel: 'Option 3' },
				{ name: 'child4', stateName: 'index.state2.childState4', stateLabel: 'Option 4' },
				{ name: 'child5', stateName: 'index.state2.childState5', stateLabel: 'Option 5' },
				{ name: 'child6', stateName: 'index.state2.childState6', stateLabel: 'Option 6' },
				{ name: 'child7', stateName: 'index.state2.childState7', stateLabel: 'Option 7' },
				{ name: 'child8', stateName: 'index.state2.childState8', stateLabel: 'Option 8' },
				{ name: 'child9', stateName: 'index.state2.childState9', stateLabel: 'Option 9' }
			]
		},
		{ name: 'state3', stateName: 'index.state3', stateLabel: 'Module and Sidebar', sidebar: '' }, /*todo*/
		{
			name: 'state4', stateName: 'index.state4', stateLabel: 'Complex Module and Sidebar',
			submenu: [
				{ name: 'child10', stateName: 'index.state4.childState10', stateLabel: 'Option 10' },
				{ name: 'child11', stateName: 'index.state4.childState11', stateLabel: 'Option 11' }
			],
			sidebar: '' /*todo*/
		},
		{ name: 'state5', stateName: 'index.state5', stateLabel: 'Placeholder 1' },
		{ name: 'state6', stateName: 'index.state6', stateLabel: 'Placeholder 2' }
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