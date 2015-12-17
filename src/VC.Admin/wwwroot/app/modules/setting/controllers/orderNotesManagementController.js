'use strict';

angular.module('app.modules.setting.controllers.orderNotesManagementController', [])
.controller('orderNotesManagementController', ['$scope', 'orderNoteService', 'toaster', 'modalUtil', 'confirmUtil', 'promiseTracker', 'gridSorterUtil', function ($scope, orderNoteService, toaster, modalUtil, confirmUtil, promiseTracker, gridSorterUtil) {
	$scope.refreshTracker = promiseTracker("refresh");
	$scope.deleteTracker = promiseTracker("delete");
	$scope.editTracker = promiseTracker("edit");
	$scope.addTracker = promiseTracker("add");

	function refreshOrderNotes() {
		orderNoteService.getOrderNotes($scope.filter, $scope.refreshTracker)
			.success(function (result) {
				if (result.Success) {
				    $scope.orderNotes = result.Data.Items;
				    $scope.totalItems = result.Data.Count;
				} else {
					toaster.pop('error', 'Error!', "Can't get access to the order specific notes");
				}
			})
			.error(function (result) {
				toaster.pop('error', "Error!", "Server error ocurred");
			});
	};

	function openModal(orderNote, editMode) {
		modalUtil.open('app/modules/setting/partials/addEditOrderNote.html', 'addEditOrderNoteController', {
			orderNote: orderNote, editMode: editMode, thenCallback: function () {
				refreshOrderNotes();
		} });
	}

	function initialize() {
	    $scope.filter = {
	        SearchText: "",
	        Paging: { PageIndex: 1, PageItemCount: 100 },
	        Sorting: gridSorterUtil.resolve(refreshOrderNotes, 'Title', 'Asc')
	    };

	    refreshOrderNotes();
	}

	$scope.pageChanged = function () {
		refreshOrderNotes();
	};

	$scope.filterOrderNotes = function () {
	    $scope.filter.Paging.PageIndex = 1;

	    refreshOrderNotes();
	};

	$scope.open = function (editMode, id) {
		var orderNote = {};
		if (editMode) {
			orderNoteService.getOrderNote(id, $scope.editTracker)
				.success(function (result) {
					if (result.Success) {
						openModal(result.Data, editMode);
					} else {
						var messages = "";
						if (result.Messages) {
							$.each(result.Messages, function(index, value) {
								messages += value.Message + "<br />";
							});
						} else {
							messages = "Can't get order note";
						}

						toaster.pop('error', 'Error!', messages, null, 'trustedHtml');
					}
				}).
				error(function(result) {
					toaster.pop('error', "Error!", "Server error ocurred");
				});
		} else {
			orderNoteService.createOrderNotePrototype($scope.addTracker)
				.success(function (result) {
					if (result.Success) {
						orderNote = result.Data;
						openModal(result.Data, editMode);
					} else {
						toaster.pop('error', 'Error!', "Can't create order note");
					}
				}).
				error(function (result) {
					toaster.pop('error', "Error!", "Server error ocurred");
				});
		}
	};

	$scope.delete = function(name, id) {
		confirmUtil.confirm(function() {
			orderNoteService.deleteOrderNote(id, $scope.deleteTracker)
				.success(function(result) {
					if (result.Success) {
						toaster.pop('success', "Success!", "Successfully deleted");
					} else {
						var messages = "";
						if (result.Messages) {
							$.each(result.Messages, function(index, value) {
								messages += value.Message + "<br />";
							});
						} else {
							messages = "Can't delete order note";
						}

						toaster.pop('error', 'Error!', messages, null, 'trustedHtml');
					}
					refreshOrderNotes();
				})
				.error(function(result) {
					toaster.pop('error', "Error!", "Server error ocurred");
					refreshOrderNotes();
				});
		}, 'Are you sure you want to delete ' + name + ' order specific note?');
	};

	initialize();
}]);