var lhnButtonN = 3881;

$(function () {
	$("body").on("click", "#btnAddGc", function () {
		var count = $("#divGcContainer input[type=text]").length;

		$("#divGcContainer").append('<br/><label></label><input class="cart-input-box input-control" name="GiftCertificateCodes[' + count + ']" type="text"/><input type="button" class="minus-button-red btnRemoveGc"/>')
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