var Shipments;

$(function ()
{
    Shipments = function (data)
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
                    url: "/Checkout/GetShipments",
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
                        $.each(self.Model.Shipments(), function (i, item)
                        {
                            item.Collapsed = ko.observable(!item.FromOrder());
                            if (item.FromOrder())
                            {
                            }

                            setShipmentHandlers(item);
                        });
                        updateAddButtonText();

                        checkCanadaIssue();
                        self.loaded(true);
                    } else
                    {
                        processErrorResponse(result);
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

        var setShipmentHandlers = function (shipment)
        {
            shipment.SelectedAvalibleAddress = ko.observable(null);
            shipment.SelectedAvalibleAddress.subscribe(function (newValue)
            {
                //TODO: handle changing
            });

            shipment.SelectedCountry = ko.observable(null);
            setSelectCountry(shipment);
            shipment.IdCountry.subscribe(function (newValue)
            {
                setSelectCountry(shipment);
            });
        };

        var setSelectCountry = function (shipment)
        {
            $.each(self.options.Countries, function (i, item)
            {
                if (item.Id == shipment.IdCountry())
                {
                    shipment.SelectedCountry(item);
                }
            });
        };

        self.remove = function (item, event)
        {
            if (confirm('Are you sure you want to delete this order?'))
            {
                self.Model.Shipments.remove(item);
            }
        };

        self.add = function ()
        {
            
        };

        self.collapse = function (item, event)
        {
            item.Collapsed(true);
        };

        self.expand = function (item, event)
        {
            $.each(self.Model.Shipments(), function(i, item){
                item.Collapsed(true);
            });
            item.Collapsed(false);
        };

        self.save = function ()
        {
            reparseElementValidators("#mainForm");
            var validator = $("#mainForm").validate();

            if ($("#mainForm").valid()) {
                self.refreshing(true);

                ajaxRequest = $.ajax({
                    url: "/Checkout/UpdateShipments",
                    dataType: "json",
                    data: ko.toJSON(self.Model),
                    contentType: "application/json; charset=utf-8",
                    type: "POST"
                }).success(function (result) {
                    $("span[data-valmsg-for]").text('');
                    $("div.validation-summary-errors").html('');
                    if (result.Success) {
                        processJsonCommands(result);
                    } else {
                        processErrorResponse(result);
                        self.refreshing(false);
                    }
                }).error(function (result) {
                    $("div.validation-summary-errors").html('');
                    $("span[data-valmsg-for]").text('');
                    processErrorResponse();
                    self.refreshing(false);
                });
            }
        };

        var updateAddButtonText = function(){
            if (self.Model.Shipments().length > 1)
            {
                self.options.AddButtonText('Add Another Recipient');
            }
            else
            {
                self.options.AddButtonText('Send Order To Multiple Receipents');
            }
        };

        function checkCanadaIssue()
        {
            var param = getQueryParameterByName("canadaissue");
            if (param)
            {
                self.refreshing(true);
                $.ajax({
                    url: "/Checkout/CanadaShippingIssueView",
                    dataType: "html",
                    type: "GET"
                }).success(function (result, textStatus, xhr)
                {
                    $(result).dialog({
                        resizable: false,
                        modal: true,
                        minWidth: 450,
                        dialogClass: "canada-shipping-notice",
                        open: function (event, ui)
                        {
                            $(this).parent().focus();
                        },
                        close: function ()
                        {
                            $(this).dialog('destroy').remove();
                        },
                        buttons: [
                            {
                                text: "Ok",
                                click: function ()
                                {
                                    $(this).dialog("close");
                                }
                            }
                        ]
                    });
                }).error(function (result)
                {
                    notifyError();
                }).complete(function ()
                {
                    self.refreshing(false);
                });
            }
        };

        load();
    }

    $('#mainForm').removeClass('hide');
    var viewModel = new Shipments();
    ko.applyBindings(viewModel);

    //controlSectionState("#ddShippingAddressesSelection", "#chkSelectOther");
    //controlSectionState("#GiftMessageBox", "#IsGiftOrder");
    //controlUseBillingState(".form-two-column", "#UseBillingAddress");

    //$("body")
    //    .on("click",
    //        "#chkSelectOther",
    //        function () { controlSectionState("#ddShippingAddressesSelection", "#chkSelectOther"); });
    //$("body")
    //    .on("click",
    //        "#IsGiftOrder",
    //        function () {
    //            controlSectionState("#GiftMessageBox", "#IsGiftOrder");
    //            controlSectionContent("#GiftMessage", "#IsGiftOrder");
    //        });
    //$("body")
    //    .on("click",
    //        "#UseBillingAddress",
    //        function () { controlUseBillingState(".form-two-column", "#UseBillingAddress"); });

    //$("body")
    //    .on("change",
    //        "#ddShippingAddressesSelection",
    //        function () {
    //            changeSelection($("#ddShippingAddressesSelection").val());
    //        });

    //$(".columns-container form").data("validator").settings.submitHandler = function (form)
    //{
    //    $(".columns-container .overlay").show();
    //    form.submit();
    //};

    //controlUpdateSavedState();
});

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

//function controlUpdateSavedState() {
//    if ($("#ShipAddressIdToOverride").val() == null || $("#ShipAddressIdToOverride").val() == "") {
//        $("#SaveToProfile").prop("checked", false);
//        $("#SaveToProfile").trigger("change");
//        $("#updateSaved").hide();
//    } else {
//        $("#updateSaved").show();
//    }
//}

//function changeSelection(selId) {
//    $.ajax({
//        url: "/Checkout/GetShippingAddress/" + selId,
//        dataType: "html"
//    }).success(function (result) {
//        $("#dynamicArea").html(result);

//        refreshCountries();

//        controlUpdateSavedState();

//        $(".phone-mask").mask("(999) 999-9999? x99999");

//        reparseElementValidators("form");
//    }).error(function (result) {
//        notifyError();
//    });
//}

//function controlSectionContent(selector, controlId) {
//    var jSel = $(selector);

//    if (!$(controlId).is(":checked")) {
//        jSel.val("");
//        jSel.trigger("change");
//        jSel.trigger("keyup");
//    }
//}

//function controlSectionState(selector, controlId) {
//    var jSel = $(selector).closest(".form-group");

//    if ($(controlId).is(":checked")) {
//        jSel.show();
//    } else {
//        jSel.hide();
//    }
//}

//var lastPrefShipMethodLabel = null;

//function controlUseBillingState(selector, controlId) {
//    var jSel = $(selector);
//    var jDrop = $("#ddShippingAddressesSelection").closest(".form-group");

//    var jChk = $("#SaveToProfile");
//    var jChkContainer = $("#updateSaved");

//    var jSpan = $("#spEnterAddress");
//    var jPrefShipMethod = $("#PreferredShipMethodLabel");

//    if ($(controlId).is(":checked")) {
//        jSel.hide();
//        jDrop.hide();
//        jChk.prop("checked", false);
//        jChk.trigger("change");
//        jChkContainer.hide();
//        jSpan.hide();
//        lastPrefShipMethodLabel = jPrefShipMethod.text();
//        jPrefShipMethod.text("Best");
//    } else {
//        jSel.show();
//        jDrop.show();
//        jChkContainer.show();
//        jSpan.show();
//        if (lastPrefShipMethodLabel !== null) {
//            jPrefShipMethod.text(lastPrefShipMethodLabel);
//        }
//        //controlSectionState("#ddShippingAddressesSelection", "#chkSelectOther");
//    }
//}
