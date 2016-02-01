var Cart;

$(function () {
	Cart = function (data) {
		var self = this;

		self.Model = ko.mapping.fromJS(data, { 'ignore': ["ShippingDate"] });
		if (data.ShippingDate !== null && data.ShippingDate !== undefined) {
		    self.Model.ShippingDate = ko.observable(moment(data.ShippingDate).format('L'));
		}
		else {
		    self.Model.ShippingDate = ko.observable("");
		}
		self.upgradeOptions = ko.observableArray([]);
		self.refreshing = true;
		self.initializing = true;

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
			if (self.Model.ShipAsap() && !self.initializing) {
				$(".date-picker").datepicker('setDate', null);
				self.Model.ShippingDate(null);
			}
		};
		self.gcLostFocus = function() {
			recalculateCart(self);
		};
		self.submitCart = function() {
			$("#viewCartForm").validate();

			recalculateCart(self, function () {
				window.location.href = "/Checkout/Welcome";
			});
		};

		ko.computed(function () {
			return ko.toJSON(self.Model);
		}).subscribe(function () {
			recalculateCart(self);
		});
	}

	initCart();

	$(".date-picker").datepicker("option", "minDate", 1);
});

function formatCurrency(value) {
	if (!value) {
		value = 0;
	}

	if (value >= 0) {
		return "$" + value.toFixed(2);
	}
	else {
		return "-$" + Math.abs(value).toFixed(2);
	}
}

function formatDisplayName(sku) {
	var displayName = sku.DisplayName();
	if (sku.SubTitle()) {
		displayName += " " + sku.SubTitle();
	}

	displayName += " (" + sku.PortionsCount() + ")";

	return displayName;
}

function recalculateCart(viewModel, successCallback) {
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
				if (successCallback) {
					successCallback();
				} else {
					ko.mapping.fromJS(result.Data, { 'ignore': ["ShipAsap", "PromoCode", "ShippingDate"] }, viewModel.Model);
					processServerMessages(viewModel.Model);
				}
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

function initCart() {
	var viewModel;

	$.ajax({
		url: "/Cart/InitCartModel",
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		type: "GET"
	}).success(function (result) {
		if (result.Success) {
			viewModel = new Cart(result.Data);

			processServerMessages(viewModel.Model);
		} else {
			notifyError(result.Messages[0]);
		}

		ko.applyBindings(viewModel);

		reparseElementValidators("form#viewCartForm");

		viewModel.refreshing = false;
		viewModel.initializing = false;
	}).error(function (result) {
		notifyError();
		viewModel.refreshing = false;
		viewModel.initializing = false;
	});
}

var originalDiscountDescription;

var originalGiftCertificates = [];

function processServerMessages(model) {
	if (model.DiscountDescription() && isDiscountValid(model) && originalDiscountDescription != model.DiscountDescription()) {
		notifySuccess(model.DiscountDescription() + "<br/>was applied to your order");
	}
	originalDiscountDescription = model.DiscountDescription();

	var actualSuccessMessages = $.grep(model.GiftCertificateCodes(), function(elem, index) {
		if (elem.SuccessMessage() && (originalGiftCertificates.length != originalGiftCertificates.length || elem.SuccessMessage() != originalGiftCertificates[index])) {
			notifySuccess(elem.SuccessMessage());
		}
	});

	originalGiftCertificates = [];
	$.each(model.GiftCertificateCodes(), function (ind, element) {
		originalGiftCertificates.push(element.SuccessMessage());
	});
}

function isDiscountValid(model) {
	var discountRelated = getDiscountRelatedErrors(model);

	return !discountRelated || discountRelated.length == 0;
}

function getDiscountRelatedErrors(model) {
	if (!model.Messages()) {
		return null;
	}

	return $.grep(model.Messages(), function(element) {
		return element.Key = "DiscountCode";
	});
}

function isGcValid(model, index) {
	if (model.GiftCertificateCodes() && model.GiftCertificateCodes().length > index) {
		var target = model.GiftCertificateCodes()[index];
		if (target.ErrorMessage && target.ErrorMessage()) {
			return false;
		}
	}
	return true;
}