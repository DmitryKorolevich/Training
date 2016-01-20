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
				{ name: 'outOfStocks', stateName: 'index.oneCol.outOfStocks', stateLabel: 'Out of Stock Requests', access: 4 },
			]
		},
        {
            name: 'marketing',
        	stateLabel: 'Marketing',
        	subMenu: [
                { name: 'manageDCs', stateName: 'index.oneCol.manageDiscounts', stateLabel: 'Manage Discount Codes', access: 14 },
                { name: 'managePromotions', stateName: 'index.oneCol.managePromotions', stateLabel: 'Manage Promotions', access: 14 },
                { name: 'manageGCs', stateName: 'index.oneCol.manageGCs', stateLabel: 'Manage Gift Certificates', access: 14 },
                { name: 'manageRedirect', stateName: 'index.oneCol.manageRedirect', stateLabel: 'Custom URL Redirects', access: 14 },
        	]
        },
		{
			name: 'reports',
			stateLabel: 'Reports',
			subMenu: [
				{
				    name: 'salesOrders',
				    stateLabel: 'Sales, Orders',
				    subMenu: [
                        { name: 'healthwisePeriods', stateName: 'index.oneCol.healthwisePeriods', stateLabel: 'HealthWise Customer Report', access: 3 },
                        //{ name: 'summarySalesReport', stateName: 'index.oneCol.summarySalesReport', stateLabel: 'Summary Sales Report', access: 3 },
                        //{ name: 'breakdownReport', stateName: 'index.oneCol.breakdownReport', stateLabel: 'Breakdown Report', access: 3 },
                        //{ name: 'sKUBreakdownReport', stateName: 'index.oneCol.sKUBreakdownReport', stateLabel: 'SKU Breakdown Report', access: 3 },
                        //{ name: 'futuresBreakdownReport', stateName: 'index.oneCol.futuresBreakdownReport', stateLabel: 'Futures Breakdown Report', access: 3 },
                        //{ name: 'orderSKUAddressReport', stateName: 'index.oneCol.orderSKUAddressReport', stateLabel: 'Order SKU and Address Report', access: 3 },
                        { name: 'ordersRegionStatistic', stateName: 'index.oneCol.ordersRegionStatistic', stateLabel: 'Regional Sales Summary', access: 3 },
                        //{ name: 'deletedOrdersReport', stateName: 'index.oneCol.deletedOrdersReport', stateLabel: 'Deleted Orders Report', access: 3 },
                        //{ name: 'orderSKUCounts', stateName: 'index.oneCol.orderSKUCounts', stateLabel: 'Order SKU Counts', access: 3 },
                        //{ name: 'shippedViaReport', stateName: 'index.oneCol.shippedViaReport', stateLabel: 'Shipped Via Report', access: 3 },
                        { name: 'productCategoriesStatistic', stateName: 'index.oneCol.productCategoriesStatistic', stateLabel: 'Category Sales Report', access: 3 },
                        //{ name: 'transactionRefundReport', stateName: 'index.oneCol.transactionRefundReport', stateLabel: 'Transaction & Refund Report', access: 3 },
				    ]
				},
				{
				    name: 'wholesale',
				    stateLabel: 'Wholesale',
				    subMenu: [
                    //    { name: 'wholesaleSummaryReport', stateName: 'index.oneCol.wholesaleSummaryReport', stateLabel: 'Wholesale Summary Report', access: 3 },
                    //    { name: 'wholesaleDropShipOrdersReport', stateName: 'index.oneCol.wholesaleDropShipOrdersReport', stateLabel: 'Wholesale Drop Ship Orders Report', access: 3 },
				    ]
				},
				{
				    name: 'affiliates',
				    stateLabel: 'Affiliates',
				    subMenu: [
                        { name: 'affiliateSummaryInformation', stateName: 'index.oneCol.affiliatesSummaryReport', stateLabel: 'Affiliate Summary Information', access: 3 },
				    ]
				},
				{
				    name: 'operations',
				    stateLabel: 'Operations',
				    subMenu: [
                        //{ name: 'weeklyAgentReport', stateName: 'index.oneCol.weeklyAgentReport', stateLabel: 'Weekly Agent Report', access: 3 },
                        //{ name: 'serviceCodeReport', stateName: 'index.oneCol.serviceCodeReport', stateLabel: 'Service Code Report', access: 3 },
                        //{ name: 'productQualityIssuesReport', stateName: 'index.oneCol.productQualityIssuesReport', stateLabel: 'Product Quality Issues Report', access: 3 },
                        //{ name: 'kPIReport', stateName: 'index.oneCol.kPIReport', stateLabel: 'KPI Report (APIs)', access: 3 },
                        { name: 'vitalGreen', stateName: 'index.oneCol.vitalGreen', stateLabel: 'Review VitalGreen Statistics', access: 3 },
				    ]
				},
				{
				    name: 'listProcessingAnalysis',
				    stateLabel: 'List Processing, Analysis',
				    subMenu: [
                        //{ name: 'matchbackPostSeasonAnalysisReport', stateName: 'index.oneCol.matchbackPostSeasonAnalysisReport', stateLabel: 'Matchback and Post-Season Analysis Report', access: 3 },
                        { name: 'giftCertificateReport', stateName: 'index.oneCol.manageGCOrders', stateLabel: 'Gift Certificate Report', access: 3 },
                        //{ name: 'giftCertificateUsageReport', stateName: 'index.oneCol.giftCertificateUsageReport', stateLabel: 'Gift Certificate Usage Report', access: 3 },
                        //{ name: 'lifeTimeCalculationReport', stateName: 'index.oneCol.lifeTimeCalculationReport', stateLabel: 'Life Time Calculation Report', access: 3 },
                        //{ name: 'mailingListReport', stateName: 'index.oneCol.mailingListReport', stateLabel: 'Mailing List Report', access: 3 },
                        //{ name: 'weeklySalesReport', stateName: 'index.oneCol.weeklySalesReport', stateLabel: 'Weekly Sales Report', access: 3 },
                        //{ name: 'newCustomersReport', stateName: 'index.oneCol.newCustomersReport', stateLabel: 'New Customers Report', access: 3 },
				    ]
				},
                { name: 'catalogRequests', stateName: 'index.oneCol.catalogRequests', stateLabel: 'Catalog Requests', access: 3 },
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
                { name: 'manageStyles', stateName: 'index.oneCol.manageStyles', stateLabel: 'Manage Custom CSS', access: 6 },
				{ name: 'filesManagement', stateName: 'index.oneCol.filesManagement', stateLabel: 'Manage Media', access: 12 },
                { name: 'manageEmailTemplate', stateName: 'index.oneCol.manageEmailTemplates', stateLabel: 'Manage Email Templates', access: 6 },
			]
		},
		{
			name: 'tools',
			stateLabel: 'Tools',
			subMenu: [
				{ name: 'manageProductTaxCodes', stateName: 'index.oneCol.manageProductTaxCodes', stateLabel: 'Manage Product Tax Codes', access: 8 },
				//{ name: 'emailAddressProfiles', stateName: 'index.oneCol.emailAddressProfiles', stateLabel: 'Multiple Email Address Profiles', access: 8 },
				{ name: 'moveOrder', stateName: 'index.oneCol.moveOrder', stateLabel: 'Reassign Transaction', access: 8 },
				{ name: 'changeOrderStatus', stateName: 'index.oneCol.changeOrderStatus', stateLabel: 'Change Order Status', access: 8 },
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
				//{ name: 'viewWiki', stateName: 'index.oneCol.viewWiki', stateLabel: 'View Wiki', access: 11 },
				{ name: 'viewLogs', stateName: 'index.oneCol.manageLogs', stateLabel: 'View Logs', access: 10 }
			]
		}
	];

	return {
		menu : menu
	};

}]);