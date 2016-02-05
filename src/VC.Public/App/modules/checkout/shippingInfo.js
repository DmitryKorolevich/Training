$(function() {
	$("body").on("click", "#chkSelectOther", function() { controlSectionState("#ddShippingAddressesSelection", "#chkSelectOther"); });
	$("body").on("click", "#IsGiftCertificate", function () { controlSectionState("#GiftMessage", "#IsGiftCertificate"); });
	$("body").on("click", "#UseBillingAddress", function () { controlUseBillingState(".form-two-column", "#UseBillingAddress"); });

	$('body').on("keyup", "textarea", function (elem) { processCharcount(elem); });
});

function controlSectionState(selector, controlId) {
	jSel = $(selector).closest(".form-group");

	if ($(controlId).is(":checked")) {
		jSel.show();
	} else {
		jSel.hide();
	}
}

function processCharcount(ev) {
	var elem = ev.target;

	var max = $(elem).attr("maxlength");
	var len = $(elem).val().length;
	if (len >= max) {
		$(elem).next().html('you have reached the limit');
	} else {
		var char = max - len;
		$(elem).next().html('<b>' + char + '</b> characters remaining');
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

		controlSectionState("#ddShippingAddressesSelection", "#chkSelectOther");
	}
}
