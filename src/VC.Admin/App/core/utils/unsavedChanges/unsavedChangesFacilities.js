'use strict';

angular.module('app.core.utils.unsavedChanges.unsavedChangesFacilities', [])
	.service('unsavedWarningSharedService', ['confirmUtil',
		'$rootScope', '$injector', 'ngProgress',
		function (confirmUtil, $rootScope, $injector, ngProgress) {

			// Controller scopped variables
			var _this = this;
			var allForms = [];
			var areAllFormsClean = true;
			var removeFunctions = [angular.noop];

			// @note only exposed for testing purposes.
			this.allForms = function() {
				return allForms;
			};

			this.allFormsCleanUp = function () {
				angular.forEach(_this.allForms(), function (item, idx) {
					if (item.$dirty) {
						item.$setPristine();
					}
				});
			};

			// save shorthand reference to messages
			var messages = {
				navigate: 'You will lose unsaved changes if you leave this page. Are you sure you want to proceed?',
				reload: 'You will lose unsaved changes if you reload this page'
			};

			// Check all registered forms 
			// if any one is dirty function will return true

			function allFormsClean() {
				areAllFormsClean = true;
				angular.forEach(allForms, function(item, idx) {
					if (item.$dirty) {
						areAllFormsClean = false;
					}
				});
				return areAllFormsClean; // no dirty forms were found
			}

			// adds form controller to registered forms array
			// this array will be checked when user navigates away from page
			this.init = function(form) {
				if (allForms.length === 0) setup();
				allForms.push(form);
			};

			this.removeForm = function(form) {
				var idx = allForms.indexOf(form);

				// this form is not present array
				// @todo needs test coverage 
				if (idx === -1) return;

				allForms.splice(idx, 1);

				if (allForms.length === 0) tearDown();
			};

			function tearDown() {
				angular.forEach(removeFunctions, function(fn) {
					fn();
				});
				window.onbeforeunload = null;
			}

			// Function called when user tries to close the window
			this.confirmExit = function() {
				// @todo this could be written a lot cleaner! 
				if (!allFormsClean()) return messages.reload;
				tearDown();
			};

			// bind to window close
			// @todo investigate new method for listening as discovered in previous tests

			function setup() {
				window.onbeforeunload = _this.confirmExit;

				var eventsToWatchFor = ['$stateChangeStart'];

				angular.forEach(eventsToWatchFor, function(aEvent) {
					// calling this function later will unbind this, acting as $off()
					var removeFn = $rootScope.$on(aEvent, function(event, next, nextParams) {
						// @todo this could be written a lot cleaner! 
						if (!allFormsClean()) {
						    confirmUtil.confirm(null, messages.navigate, 
                                function ()
                                {
                                	_this.allFormsCleanUp();
                                	$rootScope.$state.go(next.name, nextParams);
                                }, "Close and review all changes", "Proceed and lose all changes");

							ngProgress.reset();
							event.preventDefault();
						}
					});
					removeFunctions.push(removeFn);
				});
			}
		}
	])
	.directive('unsavedWarningClear', [
		'unsavedWarningSharedService',
		function(unsavedWarningSharedService) {
			return {
				scope: true,
				priority: 3000,
				link: function(scope, element, attrs) {
					element.bind('click', function(event) {
						unsavedWarningSharedService.allFormsCleanUp();
					});
				}
			};
		}
	])
	.directive('unsavedWarningForm', [
		'unsavedWarningSharedService',
		function(unsavedWarningSharedService) {
			return {
				require: 'form',
				link: function(scope, formElement, attrs, formCtrl) {

					// register this form
					unsavedWarningSharedService.init(formCtrl);

					// @todo check destroy on clear button too? 
					scope.$on('$destroy', function() {
						unsavedWarningSharedService.removeForm(formCtrl);
					});
				}
			};
		}
	]);