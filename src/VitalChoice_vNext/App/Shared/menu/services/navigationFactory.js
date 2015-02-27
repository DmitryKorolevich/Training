﻿'use strict';

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
				{ name: 'salesOrders', stateName: 'index.twoCols.salesOrders', stateLabel: 'Sales, Orders' },
				{ name: 'wholesale', stateName: 'index.twoCols.wholesale', stateLabel: 'Wholesale' },
				{ name: 'affiliates', stateName: 'index.oneCol.affiliates', stateLabel: 'Affiliates' },
				{ name: 'operations', stateName: 'index.twoCols.operations', stateLabel: 'Operations' },
				{ name: 'listProcessingAnalysis', stateName: 'index.twoCols.listProcessingAnalysis', stateLabel: 'List Processing, Analysis' }
			]
		},
		{
			name: 'products',
			stateLabel: 'Products',
			subMenu: [
				{ name: 'locateProduct', stateName: 'index.oneCol.locateProduct', stateLabel: 'Locate a Product' },
				{ name: 'addNewProduct', stateName: 'index.oneCol.addNewProduct', stateLabel: 'Add New Product' },
				{ name: 'manageCategories', stateName: 'index.oneCol.manageCategories', stateLabel: 'Manage Categories' },
				{ name: 'manageProductReviews', stateName: 'index.oneCol.manageProductReviews', stateLabel: 'Manage Product Reviews' }
				//{ name: 'demo', stateName: 'index.oneCol.demo4', stateLabel: 'Manage Categories' },
				//{ name: 'demo', stateName: 'index.oneCol.demo5', stateLabel: 'Manage Brands' },
				//{ name: 'demo', stateName: 'index.oneCol.demo6', stateLabel: 'Manage Custom Fields' },
				//{ name: 'demo', stateName: 'index.oneCol.demo7', stateLabel: 'Manage Gift Certificates' },
				//{ name: 'demo', stateName: 'index.oneCol.demo8', stateLabel: 'Manage Product Options' },
				//{ name: 'demo', stateName: 'index.oneCol.demo9', stateLabel: 'Manage Product Reviews' },
				//{ name: 'demo', stateName: 'index.oneCol.demo10', stateLabel: 'Manage Suppliers' },
				//{ name: 'demo', stateName: 'index.oneCol.demo11', stateLabel: 'Manage Drop-Shippers' },
				//{ name: 'demo', stateName: 'index.oneCol.demo12', stateLabel: 'Update Multiple Products' },
				//{ name: 'demo', stateName: 'index.oneCol.demo13', stateLabel: 'Cart Cross Selling Items' },
				//{ name: 'demo', stateName: 'index.oneCol.demo14', stateLabel: 'Out of Stock Requests' },
				//{ name: 'demo', stateName: 'index.oneCol.demo15', stateLabel: 'Perishable Cart Threshold Amount' }
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
				{ name: 'managePages', stateName: 'index.oneCol.managePages', stateLabel: 'Manage Pages' },
				{ name: 'manageArticles', stateName: 'index.oneCol.manageArticles', stateLabel: 'Manage Articles' },
				{ name: 'manageRecipes', stateName: 'index.oneCol.manageRecipes', stateLabel: 'Manage Recipes' },
				{ name: 'manageFaqs', stateName: 'index.oneCol.manageFaqs', stateLabel: 'Manage FAQs' }
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
				{ name: 'manageCountries', stateName: 'index.oneCol.manageCountries', stateLabel: 'Manage Countries' },
				{ name: 'manageStates', stateName: 'index.oneCol.manageStates', stateLabel: 'Manage States' },
				{ name: 'perishableCartThreshold', stateName: 'index.oneCol.perishableCartThreshold', stateLabel: 'Perishable Cart Threshold' }
			]
		},
		{
			name: 'help',
			stateLabel: 'Help',
			subMenu: [
				{ name: 'submitBug', stateName: 'index.oneCol.submitBug', stateLabel: 'Submit Bug' },
				{ name: 'viewWiki', stateName: 'index.oneCol.viewWiki', stateLabel: 'View Wiki' }
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