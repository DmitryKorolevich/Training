'use strict';

angular.module('app.modules.customer.controllers.addEditCustomerController', [])
	.controller('addEditCustomerController', [
		'$scope', '$injector', '$filter', 'customerService', 'toaster', 'promiseTracker', '$rootScope', '$q',
		'$state', '$stateParams', 'customerEditService', 'Upload',
		function ($scope, $injector, $filter, customerService, toaster, promiseTracker, $rootScope, $q, $state, $stateParams, customerEditService, Upload) {
			$scope.addEditTracker = promiseTracker("addEdit");
			$scope.deleteFileTracker = promiseTracker("deleteFile");

			function initialize() {
				$scope.logFileRequest = {};

				$scope.editMode = $stateParams.id != null;

				$scope.inceptionDateOpened = false;

				$scope.accountProfileTab = {
					active: true,
					formName: 'profile',
				};
				$scope.shippingAddressTab = {
					active: false,
					formName: 'shipping'
				};
				$scope.customerNotesTab = {
					active: false,
					formName: 'customerNote'
				};
				$scope.paymentInfoTab = {
					active: false,
					formName: 'billing'
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
										$scope.accountProfileTab.Address = $scope.currentCustomer.ProfileAddress;
										$scope.shippingAddressTab.Address = $scope.currentCustomer.Shipping[0];
										$scope.customerNotesTab.CustomerNote = $scope.currentCustomer.CustomerNotes[0];
										$scope.paymentInfoTab.Address = {};
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
										$scope.accountProfileTab.Address = $scope.currentCustomer.ProfileAddress;
										$scope.paymentInfoTab.PaymentMethodType = $scope.currentCustomer.DefaultPaymentMethod;
										$scope.paymentInfoTab.Address = {};
										angular.forEach($scope.currentCustomer.Shipping, function(shippingItem) {
											customerEditService.syncCountry($scope, shippingItem);
											if (shippingItem.Default) {
												$scope.shippingAddressTab.Address = shippingItem;
											}
										});

										customerEditService.syncCountry($scope, $scope.currentCustomer.ProfileAddress);

										angular.forEach($scope.currentCustomer.CreditCards, function (creditCard) {
											creditCard.formName = $scope.paymentInfoTab.formName;
											customerEditService.syncCountry($scope, creditCard.Address);
										});

										if ($scope.currentCustomer.CreditCards && $scope.currentCustomer.CreditCards[0])
											$scope.paymentInfoTab.CreditCard = $scope.currentCustomer.CreditCards[0];

										if ($scope.currentCustomer.Oac) {
											$scope.currentCustomer.Oac.formName = $scope.paymentInfoTab.formName;
											customerEditService.syncCountry($scope, $scope.currentCustomer.Oac.Address);
										}
										if ($scope.currentCustomer.Check) {
											$scope.currentCustomer.Check.formName = $scope.paymentInfoTab.formName;
											customerEditService.syncCountry($scope, $scope.currentCustomer.Check.Address);
										}

										customerEditService.syncDefaultPaymentMethod($scope);
										createCustomerNoteProto();
										customerEditService.showHighPriNotes($scope);
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

			function createCustomerNoteProto() {
				customerService.createCustomerNotePrototype($scope.addEditTracker)
					.success(function(result) {
						if (result.Success) {
							$scope.customerNotesTab.CustomerNote = result.Data;
						} else {
							toaster.pop('error', 'Error!', "Can't process customer notes");
						}
					}).
					error(function(result) {
						toaster.pop('error', "Error!", "Server error ocurred");
					})
					.then(function() {
						$scope.forms.submitted['customerNote'] = false;
					});
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
				$.each($scope.tabs, function(index, item) {
					if (item.formName == formName) {
						item.active = true;
						return false;
					}
				});
			};

			function successHandler(result) {
				if (result.Success) {
					toaster.pop('success', "Success!", "Successfully saved");
				} else {
					var messages = "";
					if (result.Messages) {
						$scope.forms.submitted['profile'] = true;
						$scope.forms.submitted['shipping'] = true;
						$scope.forms.submitted['customerNote'] = true;
						$scope.forms.submitted['billing'] = true;
						$scope.forms.submitted['customerFile'] = true;
						$scope.serverMessages = new ServerMessages(result.Messages);
						var formForShowing = null;
						$.each(result.Messages, function(index, value) {
							if (value.Field) {
								if (value.Field.indexOf("::") >= 0) {
									var arr = value.Field.split("::");
									var formName = arr[0];
									var fieldName = arr[1];
									var form = $scope.forms[formName];
									if (form[fieldName] != undefined) {
										form[fieldName].$setValidity("server", false);
										if (formForShowing == null) {
											formForShowing = formName;
										}
										return false;
									}
								} else {
									$.each($scope.forms, function(index, form) {
										if (form && index !== "submitted") {
											if (form[value.Field] != undefined) {
												form[value.Field].$setValidity("server", false);
												if (formForShowing == null) {
													formForShowing = index;
												}
												return false;
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
					$scope.forms.submitted['customerNote'] = true;
					$scope.forms.submitted['billing'] = true;
					$scope.forms.submitted['customerFile'] = true;
				}
			};
            
			$scope.deleteCustomerNote = function(id) {
				var idx = -1;

				angular.forEach($scope.currentCustomer.CustomerNotes, function(item, index) {
					if (item.Id == id) {
						idx = index;
						return;
					}
				});

				$scope.currentCustomer.CustomerNotes.splice(idx, 1);
			}

			$scope.addNewCustomerNote = function() {
				clearServerValidation();

				if ($scope.forms.customerNote.$valid) {
					$scope.currentCustomer.CustomerNotes.push(angular.copy($scope.customerNotesTab.CustomerNote));
					createCustomerNoteProto();
				} else {
					$scope.forms.submitted['customerNote'] = true;
				}
			};

			$scope.upload = function (files) {
				
				var logRequest = $scope.logFileRequest;
				logRequest.type = "upload";
				logRequest.progress = 0;
				logRequest.state = "progress";

				if (files && files.length > 0) {

					logRequest.name = files[0].FileName;

					$scope.uploading = true;
					Upload.upload({
						url: '/api/customer/UploadCustomerFile',
						data: $scope.currentCustomer.PublicId,
						file: files[0]
					}).progress(function(evt) {
						var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
						logRequest.progress = progressPercentage;

						console.log('progress: ' + progressPercentage + '% ' + evt.config.file.name);
					}).success(function (result, status, headers, config) {
						if (result.Success) {
							logRequest.progress = 100;
							logRequest.state = 'done';

							var resFile = result.Data;
							resFile.Description = $scope.currentCustomer.currentFileDescription;
							$scope.currentCustomer.Files.push(resFile);

							console.log('file ' + config.file.name + 'uploaded. Response: ' + result);
						} else {
							logRequest.progress = 100;
							logRequest.state = 'error';

							toaster.pop('error', 'Error!', "Can't upload file");
						}

						files = [];
						$scope.currentCustomer.currentFileDescription = "";

						$scope.uploading = false;
					}).error(function (data, status, headers, config) {
						$scope.uploading = false;

						logRequest.progress = 100;
						logRequest.state = 'error';

						toaster.pop('error', "Error!", "Server error ocurred");

						console.log('error status: ' + status);
					});
				}
			};

			$scope.deleteFile = function (index, fileName) {
				customerService.deleteCustomerFile($scope.currentCustomer.PublicId, fileName, $scope.deleteFileTracker).success(function(result) {
						if (result.Success) {
							$scope.currentCustomer.Files.splice(index, 1);
						} else {
							toaster.pop('error', 'Error!', "Can't delete customer file");
						}
					}).
					error(function(result) {
						toaster.pop('error', "Error!", "Server error ocurred");
					});
			};

			initialize();
		}
	]);