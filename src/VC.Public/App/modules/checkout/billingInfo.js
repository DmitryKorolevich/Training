$(function() {
	$("body").on("click", "#GuestCheckout", function() {
		if ($("#GuestCheckout").is(":checked")) {
			$("#Password").closest(".form-group").hide();
			$("#ConfirmPassword").closest(".form-group").hide();
			$("#spGuestNotify").show();
			$("#spPasswordHint").hide();
		} else {
			$("#Password").closest(".form-group").show();
			$("#ConfirmPassword").closest(".form-group").show();
			$("#spGuestNotify").hide();
			$("#spPasswordHint").show();
		}
	});
});