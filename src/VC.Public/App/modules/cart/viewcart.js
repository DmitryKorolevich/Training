$(function () {
	$("body").on("click", "#btnAddGc", function () {
		var count = $("#divGcContainer input[type=text]").length;

		$("#divGcContainer").append('<div><a class="btnRemoveGc circle-button button-red"><i class="glyphicon glyphicon-minus-sign"></i></a><input class="cart-input-box input-control" name="GiftCertificateCodes[' + count + ']" type="text"/></div>')
	});

	$("body").on("click", ".btnRemoveGc", function () {
		var jContainer = $(this).parent();

		jContainer.remove();
	});
});