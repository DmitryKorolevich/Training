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
		self.refreshing = ko.observable(true);
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
			var newGc = { Value: "" };
			self.Model.GiftCertificateCodes.push(newGc);
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
			recalculateCart(self, function () {
				var url = $("#viewCartForm").attr("action");

				self.refreshing(true);

				$.ajax({
					url: url,
					dataType: "json",
					data: ko.toJSON(self.Model),
					contentType: "application/json; charset=utf-8",
					type: "POST"
				}).success(function (result) {
				    if (result.Success)
				    {
				        if (result.Data != '/checkout/receipt')
				        {
				            self.refreshing(false);
				        }
				        window.location.href = result.Data;
					} else
				    {
				        self.refreshing(false);
					    processErrorResponse(result);
					}
				}).error(function (result)
				{
				    processErrorResponse();
					self.refreshing(false);
				});
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
		return "${0}".format(value.toFixed(2));
	}
	else {
		return "(${0})".format(Math.abs(value).toFixed(2));
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

var ajax_request;
function recalculateCart(viewModel, successCallback) {
	if (viewModel.refreshing()) {
		return;
	}

	$("#viewCartForm").validate();

	if ($("#viewCartForm").valid()) {
		viewModel.refreshing(true);

		if (typeof ajaxRequest !== 'undefined') {
			ajaxRequest.abort();
		}
		ajaxRequest = $.ajax({
			url: "/Cart/UpdateCart",
			dataType: "json",
			data: ko.toJSON(viewModel.Model),
			contentType: "application/json; charset=utf-8",
			type: "POST"
		}).success(function (result)
		{
		    if (result.Success) {
		        $("span[data-valmsg-for]").text('');
		        $("div.validation-summary-errors").html('');
		        if (successCallback)
		        {
		            viewModel.refreshing(false);
				    successCallback();
				} else {
		            ko.mapping.fromJS(result.Data, { 'ignore': ["ShipAsap", "DiscountCode", "ShippingDate"] }, viewModel.Model);
					processServerMessages(viewModel.Model);
					viewModel.refreshing(false);
				}
			} else
			{
			    if (result.Data)
			    {
			        ko.mapping.fromJS(result.Data, { 'ignore': ["ShipAsap", "DiscountCode", "ShippingDate"] }, viewModel.Model);
			    }
			    processErrorResponse(result);
			    viewModel.refreshing(false);
			}
		}).error(function (result)
		{
		    processErrorResponse();
			viewModel.refreshing(false);
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
        var binded = false;
        if (result.Success) {
            $("span[data-valmsg-for]").text('');
            $("div.validation-summary-errors").html('');

            viewModel = new Cart(result.Data);

            processServerMessages(viewModel.Model);

            $("body").on("click", ".proposals-item-link", function () { addToCart($(this), viewModel); return false; });
        } else {
            if (result.Data) {
                viewModel = new Cart(result.Data);
                binded = true;
                ko.applyBindings(viewModel);
            }
            
            viewModel.refreshing(false);
            processErrorResponse(result);
        }

        if (!binded) {
            ko.applyBindings(viewModel);
        }

        reparseElementValidators("form#viewCartForm");

        viewModel.refreshing(false);
        viewModel.initializing = false;
    }).error(function (result) {
        processErrorResponse();
        viewModel.refreshing(false);
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

function processErrorResponse(result)
{
    if (result)
    {
        if (result.Command != null)
        {
            if (result.Command == 'redirect' && result.Data)
            {
                window.location = result.Data;
            }
        }
        else
        {
            trySetFormErrors(result);
        }
    }
    else
    {
        notifyError();
    }
}

function addToCart(jElem, viewModel) {
	if (viewModel.refreshing()) {
		return false;
	}

	var sku = jElem.attr("data-sku-code");

	viewModel.refreshing(true);

	$.ajax({
		url: "/Cart/AddToCart?skuCode=" + sku,
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		type: "POST"
	}).success(function (result) {
		if (result.Success) {
		    ko.mapping.fromJS(result.Data, { 'ignore': ["ShipAsap", "DiscountCode", "ShippingDate"] }, viewModel.Model);
				processServerMessages(viewModel.Model);
		} else {
			processErrorResponse(result);
		}
		viewModel.refreshing(false);
	}).error(function (result) {
		processErrorResponse();
		viewModel.refreshing(false);
	});

	return false;
}