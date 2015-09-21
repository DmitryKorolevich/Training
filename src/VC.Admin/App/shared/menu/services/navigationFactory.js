'use strict';

angular.module('app.shared.menu.services.navigationFactory', [])
.factory('navigationFactory', ['$state', 'modalUtil', function ($state, modalUtil) {
	var menu = [
		{
			name: 'customer',
			stateLabel: 'Customer',
			subMenu: [
				{ name: 'manageCustomers', stateName: 'index.oneCol.manageCustomers', stateLabel: 'Manage Customers', access: 1 },
				{ name: 'addNewCustomer', stateName: 'index.oneCol.addCustomer', stateLabel: 'Add New Customer', access: 1 }
			]
		},
		{
			name: 'orders',
			stateLabel: 'Orders',
			subMenu: [
				{ name: 'viewAllOrders', stateName: 'index.oneCol.manageOrders', stateLabel: 'Manage Orders', access: 2 },
				{ name: 'viewAllOrders', stateName: 'index.oneCol.manageHelpTickets', stateLabel: 'Manage Help Tickets', access: 11 },
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
				{ name: 'manageInventoryCategories', stateName: 'index.oneCol.manageInventoryCategories', stateLabel: 'Manage Sales Categories', access: 4 },
				{ name: 'manageProductReviews', stateName: 'index.oneCol.manageProductReviews', stateLabel: 'Manage Product Reviews', access: 4 },
			]
		},
        {
            name: 'marketing',
        	stateLabel: 'Marketing',
        	subMenu: [
                { name: 'manageDCs', stateName: 'index.oneCol.manageDiscounts', stateLabel: 'Manage Discount Codes', access: 14 },
                { name: 'managePromotions', stateName: 'index.oneCol.managePromotions', stateLabel: 'Manage Promotions', access: 14 },
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
				{ name: 'locateAffiliate', stateName: 'index.oneCol.manageAffiliates', stateLabel: 'Manage Affiliates', access: 5 },
				{ name: 'addNewAffiliate', stateName: 'index.oneCol.affiliateAdd', stateLabel: 'Add New Affilaite', access: 5 }
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
                { name: 'manageContentAreas', stateName: 'index.oneCol.manageContentAreas', stateLabel: 'Manage Design Elements', access: 6 },
                { name: 'manageStyles', stateName: 'index.oneCol.manageStyles', stateLabel: 'Manage CSS', access: 6 },
				{ name: 'filesManagement', stateName: 'index.oneCol.filesManagement', stateLabel: 'Manage Media', access: 12 },
			]
		},
		{
			name: 'tools',
			stateLabel: 'Tools',
			subMenu: [
				{ name: 'manageProductTaxCodes', stateName: 'index.oneCol.manageProductTaxCodes', stateLabel: 'Manage Product Tax Codes', access: 8 },
				{ name: 'emailAddressProfiles', stateName: 'index.oneCol.emailAddressProfiles', stateLabel: 'Multiple Email Address Profiles', access: 8 },
				{ name: 'moveOrder', stateName: 'index.oneCol.moveOrder', stateLabel: 'Reassign Transaction', access: 8 },
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
				{ name: 'manageSettings', stateName: 'index.oneCol.manageSettings', stateLabel: 'Global Settings', access: null },
				{ name: 'managePaymentMethods', stateName: 'index.oneCol.managePaymentMethods', stateLabel: 'Manage Payment Methods', access: null }, //only for super admins
				{ name: 'manageOrderNotes', stateName: 'index.oneCol.manageOrderNotes', stateLabel: 'Manage Automatic Order Specific Notes', access: null }
			]
		},
		{
			name: 'help',
			stateLabel: 'Help',
			subMenu: [
				{ name: 'manageBugTickets', stateName: 'index.oneCol.manageBugTickets', stateLabel: 'Manage Bug Tickets', access: 11 },
				{ name: 'viewWiki', stateName: 'index.oneCol.viewWiki', stateLabel: 'View Wiki', access: 11 },
				{ name: 'viewLogs', stateName: 'index.oneCol.manageLogs', stateLabel: 'View Logs', access: 10 }
			]
		}
	];

	return {
		menu : menu
	};

}]);