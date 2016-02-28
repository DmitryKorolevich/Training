'use strict';

angular.module('app.modules.content.controllers.contentCrossSellsManageController', [])
	.controller('contentCrossSellsManageController', [
		'$scope', '$rootScope', '$state', '$stateParams', 'contentService', 'toaster', 'promiseTracker', 'productService', '$timeout',
		function ($scope, $rootScope, $state, $stateParams, contentService, toaster, promiseTracker, productService, $timeout) {
			$scope.refreshTracker = promiseTracker("get");
			$scope.editTracker = promiseTracker("edit");

			$scope.sortableOptions = {
				handle: ' .sortable-move',
				items: ' .panel:not(.panel-heading)',
				axis: 'y',
				start: function (e, ui) {
					$scope.dragging = true;
				},
				stop: function (e, ui) {
					$scope.dragging = false;
				}
			}

			$scope.getSku = function (val) {
				if (val) {
					$scope.skusFilter.Code = val;
					$scope.skusFilter.DescriptionName = '';
					$scope.skusFilter.NotHiddenOnly = true;
					$scope.skusFilter.ActiveOnly = true;
					return productService.getSkus($scope.skusFilter)
						.then(function (result) {
							$scope.bufferedSkus =  result.data.Data.map(function (item) {
								return item;
							});

							return $scope.bufferedSkus;
						});
				}
			};

			$scope.cleaerTypeaheadBox = function (crossSellProduct) {
				crossSellProduct.IdSku = null;
				crossSellProduct.WholesalePrice = null;
				crossSellProduct.RetailPrice = null;
			};

			$scope.skuChanged = function (selectedItem, index) {
				var crossSell = $scope.model.Items[index];

				crossSell.IdSku = selectedItem.Id;
				crossSell.WholesalePrice = selectedItem.WholesalePrice;
				crossSell.RetailPrice = selectedItem.Price;
				crossSell.SkuCode = selectedItem.Code;
			};

			$scope.save = function () {
				clearServerValidation();
				
				var valid = true;

				if (!$scope.forms.Items.$valid) {
					valid = false;
				}

				if (valid) {
					contentService.updateContentCrossSells($scope.model, $scope.refreshTracker).success(function (result) {
						successSaveHandler(result);
					}).error(function (result) {
						errorHandler(result);
					});
				} else {
					$scope.forms.submitted = true;
					toaster.pop('error', "Error!", "Validation errors, please correct field values.", null, 'trustedHtml');
				}
			};

			var clearServerValidation = function() {
				$.each($scope.forms.Items, function (index, subForm) {
					if (index.indexOf('i') == 0) {
						$.each(subForm, function(index, element) {
							if (element && element.$name == index) {
								element.$setValidity("server", true);
							}
						});
					}
				});
			};

			function initialize() {
				$scope.id = $state.current.name == "index.oneCol.manageAddToCartCs" ? 1 : 2;

				$scope.title = $scope.id == 1 ? "Manage Add To Cart Cross Selling" : "Manage View Cart Cross Selling";

				$scope.skusFilter = {};

				refreshCrossSells();

				$scope.forms = {};
			};

			function refreshCrossSells() {
				contentService.getContentCrossSells($scope.id, $scope.refreshTracker)
                .success(function (result) {
                	if (result.Success) {
                		$scope.model = result.Data;
                	} else {
                		errorHandler(result);
                	}
                })
                .error(function (result) {
                	errorHandler(result);
                });
			}

			function successSaveHandler(result) {
				if (result.Success) {
					toaster.pop('success', "Success!", "Successfully saved.");
					$scope.model = result.Data;
				} else {
					var messages = "";
					if (result.Messages) {
						$scope.forms.submitted = true;

						$scope.serverMessages = new ServerMessages(result.Messages);
						$.each(result.Messages, function (index, value) {
							if (value.Field) {
								if (value.Field.indexOf('.') > -1) {
									var items = value.Field.split(".");
									$scope.forms.Items[items[0]][items[1]][items[2]].$setValidity("server", false);
								}
								else {
									$.each($scope.forms, function (index, form) {
										if (form && !(typeof form === 'boolean')) {
											if (form[value.Field] != undefined) {
												form[value.Field].$setValidity("server", false);
												return false;
											}
										}
									});
								}
							}
							else {
								messages += value.Message + "<br/>";
							}
						});
					}
					toaster.pop('error', "Error!", messages, null, 'trustedHtml');
				}
			};

			function errorHandler(result) {
				toaster.pop('error', "Error!", "Server error occured");
			};

			initialize();
		}
	]);