$(function () {
	$("body").on("click", "#btnAddGc", function () {
		var count = $("#divGcContainer input[type=text]").length;

		$("#divGcContainer").append('<div><a class="btnRemoveGc circle-button button-red"><i class="glyphicon glyphicon-minus-sign"></i></a><input class="cart-input-box input-control" name="GiftCertificateCodes[' + count + ']" type="text"/></div>')
	});

	$("body").on("click", ".btnRemoveGc", function () {
		var jContainer = $(this).parent();

		jContainer.remove();
	});

	$("#ShippingDate").datepicker();

	var Cart = function (data) {
		var self = this;

		self.Model = ko.mapping.fromJSON(data);
		self.removeSku = function(sku) {
			self.Model.Skus.remove(sku)
		};

		ko.computed(function () {
			return ko.toJSON(self.Model);
		}).subscribe(function () {
			alert("atata");
		});
	}

	var cartViewModel = new Cart(initialData);

	//cartViewModel.Model.Skus.subscribe(function(newValue) {
	//	alert("atata");
	//});

	ko.applyBindings(cartViewModel);

});

function formatCurrency(value) {
	return "$" + value.toFixed(2);
}