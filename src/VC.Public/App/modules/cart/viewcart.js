$(function () {
	var Cart = function (data) {
		var self = this;

		self.Model = ko.mapping.fromJSON(data);
		self.upgradeOptions = ko.observableArray([]);
		self.refreshing = true;
		self.inititalizing = true;

		self.removeSku = function(sku) {
			self.Model.Skus.remove(sku)
		};
		self.removeGc = function (item) {
			if (self.Model.GiftCertificateCodes().length > 0 ) {
				self.Model.GiftCertificateCodes.remove(item);
			}
		};
		self.addGc = function () {
		    self.refreshing = true;
		    var newGc = { Value: "" };
		    self.Model.GiftCertificateCodes.push(newGc);
			self.refreshing = false;
		};
		self.shipAsapChanged = function() {
			if (self.Model.ShipAsap() && !self.inititalizing) {
				$(".date-picker").datepicker('setDate', null);
				self.Model.ShippingDate(null);
			}
		};
		self.submitCart = function() {
			$("#viewCartForm").validate();

			if ($("#viewCartForm").valid()) {
				viewModel.refreshing = true;

				$.ajax({
					url: "/Cart/ViewCart", //todo: post action has to be added
					dataType: "json",
					data: ko.toJSON(viewModel.Model),
					contentType: "application/json; charset=utf-8",
					type: "POST"
				}).success(function (result) {
					window.location.href = "/Checkout/AddUpdateBillingAddress";
					viewModel.refreshing = false;
				}).error(function (result) {
					notifyError();
					viewModel.refreshing = false;
				});
			}
		};

		ko.computed(function () {
			return ko.toJSON(self.Model);
		}).subscribe(function () {
			recalculateCart(self);
		});
	}

	var cartViewModel = new Cart(initialData);

	ko.applyBindings(cartViewModel);

	getCartShippingOptions(function (result) {
		if (result.Success) {
			$.each(result.Data, function (index, el) {
				cartViewModel.upgradeOptions.push(el);
			});
		} else {
			notifyError(result.Messages[0]);
		}
		cartViewModel.refreshing = false;
		cartViewModel.ininitalizing = false;
	}, function (errorResult) {
		notifyError();
		cartViewModel.refreshing = false;
		cartViewModel.ininitalizing = false;
	});

	reparseElementValidators("form#viewCartForm");
});

function formatCurrency(value) {
	return "$" + value.toFixed(2);
}

function recalculateCart(viewModel) {
	if (viewModel.refreshing) {
		return;
	}

	$("#viewCartForm").validate();

	if ($("#viewCartForm").valid()) {
		viewModel.refreshing = true;

		$.ajax({
			url: "/Cart/UpdateCart",
			dataType: "json",
			data: ko.toJSON(viewModel.Model),
			contentType: "application/json; charset=utf-8",
			type: "POST"
		}).success(function (result) {
			if (result.Success) {
				ko.mapping.fromJS(result.Data, { 'ignore': ["ShipAsap", "GiftCertificateCodes", "PromoCode", "ShippingDate"] }, viewModel.Model);
			} else {
				notifyError(result.Messages[0]);
			}
			viewModel.refreshing = false;
		}).error(function (result) {
			notifyError();
			viewModel.refreshing = false;
		});
	}
}