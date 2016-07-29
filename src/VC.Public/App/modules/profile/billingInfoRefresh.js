$(function () {
	$("body").on("change", "#ddCreditCardsSelection", function () {
		changeSelection($("#ddCreditCardsSelection").val());
	});

	populateCardTypes();
});

function changeSelection(selId) {
	var orderId = 0;
	var paymentId = 0;
	if ($("#orderId").val() == selId) {
		orderId = selId;
	} else {
		paymentId = selId;
	}

	$("body .right-pane .overlay").show();
	$.ajax({
		url: "/Profile/GetBillingAddress?orderId=" + orderId + "&paymentId=" + paymentId,
		dataType: "html"
	}).success(function (result)
	{
	    $("body .right-pane .overlay").hide();
		$("#dynamicArea").html(result);

		refreshCountries();
		populateCardTypes();

		reparseElementValidators("form");
	}).error(function (result)
	{
	    $("body .right-pane .overlay").hide();
		notifyError();
	});
}