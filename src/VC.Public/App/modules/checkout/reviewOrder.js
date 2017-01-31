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
        self.blockCalculation = false;

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
                        initCart();

                        self.loaded(true);
                        self.refreshing(false);
                    } else
                    {
                        processErrorResponse(result, self);
                        self.Model = ko.mapping.fromJS(result.Data);
                        initCart();

                        self.loaded(true);
                        self.refreshing(false);
                    }
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
                setTimeout(function ()
                {
                    reparseElementValidators("#mainForm");
                }, 200);
            }
        };

        self.removeGc = function (item)
        {
            if (self.Model.GiftCertificateCodes().length > 0)
            {
                self.Model.GiftCertificateCodes.remove(item);
            }
        };

        self.addGc = function ()
        {
            var newGc = { Value: ko.observable('') };
            self.Model.GiftCertificateCodes.push(newGc);
        };

        self.applyToAll = function ()
        {
            self.blockCalculation = true;
            $.each(self.Model.Shipments(), function (i, item)
            {
                item.OrderModel.DiscountCode(self.Model.DiscountCode());
            });
            applyGCs(function ()
            {
                self.blockCalculation = false;
                recalculateCart(self);
            });
        };

        var applyGCs = function (success)
        {
            var codes = [];
            $.each(self.Model.GiftCertificateCodes(), function (i, item)
            {
                if (item.Value())
                {
                    codes.push(item.Value());
                }
            });

            self.refreshing(true);
            $.ajax({
                url: "/GC/GetGCsInfo",
                dataType: "json",
                data: ko.toJSON(codes),
                contentType: "application/json; charset=utf-8",
                type: "POST"
            }).success(function (result)
            {
                if (result.Success)
                {
                    var gcs = result.Data;
                    var orderTotals = [];

                    $.each(self.Model.Shipments(), function (i, item)
                    {
                        var orderTotal =
                            {
                                OrderModel: item.OrderModel,
                                TotalWithoutGC: item.OrderModel.OrderTotal() - item.OrderModel.GiftCertificatesTotal()
                            };
                        orderTotals.push(orderTotal);
                        item.OrderModel.GiftCertificateCodes.removeAll();
                    });

                    $.each(gcs, function (i, gc)
                    {
                        if (gc.Amount > 0)
                        {
                            var added = false;
                            $.each(orderTotals, function (j, orderTotal)
                            {
                                if (orderTotal.TotalWithoutGC > 0 && gc.Amount > 0)
                                {
                                    var usedAmount = orderTotal.TotalWithoutGC > gc.Amount ? gc.Amount : orderTotal.TotalWithoutGC;
                                    orderTotal.TotalWithoutGC = orderTotal.TotalWithoutGC - usedAmount;
                                    gc.Amount = gc.Amount - usedAmount;
                                    var newGc = { Value: ko.observable(gc.Code) };
                                    orderTotal.OrderModel.GiftCertificateCodes.push(newGc);
                                    added = true;
                                }
                            });

                            if (!added)
                            {
                                $.each(orderTotals, function (j, orderTotal)
                                {
                                    var newGc = { Value: ko.observable(gc.Code) };
                                    orderTotal.OrderModel.GiftCertificateCodes.push(newGc);
                                });
                            }
                        }
                    });

                    $.each(orderTotals, function (j, orderTotal)
                    {
                        if (orderTotal.OrderModel.GiftCertificateCodes().length == 0)
                        {
                            var newGc = { Value: ko.observable('') };
                            orderTotal.OrderModel.GiftCertificateCodes.push(newGc);
                        }
                    });

                    self.refreshing(false);
                    success();
                }
                else
                {
                    self.refreshing(false);
                }
            }).error(function (result)
            {
                processErrorResponse();
                self.refreshing(false);
            });
        };

        self.move = function (item, e)
        {
            var context = ko.contextFor(e.target);
            var fromIndex = context.$parentContext.$index();
            var skus = context.$parent.OrderModel.Skus;
            $.ajax({
                url: "/Checkout/MoveSku",
                dataType: "html"
            }).success(function (content)
            {
                $(content).dialog({
                    resizable: false,
                    modal: true,
                    minWidth: 515,
                    open: function ()
                    {
                        var options = getOrderOptions(fromIndex);
                        $('#dQTY').val(1);
                        $('#dddOrder').append(options);
                        reparseElementValidators("#popupForm");
                    },
                    close: function ()
                    {
                        $(this).dialog('destroy').remove();
                    },
                    buttons: [
                        {
                            text: "Select",
                            'class': "main-dialog-button",
                            click: function ()
                            {
                                var validator = $("#popupForm").validate();

                                if ($("#popupForm").valid())
                                {
                                    var toIndex = parseInt($('#dddOrder').val());
                                    var qty = parseInt($('#dQTY').val());
                                    if (toIndex == fromIndex)
                                    {
                                        item.Quantity(qty);
                                        $(this).dialog("close");
                                        return;
                                    }

                                    self.blockCalculation = true;
                                    if (item.Quantity() > qty)
                                    {
                                        item.Quantity(item.Quantity() - qty);
                                    }
                                    else
                                    {
                                        skus.remove(item);
                                    }
                                    var newItem = ko.mapping.fromJS(ko.toJS(item));
                                    newItem.Quantity(qty);
                                    var add=true;
                                    $.each(self.Model.Shipments()[toIndex].OrderModel.Skus(), function (i, sku)
                                    {
                                        if (sku.Code() == newItem.Code())
                                        {
                                            add = false;
                                            sku.Quantity(sku.Quantity() + newItem.Quantity());
                                            return false;
                                        }
                                    });
                                    if (add)
                                    {
                                        self.Model.Shipments()[toIndex].OrderModel.Skus.push(newItem);
                                    }
                                    self.blockCalculation = false;

                                    $(this).dialog("close");
                                    recalculateCart(self);
                                }
                            }
                        },
                        {
                            text: "Cancel",
                            click: function ()
                            {
                                $(this).dialog("close");
                            }
                        }
                    ]
                });
            });
        };

        var getOrderOptions = function (fromIndex)
        {
            var options = "";
            $.each(self.Model.Shipments(), function (i, item)
            {
                if (i != fromIndex)
                {
                    options += '<option value="{0}">{1} - {2} {3} {4}</option>'.format(i,
                        item.Name(), item.ShipToFirstName(), item.ShipToLastName(), item.ShipToAddress1());
                }
            });
            return options;
        };

        self.copy = function (item, e)
        {
            var context = ko.contextFor(e.target);
            var fromIndex = context.$parentContext.$index();
            var skus = context.$parent.OrderModel.Skus;
            $.ajax({
                url: "/Checkout/CopySku",
                dataType: "html"
            }).success(function (content)
            {
                $(content).dialog({
                    resizable: false,
                    modal: true,
                    minWidth: 515,
                    open: function ()
                    {
                        var options = getOrderOptions(fromIndex);
                        $('#dddOrder').append(options);
                        reparseElementValidators("#popupForm");
                    },
                    close: function ()
                    {
                        $(this).dialog('destroy').remove();
                    },
                    buttons: [
                        {
                            text: "Select",
                            'class': "main-dialog-button",
                            click: function ()
                            {
                                var validator = $("#popupForm").validate();

                                if ($("#popupForm").valid())
                                {
                                    var toIndex = parseInt($('#dddOrder').val());
                                    if (toIndex == fromIndex)
                                    {
                                        $(this).dialog("close");
                                        return;
                                    }

                                    self.blockCalculation = true;
                                    var newItem = ko.mapping.fromJS(ko.toJS(item));
                                    var add=true;
                                    $.each(self.Model.Shipments()[toIndex].OrderModel.Skus(), function (i, sku)
                                    {
                                        if (sku.Code() == newItem.Code())
                                        {
                                            add = false;
                                            sku.Quantity(sku.Quantity() + newItem.Quantity());
                                            return false;
                                        }
                                    });
                                    if (add)
                                    {
                                        self.Model.Shipments()[toIndex].OrderModel.Skus.push(newItem);
                                    }
                                    self.blockCalculation = false;

                                    $(this).dialog("close");
                                    recalculateCart(self);
                                }
                            }
                        },
                        {
                            text: "Cancel",
                            click: function ()
                            {
                                $(this).dialog("close");
                            }
                        }
                    ]
                });
            });
        };

        self.save = function ()
        {
            recalculateCart(self, function ()
            {
                var atLeastOneProduct = false;
                $.each(self.Model.Shipments(), function (i, shipment)
                {
                    if (shipment.OrderModel.Skus().length == 0)
                    {
                        atLeastOneProduct = true;
                        return false;
                    }
                });
                if (atLeastOneProduct)
                {
                    notifyError('Each order should contain at least one product');
                    return;
                }

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

        var initCart = function ()
        {
            if (self.Model)
            {
                $.each(self.Model.Shipments(), function (i, item)
                {
                    item.upgradeOptions = ko.observableArray([]);
                    if (item.OrderModel.ShippingDate() !== null && item.OrderModel.ShippingDate() !== undefined)
                    {
                        item.OrderModel.ShippingDate = ko.observable(moment(item.OrderModel.ShippingDate()).format('L'));
                    }
                    else
                    {
                        item.OrderModel.ShippingDate = ko.observable("");
                    }

                    item.shipAsapChanged = function ()
                    {
                        if (item.OrderModel.ShipAsap() && self.loaded())
                        {
                            var index = self.Model.Shipments().indexOf(item);
                            $(".item[data-index='" + index + "'] .date-picker").datepicker('setDate', null);
                            item.OrderModel.ShippingDate(null);
                        }
                    };

                    item.removeSku = function (sku)
                    {
                        item.OrderModel.Skus.remove(sku)
                    };

                    item.orderRemoveGc = function (gc)
                    {
                        if (item.OrderModel.GiftCertificateCodes().length > 0)
                        {
                            item.OrderModel.GiftCertificateCodes.remove(gc);
                        }
                    };

                    item.orderAddGc = function ()
                    {
                        var newGc = { Value: ko.observable('') };
                        item.OrderModel.GiftCertificateCodes.push(newGc);
                    };

                    item.orderGcLostFocus = function ()
                    {
                        recalculateCart(self);
                    };
                });


                ko.computed(function ()
                {
                    return ko.toJSON(self.Model.Shipments);
                }).subscribe(function ()
                {
                    recalculateCart(self);
                });

                setTimeout(function ()
                {
                    $(".date-picker-async").datepicker();
                    $(".date-picker-async").datepicker("option", "minDate", 1);
                    reparseElementValidators("#mainForm");
                    recalculateCart(self);
                }, 200);
            }
        };

        load();
    }

    $('#mainForm').removeClass('hide');
    var viewModel = new ReviewModel();
    ko.applyBindings(viewModel);
});

var ajax_request;
function recalculateCart(viewModel, successCallback)
{
    if (viewModel.blockCalculation)
    {
        return;
    }

    if (viewModel.refreshing())
    {
        return;
    }

    var validator = $("#mainForm").validate();

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
                    viewModel.blockCalculation = true;
                    $.each(result.Data.Shipments, function (i, item)
                    {
                        if (viewModel.Model.Shipments().length > i)
                        {
                            var uiShipment = viewModel.Model.Shipments()[i];
                            ko.mapping.fromJS(item.OrderModel, { 'ignore': ["ShipAsap", "DiscountCode", "ShippingDate"] }, uiShipment.OrderModel);
                            processServerMessages(viewModel.Model.Shipments()[i].OrderModel);
                        }
                    });
                    viewModel.blockCalculation = false;
                    viewModel.refreshing(false);
                }
            } else
            {
                if (result.Data)
                {
                    viewModel.blockCalculation = true;
                    $.each(result.Data.Shipments, function (i, item)
                    {
                        if (viewModel.Model.Shipments().length > i)
                        {
                            var uiShipment = viewModel.Model.Shipments()[i];
                            ko.mapping.fromJS(item.OrderModel, { 'ignore': ["ShipAsap", "DiscountCode", "ShippingDate"] }, uiShipment.OrderModel);
                            processServerMessages(viewModel.Model.Shipments()[i].OrderModel);
                        }
                    });
                    viewModel.blockCalculation = false;
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

function processServerMessages(model)
{
    if (!model.originalGiftCertificates)
    {
        model.originalGiftCertificates = [];
    }
    if (typeof model.originalDiscountDescription === 'undefined')
    {
        model.originalDiscountDescription = null;
    }

    if (model.DiscountDescription() && isDiscountValid(model) && model.originalDiscountDescription != model.DiscountDescription())
    {
        notifySuccess(model.DiscountDescription() + "<br/>was applied to your order");
    }
    model.originalDiscountDescription = model.DiscountDescription();

    var actualSuccessMessages = $.grep(model.GiftCertificateCodes(), function (elem, index)
    {
        if (elem.SuccessMessage() && (model.originalGiftCertificates.length != model.originalGiftCertificates.length || elem.SuccessMessage() != model.originalGiftCertificates[index]))
        {
            notifySuccess(elem.SuccessMessage());
        }
    });

    model.originalGiftCertificates = [];
    $.each(model.GiftCertificateCodes(), function (ind, element)
    {
        model.originalGiftCertificates.push(element.SuccessMessage());
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
