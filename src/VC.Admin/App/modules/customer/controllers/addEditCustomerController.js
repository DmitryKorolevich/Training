'use strict';

angular.module('app.modules.customer.controllers.addEditCustomerController', [])
	.controller('addEditCustomerController', [
		'$scope', '$injector', '$filter', 'customerService', 'toaster', 'promiseTracker', '$rootScope', '$q',
		'$state', '$stateParams', 'customerEditService',
		function ($scope, $injector, $filter, customerService, toaster, promiseTracker, $rootScope, $q, $state, $stateParams, customerEditService) {
			$scope.addEditTracker = promiseTracker("addEdit");
			$scope.resetTracker = promiseTracker("reset");
			$scope.resendTracker = promiseTracker("resend");

			function refreshHistory()
			{
			    if ($scope.currentCustomer && $scope.currentCustomer.Id)
			    {
			        var data = {};
			        data.service = customerService;
			        data.tracker = $scope.addEditTracker;
			        data.idObject = $scope.currentCustomer.Id;
			        data.idObjectType = 6//customer
			        $scope.$broadcast('objectHistorySection#in#refresh', data);
			    }
			}

			function initialize() {
				$scope.editMode = $stateParams.id != null;

				$scope.inceptionDateOpened = false;
				$scope.options = {};

				$scope.accountProfileTab = {
					active: true,
					formName: 'profile',
				};
				$scope.shippingAddressTab = {
					active: false,
					formName: 'shipping',
					ShippingEditModels: {}
				};
				$scope.customerNotesTab = {
					active: false,
					formName: 'customerNote'
				};
				$scope.paymentInfoTab = {
					active: false,
					formNames: ['card', 'oac', 'check'],
					AddressEditModels: {}
				};
				$scope.customerFilesTab = {
					active: false,
					formName: 'customerFile'
				};
				$scope.creditCardTypes = $rootScope.ReferenceData.CreditCardTypes;
				var tabs = [];
				tabs.push($scope.accountProfileTab);
				tabs.push($scope.shippingAddressTab);
				tabs.push($scope.paymentInfoTab);
				tabs.push($scope.customerNotesTab);
				tabs.push($scope.customerFilesTab);
				$scope.tabs = tabs;

				customerEditService.initBase($scope);
				customerEditService.initCustomerEdit($scope);

				$scope.forms = {};

				$scope.forms.submitted = [];

				$q.all({ countriesCall: customerService.getCountries($scope.addEditTracker) }).then(function(result) {
					if (result.countriesCall.data.Success) {
						$scope.countries = result.countriesCall.data.Data;

						if (!$scope.editMode) {
							customerService.createCustomerPrototype($scope.addEditTracker)
								.success(function(result) {
									if (result.Success) {
									    $scope.currentCustomer = result.Data;
									    $scope.options.DBStatusCode = $scope.currentCustomer.StatusCode;
										$scope.accountProfileTab.Address = $scope.currentCustomer.ProfileAddress;
										$scope.shippingAddressTab.AddressIndex = "0";
										$scope.customerNotesTab.CustomerNote = $scope.currentCustomer.CustomerNotes[0];
										$scope.paymentInfoTab.Address = {};

										refreshHistory();
										initCustomerFiles();
										initCustomerNotes();
										initOrdersList();
									} else {
										toaster.pop('error', 'Error!', "Can't load customer prototype");
									}
								}).
								error(function(result) {
									toaster.pop('error', "Error!", "Server error ocurred");
								});
						} else {
							customerService.getExistingCustomer($stateParams.id, $scope.addEditTracker)
								.success(function(result) {
									if (result.Success) {
									    $scope.currentCustomer = result.Data;
									    $scope.options.DBStatusCode = $scope.currentCustomer.StatusCode;
										$scope.accountProfileTab.Address = $scope.currentCustomer.ProfileAddress;
										$scope.paymentInfoTab.PaymentMethodType = $scope.currentCustomer.DefaultPaymentMethod;
										$scope.paymentInfoTab.Address = {};
										angular.forEach($scope.currentCustomer.Shipping, function(shippingItem, index) {
											customerEditService.syncCountry($scope, shippingItem);
											if (shippingItem.Default) {
											    $scope.shippingAddressTab.AddressIndex = index.toString();
											}
										});

										customerEditService.syncCountry($scope, $scope.currentCustomer.ProfileAddress);

										angular.forEach($scope.currentCustomer.CreditCards, function (creditCard) {
											creditCard.formName = 'card';
											customerEditService.syncCountry($scope, creditCard.Address);
										});

										if ($scope.currentCustomer.CreditCards && $scope.currentCustomer.CreditCards[0])
											$scope.paymentInfoTab.CreditCardIndex = "0";

										if ($scope.currentCustomer.Oac) {
											$scope.currentCustomer.Oac.formName = 'oac';
											customerEditService.syncCountry($scope, $scope.currentCustomer.Oac.Address);
										}
										if ($scope.currentCustomer.Check) {
											$scope.currentCustomer.Check.formName = 'check';
											customerEditService.syncCountry($scope, $scope.currentCustomer.Check.Address);
										}

										customerEditService.syncDefaultPaymentMethod($scope);
										customerEditService.showHighPriNotes($scope);
                                        
										refreshHistory();
										initCustomerFiles();
										initCustomerNotes();
										initOrdersList();
									} else {
										toaster.pop('error', 'Error!', "Can't load customer");
									}
								}).
								error(function(result) {
									toaster.pop('error', "Error!", "Server error ocurred");
								});
						}
					} else {
						toaster.pop('error', 'Error!', "Can't get reference data");
					}
				}, function(result) {
					toaster.pop('error', "Error!", "Server error ocurred");
				});
			};

			function initCustomerFiles()
			{
			    var data = {};
			    data.files = $scope.currentCustomer.Files;
			    data.publicId = $scope.currentCustomer.PublicId;
			    data.addEditTracker = $scope.addEditTracker;
			    $scope.$broadcast('customerFiles#in#init', data);
			};

			function initCustomerNotes()
			{
			    var data = {};
			    data.customerNotes = $scope.currentCustomer.CustomerNotes;
			    data.addEditTracker = $scope.addEditTracker;
			    $scope.$broadcast('customerNotes#in#init', data);
			};

			function initOrdersList()
			{
			    var data = {};
			    data.idCustomer = $scope.currentCustomer.Id;
			    $scope.$broadcast('customerOrders#in#init', data);
			};

			function clearServerValidation() {
				$.each($scope.forms, function(index, form) {
					if (form && !(typeof form === 'boolean')) {
						$.each(form, function(index, element) {
							if (element && element.$name == index) {
								element.$setValidity("server", true);
							}
						});
					}
				});
			};

			function activateTab(formName) {
				$.each($scope.tabs, function (index, item)
				{
				    var itemForActive = null;
				    if (item.formName == formName)
				    {
				        itemForActive = item;
				    }
				    if (item.formNames)
				    {
				        $.each(item.formNames, function (index, form)
				        {
				            if (form == formName)
				            {
				                itemForActive = item;
				                return false;
				            }
				        });
				    }
				    if (itemForActive)
				    {
				        itemForActive.active = true;
				        return false;
				    }
				});
			};

			function successHandler(result) {
			    if (result.Success)
			    {
			        if (!$scope.currentCustomer.Id)
			        {
			            $state.go('index.oneCol.orderAdd', { idcustomer: result.Data.Id });
			        }
			        $scope.currentCustomer.Id = result.Data.Id;
			        $scope.options.DBStatusCode = result.Data.StatusCode;
			        refreshHistory();
					toaster.pop('success', "Success!", "Successfully saved");
				} else {
					var messages = "";
					if (result.Messages) {
						$scope.forms.submitted['profile'] = true;
						$scope.forms.submitted['shipping'] = true;
						$scope.forms.submitted['card'] = true;
						$scope.forms.submitted['oac'] = true;
						$scope.forms.submitted['check'] = true;
						$scope.serverMessages = new ServerMessages(result.Messages);
						var formForShowing = null;
						var form;
						$.each(result.Messages, function(index, value) {
							if (value.Field) {
								if (value.Field.indexOf("::") >= 0) {
									var arr = value.Field.split("::");
									var formName = arr[0];
									var fieldName = arr[1];
									if (fieldName.indexOf(".") >= 0) {
									    arr = fieldName.split('.');
									    var collectionName = arr[0];
									    var indexWithName = arr[1];
									    switch(collectionName)
									    {
									        case 'Shipping':
									            var collectionIndex = indexWithName.split('i')[1];
									            $scope.shippingAddressTab.AddressIndex = collectionIndex;
									            form = $scope.forms[formName];
									            fieldName = arr[2];
									            if (form[fieldName] != undefined) {
									                form[fieldName].$setValidity("server", false);
									                if (formForShowing == null) {
									                    formForShowing = formName;
									                }
									            }
									            break;
									        case 'CreditCards':
									            var collectionIndex = indexWithName.split('i')[1];
									            $scope.paymentInfoTab.CreditCardIndex = collectionIndex;
									            form = $scope.forms[formName];
									            fieldName = arr[2];
									            if (form[fieldName] != undefined) {
									                form[fieldName].$setValidity("server", false);
									                if (formForShowing == null) {
									                    formForShowing = formName;
									                }
									            }
									            break;
									    }
									}
									form = $scope.forms[formName];
									if (form[fieldName] != undefined) {
										form[fieldName].$setValidity("server", false);
										if (formForShowing == null) {
											formForShowing = formName;
										}
									}
								} else {
									$.each($scope.forms, function(index, form) {
										if (form && index !== "submitted") {
											if (form[value.Field] != undefined) {
												form[value.Field].$setValidity("server", false);
												if (formForShowing == null) {
													formForShowing = index;
												}
											}
										}
									});
								}
							}
							messages += value.Message + "<br />";
						});

						if (formForShowing) {
							activateTab(formForShowing);
						}
					}
					toaster.pop('error', "Error!", messages, null, 'trustedHtml');
				}
			};

			function errorHandler(result) {
				toaster.pop('error', "Error!", "Server error occured");
			};

			function setCountryValidity() {
				$.each($scope.forms, function(index, form) {
					if (form && !(typeof form === 'boolean')) {
						if (form.Country && form.Country.$viewValue && form.Country.$viewValue.Id == 0) {
							form.Country.$setValidity("required", false);
						}
						if (form.State && form.State.$viewValue == 0) {
							form.State.$setValidity("required", false);
						}
					}
				});
			};

			$scope.save = function() {
				clearServerValidation();

				var valid = true;

				setCountryValidity();

				$.each($scope.forms, function(index, form) {
					if (form && !(typeof form === 'boolean')) {
						if (!form.$valid && index != 'submitted') {
							valid = false;
							activateTab(index);
							return false;
						}
					}
				});

				if (valid) {
					if ($scope.currentCustomer.newEmail || $scope.currentCustomer.emailConfirm) {
						$scope.currentCustomer.Email = $scope.currentCustomer.newEmail;
						$scope.currentCustomer.EmailConfirm = $scope.currentCustomer.emailConfirm;
					} else {
						$scope.currentCustomer.EmailConfirm = $scope.currentCustomer.Email;
					}

					if (!$scope.editMode) {
						$scope.currentCustomer.Shipping[0].Default = true;
					}

					$scope.saving = true;
					customerService.createUpdateCustomer($scope.currentCustomer, $scope.addEditTracker).success(function(result) {
							successHandler(result);
						}).
						error(function(result) {
							errorHandler(result);
						});
				} else {
					$scope.forms.submitted['profile'] = true;
					$scope.forms.submitted['shipping'] = true;
					$scope.forms.submitted['card'] = true;
					$scope.forms.submitted['oac'] = true;
					$scope.forms.submitted['check'] = true;
					toaster.pop('error', "Error!", "Validation errors, please correct field values.", null, 'trustedHtml');
				}
			};

			$scope.resend = function () {
				customerService.resendActivation($scope.currentCustomer.PublicUserId, $scope.resendTracker)
					.success(function (result) {
						if (result.Success) {
							toaster.pop('success', "Success!", "Successfully sent");

						} else {
							var messages = "";
							if (result.Messages) {
								$.each(result.Messages, function (index, value) {
									messages += value.Message + "<br />";
								});
							}
							toaster.pop('error', "Error!", messages, null, 'trustedHtml');
						}
					}).error(function () {
						toaster.pop('error', "Error!", "Server error occured");
					});
			};

			$scope.resetPassword = function () {
				customerService.resetPassword($scope.currentCustomer.PublicUserId, $scope.resetTracker)
					.success(function (result) {
						if (result.Success) {
							toaster.pop('success', "Success!", "Successfully reset");
						} else {
							var messages = "";
							if (result.Messages) {
								$.each(result.Messages, function (index, value) {
									messages += value.Message + "<br />";
								});
							}
							toaster.pop('error', "Error!", messages, null, 'trustedHtml');
						}
					}).error(function () {
						toaster.pop('error', "Error!", "Server error occured");
					});
			};

			initialize();
		}
	]);