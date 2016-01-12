var lhnButtonN = 3881;

$(function () {
	$("body").on("click", "#btnAddGc", function () {
		var count = $("#divGcContainer input[type=text]").length;

		$("#divGcContainer").append('<br/><label></label><input class="cart-input-box input-control" name="GiftCertificateCodes[' + count + ']" type="text"/><a class="btnRemoveGc circle-button button-red"><i class="glyphicon glyphicon-minus-sign"></i></a>')
	});

	$("body").on("click", ".btnRemoveGc", function () {
		var jInput = $(this).prev();
		var jLabel = jInput.prev();
		var jBr = jLabel.prev();

		jInput.remove();
		jLabel.remove();
		jBr.remove();
		$(this).remove();
	});
});