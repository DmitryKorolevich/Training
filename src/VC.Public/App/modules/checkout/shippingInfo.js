$(function () {
	controlSectionState("#ddShippingAddressesSelection", "#chkSelectOther");
	controlSectionState("#GiftMessage", "#IsGiftOrder");
	controlUseBillingState(".form-two-column", "#UseBillingAddress");

	$("body").on("click", "#chkSelectOther", function() { controlSectionState("#ddShippingAddressesSelection", "#chkSelectOther"); });
	$("body").on("click", "#IsGiftOrder", function () { controlSectionState("#GiftMessage", "#IsGiftOrder"); });
	$("body").on("click", "#UseBillingAddress", function () { controlUseBillingState(".form-two-column", "#UseBillingAddress"); });

	$("body").on("change", "#ddShippingAddressesSelection", function () {
		changeSelection($("#ddShippingAddressesSelection").val());
	});

	controlUpdateSavedState();
});

function controlUpdateSavedState() {
	if ($("#ShipAddressIdToOverride").val() == null || $("#ShipAddressIdToOverride").val() == "") {
		$("#SaveToProfile").prop("checked", false);
		$("#SaveToProfile").trigger("change");
		$("#updateSaved").hide();
	} else {
		$("#updateSaved").show();
	}
}

function changeSelection(selId) {
	$.ajax({
		url: "/Checkout/GetShippingAddress/" + selId,
		dataType: "html"
	}).success(function (result) {
		$("#dynamicArea").html(result);

		refreshCountries();

		controlUpdateSavedState();

		$(".phone-mask").mask("(999) 999-9999? x99999");

		reparseElementValidators("form");
	}).error(function (result) {
		notifyError();
	});
}

function controlSectionState(selector, controlId) {
	jSel = $(selector).closest(".form-group");

	if ($(controlId).is(":checked")) {
		jSel.show();
	} else {
		jSel.hide();
	}
}

function controlUseBillingState(selector, controlId) {
	jSel = $(selector);
	jDrop = $("#ddShippingAddressesSelection").closest(".form-group");

	jChk = $("#SaveToProfile");
	jChkContainer = $("#updateSaved");

	jSpan = $("#spEnterAddress");

	if ($(controlId).is(":checked")) {
		jSel.hide();
		jDrop.hide();
		jChk.prop("checked", false);
		jChk.trigger("change");
		jChkContainer.hide();
		jSpan.hide();
	} else {
		jSel.show();
		jDrop.show();
		jChkContainer.show();
		jSpan.show();

		//controlSectionState("#ddShippingAddressesSelection", "#chkSelectOther");
	}
}
