$(function() {
	$("body").on("click", "#btnPlaceOrder", function() {
		$("#viewCartForm").submit();
	})

	$("input[type=submit]").prop("disabled", false);
});