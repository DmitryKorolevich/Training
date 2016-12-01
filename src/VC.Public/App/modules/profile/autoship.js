$(function () {
	$("body").on("click", ".delete", function () {
		var id = $(this).attr("data-autoship-id");
		
		confirmAction(function () {
			$(".overlay").show();
			deleteAutoShip(id, function (result) {
				successHandler(result, "Successfully deleted");
			}, function (errorResult) {
				$(".overlay").hide();
				notifyError();
			});
		});
	});

	$("body").on("click", ".pause", function () {
		var id = $(this).attr("data-autoship-id");
		confirmAction(function () {
			$(".overlay").show();
			activatePauseAutoShip(id, false, function (result) {
				successHandler(result, "Successfully paused");
			}, function (errorResult) {
				$(".overlay").hide();
				notifyError();
			});
		});
	});

	$("body").on("click", ".activate", function () {
		var id = $(this).attr("data-autoship-id");
		
		confirmAction(function () {
			$(".overlay").show();
			activatePauseAutoShip(id, true, function (result) {
				successHandler(result, "Successfully started");
			}, function (errorResult) {
				$(".overlay").hide();
				notifyError();
			});
		});
	});

	$("body").on("click", ".edit-billing", function() { billingDetailsDialog(this, $(this).attr("data-autoship-billing")) });
});

function successHandler(result, successMessage) {
	$(".overlay").hide();
	if (result.Success) {
		refreshGrid();

		notifySuccess(successMessage);
	} else {
		notifyError(result.Messages[0].Message);
	}
}

function deleteAutoShip(id, successCallback, errorCallback) {
	$.ajax({
		type: "POST",
		url: "/Profile/DeleteAutoShip/" + id,
		dataType: "json"
	}).success(function (result) {
		if (successCallback) {
			successCallback(result);
		}
	}).error(function (result) {
		if (errorCallback) {
			errorCallback(result);
		}
	});
}

function activatePauseAutoShip(id, activate, successCallback, errorCallback) {
	$.ajax({
		type: "POST",
		url: "/Profile/ActivatePauseAutoShip?id=" + id + "&activate=" + activate,
		dataType: "json"
	}).success(function (result) {
		if (successCallback) {
			successCallback(result);
		}
	}).error(function (result) {
		if (errorCallback) {
			errorCallback(result);
		}
	});
}

function refreshGrid() {
	$(".overlay").show();
	$.ajax({
		type: "POST",
		url: "/Profile/RefreshAutoShipHistory",
		dataType: "html"
	}).success(function (result) {
		$("#gridcontainer").html(result);

		$('.tooltip-v').each(function () {
			var title = $(this).data("tooltip-title");
			var body = $(this).data("tooltip-body");
			settingsVertical.content = getBaseHtml(title, body);
			$(this).tooltipster(settingsVertical);
		});

		$(".overlay").hide();
	}).error(function (result) {
		notifyError();
		$(".overlay").hide();
	});
}

function billingDetailsDialog(jSelf, s) {
    var l = Ladda.create(jSelf)
    l.start();

	$.ajax({
		url: "/Profile/AutoShipBillingDetails?orderId=" + s,
		dataType: "html"
	}).success(function (result) {
		$(result).dialog({
			resizable: false,
			modal: true,
			minWidth: 750,
			close: function () {
				$(this).dialog('destroy').remove();
			},
			open: function () {
				refreshCountries();
				populateCardTypes();
				$("#ddCreditCardsSelection").val('0');
			},
			buttons: [
                {
                	text: "Save",
                	'class': "main-dialog-button",
                	click: function () {
                		var selector = "#autoShipDialog form";
                		var jForm = $(selector);
                		reparseElementValidators(selector);
                		jForm.validate()
                		if (jForm.valid()) {
                			jForm.submit();
                		}
                	}
                },
                {
                	text: "Cancel",
                	click: function () {
                		$(this).dialog("close");
                	}
                }
			]
		});
	}).error(function (result) {
		notifyError();
	}).complete(function() {
	    l.stop();
	});

	return false;
}

function submitSuccess() {
	if (success == 'True') {
		notifySuccess("Billing details updated");
		$("#autoShipDialog").dialog("close");
		refreshGrid();
	}
}

function submitError() {
	notifyError("Server error occured");
}