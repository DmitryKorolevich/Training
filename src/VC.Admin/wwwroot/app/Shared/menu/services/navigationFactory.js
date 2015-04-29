'use strict';

angular.module('app.shared.menu.services.navigationFactory', [])
.factory('navigationFactory', [function () {
	var menu = [
		{
			name: 'customer',
			stateLabel: 'Customer',
			access: 1,
			subMenu: [
				{ name: 'locateCustomer', stateName: 'index.oneCol.locateCustomer', stateLabel: 'Locate Customer', access: 1 },
				{ name: 'addNewCustomer', stateName: 'index.oneCol.addNewCustomer', stateLabel: 'Add New Customer', access: 1 }
			]
		},
		{
			name: 'orders',
			stateLabel: 'Orders',
			access: 2,
			subMenu: [
				{ name: 'viewAllOrders', stateName: 'index.oneCol.viewAllOrders', stateLabel: 'View All Orders', access: 2 },
				{ name: 'locateOrder', stateName: 'index.oneCol.locateOrder', stateLabel: 'Locate Order', access: 2 },
				{ name: 'placeNewOrder', stateName: 'index.oneCol.placeNewOrder', stateLabel: 'Place New Order', access: 2 }
			]
		},
		{
			name: 'reports',
			stateLabel: 'Reports',
			access: 3,
			subMenu: [
				{ name: 'salesOrders', stateName: 'index.twoCols.salesOrders.healthWise', stateLabel: 'Sales, Orders', access: 3 },
				{ name: 'wholesale', stateName: 'index.twoCols.wholesale.wholesaleSummary', stateLabel: 'Wholesale', access: 3 },
				{ name: 'affiliates', stateName: 'index.oneCol.affiliates', stateLabel: 'Affiliates', access: 3 },
				{ name: 'operations', stateName: 'index.twoCols.operations.vitalGreen', stateLabel: 'Operations', access: 3 },
				{ name: 'listProcessingAnalysis', stateName: 'index.oneCol.listProcessingAnalysis', stateLabel: 'List Processing, Analysis', access: 3 }
			]
		},
		{
			name: 'products',
			stateLabel: 'Products',
			access: 4,
			subMenu: [
				{ name: 'locateProduct', stateName: 'index.oneCol.locateProduct', stateLabel: 'Locate a Product', access: 4 },
				{ name: 'addNewProduct', stateName: 'index.oneCol.addNewProduct', stateLabel: 'Add New Product', access: 4 },
				{ name: 'manageCategories', stateName: 'index.oneCol.manageProductCategories', stateLabel: 'Manage Categories', access: 4 },
				{ name: 'manageProductReviews', stateName: 'index.oneCol.manageProductReviews', stateLabel: 'Manage Product Reviews', access: 4 }
			]
		},
		{
			name: 'affiliates',
			stateLabel: 'Affiliates',
			access: 5,
			subMenu: [
				{ name: 'locateAffiliate', stateName: 'index.oneCol.locateAffiliate', stateLabel: 'Locate Affiliate', access: 5 },
				{ name: 'addNewAffiliate', stateName: 'index.oneCol.addNewAffiliate', stateLabel: 'Add New Affilaite', access: 5 }
			]
		},
		{
			name: 'content',
			stateLabel: 'Content',
			access: 6,
			subMenu: [
				{ name: 'managePages', stateName: 'index.oneCol.manageContentPages', stateLabel: 'Manage Content', access: 6 },
				{ name: 'manageArticles', stateName: 'index.oneCol.manageArticles', stateLabel: 'Manage Articles' , access: 6 },
				{ name: 'manageRecipes', stateName: 'index.oneCol.manageRecipes', stateLabel: 'Manage Recipes', access: 6 },
				{ name: 'manageFaqs', stateName: 'index.oneCol.manageFaqs', stateLabel: 'Manage FAQs', access: 6 },
                { name: 'manageMasters', stateName: 'index.oneCol.manageMasters', stateLabel: 'Manage Master Templates', access: 7 }
			]
		},
		{
			name: 'tools',
			stateLabel: 'Tools',
			access: 8,
			subMenu: [
				{ name: 'productTaxCodes', stateName: 'index.oneCol.productTaxCodes', stateLabel: 'Product Tax Codes', access: 8 },
				{ name: 'emailAddressProfiles', stateName: 'index.oneCol.emailAddressProfiles', stateLabel: 'Multiple Email Address Profiles', access: 8 },
				{ name: 'reassignTransaction', stateName: 'index.oneCol.reassignTransaction', stateLabel: 'Reassign Transaction', access: 8 },
				{ name: 'changeOrderStatus', stateName: 'index.oneCol.changeOrderStatus', stateLabel: 'Change Order Status', access: 8 },
				{ name: 'healthWise', stateName: 'index.oneCol.healthWise', stateLabel: 'HealthWise', access: 8 }
			]
		},
		{
			name: 'users',
			stateLabel: 'Users',
			access: 9,
			subMenu: [
				{ name: 'manageUsers', stateName: 'index.oneCol.manageUsers', stateLabel: 'Manage Users', access: 9 }
			]
		},
		{
			name: 'settings',
			stateLabel: 'Settings',
			access: 10,
			subMenu: [
				{ name: 'manageCountries', stateName: 'index.oneCol.manageCountries', stateLabel: 'Manage Countries / States', access: 10 },
				{ name: 'manageSettings', stateName: 'index.oneCol.manageSettings', stateLabel: 'Perishable Cart Threshold', access: 10 }
			]
		},
		{
			name: 'help',
			stateLabel: 'Help',
			access: 11,
			subMenu: [
				{ name: 'submitBug', stateName: 'index.oneCol.submitBug', stateLabel: 'Submit Bug', access: 11 },
				{ name: 'viewWiki', stateName: 'index.oneCol.viewWiki', stateLabel: 'View Wiki', access: 11 },
				{ name: 'viewLogs', stateName: 'index.oneCol.manageLogs', stateLabel: 'View Logs', access: 10 }
			]
		}
	];

	return {
		menu : menu
	};

}]);