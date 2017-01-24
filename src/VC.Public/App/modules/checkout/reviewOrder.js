var ReviewModel;

$(function ()
{
    ReviewModel = function (data)
    {
        var self = this;

        self.options = {};
        self.options.AddButtonText = ko.observable('');
        self.options.Countries = [];

        self.refreshing = ko.observable(true);
        self.loaded = ko.observable(false);

        var load = function ()
        {
            getCountries(undefined, function (resultCountries)
            {
                self.options.Countries = resultCountries.Data;
                $.ajax({
                    url: "/Checkout/OrderReviewInitCartModel",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    type: "GET"
                }).success(function (result)
                {
                    $("span[data-valmsg-for]").text('');
                    $("div.validation-summary-errors").html('');
                    if (result.Success)
                    {
                        self.Model = ko.mapping.fromJS(result.Data);
                        setRecalculateCartHandler();

                        self.loaded(true);
                    } else
                    {
                        processErrorResponse(result, self);
                        self.loaded(false);
                    }
                    self.refreshing(false);
                }).error(function (result)
                {
                    $("span[data-valmsg-for]").text('');
                    processErrorResponse();
                    self.refreshing(false);
                });
            }, function () { });
        };

        self.remove = function (item, event)
        {
            if (confirm('Are you sure you want to delete this order?'))
            {
                self.Model.Shipments.remove(item);
            }
        };

        self.save = function ()
        {
            recalculateCart(self, function ()
            {
                self.refreshing(true);

                $.ajax({
                    url: "/Checkout/ReviewOrder",
                    dataType: "json",
                    data: ko.toJSON(self.Model),
                    contentType: "application/json; charset=utf-8",
                    type: "POST"
                }).success(function (result)
                {
                    if (result.Success)
                    {
                        processJsonCommands(result);
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

        var setRecalculateCartHandler = function ()
        {
            ko.computed(function ()
            {
                return ko.toJSON(self.Model);
            }).subscribe(function ()
            {
                //recalculateCart(self);
            });
        };

        load();
    }

    $('#mainForm').removeClass('hide');
    var viewModel = new ReviewModel();
    ko.applyBindings(viewModel);

    $(".date-picker").datepicker("option", "minDate", 1);

	//$("body").on("click", "#btnPlaceOrder", function() {
	//	$("#viewCartForm").submit();
	//})

	//var updateUIVisibility = function ()
	//{
	//    if ($('.item').length >1)
	//    {
	//        $('.top-billing-wrapper').show();
	//        $('.item .reivew-info-box.billing').hide();
	//        $('.item .cart-promo-container').hide();
	//        $('.item .cart-gc-container').hide();
	//    }
	//    else
	//    {
	//        $('.top-billing-wrapper').hide();
	//        $('.item .reivew-info-box.billing').show();
	//        $('.item .cart-promo-container').show();
	//        $('.item .cart-gc-container').show();
	//    }

	//    $('.item').removeClass('alternate-color');
	//    $.each($('.item'), function (i, item)
	//    {
	//        if (i % 2 == 0)
	//        {
	//            $(item).addClass('alternate-color');
	//        }
	//    });
	//};
	//updateUIVisibility();

	//$("body").on("click", ".item .delete-order", function ()
	//{
	//    $(this).closest('.item').remove();
	//    updateUIVisibility();
	//});

	//var items = $('td.cart-line-info');
	//$.each(items, function (index, item)
	//{
	//    var buttons = $('.assign-buttons.template').clone();
	//    buttons.removeClass('hide');
	//    buttons.removeClass('template');
	//    $(item).append(buttons);
	//});

	//var popupCopyContent = '<div title="Select order"><form class="form-regular small"><div class="form-group"><label class="control-label">Orders</label><div class="input-group"><select class="form-control big" id="ddOrder">' +
    //    '<option value="1">Order #1 - Gary1 Gould 806 Front ST</option>' +
    //    '<option value="2">Order #2 - Gary2 Gould 806 Front ST</option>' +
    //    '<option value="4">Order #4 - Gary4 Gould 806 Front ST</option>' +
    //    '<option value="5">Order #5 - Gary5 Gould 806 Front ST</option></select></div></div></form></div>';

	//var popupMoveContent = '<div title="Select order"><form class="form-regular small"><div class="form-group"><label class="control-label">Orders</label><div class="input-group"><select class="form-control big" id="ddOrder">' +
    //    '<option value="1">Order #1 - Gary1 Gould 806 Front ST</option>' +
    //    '<option value="2">Order #2 - Gary2 Gould 806 Front ST</option>' +
    //    '<option value="4">Order #4 - Gary4 Gould 806 Front ST</option>' +
    //    '<option value="5">Order #5 - Gary5 Gould 806 Front ST</option></select></div></div>' +
    //    '<div class="form-group"><label class="control-label">QTY</label><div class="input-group"><div class="input-group"><input class="form-control small-form-control" type="text" id="qty" value="1"></div></div>' +
    //    '</form></div>';

	//$("body").on("click", ".assign-buttons .button-blue", function ()
	//{
	//    var row = $(this).closest('tr');
	//    openAssignPopup(function (id)
	//    {
	//        row.remove();
	//        var targetItem = $('.item[data-id=' + id + ']');
	//        targetItem.find('tbody').eq(1).append(row);
	//    }, popupMoveContent);
	//});

	//$("body").on("click", ".assign-buttons .button-green", function ()
	//{
	//    var row = $(this).closest('tr');
	//    openAssignPopup(function (id)
	//    {
	//        row = row.clone();
	//        var targetItem = $('.item[data-id=' + id + ']');
	//        targetItem.find('tbody').eq(1).append(row);
	//    }, popupCopyContent);
	//});

	//var openAssignPopup = function (successCallback, content)
	//{
	//    $(content).dialog({
	//        resizable: false,
	//        modal: true,
	//        minWidth: 515,
	//        open: function ()
	//        {
	//        },
	//        close: function ()
	//        {
	//            $(this).dialog('destroy').remove();
	//        },
	//        buttons: [
    //            {
    //                text: "Select",
    //                'class': "main-dialog-button",
    //                click: function ()
    //                {
    //                    successCallback($('#ddOrder').val());
    //                    $(this).dialog("close");
    //                }
    //            },
    //            {
    //                text: "Cancel",
    //                click: function ()
    //                {
    //                    $(this).dialog("close");
    //                }
    //            }
	//        ]
	//    });
	//};
});

var ajax_request;
function recalculateCart(viewModel, successCallback)
{
    if (viewModel.refreshing())
    {
        return;
    }

    $("#mainForm").validate();

    if ($("#mainForm").valid())
    {
        viewModel.refreshing(true);

        if (typeof ajaxRequest !== 'undefined')
        {
            ajaxRequest.abort();
        }
        ajaxRequest = $.ajax({
            url: "/Checkout/OrderReviewUpdateCart",
            dataType: "json",
            data: ko.toJSON(viewModel.Model),
            contentType: "application/json; charset=utf-8",
            type: "POST"
        }).success(function (result)
        {
            $("span[data-valmsg-for]").text('');
            if (result.Success)
            {
                $("div.validation-summary-errors").html('');
                if (successCallback)
                {
                    viewModel.refreshing(false);
                    successCallback();
                } else
                {
                    $.each(result.Data.Shipments, function (i, item)
                    {
                        if (viewModel.Model.Shipments().length > i)
                        {
                            var uiShipment = viewModel.Model.Shipments()[i];
                            ko.mapping.fromJS(item.OrderModel, { 'ignore': ["ShipAsap", "DiscountCode", "ShippingDate"] }, uiShipment.OrderModel);
                            processServerMessages(viewModel.Model);
                        }
                    });
                    viewModel.refreshing(false);
                }
            } else
            {
                if (result.Data)
                {
                    $.each(result.Data.Shipments, function (i, item)
                    {
                        if (viewModel.Model.Shipments().length > i)
                        {
                            var uiShipment = viewModel.Model.Shipments()[i];
                            ko.mapping.fromJS(item.OrderModel, { 'ignore': ["ShipAsap", "DiscountCode", "ShippingDate"] }, uiShipment.OrderModel);
                            processServerMessages(viewModel.Model);
                        }
                    });
                }
                processErrorResponse(result);
                viewModel.refreshing(false);
            }
        }).error(function (result)
        {
            $("div.validation-summary-errors").html('');
            $("span[data-valmsg-for]").text('');
            processErrorResponse();
            viewModel.refreshing(false);
        });
    }
}

function formatCurrency(value)
{
    if (!value)
    {
        value = 0;
    }

    if (value >= 0)
    {
        return "${0}".format(value.toFixed(2));
    }
    else
    {
        return "(${0})".format(Math.abs(value).toFixed(2));
    }
}

function formatDisplayName(sku)
{
    var displayName = sku.DisplayName();
    if (sku.SubTitle())
    {
        displayName += " " + sku.SubTitle();
    }

    displayName += " (" + sku.PortionsCount() + ")";

    return displayName;
}

var originalDiscountDescription;

var originalGiftCertificates = [];

function processServerMessages(model)
{
    if (model.DiscountDescription() && isDiscountValid(model) && originalDiscountDescription != model.DiscountDescription())
    {
        notifySuccess(model.DiscountDescription() + "<br/>was applied to your order");
    }
    originalDiscountDescription = model.DiscountDescription();

    var actualSuccessMessages = $.grep(model.GiftCertificateCodes(), function (elem, index)
    {
        if (elem.SuccessMessage() && (originalGiftCertificates.length != originalGiftCertificates.length || elem.SuccessMessage() != originalGiftCertificates[index]))
        {
            notifySuccess(elem.SuccessMessage());
        }
    });

    originalGiftCertificates = [];
    $.each(model.GiftCertificateCodes(), function (ind, element)
    {
        originalGiftCertificates.push(element.SuccessMessage());
    });
}

function isDiscountValid(model)
{
    var discountRelated = getDiscountRelatedErrors(model);

    return !discountRelated || discountRelated.length == 0;
}

function getDiscountRelatedErrors(model)
{
    if (!model.Messages())
    {
        return null;
    }

    return $.grep(model.Messages(), function (element)
    {
        return element.Key = "DiscountCode";
    });
}

function isGcValid(model, index)
{
    if (model.GiftCertificateCodes() && model.GiftCertificateCodes().length > index)
    {
        var target = model.GiftCertificateCodes()[index];
        if (target.ErrorMessage && target.ErrorMessage())
        {
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
            processJsonCommands(result);
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
