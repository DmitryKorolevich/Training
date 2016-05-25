$(function () {
	controlGuestCheckout();

	$("body").on("click", "#GuestCheckout", function() {
		controlGuestCheckout();
	});

	$("body").on("change", "#ddCreditCardsSelection", function () {
		changeSelection($("#ddCreditCardsSelection").val());
	});

	$(".columns-container form").data("validator").settings.submitHandler = function (form)
	{
	    $(".columns-container .overlay").show();
	    setTimeout(function ()
	    {
	        form.submit();
	    }, 100);
	};

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

		$(".phone-mask").mask("(999) 999-9999? x99999");

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