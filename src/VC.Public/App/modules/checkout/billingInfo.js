$(function () {
	controlGuestCheckout();

	$("body").on("click", "#GuestCheckout", function() {
		controlGuestCheckout();
	});

	$("body").on("change", "#ddCreditCardsSelection", function () {
		changeSelection($("#ddCreditCardsSelection").val());
	});

	populateCardTypes();
});

function changeSelection(selId) {
	$.ajax({
		url: "/Checkout/GetBillingAddress/" + selId,
		dataType: "html"
	}).success(function (result) {
		$("#dynamicArea").html(result);

		refreshCountries();
		populateCardTypes();

		$('.tooltip-v').each(function () {
			var title = $(this).data("tooltip-title");
			var body = $(this).data("tooltip-body");
			settingsVertical.content = getBaseHtml(title, body);
			$(this).tooltipster(settingsVertical);
		});

		reparseElementValidators("form");
	}).error(function (result) {
		notifyError();
	});
}

function controlGuestCheckout() {
	if ($("#GuestCheckout").is(":checked")) {
		$("#Password").val("password");
		$("#Password").closest(".form-group").hide();
		$("#ConfirmPassword").val("password");
		$("#ConfirmPassword").closest(".form-group").hide();
		$("#spGuestNotify").show();
		$("#spPasswordHint").hide();
	} else {
		$("#Password").val("");
		$("#Password").closest(".form-group").show();
		$("#ConfirmPassword").val("");
		$("#ConfirmPassword").closest(".form-group").show();
		$("#spGuestNotify").hide();
		$("#spPasswordHint").show();
	}
}