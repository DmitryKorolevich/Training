'use strict';

angular.module('app.shared.menu.services.navigationFactory', [])
.factory('navigationFactory', [function () {
	var menu = [
		{
			name: 'customer',
			stateLabel: 'Customer',
			subMenu: [
				{ name: 'locateCustomer', stateName: 'index.oneCol.locateCustomer', stateLabel: 'Locate Customer' },
				{ name: 'addNewCustomer', stateName: 'index.oneCol.addNewCustomer', stateLabel: 'Add New Customer' }
			]
		},
		{
			name: 'orders',
			stateLabel: 'Orders',
			subMenu: [
				{ name: 'viewAllOrders', stateName: 'index.oneCol.viewAllOrders', stateLabel: 'View All Orders' },
				{ name: 'locateOrder', stateName: 'index.oneCol.locateOrder', stateLabel: 'Locate Order' },
				{ name: 'placeNewOrder', stateName: 'index.oneCol.placeNewOrder', stateLabel: 'Place New Order' }
			]
		},
		{
			name: 'reports',
			stateLabel: 'Reports',
			subMenu: [
				{ name: 'salesOrders', stateName: 'index.twoCols.salesOrders.healthWise', stateLabel: 'Sales, Orders' },
				{ name: 'wholesale', stateName: 'index.twoCols.wholesale.wholesaleSummary', stateLabel: 'Wholesale' },
				{ name: 'affiliates', stateName: 'index.oneCol.affiliates', stateLabel: 'Affiliates' },
				{ name: 'operations', stateName: 'index.twoCols.operations.vitalGreen', stateLabel: 'Operations' },
				{ name: 'listProcessingAnalysis', stateName: 'index.oneCol.listProcessingAnalysis', stateLabel: 'List Processing, Analysis' }
			]
		},
		{
			name: 'products',
			stateLabel: 'Products',
			subMenu: [
				{ name: 'locateProduct', stateName: 'index.oneCol.locateProduct', stateLabel: 'Locate a Product' },
				{ name: 'addNewProduct', stateName: 'index.oneCol.addNewProduct', stateLabel: 'Add New Product' },
				{ name: 'manageCategories', stateName: 'index.oneCol.manageProductCategories', stateLabel: 'Manage Categories' },
				{ name: 'manageProductReviews', stateName: 'index.oneCol.manageProductReviews', stateLabel: 'Manage Product Reviews' }
			]
		},
		{
			name: 'affiliates',
			stateLabel: 'Affiliates',
			subMenu: [
				{ name: 'locateAffiliate', stateName: 'index.oneCol.locateAffiliate', stateLabel: 'Locate Affiliate' },
				{ name: 'addNewAffiliate', stateName: 'index.oneCol.addNewAffiliate', stateLabel: 'Add New Affilaite' }
			]
		},
		{
			name: 'content',
			stateLabel: 'Content',
			subMenu: [
				{ name: 'managePages', stateName: 'index.oneCol.manageContentPages', stateLabel: 'Manage Content' },
				{ name: 'manageArticles', stateName: 'index.oneCol.manageArticles', stateLabel: 'Manage Articles' },
				{ name: 'manageRecipes', stateName: 'index.oneCol.manageRecipes', stateLabel: 'Manage Recipes' },
				{ name: 'manageFaqs', stateName: 'index.oneCol.manageFaqs', stateLabel: 'Manage FAQs' },
                { name: 'manageMasters', stateName: 'index.oneCol.manageMasters', stateLabel: 'Manage Master Templates' }
			]
		},
		{
			name: 'tools',
			stateLabel: 'Tools',
			subMenu: [
				{ name: 'productTaxCodes', stateName: 'index.oneCol.productTaxCodes', stateLabel: 'Product Tax Codes' },
				{ name: 'emailAddressProfiles', stateName: 'index.oneCol.emailAddressProfiles', stateLabel: 'Multiple Email Address Profiles' },
				{ name: 'reassignTransaction', stateName: 'index.oneCol.reassignTransaction', stateLabel: 'Reassign Transaction' },
				{ name: 'changeOrderStatus', stateName: 'index.oneCol.changeOrderStatus', stateLabel: 'Change Order Status' },
				{ name: 'healthWise', stateName: 'index.oneCol.healthWise', stateLabel: 'HealthWise' }
			]
		},
		{
			name: 'users',
			stateLabel: 'Users',
			subMenu: [
				{ name: 'manageUsers', stateName: 'index.oneCol.manageUsers', stateLabel: 'Manage Users' }
			]
		},
		{
			name: 'settings',
			stateLabel: 'Settings',
			subMenu: [
				{ name: 'manageCountries', stateName: 'index.oneCol.manageCountries', stateLabel: 'Manage Countries / States' },
				{ name: 'manageSettings', stateName: 'index.oneCol.manageSettings', stateLabel: 'Perishable Cart Threshold' }
			]
		},
		{
			name: 'help',
			stateLabel: 'Help',
			subMenu: [
				{ name: 'submitBug', stateName: 'index.oneCol.submitBug', stateLabel: 'Submit Bug' },
				{ name: 'viewWiki', stateName: 'index.oneCol.viewWiki', stateLabel: 'View Wiki' },
				{ name: 'viewLogs', stateName: 'index.oneCol.manageLogs', stateLabel: 'View Logs' },
			]
		}
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