$(function() {
	$("body").on("click", "#chkSelectOther", function() { controlSectionState("#ddShippingAddressesSelection", "#chkSelectOther"); });
	$("body").on("click", "#IsGiftCertificate", function () { controlSectionState("#GiftMessage", "#IsGiftCertificate"); });
	$("body").on("click", "#UseBillingAddress", function() { controlUseBillingState(".two-columns-block", "#UseBillingAddress"); });
});

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
	jChk = $("#chkSelectOther").closest(".form-group");
	jSpan = $("#spEnterAddress");

	if ($(controlId).is(":checked")) {
		jSel.hide();
		jDrop.hide();
		jChk.hide();
		jSpan.hide();
	} else {
		jSel.show();
		jDrop.show();
		jChk.show();
		jSpan.show();
	}
}
