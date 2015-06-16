'use strict';

angular.module('app.shared.menu.services.navigationFactory', [])
.factory('navigationFactory', ['$state', 'modalUtil', function ($state, modalUtil) {
	var menu = [
		{
			name: 'customer',
			stateLabel: 'Customer',
			subMenu: [
				{ name: 'locateCustomer', stateName: 'index.oneCol.locateCustomer', stateLabel: 'Locate Customer', access: 1 },
				{ name: 'addNewCustomer', stateName: 'index.oneCol.addNewCustomer', stateLabel: 'Add New Customer', access: 1 }
			]
		},
		{
			name: 'orders',
			stateLabel: 'Orders',
			subMenu: [
				{ name: 'viewAllOrders', stateName: 'index.oneCol.viewAllOrders', stateLabel: 'View All Orders', access: 2 },
				{ name: 'locateOrder', stateName: 'index.oneCol.locateOrder', stateLabel: 'Locate Order', access: 2 },
				{ name: 'placeNewOrder', stateName: 'index.oneCol.placeNewOrder', stateLabel: 'Place New Order', access: 2 }
			]
		},
		{
			name: 'products',
			stateLabel: 'Products',
			subMenu: [
				{ name: 'locateProduct', stateName: 'index.oneCol.manageProducts', stateLabel: 'Manage Products', access: 4 },
				{ name: 'addNewProduct', stateLabel: 'Add New Product', access: 4,
				    handler: function ()
				    {
				        modalUtil.open('app/modules/product/partials/addProductPopup.html', 'addProductPopupController', {
				            thenCallback: function (data) {
				                $state.go('index.oneCol.addNewProduct', { type: data });
				            }
				        });
				    },
                },
				{ name: 'manageCategories', stateName: 'index.oneCol.manageProductCategories', stateLabel: 'Manage Categories', access: 4 },
				{ name: 'manageProductReviews', stateName: 'index.oneCol.manageProductReviews', stateLabel: 'Manage Product Reviews', access: 4 },
			]
		},
        {
            name: 'marketing',
        	stateLabel: 'Marketing',
        	subMenu: [
                { name: 'manageDCs', stateName: 'index.oneCol.manageDCs', stateLabel: 'Manage Discount Codes', access: 14 },
                { name: 'manageGCs', stateName: 'index.oneCol.manageGCs', stateLabel: 'Manage Gift Certificates', access: 14 },
        	]
        },
		{
			name: 'reports',
			stateLabel: 'Reports',
			subMenu: [
				{ name: 'salesOrders', stateName: 'index.twoCols.salesOrders.healthWise', stateLabel: 'Sales, Orders', access: 3 },
				{ name: 'wholesale', stateName: 'index.twoCols.wholesale.wholesaleSummary', stateLabel: 'Wholesale', access: 3 },
				{ name: 'affiliates', stateName: 'index.oneCol.affiliates', stateLabel: 'Affiliates', access: 3 },
				{ name: 'operations', stateName: 'index.twoCols.operations.vitalGreen', stateLabel: 'Operations', access: 3 },
				{ name: 'listProcessingAnalysis', stateName: 'index.oneCol.listProcessingAnalysis', stateLabel: 'List Processing, Analysis', access: 3 }
			]
		},
		{
			name: 'affiliates',
			stateLabel: 'Affiliates',
			subMenu: [
				{ name: 'locateAffiliate', stateName: 'index.oneCol.locateAffiliate', stateLabel: 'Locate Affiliate', access: 5 },
				{ name: 'addNewAffiliate', stateName: 'index.oneCol.addNewAffiliate', stateLabel: 'Add New Affilaite', access: 5 }
			]
		},
		{
			name: 'content',
			stateLabel: 'Content',
			subMenu: [
				{ name: 'managePages', stateName: 'index.oneCol.manageContentPages', stateLabel: 'Manage Content', access: 6 },
				{ name: 'manageArticles', stateName: 'index.oneCol.manageArticles', stateLabel: 'Manage Articles' , access: 6 },
				{ name: 'manageRecipes', stateName: 'index.oneCol.manageRecipes', stateLabel: 'Manage Recipes', access: 6 },
				{ name: 'manageFaqs', stateName: 'index.oneCol.manageFaqs', stateLabel: 'Manage FAQs', access: 6 },
                { name: 'manageMasters', stateName: 'index.oneCol.manageMasters', stateLabel: 'Manage Master Templates', access: 7 },
				{ name: 'filesManagement', stateName: 'index.oneCol.filesManagement', stateLabel: 'Manage Media', access: 12 },
			]
		},
		{
			name: 'tools',
			stateLabel: 'Tools',
			subMenu: [
				{ name: 'productTaxCodes', stateName: 'index.oneCol.productTaxCodes', stateLabel: 'Product Tax Codes', access: 8 },
				{ name: 'emailAddressProfiles', stateName: 'index.oneCol.emailAddressProfiles', stateLabel: 'Multiple Email Address Profiles', access: 8 },
				{ name: 'reassignTransaction', stateName: 'index.oneCol.reassignTransaction', stateLabel: 'Reassign Transaction', access: 8 },
				{ name: 'changeOrderStatus', stateName: 'index.oneCol.changeOrderStatus', stateLabel: 'Change Order Status', access: 8 },
				{ name: 'healthWise', stateName: 'index.oneCol.healthWise', stateLabel: 'HealthWise', access: 8 },
			]
		},
		{
			name: 'users',
			stateLabel: 'Users',
			subMenu: [
				{ name: 'manageUsers', stateName: 'index.oneCol.manageUsers', stateLabel: 'Manage Users', access: 9 }
			]
		},
		{
			name: 'settings',
			stateLabel: 'Settings',
			subMenu: [
				{ name: 'manageCountries', stateName: 'index.oneCol.manageCountries', stateLabel: 'Manage Countries / States', access: 10 },
				{ name: 'manageSettings', stateName: 'index.oneCol.manageSettings', stateLabel: 'Perishable Cart Threshold', access: 10 },
				{ name: 'managePaymentMethods', stateName: 'index.oneCol.managePaymentMethods', stateLabel: 'Manage Payment Methods', access: null }, //only for super admins
				{ name: 'manageOrderNotes', stateName: 'index.oneCol.manageOrderNotes', stateLabel: 'Manage Order Specific Notes', access: null }
			]
		},
		{
			name: 'help',
			stateLabel: 'Help',
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